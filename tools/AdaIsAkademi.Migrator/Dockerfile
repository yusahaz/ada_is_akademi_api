FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine
WORKDIR /

COPY src/ /src/
COPY Core/ /Core/

RUN dotnet tool install --global dotnet-ef --version 10.*
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet restore /src/Api/Api.csproj --force
RUN dotnet restore /src/Persistence/Persistence.csproj --force

WORKDIR /src
ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "Persistence/Persistence.csproj", "--startup-project", "Api/Api.csproj"]
