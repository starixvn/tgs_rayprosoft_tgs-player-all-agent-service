# 請參閱 https://aka.ms/customizecontainer 了解如何自訂您的偵錯容器，以及 Visual Studio 如何使用此 Dockerfile 來組建您的映像，以加快偵錯速度。

# 此階段用於以快速模式從 VS 執行時 (偵錯設定的預設值)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# 此階段是用來組建服務專案
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Player.All.Agent.Service/Tgs.Player.All.Agent.Service.csproj", "Tgs.Player.All.Agent.Service/"]
COPY ["Tgs.Player.All.Agent.Core/Tgs.Player.All.Agent.Core.csproj", "Tgs.Player.All.Agent.Core/"]
COPY ["Tgs.Player.All.Agent.Domain/Tgs.Player.All.Agent.Domain.csproj", "Tgs.Player.All.Agent.Domain/"]
COPY ["Tgs.Player.All.Agent.Repository/Tgs.Player.All.Agent.Repository.csproj", "Tgs.Player.All.Agent.Repository/"]
RUN dotnet restore "./Tgs.Player.All.Agent.Service/Tgs.Player.All.Agent.Service.csproj"
COPY . .
WORKDIR "/src/Tgs.Player.All.Agent.Service"
RUN dotnet build "./Tgs.Player.All.Agent.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此階段可用來發佈要複製到最終階段的服務專案
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.Player.All.Agent.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此階段用於生產環境，或以一般模式從 VS 執行時 (未使用偵錯設定時的預設值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Player.All.Agent.Service.dll"]