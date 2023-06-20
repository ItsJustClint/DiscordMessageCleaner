FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DMCBot/DMCBot.csproj", "DMCBot/"]
RUN dotnet restore "DMCBot/DMCBot.csproj"
COPY . .
WORKDIR "/src/DMCBot"
RUN dotnet build "DMCBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DMCBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DMCBot.dll"]
