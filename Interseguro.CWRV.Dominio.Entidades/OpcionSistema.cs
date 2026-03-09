using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class OpcionSistema
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdAzman { get; set; }
        [DataMember]
        public int IdPadre { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public int Orden { get; set; }
        [DataMember]
        public string Ruta { get; set; }
        [DataMember]
        public string RutaIcono { get; set; }
        [DataMember]
        public string Titulo { get; set; }
        [DataMember]
        public string ToolTip { get; set; }
        [DataMember]
        public string TipoOpcion { get; set; }
        [DataMember]
        public bool Activa { get; set; }
    }
}
