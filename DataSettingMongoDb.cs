using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodigosPostales_net
{
    public class DataSettingMongoDb
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ZipCodeCollectionName { get; set; } = null!;
    }
}