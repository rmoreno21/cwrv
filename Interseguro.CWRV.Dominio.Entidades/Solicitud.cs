using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Solicitud
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
        public DateTime? FechaRecepcion { get; set; }
        [DataMember]
        public DateTime? FechaPlazoAFP { get; set; }
        [DataMember]
        public DateTime? FechaCierre { get; set; }
        [DataMember]
        public DateTime? FechaDevengue { get; set; }
        [DataMember]
        public DateTime? FechaCotizacion { get; set; }
        [DataMember]
        public DateTime? FechaSolicitudPension { get; set; }
        //<GTIINI-754>
        [DataMember]
        public DateTime? FechaUltimaActualizacion { get; set; }
        //<GTIFIN-754>
        [DataMember]
        public string Companhia { get; set; }
        //<SRI.INI-20322>
        [DataMember]
        public int NumLoteCotizacion { get; set; }
        [DataMember]
        public string IndEnviadoSbs { get; set; }
        //<SRI.FIN-20322>
        [DataMember]
        public double SaldoCIC { get; set; }
        [DataMember]
        public string FactorTasa { get; set; }

        [DataMember]
        public List<Cotizacion> Cotizaciones { get; set; }

        [DataMember]
        public List<GrupoFamiliar> Beneficiarios { get; set; }

        [DataMember]
        public Usuario Usuario { get; set; }

        [DataMember]
        public Agente Agente { get; set; }

        //<SRIINI06326>   
        [DataMember]
        public double? PorcentajeAumentoComision { get; set; }
        [DataMember]
        public double? PorcentajeDescuentoComision { get; set; }
        //<SRIFIN06326>
        //<SRIINI20322>
        [DataMember]
        public double? MontoAumentoComision { get; set; }
        //<SRIINI20322>

        //<SRI.INI-20322_E2>
        [DataMember]
        public List<CotizacionMovimiento> ListaCotizacionMovimiento { get; set; }
        [DataMember]
        public TipoMovimiento TipoMovimiento { get; set; }
        //<SRI.FIN-20322_E2>

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

        [DataMember]
        public string firmaDigitalToken { get; set; }

        //<INIGTI_4081>
        [DataMember]
        public Compania Compania { get; set; }
        //<FINGTI_4081>

        //<INIGTI_6556>
        [DataMember]
        public bool ValidarACOM { get; set; }
        [DataMember]
        public bool ValidarDTRA { get; set; }
        //<FINGTI_6556>


        //public string XMLSolicitud()
        //{
        //    XDocument xml = new XDocument(
        //    new XDeclaration("1.0", "utf-8", "yes"),
        //        new XElement("ROOT",
        //            new XElement("Solicitud",
        //                new XAttribute("fec_solicitud", FechaSolicitud.Value.ToString("yyyyMMdd")),
        //                new XAttribute("num_cuispp", Afiliado.CUSPP),
        //                new XAttribute("cod_afp", Afiliado.AFP.Id),
        //                new XAttribute("cod_tipo_cotizacion", TipoCotizacion.Id),
        //                new XAttribute("cod_tipo_pension", TipoPension.Id),
        //                new XAttribute("fec_devengue", FechaCierre.Value.ToString("yyyyMMdd")),
        //                new XAttribute("fec_recepcion", FechaRecepcion.Value.ToString("yyyyMMdd")),
        //                new XAttribute("fec_ult_actualizacion", FechaSolicitud.Value.ToString("yyyyMMdd")),
        //                new XAttribute("fec_presentacion", FechaPlazoAFP.Value.ToString("yyyyMMdd")),
        //                new XAttribute("num_vendedor", Agente.Id),
        //                new XAttribute("cod_categoria", Categoria.Id),
        //                new XAttribute("val_mto_cta_individual", SaldoCIC),
        //                new XAttribute("val_tasa_cambio", TipoCambio),
        //                new XAttribute("cod_factor_tipo_cotizacion", TipoCotizacion.Id)
        //            )
        //        )
        //    );
        //    return xml.ToString();
        //}

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
            //solicitud.Add(new XAttribute("fec_devengue", FechaCierre.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("fec_devengue", FechaDevengue.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("fec_recepcion", FechaRecepcion.Value.ToString("yyyyMMdd")));
            //<GTIINI-754>
            //solicitud.Add(new XAttribute("fec_ult_actualizacion", FechaSolicitud.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("fec_ult_actualizacion", FechaUltimaActualizacion.Value.ToString("yyyyMMdd")));
            //<GTIFIN-754>
            solicitud.Add(new XAttribute("fec_presentacion", FechaPlazoAFP.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("fec_sol_pension", FechaSolicitudPension.Value.ToString("yyyyMMdd")));
            solicitud.Add(new XAttribute("num_vendedor", Agente.Id));
            solicitud.Add(new XAttribute("cod_categoria", Categoria.Id));
            solicitud.Add(new XAttribute("val_mto_cta_individual", SaldoCIC));
            solicitud.Add(new XAttribute("val_tasa_cambio", TipoCambio));
            solicitud.Add(new XAttribute("cod_factor_tipo_cotizacion", FactorTasa));
            if (PorcentajeAumentoComision != null)
                solicitud.Add(new XAttribute("val_acom", PorcentajeAumentoComision));
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
                //<INIGTI_7012>
                atributos.Add(new XAttribute("id_grupo_familiar", ben.Id));
                //<FINGTI_7012>

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
            foreach (Cotizacion cot in Cotizaciones)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                atributos.Add(new XAttribute("fec_cotizacion", FechaCotizacion.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("num_correlativo", cot.Correlativo));
                atributos.Add(new XAttribute("cod_moneda", cot.Moneda.Id));
                atributos.Add(new XAttribute("val_moneda", (cot.Moneda.Id == "001" || cot.Moneda.Id == "013") ? 1 : TipoCambio));
                atributos.Add(new XAttribute("cod_tipo_producto", cot.Producto.Id));
                atributos.Add(new XAttribute("ind_modalidad", cot.Modalidad.Id));
                atributos.Add(new XAttribute("val_per_garantizado", cot.PeriodoGarantizado));
                atributos.Add(new XAttribute("val_per_temporal", cot.PeriodoDiferido));
                atributos.Add(new XAttribute("val_pje_rent_temp", cot.PorcentajeEntreRentas));
                atributos.Add(new XAttribute("ind_gratificacion", cot.Gratificacion ? "S" : "N"));
                atributos.Add(new XAttribute("cod_particion_capital", cot.Capital.Id));
                if (cot.AjusteTRA != null)
                    atributos.Add(new XAttribute("val_tasa_ajuste_tra", cot.AjusteTRA));

                cotizacion = new XElement("Cotizacion", atributos);

                root.Add(cotizacion);
            }
            xml.Add(root);

            return xml.ToString();
        }

        //<SRI.INI-20322_E2>
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
        //<SRI.FIN-20322_E2>

        public Respuesta Respuesta { get; set; }

        //<GTI.29372-INI>
        [DataMember]
        public bool IndCoberturaIS { get; set; }
        //<GTI.29372-FIN>
        //<GTI.59048-INI>
        public List<Beneficiario> BeneficiariosBenefi { get; set; }
        //<GTI.59048-FIN>
    }
}
