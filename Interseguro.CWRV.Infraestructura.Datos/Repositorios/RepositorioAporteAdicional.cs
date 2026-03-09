using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioAporteAdicional : IRepositorioAporteAdicional
    {
        public AporteAdicional ObtenerDatos(string CUSPP) {

            AporteAdicional aporte = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_Rvi_AporteAdicional", CUSPP))
            {
                if (dr.Read())
                {
                    aporte = new AporteAdicional();
                    aporte.num_cuispp = dr["num_cuispp"].ToString();
                    aporte.fec_pagoapad = Convert.ToDateTime(dr["fec_pagoapad"]);
                    aporte.val_pension_referencia = Convert.ToDouble(dr["val_pension_referencia"]);
                    aporte.cod_moneda_pension_ref = dr["cod_moneda_pension_ref"].ToString();
                    aporte.val_tasa_aporte = Convert.ToDouble(dr["val_tasa_aporte"]);
                    if (dr["val_monto_aporte"] != DBNull.Value)
                        aporte.val_monto_aporte = Convert.ToDouble(dr["val_monto_aporte"]);
                    if (dr["val_pension_a_pago"] != DBNull.Value)
                        aporte.val_pension_a_pago = Convert.ToDouble(dr["val_pension_a_pago"]);
                    if (dr["cod_moneda_pension_a_pago"] != DBNull.Value)
                        aporte.cod_moneda_pension_a_pago = dr["cod_moneda_pension_a_pago"].ToString();
                    if (dr["val_pension_elegida"] != DBNull.Value)
                        aporte.val_pension_elegida = Convert.ToDouble(dr["val_pension_elegida"]);
                }
            }

            return aporte;
        }

        public void Actualizar(AporteAdicional entity, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_rvi_aporteadicional");

                db.AddInParameter(dbc, "@wl_num_cuispp", DbType.String, entity.num_cuispp);
                db.AddInParameter(dbc, "@wl_fec_pagoapad", DbType.DateTime, entity.fec_pagoapad);
                db.AddInParameter(dbc, "@wl_val_pension_referencia", DbType.Double, entity.val_pension_referencia);
                db.AddInParameter(dbc, "@wl_cod_moneda_pension_ref", DbType.String, entity.cod_moneda_pension_ref);
                db.AddInParameter(dbc, "@wl_val_tasa_aporte", DbType.Double, entity.val_tasa_aporte);
                db.AddInParameter(dbc, "@wl_val_monto_aporte", DbType.Double, entity.val_monto_aporte);
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, usuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(AporteAdicional entity, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_rvi_aporteadicional");

                db.AddInParameter(dbc, "@wl_num_cuispp", DbType.String, entity.num_cuispp);
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, usuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Registrar(AporteAdicional entity)
        {
            throw new NotImplementedException();
        }
        
        public void Actualizar(AporteAdicional entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(AporteAdicional entity)
        {
            throw new NotImplementedException();
        }

        public AporteAdicional ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public AporteAdicional ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<AporteAdicional> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
