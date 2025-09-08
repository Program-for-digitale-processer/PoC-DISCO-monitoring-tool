FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PoC-DISCO-Backend/PoC-DISCO-Backend.csproj", "PoC-DISCO-Backend/"]
RUN dotnet restore "PoC-DISCO-Backend/PoC-DISCO-Backend.csproj"
COPY . .
WORKDIR "/src/PoC-DISCO-Backend"
RUN dotnet build "./PoC-DISCO-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PoC-DISCO-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PoC-DISCO-Backend.dll"]
