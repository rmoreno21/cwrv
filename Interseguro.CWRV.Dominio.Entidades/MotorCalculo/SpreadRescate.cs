using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class SpreadRescate
    {
        public int num_mes { get; set; }
        public DateTime fec_inicio_vigencia { get; set; }
        public DateTime fec_fin_vigencia { get; set; }
        public string cod_moneda { get; set; }
        public string cod_tipo_temporalidad { get; set; }
        public int ind_origen { get; set; }
        public double val_spread { get; set; }
    }
}
