using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class RefreshTokenDto
{
    /// <summary>The refresh token from the last login or refresh.</summary>
    [Required, MaxLength(100)]
    public required string RefreshToken { get; set; }
}
