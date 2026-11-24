using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRS_API.Extensions;
using TRS_Lib.Dtos;
using TRS_Lib.Models;
using TRS_Lib.Repositories;

namespace TRS_API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Organizations")]
public class OrganizationController(
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository) : ControllerBase
{
    /// <summary>Creates an organization.</summary>
    /// <remarks>The logged in user becomes the organization's owner, already approved.</remarks>
    /// <response code="201">The organization was created.</response>
    /// <response code="400">The request body is invalid.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<OrganizationDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateOrganizationDto dto, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        var organization = await organizationRepository.CreateAsync(new Organization { Name = dto.Name }, userId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new OrganizationDto { Id = organization.Id, Name = organization.Name });
    }

    /// <summary>Lists the organization's members.</summary>
    /// <remarks>
    /// The logged in user must be an approved member. Owners also see pending membership requests;
    /// other members only see approved members.
    /// </remarks>
    /// <param name="id">The organization's id.</param>
    /// <response code="200">The members.</response>
    /// <response code="403">The logged in user is not an approved member of the organization.</response>
    /// <response code="404">The organization does not exist.</response>
    [HttpGet("{id:int}/members")]
    [ProducesResponseType<List<OrganizationMemberDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListMembers(int id, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        if (await organizationRepository.FindAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        var caller = await organizationRepository.FindMemberAsync(id, userId, cancellationToken);
        if (caller is not { Status: MembershipStatus.Approved })
        {
            return Forbid();
        }

        var includeRequested = caller.MemberType == MemberType.Owner;
        return Ok(await organizationRepository.ListMembersAsync(id, includeRequested, cancellationToken));
    }

    /// <summary>Adds a user to the organization, approved directly.</summary>
    /// <remarks>
    /// Only approved owners can add members. If the user has a pending request, it is approved with the given member type.
    /// </remarks>
    /// <param name="id">The organization's id.</param>
    /// <response code="200">The user's pending request was approved.</response>
    /// <response code="201">The user was added.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="403">The logged in user is not an approved owner of the organization.</response>
    /// <response code="404">The organization or the user does not exist.</response>
    /// <response code="409">The user is already an approved member.</response>
    [HttpPost("{id:int}/members")]
    [Consumes("application/json")]
    [ProducesResponseType<OrganizationMemberDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<OrganizationMemberDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddMember(int id, AddOrganizationMemberDto dto, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        if (await organizationRepository.FindAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        if (!await IsApprovedOwnerAsync(id, userId, cancellationToken))
        {
            return Forbid();
        }

        if (await userRepository.FindAsync(dto.UserId, cancellationToken) is null)
        {
            return NotFound();
        }

        var memberType = dto.MemberType!.Value;
        var existing = await organizationRepository.FindMemberAsync(id, dto.UserId, cancellationToken);
        if (existing is { Status: MembershipStatus.Approved })
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The user is already a member of the organization.");
        }

        if (existing is not null)
        {
            await organizationRepository.ApproveMemberAsync(id, dto.UserId, memberType, cancellationToken);
            return Ok(await organizationRepository.FindMemberDtoAsync(id, dto.UserId, cancellationToken));
        }

        await organizationRepository.AddMemberAsync(new OrganizationMember
        {
            OrganizationId = id,
            UserId = dto.UserId,
            Status = MembershipStatus.Approved,
            MemberType = memberType,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, await organizationRepository.FindMemberDtoAsync(id, dto.UserId, cancellationToken));
    }

    /// <summary>Approves a pending membership request.</summary>
    /// <remarks>Only approved owners can approve requests. The status changes from "Requested" to "Approved"; the member type is kept.</remarks>
    /// <param name="id">The organization's id.</param>
    /// <param name="userId">Id of the user whose request to approve.</param>
    /// <response code="200">The request was approved.</response>
    /// <response code="403">The logged in user is not an approved owner of the organization.</response>
    /// <response code="404">The organization does not exist, or the user has not requested membership.</response>
    /// <response code="409">The user is already an approved member.</response>
    [HttpPost("{id:int}/members/{userId:int}/approve")]
    [ProducesResponseType<OrganizationMemberDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveRequest(int id, int userId, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } callerId)
        {
            return Unauthorized();
        }

        if (await organizationRepository.FindAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        if (!await IsApprovedOwnerAsync(id, callerId, cancellationToken))
        {
            return Forbid();
        }

        if (!await organizationRepository.ApproveRequestAsync(id, userId, cancellationToken))
        {
            return await organizationRepository.FindMemberAsync(id, userId, cancellationToken) is null
                ? NotFound()
                : Problem(statusCode: StatusCodes.Status409Conflict, title: "The user is already an approved member.");
        }

        return Ok(await organizationRepository.FindMemberDtoAsync(id, userId, cancellationToken));
    }

    /// <summary>Removes a user from the organization.</summary>
    /// <remarks>
    /// Approved owners can remove anyone. Any user can remove themselves, which also cancels a pending request.
    /// The organization's last owner cannot be removed.
    /// </remarks>
    /// <param name="id">The organization's id.</param>
    /// <param name="userId">Id of the user to remove.</param>
    /// <response code="204">The user was removed.</response>
    /// <response code="403">The logged in user may not remove that user.</response>
    /// <response code="404">The organization does not exist, or the user is not a member of it.</response>
    /// <response code="409">The user is the organization's last owner.</response>
    [HttpDelete("{id:int}/members/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveMember(int id, int userId, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } callerId)
        {
            return Unauthorized();
        }

        if (await organizationRepository.FindAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        if (callerId != userId && !await IsApprovedOwnerAsync(id, callerId, cancellationToken))
        {
            return Forbid();
        }

        var member = await organizationRepository.FindMemberAsync(id, userId, cancellationToken);
        if (member is null)
        {
            return NotFound();
        }

        if (member is { Status: MembershipStatus.Approved, MemberType: MemberType.Owner }
            && await organizationRepository.CountApprovedOwnersAsync(id, cancellationToken) <= 1)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The organization's last owner cannot be removed.");
        }

        await organizationRepository.RemoveMemberAsync(id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>Requests membership in the organization for the logged in user.</summary>
    /// <remarks>The request gets status "Requested" and member type "Member". An owner approves it with the approve endpoint.</remarks>
    /// <param name="id">The organization's id.</param>
    /// <response code="201">The request was created.</response>
    /// <response code="404">The organization does not exist.</response>
    /// <response code="409">The user is already a member, or has already requested membership.</response>
    [HttpPost("{id:int}/membership-requests")]
    [ProducesResponseType<OrganizationMemberDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RequestMembership(int id, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        if (await organizationRepository.FindAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        if (await organizationRepository.FindMemberAsync(id, userId, cancellationToken) is not null)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The user is already a member, or has already requested membership.");
        }

        await organizationRepository.AddMemberAsync(new OrganizationMember
        {
            OrganizationId = id,
            UserId = userId,
            Status = MembershipStatus.Requested,
            MemberType = MemberType.Member,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, await organizationRepository.FindMemberDtoAsync(id, userId, cancellationToken));
    }

    private async Task<bool> IsApprovedOwnerAsync(int organizationId, int userId, CancellationToken cancellationToken)
    {
        var member = await organizationRepository.FindMemberAsync(organizationId, userId, cancellationToken);
        return member is { Status: MembershipStatus.Approved, MemberType: MemberType.Owner };
    }
}
