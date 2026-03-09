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
    public class RepositorioActividad: IRepositorioActividad
    {
        public List<Actividad> Listar(string cuspp)
        {
            List<Actividad> listaActividades = new List<Actividad>();
            Actividad actividad;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_actividades", cuspp))
            {
                while (dr.Read())
                {
                    actividad = new Actividad();
                    actividad.Correlativo = dr["num_correlativo"].ToString();
                    actividad.TipoEvento = dr["gls_tipo_evento"].ToString();
                    actividad.GlosaEvento = dr["gls_evento"].ToString();
                    if (dr["fec_evento"] != DBNull.Value)
                        actividad.FechaEvento = Convert.ToDateTime(dr["fec_evento"]);
                    actividad.Resultado= dr["gls_resultado"].ToString();
                    actividad.Comentario = dr["gls_comentario"].ToString();
                    actividad.Username = dr["username"].ToString();

                    listaActividades.Add(actividad);
                }
            }

            return listaActividades;
        }

        public void Registrar(Actividad entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Actividad entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Actividad entity)
        {
            throw new NotImplementedException();
        }

        public Actividad ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Actividad ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Actividad> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
