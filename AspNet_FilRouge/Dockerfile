# Étape de build
FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier **tous les fichiers** depuis la racine de la solution
COPY . .

# Restaurer **la solution entière**
RUN dotnet restore FilRouge.sln

# Publier uniquement le projet Admin
RUN dotnet publish AspNet_FilRouge/AspNet_FilRouge.csproj \
    -c Release -o /app/publish --no-restore

# Étape runtime
FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copier les fichiers publiés
COPY --from=build /app/publish .

# Exposition du port Admin
EXPOSE 5100
ENV ASPNETCORE_URLS=http://0.0.0.0:5100

ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]