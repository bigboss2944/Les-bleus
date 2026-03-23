# Docker – AspNet_FilRouge API

Ce document explique comment construire et exécuter l'image Docker de l'API ASP.NET du projet Les-bleus.

## Prérequis

- [Docker Desktop](https://docs.docker.com/get-docker/) installé avec **Windows Containers** activés
- Windows 10/11 ou Windows Server 2022 (hôte compatible conteneurs Windows)

> **Note sur Raspberry Pi (ARM) :** Le projet actuel cible **.NET Framework 4.7.2** qui fonctionne
> uniquement sous Windows et ne supporte pas l'architecture ARM (Linux). Pour déployer sur
> Raspberry Pi, une migration vers **ASP.NET Core (.NET 8 ou supérieur)** est nécessaire,
> permettant d'utiliser les images `mcr.microsoft.com/dotnet/aspnet:8.0-arm64v8`.

## Basculer en mode Windows Containers

```powershell
# Depuis la barre des tâches : clic droit sur Docker > "Switch to Windows containers..."
# Ou via la ligne de commande :
& "C:\Program Files\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine
```

## Construire l'image

Depuis la racine du dépôt :

```powershell
docker build -t aspnet-filrouge:latest .
```

Pour une version taguée :

```powershell
docker build -t aspnet-filrouge:1.0 .
```

## Lancer l'image

L'application nécessite une base de données SQL Server. Vous pouvez utiliser
`docker-compose` (recommandé) ou démarrer les conteneurs manuellement.

### Avec Docker Compose (recommandé)

```powershell
docker-compose up -d
```

### Manuellement

**1. Démarrer un conteneur SQL Server (Linux) :**

```powershell
docker run -d `
  --name sqlserver `
  -e "ACCEPT_EULA=Y" `
  -e "SA_PASSWORD=VotreMotDePasse123!" `
  -p 1433:1433 `
  mcr.microsoft.com/mssql/server:2022-latest
```

**2. Démarrer le conteneur de l'API :**

```powershell
docker run -d `
  --name aspnet-filrouge `
  -p 8080:80 `
  aspnet-filrouge:latest
```

**3. Accéder à l'API :**

```
http://localhost:8080
```

## Configuration de la base de données

Le projet utilise **Web.config** pour la configuration. La chaîne de connexion par défaut
pointe vers une instance SQL Server locale. Pour un environnement Docker, modifiez
`AspNet_FilRouge/Web.Release.config` avec la transformation appropriée :

```xml
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Server=sqlserver,1433;Database=FilRougeDB;User Id=sa;Password=VotreMotDePasse123!;"
       providerName="System.Data.SqlClient"
       xdt:Transform="SetAttributes" xdt:Locator="Match(name)" />
</connectionStrings>
```

> **Note :** Le .NET Framework 4.7.2 lit la configuration depuis `Web.config`.
> L'utilisation de variables d'environnement pour surcharger la connexion nécessite
> d'utiliser les transformations Web.config (Web.Release.config) ou de lire les
> variables manuellement dans le code.

## Structure de l'image

| Stage | Image de base | Rôle |
|---|---|---|
| `build` | `mcr.microsoft.com/dotnet/framework/sdk:4.8` | Compilation MSBuild + restauration NuGet |
| `runtime` | `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2022` | IIS + ASP.NET 4.7.2 |

## Migration vers Raspberry Pi (ARM64)

Pour exécuter l'API sur Raspberry Pi, la migration vers ASP.NET Core est requise.
Les étapes principales sont :

1. Migrer le projet vers ASP.NET Core (.NET 8+)
2. Remplacer OWIN par le middleware ASP.NET Core
3. Mettre à jour Entity Framework vers EF Core
4. Utiliser le Dockerfile suivant après migration :

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish AspNet_FilRouge/AspNet_FilRouge.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-arm64v8
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]
```

```powershell
# Build pour ARM64 (Raspberry Pi 4/5)
docker buildx build --platform linux/arm64 -t aspnet-filrouge:arm64 .

# Lancer sur Raspberry Pi
docker run -d -p 8080:8080 aspnet-filrouge:arm64
```
