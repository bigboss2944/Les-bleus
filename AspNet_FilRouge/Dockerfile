FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src

COPY FilRouge.sln ./
COPY Entities/ Entities/
COPY AspNet_FilRouge/ AspNet_FilRouge/

WORKDIR /src/AspNet_FilRouge
RUN dotnet restore AspNet_FilRouge.csproj -r linux-arm
RUN dotnet publish AspNet_FilRouge.csproj -c Release -r linux-arm --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -o /app/publish

FROM arm32v7/debian:bookworm-slim
WORKDIR /app

RUN apt-get update \
	&& apt-get install -y --no-install-recommends ca-certificates libicu72 libssl3 tzdata \
	&& rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://0.0.0.0:5100
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish ./

EXPOSE 5100
ENTRYPOINT ["./AspNet_FilRouge"]