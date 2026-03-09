using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Moneda
    {
        public string cod_moneda { get; set; }
        public string gls_corta_moneda { get; set; }
        public string gls_moneda { get; set; }
        public string gls_adicional { get; set; }
        public string cod_moneda_banco { get; set; }
    }
}
