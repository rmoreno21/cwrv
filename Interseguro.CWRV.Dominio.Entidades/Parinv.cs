using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class Parinv
    {
        [DataMember]
        public string IdMoneda { get; set; }
        public List<XElement> lParinv { get; set; }

        //<INIGTI_753>
        public string IndDevolucion { get; set; }
        //<FINGTI_753>
    }
}
