using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data.SqlClient;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioAnticipo : IRepositorioAnticipo
    {
        public Anticipo ObtenerDatos(string solicitud)
        {
            Anticipo anticipo = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_sel_valida_rpt_acom_agente", solicitud))
            {
                if (dr.Read())
                {
                    anticipo = new Anticipo();
                    anticipo.Solicitud = new Solicitud
                    { 
                        Id = dr["num_operacion"].ToString()
                    };
                    anticipo.Agente = new Agente
                    {
                        Id = dr["num_agente"].ToString(),
                        Nombre = dr["nom_agente"].ToString(),
                        FechaIngreso = Convert.ToDateTime(dr["fec_inicio_vigencia"])
                    };
                    anticipo.Supervisor = new Agente
                    {
                        Id = dr["num_supervisor"].ToString(),
                        Nombre = dr["nom_supervisor"].ToString(),
                    };
                    anticipo.Monto = Convert.ToDouble(dr["val_importe_documento"]);
                    anticipo.Estado = dr["cod_estado"].ToString();
                    anticipo.Afiliado = new Afiliado
                    {
                        Nombre = dr["nom_persona"].ToString(),
                        CUSPP = dr["num_cuspp"].ToString()
                    };
                    anticipo.MesesIngreso = Convert.ToInt32(dr["num_meses_ingreso"].ToString());
                    anticipo.MontoMaximo = Convert.ToDouble(dr["val_monto_maximo"].ToString());
                    anticipo.DiasDevolucion = Convert.ToInt32(dr["num_dias_devolucion"].ToString());
                }
            }

            return anticipo;
        }

        public Anticipo ObtenerDatosAceptacion(string solicitud, string agente)
        {
            Anticipo anticipo = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_anticipo_aceptado", agente, solicitud))
            {
                if (dr.Read())
                {
                    anticipo = new Anticipo();
                    anticipo.Solicitud = new Solicitud
                    {
                        Id = dr["num_solicitud"].ToString()
                    };
                    //anticipo.Agente = new Agente
                    //{
                    //    Id = dr["num_agente"].ToString(),
                    //    Nombre = dr["nom_agente"].ToString(),
                    //    FechaIngreso = Convert.ToDateTime(dr["fec_inicio_vigencia"])
                    //};
                    anticipo.Agente = new Agente
                    {
                        Id = dr["num_agente"].ToString()
                    };
                    anticipo.FechaAceptacion = Convert.ToDateTime(dr["fec_aceptacion"]);
                    //anticipo.Supervisor = new Agente
                    //{
                    //    Id = dr["num_supervisor"].ToString(),
                    //    Nombre = dr["nom_supervisor"].ToString(),
                    //};
                    //anticipo.Monto = Convert.ToDouble(dr["val_importe_documento"]);
                    //anticipo.Estado = dr["cod_estado"].ToString();
                    //anticipo.Afiliado = new Afiliado
                    //{
                    //    Nombre = dr["nom_persona"].ToString(),
                    //    CUSPP = dr["num_cuspp"].ToString()
                    //};
                    //anticipo.MesesIngreso = Convert.ToInt32(dr["num_meses_ingreso"].ToString());
                    //anticipo.MontoMaximo = Convert.ToDouble(dr["val_monto_maximo"].ToString());
                    //anticipo.DiasDevolucion = Convert.ToInt32(dr["num_dias_devolucion"].ToString());
                }
            }

            return anticipo;
        }

        public Anticipo ObtenerDatosCondiciones(string solicitud)
        {
            Anticipo anticipo = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_anticipo_condiciones", solicitud))
            {
                if (dr.Read())
                {
                    anticipo = new Anticipo();
                    anticipo.Solicitud = new Solicitud
                    {
                        Id = dr["num_solicitud"].ToString()
                    };
                    anticipo.Agente = new Agente
                    {
                        Id = dr["num_agente"].ToString(),
                        Nombre = dr["nom_agente"].ToString(),
                        FechaIngreso = Convert.ToDateTime(dr["fec_inicio_vigencia"])
                    };
                    anticipo.Supervisor = new Agente
                    {
                        Id = dr["num_supervisor"].ToString(),
                        Nombre = dr["nom_supervisor"].ToString(),
                    };
                    anticipo.Monto = Convert.ToDouble(dr["val_importe_documento"]);
                    anticipo.Afiliado = new Afiliado
                    {
                        Nombre = dr["nom_persona"].ToString(),
                        CUSPP = dr["num_cuspp"].ToString()
                    };
                    anticipo.MesesIngreso = Convert.ToInt32(dr["num_meses_ingreso"].ToString());
                    anticipo.MontoMaximo = Convert.ToDouble(dr["val_monto_maximo"].ToString());
                    anticipo.DiasDevolucion = Convert.ToInt32(dr["num_dias_devolucion"].ToString());
                }
            }

            return anticipo;
        }

        public void RegistrarAceptacion(Anticipo entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_anticipo_aceptado");

                db.AddInParameter(dbc, "@wl_num_agente", DbType.String, entity.Agente.Id);
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.Solicitud.Id);
                db.AddInParameter(dbc, "@wl_val_monto", DbType.Double, entity.Monto);
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Registrar(Anticipo entity)
        {
            throw new System.NotImplementedException();
        }

        public void Actualizar(Anticipo entity)
        {
            throw new System.NotImplementedException();
        }

        public void Eliminar(Anticipo entity)
        {
            throw new System.NotImplementedException();
        }

        public Anticipo ObtenerPorId(long Id)
        {
            throw new System.NotImplementedException();
        }

        public List<Anticipo> Listar()
        {
            throw new System.NotImplementedException();
        }

        public List<Anticipo> ListarAceptacion(string solicitud, string agente, DateTime? fechaInicio, DateTime? fechaFin, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
        {
            List<Anticipo> listaAnticipos = new List<Anticipo>();
            Anticipo anticipo;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_listar_anticipo_aceptado");
            db.AddInParameter(dbc, "@wl_num_agente", DbType.Int32, agente);
            db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, solicitud);
            db.AddInParameter(dbc, "@wl_fec_inicio", DbType.DateTime, fechaInicio);
            db.AddInParameter(dbc, "@wl_fec_fin", DbType.DateTime, fechaFin);
            db.AddInParameter(dbc, "@wl_pagina", DbType.Int32, indicePagina);
            db.AddInParameter(dbc, "@wl_registros", DbType.Int32, tamanhoPagina);
            db.AddInParameter(dbc, "@wl_orden", DbType.Int32, columnaOrdenar);
            db.AddInParameter(dbc, "@wl_direccion", DbType.String, direccionOrdenar);
            db.AddOutParameter(dbc, "@wo_registros", DbType.Int32, 0);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    anticipo = new Anticipo();
                    anticipo.Agente = new Agente
                    {
                        Id = dr["num_agente"].ToString(),
                        Nombre = dr["nom_agente"].ToString()
                    };
                    anticipo.Solicitud = new Solicitud
                    {
                        Id = dr["num_solicitud"].ToString()
                    };
                    anticipo.Monto = Convert.ToDouble(dr["val_monto"]);
                    anticipo.FechaAceptacion = Convert.ToDateTime(dr["fec_aceptacion"]);

                    listaAnticipos.Add(anticipo);
                }
            }

            return listaAnticipos;
        }

        public Anticipo ObtenerPorId(string Id)
        {
            throw new System.NotImplementedException();
        }
    }
}
