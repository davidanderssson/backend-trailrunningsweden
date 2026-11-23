using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class LoginDto
{
    /// <summary>Email address of an existing user.</summary>
    /// <example>anna@example.com</example>
    [Required, EmailAddress, MaxLength(320)]
    public required string Email { get; set; }
}
