namespace TRS_Lib.Models;

public class OneTimePassword
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public required string CodeHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public int Attempts { get; set; }
}
