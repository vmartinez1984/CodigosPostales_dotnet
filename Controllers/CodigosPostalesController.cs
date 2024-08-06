using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodigosPostales_net.Controllers
{
    /// <summary>
    /// Contralador de codigos postales
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CodigosPostalesController : ControllerBase
    {
        // private readonly ILogger<CodigosPostalesController> _logger;        
        // private readonly IServiceScopeFactory _scopeFactory;
        private readonly AppDbContext _appDbContext;
        private readonly RepositorioMongoDb _repositorioMongoDb;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appDbContext"></param>
        public CodigosPostalesController(
            //ILogger<CodigosPostalesController> logger,            
            //IServiceScopeFactory scopeFactory,
            AppDbContext appDbContext,
            RepositorioMongoDb repositorioMongoDb
            )
        {
            //_logger = logger;            
            //_scopeFactory = scopeFactory;
            _appDbContext = appDbContext;
            _repositorioMongoDb = repositorioMongoDb;
        }

        /// <summary>
        /// Lista de estados
        /// </summary>
        /// <returns></returns>
        [HttpGet("Estados", Name = "Estados")]
        public async Task<IActionResult> GetStates()
        {
            var lista = await _appDbContext.CodigoPostal
            .Select(x => new
            {
                Nombre = x.Estado,
                Id = x.EstadoId
            }).Distinct()
            .OrderBy(x => x.Id)
            .ToListAsync();

            return Ok(lista);
        }

        /// <summary>
        /// Lista de municipios por estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        [HttpGet("Estados/{estado}/Alcaldias")]
        public async Task<IActionResult> ObtenerAlcaldias(string estado)
        {
            int estadoId;
            IQueryable<CodigoPostalEntidad> queryable;

            if (int.TryParse(estado, out estadoId))
                queryable = _appDbContext.CodigoPostal.Where(item => item.EstadoId == estadoId);
            else
                queryable = _appDbContext.CodigoPostal.Where(item => item.Estado == estado);

            var list = await queryable.Select(x => new
            {
                x.Alcaldia,
                Id = x.AlcaldiaId
            })
              .Distinct()
              .OrderBy(x => x.Id)
              .ToListAsync();

            return Ok(list);
        }

        // [HttpGet("Estados/{estado}/Alcaldias/{alcaldia}")]
        // public async Task<IActionResult> GetZipCodesByMunicipality(string estado, string alcaldia)
        // {
        //     List<CodigoPostalDto> list;

        //     list = await _unitOfWorkBl.CodigoPostal.GetZipCodesByMunicipalityAsync(estado, alcaldia);

        //     return Ok(list);
        // }

        /// <summary>
        /// Obtener la lista de codigos postales
        /// </summary>        
        /// <returns></returns>
        [HttpGet("{codigoPostal}")]
        public async Task<IActionResult> ObtenerCodigosPostales([StringLength(5, MinimumLength = 5)] string codigoPostal)
        {
            var list = await _appDbContext.CodigoPostal
            .Where(item => item.CodigoPostal == codigoPostal)
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
            .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Obtener los codigos psotales a partir del nombre de una colonia
        /// </summary>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        [HttpGet("{asentamiento}/Buscar")]
        public async Task<IActionResult> ObtenerCodigosPorstalesPorAsentamientamiento(string asentamiento)
        {
            var list = await _appDbContext.CodigoPostal
            .Where(item => item.Asentamiento.Contains(asentamiento))
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
            .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Subir coleccion de codigos postales
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile formFile)
        {
            string[] lines;

            var fechaInicial = DateTime.Now; var fechaFinal = DateTime.Now;
            StreamReader reader = new StreamReader(formFile.OpenReadStream(), System.Text.Encoding.Latin1);
            string text = reader.ReadToEnd();
            lines = text.Split("\n");
            try
            {
                int j = 0;

                Console.WriteLine("Borrando los datos e insertando Codigos postales");
                _appDbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE CodigoPostal");
                await _appDbContext.SaveChangesAsync();
                // d_codigo|d_asenta   |d_tipo_asenta|D_mnpio         |d_estado         |d_ciudad           | d_CP  | c_estado  |  c_oficina|c_CP|c_tipo_asenta|c_mnpio |id_asenta_cpcons|d_zona|c_cve_ciudad
                // 01000   | San Ángel | Colonia     | Álvaro Obregón | Ciudad de México| Ciudad de México  | 01001 | 09        |  01001    |    | 09          | 010    |0001            |Urbano|01
                // 0       | 1         | 2           | 3              | 4               | 5                 | 6     | 7         | 8         | 9  | 10          | 11     | 12             | 13   | 14
                for (int i = 2; i < lines.Count(); i++)
                {
                    string[] array;
                    CodigoPostalEntidad codigoPostalEntity;

                    array = lines[i].Split("|");
                    Console.WriteLine($"{i} Count: " + array.Count() + " ->" + lines[i]);
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
                        _appDbContext.CodigoPostal.Add(codigoPostalEntity);
                    }
                    if (j == 10000)
                    {
                        await _appDbContext.SaveChangesAsync();
                        j = 0;
                    }
                    j++;
                }
                await _appDbContext.SaveChangesAsync();
                Console.WriteLine("Terminado");
                fechaFinal = DateTime.Now;
            }
            catch (Exception)
            {
                throw;
            }

            return Accepted(new { fechaInicial, fechaFinal, TotalDeRegistros = lines.Count(), Tiempo = (fechaInicial - fechaFinal).TotalSeconds });
        }

        /// <summary>
        /// Subir coleccion de codigos postales
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("Mongo")]
        public async Task<IActionResult> AgregarAMongo(IFormFile formFile)
        {
            string[] lines;

            var fechaInicial = DateTime.Now; var fechaFinal = DateTime.Now;
            StreamReader reader = new StreamReader(formFile.OpenReadStream(), System.Text.Encoding.Latin1);
            string text = reader.ReadToEnd();
            lines = text.Split("\n");
            List<CodigoPostalEntidad> lista = new List<CodigoPostalEntidad>();
            try
            {               
                Console.WriteLine("Borrando los datos e insertando Codigos postales");
                //_appDbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE CodigoPostal");                
                // d_codigo|d_asenta   |d_tipo_asenta|D_mnpio         |d_estado         |d_ciudad           | d_CP  | c_estado  |  c_oficina|c_CP|c_tipo_asenta|c_mnpio |id_asenta_cpcons|d_zona|c_cve_ciudad
                // 01000   | San Ángel | Colonia     | Álvaro Obregón | Ciudad de México| Ciudad de México  | 01001 | 09        |  01001    |    | 09          | 010    |0001            |Urbano|01
                // 0       | 1         | 2           | 3              | 4               | 5                 | 6     | 7         | 8         | 9  | 10          | 11     | 12             | 13   | 14
                for (int i = 2; i < lines.Count(); i++)
                {
                    string[] array;
                    CodigoPostalEntidad codigoPostalEntity;

                    array = lines[i].Split("|");
                    Console.WriteLine($"{i} Count: " + array.Count() + " ->" + lines[i]);
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
                        //_appDbContext.CodigoPostal.Add(codigoPostalEntity);
                        lista.Add(codigoPostalEntity);
                    }                   
                }
                
                await _repositorioMongoDb.AgregarAsynx(lista);
                Console.WriteLine("Terminado");
                fechaFinal = DateTime.Now;
            }
            catch (Exception)
            {
                throw;
            }

            return Accepted(new { fechaInicial, fechaFinal, TotalDeRegistros = lines.Count(), Tiempo = (fechaInicial - fechaFinal).TotalSeconds });
        }

        /// <summary>
        /// Obtiene un codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        [HttpGet("Aleatorio")]
        public async Task<IActionResult> ObtenerCodigoPostalAleatorio()
        {
            int total;
            Random random = new Random();
            CodigoPostalEntidad x;

            total = _appDbContext.CodigoPostal.Count();
            do
            {
                int id;

                id = random.Next(1, total);
                x = await _appDbContext.CodigoPostal.Where(x => x.Id == id).FirstOrDefaultAsync();
            } while (x == null);

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
    }
}