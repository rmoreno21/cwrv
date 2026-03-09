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
    public class RepositorioTipoMovimiento : IRepositorioTipoMovimiento
    {

        public List<RolAzmanTipoMovimiento> ObtenerTipoMovimientoPorRolAzman(string codRol)
        {
            List<RolAzmanTipoMovimiento> listaRolAzmanTipoMovimiento = new List<RolAzmanTipoMovimiento>();
            RolAzmanTipoMovimiento rolAzmanTipoMovimiento;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_lis_rol_azman_tipo_movimiento", codRol))
            {
                while (dr.Read())
                {
                    rolAzmanTipoMovimiento = new RolAzmanTipoMovimiento();
                    rolAzmanTipoMovimiento.CodRolAzman = dr["cod_rol_azman"].ToString();
                    rolAzmanTipoMovimiento.CodTipoMovimiento = Convert.ToInt32(dr["cod_tipo_movimiento"]);
                    if (dr["gls_rol_azman"] != DBNull.Value && dr["gls_rol_azman"].ToString() != string.Empty)
                        rolAzmanTipoMovimiento.GlsRolAzman = dr["gls_rol_azman"].ToString();

                    listaRolAzmanTipoMovimiento.Add(rolAzmanTipoMovimiento);
                }
            }

            return listaRolAzmanTipoMovimiento;
        }

        public List<CotizacionMovimiento> ObtenerCotizacionTipoMovimientoPorSolicitud(string numSolicitud)
        {
            List<CotizacionMovimiento> listaCotizacionMovimiento = new List<CotizacionMovimiento>();
            CotizacionMovimiento cotizacionMovimiento;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_lis_cotiza_movimiento", numSolicitud))
            {
                while (dr.Read())
                {
                    cotizacionMovimiento = new CotizacionMovimiento();
                    cotizacionMovimiento.NumSolicitud = dr["num_solicitud"].ToString();
                    cotizacionMovimiento.FecCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    //cotizacionMovimiento.Correlativo = Convert.ToInt32(dr["num_correlativo"]);
                    cotizacionMovimiento.NumMovimiento = Convert.ToInt32(dr["num_movimiento"]);
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        cotizacionMovimiento.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };
                    if (dr["gls_movimiento"] != DBNull.Value && dr["gls_movimiento"].ToString() != string.Empty)
                        cotizacionMovimiento.GlsMovimiento = dr["gls_movimiento"].ToString();
                    //cotizacionMovimiento.ValTasaAjusteTra = Convert.ToInt32(dr["val_tasa_ajuste_tra"]);
                    cotizacionMovimiento.FecInicioMovimiento = Convert.ToDateTime(dr["fec_inicio_movimiento"]);
                    if (dr["fec_fin_movimiento"] != DBNull.Value && dr["fec_fin_movimiento"].ToString() != string.Empty)
                        cotizacionMovimiento.FecFinMovimiento = Convert.ToDateTime(dr["fec_fin_movimiento"]);
                    if (dr["aud_usr_ingreso"] != DBNull.Value && dr["aud_usr_ingreso"].ToString() != string.Empty)
                        cotizacionMovimiento.Usuario = new Usuario
                        {
                            NombreUsuario = dr["aud_usr_ingreso"].ToString(),
                            FecIngreso = (dr["aud_fec_ingreso"] != DBNull.Value && dr["aud_fec_ingreso"].ToString() != string.Empty) ? Convert.ToDateTime(dr["aud_fec_ingreso"]) : Convert.ToDateTime("01/01/1900")
                        };

                    listaCotizacionMovimiento.Add(cotizacionMovimiento);
                }
            }

            return listaCotizacionMovimiento;
        }

        public void Registrar(TipoMovimiento entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(TipoMovimiento entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(TipoMovimiento entity)
        {
            throw new NotImplementedException();
        }

        public TipoMovimiento ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public TipoMovimiento ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<TipoMovimiento> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
