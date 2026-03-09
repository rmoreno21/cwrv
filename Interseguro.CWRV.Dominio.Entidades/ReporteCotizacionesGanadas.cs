using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class ReporteCotizacionesGanadas
    {
        [DataMember]
        public int num_operacion { get; set; }
        [DataMember]
        public string gls_afp { get; set; }
        [DataMember]
        public string num_cuspp { get; set; }
        [DataMember]
        public string afiliado { get; set; }
        [DataMember]
        public string gls_tipo_identificacion { get; set; }
        [DataMember]
        public string num_documento { get; set; }
        [DataMember]
        public string cod_genero_afiliado { get; set; }
        [DataMember]
        public DateTime fec_nacimiento_afiliado { get; set; }
        [DataMember]
        public string cod_Grado_invalidez { get; set; }
        [DataMember]
        public string cod_estado_sobrevivencia { get; set; }
        [DataMember]
        public string gls_tipo_pension_sbs { get; set; }
        [DataMember]
        public string cod_cambio_modalidad { get; set; }
        [DataMember]
        public string ind_pension_preliminar { get; set; }
        [DataMember]
        public double val_tasa_rp_rt { get; set; }
        [DataMember]
        public double val_tipo_cambio { get; set; }
        [DataMember]
        public DateTime fec_envio { get; set; }
        [DataMember]
        public DateTime fec_devengue { get; set; }
        [DataMember]
        public DateTime fec_cierre { get; set; }
        [DataMember]
        public string cod_moneda_fondo { get; set; }
        [DataMember]
        public double val_capital_pension { get; set; }
        [DataMember]
        public double val_saldo_cic { get; set; }
        [DataMember]
        public double val_cuota { get; set; }
        [DataMember]
        public double val_saldo_cuota { get; set; }
        [DataMember]
        public double val_bono_actualizado { get; set; }
        [DataMember]
        public string ind_tiene_cobertura { get; set; }
        [DataMember]
        public double val_aporte_adicional { get; set; }
        [DataMember]
        public double val_tipo_cambio_compra_AA { get; set; }
        [DataMember]
        public string gls_compania { get; set; }
        [DataMember]
        public string gls_modalidad { get; set; }
        [DataMember]
        public string cod_moneda_producto { get; set; }
        [DataMember]
        public int num_anos_RT { get; set; }
        [DataMember]
        public int val_porcentaje_RVD { get; set; }
        [DataMember]
        public int val_periodo_garantizado { get; set; }
        [DataMember]
        public string ind_derecho_crecer { get; set; }
        [DataMember]
        public string ind_gratificacion { get; set; }
        [DataMember]
        public double pje_cobertura_conyuge { get; set; }
        [DataMember]
        public string cod_particion_capital { get; set; }
        [DataMember]
        public string ajusteMoneda { get; set; }
        [DataMember]
        public List<ReporteCotizacionesGanadasBeneficiarios> reporteCotizacionesGanadasBeneficiarios { get; set; }

    }

}
