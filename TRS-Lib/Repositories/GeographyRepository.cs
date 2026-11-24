using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TRS_Lib.Data;
using TRS_Lib.Dtos;
using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public class GeographyRepository(TrsDbContext db) : IGeographyRepository
{
    // The database's collation doesn't sort Å, Ä and Ö the Swedish way, so lists are sorted here
    private static readonly StringComparer SwedishOrder = StringComparer.Create(CultureInfo.GetCultureInfo("sv-SE"), ignoreCase: true);

    public async Task<List<CountryDto>> ListCountriesAsync(CancellationToken cancellationToken = default)
    {
        var countries = await db.Countries.Select(c => new CountryDto { Id = c.Id, Name = c.Name }).ToListAsync(cancellationToken);
        return countries.OrderBy(c => c.Name, SwedishOrder).ToList();
    }

    public Task<CountryDto?> FindCountryAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Countries.Where(c => c.Id == id)
            .Select(c => new CountryDto { Id = c.Id, Name = c.Name })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CountryDto> CreateCountryAsync(CountryInputDto input, CancellationToken cancellationToken = default)
    {
        var country = new Country { Name = input.Name };
        db.Countries.Add(country);
        await db.SaveChangesAsync(cancellationToken);
        return new CountryDto { Id = country.Id, Name = country.Name };
    }

    public async Task<CountryDto?> UpdateCountryAsync(int id, CountryInputDto input, CancellationToken cancellationToken = default)
    {
        var country = await db.Countries.FindAsync([id], cancellationToken);
        if (country is null)
        {
            return null;
        }

        country.Name = input.Name;
        await db.SaveChangesAsync(cancellationToken);
        return new CountryDto { Id = country.Id, Name = country.Name };
    }

    public async Task<bool> DeleteCountryAsync(int id, CancellationToken cancellationToken = default)
    {
        return await db.Countries.Where(c => c.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public Task<bool> CountryHasProvincesAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Provinces.AnyAsync(p => p.CountryId == id, cancellationToken);
    }

    public async Task<List<ProvinceDto>> ListProvincesAsync(int? countryId, CancellationToken cancellationToken = default)
    {
        var query = db.Provinces.AsQueryable();
        if (countryId is not null)
        {
            query = query.Where(p => p.CountryId == countryId);
        }

        var provinces = await query.Select(p => new ProvinceDto { Id = p.Id, CountryId = p.CountryId, Name = p.Name }).ToListAsync(cancellationToken);
        return provinces.OrderBy(p => p.Name, SwedishOrder).ToList();
    }

    public Task<ProvinceDto?> FindProvinceAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Provinces.Where(p => p.Id == id)
            .Select(p => new ProvinceDto { Id = p.Id, CountryId = p.CountryId, Name = p.Name })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProvinceDto?> CreateProvinceAsync(ProvinceInputDto input, CancellationToken cancellationToken = default)
    {
        if (!await db.Countries.AnyAsync(c => c.Id == input.CountryId, cancellationToken))
        {
            return null;
        }

        var province = new Province { CountryId = input.CountryId, Name = input.Name };
        db.Provinces.Add(province);
        await db.SaveChangesAsync(cancellationToken);
        return new ProvinceDto { Id = province.Id, CountryId = province.CountryId, Name = province.Name };
    }

    public async Task<ProvinceDto?> UpdateProvinceAsync(int id, ProvinceInputDto input, CancellationToken cancellationToken = default)
    {
        var province = await db.Provinces.FindAsync([id], cancellationToken);
        if (province is null || !await db.Countries.AnyAsync(c => c.Id == input.CountryId, cancellationToken))
        {
            return null;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        if (province.CountryId != input.CountryId)
        {
            // Municipalities store their country too, so keep them in step with the province
            await db.Municipalities
                .Where(m => m.ProvinceId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.CountryId, input.CountryId), cancellationToken);
        }

        province.CountryId = input.CountryId;
        province.Name = input.Name;
        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return new ProvinceDto { Id = province.Id, CountryId = province.CountryId, Name = province.Name };
    }

    public async Task<bool> DeleteProvinceAsync(int id, CancellationToken cancellationToken = default)
    {
        return await db.Provinces.Where(p => p.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public Task<bool> ProvinceHasMunicipalitiesAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Municipalities.AnyAsync(m => m.ProvinceId == id, cancellationToken);
    }

    public async Task<List<MunicipalityDto>> ListMunicipalitiesAsync(int? countryId, int? provinceId, CancellationToken cancellationToken = default)
    {
        var query = db.Municipalities.AsQueryable();
        if (countryId is not null)
        {
            query = query.Where(m => m.CountryId == countryId);
        }
        if (provinceId is not null)
        {
            query = query.Where(m => m.ProvinceId == provinceId);
        }

        var municipalities = await query
            .Select(m => new MunicipalityDto { Id = m.Id, ProvinceId = m.ProvinceId, CountryId = m.CountryId, Name = m.Name })
            .ToListAsync(cancellationToken);
        return municipalities.OrderBy(m => m.Name, SwedishOrder).ToList();
    }

    public Task<MunicipalityDto?> FindMunicipalityAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Municipalities.Where(m => m.Id == id)
            .Select(m => new MunicipalityDto { Id = m.Id, ProvinceId = m.ProvinceId, CountryId = m.CountryId, Name = m.Name })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MunicipalityDto?> CreateMunicipalityAsync(MunicipalityInputDto input, CancellationToken cancellationToken = default)
    {
        var countryId = await FindProvinceCountryIdAsync(input.ProvinceId, cancellationToken);
        if (countryId is null)
        {
            return null;
        }

        var municipality = new Municipality { ProvinceId = input.ProvinceId, CountryId = countryId.Value, Name = input.Name };
        db.Municipalities.Add(municipality);
        await db.SaveChangesAsync(cancellationToken);
        return new MunicipalityDto { Id = municipality.Id, ProvinceId = municipality.ProvinceId, CountryId = municipality.CountryId, Name = municipality.Name };
    }

    public async Task<MunicipalityDto?> UpdateMunicipalityAsync(int id, MunicipalityInputDto input, CancellationToken cancellationToken = default)
    {
        var municipality = await db.Municipalities.FindAsync([id], cancellationToken);
        var countryId = await FindProvinceCountryIdAsync(input.ProvinceId, cancellationToken);
        if (municipality is null || countryId is null)
        {
            return null;
        }

        municipality.ProvinceId = input.ProvinceId;
        municipality.CountryId = countryId.Value;
        municipality.Name = input.Name;
        await db.SaveChangesAsync(cancellationToken);
        return new MunicipalityDto { Id = municipality.Id, ProvinceId = municipality.ProvinceId, CountryId = municipality.CountryId, Name = municipality.Name };
    }

    public async Task<bool> DeleteMunicipalityAsync(int id, CancellationToken cancellationToken = default)
    {
        return await db.Municipalities.Where(m => m.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    private async Task<int?> FindProvinceCountryIdAsync(int provinceId, CancellationToken cancellationToken)
    {
        return await db.Provinces.Where(p => p.Id == provinceId).Select(p => (int?)p.CountryId).FirstOrDefaultAsync(cancellationToken);
    }
}
