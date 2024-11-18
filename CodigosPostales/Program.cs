using CodigosPostales_net;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Debugging;
using System.Reflection;
using VMtz84.Logger.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configura Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Lee configuración desde appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Reemplaza el logger predeterminado por Serilog
builder.Host.UseSerilog();
//Muestra el error de serilog
SelfLog.Enable(Console.Error);

//builder.Services.AddScoped<AppDbContext>();
//builder.Services.AddScoped<IRepositorio, RepositorioSql>();

builder.Services.AddScoped<IRepositorio, RepositorioMongoDb>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v3",
        Title = "Api de Código Postales",
        Description = @"Es un api de los códigos postales de SEPOMEX",
        Contact = new OpenApiContact
        {
            Name = "Víctor Martínez",
            Url = new Uri("mailto:ahal_tocob@hotmail.com")
        }
    });
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(options => options.AddPolicy("AllowWebApp", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "");
    c.RoutePrefix = "";
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowWebApp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();