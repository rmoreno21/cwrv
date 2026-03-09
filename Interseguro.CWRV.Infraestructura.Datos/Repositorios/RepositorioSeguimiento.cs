using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioSeguimiento : IRepositorioSeguimiento
    {

        public List<Seguimiento> Listar(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            List<Seguimiento> listaSeguimiento = new List<Seguimiento>();
            Seguimiento seguimeinto;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_consultar_reporte_seguimiento");
            db.AddInParameter(dbc, "@wl_num_jefe", DbType.String, (idJefe != "0") ? idJefe : null);
            db.AddInParameter(dbc, "@wl_num_supervisor", DbType.String, (idSupervisor != "0") ? idSupervisor : null);
            db.AddInParameter(dbc, "@wl_num_agente", DbType.String, (idAgente != "0") ? idAgente : null);
            db.AddInParameter(dbc, "@wl_num_cuispp", DbType.String, (cuspp != String.Empty) ? cuspp : null);
            db.AddInParameter(dbc, "@wl_fec_inicial", DbType.DateTime, fechaInicio);
            db.AddInParameter(dbc, "@wl_fec_final", DbType.DateTime, fechaTermino);
            db.AddInParameter(dbc, "@wl_pagina", DbType.Int32, indicePagina);
            db.AddInParameter(dbc, "@wl_registros", DbType.Int32, tamanhoPagina);
            db.AddInParameter(dbc, "@wl_orden", DbType.Int32, columnaOrdenar);
            db.AddInParameter(dbc, "@wl_direccion", DbType.String, direccionOrdenar);
            db.AddOutParameter(dbc, "@wo_registros", DbType.Int32, 0);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    seguimeinto = new Seguimiento();
                    seguimeinto.Jefe = dr["gls_jefe"].ToString();
                    seguimeinto.Supervisor = dr["gls_supervisor"].ToString();
                    seguimeinto.Agente = dr["gls_agente"].ToString();
                    //<SRIINI06326>
                    if (dr["ind_consentimiento"].ToString() == "S")
                    {
                        seguimeinto.CUSPP = dr["num_cuispp"].ToString();
                        seguimeinto.Persona = dr["gls_persona"].ToString();
                    }
                    else
                    {
                        seguimeinto.CUSPP = string.Empty;
                        seguimeinto.Persona = string.Empty;
                    }
                    //<SRIFIN06326>
                    seguimeinto.SitioGenerado = dr["cod_origen"].ToString();
                    seguimeinto.Telefono = dr["gls_telefono"].ToString();
                    seguimeinto.SaldoCIC = Convert.ToDouble(dr["val_mto_cta_individual"]);
                    seguimeinto.NroSolicitud = dr["num_solicitud"].ToString();
                    seguimeinto.Categoria = dr["gls_categoria"].ToString();
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                        seguimeinto.FechaIngreso = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                    if (dr["fec_cierre"] != DBNull.Value)
                        seguimeinto.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);
                    seguimeinto.Companhia = dr["gls_cia"].ToString();
                    listaSeguimiento.Add(seguimeinto);
                }
            }
            totalRegistros = Convert.ToInt32(db.GetParameterValue(dbc, "@wo_registros"));

            return listaSeguimiento;
        }

        public List<Seguimiento> ListarExcel(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar)
        {
            List<Seguimiento> listaSeguimiento = new List<Seguimiento>();
            Seguimiento seguimeinto;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_consultar_reporte_seguimiento_xls");
            db.AddInParameter(dbc, "@wl_num_jefe", DbType.String, (idJefe != "0") ? idJefe : null);
            db.AddInParameter(dbc, "@wl_num_supervisor", DbType.String, (idSupervisor != "0") ? idSupervisor : null);
            db.AddInParameter(dbc, "@wl_num_agente", DbType.String, (idAgente != "0") ? idAgente : null);
            db.AddInParameter(dbc, "@wl_num_cuispp", DbType.String, (cuspp != String.Empty) ? cuspp : null);
            db.AddInParameter(dbc, "@wl_fec_inicial", DbType.DateTime, fechaInicio);
            db.AddInParameter(dbc, "@wl_fec_final", DbType.DateTime, fechaTermino);
            db.AddInParameter(dbc, "@wl_orden", DbType.Int32, columnaOrdenar);
            db.AddInParameter(dbc, "@wl_direccion", DbType.String, direccionOrdenar);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    seguimeinto = new Seguimiento();
                    seguimeinto.Jefe = dr["gls_jefe"].ToString();
                    seguimeinto.Supervisor = dr["gls_supervisor"].ToString();
                    seguimeinto.Agente = dr["gls_agente"].ToString();
                    //<SRIINI06326>
                    if (dr["ind_consentimiento"].ToString() == "S")
                    {
                        seguimeinto.CUSPP = dr["num_cuispp"].ToString();
                        seguimeinto.Persona = dr["gls_persona"].ToString();
                    }
                    else 
                    {
                        seguimeinto.CUSPP = string.Empty;
                        seguimeinto.Persona = string.Empty;
                    }
                    //<SRIFIN06326>
                    seguimeinto.SitioGenerado = dr["cod_origen"].ToString();
                    seguimeinto.Telefono = dr["gls_telefono"].ToString();
                    seguimeinto.SaldoCIC = Convert.ToDouble(dr["val_mto_cta_individual"]);
                    seguimeinto.NroSolicitud = dr["num_solicitud"].ToString();
                    seguimeinto.Categoria = dr["gls_categoria"].ToString();
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                        seguimeinto.FechaIngreso = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                    if (dr["fec_cierre"] != DBNull.Value)
                        seguimeinto.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);
                    seguimeinto.Companhia = dr["gls_cia"].ToString();
                    listaSeguimiento.Add(seguimeinto);
                }
            }

            return listaSeguimiento;
        }

        public void Registrar(Seguimiento entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Seguimiento entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Seguimiento entity)
        {
            throw new NotImplementedException();
        }

        public Seguimiento ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Seguimiento ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Seguimiento> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
