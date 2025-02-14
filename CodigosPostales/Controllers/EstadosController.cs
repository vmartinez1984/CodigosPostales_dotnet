using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodigosPostales_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosController : ControllerBase
    {
        private readonly IRepositorio _repositorio;

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="repositorio"></param>
        public EstadosController(IRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        /// <summary>
        /// Lista de estados
        /// </summary>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet(Name = "Estados")]
        [ProducesResponseType(typeof(List<Estado>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ObtenerEstadosAsync()
        {
            var lista = await _repositorio.ObtenerEstadosASync();

            return Ok(lista.OrderBy(x => x.Nombre));
        }

        /// <summary>
        /// Lista de municipios por estado
        /// </summary>
        /// <param name="estado"></param>        
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet("{estado}/Alcaldias")]
        [ProducesResponseType(typeof(List<Alcaldia>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ObtenerAlcaldias(string estado)
        {
            var lista = await _repositorio.ObtenerAlcaldiasAsync(estado);
            HttpContext.Response.Headers.Append("Total", lista.Count.ToString());

            return Ok(lista);
        }
    }
}
