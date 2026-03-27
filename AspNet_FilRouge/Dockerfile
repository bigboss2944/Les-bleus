FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier solution et les projets nécessaires
COPY FilRouge.sln .
COPY Entities/ Entities/
COPY Database/ Database/
COPY AspNet_FilRouge/ AspNet_FilRouge/
RUN ls -l /src

WORKDIR /src/AspNet_FilRouge
RUN ls -l /src/AspNet_FilRouge
RUN dotnet restore ../FilRouge.sln
RUN dotnet publish AspNet_FilRouge.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5100
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]