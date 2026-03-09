using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

//<INIGTI_4081>
namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioFlujoMovimiento : IRepositorioFlujoMovimiento
    {

        public List<FlujoMovimiento> ObtenerFlujos(DateTime fecCotizacion, string evento, string rol)
        {
            List<FlujoMovimiento> listaFlujoMovimiento = new List<FlujoMovimiento>();
            FlujoMovimiento flujoMovimiento;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_flujo_movimiento", fecCotizacion, evento, rol))
            {
                while (dr.Read())
                {
                    flujoMovimiento = new FlujoMovimiento();
                    flujoMovimiento.Id = Convert.ToInt32(dr["cod_tipo_movimiento"]);
                    flujoMovimiento.GlsMovimiento = dr["gls_tipo_movimiento"].ToString();
                    flujoMovimiento.MsjAlerta = dr["msj_alerta"].ToString();
                    flujoMovimiento.Rol = dr["cod_rol"].ToString();
                    flujoMovimiento.Evento = dr["evento"].ToString();
                    flujoMovimiento.Evento = dr["evento"].ToString();
                    flujoMovimiento.Origen = Convert.ToInt32(dr["cod_tipo_movimiento_origen"]);
                    flujoMovimiento.Destino = Convert.ToInt32(dr["cod_tipo_movimiento_destino"]);
                    flujoMovimiento.MsjFlujo = dr["msj_flujo"].ToString();
                    listaFlujoMovimiento.Add(flujoMovimiento);
                }
            }
            return listaFlujoMovimiento;
        }

        public void Registrar(FlujoMovimiento entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(FlujoMovimiento entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(FlujoMovimiento entity)
        {
            throw new NotImplementedException();
        }

        public FlujoMovimiento ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public FlujoMovimiento ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<FlujoMovimiento> Listar()
        {
            throw new NotImplementedException();
        }


        public bool ValidaFlujoSolicitudRol(string num_solicitud, string rol, string evento)
        {
            bool valida = false;
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_valida_flujo_solicitud_rol", num_solicitud, rol, evento))
            {
                while (dr.Read())
                {
                    valida = true;
                }
            }
            return valida;
        }

        public string ObtenerArchivosExistentes(string num_solicitud)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            string archivos_Existentes = string.Empty;

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_archivos_existentes", num_solicitud))
            {
                while (dr.Read())
                {
                    archivos_Existentes += dr["gls_archivos_existentes"].ToString() + ",";
                }
            }
            return archivos_Existentes;
        }
   
    }
}

//<FINGTI_4081>