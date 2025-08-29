FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/LinkShopHub.Web/LinkShopHub.Web.csproj", "src/LinkShopHub.Web/"]
RUN dotnet restore "src/LinkShopHub.Web/LinkShopHub.Web.csproj"

COPY . .
WORKDIR "/src/src/LinkShopHub.Web"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkShopHub.Web.dll"]