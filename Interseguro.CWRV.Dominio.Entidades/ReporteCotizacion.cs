using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    
    public class ReporteCotizacion
    {

        [DataMember]
        public int? cod_Id { get; set; }
        
        [DataMember]
        public string gls_Titulo { get; set; }
        
        [DataMember]
        public string gls_Descripcion { get; set; }
        
        [DataMember]
        public int? num_Tamanho { get; set; }
        
        [DataMember]
        public int? num_Posicion { get; set; }
        
        [DataMember]
        public string gls_Negrita { get; set; }
        
        [DataMember]
        public string gls_Color { get; set; }
        
        [DataMember]
        public string gls_Alineamiento { get; set; }

    }

}
