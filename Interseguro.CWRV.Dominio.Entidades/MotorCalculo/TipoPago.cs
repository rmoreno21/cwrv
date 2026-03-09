using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class TipoPago
    {
        public string cod_tipo_pago { get; set; }
        public string gls_corta_tipo_pago { get; set; }
        public string gls_tipo_pago { get; set; }
    }
}
