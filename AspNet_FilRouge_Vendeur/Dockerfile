FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie toutes les sources nécessaires au build (attention : ne build QU'UN projet à la fois !)
COPY ../Database/ ../Database/
COPY ../Entities/ ../Entities/
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5101
ENV ASPNETCORE_URLS=http://0.0.0.0:5101
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]

# # ---- Build stage ----
# # Raspberry Pi 3 utilise ARMv7 32-bit, pas ARM64
# FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src

# # Copy csproj and restore dependencies
# COPY *.csproj ./
# RUN dotnet restore

# # Copy remaining files and publish release
# COPY ../Database/ Database/
# COPY ../Entities/ Entities/
# COPY . ./
# RUN dotnet publish -c Release -o /app/publish --no-restore

# # ---- Runtime stage ----
# FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app

# # Copy published output
# COPY --from=build /app/publish .

# # Expose HTTP port
# EXPOSE 5101

# # Configure Kestrel to bind to all network interfaces
# ENV ASPNETCORE_URLS=http://0.0.0.0:5101

# # Entry point: reference your published DLL directly
# ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]