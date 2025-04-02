using CodigosPostales.Repositorios;

namespace CodigosPostales.ReglasDeNegocio
{
    public class CodigoPostalRdn : ICodigoPostalRdn
    {
        private readonly IRepositorio _repositorio;

        public CodigoPostalRdn(IRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<EstadoDto>> ObtenerEstadosAsync()
        {
            List<Estado> estados;

            estados = await _repositorio.ObtenerEstadosASync();

            return estados.Select(x => new EstadoDto { Id = x.Id, Nombre = x.Nombre }).ToList();
        }

        public async Task<List<AlcaldiaDto>> ObtenerAlcaldiasAsync(string estado)
        {
            var lista = await _repositorio.ObtenerAlcaldiasAsync(estado);

            return lista.Select(x => new AlcaldiaDto { Id = x.Id, Nombre = x.Nombre }).ToList();
        }

        public async Task<List<CodigoPostalDto>> ObtenerCodigosPostalesAsync(string codigoPostal)
        => (await _repositorio.ObtenerCodigosPostalesAsync(codigoPostal)).ToDtos();
        public async Task<List<CodigoPostalDto>> ObtenerCodigosPostalesAsync(string estado, string alcaldia)
        => (await _repositorio.ObtenerCodigosPostalesAsync(estado, alcaldia)).ToDtos();
        public async Task<List<CodigoPostalDto>> ObtenerCodigosPostalesAsync(string estado, string alcaldia, string asentamiento)
        => (await _repositorio.ObtenerCodigosPostalesAsync(estado, alcaldia, asentamiento)).ToDtos();
        public async Task<List<CodigoPostalDto>> BuscarCodigosPostalesAsync(string asentamiento)
        => (await _repositorio.ObtenerCodigosPostalesPorAsentamientoAsync(asentamiento)).ToDtos();
        public async Task<CodigoPostalDto> ObtenerCodigoPostalAleatorioAsync()
            => (await _repositorio.ObtenerCodigoPostalAleatorioAsync()).ToDto();
        public async Task<CodigoPostalDto> ObtenerCodigoPostalAleatorioAsync(string estado)
        => (await _repositorio.ObtenerCodigoPostalAleatorioAsync(estado)).ToDto();

        public async Task AgregarCodigosPostalesAsync(string[] lines)
        {
            List<CodigoPostalEntidad> codigos = new List<CodigoPostalEntidad>();
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
        }
    }
}
