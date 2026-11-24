namespace TRS_Lib.Models;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    // Admins can create, update and delete reference data such as geography. Not settable through the API.
    public bool Admin { get; set; }
}
