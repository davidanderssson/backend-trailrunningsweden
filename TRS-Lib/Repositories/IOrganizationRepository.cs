using TRS_Lib.Dtos;
using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public interface IOrganizationRepository
{
    // Creates the organization with the given user as its approved owner
    Task<Organization> CreateAsync(Organization organization, int ownerUserId, CancellationToken cancellationToken = default);
    Task<Organization?> FindAsync(int id, CancellationToken cancellationToken = default);

    Task<OrganizationMember?> FindMemberAsync(int organizationId, int userId, CancellationToken cancellationToken = default);
    Task<OrganizationMemberDto?> FindMemberDtoAsync(int organizationId, int userId, CancellationToken cancellationToken = default);
    Task<List<OrganizationMemberDto>> ListMembersAsync(int organizationId, bool includeRequested, CancellationToken cancellationToken = default);

    Task AddMemberAsync(OrganizationMember member, CancellationToken cancellationToken = default);

    // Sets an existing membership to approved with the given type. Returns false if there is no membership.
    Task<bool> ApproveMemberAsync(int organizationId, int userId, MemberType memberType, CancellationToken cancellationToken = default);

    // Atomically switches a pending request to approved, keeping its member type. Returns false if there is no pending request.
    Task<bool> ApproveRequestAsync(int organizationId, int userId, CancellationToken cancellationToken = default);

    // Returns false if there was no membership
    Task<bool> RemoveMemberAsync(int organizationId, int userId, CancellationToken cancellationToken = default);

    Task<int> CountApprovedOwnersAsync(int organizationId, CancellationToken cancellationToken = default);
}
