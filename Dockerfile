# ============================================================================
# Dockerfile multi-stage pour une API ASP.NET Core (.NET 10) - Linux Containers
# ============================================================================

# ----------------------------------------------------------------------------
# Stage 1 : Build / Publish
# ----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Chemin du projet ASP.NET Core (.NET 10)
ARG API_PROJECT="AspNet_FilRouge/AspNet_FilRouge.csproj"

# Copier uniquement les fichiers solution/projet d'abord (optimise le cache)
COPY *.sln ./
COPY AspNet_FilRouge/*.csproj AspNet_FilRouge/

# Restaurer les dépendances
RUN dotnet restore "$API_PROJECT"

# Copier le reste du code
COPY . .

# Publier en Release
RUN dotnet publish "$API_PROJECT" -c Release -o /app/publish /p:UseAppHost=false

# ----------------------------------------------------------------------------
# Stage 2 : Runtime
# ----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copier l'output publié
COPY --from=build /app/publish .

# Exposer le port HTTP (Kestrel)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Démarrer l'application
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]
