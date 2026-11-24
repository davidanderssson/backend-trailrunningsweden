namespace TRS_Lib.Models;

public class Municipality
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public Province? Province { get; set; }
    // Same country as the province; stored here too so municipalities can be listed per country directly
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    public required string Name { get; set; }
}
