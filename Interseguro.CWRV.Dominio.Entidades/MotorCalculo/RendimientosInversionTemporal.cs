using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class RendimientosInversionTemporal
    {
        public string cod_moneda { get; set; }
        public int num_periodo { get; set; }
        public double val_parametro { get; set; }
    }
}
