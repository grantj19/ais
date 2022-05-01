using System.Reflection;
using AISapi.BA;
using AISapi.BA.Interfaces;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<VesselBA>();
builder.Services.AddScoped<IAISMessageBA, AISMessageBA>();
builder.Services.AddScoped<PositionReportBA>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AIS Api",
        Version = "v1",
        Description = "An API to perform AISMessage and Vessel operations."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

