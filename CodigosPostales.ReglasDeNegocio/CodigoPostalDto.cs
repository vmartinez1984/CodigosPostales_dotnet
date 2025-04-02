namespace CodigosPostales.ReglasDeNegocio;
public class CodigoPostalDto
{
    public string CodigoPostal { get; set; }
    public int AlcaldiaId { get; set; }
    public string Estado { get; set; }
    public int EstadoId { get; set; }
    public string Alcaldia { get; set; }
    public string TipoDeAsentamiento { get; set; }
    public string Asentamiento { get; set; }
}
public class EstadoDto
{
    public string Id { get; set; }

    public string Nombre { get; set; }
}

public class AlcaldiaDto
{
    public string Id { get; set; }

    public string Nombre { get; set; }
}

