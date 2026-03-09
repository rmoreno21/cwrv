using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiariosRPP : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Beneficiarios { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TablaRPPBeneficiarios.DataSource = Beneficiarios;
            TablaRPPBeneficiarios.DataBind();
        }
    }
}