using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioCita : IRepositorioCita
    {

        public List<Cita> Listar(Cita citaIn)
        {
            List<Cita> listaCita = new List<Cita>();
            Cita cita;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("ConexionCRM");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_lis_crm_visitas_agentes", citaIn.Agente.Id, citaIn.Afiliado.CUSPP, citaIn.XMLParametroEstadoCita()))
            {
                while (dr.Read())
                {
                    cita = new Cita();

                    if (dr["fec_real"] != DBNull.Value)
                        cita.FechaReal = Convert.ToDateTime(dr["fec_real"]);
                    if (dr["fec_programada"] != DBNull.Value)
                        cita.FechaProgramada = Convert.ToDateTime(dr["fec_programada"]);
                    if (dr["cod_estado_cita"] != DBNull.Value)
                        cita.EstadoCita = new EstadoCita { Id = dr["cod_estado_cita"].ToString() };
                    if (dr["fec_ultima_cita"] != DBNull.Value)
                        cita.UltimaCitaEfectiva = Convert.ToDateTime(dr["fec_ultima_cita"]);
                    if (dr["num_cuspp"] != DBNull.Value)
                        cita.Afiliado = new Afiliado { CUSPP = dr["num_cuspp"].ToString() };
                    if (dr["cod_pronostico"] != DBNull.Value)
                        cita.Pronostico = new Pronostico { Id = dr["cod_pronostico"].ToString(), Nombre = dr["val_pronostico"].ToString() };
                    if (dr["val_avance_cartera"] != DBNull.Value)
                        cita.AvanceCartera = Convert.ToDouble(dr["val_avance_cartera"]);

                    listaCita.Add(cita);
                }
            }

            return listaCita;
        }

        public void Registrar(ref Cita entity)
        {
            throw new NotImplementedException();
        }

        public void RegistrarExtraoficial(ref Cita entity)
        {
            throw new NotImplementedException();
        }

        public void Registrar(Cita entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Cita entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Cita entity)
        {
            throw new NotImplementedException();
        }

        public Cita ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Cita ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Cita> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
