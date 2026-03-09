using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Usuario
    {
        public Usuario()
        {
            //this.Asistencias = new HashSet<Asistencia>();
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string NombreUsuario { get; set; }
        [DataMember]
        public string CodigoEmpleado { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Rol { get; set; }
        //<SRI.INI-20322_E2>
        [DataMember]
        public DateTime? FecIngreso { get; set; }
        //<SRI.FIN-20322_E2>

    }
}
