using Microsoft.EntityFrameworkCore;

namespace CodigosPostales_net
{
    /// <summary>
    /// REpositorio Sql
    /// </summary>
    public class RepositorioSql : IRepositorio
    {
        private readonly AppDbContext _appDbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDbContext"></param>
        public RepositorioSql(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Obtiene los estados
        /// </summary>
        /// <returns></returns>
        public async Task<List<Estado>> ObtenerEstadosASync()
        {
            var lista = await _appDbContext.CodigoPostal
           .Select(x => new Estado
           {
               Nombre = x.Estado,
               Id = x.EstadoId.ToString(),
           }).Distinct()
           .OrderBy(x => x.Id)
           .ToListAsync();

            return lista;
        }

        /// <summary>
        /// Obtiene las alcaldias de un estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public async Task<List<Alcaldia>> ObtenerAlcaldiasAsync(string estado)
        {
            int estadoId;
            IQueryable<CodigoPostalEntidad> queryable;

            if (int.TryParse(estado, out estadoId))
                queryable = _appDbContext.CodigoPostal.Where(item => item.EstadoId == estadoId);
            else
                queryable = _appDbContext.CodigoPostal.Where(item => item.Estado == estado);

            var lista = await queryable.Select(x => new Alcaldia
            {
                Nombre = x.Alcaldia,
                Id = x.AlcaldiaId.ToString()
            })
            .Distinct()
            .OrderBy(x => x.Id)
            .ToListAsync();

            return lista;
        }

        /// <summary>
        /// Obtener codigos postales
        /// </summary>
        /// <param name="codigoPostal"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string codigoPostal)
        {
            var list = await _appDbContext.CodigoPostal
           .Where(item => item.CodigoPostal == codigoPostal)
           .Select(x => new CodigoPostalEntidad
           {
               CodigoPostal = x.CodigoPostal,
               AlcaldiaId = x.AlcaldiaId,
               Estado = x.Estado,
               EstadoId = x.EstadoId,
               Alcaldia = x.Alcaldia,
               TipoDeAsentamiento = x.TipoDeAsentamiento,
               Asentamiento = x.Asentamiento
           })
           .ToListAsync();

            return list;
        }

        /// <summary>
        /// Agrega los codigos postales en la base de datos
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public async Task AgregarAsynx(List<CodigoPostalEntidad> lista)
        {
            try
            {
                int j = 0;

                for (int i = 0; i < lista.Count(); i++)
                {
                    Console.WriteLine($"{i} Count: " + lista.Count() + " ->" + lista[i]);
                    _appDbContext.CodigoPostal.Add(lista[0]);

                    if (j == 10000)
                    {
                        await _appDbContext.SaveChangesAsync();
                        j = 0;
                    }
                    j++;
                }
                await _appDbContext.SaveChangesAsync();
                Console.WriteLine("Terminado");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Borra todos los registros de la Db
        /// </summary>
        /// <returns></returns>
        public async Task BorrarAsync()
        {
            _appDbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE CodigoPostal");
            await _appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Obtener un codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        public async Task<CodigoPostalEntidad> ObtenerCodigoPostalAleatorioAsync()
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

            return x;
        }

        /// <summary>
        /// Obtiene los registros de codigos posta
        /// </summary>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesPorAsentamientoAsync(string asentamiento)
        {
            var list = await _appDbContext.CodigoPostal
            .Where(item => item.Asentamiento.Contains(asentamiento))
            .ToListAsync();

            return list;
        }

        /// <summary>
        /// Obtener un codigo postal por estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public async Task<CodigoPostalEntidad> ObtenerCodigoPostalAleatorioAsync(string estado)
        {
            int total, estadoId;
            Random random = new Random();
            CodigoPostalEntidad x;

            if (int.TryParse(estado, out estadoId))
            { }
            else
            {
                var estados = await ObtenerEstadosASync();
                estadoId = int.Parse(estados.FirstOrDefault(x => x.Nombre == estado).Id);
            }
            total = _appDbContext.CodigoPostal.Count();
            do
            {
                int id;

                id = random.Next(1, total);
                x = await _appDbContext.CodigoPostal.Where(x => x.Id == id && x.EstadoId == estadoId).FirstOrDefaultAsync();
            } while (x == null);

            return x;
        }

        /// <summary>
        /// Obtener lista de codigos postales por alcaldia y estado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string estado, string alcaldia)
        {
            int estadoId, alcaldiaId;
            IQueryable<CodigoPostalEntidad> queryable;

            if (int.TryParse(estado, out estadoId) && int.TryParse(alcaldia, out alcaldiaId))
                queryable = _appDbContext.CodigoPostal.Where(item => item.EstadoId == estadoId && item.AlcaldiaId == alcaldiaId);
            else
                queryable = _appDbContext.CodigoPostal.Where(item => item.Estado == estado && item.Alcaldia == alcaldia);

            return await queryable.ToListAsync();
        }

        /// <summary>
        /// Obtiene lista de cogidos posta por alcaldia, estado y asentamiento
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string estado, string alcaldia, string asentamiento)
        {
            int estadoId, alcaldiaId;
            IQueryable<CodigoPostalEntidad> queryable;

            if (int.TryParse(estado, out estadoId) && int.TryParse(alcaldia, out alcaldiaId))
                queryable = _appDbContext.CodigoPostal.Where(item => item.EstadoId == estadoId && item.AlcaldiaId == alcaldiaId && item.Asentamiento.Contains(asentamiento));
            else
                queryable = _appDbContext.CodigoPostal.Where(item => item.Estado == estado && item.Alcaldia == alcaldia && item.Asentamiento.Contains(asentamiento));

            return await queryable.ToListAsync();
        }
    }
}