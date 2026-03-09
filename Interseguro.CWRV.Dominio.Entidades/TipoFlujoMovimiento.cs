using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [DataContract]
    [Serializable]
    public class TipoFlujoMovimiento
    {

        [DataMember]
        public int? cod_tipo_flujo_evaluacion { get; set; }

        [DataMember]
        public string gls_tipo_flujo_evaluacion { get; set; }

        [DataMember]
        public string aud_usr_ingreso { get; set; }

        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }

        [DataMember]
        public string aud_usr_modificacion { get; set; }

        [DataMember]
        public DateTime? aud_fec_modificacion { get; set; }


    }

}
