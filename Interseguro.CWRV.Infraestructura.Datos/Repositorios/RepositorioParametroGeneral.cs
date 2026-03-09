//<SRIINI06326>
using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioParametroGeneral: IRepositorioParametroGeneral 
    {
        public double obtenerValorComisionAgente(string moneda)
        {
            double valor=0;
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_param_comision_agente", moneda))
            {
                while (dr.Read())
                {
                    valor = Convert.ToDouble( dr["val_parametro"]);
                }
            }
            return valor;
        }

        public SDAReporte obtenerPreCubo(int numeroAgente, string cuspp)
        {
            SDAReporte reporte = new SDAReporte();
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("ConexionSDA");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_pre_cubo", numeroAgente, cuspp))
            {
                if (dr.Read())
                {
                    if (dr["precubo"] != DBNull.Value)
                        reporte.Precubo = dr["precubo"].ToString();
                    if (dr["avancecartera"] != DBNull.Value)
                        reporte.AvanceCartera = Convert.ToDouble(dr["avancecartera"]);
                }
            }
            return reporte;
        }

        public ConfiguracionCorreo ObtenerConfiguracionCorreo(int cod_proceso, DateTime fec_solicitud)
        {
            ConfiguracionCorreo configuracionCorreo = new ConfiguracionCorreo();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_configuracion_correo_sel", cod_proceso, fec_solicitud))
            {
                while (dr.Read())
                {
                    configuracionCorreo.cod_proceso = Convert.ToInt32(dr["cod_proceso"]);
                    configuracionCorreo.fec_inicio_vigencia = Convert.ToDateTime(dr["fec_inicio_vigencia"]);
                    configuracionCorreo.fec_termino_vigencia = Convert.ToDateTime(dr["fec_termino_vigencia"]);
                    configuracionCorreo.gls_configuracion = dr["gls_configuracion"].ToString();
                    configuracionCorreo.arc_documento_correo = dr["arc_documento_correo"].ToString();
                    configuracionCorreo.gls_asunto = dr["gls_asunto"].ToString();
                    configuracionCorreo.gls_remitente = dr["gls_remitente"].ToString();
                    configuracionCorreo.gls_display_name = dr["gls_display_name"].ToString();
                    //configuracionCorreo.gls_destinatario = dr["gls_destinatario"].ToString();
                    //configuracionCorreo.gls_ruta_servicio = dr["gls_ruta_servicio"].ToString();
                }
            }

            return configuracionCorreo;
        }

        public double ObtenerTipoCambio(string codigo, DateTime fecha, string usuario)
        {
            try
            {
                double valor = 0;
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_tipo_cambio", fecha, codigo, usuario))
                {
                    if (dr.Read())
                    {
                        valor = Convert.ToDouble(dr["val_dato"]);
                    }
                }
                return valor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
