using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class TipoMovimiento
    {
        [DataMember]
        public Int16 Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }
    }
}
