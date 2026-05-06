FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

USER $APP_UID
EXPOSE 8080
EXPOSE 8081

ENV DOTNET_gcServer=1 \
    DOTNET_ThreadPool_MinThreads=100 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_ENVIRONMENT=Production

USER root
RUN apk add --no-cache icu-libs krb5-libs
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && chmod -R 777 /home/app/.aspnet
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

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

COPY src/ ./src/
COPY Core/ ./Core/
WORKDIR /src/src/Api
RUN dotnet build "Api.csproj" --configuration $BUILD_CONFIGURATION --no-restore --output /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Api.csproj" --configuration $BUILD_CONFIGURATION --no-restore --output /app/publish /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Azoxia.AdaIsAkademi.Api.dll"]