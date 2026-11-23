namespace TRS_Lib.Dtos;

public class LoginResponseDto
{
    /// <summary>The 6 digit one-time code. Only returned while there is no email service.</summary>
    public required string Code { get; set; }

    /// <summary>When the code stops being valid (UTC).</summary>
    public DateTimeOffset ExpiresAt { get; set; }
}
