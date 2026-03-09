using System;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class Lote
    {
        [DataMember]
        public int Numero { get; set; }
        [DataMember]
        public DateTime FechaCierre { get; set; }
        [DataMember]
        public DateTime FechaEnvio { get; set; }
    }
}
