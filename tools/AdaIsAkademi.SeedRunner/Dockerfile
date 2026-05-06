FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /

COPY src/ /src/
COPY Core/ /Core/
COPY tools/AdaIsAkademi.SeedRunner/ /tools/AdaIsAkademi.SeedRunner/

WORKDIR /tools/AdaIsAkademi.SeedRunner
RUN dotnet restore "AdaIsAkademi.SeedRunner.csproj"
RUN dotnet publish "AdaIsAkademi.SeedRunner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Azoxia.AdaIsAkademi.SeedRunner.dll"]
