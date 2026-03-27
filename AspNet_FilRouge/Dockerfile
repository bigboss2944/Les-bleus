FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .
RUN ls -R /src
WORKDIR /src/AspNet_FilRouge

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM --platform=linux/arm/v7 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5101
ENV ASPNETCORE_URLS=http://0.0.0.0:5101

ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]