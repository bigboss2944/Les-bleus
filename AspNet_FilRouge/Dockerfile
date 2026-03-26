FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build


# Copie toutes les sources nécessaires au build (attention : ne build QU'UN projet à la fois !)
COPY Database/ Database/
COPY Entities/ Entities/
COPY AspNet_FilRouge/ ./ 

WORKDIR /src/AspNet_FilRouge
RUN ls -l /src/AspNet_FilRouge
RUN dotnet restore /src/AspNet_FilRouge/AspNet_FilRouge.csproj
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5100
ENV ASPNETCORE_URLS=http://0.0.0.0:5100
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]