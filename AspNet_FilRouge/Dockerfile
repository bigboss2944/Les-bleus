FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# Restaurer la solution avec le chemin complet
RUN dotnet restore /src/FilRouge.sln

# Publier uniquement le projet Admin
RUN dotnet publish /src/AspNet_FilRouge/AspNet_FilRouge.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5100
ENV ASPNETCORE_URLS=http://0.0.0.0:5100

ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]