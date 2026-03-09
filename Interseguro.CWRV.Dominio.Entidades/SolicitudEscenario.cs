using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class SolicitudEscenario
    {

        [DataMember]
        public DateTime? FechaPresentacion { get; set; }
        //<SRIINI-25781>
        [DataMember]
        public DateTime? FechaCierreLote { get; set; }
        //<SRIFIN-25781>
        [DataMember]
        public Int64 NumOperacion { get; set; }
        [DataMember]
        public string NumSolicitud { get; set; }
        [DataMember]
        public DateTime? FecRegistroEscenario { get; set; }
        [DataMember]
        public double CodPjeCesionComision { get; set; }
        [DataMember]
        public double PjeAumentoComision { get; set; }
        [DataMember]
        public double ValTasaAjusteTra { get; set; }
        [DataMember]
        public string IndCondicionEspecial { get; set; }
        [DataMember]
        public string IndAprueba { get; set; }
        [DataMember]
        public DateTime? FecRespuesta { get; set; }
        [DataMember]
        public double ValTotalCic { get; set; }
        [DataMember]
        public double ValBonoActualizado { get; set; }
        [DataMember]
        public Int64 CodPadre { get; set; }
        [DataMember]
        public string CodNodo { get; set; }
        [DataMember]
        public string IndEstadoSeleccion { get; set; }
        [DataMember]
        public DateTime? FecDiaCita { get; set; }
        [DataMember]
        public string IndVigenciaAgente { get; set; }
        [DataMember]
        public DateTime? FecCierre { get; set; }
        [DataMember]
        public double ValMtoAgenteAcom { get; set; }
        [DataMember]
        public Int64 NumCotizacionElegida { get; set; }
        [DataMember]
        public DateTime? FecSolicitud { get; set; }
        [DataMember]
        public string NumSolicitudCopia { get; set; }

        [DataMember]
        public Afiliado Afiliado { get; set; }
        [DataMember]
        public Categoria Categoria { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        [DataMember]
        public Supervision Supervision { get; set; }
        [DataMember]
        public Usuario Usuario { get; set; }
        [DataMember]
        public DateTime? FechaCotizacion { get; set; }
        [DataMember]
        public TipoMovimiento TipoMovimiento { get; set; }
        [DataMember]
        public bool ValidarACOM { get; set; }
        [DataMember]
        public bool ValidarDTRA { get; set; }
    
        [DataMember]
        public List<Cotizacion> Cotizaciones { get; set; }

        [DataMember]
        public Int64 NumNivel { get; set; }

        //<INIGTI_4081>
        [DataMember]
        public Compania Compania { get; set; }

        [DataMember]
        public AFP AFP { get; set; }

        [DataMember]
        public string Recotizacion { get; set; }

        //<FINGTI_4081>

        public string XMLSolicitudEscenario()
        {
            XElement root = new XElement("ROOT");
            XElement solicitudEscenario = new XElement("Solicitud");
            
            solicitudEscenario.Add(new XAttribute("num_solicitud", NumSolicitud));
            solicitudEscenario.Add(new XAttribute("num_operacion", NumOperacion));//
            solicitudEscenario.Add(new XAttribute("cod_pje_cesion_comision", CodPjeCesionComision));
            solicitudEscenario.Add(new XAttribute("pje_aumento_comision", PjeAumentoComision));
            solicitudEscenario.Add(new XAttribute("val_tasa_ajuste_tra", ValTasaAjusteTra));
            solicitudEscenario.Add(new XAttribute("ind_condicion_especial", IndCondicionEspecial));
            solicitudEscenario.Add(new XAttribute("ind_aprueba", IndAprueba));
            solicitudEscenario.Add(new XAttribute("fec_cierre", FecCierre.Value.ToString("yyyyMMdd")));
            solicitudEscenario.Add(new XAttribute("num_agente", Agente.Id));
            solicitudEscenario.Add(new XAttribute("cod_username", Usuario.NombreUsuario));
            solicitudEscenario.Add(new XAttribute("ind_estado_seleccion", IndEstadoSeleccion));
            solicitudEscenario.Add(new XAttribute("num_solicitud_copia", NumSolicitudCopia));//
            solicitudEscenario.Add(new XAttribute("val_mto_acom_agente", ValMtoAgenteAcom));
            solicitudEscenario.Add(new XAttribute("num_cotizacion_elegida", NumCotizacionElegida));//

            //<INIGTI_4081>
            solicitudEscenario.Add(new XAttribute("cod_compania", Compania.Id));//
            //<FINGTI_4081>

            root.Add(solicitudEscenario);

            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            xml.Add(root);

            return xml.ToString();
        }

        public string XMLCotizacionesEscenario()
        {
            XElement root = new XElement("ROOT");

            foreach (Cotizacion cotizacion in Cotizaciones)
            {
                XElement solicitudEscenario = new XElement("Cotizacion");

                solicitudEscenario.Add(new XAttribute("num_correlativo", cotizacion.Correlativo));
                solicitudEscenario.Add(new XAttribute("val_tasa_ajuste_tra", cotizacion.AjusteTRA));
                //<INIGTI_4081>
                solicitudEscenario.Add(new XAttribute("pbs", cotizacion.pbs));
                //<FINGTI_4081>
                //<GTI.INI-29372>
                solicitudEscenario.Add(new XAttribute("ind_envio_obligatorio", cotizacion.IndEnvioObligatorio ? "S" : "N"));
                //<GTI.FIN-29372>

                root.Add(solicitudEscenario);
            }

            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            xml.Add(root);

            return xml.ToString();
        }

        public Respuesta Respuesta { get; set; }
    }
}
