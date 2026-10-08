using ItsmAi.Application.Incidents.Create;
using ItsmAi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<CreateIncidentHandler>();
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();


