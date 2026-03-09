using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Beneficiario
    {
        [DataMember]
        public string Id { get; set; }
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
        public Afiliado Afiliado { get; set; }

        //<INI.GTI_26560>
        [DataMember]
        public string numCuspp { get; set; }
        [DataMember]
        public int numCorrelativo { get; set; }
        [DataMember]
        public string CorreoElectronico { get; set; }
        [DataMember]
        public string numTelefono { get; set; }
        [DataMember]
        public string numCelular { get; set; }
        [DataMember]
        public string centroLaboral { get; set; }
        [DataMember]
        public string envioPoliza { get; set; }
        [DataMember]
        public BeneficiarioDireccion direccionPrincipal { get; set; }
        [DataMember]
        public BeneficiarioDireccion direccionAlterna { get; set; }
        [DataMember]
        public string usuario { get; set; }
        [DataMember]
        public string numSolicitud { get; set; }
        [DataMember]
        public FirmaDigital firmaDigital { get; set; }
        [DataMember]
        public bool Apoderado { get; set; }
        [DataMember]
        public string ApellidoPaternoApodero { get; set; }
        [DataMember]
        public string ApellidoMaternoApodero { get; set; }
        [DataMember]
        public string NombreApodero { get; set; }
        [DataMember]
        public Identificacion IdentificacionApodero { get; set; }

        [DataMember]
        public string CorreoElectronicoValidacion { get; set; }
        [DataMember]
        public string numTelefonoValidacion { get; set; }
        [DataMember]
        public string numCelularValidacion { get; set; }
        //<FIN.GTI_26560>

        [DataMember]
        public string indPEP { get; set; }
        [DataMember]
        public string glsCategoria { get; set; }
        [DataMember]
        public bool ind_tiene_apoderado { get; set; }
        [DataMember]
        public string ind_seleccionado { get; set; }

        //<GTI.59048-INI>
        [DataMember]
        public DateTime? FechaFallecimiento { get; set; }
        [DataMember]
        public Nacionalidad Nacionalidad { get; set; }
        [DataMember]
        public Temporal PaisOrigen { get; set; }
        [DataMember]
        public Temporal VinculoFamiliar { get; set; }
        [DataMember]
        public string NroVinculoFamiliar { get; set; }
        [DataMember]
        public Temporal DescuentoESSALUD { get; set; }
        //<GTI.59048-FIN>

    }

    public class BeneficiarioRV
    {
        [DataMember]
        public int num_poliza { get; set; }
        [DataMember]
        public string cod_tipo_identificacion { get; set; }
        [DataMember]
        public int num_identificacion { get; set; }
        //Persona
        [DataMember]
        public DateTime fec_nacimiento { get; set; }
        [DataMember]
        public DateTime? fec_fallecimiento { get; set; }
        [DataMember]
        public string cod_sexo { get; set; }
        [DataMember]
        public string ind_invalidez { get; set; }
        [DataMember]
        public string cod_tipo_invalidez { get; set; }
        [DataMember]
        public DateTime? fec_invalidez { get; set; }
        [DataMember]
        public string ape_paterno { get; set; }
        [DataMember]
        public string ape_materno { get; set; }
        [DataMember]
        public string nom_persona { get; set; }
        [DataMember]
        public string gls_persona { get; set; }
        [DataMember]
        public string cod_nacionalidad { get; set; }

        //<GTI.59048-INI>
        [DataMember]
        public string cod_equivalencia_nacionalidad { get; set; }
        [DataMember]
        public string cod_tipo_pacto_salud { get; set; }
        //<GTI.59048-FIN>

        [DataMember]
        public string cod_pais_origen_doc { get; set; }
        [DataMember]
        public string cod_tipo_identificacion_pdt { get; set; }
        [DataMember]
        public string num_identificacion_pdt { get; set; }
        //Beneficiario
        [DataMember]
        public int num_correlativo { get; set; }
        [DataMember]
        public string cod_tipo_identificacion_apoderado { get; set; }
        [DataMember]
        public string num_identificacion_apoderado { get; set; }
        [DataMember]
        public string cod_parentezco { get; set; }
        [DataMember]
        public string cod_estado_beneficiario { get; set; }
        [DataMember]
        public string cod_causal_estado_ben { get; set; }
        [DataMember]
        public double pje_pension { get; set; }
        [DataMember]
        public string num_via_pago { get; set; }
        [DataMember]
        public string cod_docparentesco { get; set; }
        [DataMember]
        public string num_docparentesco { get; set; }
        [DataMember]
        public string gls_mail { get; set; }
        //--
        [DataMember]
        public string num_celular { get; set; }
        //Direccion
        [DataMember]
        public string gls_direccion { get; set; }
        [DataMember]
        public string cod_comuna { get; set; }
        [DataMember]
        public string cod_ciudad { get; set; }
        [DataMember]
        public string num_telefono { get; set; }
        [DataMember]
        public string ind_vigencia { get; set; }
        [DataMember]
        public string cod_tipo_via_rviadm { get; set; }
        [DataMember]
        public string gls_nom_via { get; set; }
        [DataMember]
        public string gls_espacio_urbano { get; set; }
        [DataMember]
        public string gls_num_via { get; set; }
        [DataMember]
        public string gls_num_interior { get; set; }
        [DataMember]
        public string cod_tipo_zona { get; set; }
        [DataMember]
        public string gls_nom_zona { get; set; }
        [DataMember]
        public string gls_referencia { get; set; }
        [DataMember]
        public string cod_larga_distancia { get; set; }
        [DataMember]
        public string gls_departamento { get; set; }
        [DataMember]
        public string gls_manzana { get; set; }
        [DataMember]
        public string gls_lote { get; set; }
        [DataMember]
        public string gls_kilometro { get; set; }
        [DataMember]
        public string gls_block { get; set; }
        [DataMember]
        public string gls_etapa { get; set; }
    }
}
