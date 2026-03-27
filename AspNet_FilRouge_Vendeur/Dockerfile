FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copier toute la solution (IMPORTANT)
COPY . .

# Aller dans le projet vendeur
WORKDIR /src/AspNet_FilRouge_Vendeur

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5100
ENV ASPNETCORE_URLS=http://0.0.0.0:5100

ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]