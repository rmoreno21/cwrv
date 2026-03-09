using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    public class ValPar : BaseAuditoria
    {
        [DataMember]
        public DateTime fec_ini_rango { get; set; }
        [DataMember]
        public DateTime fec_fin_rango { get; set; }
        [DataMember]
        public string cod_parametro { get; set; }
        [DataMember]
        public string cod_moneda { get; set; }
        [DataMember]
        public int num_tramo { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
        [DataMember]
        public double val_parametro { get; set; }
        [DataMember]
        public string cod_tipo_pension { get; set; }
        [DataMember]
        public string cod_departamento { get; set; }
        [DataMember]
        public string ind_origen { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }
        [DataMember]
        public string aud_usr_modificacion { get; set; }
        [DataMember]
        public DateTime? aud_fec_modificacion { get; set; }
    }
}