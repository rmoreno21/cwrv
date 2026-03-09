using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CotizacionMovimiento
    {
        [DataMember]
        public string NumSolicitud { get; set; }
        [DataMember]
        public DateTime? FecCotizacion { get; set; }
        [DataMember]
        public Int64 Correlativo { get; set; }
        [DataMember]
        public Int64 NumMovimiento { get; set; }
        [DataMember]
        public TipoMovimiento TipoMovimiento { get; set; }
        [DataMember]
        public string GlsMovimiento { get; set; }
        [DataMember]
        public double ValTasaAjusteTra { get; set; }
        [DataMember]
        public DateTime? FecInicioMovimiento { get; set; }
        [DataMember]
        public DateTime? FecFinMovimiento { get; set; }
        [DataMember]
        public Usuario Usuario { get; set; }

        //[DataMember]
        //public List<CotizacionMovimiento> ListaCotizacionMovimiento { get; set; }

        //public string XMLCotizacionMovimiento()
        //{
        //    XElement root = new XElement("ROOT");
        //    XElement movimiento = new XElement("Movimiento");

        //    movimiento.Add(new XAttribute("num_solicitud", NumSolicitud));
        //    movimiento.Add(new XAttribute("fec_cotizacion", FecCotizacion));
        //    movimiento.Add(new XAttribute("num_correlativo", Correlativo));
        //    movimiento.Add(new XAttribute("cod_tipo_movimiento", TipoMovimiento.Id));
        //    movimiento.Add(new XAttribute("gls_movimiento", GlsMovimiento));

        //    root.Add(movimiento);

        //    XDocument xml = new XDocument();
        //    xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
        //    xml.Add(root);

        //    return xml.ToString();
        //}

        //public string XMLListaCotizacionMovimiento()
        //{
        //    XDocument xml = new XDocument();
        //    xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
        //    XElement root = new XElement("ROOT");

        //    XElement movimiento;
        //    foreach (CotizacionMovimiento cotizacionMovimiento in ListaCotizacionMovimiento)
        //    {
        //        List<XAttribute> atributos = new List<XAttribute>();
        //        atributos.Add(new XAttribute("num_solicitud", cotizacionMovimiento.NumSolicitud));
        //        atributos.Add(new XAttribute("fec_cotizacion", cotizacionMovimiento.FecCotizacion));
        //        atributos.Add(new XAttribute("num_correlativo", cotizacionMovimiento.Correlativo));
        //        atributos.Add(new XAttribute("cod_tipo_movimiento", cotizacionMovimiento.TipoMovimiento.Id));
        //        if (cotizacionMovimiento.GlsMovimiento != null)
        //            atributos.Add(new XAttribute("gls_movimiento", cotizacionMovimiento.GlsMovimiento));

        //        movimiento = new XElement("Movimiento", atributos);

        //        root.Add(movimiento);
        //    }
        //    xml.Add(root);

        //    return xml.ToString();
        //}

        //public Respuesta Respuesta { get; set; }
    }
}
