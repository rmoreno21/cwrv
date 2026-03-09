using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class InversionVitalicia
    {
        public string cod_moneda { get; set; }
        public int num_anio { get; set; }
        public double val_inv { get; set; }
        public string cod_tipo_temporalidad { get; set; }
    }
}
