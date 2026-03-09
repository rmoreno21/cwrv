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
    public class RepositorioSupervision : IRepositorioSupervision
    {

        public List<Supervision> Listar(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            List<Supervision> listaSupervision = new List<Supervision>();
            Supervision supervision;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_consultar_reporte_supervision");
            db.AddInParameter(dbc, "@wl_num_jefe", DbType.String, (idJefe != "0") ? idJefe : null);
            db.AddInParameter(dbc, "@wl_num_supervisor", DbType.String, (idSupervisor != "0") ? idSupervisor : null);
            db.AddInParameter(dbc, "@wl_num_agente", DbType.String, (idAgente != "0") ? idAgente : null);
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
                    supervision = new Supervision();
                    supervision.NumeroRegistro = Convert.ToInt32(dr["num_registro"]);
                    supervision.Jefe = dr["gls_jefe"].ToString();
                    supervision.Supervisor = dr["gls_supervisor"].ToString();
                    supervision.Agente = dr["gls_agente"].ToString();
                    supervision.Usuario = dr["cod_username"].ToString();
                    if (dr["fec_evento"] != DBNull.Value)
                        supervision.FechaEvento = Convert.ToDateTime(dr["fec_evento"]);
                    supervision.Evento = dr["gls_evento_larga"].ToString();
                    supervision.Detalle = dr["gls_detalle"].ToString();
                    listaSupervision.Add(supervision);
                }
            }
            totalRegistros = Convert.ToInt32(db.GetParameterValue(dbc, "@wo_registros"));

            return listaSupervision;
        }

        public List<Supervision> ListarExcel(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar)
        {
            List<Supervision> listaSupervision = new List<Supervision>();
            Supervision supervision;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_consultar_reporte_supervision_xls");
            db.AddInParameter(dbc, "@wl_num_jefe", DbType.String, (idJefe != "0") ? idJefe : null);
            db.AddInParameter(dbc, "@wl_num_supervisor", DbType.String, (idSupervisor != "0") ? idSupervisor : null);
            db.AddInParameter(dbc, "@wl_num_agente", DbType.String, (idAgente != "0") ? idAgente : null);
            db.AddInParameter(dbc, "@wl_fec_inicial", DbType.DateTime, fechaInicio);
            db.AddInParameter(dbc, "@wl_fec_final", DbType.DateTime, fechaTermino);
            db.AddInParameter(dbc, "@wl_orden", DbType.Int32, columnaOrdenar);
            db.AddInParameter(dbc, "@wl_direccion", DbType.String, direccionOrdenar);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    supervision = new Supervision();
                    supervision.NumeroRegistro = Convert.ToInt32(dr["num_registro"]);
                    supervision.Jefe = dr["gls_jefe"].ToString();
                    supervision.Supervisor = dr["gls_supervisor"].ToString();
                    supervision.Agente = dr["gls_agente"].ToString();
                    supervision.Usuario = dr["cod_username"].ToString();
                    if (dr["fec_evento"] != DBNull.Value)
                        supervision.FechaEvento = Convert.ToDateTime(dr["fec_evento"]);
                    supervision.Evento = dr["gls_evento_larga"].ToString();
                    supervision.Detalle = dr["gls_detalle"].ToString();
                    listaSupervision.Add(supervision);
                }
            }

            return listaSupervision;
        }

        public void Registrar(Supervision entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Supervision entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Supervision entity)
        {
            throw new NotImplementedException();
        }

        public Supervision ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Supervision ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Supervision> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
