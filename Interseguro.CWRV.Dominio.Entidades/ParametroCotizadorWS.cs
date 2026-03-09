using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ParametroCotizadorWS
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
        public double[][] ppu_vllx { get; set; }
        //<SOLINI-19737>
        [DataMember]
        public double[][] ppu_vllx_vol { get; set; }
        [DataMember]
        public double[][] ppu_fmqx { get; set; }
        [DataMember]
        public long[][] ppu_inf_tm { get; set; }
        [DataMember]
        public long[][] ppu_inf_fm { get; set; }
        //<SOLFIN-19737>
        [DataMember]
        public double[][] ppu_arr_vllx { get; set; }


        //ppu_fmqx_sbs
        //<INIGTI_2145>
        [DataMember]
        public double[][] ppu_fmqx_sbs { get; set; }

        [DataMember]
        public int[][] ppu_inf_tm_sbs { get; set; }

        [DataMember]
        public int[][] ppu_inf_fm_sbs { get; set; }

        //<FINGTI_2145>

        /*[DataMember]
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
        public string wl_XML_Pje { get; set; }*/
    }
}
