using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodigosPostales_net
{
    public class CodigoPostalEntidad
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [NotMapped]
        public string _id { get; set; }
        
        [Key]
        public int Id { get; set; }

        [StringLength(5)]
        public string CodigoPostal { get; set; }

        [StringLength(50)]
        public string Estado { get; set; }

        public int EstadoId { get; set; }

        [StringLength(250)]
        public string Alcaldia { get; set; }

        public int AlcaldiaId { get; set; }

        [StringLength(50)]
        public string TipoDeAsentamiento { get; set; }

        [StringLength(100)]
        public string Asentamiento { get; set; }
    }

    public class Estado
    {
        public string Id { get; set; }

        public string Nombre { get; set; }
    }

    public class Alcaldia
    {
        public string Id { get; set; }

        public string Nombre { get; set; }
    }
}