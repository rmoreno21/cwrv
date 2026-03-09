using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class ReporteIndicadoresRRVV
    {
        [DataMember]
        public string CUSPP { get; set; }
        [DataMember]
        public string Solicitud { get; set; }
        [DataMember]
        public int? NroBeneficiario { get; set; }
        [DataMember]
        public string TipoPensión { get; set; }
        [DataMember]
        public string Mail { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public string Nombres { get; set; }
        [DataMember]
        public string ConsentimientoCdA { get; set; }
        [DataMember]
        public string FechaEnvioConsentimientoCdA { get; set; }
        [DataMember]
        public string FechaAceptacionConsentimientoCdA { get; set; }
        [DataMember]
        public string ConsentimientoVCTP { get; set; }
        [DataMember]
        public string FechaEnvioConsentimientoVCTP { get; set; }
        [DataMember]
        public string FechaAceptacionConsentimientoVCTP { get; set; }
        [DataMember]
        public string FechaCierreComercial { get; set; }

    }
}
