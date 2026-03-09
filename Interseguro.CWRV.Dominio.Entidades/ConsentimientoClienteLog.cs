using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConsentimientoClienteLog
    {
        [DataMember]
        public int id_consentimiento_log { get; set; }
        [DataMember]
        public string gls_mail { get; set; }
        [DataMember]
        public string gls_telefono { get; set; }
        [DataMember]
        public string gls_celular { get; set; }
        [DataMember]
        public List<ConsentimientoClienteLogDetalle> LogDetalle { get; set; }
}
}
