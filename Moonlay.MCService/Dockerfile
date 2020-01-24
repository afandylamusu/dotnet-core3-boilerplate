#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Moonlay.MCService/Moonlay.MasterData.OpenApi.csproj", "Moonlay.MCService/"]
COPY ["Moonlay.MCService/nuget.config", "Moonlay.MCService/"]
COPY ["Moonlay.Core.Models/Moonlay.Core.Models.csproj", "Moonlay.Core.Models/"]
COPY ["Moonlay.Topics/Moonlay.Topics.csproj", "Moonlay.Topics/"]
COPY ["Moonlay.Confluent.Kafka/Moonlay.Confluent.Kafka.csproj", "Moonlay.Confluent.Kafka/"]
RUN dotnet restore "Moonlay.MCService/Moonlay.MasterData.OpenApi.csproj"
COPY . .
WORKDIR "/src/Moonlay.MCService"
RUN dotnet build "Moonlay.MasterData.OpenApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Moonlay.MasterData.OpenApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Moonlay.MasterData.OpenApi.dll"]