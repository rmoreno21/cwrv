using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Beneficiario
    {
        public Int64? cod_beneficiario { get; set; }
        public int? item { get; set; }
        public string cod_parentesco { get; set; }
        public DateTime fec_nacimiento { get; set; }
        public bool ind_invalido { get; set; }
        public string cod_sexo { get; set; }
        public double val_pje_renta { get; set; }
        public double val_pje_adicional { get; set; }
        public EstadoBeneficiario EstadoBeneficiario { get; set; }
        public EstadoCausalBeneficiario EstadoCausalBeneficiario { get; set; }
        public Documento Documento { get; set; }
    }
}
