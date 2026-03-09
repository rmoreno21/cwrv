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
    public class RepositorioGestionVentas : IRepositorioGestionVentas
    {
        public List<GestionVentas> Listar(string cuspp)
        {
            throw new NotImplementedException();
        }

        public List<GestionVentas> ConsultarGestionVentas(DateTime fechaInicial, DateTime fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_consultar_reporte_gestion_ventas");

            db.AddInParameter(dbc, "@wl_fecha_inicial", DbType.DateTime, fechaInicial);
            db.AddInParameter(dbc, "@wl_fecha_final", DbType.DateTime, fechaFinal);
            db.AddInParameter(dbc, "@wl_num_jefe", DbType.Int32, numJefe);
            db.AddInParameter(dbc, "@wl_num_superv", DbType.Int32, numSuperv);
            db.AddInParameter(dbc, "@wl_num_agente", DbType.Int32, numAgente);
            db.AddInParameter(dbc, "@wl_indicador_cierre", DbType.String, indCierre);
            db.AddInParameter(dbc, "@wl_tipo_cotizacion", DbType.String, tipoCotizacion);
            db.AddInParameter(dbc, "@wl_cod_cia_seguro", DbType.String, codCiaSeguro);

            dbc.CommandTimeout = 0;

            List<GestionVentas> lstGestionVentas = new List<GestionVentas>();

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    GestionVentas gestionVentas = new GestionVentas();

                    gestionVentas.NumeroMeler = dr["num_solicitud"].ToString();
                    if (dr["fec_presentacion"] != DBNull.Value)
                        gestionVentas.FechaPlazoAFP = Convert.ToDateTime(dr["fec_presentacion"]);

                    gestionVentas.CUSPP = dr["num_cuispp"].ToString();
                    gestionVentas.NombreCliente = dr["gls_nom_persona"].ToString();
                    gestionVentas.AFP = new AFP { Nombre = dr["gls_afp"].ToString() };
                    gestionVentas.Categoria = new Categoria { Id = dr["cod_categoria"].ToString(), Nombre = dr["gls_categoria"].ToString() };
                    gestionVentas.CIC = Convert.ToDouble(dr["val_mto_cta_individual"]);

                    if (dr["fec_cierre"] != DBNull.Value)
                        gestionVentas.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);

                    if (dr["cod_modalidad"] != DBNull.Value)
                        gestionVentas.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString(), Nombre = dr["ind_modalidad"].ToString() };

                    if (dr["num_cotizacion"] != DBNull.Value)
                        gestionVentas.NumeroCotizacion = dr["num_cotizacion"].ToString();

                    if (dr["gls_moneda"] != DBNull.Value)
                        gestionVentas.Moneda = new Moneda { Nombre = dr["gls_moneda"].ToString() };

                    if (dr["ACom"] != DBNull.Value)
                        gestionVentas.ACOM = Convert.ToDouble(dr["ACom"]);

                    if (dr["DCom"] != DBNull.Value)
                        gestionVentas.DCOM = Convert.ToDouble(dr["DCom"]);

                    if (dr["DifTra"] != DBNull.Value)
                        gestionVentas.DifTra = Convert.ToDouble(dr["DifTra"]);

                    if (dr["ValTasaIS"] != DBNull.Value)
                        gestionVentas.TasaIS = Convert.ToDouble(dr["ValTasaIS"]);

                    if (dr["ValTasaCIAGanadora"] != DBNull.Value)
                        gestionVentas.TasaCiaGanadora = Convert.ToDouble(dr["ValTasaCIAGanadora"]);

                    if (dr["fec_dia_cita"] != DBNull.Value)
                        gestionVentas.FechaCita = Convert.ToDateTime(dr["fec_dia_cita"]);

                    //if (dr["gls_nom_direccion"] != DBNull.Value)
                    //    gestionVentas.LugarCita = dr["gls_nom_direccion"].ToString();
                    if (dr["gls_lugar_cita"] != DBNull.Value)
                        gestionVentas.LugarCita = dr["gls_lugar_cita"].ToString();

                    if (dr["num_vendedor"] != DBNull.Value)
                        gestionVentas.Agente = new Agente { Id = dr["num_vendedor"].ToString(), Nombre = dr["nom_vendedor"].ToString() };

                    if (dr["gls_nom_ciudad"] != DBNull.Value)
                        gestionVentas.UbigeoAgente = dr["gls_nom_ciudad"].ToString();

                    if (dr["gls_nom_supervisor"] != DBNull.Value)
                        gestionVentas.Supervisor = dr["gls_nom_supervisor"].ToString();

                    if (dr["gls_nom_agencia"] != DBNull.Value)
                        gestionVentas.NombreAgencia = dr["gls_nom_agencia"].ToString();

                    if (dr["ind_recotiza"] != DBNull.Value)
                        gestionVentas.Recotizacion = dr["ind_recotiza"].ToString();

                    if (dr["num_supervisor"] != DBNull.Value)
                        gestionVentas.Num_Supervisor = Convert.ToInt32(dr["num_supervisor"]);

                    if (dr["val_per_temporal"] != DBNull.Value)
                        gestionVentas.PeriodoDiferido = Convert.ToInt32(dr["val_per_temporal"]);

                    if (dr["val_per_garantizado"] != DBNull.Value)
                        gestionVentas.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);

                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        gestionVentas.PorcentajeRenta = Convert.ToInt32(dr["val_pje_rent_temp"]);

                    if (dr["gls_corta_compania_ganadora"] != DBNull.Value)
                        gestionVentas.CompaniaGanadora = dr["gls_corta_compania_ganadora"].ToString();

                    //<INIGTI_4081_3>
                    if (dr["num_jefe"] != DBNull.Value)
                        gestionVentas.Num_Jefe = Convert.ToInt32(dr["num_jefe"]);

                    if (dr["nom_jefe"] != DBNull.Value)
                        gestionVentas.Jefe = dr["nom_jefe"].ToString();
                    //<FINGTI_4081_3>

                    lstGestionVentas.Add(gestionVentas);
                }
            }
            return lstGestionVentas;
        }

        public void Registrar(GestionVentas entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(GestionVentas entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(GestionVentas entity)
        {
            throw new NotImplementedException();
        }

        public GestionVentas ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public GestionVentas ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<GestionVentas> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
