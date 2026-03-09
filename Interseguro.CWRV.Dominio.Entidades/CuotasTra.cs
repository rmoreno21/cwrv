using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CuotasTra
    {
        //[DataMember]
        //public string RolAzman { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        [DataMember]
        public DateTime? FecInicioVigencia { get; set; }
        [DataMember]
        public DateTime? FecFinVigencia { get; set; }
        [DataMember]
        public int NroCasosTotal { get; set; }
        [DataMember]
        public int NroCasosSolicitados { get; set; }
        [DataMember]
        public int NroCasosEfectivos { get; set; }

        [DataMember]
        public DateTime? FecCotizacion { get; set; }

        [DataMember]
        public string FecInicioVigenciaStr { get; set; }
        [DataMember]
        public string FecFinVigenciaStr { get; set; }



        public string XMLCuotas(List<CuotasTra> lstCuotas)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("ROOT");

            XElement parametrosEspecial;
            foreach (var cuo in lstCuotas)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                atributos.Add(new XAttribute("num_agente", cuo.Agente.Id));
                atributos.Add(new XAttribute("fec_inicio_vigencia", cuo.FecInicioVigencia.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("fec_fin_vigencia", cuo.FecFinVigencia.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("nro_casos_total", cuo.NroCasosTotal));
                parametrosEspecial = new XElement("Cuotas", atributos);
                root.Add(parametrosEspecial);
            }
            xml.Add(root);

            return xml.ToString();
        }

    }
}
