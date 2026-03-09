using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class FactorPUMI
    {
        public string cod_sexo { get; set; }
        public double val_tope { get; set; }
        public double pje_ajuste { get; set; }
    }
}
