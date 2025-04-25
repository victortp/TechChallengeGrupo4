using ContatosGrupo4.Application.Extensions;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.InfraStructure.Extensions;
using Prometheus;
using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Infrastructure.Messaging.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddUseCases();
builder.Services.AddOptions<RabbitMQOptions>().BindConfiguration("RabbitMQ");

builder.Services.AddHostedService<ContatoConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHttpMetrics();
app.MapMetrics();

app.Run();

[ExcludeFromCodeCoverage] public partial class Program { }
