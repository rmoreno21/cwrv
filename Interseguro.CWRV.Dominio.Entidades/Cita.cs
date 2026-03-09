using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class Cita
    {
        //<GTIINI-6623>
        [DataMember]
        public Afiliado Afiliado { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        [DataMember]
        public DateTime FechaProgramada { get; set; }
        [DataMember]
        public DateTime FechaReal { get; set; }
        [DataMember]
        public DateTime UltimaCitaEfectiva { get; set; }
        [DataMember]
        public EstadoCita EstadoCita { get; set; }
        [DataMember]
        public Pronostico Pronostico { get; set; }
        [DataMember]
        public double AvanceCartera { get; set; }
        //<GTIFIN-6623>

        [DataMember]
        public List<Parametro> Parametro { get; set; }
        [DataMember]
        //<GTIINI-6623>
        public DateTime Fecha { get; set; }
        //<GTIFIN-6623>


        public string XMLParametroEstadoCita()
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("ROOT");

            XElement estado;
            foreach (Parametro estadoCita in Parametro)
            {
                List<XAttribute> atributos = new List<XAttribute>();
                atributos.Add(new XAttribute("valor_1", estadoCita.Valor_1));
                if (estadoCita.Valor_2 != null)
                    atributos.Add(new XAttribute("valor_2", estadoCita.Valor_2));

                estado = new XElement("Estado", atributos);

                root.Add(estado);
            }
            xml.Add(root);

            return xml.ToString();
        }

        public Respuesta Respuesta { get; set; }

    }
}
