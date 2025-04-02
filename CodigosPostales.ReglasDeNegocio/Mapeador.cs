using CodigosPostales.Repositorios;

namespace CodigosPostales.ReglasDeNegocio
{
    public static class Mapeador
    {
        public static CodigoPostalDto ToDto(this CodigoPostalEntidad entidad) 
        => new CodigoPostalDto
        {
            Alcaldia = entidad.Alcaldia,
            AlcaldiaId = entidad.AlcaldiaId,
            Asentamiento = entidad.Asentamiento,
            CodigoPostal = entidad.CodigoPostal,
            Estado = entidad.Estado,
            EstadoId = entidad.EstadoId,
            TipoDeAsentamiento = entidad.TipoDeAsentamiento
        };

        public static List<CodigoPostalDto> ToDtos(this List<CodigoPostalEntidad> entidadas) 
            => entidadas.Select(x => x.ToDto()).ToList();
    }
}
