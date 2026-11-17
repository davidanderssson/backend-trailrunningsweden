FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so the package layer is cached between code changes
COPY TRS.sln ./
COPY TRS-API/TRS-API.csproj TRS-API/
COPY TRS-Lib/TRS-Lib.csproj TRS-Lib/
RUN dotnet restore TRS-API/TRS-API.csproj

COPY . .
RUN dotnet publish TRS-API/TRS-API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Railway injects PORT; fall back to 8080 when running locally
ENV PORT=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["sh", "-c", "ASPNETCORE_HTTP_PORTS=$PORT exec dotnet TRS-API.dll"]
