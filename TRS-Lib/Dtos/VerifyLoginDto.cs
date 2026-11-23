using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class VerifyLoginDto
{
    /// <summary>Email address of the user that requested the code.</summary>
    /// <example>anna@example.com</example>
    [Required, EmailAddress, MaxLength(320)]
    public required string Email { get; set; }

    /// <summary>The 6 digit code from the login step.</summary>
    /// <example>123456</example>
    [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "The code must be 6 digits.")]
    public required string Code { get; set; }
}
