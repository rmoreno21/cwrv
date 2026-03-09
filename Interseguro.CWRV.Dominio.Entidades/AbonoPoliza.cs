using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class AbonoPoliza
    {
        [DataMember]
        public int? num_imputacion_mov { get; set; }
        [DataMember]
        public string cod_concepto_abono_cargo { get; set; }
        [DataMember]
        public double? val_pesos_abono { get; set; }
    }
}
