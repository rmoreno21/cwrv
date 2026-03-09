//<SRIINI06326>
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
    public class RepositorioDescuentoComision:IRepositorioDescuentoComision
    {
        public List<PorcentajeComision> ListarDescuentos(DateTime fechaCotizacion)
        {
            List<PorcentajeComision> listaDescuentos = new List<PorcentajeComision>();

            PorcentajeComision porcentaje;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_descuento_comision", fechaCotizacion))//<INIGTI_4022>
            {
                while (dr.Read())
                {
                    porcentaje = new PorcentajeComision();
                    porcentaje.Codigo = dr["cod_codigo"].ToString();
                    porcentaje.Glosa = dr["gls_corta"].ToString();
                    porcentaje.ValorAdicional = Convert.ToDouble(dr["val_valor_adicional"]);

                    listaDescuentos.Add(porcentaje);
                }
            }

            return listaDescuentos;
        }
    }
}
//<SRIFIN06326>