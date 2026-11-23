using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class UserDto
{
    /// <example>Anna</example>
    [Required, MaxLength(100)]
    public required string FirstName { get; set; }

    /// <example>Svensson</example>
    [Required, MaxLength(100)]
    public required string LastName { get; set; }

    /// <example>anna@example.com</example>
    [Required, EmailAddress, MaxLength(320)]
    public required string Email { get; set; }
}
