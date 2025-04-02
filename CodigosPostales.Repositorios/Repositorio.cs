using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace CodigosPostales.Repositorios
{
    public class Repositorio : IRepositorio
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<CodigoPostalEntidad> _collection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Repositorio(IConfiguration configuration)
        {
            string stringConnection = configuration.GetConnectionString("CodigosPostales");
            var mongoClient = new MongoClient(stringConnection);
            string databaseName = stringConnection.Split("/").Last().Split("?").First();
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
            _collection = _mongoDatabase.GetCollection<CodigoPostalEntidad>("CodigosPostales");
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
        /// Obtener alcaldias de un estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public async Task<List<Alcaldia>> ObtenerAlcaldiasAsync(string estado)
        {
            List<Alcaldia> list;
            List<CodigoPostalEntidad> codigos = new List<CodigoPostalEntidad>();
            IAsyncCursor<string> eAlcaldias;
            int estadoId;
            if (int.TryParse(estado, out estadoId))
                eAlcaldias = await _collection.DistinctAsync<string>("Alcaldia", new BsonDocument("EstadoId", estadoId));
            else
                eAlcaldias = await _collection.DistinctAsync<string>("Alcaldia", new BsonDocument("Estado", estado));
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
            // Agregación para obtener un documento aleatorio
            var pipeline = new[]
            {                
                new BsonDocument("$sample", new BsonDocument("size", 1)) // Obtiene 1 documento aleatorio
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var objeto = BsonSerializer.Deserialize<CodigoPostalEntidad>(result);

            return objeto;
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
            var random = new Random();
            BsonDocument filtro;

            // Definir el filtro
            if (int.TryParse(estado, out int estadoId))
            {
                filtro = new BsonDocument("EstadoId", estadoId); // Reemplaza con tu condición
            }
            else
            {
                filtro = new BsonDocument("Estado", estado); // Reemplaza con tu condición             
            }

            // Agregación para obtener un documento aleatorio
            var pipeline = new[]
            {
                new BsonDocument("$match", filtro),  // Filtrar documentos
                new BsonDocument("$sample", new BsonDocument("size", 1)) // Obtiene 1 documento aleatorio
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var objeto = BsonSerializer.Deserialize<CodigoPostalEntidad>(result);

            return objeto;
        }

        /// <summary>
        /// Obtiene los codigos postales de una alcaldia
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <returns></returns>
        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string estado, string alcaldia)
        {
            var lista = await _collection.FindAsync(x => x.EstadoId == int.Parse(estado) && x.AlcaldiaId == int.Parse(alcaldia));

            return lista.ToList();
        }

        /// <summary>
        /// Obtiene los codigos postales de una alcaldia
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        async Task<List<CodigoPostalEntidad>> IRepositorio.ObtenerCodigosPostalesAsync(string estado, string alcaldia, string asentamiento)
        {
            var lista = await _collection.FindAsync(x => x.EstadoId == int.Parse(estado) && x.AlcaldiaId == int.Parse(alcaldia) && x.Asentamiento.Contains(asentamiento));

            return lista.ToList();
        }
    }
}
