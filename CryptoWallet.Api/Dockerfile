# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY CryptoWallet.Api.sln ./
COPY CryptoWallet.Api/CryptoWallet.Api.csproj CryptoWallet.Api/
COPY CryptoWallet.Tests/CryptoWallet.Tests.csproj CryptoWallet.Tests/

RUN dotnet restore CryptoWallet.Api/CryptoWallet.Api.csproj

# Copy the rest of the source
COPY . .

# Publish (trim symbols, ready-to-run for faster startup)
RUN dotnet publish CryptoWallet.Api/CryptoWallet.Api.csproj -c Release -o /app/publish --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Optionally create a non-root user (good practice)
# RUN adduser --disabled-password --gecos '' app && chown -R app:app /app
# USER app

# ASP.NET Core listens on this URL inside the container
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

# Connection string can be injected at runtime (overrides appsettings.json)
# Example (docker run or compose):
#   -e ConnectionStrings__PostgresConnectionString="Host=db;Database=cryptowallet;Username=postgres;Password=postgres;SSL Mode=Disable"
# The double underscore __ maps to ':' in ASP.NET Core configuration.
ENV ConnectionStrings__PostgresConnectionString=""

EXPOSE 8080

COPY --from=build /app/publish ./

# Default entrypoint
ENTRYPOINT ["dotnet", "CryptoWallet.Api.dll"]
