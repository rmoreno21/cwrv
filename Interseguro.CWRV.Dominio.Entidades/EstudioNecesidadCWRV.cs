using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class EstudioNecesidadCWRV
    {
        [DataMember]
        public int id_estudio_necesidades { get; set; }
        [DataMember]
        public int cod_formato { get; set; }
        [DataMember]
        public string gls_texto_formato_cotizacion { get; set; }
        [DataMember]
        public DateTime fec_inicio_vigencia { get; set; }
        [DataMember]
        public DateTime fec_fin_vigencia { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }
    }
}







