# LinkShopHub-Template
Minimal SaaS starter: .NET 9 + MudBlazor + Postgres.

## Dev quick-start

```bash
docker compose up -d   # Postgres + pgAdmin
dotnet restore
dotnet run --project src/LinkShopHub.Api
