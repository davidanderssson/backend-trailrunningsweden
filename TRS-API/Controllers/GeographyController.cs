using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TRS_API.Authorization;
using TRS_API.Extensions;
using TRS_Lib.Dtos;
using TRS_Lib.Repositories;

namespace TRS_API.Controllers;

// Reading needs only the API key. Creating, changing and deleting needs a logged in admin.
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Tags("Geography")]
public class GeographyController(IGeographyRepository geography) : ControllerBase
{
    // Countries

    /// <summary>Lists all countries.</summary>
    /// <response code="200">The countries, sorted by name.</response>
    [HttpGet("countries")]
    [ProducesResponseType<List<CountryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListCountries(CancellationToken cancellationToken)
    {
        return Ok(await geography.ListCountriesAsync(cancellationToken));
    }

    /// <summary>Gets a country.</summary>
    /// <param name="id">The country's id.</param>
    /// <response code="200">The country.</response>
    /// <response code="404">The country does not exist.</response>
    [HttpGet("countries/{id:int}")]
    [ProducesResponseType<CountryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCountry(int id, CancellationToken cancellationToken)
    {
        var country = await geography.FindCountryAsync(id, cancellationToken);
        return country is null ? NotFound() : Ok(country);
    }

    /// <summary>Lists a country's provinces.</summary>
    /// <param name="id">The country's id.</param>
    /// <response code="200">The provinces, sorted by name.</response>
    /// <response code="404">The country does not exist.</response>
    [HttpGet("countries/{id:int}/provinces")]
    [ProducesResponseType<List<ProvinceDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListCountryProvinces(int id, CancellationToken cancellationToken)
    {
        if (await geography.FindCountryAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        return Ok(await geography.ListProvincesAsync(id, cancellationToken));
    }

    /// <summary>Lists a country's municipalities.</summary>
    /// <param name="id">The country's id.</param>
    /// <response code="200">The municipalities, sorted by name.</response>
    /// <response code="404">The country does not exist.</response>
    [HttpGet("countries/{id:int}/municipalities")]
    [ProducesResponseType<List<MunicipalityDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListCountryMunicipalities(int id, CancellationToken cancellationToken)
    {
        if (await geography.FindCountryAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        return Ok(await geography.ListMunicipalitiesAsync(id, null, cancellationToken));
    }

    /// <summary>Creates a country.</summary>
    /// <response code="201">The country was created.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="409">A country with that name already exists.</response>
    [HttpPost("countries")]
    [Authorize(Policy = Policies.Admin)]
    [Consumes("application/json")]
    [ProducesResponseType<CountryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCountry(CountryInputDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var country = await geography.CreateCountryAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "A country with that name already exists.");
        }
    }

    /// <summary>Updates a country.</summary>
    /// <param name="id">The country's id.</param>
    /// <response code="200">The updated country.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="404">The country does not exist.</response>
    /// <response code="409">A country with that name already exists.</response>
    [HttpPut("countries/{id:int}")]
    [Authorize(Policy = Policies.Admin)]
    [Consumes("application/json")]
    [ProducesResponseType<CountryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCountry(int id, CountryInputDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var country = await geography.UpdateCountryAsync(id, dto, cancellationToken);
            return country is null ? NotFound() : Ok(country);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "A country with that name already exists.");
        }
    }

    /// <summary>Deletes a country.</summary>
    /// <remarks>A country that still has provinces cannot be deleted.</remarks>
    /// <param name="id">The country's id.</param>
    /// <response code="204">The country was deleted.</response>
    /// <response code="404">The country does not exist.</response>
    /// <response code="409">The country still has provinces.</response>
    [HttpDelete("countries/{id:int}")]
    [Authorize(Policy = Policies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCountry(int id, CancellationToken cancellationToken)
    {
        if (await geography.CountryHasProvincesAsync(id, cancellationToken))
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The country still has provinces.");
        }

        return await geography.DeleteCountryAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    // Provinces

    /// <summary>Lists all provinces.</summary>
    /// <response code="200">The provinces, sorted by name.</response>
    [HttpGet("provinces")]
    [ProducesResponseType<List<ProvinceDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProvinces(CancellationToken cancellationToken)
    {
        return Ok(await geography.ListProvincesAsync(null, cancellationToken));
    }

    /// <summary>Gets a province.</summary>
    /// <param name="id">The province's id.</param>
    /// <response code="200">The province.</response>
    /// <response code="404">The province does not exist.</response>
    [HttpGet("provinces/{id:int}")]
    [ProducesResponseType<ProvinceDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProvince(int id, CancellationToken cancellationToken)
    {
        var province = await geography.FindProvinceAsync(id, cancellationToken);
        return province is null ? NotFound() : Ok(province);
    }

    /// <summary>Lists a province's municipalities.</summary>
    /// <param name="id">The province's id.</param>
    /// <response code="200">The municipalities, sorted by name.</response>
    /// <response code="404">The province does not exist.</response>
    [HttpGet("provinces/{id:int}/municipalities")]
    [ProducesResponseType<List<MunicipalityDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListProvinceMunicipalities(int id, CancellationToken cancellationToken)
    {
        if (await geography.FindProvinceAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        return Ok(await geography.ListMunicipalitiesAsync(null, id, cancellationToken));
    }

    /// <summary>Creates a province.</summary>
    /// <response code="201">The province was created.</response>
    /// <response code="400">The request body is invalid, or the country does not exist.</response>
    /// <response code="409">The country already has a province with that name.</response>
    [HttpPost("provinces")]
    [Authorize(Policy = Policies.Admin)]
    [Consumes("application/json")]
    [ProducesResponseType<ProvinceDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProvince(ProvinceInputDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var province = await geography.CreateProvinceAsync(dto, cancellationToken);
            return province is null
                ? CountryNotFound(dto.CountryId)
                : CreatedAtAction(nameof(GetProvince), new { id = province.Id }, province);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The country already has a province with that name.");
        }
    }

    /// <summary>Updates a province.</summary>
    /// <remarks>Moving a province to another country also moves its municipalities.</remarks>
    /// <param name="id">The province's id.</param>
    /// <response code="200">The updated province.</response>
    /// <response code="400">The request body is invalid, or the country does not exist.</response>
    /// <response code="404">The province does not exist.</response>
    /// <response code="409">The country already has a province with that name.</response>
    [HttpPut("provinces/{id:int}")]
    [Authorize(Policy = Policies.Admin)]
    [Consumes("application/json")]
    [ProducesResponseType<ProvinceDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProvince(int id, ProvinceInputDto dto, CancellationToken cancellationToken)
    {
        if (await geography.FindProvinceAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        try
        {
            var province = await geography.UpdateProvinceAsync(id, dto, cancellationToken);
            return province is null ? CountryNotFound(dto.CountryId) : Ok(province);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The country already has a province with that name.");
        }
    }

    /// <summary>Deletes a province.</summary>
    /// <remarks>A province that still has municipalities cannot be deleted.</remarks>
    /// <param name="id">The province's id.</param>
    /// <response code="204">The province was deleted.</response>
    /// <response code="404">The province does not exist.</response>
    /// <response code="409">The province still has municipalities.</response>
    [HttpDelete("provinces/{id:int}")]
    [Authorize(Policy = Policies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteProvince(int id, CancellationToken cancellationToken)
    {
        if (await geography.ProvinceHasMunicipalitiesAsync(id, cancellationToken))
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The province still has municipalities.");
        }

        return await geography.DeleteProvinceAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    // Municipalities

    /// <summary>Lists all municipalities.</summary>
    /// <response code="200">The municipalities, sorted by name.</response>
    [HttpGet("municipalities")]
    [ProducesResponseType<List<MunicipalityDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListMunicipalities(CancellationToken cancellationToken)
    {
        return Ok(await geography.ListMunicipalitiesAsync(null, null, cancellationToken));
    }

    /// <summary>Gets a municipality.</summary>
    /// <param name="id">The municipality's id.</param>
    /// <response code="200">The municipality.</response>
    /// <response code="404">The municipality does not exist.</response>
    [HttpGet("municipalities/{id:int}")]
    [ProducesResponseType<MunicipalityDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMunicipality(int id, CancellationToken cancellationToken)
    {
        var municipality = await geography.FindMunicipalityAsync(id, cancellationToken);
        return municipality is null ? NotFound() : Ok(municipality);
    }

    /// <summary>Creates a municipality.</summary>
    /// <remarks>The country is taken from the province.</remarks>
    /// <response code="201">The municipality was created.</response>
    /// <response code="400">The request body is invalid, or the province does not exist.</response>
    /// <response code="409">The province already has a municipality with that name.</response>
    [HttpPost("municipalities")]
    [Authorize(Policy = Policies.Admin)]
    [Consumes("application/json")]
    [ProducesResponseType<MunicipalityDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMunicipality(MunicipalityInputDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var municipality = await geography.CreateMunicipalityAsync(dto, cancellationToken);
            return municipality is null
                ? ProvinceNotFound(dto.ProvinceId)
                : CreatedAtAction(nameof(GetMunicipality), new { id = municipality.Id }, municipality);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The province already has a municipality with that name.");
        }
    }

    /// <summary>Updates a municipality.</summary>
    /// <remarks>The country is taken from the province.</remarks>
    /// <param name="id">The municipality's id.</param>
    /// <response code="200">The updated municipality.</response>
    /// <response code="400">The request body is invalid, or the province does not exist.</response>
    /// <response code="404">The municipality does not exist.</response>
    /// <response code="409">The province already has a municipality with that name.</response>
    [HttpPut("municipalities/{id:int}")]
    [Authorize(Policy = Policies.Admin)]
    [Consumes("application/json")]
    [ProducesResponseType<MunicipalityDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateMunicipality(int id, MunicipalityInputDto dto, CancellationToken cancellationToken)
    {
        if (await geography.FindMunicipalityAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        try
        {
            var municipality = await geography.UpdateMunicipalityAsync(id, dto, cancellationToken);
            return municipality is null ? ProvinceNotFound(dto.ProvinceId) : Ok(municipality);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "The province already has a municipality with that name.");
        }
    }

    /// <summary>Deletes a municipality.</summary>
    /// <param name="id">The municipality's id.</param>
    /// <response code="204">The municipality was deleted.</response>
    /// <response code="404">The municipality does not exist.</response>
    [HttpDelete("municipalities/{id:int}")]
    [Authorize(Policy = Policies.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMunicipality(int id, CancellationToken cancellationToken)
    {
        return await geography.DeleteMunicipalityAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    private ObjectResult CountryNotFound(int countryId) =>
        Problem(statusCode: StatusCodes.Status400BadRequest, title: $"Country {countryId} does not exist.");

    private ObjectResult ProvinceNotFound(int provinceId) =>
        Problem(statusCode: StatusCodes.Status400BadRequest, title: $"Province {provinceId} does not exist.");
}
