namespace CodigosPostales.Repositorios
{
    public interface IRepositorio
    {
        /// <summary>
        /// Obtiene los estados
        /// </summary>
        /// <returns></returns>        
        Task<List<Estado>> ObtenerEstadosASync();

        /// <summary>
        /// Obtiene la lista de codigos postales
        /// </summary>
        /// <param name="codigoPostal"></param>
        /// <returns></returns>
        Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string codigoPostal);

        /// <summary>
        /// Obtiene las alcaldias de un estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        Task<List<Alcaldia>> ObtenerAlcaldiasAsync(string estado);

        /// <summary>
        /// Para insertar todos
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        Task AgregarAsynx(List<CodigoPostalEntidad> lista);

        /// <summary>
        /// Obtiene un codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        Task<CodigoPostalEntidad> ObtenerCodigoPostalAleatorioAsync(string estado);

        /// <summary>
        /// Obtiene los codigos postales por nombre de asentamiento
        /// </summary>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesPorAsentamientoAsync(string asentamiento);

        /// <summary>
        /// Borra todos los registros de la tabla
        /// </summary>
        /// <returns></returns>
        Task BorrarAsync();

        /// <summary>
        /// Obtiene los codigos postales por nombre de asentamiento
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <param name="asentamiento"></param>
        /// <returns></returns>
        Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string estado, string alcaldia, string asentamiento);

        /// <summary>
        /// Obtener codigo postal aleatorio
        /// </summary>
        /// <returns></returns>
        Task<CodigoPostalEntidad> ObtenerCodigoPostalAleatorioAsync();

        /// <summary>
        /// Obtiene todos lo códigos postales de una alcaldia
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="alcaldia"></param>
        /// <returns></returns>
        Task<List<CodigoPostalEntidad>> ObtenerCodigosPostalesAsync(string estado, string alcaldia);
    }
}
