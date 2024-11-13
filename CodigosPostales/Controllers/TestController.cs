using Microsoft.AspNetCore.Mvc;

namespace CodigosPostales_net.Controllers
{
    /// <summary>
    /// Controller para pruebas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestsController> logger;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TestsController(IConfiguration configuration, ILogger<TestsController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Obtiene el ambiente que tiene registrado en el appSettings
        /// </summary>
        /// <returns></returns>
        [HttpGet("Ambiente")]
        public IActionResult Get()
        {
            string ambiente = configuration.GetSection("Ambiente").Value;
            logger.LogInformation($"Ambiente: {ambiente}");

            return Ok(new { Ambiente = ambiente });
        }

        /// <summary>
        /// Prueba para registrar errores
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpDelete("Errores")]
        public IActionResult Delete(string error)
        {
            if (string.IsNullOrEmpty(error))
                error = Guid.NewGuid().ToString();
            throw new Exception(error);
        }
        /// <summary>
        /// Hola mundo
        /// </summary>
        /// <returns></returns>
        [HttpGet("HolaMundo")]
        public IActionResult HolaMundo(string saludo)
        {
            if (string.IsNullOrEmpty(saludo))
                saludo = "Hola mundo";

            return Ok(new { Mensaje = saludo, Fecha = DateTime.Now, Id = Guid.NewGuid() });
        }
    }
}