using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CargaLocalidadVCTP
    {
        [DataMember]
        public DateTime fec_periodo { get; set; }

        [DataMember]
        public int cod_supervisor { get; set; }

        [DataMember]
        public string nom_supervisor { get; set; }

        [DataMember]
        public int cod_jefe { get; set; }

        [DataMember]
        public string nom_jefe { get; set; }

        [DataMember]
        public string gls_localidad { get; set; }

        [DataMember]
        public string aud_usr_ingreso { get; set; }

        [DataMember]
        public string aud_usr_modificacion { get; set; }

    }
}
