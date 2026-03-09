using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    public class RespuestaCotizacion
    {
    public bool ExitoOperacion { get; set; }
    public string MensajeError { get; set; }
    public string idSolicitud { get; set; }
    public string correlativos { get; set; }
    public string XmlCotizacion { get; set; }
    public string XmlBeneficiarios { get; set; }
    public string XmlPorcentajes { get; set; }
}
}
