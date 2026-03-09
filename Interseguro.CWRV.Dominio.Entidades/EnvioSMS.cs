using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class EnvioSMS
    {
        public int codProveedor { get; set; }
        public string numCelular { get; set; }
        public string mensaje { get; set; }
    }
}
