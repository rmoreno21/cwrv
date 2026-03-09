using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class FactorMejora
    {
        public int num_edad_mes { get; set; }
        public double val_lx_mbh { get; set; }
        public double val_lx_mbm { get; set; }
        public double val_lx_mih { get; set; }
        public double val_lx_mim { get; set; }
        public double val_lx_mvh { get; set; }
        public double val_lx_mvm { get; set; }
        public string val_tipo_beneficiario { get; set; }
        public int num_anio_factor { get; set; }
    }
}
