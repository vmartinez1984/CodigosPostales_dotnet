using CodigosPostales.ReglasDeNegocio;
using CodigosPostales_net;
using Microsoft.OpenApi.Models;
using SoapCore;
using System.Reflection;
using VMtz84.Logger.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSoapCore();

builder.Services.AddSingleton<IRepositorio, RepositorioMongoDb>();

builder.Services.AgregarReglasDeNegocio();

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

builder.Services.AddCors(options =>  options.AddPolicy("AllowWebApp", 
    builder => builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()));

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
app.UseSoapEndpoint<ICodigoPostalRdn>("/CodigosPostales.asmx", new SoapEncoderOptions());
//app.UseRouting();
//app.UseEndpoints(endpoints => {
//    endpoints.UseSoapEndpoint<ICodigoPostalRdn>(opt =>
//    {
//        opt.Path = "/CodigosPostales.asmx";
//        opt.SoapSerializer = SoapSerializer.DataContractSerializer;        
//    });
//});


app.MapControllers();

app.Run();