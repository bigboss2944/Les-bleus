# =============================================================================
# Dockerfile multi-stage pour l'API ASP.NET (AspNet_FilRouge)
# Basé sur .NET Framework 4.7.2 - Nécessite des conteneurs Windows
# =============================================================================
# IMPORTANT : Cette image nécessite des conteneurs Windows (Windows Containers).
# Le framework .NET 4.7.2 classique ne supporte pas Linux/ARM.
# Pour un déploiement sur Raspberry Pi (ARM), une migration vers
# ASP.NET Core (.NET 8+) est requise.
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1 : Build
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build

WORKDIR /src

# Copier les fichiers de configuration NuGet pour optimiser le cache des layers
COPY FilRouge.sln .
COPY AspNet_FilRouge/packages.config AspNet_FilRouge/

# Restaurer les packages NuGet
RUN nuget restore FilRouge.sln

# Copier les sources et compiler en mode Release
COPY . .
RUN msbuild AspNet_FilRouge\AspNet_FilRouge.csproj /p:Configuration=Release /p:OutputPath=bin

# -----------------------------------------------------------------------------
# Stage 2 : Runtime (IIS + ASP.NET 4.7.2)
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2022 AS runtime

WORKDIR /inetpub/wwwroot

# Copier les DLLs compilées et les dépendances
COPY --from=build /src/AspNet_FilRouge/bin ./bin

# Copier les fichiers web (vues, contenu statique, configuration)
COPY --from=build /src/AspNet_FilRouge/Views ./Views
COPY --from=build /src/AspNet_FilRouge/Content ./Content
COPY --from=build /src/AspNet_FilRouge/Scripts ./Scripts
COPY --from=build /src/AspNet_FilRouge/Pictures ./Pictures
COPY --from=build /src/AspNet_FilRouge/Web.config .
COPY --from=build /src/AspNet_FilRouge/Web.Release.config .
COPY --from=build /src/AspNet_FilRouge/Global.asax .
COPY --from=build /src/AspNet_FilRouge/favicon.ico .

# Exposer le port HTTP
EXPOSE 80
