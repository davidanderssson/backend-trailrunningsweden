using Microsoft.EntityFrameworkCore;
using TRS_Lib.Data;
using TRS_Lib.Dtos;
using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public class OrganizationRepository(TrsDbContext db) : IOrganizationRepository
{
    public async Task<Organization> CreateAsync(Organization organization, int ownerUserId, CancellationToken cancellationToken = default)
    {
        // Adding the membership through the navigation saves both rows in one SaveChanges, so either both or neither exist
        db.OrganizationMembers.Add(new OrganizationMember
        {
            Organization = organization,
            UserId = ownerUserId,
            Status = MembershipStatus.Approved,
            MemberType = MemberType.Owner,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync(cancellationToken);
        return organization;
    }

    public Task<Organization?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public Task<OrganizationMember?> FindMemberAsync(int organizationId, int userId, CancellationToken cancellationToken = default)
    {
        return db.OrganizationMembers.AsNoTracking()
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == userId, cancellationToken);
    }

    public Task<OrganizationMemberDto?> FindMemberDtoAsync(int organizationId, int userId, CancellationToken cancellationToken = default)
    {
        return ToDto(db.OrganizationMembers.Where(m => m.OrganizationId == organizationId && m.UserId == userId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<OrganizationMemberDto>> ListMembersAsync(int organizationId, bool includeRequested, CancellationToken cancellationToken = default)
    {
        var members = db.OrganizationMembers.Where(m => m.OrganizationId == organizationId);
        if (!includeRequested)
        {
            members = members.Where(m => m.Status == MembershipStatus.Approved);
        }

        return ToDto(members)
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMemberAsync(OrganizationMember member, CancellationToken cancellationToken = default)
    {
        db.OrganizationMembers.Add(member);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ApproveMemberAsync(int organizationId, int userId, MemberType memberType, CancellationToken cancellationToken = default)
    {
        var updated = await db.OrganizationMembers
            .Where(m => m.OrganizationId == organizationId && m.UserId == userId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Status, MembershipStatus.Approved)
                .SetProperty(m => m.MemberType, memberType), cancellationToken);
        return updated > 0;
    }

    public async Task<bool> ApproveRequestAsync(int organizationId, int userId, CancellationToken cancellationToken = default)
    {
        var updated = await db.OrganizationMembers
            .Where(m => m.OrganizationId == organizationId && m.UserId == userId && m.Status == MembershipStatus.Requested)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.Status, MembershipStatus.Approved), cancellationToken);
        return updated > 0;
    }

    public async Task<bool> RemoveMemberAsync(int organizationId, int userId, CancellationToken cancellationToken = default)
    {
        var deleted = await db.OrganizationMembers
            .Where(m => m.OrganizationId == organizationId && m.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
        return deleted > 0;
    }

    public Task<int> CountApprovedOwnersAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        return db.OrganizationMembers.CountAsync(m =>
            m.OrganizationId == organizationId &&
            m.Status == MembershipStatus.Approved &&
            m.MemberType == MemberType.Owner, cancellationToken);
    }

    private static IQueryable<OrganizationMemberDto> ToDto(IQueryable<OrganizationMember> members)
    {
        return members.Select(m => new OrganizationMemberDto
        {
            OrganizationId = m.OrganizationId,
            UserId = m.UserId,
            FirstName = m.User!.FirstName,
            LastName = m.User.LastName,
            Email = m.User.Email,
            Status = m.Status,
            MemberType = m.MemberType,
            CreatedAt = m.CreatedAt
        });
    }
}
