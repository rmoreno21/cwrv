using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class TablaVOLA
    {
        public string cod_moneda { get; set; }
        public int num_mes { get; set; }
        public double val_vola { get; set; }
        public string cod_tipo_temporalidad { get; set; }
    }
}
