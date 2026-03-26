# Étape build : SDK .NET 10 (image tierce Hardened Images)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copie seulement le projet pour le restore (plus de cache Docker)
COPY ["AspNet_FilRouge_Vendeur.csproj", "AspNet_FilRouge_Vendeur/"]

RUN dotnet restore "AspNet_FilRouge_Vendeur.csproj"

# Copie le reste des fichiers
COPY . .

WORKDIR "/src/AspNet_FilRouge_Vendeur"
RUN dotnet build "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/build
RUN dotnet publish "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/publish

# Étape runtime : ASP.NET 10 (image tierce Hardened Images)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]