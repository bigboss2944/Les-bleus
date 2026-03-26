FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build


# Copie toutes les sources nécessaires au build (attention : ne build QU'UN projet à la fois !)
COPY Database/ Database/
COPY Entities/ Entities/
COPY AspNet_FilRouge_Vendeur/ ./      

WORKDIR /src/AspNet_FilRouge_Vendeur
RUN ls -l /src/AspNet_FilRouge_Vendeur
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5101
ENV ASPNETCORE_URLS=http://0.0.0.0:5101
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]