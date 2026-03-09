using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class Notificacion
    {
        [DataMember]
        public string p_remitente { get; set; }

        [DataMember]
        public string p_displayName { get; set; }

        [DataMember]
        public string p_destinatario { get; set; }

        [DataMember]
        public string p_asunto { get; set; }

        [DataMember]
        public string p_mensaje { get; set; }

        [DataMember]
        public string p_ruta_archivo_adjunto { get; set; }
    }
}
