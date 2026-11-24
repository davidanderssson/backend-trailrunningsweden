using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class CreateOrganizationDto
{
    /// <summary>The organization's name.</summary>
    /// <example>Trailrunning Sweden</example>
    [Required, MaxLength(200)]
    public required string Name { get; set; }
}
