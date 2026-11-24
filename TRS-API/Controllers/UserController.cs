using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TRS_API.Extensions;
using TRS_Lib.Dtos;
using TRS_Lib.Models;
using TRS_Lib.Repositories;
using TRS_Lib.Services;

namespace TRS_API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Tags("Users")]
public class UserController(
    IUserRepository userRepository,
    OneTimePasswordService oneTimePasswordService,
    TokenService tokenService) : ControllerBase
{
    /// <summary>Starts a login by creating a one-time code for the user.</summary>
    /// <remarks>
    /// The code is valid for 10 minutes and replaces any earlier code for the same user.
    /// For now the code is returned in the response for debugging; it will be sent by email later.
    /// </remarks>
    /// <response code="200">A code was created.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="404">No user has that email address.</response>
    // TODO: send the code by email and stop returning it once the email service exists
    [HttpPost("login")]
    [Consumes("application/json")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailAsync(dto.Email, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var (code, expiresAt) = await oneTimePasswordService.CreateAsync(user.Id, cancellationToken);
        return Ok(new LoginResponseDto { Code = code, ExpiresAt = expiresAt });
    }

    /// <summary>Completes a login by verifying the one-time code.</summary>
    /// <remarks>
    /// A code can only be used once and allows 5 attempts. The same error is returned whether the
    /// email is unknown, the code is wrong, expired, already used or out of attempts.
    /// </remarks>
    /// <response code="200">The code is valid. Returns an access token and a refresh token.</response>
    /// <response code="400">The request body is invalid, or the code is not valid.</response>
    [HttpPost("login/verify")]
    [Consumes("application/json")]
    [ProducesResponseType<TokenResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyLogin(VerifyLoginDto dto, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailAsync(dto.Email, cancellationToken);
        if (user is null || !await oneTimePasswordService.VerifyAsync(user.Id, dto.Code, cancellationToken))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid or expired code.");
        }

        return Ok(await tokenService.IssueAsync(user, cancellationToken));
    }

    /// <summary>Exchanges a refresh token for a new access token and refresh token.</summary>
    /// <remarks>
    /// Each refresh token works once. Reusing one revokes all of the user's refresh tokens,
    /// since it suggests the token was stolen; the user then has to log in again.
    /// </remarks>
    /// <response code="200">Returns a new access token and refresh token.</response>
    /// <response code="400">The refresh token is invalid, expired, used or revoked.</response>
    [HttpPost("refresh")]
    [Consumes("application/json")]
    [ProducesResponseType<TokenResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var tokens = await tokenService.RefreshAsync(dto.RefreshToken, cancellationToken);
        if (tokens is null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid or expired refresh token.");
        }

        return Ok(tokens);
    }

    /// <summary>Logs out by revoking the refresh token.</summary>
    /// <remarks>The access token stays valid until it expires, at most 60 minutes.</remarks>
    /// <response code="204">The refresh token is revoked, or did not exist.</response>
    /// <response code="400">The request body is invalid.</response>
    [HttpPost("logout")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        await tokenService.RevokeAsync(dto.RefreshToken, cancellationToken);
        return NoContent();
    }

    /// <summary>Returns the logged in user.</summary>
    /// <remarks>Requires an access token.</remarks>
    /// <response code="200">The logged in user.</response>
    /// <response code="404">The user in the token no longer exists.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        var user = await userRepository.FindAsync(userId, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Creates a new user.</summary>
    /// <response code="201">The user was created.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="409">A user with that email address already exists.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(UserDto dto, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        try
        {
            await userRepository.CreateAsync(user, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "A user with that email already exists.");
        }

        return StatusCode(StatusCodes.Status201Created, user);
    }
}
