# Sati - Stone Production Tracker

ASP.NET Core MVC application that tracks stone and granite slabs through their production steps.

## What it holds
- `Sati/Controllers/StoneProductionController.cs` - create a production record, search by stone name, update each step's status, edit a record
- `Sati/Models/` - `Stone`, production steps, per-step status with start and end times, EF Core context
- `Sati/Views/StoneProduction/` - Razor views for creating records and showing step progress

## Stack
C#, ASP.NET Core MVC (.NET 8), Entity Framework Core, SQL Server, Razor

## Run it
```bash
cd Sati
dotnet run
```
Set your own SQL Server connection string in `appsettings.Development.json`.
