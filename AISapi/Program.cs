using AISapi.BA;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<VesselBA>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();

