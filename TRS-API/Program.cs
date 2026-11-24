using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TRS_API.Middleware;
using TRS_API.OpenApi;
using TRS_Lib.Data;
using TRS_Lib.Repositories;
using TRS_Lib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ConnectionStrings:Default wins; otherwise use DATABASE_URL, which Railway provides
var databaseUrl = builder.Configuration["DATABASE_URL"];
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? (string.IsNullOrWhiteSpace(databaseUrl) ? null : DatabaseUrl.ToConnectionString(databaseUrl));

builder.Services.AddDbContext<TrsDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOneTimePasswordRepository, OneTimePasswordRepository>();
builder.Services.AddScoped<OneTimePasswordService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();

var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep claim names as they are in the token ("sub", "email"), instead of mapping them to long URIs
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = jwtSettings.SigningKey,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

// Enums are sent and received as their names ("Approved", "Owner"), not as numbers
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
// The OpenAPI document reads these options, so Swagger shows the enums as strings too
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "TRS API";
        document.Info.Version = "v1";
        return Task.CompletedTask;
    });
    options.AddDocumentTransformer<SecurityTransformer>();
    options.AddOperationTransformer<SecurityTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger is on in Development; enable it elsewhere with the Swagger__Enabled=true setting
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.MapOpenApi().WithMetadata(new AllowWithoutApiKeyAttribute());
    // Registered before the API key check so the UI itself loads without a key
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "TRS API v1"));
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
