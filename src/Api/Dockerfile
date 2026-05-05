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

COPY ["AdaIsAkademi/src/Api/Api.csproj", "AdaIsAkademi/src/Api/"]
COPY ["AdaIsAkademi/src/Application/Application.csproj", "AdaIsAkademi/src/Application/"]
COPY ["AdaIsAkademi/src/Domain/Domain.csproj", "AdaIsAkademi/src/Domain/"]
COPY ["AdaIsAkademi/src/Infrastructure/Infrastructure.csproj", "AdaIsAkademi/src/Infrastructure/"]
COPY ["AdaIsAkademi/src/Persistence/Persistence.csproj", "AdaIsAkademi/src/Persistence/"]
COPY ["Core/src/Api/Api.csproj", "Core/src/Api/"]
COPY ["Core/src/Application/Application.csproj", "Core/src/Application/"]
COPY ["Core/src/Core/Core.csproj", "Core/src/Core/"]
COPY ["Core/src/Infrastructure/Infrastructure.csproj", "Core/src/Infrastructure/"]
COPY ["Core/src/Persistence/Persistence.csproj", "Core/src/Persistence/"]
COPY ["Core/src/Generators/Generators.csproj", "Core/src/Generators/"]

RUN dotnet restore "AdaIsAkademi/src/Api/Api.csproj"

COPY AdaIsAkademi/ ./AdaIsAkademi/
COPY Core/ ./Core/
WORKDIR /src/AdaIsAkademi/src/Api
RUN dotnet build "Api.csproj" --configuration $BUILD_CONFIGURATION --no-restore --output /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Api.csproj" --configuration $BUILD_CONFIGURATION --no-restore --output /app/publish /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Azoxia.AdaIsAkademi.Api.dll"]