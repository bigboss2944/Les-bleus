# Dockerfile pour AspNet_FilRouge (ARM/ARM64)
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-bullseye-slim AS build
WORKDIR /src
COPY ["AspNet_FilRouge/AspNet_FilRouge.csproj", "AspNet_FilRouge/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "AspNet_FilRouge/AspNet_FilRouge.csproj"
COPY . .
WORKDIR "/src/AspNet_FilRouge"
RUN dotnet publish "AspNet_FilRouge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]
