using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCuotasTra : System.Web.UI.UserControl
    {
        public List<CuotasTra> lstCuotasTra { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TabCuotasTra.DataSource = lstCuotasTra;
                TabCuotasTra.DataBind();
                if (lstCuotasTra.Count > 0)
                {
                    TabCuotasTra.HeaderRow.TableSection = TableRowSection.TableHeader;
                    var json = new JavaScriptSerializer().Serialize(lstCuotasTra);
                    HCuotasTra.Value = json;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}