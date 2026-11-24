namespace TRS_Lib.Dtos;

public class TokenResponseDto
{
    /// <summary>JWT to send as "Authorization: Bearer {token}". Valid for 60 minutes.</summary>
    public required string AccessToken { get; set; }

    /// <summary>When the access token expires (UTC).</summary>
    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    /// <summary>Exchange this for a new token pair at /user/refresh. Works once and is valid for 30 days.</summary>
    public required string RefreshToken { get; set; }

    /// <summary>When the refresh token expires (UTC).</summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}
