public class ResultadoCotizacionDto
{
    public bool ExitoOperacion { get; set; }
    public string MensajeError { get; set; }
    public string idSolicitud { get; set; }
    public string correlativos { get; set; }
    public string XmlCotizacion { get; set; }
    public string XmlBeneficios { get; set; }
    public string XmlPorcentajes { get; set; }

}