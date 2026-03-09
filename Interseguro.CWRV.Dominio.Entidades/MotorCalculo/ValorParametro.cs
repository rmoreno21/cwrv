namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    public interface ValorParametro
    {
        string cod_moneda { get; set; }
        int num_mes { get; set; }
        double val_vtd { get; set; }
        string cod_tipo_temporalidad { get; set; }
    }
}
