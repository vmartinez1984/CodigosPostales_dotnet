
using Microsoft.EntityFrameworkCore;

namespace CodigosPostales_net
{
    public class RepositorioSql : IRepositorio
    {
        private readonly AppDbContext _appDbContext;
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

        public Task AgregarAsynx(List<CodigoPostalEntidad> lista)
        {
            throw new NotImplementedException();
        }
    }
}