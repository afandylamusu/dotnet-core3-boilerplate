#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Moonlay.WebApp/Moonlay.WebApp.csproj", "Moonlay.WebApp/"]
COPY ["Moonlay.Core.Models/Moonlay.Core.Models.csproj", "Moonlay.Core.Models/"]
COPY ["Moonlay.Topics/Moonlay.Topics.csproj", "Moonlay.Topics/"]
COPY ["Moonlay.Confluent.Kafka/Moonlay.Confluent.Kafka.csproj", "Moonlay.Confluent.Kafka/"]
RUN dotnet restore "Moonlay.WebApp/Moonlay.WebApp.csproj"
COPY . .
WORKDIR "/src/Moonlay.WebApp"
RUN dotnet build "Moonlay.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Moonlay.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Moonlay.WebApp.dll"]