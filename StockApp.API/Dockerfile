FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ConnectionStrings__Default="User ID=postgres;Password=root;Host=localhost;Port=5432;Database=StockAppDataBase;"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StockApp.API/StockApp.API.csproj", "StockApp.API/"]
COPY ["StockApp.Models/StockApp.Models.csproj", "StockApp.Models/"]
RUN dotnet restore "StockApp.API/StockApp.API.csproj"
COPY . .
WORKDIR "/src/StockApp.API"
RUN dotnet build "StockApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StockApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockApp.API.dll"]
