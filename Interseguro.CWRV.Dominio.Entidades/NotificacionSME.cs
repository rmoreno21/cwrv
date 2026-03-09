using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class NotificacionSME
    {
        [DataMember]
        public string Cliente { get; set; }

        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public string Contrasenha { get; set; }

        [DataMember]
        public string De { get; set; }

        [DataMember]
        public string DeNombre { get; set; }

        [DataMember]
        public string Para { get; set; }

        [DataMember]
        public string ResponderA { get; set; }

        [DataMember]
        public string ResponderANombre { get; set; }

        [DataMember]
        public string Asunto { get; set; }

        [DataMember]
        public string Cuerpo { get; set; }
    }
}
