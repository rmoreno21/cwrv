using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConfiguracionCorreo
    {
        [DataMember]
        public int cod_proceso { get; set; }
        [DataMember]
        public DateTime fec_inicio_vigencia { get; set; }
        [DataMember]
        public DateTime fec_termino_vigencia { get; set; }
        [DataMember]
        public string gls_configuracion { get; set; }
        [DataMember]
        public string arc_documento_correo { get; set; }
        [DataMember]
        public string gls_asunto { get; set; }
        [DataMember]
        public string gls_remitente { get; set; }
        [DataMember]
        public string gls_display_name { get; set; }
        [DataMember]
        public string gls_destinatario { get; set; }
        [DataMember]
        public string gls_ruta_servicio { get; set; }
        [DataMember]
        public string enviar_correo { get; set; } //S=Si, N=No
    }
}
