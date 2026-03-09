using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace Interseguro.CWRV.Dominio.Entidades
{
    //<INIGTI_4081>
    [DataContract]
    public class FlujoMovimiento
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string GlsMovimiento { get; set; }
        [DataMember]
        public string MsjAlerta { get; set; }

        [DataMember]
        public string Rol { get; set; }

        [DataMember]
        public string Evento { get; set; }

        [DataMember]
        public int Origen { get; set; }

        [DataMember]
        public int Destino { get; set; }

        [DataMember]
        public string MsjFlujo { get; set; }

    }
    //<INIGTI_4081>
}
