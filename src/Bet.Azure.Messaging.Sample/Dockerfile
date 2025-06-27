FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["build/", "build/"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Build.targets", "Directory.Build.targets"]

COPY ["src/Bet.Azure.Messaging.Sample/Bet.Azure.Messaging.Sample.csproj", "src/Bet.Azure.Messaging.Sample/"]
RUN dotnet restore "src/Bet.Azure.Messaging.Sample/Bet.Azure.Messaging.Sample.csproj"
COPY . .

WORKDIR "/src/src/Bet.Azure.Messaging.Sample"
RUN dotnet build "Bet.Azure.Messaging.Sample.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Bet.Azure.Messaging.Sample.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bet.Azure.Messaging.Sample.dll"]
