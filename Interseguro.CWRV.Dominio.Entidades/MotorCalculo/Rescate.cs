using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Rescate
    {
        public int mes_rescate { get; set; }
        public double tasa_venta { get; set; }
        public double spread { get; set; }
        public double tasa_rescate_anual { get; set; }
        public double tasa_rescate_mensual { get; set; }
        public double valor_rescate { get; set; }
    }
}
