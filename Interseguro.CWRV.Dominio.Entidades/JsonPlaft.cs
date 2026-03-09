using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class JsonTokenPlaft
    {
        [DataMember]
        public _meta _meta { get; set; }
        [DataMember]
        public records_token records { get; set; }
    }


    [DataContract]
    public class JsonPropuesta
    {
        [DataMember]
        public _meta _meta { get; set; }
        [DataMember]
        public records_propuesta records { get; set; }
    }


    [DataContract]
    public class JsonEmail
    {
        [DataMember]
        public _meta _meta { get; set; }
        [DataMember]
        public List<records_email> records { get; set; }
    }


    [DataContract]
    public class JsonCoincidencia
    {
        [DataMember]
        public _meta _meta { get; set; }
        [DataMember]
        public records_coincidencia records { get; set; }
    }

    [DataContract]
    public class JsonCoincidenciaLN
    {
        [DataMember]
        public _meta _meta { get; set; }
        [DataMember]
        public records_coincidencia_ln records { get; set; }
    }

    [DataContract]
    public class JsonRechazo
    {
        [DataMember]
        public _meta _meta { get; set; }
        [DataMember]
        public records_rechazo records { get; set; }
    }

    [DataContract]
    public class _meta
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public int count { get; set; }
    }

    [DataContract]
    public class records_token
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public int expires_in { get; set; }
    }

    [DataContract]
    public class records_propuesta
    {
        [DataMember]
        public int imprimible { get; set; }
        [DataMember]
        public string descripcion { get; set; }
        [DataMember]
        public int codigo { get; set; }
    }

    [DataContract]
    public class records_email
    {
        [DataMember]
        public string nombre { get; set; }
        [DataMember]
        public string email { get; set; }
    }

    [DataContract]
    public class records_coincidencia
    {
        [DataMember]
        public int coincidencia { get; set; }
    }

    [DataContract]
    public class records_coincidencia_ln
    {
        [DataMember]
        public int PEP { get; set; }
        [DataMember]
        public int LN { get; set; }
    }

    [DataContract]
    public class Propuesta
    {
        [DataMember]
        public string client_id { get; set; }
        [DataMember]
        public string propuesta { get; set; }
        [DataMember]
        public double prima_anualizada { get; set; }
        [DataMember]
        public string producto { get; set; }
        [DataMember]
        public string moneda { get; set; }//
        [DataMember]
        public string contratante_tipo_documento { get; set; }
        [DataMember]
        public string contratante_documento { get; set; }
        [DataMember]
        public string contratante_actividad_economica { get; set; }
        [DataMember]
        public string contratante_profesion { get; set; }
        [DataMember]
        public string contratante_sujeto_obligado { get; set; }
        [DataMember]
        public string contratante_residencia { get; set; }
        [DataMember]
        public string contratante_nacionalidad { get; set; }
        [DataMember]
        public string contratante_pep { get; set; }
        [DataMember]
        public string contratante_nombre1 { get; set; }
        [DataMember]
        public string contratante_nombre2 { get; set; }
        [DataMember]
        public string contratante_nombre3 { get; set; }
        [DataMember]
        public string contratante_apellido_paterno { get; set; }
        [DataMember]
        public string contratante_apellido_materno { get; set; }
        [DataMember]
        public string contratante_razon_social { get; set; }
        [DataMember]
        public string contratante_domicilio { get; set; }
        [DataMember]
        public string contratante_cargo { get; set; }
        [DataMember]
        public string contratante_centro_labores { get; set; }
        [DataMember]
        public string contratante_ingreso_mensual { get; set; }
        [DataMember]
        public DateTime contratante_fec_nacimiento { get; set; }

        [DataMember]
        public string asegurado_tipo_documento { get; set; }
        [DataMember]
        public string asegurado_documento { get; set; }
        [DataMember]
        public string asegurado_actividad_economica { get; set; }
        [DataMember]
        public string asegurado_profesion { get; set; }
        [DataMember]
        public string asegurado_sujeto_obligado { get; set; }
        [DataMember]
        public string asegurado_residencia { get; set; }
        [DataMember]
        public string asegurado_nacionalidad { get; set; }
        [DataMember]
        public string asegurado_pep { get; set; }
        [DataMember]
        public string asegurado_nombre1 { get; set; }
        [DataMember]
        public string asegurado_nombre2 { get; set; }
        [DataMember]
        public string asegurado_nombre3 { get; set; }
        [DataMember]
        public string asegurado_apellido_paterno { get; set; }
        [DataMember]
        public string asegurado_apellido_materno { get; set; }
        [DataMember]
        public string asegurado_razon_social { get; set; }

        [DataMember]
        public string usuario { get; set; }

        [DataMember]
        public string CodigoEstadoPlaft { get; set; }

        [DataMember]
        public string ArchivosExistentes { get; set; }

        [DataMember]
        public string CodigoEstado { get; set; }

        [DataMember]
        public List<Agente> Agentes { get; set; }

        [DataMember]
        public string CodCanalDistribucion { get; set; }
    }

    [DataContract]
    public class records_rechazo
    {
        [DataMember]
        public int rechazo { get; set; }
        [DataMember]
        public int errorCode { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public string more { get; set; }
    }

}
