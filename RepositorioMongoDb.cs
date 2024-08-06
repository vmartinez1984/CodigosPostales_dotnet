using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodigosPostales_net
{
    /// <summary>
    /// 
    /// </summary>
    public class RepositorioMongoDb : IRepositorio
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<CodigoPostalEntidad> _collection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSettings"></param>
        public RepositorioMongoDb(IOptions<DataSettingMongoDb> dataSettings)
        {
            var mongoClient = new MongoClient(dataSettings.Value.ConnectionString);

            _mongoDatabase = mongoClient.GetDatabase(dataSettings.Value.DatabaseName);

            _collection = _mongoDatabase.GetCollection<CodigoPostalEntidad>(dataSettings.Value.CollectionName);
        }

        /// <summary>
        /// Obtener alcaldias de un estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public async Task<List<Alcaldia>> ObtenerAlcaldiasAsync(string estado)
        {
            List<Alcaldia> list;
            List<CodigoPostalEntidad> codigos = new List<CodigoPostalEntidad>();

            var eAlcaldias = await _collection.DistinctAsync<string>("Alcaldia", new BsonDocument("Estado", estado));
            var alcaldias = eAlcaldias.ToList();
            foreach (var alcaldia in alcaldias)
            {
                var codigo = await _collection.Find(x => x.Alcaldia == alcaldia).FirstOrDefaultAsync();
                codigos.Add(codigo);
            }
            list = codigos.OrderBy(x => x.Alcaldia).Select(x => new Alcaldia
            {
                Nombre = x.Alcaldia,
                Id = x.AlcaldiaId.ToString()
            }).ToList();

            return list;
        }

        /// <summary>
        /// Obtener los registros de codigos postales
        /// </summary>
        /// <param name="codigoPostal"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string codigoPostal)
        {
            var list = await _collection.Find(x => x.CodigoPostal == codigoPostal).ToListAsync();

            return list;
        }

        /// <summary>
        /// Obtiene un codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        public async Task<CodigoPostalEntidad> ObtenerCodigoPostalAleatorioAsync()
        {
            int total;
            Random random = new Random();
            CodigoPostalEntidad x;

            total = (int)await _collection.CountDocumentsAsync(_ => true);
            do
            {
                int id;

                id = random.Next(1, total);
                var results = await _collection.FindAsync(x => x.Id == id);
                x = await results.SingleAsync();
            } while (x == null);

            return x;
        }

        /// <summary>
        /// Lista de estados
        /// </summary>
        /// <returns></returns>
        public async Task<List<Estado>> ObtenerEstadosASync()
        {
            List<CodigoPostalEntidad> codigos = new List<CodigoPostalEntidad>();

            var listaDeEstados = await _collection.DistinctAsync<string>("Estado", new BsonDocument());
            var estados = listaDeEstados.ToList();
            foreach (var estado in estados)
            {
                var codigo = await _collection.Find(x => x.Estado == estado).FirstOrDefaultAsync();
                codigos.Add(codigo);
            }

            return codigos.Select(x => new Estado
            {
                Id = x.EstadoId.ToString(),
                Nombre = x.Estado
            }).ToList();
        }

        /// <summary>
        /// Para insertar todos
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public async Task AgregarAsynx(List<CodigoPostalEntidad> lista)
        {
            for (var i = 0; i < lista.Count; i++)
                lista[i].Id = i + 1;

            await _collection.InsertManyAsync(lista);
        }

        /// <summary>
        ///  Borra la tabla
        /// </summary>
        /// <returns></returns>
        public async Task BorrarAsync()
        {
            await _mongoDatabase.DropCollectionAsync(_collection.CollectionNamespace.CollectionName);
        }

        internal async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string estado, string alcaldia)
        {
            return (await _collection.FindAsync(
            new BsonDocument("$and", new BsonArray
            {
                new BsonDocument("Estado", estado),
                new BsonDocument("Alcaldia",alcaldia)
            }))).ToList();
        }

        /// <summary>
        /// Obtiene los codigos postales por el nombre del asentamiento
        /// </summary>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesPorAsentamientoAsync(string asentamiento)
        {
            // Patrón de búsqueda tipo LIKE (por ejemplo, buscar personas cuyo nombre contenga "Juan")
            var filtro = Builders<CodigoPostalEntidad>.Filter.Regex("Asentamiento", new BsonRegularExpression(asentamiento, "i"));

            // Buscar documentos que coincidan con el patrón
            var codigos = await _collection.Find(filtro).ToListAsync();

            return codigos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public async Task<CodigoPostalEntidad> ObtenerCodigoPostalAleatorioAsync(string estado)
        {
            Random random = new Random();
            List<CodigoPostalEntidad> codigos;
            int estadoId;

            if (int.TryParse(estado, out estadoId))
                codigos = (await _collection.FindAsync(x => x.EstadoId == estadoId)).ToList();
            else
                codigos = (await _collection.FindAsync(x => x.Estado == estado)).ToList();

            var i = random.Next(0, codigos.Count);

            return codigos[i];
        }
    }
}