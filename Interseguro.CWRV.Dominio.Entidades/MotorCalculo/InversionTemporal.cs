using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class InversionTemporal
    {
        public string cod_moneda { get; set; }
        public int num_instrumento { get; set; }
        public int num_periodo { get; set; }
        public double valc { get; set; }
        public double porc { get; set; }
        public double tirc { get; set; }
        public double tasa_inv { get; set; }
        public bool ind_devolucion { get; set; }
        public string cod_tipo_temporalidad { get; set; }
    }
}
