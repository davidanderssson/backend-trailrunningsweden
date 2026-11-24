using System.ComponentModel.DataAnnotations;

namespace TRS_Lib.Dtos;

public class MunicipalityInputDto
{
    /// <summary>Id of the province the municipality belongs to. The country is taken from the province.</summary>
    /// <example>15</example>
    [Range(1, int.MaxValue)]
    public int ProvinceId { get; set; }

    /// <summary>The municipality's name. Must be unique within the province.</summary>
    /// <example>Uppsala</example>
    [Required, MaxLength(100)]
    public required string Name { get; set; }
}
