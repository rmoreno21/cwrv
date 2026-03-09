using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    [Serializable]
    public class SolicitudesCerradas
    {
        [DataMember]
        public int id_solicitudes_cerradas { get; set; }

        [DataMember]
        public string num_solicitud { get; set; }

        [DataMember]
        public DateTime fec_inicio_plazo { get; set; }

        [DataMember]
        public DateTime fec_fin_plazo { get; set; }

        [DataMember]
        public string ind_envio_automatico_VCTP { get; set; }

        [DataMember]
        public string aud_usr_ingreso { get; set; }

        [DataMember]
        public DateTime aud_fec_ingreso { get; set; }

        [DataMember]
        public string aud_usr_modificacion { get; set; }

        [DataMember]
        public DateTime aud_fec_modificacion { get; set; }
    }
}
