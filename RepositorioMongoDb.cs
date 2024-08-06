using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodigosPostales_net
{
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

            _collection = _mongoDatabase.GetCollection<CodigoPostalEntidad>(dataSettings.Value.ZipCodeCollectionName);
        }

        public async Task<List<Alcaldia>> ObtenerAlcaldiasAsync(string estado)
        {
            List<Alcaldia> list;

            // var filter = Builders<BsonDocument>.Filter
            var data = await _collection.Find(x => x.Estado == estado).ToListAsync();
            list = data.OrderBy(x => x.Alcaldia).Select(x => new Alcaldia
            {
                Nombre = x.Alcaldia,
                Id = x.AlcaldiaId.ToString()
            }).Distinct().ToList();

            return list;
        }

        public async Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string codigoPostal)
        {
            List<CodigoPostalEntidad> entities;

            var list = await _collection.Find(x => x.CodigoPostal == codigoPostal).ToListAsync();

            return list;
        }

        public async Task<List<Estado>> ObtenerEstadosASync()
        {
            // var list = await _collection.DistinctAsync<string>("Estado", new BsonDocument());
            // return list.ToList().Select(x => new Estado
            // {
            //     Id = x.estadoId,
            //     Nombre = x.estado
            // }).ToList();
            throw new NotImplementedException();
        }


        // public async Task<string> AgregarAync(CodigoPostalEntidad codigoPostal){
        //     codigoPostal.Id = 
        //     await _collection.InsertOneAsync(codigoPostal);
        // }

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
    }
}