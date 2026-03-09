//<SRIINI06326>
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class MontoCIC : BaseAuditoria 
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
    }
}
//<SRIFIN06326>