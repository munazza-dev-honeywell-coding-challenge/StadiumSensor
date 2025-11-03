using System;
using StadiumSensor.Core.Models;
using StadiumSensor.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using StadiumSensor.Infrastructure.Repos;
using StadiumSensor.Infrastructure.Services;
using Serilog;
using System.Threading.Channels;
using StadiumSensor.Core.Models.AppSettings;
using StadiumSensor.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<AzureServiceBus>(builder.Configuration.GetSection("ServiceBus"));
builder.Services.AddSwaggerGen();

// Add SQLite
builder.Services.AddDbContext<SensorEventDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=sensor_events.db"));

var channel = Channel.CreateUnbounded<SensorEvent>();
builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);

// Add repositories and services
builder.Services.AddScoped<ISensorEventRepo, SensorEventRepo>();
builder.Services.AddHostedService<EventSimulationService>();
builder.Services.AddHostedService<EventConsumerService>();

// following service for Azure service bus would be enabled for integration of Azure service bus for real time event payload capturing, currently excluded to allow simulation for event generation for development and testing locally.

//builder.Services.AddHostedService<SensorEventBusConsumerService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SensorEventDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
