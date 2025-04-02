using System.ServiceModel;

namespace CodigosPostales.ReglasDeNegocio
{
    [ServiceContract]
    public interface ICodigoPostalRdn
    {
        Task AgregarCodigosPostalesAsync(string[] lines);

        [OperationContract]
        Task<List<CodigoPostalDto>> BuscarCodigosPostalesAsync(string asentamiento);

        [OperationContract]
        Task<List<AlcaldiaDto>> ObtenerAlcaldiasAsync(string estado);

        [OperationContract]
        Task<CodigoPostalDto> ObtenerCodigoPostalAleatorioAsync();

        //[OperationContract]
        Task<CodigoPostalDto> ObtenerCodigoPostalAleatorioAsync(string estado);

        //[OperationContract]
        Task<List<CodigoPostalDto>> ObtenerCodigosPostalesAsync(string estado, string alcaldia);

        [OperationContract]
        Task<List<CodigoPostalDto>> ObtenerCodigosPostalesAsync(string codigoPostal);

        //[OperationContract]
        Task<List<CodigoPostalDto>> ObtenerCodigosPostalesAsync(string estado, string alcaldia, string asentamiento);

        [OperationContract]
        Task<List<EstadoDto>> ObtenerEstadosAsync();
    }
}