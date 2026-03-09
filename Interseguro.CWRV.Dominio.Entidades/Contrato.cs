using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class Contrato : BaseAuditoria
    {
        [DataMember]
        public int IdContratoCotizacion { get; set; }
        [DataMember]
        public string GlsContratoCotizacion { get; set; }
        [DataMember]
        public DateTime FecInicio { get; set; }
        [DataMember]
        public DateTime FecFin { get; set; }
        [DataMember]
        public string CodAfp { get; set; }
        [DataMember]
        public string GlsAfp { get; set; }
    }
}
