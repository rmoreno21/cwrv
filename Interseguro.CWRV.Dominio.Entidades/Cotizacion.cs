using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Cotizacion
    {
        [DataMember]
        public Int64 Correlativo { get; set; }
        [DataMember]
        public DateTime? FechaCotizacion  { get; set; }
        [DataMember]
        public string EstadoCotizacion { get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }
        [DataMember]
        public double ValorMoneda { get; set; }
        [DataMember]
        public Producto Producto { get; set; }
        [DataMember]
        public Modalidad Modalidad { get; set; }
        [DataMember]
        public int PeriodoDiferido { get; set; }
        [DataMember]
        public int PorcentajeEntreRentas { get; set; }
        [DataMember]
        public int PeriodoGarantizado { get; set; }
        [DataMember]
        public bool DerechoCrecer { get; set; }
        [DataMember]
        public bool Gratificacion { get; set; }
        [DataMember]
        public Capital Capital { get; set; }
        [DataMember]
        public double? AjusteTRA { get; set; }

        [DataMember]
        public double MontoCia { get; set; }
        [DataMember]
        public double PensionCia { get; set; }
        [DataMember]
        public double PensionCiaMO { get; set; }
        [DataMember]
        public double PuurCia { get; set; }
        [DataMember]
        public double TasaAFP { get; set; }
        [DataMember]
        public double MontoAFP { get; set; }
        [DataMember]
        public double PensionAFP { get; set; }
        [DataMember]
        public double PuurAFP { get; set; }
        //<SRI.INI-20322>
        [DataMember]
        public double TasaVenta { get; set; }
        [DataMember]
        public double TasaVentaSbs { get; set; }
        [DataMember]
        public double TasaVentaSbs2 { get; set; }
        [DataMember]
        public double TasaCostoEquivalente { get; set; }
        [DataMember]
        public double TasaCostoEquivalente2 { get; set; }
        [DataMember]
        public double Duration { get; set; }
        [DataMember]
        public double Duration2 { get; set; }
        [DataMember]
        public TipoCalculo TipoCalculo { get; set; }
        [DataMember]
        public double TasaRetornoAccionista { get; set; }
        [DataMember]
        public double TotalGarantizado { get; set; }
        //<SRI.FIN-20322>
        //<SRIINI25781>
        [DataMember]
        public double TasaVentaMaxima { get; set; }
        [DataMember]
        public double TasaRetornoAccionistaMinimo { get; set; }
        [DataMember]
        public string IndCotiza { get; set; }
        [DataMember]
        public int IndErrorCotiza { get; set; }
        [DataMember]

        public int IndErrorCotiza2 { get; set; }
        //<SRIFIN25781>
        //<SRI.INI-20322_E2>
        [DataMember]
        public CotizacionMovimiento CotizacionMovimiento { get; set; }
        //<SRI.FIN-20322_E2>

        [DataMember]
        public double PrimaUnicaAFPEESS { get; set; }
        [DataMember]
        public double PrimaUnicaEESS { get; set; }
        [DataMember]
        public int PorcentajeConyuge { get; set; }
        [DataMember]
        public bool IndCotizacion { get; set; }
        [DataMember]
        public string Cotiza { get; set; }

        [DataMember]
        public double PrimeraPensionRV { get; set; }
        [DataMember]
        public double TasaInteresRV { get; set; }
        [DataMember]
        public double PrimeraPensionRT { get; set; }
        [DataMember]
        public double TasaInteresRT { get; set; }
        [DataMember]
        public double PrimeraPensionRVD { get; set; }
        [DataMember]
        public double TasaInteresRVD { get; set; }

        //<INIGTI_4081>
        [DataMember]
        public double pbs { get; set; }

        [DataMember]
        public double TasaVentaSbsObjetivo { get; set; }
        [DataMember]
        public double TasaRetornoAccionistaObjetivo { get; set; }
        [DataMember]
        public double PensionCiaObjetivo { get; set; }

        [DataMember]
        public double TasaVentaSbsOrigen { get; set; }
        [DataMember]
        public double PensionCiaOrigen { get; set; }
        [DataMember]
        public double PensionCiaMOOrigen { get; set; }
        //<FINGTI_4081>

        //<INIGTI_4081_2>
        [DataMember]
        public Int64 NumMovimiento { get; set; }//de cwrv_cotiza_movimiento
        //<FINGTI_4081_2>

        //<INIGTI_6623>
        [DataMember]
        public double PensionCiaMOObjetivo { get; set; }

        [DataMember]
        public double PensionAFPOrigen { get; set; }

        [DataMember]
        public double PensionAFPObjetivo { get; set; }
        //<FINGTI_6623>

        //<GTI.INI-29372>
        [DataMember]
        public bool IndEnvioObligatorio { get; set; }
        //<GTI.FIN-29372>
    }
}
