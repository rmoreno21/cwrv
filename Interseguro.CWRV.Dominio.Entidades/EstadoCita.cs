using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class EstadoCita
    {

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public DateTime? FecEstadoUsuario { get; set; }
        [DataMember]
        public DateTime? FecEstadoSistema { get; set; }

    }
}
