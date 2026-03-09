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
    public class RepositorioCausalPoliza: IRepositorioCausalPoliza
    {
        public List<CausalPoliza> ListarCausalPolizaPlus()
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_obtener_causante_RtaPrvdPlus");

            dbc.CommandTimeout = 0;

            List<CausalPoliza> lstCausalPoliza = new List<CausalPoliza>();

            CausalPoliza causalPoliza = null;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    causalPoliza = new CausalPoliza();

                    if (dr["cod_causal_estado_poliza"] != DBNull.Value)
                        causalPoliza.Id = dr["cod_causal_estado_poliza"].ToString();

                    if (dr["gls_corta_caupol"] != DBNull.Value)
                        causalPoliza.NombreCorto = dr["gls_corta_caupol"].ToString();

                    if (dr["gls_causal_estado_poliza"] != DBNull.Value)
                        causalPoliza.NombreLargo = dr["gls_causal_estado_poliza"].ToString();

                    lstCausalPoliza.Add(causalPoliza);
                }
            }
            return lstCausalPoliza;
        }

        public void Registrar(CausalPoliza entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(CausalPoliza entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(CausalPoliza entity)
        {
            throw new NotImplementedException();
        }

        public CausalPoliza ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public CausalPoliza ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<CausalPoliza> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
