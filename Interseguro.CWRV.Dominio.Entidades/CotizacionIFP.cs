using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CotizacionIFP
    {
        [DataMember]
        public Int64 Correlativo { get; set; }

        [DataMember]
        public int Item { get; set; }

        [DataMember]
        public Moneda Moneda { get; set; }

        [DataMember]
        public Producto Producto { get; set; }

        [DataMember]
        public Modalidad Modalidad { get; set; }

        [DataMember]
        public int PeriodoGarantizado { get; set; }

        [DataMember]
        public double? AjusteTRA { get; set; }

        [DataMember]
        public double PensionCia { get; set; }

        [DataMember]
        public double PensionCiaMO { get; set; }

        [DataMember]
        public double PuurCia { get; set; }

        [DataMember]
        public double TasaVenta { get; set; }

        [DataMember]
        public double TasaVentaSbs { get; set; }

        [DataMember]
        public double TasaRetornoAccionista { get; set; }

        [DataMember]
        public double PagoDoble { get; set; }

        [DataMember]
        public double PjePagoDoble { get; set; }

        [DataMember]
        public string IndGastoSepelio { get; set; }

        [DataMember]
        public double ValPjeDev { get; set; }

        [DataMember]
        public double ValDev { get; set; }

        [DataMember]
        public double ValFacDev { get; set; }

        [DataMember]
        public double ValMtoDev { get; set; }

        [DataMember]
        public double Pension2doTramo { get; set; }

        [DataMember]
        public double Pension2doTramoSinAjuste { get; set; }

        [DataMember]
        public string IndCotiza { get; set; }

        [DataMember]
        public int IndErrorCotiza { get; set; }

        [DataMember]
        public double ValMonAju { get; set; }

        [DataMember]
        public string EstadoCotizacion { get; set; }

        [DataMember]
        public string IndSeleccionada { get; set; }

        [DataMember]
        public string NumSolicitud { get; set; }

        //[DataMember]
        //public double ValTotalPeriodoGarantizado { get; set; }

        [DataMember]
        public double ValTasaCostoEquiv { get; set; }

        [DataMember]
        public double ValPjeDCOM { get; set; }

        [DataMember]
        public bool IndRescate { get; set; }

        [DataMember]
        public Temporalidad Temporalidad { get; set; }

        [DataMember]
        public double ValPerDiferido { get; set; }

        [DataMember]
        public double ValPjeDevFallec { get; set; }

        [DataMember]
        public double ValDevFallec { get; set; }

        [DataMember]
        public double TasaRetornoAccionistaMinima { get; set; }

        [DataMember]
        public Plan Plan { get; set; }

        [DataMember]
        public double CobAdicDevengue { get; set; }

        [DataMember]
        public double CobAdicCapFallecimiento { get; set; }

        [DataMember]
        public double ValPjeFallecNoDeveng { get; set; }

        [DataMember]
        public double ValResPension { get; set; }

        [DataMember]
        public double ValResSepelio { get; set; }

        [DataMember]
        public double ValResDevolucion { get; set; }

        [DataMember]
        public double ValPjeCACy { get; set; }

        [DataMember]
        public double ValPjeCAPa { get; set; }

        [DataMember]
        public double ValPjeCAMa { get; set; }

        [DataMember]
        public string ValPjeCATotal { get; set; }

        [DataMember]
        public double ValPrimeraRentaIS { get; set; }

        [DataMember]
        public double ValResFallecimiento { get; set; }

        [DataMember]
        public double ValPjeConyuge { get; set; }

        [DataMember]
        public List<PjeBen> PorcentajeBeneficiarios { get; set; }
    }
}
