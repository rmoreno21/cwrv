using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Feriado
    {
        [DataMember]
        public int Anio { get; set; }
        [DataMember]
        public string Fecha { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
    }
}
