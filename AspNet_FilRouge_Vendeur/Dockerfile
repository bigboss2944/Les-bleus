FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
RUN ls -l /src
# Copier le fichier solution et les projets nécessaires
COPY FilRouge.sln .
COPY Entities/ Entities/
COPY Database/ Database/
COPY AspNet_FilRouge_Vendeur/ AspNet_FilRouge_Vendeur/
COPY AspNet_FilRouge_Vendeur/Tests/ AspNet_FilRouge_Vendeur/Tests/
RUN ls -l /src

WORKDIR /src/AspNet_FilRouge_Vendeur
RUN ls -l /src/AspNet_FilRouge_Vendeur
RUN dotnet restore AspNet_FilRouge_Vendeur.csproj
RUN dotnet publish AspNet_FilRouge_Vendeur.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5101
ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]