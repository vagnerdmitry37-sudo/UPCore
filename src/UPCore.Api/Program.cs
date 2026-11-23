using System.Text.Json.Serialization;

using UPCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
