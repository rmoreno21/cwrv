using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class RviBdIni
    {
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }
        [DataMember]
        public string aud_usr_modificacion { get; set; }
        [DataMember]
        public DateTime? aud_fec_modificacion { get; set; }
        [DataMember]
        public string num_cuspp { get; set; }
        [DataMember]
        public string cod_tipo_identificacion { get; set; }
        [DataMember]
        public string num_identificacion { get; set; }
        [DataMember]
        public string gls_nombres { get; set; }
        [DataMember]
        public string gls_apellido_paterno { get; set; }
        [DataMember]
        public string gls_apellido_materno { get; set; }
        [DataMember]
        public string cod_sexo { get; set; }
        [DataMember]
        public DateTime? fec_nacimiento { get; set; }
        [DataMember]
        public string cod_afp { get; set; }
        [DataMember]
        public string gls_afp { get; set; }
        [DataMember]
        public double? val_cic { get; set; }
        [DataMember]
        public string cod_categoria { get; set; }
        [DataMember]
        public string gls_categoria { get; set; }
        [DataMember]
        public string gls_mail { get; set; }
        [DataMember]
        public string gls_telefono { get; set; }
        [DataMember]
        public string gls_celular { get; set; }
        [DataMember]
        public string cod_estado_civil { get; set; }
        [DataMember]
        public string gls_estado_civil { get; set; }
        [DataMember]
        public string gls_rango_inversion { get; set; }
        [DataMember]
        public string gls_centro_laboral { get; set; }
        [DataMember]
        public int? num_agente { get; set; }
        [DataMember]
        public string cod_cartera { get; set; }
        [DataMember]
        public string gls_mail_cliente { get; set; }
        [DataMember]
        public string gls_telefono_cliente { get; set; }
        [DataMember]
        public string gls_celular_cliente { get; set; }
        [DataMember]
        public string ind_invalidez { get; set; }
        [DataMember]
        public string cod_tipo_invalidez { get; set; }
    }
}
