using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public partial class TablaVTD : ValorParametro
    {
        public string cod_moneda { get; set; }
        public int num_mes { get; set; }
        public double val_vtd { get; set; }
        public string cod_tipo_temporalidad { get; set; }
    }
}
