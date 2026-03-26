# Étape build : SDK .NET 10 (image tierce Hardened Images)
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# WORKDIR /src

# # Copie seulement le projet pour le restore (plus de cache Docker)
# COPY ["AspNet_FilRouge_Vendeur.csproj", "AspNet_FilRouge_Vendeur/"]

# RUN dotnet restore "AspNet_FilRouge_Vendeur.csproj"

# # Copie le reste des fichiers
# COPY . .

# WORKDIR "/src/AspNet_FilRouge_Vendeur"
# RUN dotnet build "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/build
# RUN dotnet publish "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/publish

# # Étape runtime : ASP.NET 10 (image tierce Hardened Images)
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# WORKDIR /app
# COPY --from=build /app/publish .
# ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]

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
EXPOSE 5101

# Configure Kestrel to bind to all network interfaces
ENV ASPNETCORE_URLS=http://0.0.0.0:5101

# Entry point: reference your published DLL directly
ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]