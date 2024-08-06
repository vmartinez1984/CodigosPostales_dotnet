using CodigosPostales_net;
using Microsoft.OpenApi.Models;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.Configure<DataSettingMongoDb>(
//     builder.Configuration.GetSection("DataSettingMongoDb")
// );
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<RepositorioMongoDb>();
builder.Services.Configure<DataSettingMongoDb>(
    builder.Configuration.GetSection("DataSettingMongoDb")
);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.1",
        Title = "Api de museos de la CDMX",
        Description = @"Es una ApiRest que muestra información de museos de la ciudad de México, con el objetivo 
        de probar los workerservices.
        probando el scraping web para obtener de detalles de costos, información adicional que no esta en el json.
        <br/>La información fue publicada por el Sistema de información de Cultura/Secretaría de cultura 
        Consultado en https://sic.cultura.gob.mx/opendata/d/9_museo_directorio.json el 10 de agosto de 2022",
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

// builder.Services.AddRepositoryCodigosPostalesSql();
// builder.Services.AddCodigosPostalesMappers();
// builder.Services.AddCodigosPostalesBusinessLayer();

builder.Services.AddCors(options => options.AddPolicy("AllowWebApp", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowWebApp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();