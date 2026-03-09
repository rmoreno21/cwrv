using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class SolicitudRP
    {
        [DataMember]
        public Afiliado Afiliado { get; set; }
        [DataMember]
        public AFP AFP { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public double TipoCambio { get; set; }
        [DataMember]
        public TipoPension TipoPension { get; set; }
        [DataMember]
        public Categoria Categoria { get; set; }
        [DataMember]
        public DateTime? FechaSolicitud { get; set; }
        [DataMember]
        public TipoCotizacion TipoCotizacion { get; set; }
        [DataMember]
        public DateTime? FechaDevengue { get; set; }
        [DataMember]
        public DateTime? FechaCotizacion { get; set; }
        //<GTIINI-754>
        [DataMember]
        public DateTime? FechaUltimaActualizacion { get; set; }
        //<GTIFIN-754>
        [DataMember]
        public Moneda MonedaPrimaUnica { get; set; }
        [DataMember]
        public Temporalidad Temporalidad { get; set; }
        [DataMember]
        public double PrimaUnica { get; set; }
        [DataMember]
        public string FactorTasa { get; set; }

        [DataMember]
        public List<CotizacionRP> Cotizaciones { get; set; }

        [DataMember]
        public List<GrupoFamiliar> Beneficiarios { get; set; }

        [DataMember]
        public Usuario Usuario { get; set; }

        [DataMember]
        public Agente Agente { get; set; }

        [DataMember]
        public double? PorcentajeDescuentoComision { get; set; }

        [DataMember]
        public List<CotizacionMovimiento> ListaCotizacionMovimiento { get; set; }
        [DataMember]
        public TipoMovimiento TipoMovimiento { get; set; }

        // Parámetros para la cotización masiva por lote
        [DataMember]
        public bool Habilitado { get; set; }
        [DataMember]
        public bool Enviada { get; set; }
        [DataMember]
        public bool TieneCotizacion { get; set; }
        [DataMember]
        public string Observacion { get; set; }
        [DataMember]
        public string Cartera { get; set; }
        [DataMember]
        public string OrigenTasa { get; set; }
        [DataMember]
        public string PorcentajeCesionComision { get; set; }
        [DataMember]
        public int NumeroPoliza { get; set; }
        [DataMember]
        public DateTime FechaCargaConfirmacion { get; set; }

        public string XMLSolicitud()
        {
            XElement root = new XElement("ROOT");
            XElement solicitud = new XElement("Solicitud");
            if (Id != null)
                solicitud.Add(new XAttribute("num_solicitud", Id));
            solicitud.Add(new XAttribute("fec_solicitud", FechaSolicitud.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("num_cuispp", Afiliado.CUSPP));
            solicitud.Add(new XAttribute("cod_afp", Afiliado.AFP.Id));
            solicitud.Add(new XAttribute("cod_tipo_cotizacion", TipoCotizacion.Id));
            solicitud.Add(new XAttribute("cod_tipo_pension", TipoPension.Id));
            solicitud.Add(new XAttribute("cod_categoria", Categoria.Id));
            solicitud.Add(new XAttribute("fec_devengue", FechaDevengue.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("num_vendedor", Agente.Id));
            solicitud.Add(new XAttribute("cod_moneda_cta_indiv", MonedaPrimaUnica.Id));
            solicitud.Add(new XAttribute("val_mto_cta_individual", PrimaUnica));
            solicitud.Add(new XAttribute("cod_factor_tipo_cotizacion", FactorTasa));
            solicitud.Add(new XAttribute("cod_tipo_temporalidad", Temporalidad.Id));
            if (PorcentajeDescuentoComision != null)
                solicitud.Add(new XAttribute("val_dcom", PorcentajeDescuentoComision));

            root.Add(solicitud);

            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            xml.Add(root);

            return xml.ToString();
        }

        public string XMLBeneficiario()
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("ROOT");

            XElement beneficiario;
            foreach (GrupoFamiliar ben in Beneficiarios)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                if (ben.ApellidoPaterno != null)
                    atributos.Add(new XAttribute("ape_paterno", ben.ApellidoPaterno));
                if (ben.ApellidoMaterno != null)
                    atributos.Add(new XAttribute("ape_materno", ben.ApellidoMaterno));
                if (ben.Nombre != null)
                    atributos.Add(new XAttribute("nom_persona", ben.Nombre));
                if (ben.Identificacion != null && ben.Identificacion.Numero != null)
                    atributos.Add(new XAttribute("num_identificacion", ben.Identificacion.Numero));
                if (ben.Identificacion != null && ben.Identificacion.IdTipo != null)
                    atributos.Add(new XAttribute("cod_tipo_identificacion", ben.Identificacion.IdTipo));
                if (ben.Parentesco.Id != null)
                    atributos.Add(new XAttribute("cod_parentezco", ben.Parentesco.Id));
                if (ben.FechaNacimiento != null)
                    atributos.Add(new XAttribute("fec_nacimiento", ben.FechaNacimiento.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("ind_invalidez", ben.Invalido ? "S" : "N"));
                atributos.Add(new XAttribute("cod_tipo_invalidez", ben.TipoInvalidez.Id));
                if (ben.FechaInvalidez != null)
                    atributos.Add(new XAttribute("fec_invalidez", ben.FechaInvalidez.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("cod_sexo", ben.Sexo));
                atributos.Add(new XAttribute("num_cuissp", Afiliado.CUSPP));
                atributos.Add(new XAttribute("cod_afp", Afiliado.AFP.Id));
                atributos.Add(new XAttribute("cod_cartera", Agente.IdCartera));

                beneficiario = new XElement("Beneficiario", atributos);

                root.Add(beneficiario);
            }
            xml.Add(root);

            return xml.ToString();
        }

        public string XMLCotizacion()
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("ROOT");

            XElement cotizacion;
            foreach (CotizacionRP cot in Cotizaciones)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                atributos.Add(new XAttribute("fec_cotizacion", FechaCotizacion.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("num_correlativo", cot.Correlativo));
                atributos.Add(new XAttribute("cod_moneda", cot.Moneda.Id));
                atributos.Add(new XAttribute("val_moneda", (cot.Moneda.Id == "001" || cot.Moneda.Id == "013") ? 1 : TipoCambio));
                atributos.Add(new XAttribute("cod_tipo_producto", "01"));
                atributos.Add(new XAttribute("ind_modalidad", "I"));
                atributos.Add(new XAttribute("val_per_garantizado", cot.PeriodoGarantizado));
                atributos.Add(new XAttribute("val_per_temporal", "0"));
                atributos.Add(new XAttribute("val_pje_rent_temp", "0"));
                atributos.Add(new XAttribute("ind_gratificacion", "N"));
                atributos.Add(new XAttribute("cod_particion_capital", "-"));
                if (cot.AjusteTRA != null)
                    atributos.Add(new XAttribute("val_tasa_ajuste_tra", cot.AjusteTRA));

                cotizacion = new XElement("Cotizacion", atributos);

                root.Add(cotizacion);
            }
            xml.Add(root);

            return xml.ToString();
        }

        public string XMLListaCotizacionMovimiento()
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("ROOT");

            XElement movimiento;
            foreach (CotizacionMovimiento cotizacionMovimiento in ListaCotizacionMovimiento)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                atributos.Add(new XAttribute("num_solicitud", cotizacionMovimiento.NumSolicitud));
                atributos.Add(new XAttribute("fec_cotizacion", cotizacionMovimiento.FecCotizacion));
                atributos.Add(new XAttribute("num_correlativo", cotizacionMovimiento.Correlativo));
                atributos.Add(new XAttribute("cod_tipo_movimiento", cotizacionMovimiento.TipoMovimiento.Id));
                if (cotizacionMovimiento.GlsMovimiento != null)
                    atributos.Add(new XAttribute("gls_movimiento", cotizacionMovimiento.GlsMovimiento));

                movimiento = new XElement("Movimiento", atributos);

                root.Add(movimiento);
            }
            xml.Add(root);

            return xml.ToString();
        }

        public Respuesta Respuesta { get; set; }
    }
}
