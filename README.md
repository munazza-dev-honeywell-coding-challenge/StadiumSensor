# StadiumSensor.API

This project runs the Stadium Sensor Web API (ASP.NET Core, .NET 8).

Prerequisites
- .NET 8 SDK installed

## Local Development
1. Build:
   dotnet build

2. Run API:
   dotnet run --project src\StadiumSensor.API

3. Open Swagger:
   https://localhost:5001/swagger

Notes
- The app uses SQLite at `sensor_events.db` in the API project folder by default.


### Configure Azure Service bus Secrets

- To switch to Azure Service Bus intigration for real event capturing and consumption (no simulation):
  - Add `Azure.Messaging.ServiceBus` to `StadiumSensor.Infrastructure`. (currently disabled/commented)
  - Replace or disable `EventSimulationService` and register a `SensorEventBusConsumerService`.(currently disabled/commented)
  - Configure ServiceBus connection in `appsettings.json` under `ServiceBus:ConnectionString` and `ServiceBus:QueueName`.(currently disabled/commented)
  To manage the User Secrets, just right click the API project in Visual Studio and select 'Manage User Secrets'. and aad secrets.
	- for deployment screts would be managed in Azure Key vault referenced in build pipelines (can be explained in detail while interview)
	- for local testing only default connection for SQLite is used.
Troubleshooting
- If database not created, ensure `db.Database.EnsureCreatedAsync()` is executed on Program.cs.

Time spent: around 5- 5.5 hours