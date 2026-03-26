# Pour l'étape build (SDK)
FROM hardenedimages/dotnet/debian-13/10-sdk@sha256:6eb6853793f14f52bea7efe2fd0762dda8b78d8cdd7b8d732545c0d811306a1e AS build
WORKDIR /src
COPY ["AspNet_FilRouge/AspNet_FilRouge.csproj", "AspNet_FilRouge/"]
RUN dotnet restore "AspNet_FilRouge/AspNet_FilRouge.csproj"
COPY . .
WORKDIR "/src/AspNet_FilRouge"
RUN dotnet build "AspNet_FilRouge.csproj" -c Release -o /app/build
RUN dotnet publish "AspNet_FilRouge.csproj" -c Release -o /app/publish

# Pour l’étape runtime (si tu veux une image plus petite pour exécution)
FROM hardenedimages/dotnet/debian-13/10-aspnet@sha256:3e418cf6e93fbb8c344052868e8915f4c432cf6496658fb80edefcd4d1c3bf2b AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNet_FilRouge.dll"]
