using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class EmisionPoliza
    {
        [DataMember]
        public SolicitudRPPlus SolicitudRPPlus { get; set; }
        [DataMember]
        public Poliza Poliza { get; set; }
        [DataMember]
        public double PjeComision { get; set; }
        [DataMember]
        public string gls_PjeComision { get; set; }

        [DataMember]
        public double ValGastoSepelio { get; set; }

        [DataMember]
        public SolicitudIFP SolicitudIFP { get; set; }
    }
}
