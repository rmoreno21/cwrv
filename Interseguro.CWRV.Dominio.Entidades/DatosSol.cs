using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class DatosSol
    {

        [DataMember]
        public string num_cuissp_afiliado { get; set; }
        [DataMember]
        public string ape_paterno_afiliado { get; set; }
        [DataMember]
        public string ape_materno_afiliado { get; set; }
        [DataMember]
        public string nom_nombre_afiliado { get; set; }
        [DataMember]
        public DateTime fec_nacimiento_afiliado { get; set; }
        [DataMember]
        public string cod_tipo_documento_afiliado { get; set; }
        [DataMember]
        public string rut_persona_afiliado { get; set; }
        [DataMember]
        public char cod_sexo_afiliado { get; set; }
        [DataMember]
        public string cod_EstadoCivil_afiliado { get; set; }
        [DataMember]
        public int id_grupo_familiar_beneficiario { get; set; }
        [DataMember]
        public string num_cuissp_beneficiario { get; set; }
        [DataMember]
        public string ape_paterno_beneficiario { get; set; }
        [DataMember]
        public string ape_materno_beneficiario { get; set; }
        [DataMember]
        public string nom_persona_beneficiario { get; set; }
        [DataMember]
        public DateTime fec_nacimiento_beneficiario { get; set; }
        [DataMember]
        public string cod_tipo_identificacion_beneficiario { get; set; }
        [DataMember]
        public string num_identificacion_beneficiario { get; set; }
        [DataMember]
        public string cod_parentesco_beneficiario { get; set; }
        [DataMember]
        public string gls_parentesco_beneficiario { get; set; }
        [DataMember]
        public string cod_Nacionalidad_beneficiario { get; set; }
        [DataMember]
        public string cod_Profesion_beneficiario { get; set; }
        [DataMember]
        public string cod_Residencia_beneficiario { get; set; }
        [DataMember]
        public string ind_PEP_beneficiario { get; set; }
        [DataMember]
        public string ind_SujetoObligado_beneficiario { get; set; }
        [DataMember]
        public string ind_PEP_afiliado { get; set; }
        [DataMember]
        public string ind_SujetoObligado_afiliado { get; set; }
        [DataMember]
        public string num_solicitud { get; set; }
        [DataMember]
        public DateTime fec_solicitud { get; set; }
        [DataMember]
        public string cod_afp { get; set; }
        [DataMember]
        public string cod_tipo_cotizacion { get; set; }
        [DataMember]
        public double? val_mto_cta_individual { get; set; }
        [DataMember]
        public string cod_moneda_cta_indiv { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_day1 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_day2 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_month1 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_month2 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_year1 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_year2 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_year3 { get; set; }
        [DataMember]
        public string fec_nacimiento_beneficiario_year4 { get; set; }
        [DataMember]
        public string nom_agente { get; set; }
        [DataMember]
        public string rut_agente { get; set; }

        [DataMember]
        public string cod_tipo_plan_rpp { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
        [DataMember]
        public string val_pen_cia { get; set; }
        [DataMember]
        public string val_mon_aju { get; set; }
        [DataMember]
        public string cod_moneda { get; set; }
        [DataMember]
        public string val_per_garantizado { get; set; }
        [DataMember]
        public string ind_gasto_sepelio { get; set; }
        [DataMember]
        public string val_per_temporal { get; set; }
        [DataMember]
        public string val_pje_dev { get; set; }
        [DataMember]
        public string val_pje_conyuge { get; set; }
        [DataMember]
        public string nom_agencia { get; set; }

        [DataMember]
        public string MontoSepelio { get; set; }

        [DataMember]
        public string Nacionalidad_beneficiario_cod { get; set; }
        [DataMember]
        public string Profesion_beneficiario_cod { get; set; }
        [DataMember]
        public string Residencia_beneficiario_cod { get; set; }

        [DataMember]
        public string Confidencialidaddatos { get; set; }
        [DataMember]
        public string Banco { get; set; }
        [DataMember]
        public string NroCtaBancaria { get; set; }
        [DataMember]
        public string Comunicacion { get; set; }

        [DataMember]
        public string direccion { get; set; }
        [DataMember]
        public string cod_departamento { get; set; }
        [DataMember]
        public string cod_distrito { get; set; }
        [DataMember]
        public string cod_provincia { get; set; }
        [DataMember]
        public string cod_tipovia { get; set; }
        [DataMember]
        public string nromzlt { get; set; }

        [DataMember]
        public string num_agente { get; set; }

        [DataMember]
        public int CodigoEstado { get; set; }
        [DataMember]
        public int CodigoEstadoPlaft { get; set; }

        [DataMember]
        public int coberturaAdicionalDevolucion { get; set; }
        [DataMember]
        public int coberturaAdicionalFallecimiento { get; set; }
        [DataMember]
        public double pjeFallecimientonoDevengados { get; set; }
        [DataMember]
        public double pjeDevolucionFallecimiento { get; set; }
        [DataMember]
        public int periodoDiferido { get; set; }
        [DataMember]
        public string codigoPlan { get; set; }

        [DataMember]
        public string glsPlan { get; set; }

        [DataMember]
        public double valPjeRenta { get; set; }

        [DataMember]
        public string glsMail { get; set; }

        [DataMember]
        public string cod_canal_distribucion { get; set; }

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
        public string EstadoCivil { get; set; }
        [DataMember]
        public string Nacionalidad { get; set; }
        [DataMember]
        public string OrigenCotizacion { get; set; }
        [DataMember]
        public int ValPjeCACy { get; set; }
        [DataMember]
        public int ValPjeCAPa { get; set; }
        [DataMember]
        public int ValPjeCAMa { get; set; }
        [DataMember]
        public List<BeneficiariosCA> BeneficiariosCA { get; set; }

        [DataMember]
        public string gls_identificacion_afiliado { get; set; }
        [DataMember]
        public double Val_prima_unica_sepelio { get; set; }
        [DataMember]
        public double Val_prima_unica_pension { get; set; }
        [DataMember]
        public double Val_prima_unica_devolucion { get; set; }
        [DataMember]
        public double Val_prima_unica_fallecimiento { get; set; }
    }


    [DataContract]
    public class BeneficiariosCA
    {
        [DataMember]
        public string nombres { get; set; }
        [DataMember]
        public string fechaNacimiento { get; set; }
        [DataMember]
        public string docIdentidad { get; set; }
        [DataMember]
        public string parentesco { get; set; }
        [DataMember]
        public int renta { get; set; }

    }

}
