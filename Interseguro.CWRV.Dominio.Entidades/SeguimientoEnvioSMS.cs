using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    public class SeguimientoEnvioSMS
    {
        public int codigo { get; set; }
        public string cod_tipo_identificacion { get; set; }
        public string gls_num_identificacion { get; set; }
        public string gls_celular { get; set; }
        public string cod_agente { get; set; }
        public int id_proceso_envio { get; set; }
        public DateTime fec_envio { get; set; }
        public int cod_proveedor_envio_sms { get; set; }
        public bool? ind_enviado { get; set; }
        public string id_envio { get; set; }

        public string aud_usr_ingreso { get; set; }
        public DateTime aud_fec_ingreso { get; set; }
        public string aud_usr_modificacion { get; set; }
        public DateTime? aud_fec_modificacion { get; set; }
    }
}
