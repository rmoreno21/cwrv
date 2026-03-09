using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class EstadoBeneficiario
    {
        public string cod_estado_beneficiario { get; set; }
        public string gls_estado_beneficiario { get; set; }
        public string gls_corta_estado_ben { get; set; }
    }
}
