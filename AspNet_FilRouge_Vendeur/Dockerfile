FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore AspNet_FilRouge_Vendeur.csproj
RUN dotnet publish AspNet_FilRouge_Vendeur.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5101

ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]