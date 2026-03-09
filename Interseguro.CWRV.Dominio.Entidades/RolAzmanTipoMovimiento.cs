using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class RolAzmanTipoMovimiento
    {

        [DataMember]
        public string CodRolAzman { get; set; }
        [DataMember]
        public Int32 CodTipoMovimiento { get; set; }
        [DataMember]
        public string GlsRolAzman { get; set; }

    }
}
