# Étape build : SDK .NET 10 (image tierce Hardened Images)
FROM hardenedimages/dotnet/debian-13/10-sdk@sha256:6eb6853793f14f52bea7efe2fd0762dda8b78d8cdd7b8d732545c0d811306a1e AS build

WORKDIR /src

# Copie seulement le projet pour le restore (plus de cache Docker)
COPY ["AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj", "AspNet_FilRouge_Vendeur/"]

RUN dotnet restore "AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj"

# Copie le reste des fichiers
COPY . .

WORKDIR "/src/AspNet_FilRouge_Vendeur"
RUN dotnet build "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/build
RUN dotnet publish "AspNet_FilRouge_Vendeur.csproj" -c Release -o /app/publish

# Étape runtime : ASP.NET 10 (image tierce Hardened Images)
FROM hardenedimages/dotnet/debian-13/10-aspnet@sha256:3e418cf6e93fbb8c344052868e8915f4c432cf6496658fb80edefcd4d1c3bf2b AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNet_FilRouge_Vendeur.dll"]