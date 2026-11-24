using TRS_Lib.Models;

namespace TRS_Lib.Dtos;

public class OrganizationMemberDto
{
    /// <example>1</example>
    public int OrganizationId { get; set; }

    /// <example>2</example>
    public int UserId { get; set; }

    /// <example>Anna</example>
    public required string FirstName { get; set; }

    /// <example>Svensson</example>
    public required string LastName { get; set; }

    /// <example>anna@example.com</example>
    public required string Email { get; set; }

    /// <summary>"Requested" or "Approved".</summary>
    /// <example>Approved</example>
    public MembershipStatus Status { get; set; }

    /// <summary>"Owner" or "Member".</summary>
    /// <example>Member</example>
    public MemberType MemberType { get; set; }

    /// <summary>When the user was added or requested membership (UTC).</summary>
    public DateTimeOffset CreatedAt { get; set; }
}
