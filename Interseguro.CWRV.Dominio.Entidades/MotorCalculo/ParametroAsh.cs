using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class ParametroAsh
    {
        public double val_dtra;

        public double val_cmor { get; set; }
        public double val_fcon { get; set; }
        public double tas_htra { get; set; }
        public double tas_htva { get; set; }
        public double tas_ltra { get; set; }
        public double tas_ltva { get; set; }
        public double val_rend { get; set; }
        public double tas_tgpd { get; set; }
        public int flg_ibtp { get; set; }
        public int flg_ivnt { get; set; }
        public int flg_ideb { get; set; }
        public int flg_iajm { get; set; }
        public int flg_icmo { get; set; }
        public double tas_timp { get; set; }
        public double tas_tsbs { get; set; }
        public double tas_ttec { get; set; }
        public double val_gfi1 { get; set; }
        public double val_gfi2 { get; set; }
        public double val_comi { get; set; }
        public double val_coba { get; set; }
        public double val_pumi { get; set; }
        public double num_nins { get; set; }
        public double num_nper { get; set; }
        public double val_ltit { get; set; }
        public double val_htit { get; set; }
        public double ini_tra2 { get; set; }
        public double val_tasa_inv { get; set; }
        public double val_tasa_inf { get; set; }
        public double val_tasa_mrg_solv { get; set; }
        public double val_tasa_costo_cap { get; set; }
        public double val_vtax { get; set; }
    }
}
