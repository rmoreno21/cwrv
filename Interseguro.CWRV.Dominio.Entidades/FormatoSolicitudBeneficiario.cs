using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class FormatoSolicitudBeneficiario
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdFormatoSolicitud { get; set; }
        [DataMember]
        public int Correlativo { get; set; }
        [DataMember]
        public string NumeroSolicitud { get; set; }
        [DataMember]
        public int Item { get; set; }
        [DataMember]
        public string Nombres { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public DateTime FechaNacimiento  { get; set; }
        [DataMember]
        public Identificacion Identificacion { get; set; }
        [DataMember]
        public Parentesco Parentesco { get; set; }
        [DataMember]
        public string Sexo { get; set; }
        [DataMember]
        public EstadoCivil EstadoCivil { get; set; }
        [DataMember]
        public Nacionalidad Nacionalidad { get; set; }
        [DataMember]
        public Departamento Residencia { get; set; }
        [DataMember]
        public Profesion Profesion { get; set; }
        [DataMember]
        public string CentroLaboral { get; set; }
        [DataMember]
        public string Cargo { get; set; }
        [DataMember]
        public string ActividadEconomica { get; set; }
        [DataMember]
        public Moneda MonedaIngreso { get; set; }
        [DataMember]
        public double IngresoNeto { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public string Celular { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public char PEP { get; set; }
        [DataMember]
        public char SujetoObligado { get; set; }
        [DataMember]
        public double PorcentajeRenta { get; set; }
        [DataMember]
        public string CodigoBanco { get; set; }
        [DataMember]
        public string NombreBanco { get; set; }
        [DataMember]
        public string CodigoTipoCuenta { get; set; }
        [DataMember]
        public string NombreTipoCuenta { get; set; }
        [DataMember]
        public string NumeroCuenta { get; set; }
        [DataMember]
        public string CodigoConfidencialidadDatos { get; set; }
        [DataMember]
        public string CodigoComunicacion { get; set; }
        [DataMember]
        public int codigoTipoPeriodoBeneficiario { get; set; }
    }
}
