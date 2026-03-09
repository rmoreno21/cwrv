using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class GestionVentas
    {
        [DataMember]
        public string NumeroMeler { get; set; }
        [DataMember]
        public DateTime FechaPlazoAFP { get; set; }
        [DataMember]
        public string CUSPP { get; set; }
        [DataMember]
        public string NombreCliente { get; set; }
        [DataMember]
        public Categoria Categoria { get; set; }
        [DataMember]
        public double CIC { get; set; }
        [DataMember]
        public DateTime? FechaCierre { get; set; }
        [DataMember]
        public Modalidad Modalidad { get; set; }
        [DataMember]
        public string NumeroCotizacion { get; set; }
        [DataMember]
        public double ACOM { get; set; }
        [DataMember]
        public double DCOM { get; set; }
        [DataMember]
        public double DifTra { get; set; }
        [DataMember]
        public double TasaIS { get; set; }
        //[DataMember]
        //public double TasaMeler { get; set; }
        [DataMember]
        public double TasaCiaGanadora { get; set; }
        [DataMember]
        public string Recotizacion { get; set; }
        [DataMember]
        public DateTime FechaCita { get; set; }
        [DataMember]
        public string LugarCita { get; set; }
        [DataMember]
        public string EstadoSolicitud { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        [DataMember]
        public string UbigeoAgente { get; set; }
        [DataMember]
        public string Supervisor { get; set; }
        [DataMember]
        public string Jefe { get; set; }
        [DataMember]
        public string NombreAgencia { get; set; }
        [DataMember]
        public AFP AFP { get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }

        [DataMember]
        public int Num_Supervisor { get; set; }

        [DataMember]
        public int PeriodoDiferido { get; set; }

        [DataMember]
        public int PorcentajeRenta { get; set; }

        [DataMember]
        public int PeriodoGarantizado { get; set; }

        [DataMember]
        public string CompaniaGanadora { get; set; }

        //<INIGTI_4081_3>
        [DataMember]
        public int Num_Jefe { get; set; }
        //<FINGTI_4081_3>
    }
}
