using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class JsonArchivos
    {
        [DataMember]
        public List<Archivo> Archivos { get; set; }

        [DataMember]
        public string NombreArchivo { get; set; }

        [DataMember]
        public string RutaArchivo { get; set; }

        [DataMember]
        public string ArchivoSerializado { get; set; }
    }

    public partial class Archivo
    {
        public Archivo()
        {
            this.Trama = new object();
        }

        [DataMember]
        public string RutaWord { get; set; }

        [DataMember]
        public object Trama { get; set; }
    }

    public class CondicionesGenerales
    {
        public string Plan1 { get; set; }
        public string Plan2 { get; set; }
        public string CodigoSBS { get; set; }
    }

    public class CondicionesParticulares
    {
        public string Plan { get; set; }
        public string SBS { get; set; }
        public string NroPoliza { get; set; }
        public string ApellidosNombres { get; set; }
        public string DocIdentidad { get; set; }
        public string NumeroDocIdentidad { get; set; }
        public string FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Direccion { get; set; }
        public string Distrito { get; set; }
        public string Provincia { get; set; }
        public string Departamento { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string MonedaPrima { get; set; }
        public string FechaEmision { get; set; }
        public string FechaInicioVigencia { get; set; }
        public string FechaFinVigencia { get; set; }
        public string FechaDevengueRenta { get; set; }
        public string PlazoVigencia { get; set; }
        public string MonedaPagoRenta { get; set; }
        public string FactorAjusteRenta { get; set; }
        public string FechaInicioPagoRentas { get; set; }
        public string MontoBaseRentMensual { get; set; }
        public string RembolsoGastoSepelio { get; set; }
        public string PeriodoGara { get; set; }
        public string FechaClausula { get; set; }
        public string PagoDoble { get; set; }
        public string AñosPagoDoble { get; set; }
        public string DevolucionPrimaUnica { get; set; }
        public string PDevoPUFalle { get; set; }
        public string DevoPUFallecimiento { get; set; }
        public string Banco { get; set; }
        public string NumCuentaAhorros { get; set; }
        public string PrimaComercialTotal { get; set; }
        public string IGVTotal { get; set; }
        public string PrimaComercialTotalIGV { get; set; }
        public string TIRGarantizada { get; set; }
        public string PagoRenta { get; set; }
        public string PrimaComercial { get; set; }
        public string IGV { get; set; }
        public string PrimaComercialIGV { get; set; }
        public string CoberturaAdicional1 { get; set; }
        public string CoberturaAdicional2 { get; set; }
        public string PagoSepelio { get; set; }
        public string DevSepe { get; set; }
        public string PrimaComercialSepelio { get; set; }
        public string IGVSepelio { get; set; }
        public string PrimaComercialIGVSepelio { get; set; }
        public string PagoDevolucion { get; set; }
        public string PrimaComercialDevolucion { get; set; }
        public string IGVDevolucion { get; set; }
        public string PrimaComercialIGVDevo { get; set; }
        public string ApellidosNombresGrilla1 { get; set; }
        public string ApellidosNombresGrilla2 { get; set; }
        public string ApellidosNombresGrilla3 { get; set; }
        public string ApellidosNombresGrilla4 { get; set; }
        public string ApellidosNombresGrilla5 { get; set; }
        public string ApellidosNombresGrilla6 { get; set; }
        public string ApellidosNombresGrilla7 { get; set; }
        public string ApellidosNombresGrilla8 { get; set; }
        public string ApellidosNombresGrilla9 { get; set; }
        public string ApellidosNombresGrilla10 { get; set; }
        public string DocIdentidadGrilla1 { get; set; }
        public string DocIdentidadGrilla2 { get; set; }
        public string DocIdentidadGrilla3 { get; set; }
        public string DocIdentidadGrilla4 { get; set; }
        public string DocIdentidadGrilla5 { get; set; }
        public string DocIdentidadGrilla6 { get; set; }
        public string DocIdentidadGrilla7 { get; set; }
        public string DocIdentidadGrilla8 { get; set; }
        public string DocIdentidadGrilla9 { get; set; }
        public string DocIdentidadGrilla10 { get; set; }
        public string NroDocIdGrilla1 { get; set; }
        public string NroDocIdGrilla2 { get; set; }
        public string NroDocIdGrilla3 { get; set; }
        public string NroDocIdGrilla4 { get; set; }
        public string NroDocIdGrilla5 { get; set; }
        public string NroDocIdGrilla6 { get; set; }
        public string NroDocIdGrilla7 { get; set; }
        public string NroDocIdGrilla8 { get; set; }
        public string NroDocIdGrilla9 { get; set; }
        public string NroDocIdGrilla10 { get; set; }
        public string FecNacGrilla1 { get; set; }
        public string FecNacGrilla2 { get; set; }
        public string FecNacGrilla3 { get; set; }
        public string FecNacGrilla4 { get; set; }
        public string FecNacGrilla5 { get; set; }
        public string FecNacGrilla6 { get; set; }
        public string FecNacGrilla7 { get; set; }
        public string FecNacGrilla8 { get; set; }
        public string FecNacGrilla9 { get; set; }
        public string FecNacGrilla10 { get; set; }
        public string ParentescoGrilla1 { get; set; }
        public string ParentescoGrilla2 { get; set; }
        public string ParentescoGrilla3 { get; set; }
        public string ParentescoGrilla4 { get; set; }
        public string ParentescoGrilla5 { get; set; }
        public string ParentescoGrilla6 { get; set; }
        public string ParentescoGrilla7 { get; set; }
        public string ParentescoGrilla8 { get; set; }
        public string ParentescoGrilla9 { get; set; }
        public string ParentescoGrilla10 { get; set; }
        public string PorRentaGrilla1 { get; set; }
        public string PorRentaGrilla2 { get; set; }
        public string PorRentaGrilla3 { get; set; }
        public string PorRentaGrilla4 { get; set; }
        public string PorRentaGrilla5 { get; set; }
        public string PorRentaGrilla6 { get; set; }
        public string PorRentaGrilla7 { get; set; }
        public string PorRentaGrilla8 { get; set; }
        public string PorRentaGrilla9 { get; set; }
        public string PorRentaGrilla10 { get; set; }
        public string ApellidosNombres1 { get; set; }
        public string ApellidosNombres2 { get; set; }
        public string ApellidosNombres3 { get; set; }
        public string TipoDocIden1 { get; set; }
        public string TipoDocIden2 { get; set; }
        public string TipoDocIden3 { get; set; }
        public string NroDocIde1 { get; set; }
        public string NroDocIde2 { get; set; }
        public string NroDocIde3 { get; set; }
        public string FecNac1 { get; set; }
        public string FecNac2 { get; set; }
        public string FecNac3 { get; set; }
        public string Parentesco1 { get; set; }
        public string Parentesco2 { get; set; }
        public string Parentesco3 { get; set; }
        public string PorcCober1 { get; set; }
        public string PorcCober2 { get; set; }
        public string PorcCober3 { get; set; }

        public string ComisionPromotor { get; set; }

        public string PlanSeguro { get; set; }
        public string PeriodoGarantizado { get; set; }
        public string MontoBaseRentaMensual { get; set; }
        public string ReembolsoGastosSepelio { get; set; }
        public string ApePat1 { get; set; }
        public string ApePat2 { get; set; }
        public string ApePat3 { get; set; }
        public string ApePat4 { get; set; }
        public string ApePat5 { get; set; }
        public string ApePat6 { get; set; }
        public string ApePat7 { get; set; }
        public string ApePat8 { get; set; }
        public string ApeMat1 { get; set; }
        public string ApeMat2 { get; set; }
        public string ApeMat3 { get; set; }
        public string ApeMat4 { get; set; }
        public string ApeMat5 { get; set; }
        public string ApeMat6 { get; set; }
        public string ApeMat7 { get; set; }
        public string ApeMat8 { get; set; }
        public string Nombre1 { get; set; }
        public string Nombre2 { get; set; }
        public string Nombre3 { get; set; }
        public string Nombre4 { get; set; }
        public string Nombre5 { get; set; }
        public string Nombre6 { get; set; }
        public string Nombre7 { get; set; }
        public string Nombre8 { get; set; }
        public string DocIden1 { get; set; }
        public string DocIden2 { get; set; }
        public string DocIden3 { get; set; }
        public string DocIden4 { get; set; }
        public string DocIden5 { get; set; }
        public string DocIden6 { get; set; }
        public string DocIden7 { get; set; }
        public string DocIden8 { get; set; }
        public string NroDocIden1 { get; set; }
        public string NroDocIden2 { get; set; }
        public string NroDocIden3 { get; set; }
        public string NroDocIden4 { get; set; }
        public string NroDocIden5 { get; set; }
        public string NroDocIden6 { get; set; }
        public string NroDocIden7 { get; set; }
        public string NroDocIden8 { get; set; }
        //public string FecNac1 { get; set; }
        //public string FecNac2 { get; set; }
        //public string FecNac3 { get; set; }
        public string FecNac4 { get; set; }
        public string FecNac5 { get; set; }
        public string FecNac6 { get; set; }
        public string FecNac7 { get; set; }
        public string FecNac8 { get; set; }
        public string Parent1 { get; set; }
        public string Parent2 { get; set; }
        public string Parent3 { get; set; }
        public string Parent4 { get; set; }
        public string Parent5 { get; set; }
        public string Parent6 { get; set; }
        public string Parent7 { get; set; }
        public string Parent8 { get; set; }
        public string PorcRta1 { get; set; }
        public string PorcRta2 { get; set; }
        public string PorcRta3 { get; set; }
        public string PorcRta4 { get; set; }
        public string PorcRta5 { get; set; }
        public string PorcRta6 { get; set; }
        public string PorcRta7 { get; set; }
        public string PorcRta8 { get; set; }

    }

    public class Carta
    {
        public string Fecha { get; set; }
        public string ApellidosNombres { get; set; }
        public string Direccion { get; set; }
        public string Distrito { get; set; }
        public string ApellidoPaterno { get; set; }
        public string NroPoliza { get; set; }
    }

    public class JsonCorreoPolizaElectonica
    {

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string NumeroPoliza { get; set; }

        [DataMember]
        public string NumeroDocumento { get; set; }

        [DataMember]
        public string Destinatario { get; set; }

        [DataMember]
        public string ProcesoSme { get; set; }

        [DataMember]
        public string RutaPdf { get; set; }

        [DataMember]
        public string Contrasenia { get; set; }

        [DataMember]
        public JsonCamposDinamicos CamposDinamicos { get; set; }
    }

    public class JsonCamposDinamicos
    {
        [DataMember]
        public string Id_Nombre { get; set; }
        [DataMember]
        public string Id_Renta { get; set; }
        [DataMember]
        public string Id_Moneda { get; set; }
        [DataMember]
        public string Id_Temporalidad { get; set; }
        [DataMember]
        public string Id_FechaDevengue { get; set; }
    }

    public class JsonFormatoSolicitud
    {
        [DataMember]
        public string NumeroSolicitud { get; set; }
        [DataMember]
        public string Titulo { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public string Nombres { get; set; }
        [DataMember]
        public string Dia { get; set; }
        [DataMember]
        public string Mes { get; set; }
        [DataMember]
        public string Anho { get; set; }
        [DataMember]
        public string DNI { get; set; }
        [DataMember]
        public string CE { get; set; }
        [DataMember]
        public string NumeroDocumento { get; set; }
        [DataMember]
        public string Masculino { get; set; }
        [DataMember]
        public string Femenino { get; set; }
        [DataMember]
        public string Soltero { get; set; }
        [DataMember]
        public string Casado { get; set; }
        [DataMember]
        public string Viudo { get; set; }
        [DataMember]
        public string Divorciado { get; set; }
        [DataMember]
        public string Conviviente { get; set; }
        [DataMember]
        public string Nacionalidad { get; set; }
        [DataMember]
        public string Direccion { get; set; }
        [DataMember]
        public string Jiron { get; set; }
        [DataMember]
        public string Avenida { get; set; }
        [DataMember]
        public string Calle { get; set; }
        [DataMember]
        public string Pasaje { get; set; }
        [DataMember]
        public string Domicilio { get; set; }
        [DataMember]
        public string Residencia { get; set; }
        [DataMember]
        public string Distrito { get; set; }
        [DataMember]
        public string Provincia { get; set; }
        [DataMember]
        public string Departamento { get; set; }
        [DataMember]
        public string TelefonoCasa { get; set; }
        [DataMember]
        public string Celular { get; set; }
        [DataMember]
        public string CorreoElectronico { get; set; }
        [DataMember]
        public string EsAsegPEP { get; set; }
        [DataMember]
        public string NoEsAsegPEP { get; set; }
        [DataMember]
        public string EsBenPEP { get; set; }
        [DataMember]
        public string NoEsBenPEP { get; set; }
        [DataMember]
        public string EsAsegSO { get; set; }
        [DataMember]
        public string NoEsAsegSO { get; set; }
        [DataMember]
        public string EsBenSO { get; set; }
        [DataMember]
        public string NoEsBenSO { get; set; }
        [DataMember]
        public string BenPG1_Nombre { get; set; }
        [DataMember]
        public string BenPG2_Nombre { get; set; }
        [DataMember]
        public string BenPG3_Nombre { get; set; }
        [DataMember]
        public string BenPG4_Nombre { get; set; }
        [DataMember]
        public string BenPG5_Nombre { get; set; }
        [DataMember]
        public string BenPG6_Nombre { get; set; }
        [DataMember]
        public string BenPG7_Nombre { get; set; }
        [DataMember]
        public string BenPG8_Nombre { get; set; }
        [DataMember]
        public string BenPG9_Nombre { get; set; }



        [DataMember]
        public string BenPG1_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG2_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG3_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG4_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG5_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG6_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG7_Ape_Paterno { get; set; }
        [DataMember]
        public string BenPG8_Ape_Paterno { get; set; }


        
        [DataMember]
        public string BenPG1_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG2_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG3_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG4_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG5_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG6_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG7_Ape_Materno { get; set; }
        [DataMember]
        public string BenPG8_Ape_Materno { get; set; }



        [DataMember]
        public string BenPG1_FN { get; set; }
        [DataMember]
        public string BenPG2_FN { get; set; }
        [DataMember]
        public string BenPG3_FN { get; set; }
        [DataMember]
        public string BenPG4_FN { get; set; }
        [DataMember]
        public string BenPG5_FN { get; set; }
        [DataMember]
        public string BenPG6_FN { get; set; }
        [DataMember]
        public string BenPG7_FN { get; set; }
        [DataMember]
        public string BenPG8_FN { get; set; }
        [DataMember]
        public string BenPG9_FN { get; set; }
        [DataMember]
        public string BenPG1_DI { get; set; }
        [DataMember]
        public string BenPG2_DI { get; set; }
        [DataMember]
        public string BenPG3_DI { get; set; }
        [DataMember]
        public string BenPG4_DI { get; set; }
        [DataMember]
        public string BenPG5_DI { get; set; }
        [DataMember]
        public string BenPG6_DI { get; set; }
        [DataMember]
        public string BenPG7_DI { get; set; }
        [DataMember]
        public string BenPG8_DI { get; set; }
        [DataMember]
        public string BenPG9_DI { get; set; }
        [DataMember]
        public string BenPG1_Parentesco { get; set; }
        [DataMember]
        public string BenPG2_Parentesco { get; set; }
        [DataMember]
        public string BenPG3_Parentesco { get; set; }
        [DataMember]
        public string BenPG4_Parentesco { get; set; }
        [DataMember]
        public string BenPG5_Parentesco { get; set; }
        [DataMember]
        public string BenPG6_Parentesco { get; set; }
        [DataMember]
        public string BenPG7_Parentesco { get; set; }
        [DataMember]
        public string BenPG8_Parentesco { get; set; }
        [DataMember]
        public string BenPG9_Parentesco { get; set; }
        [DataMember]
        public string BenPG1_Pje { get; set; }
        [DataMember]
        public string BenPG2_Pje { get; set; }
        [DataMember]
        public string BenPG3_Pje { get; set; }
        [DataMember]
        public string BenPG4_Pje { get; set; }
        [DataMember]
        public string BenPG5_Pje { get; set; }
        [DataMember]
        public string BenPG6_Pje { get; set; }
        [DataMember]
        public string BenPG7_Pje { get; set; }
        [DataMember]
        public string BenPG8_Pje { get; set; }
        [DataMember]
        public string BenPG9_Pje { get; set; }
        [DataMember]
        public string BenCA1_Nombre { get; set; }
        [DataMember]
        public string BenCA2_Nombre { get; set; }
        [DataMember]
        public string BenCA3_Nombre { get; set; }
        [DataMember]
        public string BenCA1_FN { get; set; }
        [DataMember]
        public string BenCA2_FN { get; set; }
        [DataMember]
        public string BenCA3_FN { get; set; }
        [DataMember]
        public string BenCA1_DI { get; set; }
        [DataMember]
        public string BenCA2_DI { get; set; }
        [DataMember]
        public string BenCA3_DI { get; set; }
        [DataMember]
        public string BenCA1_Parentesco { get; set; }
        [DataMember]
        public string BenCA2_Parentesco { get; set; }
        [DataMember]
        public string BenCA3_Parentesco { get; set; }
        [DataMember]
        public string BenCA1_Pje { get; set; }
        [DataMember]
        public string BenCA2_Pje { get; set; }
        [DataMember]
        public string BenCA3_Pje { get; set; }
        [DataMember]
        public string MontoRenta { get; set; }
        [DataMember]
        public string TipoMonedaRentaSOL { get; set; }
        [DataMember]
        public string TipoMonedaRentaUSD { get; set; }
        [DataMember]
        public string TipoMonedaNominalSOL { get; set; }
        [DataMember]
        public string TipoMonedaIPCSOL { get; set; }
        [DataMember]
        public string TipoMonedaTasaSOL { get; set; }
        [DataMember]
        public string TipoMonedaNominalUSD { get; set; }
        [DataMember]
        public string TipoMonedaTasaUSD { get; set; }
        [DataMember]
        public string PorcentajeAjusteMoneda { get; set; }
        [DataMember]
        public string ConPG { get; set; }
        [DataMember]
        public string SinPG { get; set; }
        [DataMember]
        public string PeriodoGarantizado { get; set; }
        [DataMember]
        public string ConGS { get; set; }
        [DataMember]
        public string SinGS { get; set; }
        [DataMember]
        public string GastosSepelio { get; set; }
        [DataMember]
        public string ConCF { get; set; }
        [DataMember]
        public string SinCF { get; set; }
        [DataMember]
        public string CoberturaFallecimiento { get; set; }
        [DataMember]
        public string ConCS { get; set; }
        [DataMember]
        public string SinCS { get; set; }
        [DataMember]
        public string CoberturaSobrevivencia { get; set; }
        [DataMember]
        public string PlazorentaVitalicia { get; set; }
        [DataMember]
        public string PlazorentaPlazoFijo { get; set; }
        [DataMember]
        public string Plazorenta { get; set; }
        [DataMember]
        public string ConPD { get; set; }
        [DataMember]
        public string SinPD { get; set; }
        [DataMember]
        public string PagoDoble { get; set; }
        [DataMember]
        public string ConDP { get; set; }
        [DataMember]
        public string SinDP { get; set; }
        [DataMember]
        public string DevolucionPrima { get; set; }
        [DataMember]
        public string ConCoberturaAdicional { get; set; }
        [DataMember]
        public string SinCoberturaAdicional { get; set; }
        [DataMember]
        public string PorcentajeCY { get; set; }
        [DataMember]
        public string PorcentajePA { get; set; }
        [DataMember]
        public string PorcentajeMA { get; set; }
        [DataMember]
        public string Irrevocabilidad { get; set; }
        [DataMember]
        public string MontoPrimaSOL { get; set; }
        [DataMember]
        public string MontoPrimaUSD { get; set; }
        [DataMember]
        public string MontoPrima { get; set; }
        [DataMember]
        public string ModalidadPagoAhorro { get; set; }
        [DataMember]
        public string ModalidadPagoCaja { get; set; }
        [DataMember]
        public string Banco { get; set; }
        [DataMember]
        public string NumeroCuenta { get; set; }
        [DataMember]
        public string Agente { get; set; }
        [DataMember]
        public string Codigo { get; set; }
        [DataMember]
        public string Agencia { get; set; }
        [DataMember]
        public string OrigenVenta { get; set; }
        [DataMember]
        public string DNIAgente { get; set; }
        [DataMember]
        public string MontoPagar { get; set; }
        [DataMember]
        public string MontoPagarSOL { get; set; }
        [DataMember]
        public string MontoPagarUSD { get; set; }
        [DataMember]
        public string TipoPago { get; set; }
        [DataMember]
        public string EsContratante { get; set; }
        [DataMember]
        public string NoEsContratante { get; set; }
        [DataMember]
        public string PlanRentaIndividual { get; set; }
        [DataMember]
        public string PlanRentaFamiliar { get; set; }
        [DataMember]
        public string PorcentajeAdicionalSiCY { get; set; }
        [DataMember]
        public string PorcentajeAdicionalNoCY { get; set; }
        [DataMember]
        public string ComunicacionCorreo { get; set; }
        [DataMember]
        public string ComunicacionDireccion { get; set; }



        [DataMember]
        public string BenPG1_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG2_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG3_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG4_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG5_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG6_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG7_Nacionalidad { get; set; }
        [DataMember]
        public string BenPG8_Nacionalidad { get; set; }



        [DataMember]
        public string BenPG1_Profesion { get; set; }
        [DataMember]
        public string BenPG2_Profesion { get; set; }
        [DataMember]
        public string BenPG3_Profesion { get; set; }
        [DataMember]
        public string BenPG4_Profesion { get; set; }
        [DataMember]
        public string BenPG5_Profesion { get; set; }
        [DataMember]
        public string BenPG6_Profesion { get; set; }
        [DataMember]
        public string BenPG7_Profesion { get; set; }
        [DataMember]
        public string BenPG8_Profesion { get; set; }



        [DataMember]
        public string BenPG1_Residencia { get; set; }
        [DataMember]
        public string BenPG2_Residencia { get; set; }
        [DataMember]
        public string BenPG3_Residencia { get; set; }
        [DataMember]
        public string BenPG4_Residencia { get; set; }
        [DataMember]
        public string BenPG5_Residencia { get; set; }
        [DataMember]
        public string BenPG6_Residencia { get; set; }
        [DataMember]
        public string BenPG7_Residencia { get; set; }
        [DataMember]
        public string BenPG8_Residencia { get; set; }

    }

    public class JsonFormatoOF
    {
        //[DataMember]
        //public string propuesta { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public string Nombres { get; set; }
        //[DataMember]
        //public string dni { get; set; }
        //[DataMember]
        //public string ce { get; set; }
        [DataMember]
        public string FechaNacimiento { get; set; }
        [DataMember]
        public string DocIdentidad { get; set; }
        [DataMember]
        public string Sexo { get; set; }
        [DataMember]
        public string EstadoCivil { get; set; }
        [DataMember]
        public string Nacionalidad { get; set; }
        [DataMember]
        public string Domicilio { get; set; }
        [DataMember]
        public string Distrito { get; set; }
        [DataMember]
        public string Provincia { get; set; }
        [DataMember]
        public string Departamento { get; set; }
        [DataMember]
        public string Pais { get; set; }
        [DataMember]
        public string CentroTrabajo { get; set; }
        [DataMember]
        public string ActividadEconomica { get; set; }
        [DataMember]
        public string IngresoNetoMensual { get; set; }
        [DataMember]
        public string Cargo { get; set; }
        [DataMember]
        public string Ocupacion { get; set; }
        [DataMember]
        public string MontoOperacion { get; set; }
        [DataMember]
        public string Moneda { get; set; }
        [DataMember]
        public string NumeroSolicitud { get; set; }
        //[DataMember]
        //public string piepagina1 { get; set; }
        //[DataMember]
        //public string piepaginasolicitud1 { get; set; }        
    }

    public class JsonFormatoPEP
    {
        [DataMember]
        public string propuesta { get; set; }
        [DataMember]
        public string apellidoPaterno { get; set; }
        [DataMember]
        public string apellidoMaterno { get; set; }
        [DataMember]
        public string nombre { get; set; }
        [DataMember]
        public string dni { get; set; }
        [DataMember]
        public string ce { get; set; }
        [DataMember]
        public string docIdentidad { get; set; }
        [DataMember]
        public string nacionalidadresidencia { get; set; }
        [DataMember]
        public string profesion { get; set; }
        [DataMember]
        public string nombreinstitucionlabora { get; set; }
        [DataMember]
        public string cargo { get; set; }
        [DataMember]
        public string piepagina1 { get; set; }
        [DataMember]
        public string piepaginasolicitud1 { get; set; }
    }
    
}
