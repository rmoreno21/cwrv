using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ParametroCotizador
    {
        [DataMember]
        public string cot_gls_ikey { get; set; }
        [DataMember]
        public string cot_num_soli { get; set; }
        [DataMember]
        public int cot_num_coti { get; set; }
        [DataMember]
        public int ind_orden { get; set; }
        [DataMember]
        public int cot_num_mdif { get; set; }
        [DataMember]
        public int cot_num_mgar { get; set; }
        [DataMember]
        public int cot_num_nben { get; set; }
        [DataMember]
        public int cot_fec_fcal { get; set; }
        [DataMember]
        public int cot_fec_fdev { get; set; }
        [DataMember]
        public int cot_num_tpen { get; set; }
        [DataMember]
        public int cot_num_tcal { get; set; }
        [DataMember]
        public int cot_flg_idac { get; set; }
        [DataMember]
        public int cot_flg_igra { get; set; }
        [DataMember]
        public int cot_num_cmon { get; set; }
        [DataMember]
        public int cot_num_trea { get; set; }
        [DataMember]
        public int cot_num_frea { get; set; }
        [DataMember]
        public int cot_flg_irea { get; set; }
        [DataMember]
        public double cot_tas_vrea { get; set; }
        [DataMember]
        public double cot_tas_tasa { get; set; }
        [DataMember]
        public double cot_val_vpen { get; set; }
        [DataMember]
        public double cot_tas_vtra { get; set; }
        [DataMember]
        public double cot_tas_tafp { get; set; }
        [DataMember]
        public double cot_val_acom { get; set; }
        [DataMember]
        public double cot_val_dcom { get; set; }
        [DataMember]
        public double cot_val_puam { get; set; }
        [DataMember]
        public double cot_val_puni { get; set; }
        [DataMember]
        public double cot_por_prrt { get; set; }
        [DataMember]
        public double cot_val_tgfi { get; set; }
        [DataMember]
        public int cot_val_tope { get; set; }
        [DataMember]
        public string cot_xml_benefi { get; set; }
        [DataMember]
        public string cot_xml_tabico { get; set; }
        [DataMember]
        public string cot_xml_parash { get; set; }
        [DataMember]
        public string cot_xml_parinv { get; set; }
        [DataMember]
        public string cot_xml_ajutdm { get; set; }
        [DataMember]
        public string cot_xml_fluaju { get; set; }
        [DataMember]
        public double[,] ppu_vllx { get; set; }
        [DataMember]
        public double[,] ppu_arr_vllx { get; set; }
        //<SOLINI-19737>
        [DataMember]
        public double[,] ppu_vllx_cot { get; set; }
        [DataMember]
        public double[,] ppu_fmqx { get; set; }
        [DataMember]
        public double[,] ppu_fmqx_sbs { get; set; }
        [DataMember]
        public int[,] ppu_inf_tm { get; set; }
        [DataMember]
        public int[,] ppu_inf_fm { get; set; }
        [DataMember]
        public int[,] ppu_inf_tm_sbs { get; set; }
        [DataMember]
        public int[,] ppu_inf_fm_sbs { get; set; }
        //<SOLFIN-19737>
        //<GTI.INI-15819>
        [DataMember]
        public double[] cot_fac_dto { get; set; }
        //<GTI.FIN-15819>
        [DataMember]
        public double x_cot_tas_vtva { get; set; }
        [DataMember]
        public double x_cot_val_mdco { get; set; }
        [DataMember]
        public double x_cia_val_pens { get; set; }
        [DataMember]
        public double x_cia_val_ppag { get; set; }
        [DataMember]
        public double x_cia_val_puni { get; set; }
        [DataMember]
        public double x_cia_val_puur { get; set; }
        [DataMember]
        public double x_afp_val_pens { get; set; }
        [DataMember]
        public double x_afp_val_puni { get; set; }
        [DataMember]
        public double x_afp_val_puur { get; set; }
        [DataMember]
        public double x_ash_val_vpen { get; set; }
        [DataMember]
        public double x_ash_tas_vtva { get; set; }
        [DataMember]
        public double x_ash_tas_vtra { get; set; }
        [DataMember]
        public double x_ash_tas_vtce { get; set; }
        [DataMember]
        public double x_ash_val_dura { get; set; }
        [DataMember]
        public StringBuilder x_cot_xml_benefi { get; set; }
        [DataMember]
        public int x_cot_num_cmsg { get; set; }

        // Propiedades Auxiliares
        [DataMember]
        public DateTime fec_cotizacion { get; set; }
        [DataMember]
        public double wl_val_pension_minimo { get; set; }
        [DataMember]
        public string cod_tipo_pension { get; set; }
        [DataMember]
        public string cod_tipo_invalidez { get; set; }
        [DataMember]
        public string cod_moneda { get; set; }
        [DataMember]
        public double val_moneda { get; set; }
        [DataMember]
        public string cod_tipo_producto { get; set; }
        [DataMember]
        public string ind_modalidad { get; set; }
        [DataMember]
        public List<double> fluaju { get; set; }
        [DataMember]
        public double val_ltit { get; set; }
        [DataMember]
        public double val_htit { get; set; }
        [DataMember]
        public string wl_XML_Pje { get; set; }

        //<SOLINI25621>
        [DataMember]
        public double[,] rango_capital { get; set; }

        [DataMember]
        public double[,] rango_ajutra { get; set; }

        [DataMember]
        public double cot_tas_vtra_sintra { get; set; }
        //<SOLINI25621>

        //<INIGTI_1092>
        [DataMember]
        public double wl_val_ltra { get; set; }

        
        [DataMember]
        public double val_ajuste_invalidez { get; set; }

        //<FINGTI_1092>
        
        //<INIGTI_754>
        [DataMember]
        public int x_cot_ini_tra2 { get; set; }
        
        [DataMember]
        public double x_cot_pje_rent { get; set; }
        //<FINGTI_754>

        //<INIGTI_753>
        [DataMember]
        public int x_cot_pje_devo { get; set; }

        [DataMember]
        public string x_ind_devo { get; set; }

        [DataMember]
        public double x_val_mon_aju { get; set; }
        //<FINGTI_753>

        //<INIGTI_4081>
        [DataMember]
        public double x_pbs { get; set; }

        [DataMember]
        public double x_val_tasa_int_vit { get; set; }

        [DataMember]
        public double x_val_tasa_ajuste_tra { get; set; }

        [DataMember]
        public double wl_val_htva { get; set; }
        //<FINGTI_4081>

    }
}
