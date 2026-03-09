using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CorreoElectronico
    {
        [DataMember]
        public string De { get; set; }
        [DataMember]
        public string DeNombre { get; set; }
        [DataMember]
        public string Para { get; set; }
        [DataMember]
        public string ParaNombre { get; set; }
        [DataMember]
        public string Asunto { get; set; }
        [DataMember]
        public string Adjunto { get; set; }
        [DataMember]
        public bool Html { get; set; }
        [DataMember]
        public byte[] BinarioAdjunto { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
        [DataMember]
        public Respuesta Respuesta { get; set; }
    }
}
