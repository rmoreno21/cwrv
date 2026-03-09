using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class FormatoSolicitud
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public FirmaDigitalLog FirmaDigitalLog { get; set; }
        [DataMember]
        public int Correlativo { get; set; }
        [DataMember]
        public string NumeroSolicitud { get; set; }
        [DataMember]
        public DateTime FechaSolicitud { get; set; }
        [DataMember]
        public long NumeroCotizacion { get; set; }
        [DataMember]
        public FormatoSolicitudBeneficiario Afiliado { get; set; }
        [DataMember]
        public TipoCotizacion TipoCotizacion { get; set; }
        [DataMember]
        public string CodigoPlan { get; set; }
        [DataMember]
        public string CodigoTipoPlanRPP { get; set; }
        [DataMember]
        public Moneda MonedaCIC { get; set; }
        [DataMember]
        public double CIC { get; set; }
        [DataMember]
        public Temporalidad Temporalidad { get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }
        [DataMember]
        public double PorcentajeAjuste { get; set; }
        [DataMember]
        public int MesesTramo1 { get; set; }
        [DataMember]
        public double PorcentajeTramo2 { get; set; }
        [DataMember]
        public int MesesGarantizados { get; set; }
        [DataMember]
        public int IndCoberturaAdicionalFallecimiento { get; set; }
        [DataMember]
        public double PorcentajeDevolucionFallecimiento { get; set; }
        [DataMember]
        public int IndCoberturaAdicionalDevolucion { get; set; }
        [DataMember]
        public double PorcentajeDevolucionSobrevivencia { get; set; }
        [DataMember]
        public char IndSepelio { get; set; }
        [DataMember]
        public double Renta { get; set; }
        [DataMember]
        public double RentaTramo2 { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        [DataMember]
        public char IndConsentimientoNecesario { get; set; }
        [DataMember]
        public char IndConsentimientoOpcional { get; set; }
        [DataMember]
        public double PorcentajeConyuge { get; set; }
        [DataMember]
        public DateTime FechaVigencia { get; set; }
        [DataMember]
        public bool Rescate { get; set; }
        [DataMember]
        public int Origen { get; set; }
        [DataMember]
        public string DeclaracionJurada { get; set; }
        [DataMember]
        public Direccion Direccion { get; set; }
    }
}
