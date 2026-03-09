using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class OrigenFondo : BaseAuditoria
    {
        [DataMember]
        public int idOrigenFondo { get; set; }
        [DataMember]
        public string numSolicitud { get; set; }
        [DataMember]
        public string declaracionJurada { get; set; }
    }
}
