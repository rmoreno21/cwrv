using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Telefono
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public TipoTelefono Tipo { get; set; }
        [DataMember]
        public string Numero { get; set; }
        [DataMember]
        public DateTime FechaIngreso { get; set; }
        [DataMember]
        public bool Principal { get; set; }
        [DataMember]
        public Afiliado Afiliado { get; set; }
        [DataMember]
        public Usuario Usuario { get; set; }
    }
}
