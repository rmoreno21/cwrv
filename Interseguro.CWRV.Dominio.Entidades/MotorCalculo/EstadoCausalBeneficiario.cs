using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class EstadoCausalBeneficiario
    {
        public string cod_estado_causal { get; set; }
        public string gls_causal_estado_ben { get; set; }
        public string gls_corta_causal_est_ben { get; set; }
    }
}
