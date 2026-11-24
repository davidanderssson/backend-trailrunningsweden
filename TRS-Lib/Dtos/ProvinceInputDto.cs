using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class ProvinceInputDto
{
    /// <summary>Id of the country the province belongs to.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    /// <summary>The province's name. Must be unique within the country.</summary>
    /// <example>Uppland</example>
    [Required, MaxLength(100)]
    public required string Name { get; set; }
}
