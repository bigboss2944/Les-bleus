# ---- Build stage ----
# Raspberry Pi 3 utilise ARMv7 32-bit, pas ARM64
FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy remaining files and publish release
COPY ../Database/ Database/
COPY ../Entities/ Entities/
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 5100

# Configure Kestrel to bind to all network interfaces
ENV ASPNETCORE_URLS=http://0.0.0.0:5100

# Entry point: reference your published DLL directly
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]