namespace TRS_Lib.Models;

// Links users to organizations (many to many). The key is OrganizationId + UserId.
public class OrganizationMember
{
    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public MembershipStatus Status { get; set; }
    public MemberType MemberType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
