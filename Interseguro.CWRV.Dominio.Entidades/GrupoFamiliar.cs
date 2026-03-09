using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class GrupoFamiliar
    {
        [DataMember]
        public Int64 Id { get; set; }
        [DataMember]
        public Afiliado Afiliado { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public Identificacion Identificacion { get; set; }
        [DataMember]
        public Parentesco Parentesco { get; set; }
        [DataMember]
        public char Sexo { get; set; }
        [DataMember]
        public DateTime? FechaNacimiento { get; set; }
        [DataMember]
        public bool Invalido { get; set; }
        [DataMember]
        public TipoInvalidez TipoInvalidez { get; set; }
        [DataMember]
        public DateTime? FechaInvalidez { get; set; }
        [DataMember]
        public bool Seleccionado { get; set; }
        [DataMember]
        public Usuario Usuario { get; set; }
        [DataMember]
        public bool ind_PEP { get; set; }
        [DataMember]
        public bool ind_SujetoObligado { get; set; }
        [DataMember]
        public Temporal Nacionalidad { get; set; }
        [DataMember]
        public Temporal Profesion { get; set; }
        [DataMember]
        public Temporal Residencia { get; set; }

        [DataMember]
        public Parametro Banco { get; set; }
        [DataMember]
        public Parametro Comunicacion { get; set; }
        [DataMember]
        public string NumeroBanco { get; set; }
        [DataMember]
        public Parametro Confidencialidaddatos { get; set; }
        [DataMember]
        public string flagRenta { get; set; }

        [DataMember]
        public string ApellidosNombres { get; set; }

        [DataMember]
        public double ValPjeRenta { get; set; }


        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string Distrito { get; set; }

        [DataMember]
        public string Provincia { get; set; }

        [DataMember]
        public string Departamento { get; set; }

        [DataMember]
        public string Telefono1 { get; set; }

        [DataMember]
        public string Telefono2 { get; set; }

        [DataMember]
        public string CorreoElectronico { get; set; }

        [DataMember]
        public int num_item { get; set; }

        [DataMember]
        public Parametro TipoCtaBanco { get; set; }

        [DataMember]
        public SolicitudRPPlus SolicitudRPPlus { get; set; }
        [DataMember]
        public string estadoCivil { get; set; }

        [DataMember]
        public SolicitudIFP SolicitudIFP { get; set; }

        //S38
        [DataMember]
        public long IdGrupoFamiliar { get; set; }

        //S40
        [DataMember]
        public string telefono { get; set; }
        [DataMember]
        public string celular { get; set; }
        [DataMember]
        public string cargo { get; set; }
        [DataMember]
        public Moneda monedaIngreso { get; set; }
        [DataMember]
        public string centroLaboral { get; set; }
        [DataMember]
        public string actividadEconomica { get; set; }
        [DataMember]
        public float ingresoNeto { get; set; }

        [DataMember]
        public string TipoCobertura { get; set; }

        [DataMember]
        public double ValPjeRentaCA { get; set; }

        [DataMember]
        public bool Ind_Cierre { get; set; }

        [DataMember]
        public string ApellidoPaternoApdo { get; set; }
        [DataMember]
        public string ApellidoMaternoApdo { get; set; }
        [DataMember]
        public string NombresApdo { get; set; }
        [DataMember]
        public Identificacion IdentificacionApdo { get; set; }
        [DataMember]
        public char SexoApdo { get; set; }
        [DataMember]
        public DateTime? FechaNacimientoApdo { get; set; }
        [DataMember]
        public OrigenFondo OrigenFondo { get; set; }
        [DataMember]
        public bool IndTieneApoderado { get; set; }
        [DataMember]
        public double ValPjeAdicional { get; set; }
        [DataMember]
        public bool IndBloquearCampos { get; set; }
        [DataMember]
        public int IdTipoPeriodoBeneficiario { get; set; }
        [DataMember]
        public double ValPjeBeneficiario { get; set; }

    }
}
