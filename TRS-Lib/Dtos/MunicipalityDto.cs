namespace TRS_Lib.Dtos;

public class MunicipalityDto
{
    /// <example>32</example>
    public int Id { get; set; }

    /// <example>15</example>
    public int ProvinceId { get; set; }

    /// <example>1</example>
    public int CountryId { get; set; }

    /// <example>Uppsala</example>
    public required string Name { get; set; }
}
