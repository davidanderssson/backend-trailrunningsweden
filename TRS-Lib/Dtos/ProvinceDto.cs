namespace TRS_Lib.Dtos;

public class ProvinceDto
{
    /// <example>15</example>
    public int Id { get; set; }

    /// <example>1</example>
    public int CountryId { get; set; }

    /// <example>Uppland</example>
    public required string Name { get; set; }
}
