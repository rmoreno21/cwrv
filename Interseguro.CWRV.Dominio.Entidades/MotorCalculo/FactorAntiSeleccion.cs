using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class FactorAntiSeleccion
    {
        public string cod_sexo { get; set; }
        public double val_tope { get; set; }
        public double pje_ajuste { get; set; }
    }
}
