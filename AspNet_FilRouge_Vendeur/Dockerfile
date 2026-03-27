# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier **tous les fichiers** depuis la racine de la solution
COPY . .

# Restaurer **la solution entière**
RUN dotnet restore FilRouge.sln

# Publier uniquement le projet Vendeur
RUN dotnet publish AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj \
    -c Release -o /app/publish --no-restore

# Étape runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copier les fichiers publiés
COPY --from=build /app/publish .

# Exposition du port Vendeur
EXPOSE 5101
ENV ASPNETCORE_URLS=http://0.0.0.0:5101

ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]