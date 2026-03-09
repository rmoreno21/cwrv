using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Afiliado
    {
        public Afiliado()
        {
            TipoIdentificacion = string.Empty;
            NumeroIdentificacion = string.Empty;
            ApellidoPaterno = string.Empty;
            ApellidoMaterno = string.Empty;
            Nombre = string.Empty;
            FechaNacimiento = null;
            Sexo = null;
        }

        [DataMember]
        public string CUSPP { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public DateTime? FechaNacimiento { get; set; }
        [DataMember]
        public char? Sexo { get; set; }
        [DataMember]
        public string CorreoElectronico { get; set; }
        [DataMember]
        public Categoria Categoria { get; set; }
        [DataMember]
        public AFP AFP { get; set; }
        [DataMember]
        public double? SaldoCIC { get; set; }
        [DataMember]
        public string NombreEmpresa { get; set; }
        [DataMember]
        public string DireccionEmpresa { get; set; }
        [DataMember]
        public Ciudad CiudadEmpresa { get; set; }
        [DataMember]
        public Comuna ComunaEmpresa { get; set; }
        [DataMember]
        public string TelefonoEmpresa { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        //<SRIINI06326>
        [DataMember]
        public bool Consentimiento { get; set; }
        [DataMember]
        public DateTime FechaConsentimiento { get; set; }
        //<SRIFIN06326>

        [DataMember]
        public Respuesta Respuesta { get; set; }

        //<INIGTI_7012>
        [DataMember]
        public Temporal EstadoCivil { get; set; }
        [DataMember]
        public Parametro ConfidencialidadDatos { get; set; }
        //<FINGTI_7012>

        //<INIGTI_7012_S24>
        [DataMember]
        public string NumeroIdentificacion { get; set; }
        //<FINGTI_7012_S24>

        [DataMember]
        public string TipoIdentificacion { get; set; }

        //[DataMember]
        //public ConsentimientoAsesoria consentimientoAsesoria { get; set; }
        [DataMember]
        public string IdConsentimientoAsesoria { get; set; }

        [DataMember]
        public string Telefonos { get; set; }
        [DataMember]
        public string Celulares { get; set; }

        [DataMember]
        public string TelefonoCliente { get; set; }
        [DataMember]
        public string CelularCliente { get; set; }
        [DataMember]
        public string CorreoElectronicoCliente { get; set; }
        [DataMember]
        public string RangoInversion { get; set; }
        [DataMember]
        public string CentroLaboral { get; set; }
        
    }
}
