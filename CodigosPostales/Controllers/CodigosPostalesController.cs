using System.ComponentModel.DataAnnotations;
using CodigosPostales_net.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CodigosPostales_net.Controllers
{
    /// <summary>
    /// Contralador de codigos postales
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CodigosPostalesController : ControllerBase
    {
        private readonly IRepositorio _repositorio;

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="repositorio"></param>
        public CodigosPostalesController(IRepositorio repositorio)
        {
            _repositorio = repositorio;
        }       

        /// <summary>
        /// Códigos por estado y alcaldia
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <returns>Codigos postales</returns>
        [HttpGet("Estados/{estado}/Alcaldias/{alcaldia}")]
        [ProducesResponseType(typeof(List<CodigoPostalDto>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetZipCodesByMunicipality(string estado, string alcaldia)
        {
            var lista = (await _repositorio.ObtenerCodigosPostalesAsync(estado, alcaldia))
            .Select(x => new CodigoPostalDto
            {
                CodigoPostal = x.CodigoPostal,
                AlcaldiaId = x.AlcaldiaId,
                Estado = x.Estado,
                EstadoId = x.EstadoId,
                Alcaldia = x.Alcaldia,
                TipoDeAsentamiento = x.TipoDeAsentamiento,
                Asentamiento = x.Asentamiento
            })
            .ToList();

            HttpContext.Response.Headers.Append("Total", lista.Count.ToString());
            return Ok(lista);
        }

        /// <summary>
        /// Obtener la lista de codigos postales
        /// </summary>        
        /// <returns></returns>         
        [HttpGet("{codigoPostal}")]
        [ProducesResponseType(typeof(List<CodigoPostalDto>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ObtenerCodigosPostales([StringLength(5, MinimumLength = 5)] string codigoPostal)
        {
            var lista = (await _repositorio.ObtenerCodigosPostalesAsync(codigoPostal))
            .Select(x => new
            {
                x.CodigoPostal,
                x.AlcaldiaId,
                x.Estado,
                x.EstadoId,
                x.Alcaldia,
                x.TipoDeAsentamiento,
                x.Asentamiento
            })
            .ToList();

            HttpContext.Response.Headers.Append("Total", lista.Count.ToString());
            return Ok(lista);
        }

        /// <summary>
        /// Obtener los codigos psotales a partir del nombre de una colonia
        /// </summary>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        [HttpGet("{asentamiento}/Buscar")]
        [ProducesResponseType(typeof(List<CodigoPostalDto>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ObtenerCodigosPostalesPorAsentamientamiento(string asentamiento)
        {
            var lista = (await _repositorio.ObtenerCodigosPostalesPorAsentamientoAsync(asentamiento))
            .Select(x => new
            {
                x.CodigoPostal,
                x.AlcaldiaId,
                x.Estado,
                x.EstadoId,
                x.Alcaldia,
                x.TipoDeAsentamiento,
                x.Asentamiento
            })
            .ToList();

            HttpContext.Response.Headers.Append("Total", lista.Count.ToString());
            return Ok(lista);
        }

        /// <summary>
        /// Obtener los codigos psotales a partir del nombre de una colonia
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        [HttpGet("Estados/{estado}/Alcaldias/{alcaldia}/{asentamiento}/Buscar")]
        [ProducesResponseType(typeof(List<CodigoPostalDto>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetZipCodesByMunicipality(string estado, string alcaldia, string asentamiento)
        {
            var lista = (await _repositorio.ObtenerCodigosPostalesAsync(estado, alcaldia, asentamiento))
            .Select(x => new
            {
                x.CodigoPostal,
                x.AlcaldiaId,
                x.Estado,
                x.EstadoId,
                x.Alcaldia,
                x.TipoDeAsentamiento,
                x.Asentamiento
            })
            .ToList();

            HttpContext.Response.Headers.Append("Total", lista.Count.ToString());
            return Ok(lista);
        }

        /// <summary>
        /// Obtiene un codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        [HttpGet("Aleatorio")]
        [ProducesResponseType(typeof(CodigoPostalDto), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ObtenerCodigoPostalAleatorio()
        {
            CodigoPostalEntidad x;

            x = await _repositorio.ObtenerCodigoPostalAleatorioAsync();

            return Ok(new
            {
                x.CodigoPostal,
                x.AlcaldiaId,
                x.Estado,
                x.EstadoId,
                x.Alcaldia,
                x.TipoDeAsentamiento,
                x.Asentamiento
            });
        }

        /// <summary>
        /// Obtiene un codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        [HttpGet("Estados/{estado}/Aleatorio")]
        [ProducesResponseType(typeof(CodigoPostalDto), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ObtenerCodigoPostalAleatorioPorEstadoAsync(string estado)
        {
            int estadoId;

            if (int.TryParse(estado, out estadoId))
            {
                if (estadoId > 0 && estadoId >= 33)
                {
                    return BadRequest(new { Menseje = "El estadoId debe de ser de 1 a 32" });
                }

            }
            CodigoPostalEntidad x;

            x = await _repositorio.ObtenerCodigoPostalAleatorioAsync(estado);

            return Ok(new
            {
                x.CodigoPostal,
                x.AlcaldiaId,
                x.Estado,
                x.EstadoId,
                x.Alcaldia,
                x.TipoDeAsentamiento,
                x.Asentamiento
            });
        }

        /// <summary>
        /// Subir coleccion de codigos postales
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgregarCodigosPostalesAsync([Required] IFormFile formFile)
        {
            string[] lines;
            List<CodigoPostalEntidad> codigos = new List<CodigoPostalEntidad>();

            var fechaInicial = DateTime.Now;
            var fechaFinal = DateTime.Now;
            StreamReader reader = new StreamReader(formFile.OpenReadStream(), System.Text.Encoding.Latin1);
            string text = reader.ReadToEnd();
            lines = text.Split("\n");
            // d_codigo|d_asenta   |d_tipo_asenta|D_mnpio         |d_estado         |d_ciudad           | d_CP  | c_estado  |  c_oficina|c_CP|c_tipo_asenta|c_mnpio |id_asenta_cpcons|d_zona|c_cve_ciudad
            // 01000   | San Ángel | Colonia     | Álvaro Obregón | Ciudad de México| Ciudad de México  | 01001 | 09        |  01001    |    | 09          | 010    |0001            |Urbano|01
            // 0       | 1         | 2           | 3              | 4               | 5                 | 6     | 7         | 8         | 9  | 10          | 11     | 12             | 13   | 14
            for (int i = 2; i < lines.Count(); i++)
            {
                string[] array;
                CodigoPostalEntidad codigoPostalEntity;

                array = lines[i].Split("|");
                if (array.Length > 10)
                {
                    codigoPostalEntity = new CodigoPostalEntidad
                    {
                        CodigoPostal = array[0],
                        Asentamiento = array[1],
                        TipoDeAsentamiento = array[2],
                        Alcaldia = array[3],
                        Estado = array[4],
                        EstadoId = Convert.ToInt32(array[7]),
                        AlcaldiaId = Convert.ToInt32(array[11]),
                    };
                    codigos.Add(codigoPostalEntity);
                }
            }
            await _repositorio.BorrarAsync();
            await _repositorio.AgregarAsynx(codigos);
            fechaFinal = DateTime.Now;

            return Accepted(new { fechaInicial, fechaFinal, TotalDeRegistros = lines.Count(), Tiempo = (fechaInicial - fechaFinal).TotalSeconds });
        }

    }//end class
}