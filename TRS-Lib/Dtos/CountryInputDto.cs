using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class CountryInputDto
{
    /// <summary>The country's name. Must be unique.</summary>
    /// <example>Sverige</example>
    [Required, MaxLength(100)]
    public required string Name { get; set; }
}
