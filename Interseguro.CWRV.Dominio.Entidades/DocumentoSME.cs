using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class DocumentoSME
    {
        public string Email { get; set; }
        public string NumeroPoliza { get; set; }
        public string NumeroDocumento { get; set; }
        public string Destinatario { get; set; }
        public int ProcesoSme { get; set; }
        public string RutaPdf { get; set; }
        public string Contrasenia { get; set; }
        public dynamic CamposDinamicos { get; set; }
        public long CodigoSME { get; set; }
    }
}
