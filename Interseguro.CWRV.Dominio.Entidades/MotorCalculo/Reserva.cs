using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Reserva
    {
        public double val_res_pension { get; set; }
        public double val_res_pension_cpg { get; set; }
        public double val_res_pension_spg { get; set; }
        public double val_res_sepelio { get; set; }
        public double val_res_devolucion { get; set; }
        public double val_res_fallecimiento { get; set; }
        public double val_res_total { get; set; }
        public double val_res_cru_pension { get; set; }
        public double val_res_cru_sepelio { get; set; }
        public double val_res_cru_devolucion { get; set; }
        public double val_res_cru_fallecimiento { get; set; }
        public double val_para_duration { get; set; }
        public Cotizacion cotizacion { get; set; }

        //Campos Mapeados del Motor
        public double val_prima_unica_pension { get; set; }
        public double val_prima_unica_sepelio { get; set; }
        public double val_prima_unica_devolucion { get; set; }
        public double val_prima_unica_fallecimiento { get; set; }
    }
}
