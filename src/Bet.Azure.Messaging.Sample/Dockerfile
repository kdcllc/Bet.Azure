#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["bet.azure.messaging.sample/bet.azure.messaging.sample.csproj", "bet.azure.messaging.sample/"]
RUN dotnet restore "bet.azure.messaging.sample/bet.azure.messaging.sample.csproj"
COPY . .
WORKDIR "/src/bet.azure.messaging.sample"
RUN dotnet build "bet.azure.messaging.sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bet.azure.messaging.sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bet.azure.messaging.sample.dll"]