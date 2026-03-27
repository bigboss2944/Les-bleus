FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
RUN ls -l /src
# Copier le fichier solution et les projets nécessaires
COPY FilRouge.sln .
COPY Entities/ Entities/
COPY Database/ Database/
COPY AspNet_FilRouge/ AspNet_FilRouge/



RUN ls -l /src

WORKDIR /src/AspNet_FilRouge
RUN ls -l /src/AspNet_FilRouge
RUN dotnet restore AspNet_FilRouge.csproj
RUN dotnet publish AspNet_FilRouge.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Copie manuelle des scripts si nécessaire
COPY AspNet_FilRouge/Scripts ./Scripts

# IMPORTANT pour que Kestrel écoute sur le port exposé
ENV ASPNETCORE_URLS=http://0.0.0.0:5100

EXPOSE 5100
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]