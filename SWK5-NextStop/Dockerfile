FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SWK5-NextStop/SWK5-NextStop.csproj", "SWK5-NextStop/"]
RUN dotnet restore "SWK5-NextStop/SWK5-NextStop.csproj"
COPY . .
WORKDIR "/src/SWK5-NextStop"
RUN dotnet build "SWK5-NextStop.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SWK5-NextStop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SWK5-NextStop.dll"]
