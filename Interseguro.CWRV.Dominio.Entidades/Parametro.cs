using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Parametro
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public int Correlativo { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Glosa { get; set; }
        [DataMember]
        public double Valor { get; set; }
        //<SRI.INI-20322_E2>
        [DataMember]
        public string Valor_1 { get; set; }
        [DataMember]
        public string Valor_2 { get; set; }
        //<SRI.FIN-20322_E2>
        [DataMember]
        public DateTime? FecInicioVigencia { get; set; }
        [DataMember]
        public DateTime? FecFinVigencia { get; set; }

        //<SOLINI25621>
        [DataMember]
        public string UsuarioModificacion { get; set; }
        //<SOLFIN25621>
    }
}
