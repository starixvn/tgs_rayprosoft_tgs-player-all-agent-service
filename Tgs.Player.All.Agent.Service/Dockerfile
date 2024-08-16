#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Player.All.Agent.Service/Tgs.Player.All.Agent.Service.csproj", "Tgs.Player.All.Agent.Service/"]
COPY ["Tgs.Player.All.Agent.Core/Tgs.Player.All.Agent.Core.csproj", "Tgs.Player.All.Agent.Core/"]
COPY ["Tgs.Player.All.Agent.Domain/Tgs.Player.All.Agent.Domain.csproj", "Tgs.Player.All.Agent.Domain/"]
COPY ["Tgs.Player.All.Agent.Repository/Tgs.Player.All.Agent.Repository.csproj", "Tgs.Player.All.Agent.Repository/"]
RUN dotnet restore "Tgs.Player.All.Agent.Service/Tgs.Player.All.Agent.Service.csproj"
COPY . .
WORKDIR "/src/Tgs.Player.All.Agent.Service"
RUN dotnet build "Tgs.Player.All.Agent.Service.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Player.All.Agent.Service.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Player.All.Agent.Service.dll"]