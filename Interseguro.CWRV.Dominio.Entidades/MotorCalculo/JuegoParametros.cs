using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class JuegoParametros
    {
        public string CodigoTemporalidad { get; set; }
        public string CodigoMoneda { get; set; }
        public string CodigoOrigen { get; set; }
        public Parametros Parametros { get; set; }
    }
}
