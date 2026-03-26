# Pour l'étape build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AspNet_FilRouge/AspNet_FilRouge.csproj", "AspNet_FilRouge/"]
RUN dotnet restore "AspNet_FilRouge/AspNet_FilRouge.csproj"
COPY . .
WORKDIR "/src/AspNet_FilRouge"
RUN dotnet build "AspNet_FilRouge.csproj" -c Release -o /app/build
RUN dotnet publish "AspNet_FilRouge.csproj" -c Release -o /app/publish

# Pour l’étape runtime (si tu veux une image plus petite pour exécution)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]
