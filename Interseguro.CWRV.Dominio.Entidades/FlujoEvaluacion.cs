using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [DataContract]
    public class FlujoEvaluacion
    {

        [DataMember]
        public string num_solicitud { get; set; }

        [DataMember]
        public int? num_correlativo { get; set; }

        [DataMember]
        public int? cod_tipo_flujo_evaluacion { get; set; }

        [DataMember]
        public string gls_observacion { get; set; }

        [DataMember]
        public DateTime? fec_inicio_flujo_evaluacion { get; set; }

        [DataMember]
        public DateTime? fec_fin_flujo_evaluacion { get; set; }

        [DataMember]
        public string gls_archivos_existentes { get; set; }

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
