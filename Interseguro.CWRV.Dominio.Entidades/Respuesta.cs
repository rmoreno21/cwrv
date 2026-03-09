using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
//<SRIINI06326>
using System.Data;
//<SRIFIN06326>

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    [Serializable]
    public class Respuesta
    {
        [DataMember]
        public string Estado { get; set; }
        [DataMember]
        public string Titulo { get; set; }
        [DataMember]
        public string Icono { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
        [DataMember]
        public List<string> Controles { get; set; }
        
        [DataMember]
        public string Contenido { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico1 { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico2 { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico3 { get; set; }
        //<SRI.INI-20322>
        [DataMember]
        public List<GraficoLineal> Grafico4 { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico5 { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico6 { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico7 { get; set; }
        [DataMember]
        public List<GraficoLineal> Grafico8 { get; set; }
        //<SRI.FIN-20322>

        [DataMember]
        public DateTime FechaHora { get; set; }

        //<SRIINI06326>
        [DataMember]
        public DataSet Data { get; set; }
        //<SRIFIN06326>

        [DataMember]
        public string ArchivoSerializado { get; set; }


        [DataMember]
        public List<byte[]> ArchivosByte { get; set; }
        [DataMember]
        public List<string> Archivos { get; set; }

    }
}
