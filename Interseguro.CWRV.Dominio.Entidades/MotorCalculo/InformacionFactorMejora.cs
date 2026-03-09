using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class InformacionFactorMejora
    {
        public string val_tipo_beneficiario { get; set; }
        public int num_anio_factor { get; set; }
    }
}
