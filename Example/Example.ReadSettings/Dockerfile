FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Example/Example.ReadSettings/Example.ReadSettings.csproj", "Example/Example.ReadSettings/"]
RUN dotnet restore "Example/Example.ReadSettings/Example.ReadSettings.csproj"
COPY . .
WORKDIR "/src/Example/Example.ReadSettings"
RUN dotnet build "Example.ReadSettings.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Example.ReadSettings.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Example.ReadSettings.dll"]
