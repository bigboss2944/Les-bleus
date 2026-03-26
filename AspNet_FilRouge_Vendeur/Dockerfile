# Dockerfile pour AspNet_FilRouge_Vendeur (ARM/ARM64)
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-bullseye-slim AS build
WORKDIR /src
COPY ["AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj", "AspNet_FilRouge_Vendeur/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj"
COPY . .
WORKDIR "/src/AspNet_FilRouge_Vendeur"
RUN dotnet publish "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]
