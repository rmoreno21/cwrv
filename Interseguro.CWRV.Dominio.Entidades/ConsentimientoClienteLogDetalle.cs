using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConsentimientoClienteLogDetalle
    {
        [DataMember]
        public int id_consentimiento_log_detalle { get; set; }
        [DataMember]
        public int id_consentimiento_log { get; set; }
        [DataMember]
        public int id_tratamiento { get; set; }
        [DataMember]
        public string ind_tratamiento { get; set; }
    }
}
