using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class TablaIcob
    {
        public double num_indi { get; set; }
        public double num_imon { get; set; }
        public double val_icop { get; set; }
        public double val_lcop { get; set; }
        public string cod_tipo_temporalidad { get; set; }
    }
}
