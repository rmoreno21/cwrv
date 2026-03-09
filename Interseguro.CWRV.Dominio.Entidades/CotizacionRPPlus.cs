using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CotizacionRPPlus
    {
        [DataMember]
        public long Correlativo { get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }
        [DataMember]
        public double ValMonAju { get; set; }
        [DataMember]
        public Producto Producto { get; set; }
        [DataMember]
        public Modalidad Modalidad { get; set; }
        [DataMember]
        public int PeriodoDiferido { get; set; } // Para uso futuro
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
        public double PagoEscalonada { get; set; }

        [DataMember]
        public double PjePE { get; set; }

        [DataMember]
        public string IndGastoSepelio { get; set; }

        [DataMember]
        public double ValPjeDev { get; set; }

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
        public double ValPjeConyuge { get; set; }

        [DataMember]
        public string EstadoCotizacion { get; set; }

        [DataMember]
        public string IndSeleccionada { get; set; }

        [DataMember]
        public string NumSolicitud { get; set; }
        [DataMember]
        public double ValTotalPeriodoGarantizado { get; set; }

        [DataMember]
        public double ValTasaCostoEquiv { get; set; }

        [DataMember]
        public bool Gratificacion { get; set; }// Para uso futuro

        [DataMember]
        public List<PjeBen> PorcentajeBeneficiarios { get; set; }

        [DataMember]
        public double? Val_prima_unica_sepelio { get; set; }
        [DataMember]
        public double? Val_prima_unica_pension { get; set; }
        [DataMember]
        public double? Val_prima_unica_devolucion { get; set; }
        [DataMember]
        public double? Val_prima_unica_fallecimiento { get; set; }
    }
}
