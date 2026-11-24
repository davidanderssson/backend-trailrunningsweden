using TRS_Lib.Dtos;

namespace TRS_Lib.Repositories;

// Lists are sorted by name in Swedish order (Å, Ä, Ö last).
// Create and update return null when the referenced parent (country or province) does not exist.
public interface IGeographyRepository
{
    Task<List<CountryDto>> ListCountriesAsync(CancellationToken cancellationToken = default);
    Task<CountryDto?> FindCountryAsync(int id, CancellationToken cancellationToken = default);
    Task<CountryDto> CreateCountryAsync(CountryInputDto input, CancellationToken cancellationToken = default);
    Task<CountryDto?> UpdateCountryAsync(int id, CountryInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteCountryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CountryHasProvincesAsync(int id, CancellationToken cancellationToken = default);

    Task<List<ProvinceDto>> ListProvincesAsync(int? countryId, CancellationToken cancellationToken = default);
    Task<ProvinceDto?> FindProvinceAsync(int id, CancellationToken cancellationToken = default);
    Task<ProvinceDto?> CreateProvinceAsync(ProvinceInputDto input, CancellationToken cancellationToken = default);
    // Moving a province to another country also moves its municipalities
    Task<ProvinceDto?> UpdateProvinceAsync(int id, ProvinceInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteProvinceAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ProvinceHasMunicipalitiesAsync(int id, CancellationToken cancellationToken = default);

    Task<List<MunicipalityDto>> ListMunicipalitiesAsync(int? countryId, int? provinceId, CancellationToken cancellationToken = default);
    Task<MunicipalityDto?> FindMunicipalityAsync(int id, CancellationToken cancellationToken = default);
    Task<MunicipalityDto?> CreateMunicipalityAsync(MunicipalityInputDto input, CancellationToken cancellationToken = default);
    Task<MunicipalityDto?> UpdateMunicipalityAsync(int id, MunicipalityInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteMunicipalityAsync(int id, CancellationToken cancellationToken = default);
}
