using System.ComponentModel.DataAnnotations;
using TRS_Lib.Models;

namespace TRS_Lib.Dtos;

public class AddOrganizationMemberDto
{
    /// <summary>Id of the user to add.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    /// <summary>The new member's role: "Owner" or "Member".</summary>
    /// <example>Member</example>
    // Nullable so that a missing value is rejected instead of silently becoming the first enum value
    [Required]
    public MemberType? MemberType { get; set; }
}
