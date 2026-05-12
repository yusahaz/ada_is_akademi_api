FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

USER $APP_UID
EXPOSE 8080
EXPOSE 8081

ENV DOTNET_gcServer=1 \
    DOTNET_ThreadPool_MinThreads=100 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080

USER root
RUN apk add --no-cache icu-libs krb5-libs curl
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && chmod -R 777 /home/app/.aspnet
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Persistence/Persistence.csproj", "src/Persistence/"]
COPY ["Core/src/Api/Api.csproj", "Core/src/Api/"]
COPY ["Core/src/Application/Application.csproj", "Core/src/Application/"]
COPY ["Core/src/Core/Core.csproj", "Core/src/Core/"]
COPY ["Core/src/Infrastructure/Infrastructure.csproj", "Core/src/Infrastructure/"]
COPY ["Core/src/Persistence/Persistence.csproj", "Core/src/Persistence/"]
COPY ["Core/src/Generators/Generators.csproj", "Core/src/Generators/"]

RUN dotnet restore "src/Api/Api.csproj"

COPY src/ /src/
COPY Core/ /Core/
WORKDIR /src/Api
RUN dotnet publish "Api.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false \
    /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
ARG BUILD_VERSION=dev

WORKDIR /app

LABEL org.opencontainers.image.title="Ada Is Akademi API" \
    org.opencontainers.image.description="Azoxia AdaIsAkademi HTTP API" \
    org.opencontainers.image.version="${BUILD_VERSION}"

COPY --from=build /app/publish .

HEALTHCHECK --interval=15s --timeout=5s --start-period=40s --retries=5 \
    CMD curl -fsS http://127.0.0.1:8080/health >/dev/null || exit 1

ENTRYPOINT ["dotnet", "Azoxia.AdaIsAkademi.Api.dll"]
