FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["WifiWarriorAPI/WifiWarriorAPI.csproj", "WifiWarriorAPI/"]
RUN dotnet restore "WifiWarriorAPI/WifiWarriorAPI.csproj"
COPY . .
WORKDIR "/src/WifiWarriorAPI"
RUN dotnet build "WifiWarriorAPI.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "WifiWarriorAPI.csproj" -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WifiWarriorAPI.dll"]
