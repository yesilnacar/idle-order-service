FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY IdleOrderService.sln ./
COPY src/ ./src/

RUN dotnet restore IdleOrderService.sln
RUN dotnet build IdleOrderService.sln -c Release

RUN dotnet publish "src/IdleOrderService.Api/IdleOrderService.Api.csproj" \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5777

ENTRYPOINT ["dotnet", "IdleOrderService.Api.dll"]