using Distribuidora.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Repositories and Services
// We will update these to be Scoped/Transient as we refactor them
builder.Services.AddScoped<IRepositorioProdutos, RepositorioProdutos>();
// builder.Services.AddScoped<DistribuidoraService>(); // This service is stateful and needs refactoring, skipping for now

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // For serving the frontend

app.UseAuthorization();

app.MapControllers();

// Fallback to index.html for SPA-like behavior if needed, or just default to static files
app.MapFallbackToFile("index.html");

app.Run();