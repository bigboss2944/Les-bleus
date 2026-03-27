FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore /src/FilRouge.sln
RUN dotnet publish /src/AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5101
ENV ASPNETCORE_URLS=http://0.0.0.0:5101

ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]