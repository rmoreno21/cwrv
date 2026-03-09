using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ParametroEspecial
    {
        [DataMember]
        public string Id{ get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }
        [DataMember]
        public DateTime? FecIniRango{ get; set; }
        [DataMember]
        public DateTime? FecFinRango { get; set; }
        [DataMember]
        public double NumTramo { get; set; }
        [DataMember]
        public double ValParametro { get; set; }
        [DataMember]
        public double ValAjuTra{ get; set; }
        [DataMember]
        public double ValDiferencia { get; set; }

        [DataMember]
        public string NumSolicitud { get; set; }

        [DataMember]
        public string FecIniRangoStr { get; set; }
        [DataMember]
        public string FecFinRangoStr { get; set; }
        
    

        public string XMLParametro(List<ParametroEspecial> parametros)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("ROOT");

            XElement parametrosEspecial;
            foreach (ParametroEspecial par in parametros)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                atributos.Add(new XAttribute("num_solicitud", par.NumSolicitud));
                atributos.Add(new XAttribute("cod_parametro", par.Id));
                atributos.Add(new XAttribute("cod_moneda", par.Moneda.Id));
                atributos.Add(new XAttribute("num_tramo", par.NumTramo));
                atributos.Add(new XAttribute("fec_ini_rango", par.FecIniRango.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("fec_fin_rango", par.FecFinRango.Value.ToString("yyyyMMdd")));
                atributos.Add(new XAttribute("val_parametro", par.ValParametro));
                atributos.Add(new XAttribute("val_diferencia", par.ValDiferencia));

                parametrosEspecial = new XElement("Solicitud", atributos);

                root.Add(parametrosEspecial);
            }
            xml.Add(root);

            return xml.ToString();
        }
    }
}
