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
    public class RepositorioAdelantoComision: IRepositorioAdelantoComision
    {


        public List<PorcentajeComision> ListarAdelantos()
        {
            List<PorcentajeComision> listaAdelantos = new List<PorcentajeComision>();

            PorcentajeComision porcentaje;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_adelanto_comision"))
            {
                while (dr.Read())
                {
                    porcentaje = new PorcentajeComision();
                    porcentaje.Codigo = dr["cod_codigo"].ToString();
                    porcentaje.Glosa = dr["gls_corta"].ToString();
                    porcentaje.ValorAdicional = Convert.ToDouble(dr["val_valor_adicional"]);

                    listaAdelantos.Add(porcentaje);
                }
            }

            return listaAdelantos;
        }
        
        
        public AdelantoComision ObtenerConfiguracionAdelanto(AdelantoComision parametros)
        {
            AdelantoComision config=null;
            
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_obtener_configuracion_adelanto_comision");
            db.AddInParameter(dbc, "@wl_val_mto_cta_individual", DbType.Decimal, parametros.MontoCIC);
            db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, parametros.Afp.Id);
            db.AddInParameter(dbc, "@wl_cod_tipo_pension", DbType.String, parametros.CodigoTipoPension);
            db.AddInParameter(dbc, "@wl_ind_reja", DbType.String, parametros.IndReja);
            db.AddInParameter(dbc, "@wl_fec_cotizacion", DbType.DateTime , parametros.FechaCotizacion);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    config = new AdelantoComision();
                    config.ValAcom  = Convert.ToDouble(dr["val_acom"]);
                    config.RangoInicial = Convert.ToDecimal(dr["num_rango_ini"]);
                    config.RangoFinal = Convert.ToDecimal(dr["num_rango_fin"]);
                    config.Afp = new AFP { Id = dr["cod_afp"].ToString() };
                    config.CodigoTipoPension = dr["cod_tipo_pension"].ToString();
                    config.IndReja = dr["ind_reja"].ToString();
                    config.InicioVigencia =Convert.ToDateTime(dr["fec_ini_vigencia"]);
                    config.TerminoVigencia = Convert.ToDateTime(dr["fec_fin_vigencia"]);
                }
            }
            return config;
        }
        
        
        public void Registrar(AdelantoComision entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(AdelantoComision entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(AdelantoComision entity)
        {
            throw new NotImplementedException();
        }

        public AdelantoComision ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public AdelantoComision ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<AdelantoComision> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
//<SRIFIN06326>