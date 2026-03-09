using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConsentimientoCliente
    {
        [DataMember]
        public int id_consentimiento_asesoria { get; set; }
        [DataMember]
        public int id_configuracion { get; set; }
        [DataMember]
        public string gls_token { get; set; }
        [DataMember]
        public string cod_tipo_identificacion { get; set; }
        [DataMember]
        public string gls_num_identificacion { get; set; }
        //[DataMember]
        //public string cod_producto { get; set; }

        [DataMember]
        public string gls_nombres { get; set; }
        [DataMember]
        public string gls_apellido_paterno { get; set; }
        [DataMember]
        public string gls_apellido_materno { get; set; }
        [DataMember]
        public string gls_sexo { get; set; }
        [DataMember]
        public DateTime? fec_nacimiento { get; set; }
        [DataMember]
        public string gls_mail { get; set; }
        [DataMember]
        public string gls_telefono { get; set; }
        [DataMember]
        public string gls_celular { get; set; }
        [DataMember]
        public string gls_campos_dinamicos { get; set; }
        [DataMember]
        public string ind_consentimiento { get; set; }
        [DataMember]
        public DateTime? fec_ultimo_consentimiento { get; set; }
        [DataMember]
        public string gls_url_exito { get; set; }
        [DataMember]
        public string gls_url_error { get; set; }
        [DataMember]
        public string gls_nombres_agente { get; set; }
        [DataMember]
        public string gls_mail_agente { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public string aud_usr_modificacion { get; set; }
        [DataMember]
        public ConsentimientoDireccion direccion { get; set; }
        [DataMember]
        public ConsentimientoClienteLog Log { get; set; }
        [DataMember]
        public int? num_agente { get; set; }
        [DataMember]
        public DateTime? fec_primer_envio { get; set; }
        [DataMember]
        public DateTime? fec_ultimo_envio { get; set; }
    }

}
