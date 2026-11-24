namespace TRS_Lib.Models;

public class Province
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    public required string Name { get; set; }
}
