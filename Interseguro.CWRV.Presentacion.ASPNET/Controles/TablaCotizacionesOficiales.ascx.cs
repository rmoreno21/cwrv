using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using System.Configuration;
using System.Web.Configuration;
using Interseguro.CWRV.Infraestructura.General;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesOficiales : System.Web.UI.UserControl
    {
        public List<Cotizacion> Cotizaciones { get; set; }
        public long CotizacionElegida { get; set; }
        public bool PermisoTRA { get; set; }
        public bool PermisoRadio { get; set; }
        public bool MostrarPorcentajeCapital { get; set; }
        public bool MostrarTasas { get; set; }

        public int minPBS { get; set; }
        public int maxPBS { get; set; }
        public bool RedLocal { get; set; }

        public string Origen { get; set; } //Origen="OFICIAL","BANDEJA"
        public string Modo { get; set; } //Modo="M","N" (Modificar, Nuevo)
        public bool mostrarTraMaxMin { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //<INIGTI_4081>
            minPBS = Convert.ToInt32(ConfigurationManager.AppSettings["MinPBS"]);
            maxPBS = Convert.ToInt32(ConfigurationManager.AppSettings["MaxPBS"]);

            string codRol = (string)HttpContext.Current.Session["RolAzman"];
            bool mostrarTRA = false;

            switch (codRol)
            {
                case "AGT.LIM.RVI"://AgenteLima
                case "AGT.PRO.RVI"://AgenteProvincia
                case "SPV.LIM.RVI"://SupervisorLima
                case "SPV.PRO.RVI"://SupervisorProvincia
                    mostrarTRA = false;
                    break;
                case "JEF.VTA.LIM.RVI"://JefeVentaLima 
                case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
                case "AST.RVI.COM"://AsistenteComercial
                case "GTE.DIV.RVI"://GerenteDivision 
                case "JEF.RVI.OPE"://JefeOperaciones
                case "AST.RVI.OPE"://AsistenteOperaciones
                case "ANL.RVI": //AnalistaOperaciones
                    mostrarTRA = true;
                    break;

            };

            bool mostrarInd = false;

            switch (codRol)
            {
                case "AGT.LIM.RVI"://AgenteLima
                case "AGT.PRO.RVI"://AgenteProvincia
                case "SPV.LIM.RVI"://SupervisorLima
                case "SPV.PRO.RVI"://SupervisorProvincia
                    mostrarInd = false;
                    break;
                case "JEF.VTA.LIM.RVI"://JefeVentaLima //<INIGTI_6842>
                case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
                case "AST.RVI.COM"://AsistenteComercial
                case "GTE.DIV.RVI"://GerenteDivision //<FINGTI_6842>
                case "JEF.RVI.OPE"://JefeOperaciones
                case "AST.RVI.OPE"://AsistenteOperaciones
                case "ANL.RVI": //AnalistaOperaciones
                    mostrarInd = true;
                    break;
            };

            bool mostrarTRA_Esperado = false;

            switch (codRol)
            {
                case "AGT.LIM.RVI"://AgenteLima
                case "AGT.PRO.RVI"://AgenteProvincia
                case "SPV.LIM.RVI"://SupervisorLima
                case "SPV.PRO.RVI"://SupervisorProvincia
                    mostrarTRA_Esperado = false;
                    break;
                case "JEF.VTA.LIM.RVI"://JefeVentaLima
                case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
                case "AST.RVI.COM"://AsistenteComercial
                case "GTE.DIV.RVI"://GerenteDivision
                case "JEF.RVI.OPE"://JefeOperaciones
                case "AST.RVI.OPE"://AsistenteOperaciones
                case "ANL.RVI": //AnalistaOperaciones
                    mostrarTRA_Esperado = true;
                    break;
            };

            switch (codRol)
            {
                case "AGT.LIM.RVI"://AgenteLima
                case "AGT.PRO.RVI"://AgenteProvincia
                case "SPV.LIM.RVI"://SupervisorLima
                case "SPV.PRO.RVI"://SupervisorProvincia
                    mostrarTraMaxMin = false;
                    break;
                case "JEF.VTA.LIM.RVI"://JefeVentaLima
                case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
                case "AST.RVI.COM"://AsistenteComercial
                case "GTE.DIV.RVI"://GerenteDivision
                case "JEF.RVI.OPE"://JefeOperaciones
                case "AST.RVI.OPE"://AsistenteOperaciones
                case "ANL.RVI": //AnalistaOperaciones
                    mostrarTraMaxMin = true;
                    break;
                default:
                    mostrarTraMaxMin = false;
                    break;
            };

            //<GTI.INI-29372>
            bool mostrarEnvioObligatorio = false;
            switch (codRol)
            {
                case "JEF.RVI.OPE"://JefeOperaciones
                case "AST.RVI.OPE"://AsistenteOperaciones
                case "ANL.RVI": //AnalistaOperaciones
                    mostrarEnvioObligatorio = true;
                    break;
                default:
                    mostrarEnvioObligatorio = false;
                    break;
            };
            //<GTI.FIN-29372>

            if (Cotizaciones.Count > 0)
            {
                Cotizaciones.ForEach(
                    p =>{
                        p.Moneda.Nombre = "&nbsp;&nbsp;" + p.Moneda.Nombre;
                        p.Modalidad.Id = "&nbsp;" + p.Modalidad.Id;
                        }
                    );
            }

            TabCotizaciones.DataSource = Cotizaciones;
            TabCotizaciones.Columns[0].Visible = PermisoRadio;
            TabCotizaciones.DataBind();
            if (Cotizaciones.Count > 0)
            {
                TabCotizaciones.HeaderRow.TableSection = TableRowSection.TableHeader;
            }


            TabCotizaciones.Columns[9].Visible = MostrarPorcentajeCapital;
            
            TabCotizaciones.Columns[22].Visible = RedLocal ? PermisoTRA : false;//Dif. TRA

            TabCotizaciones.Columns[17].Visible = RedLocal ? PermisoTRA : false;//Pensión Esper.

            TabCotizaciones.Columns[13].Visible = RedLocal ? PermisoTRA : false;//Tas SBS Esper.
            TabCotizaciones.Columns[23].Visible = RedLocal ? PermisoTRA : false;//PBS

            TabCotizaciones.Columns[19].Visible = mostrarTRA;//TRA
            TabCotizaciones.Columns[21].Visible = mostrarInd;//Ind.
            TabCotizaciones.Columns[20].Visible = mostrarTRA_Esperado;//TRA Esper.

            if (codRol == Enums.RolAzman.JefeVentaLima.StringValue() ||
                codRol == Enums.RolAzman.JefeVentaProvincia.StringValue() ||
                codRol == Enums.RolAzman.AnalistaOperaciones.StringValue() ||
                codRol == Enums.RolAzman.AsistenteOperaciones.StringValue())
            {
                TabCotizaciones.Columns[14].Visible = false;//Tas Vta Max
            }
            else {
                TabCotizaciones.Columns[14].Visible = mostrarTraMaxMin;//Tas Vta Max
            }

            TabCotizaciones.Columns[18].Visible = mostrarTraMaxMin;//TRA Min.
            TabCotizaciones.Columns[24].Visible = mostrarEnvioObligatorio;//Envío obligatorio

            if (Modo == "N")//En caso se Nuevo Se Oculta
            {
                TabCotizaciones.Columns[14].Visible = false;//Tas Vta Max
                TabCotizaciones.Columns[19].Visible = false;//TRA
                TabCotizaciones.Columns[18].Visible = false;//TRA Min.
                TabCotizaciones.Columns[21].Visible = false;//Ind.

                TabCotizaciones.Columns[17].Visible = false;//Pensión Esper.
                TabCotizaciones.Columns[13].Visible = false;//Tas SBS Esper.
                TabCotizaciones.Columns[20].Visible = false;//TRA Esper.
                TabCotizaciones.Columns[23].Visible = false;//PBS

                TabCotizaciones.Columns[24].Visible = false;//Envío obligatorio
            }
        }

        protected void TabCotizaciones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                TextBox TabCotTRA;
                TabCotTRA = (TextBox)(fila.FindControl("TabCotTRA"));
                TabCotTRA.Text = Cotizaciones[e.Row.RowIndex].AjusteTRA.ToString();

                TabCotTRA.Enabled = PermisoTRA;

                if (MostrarTasas)
                {
                    if (Cotizaciones[e.Row.RowIndex].IndCotiza == "**" || Cotizaciones[e.Row.RowIndex].IndCotiza == "***")
                    {
                        e.Row.CssClass = "grilla_error_tra";
                    }
                }
                if (mostrarTraMaxMin)
                {
                    if (Cotizaciones[e.Row.RowIndex].TasaRetornoAccionistaObjetivo < Cotizaciones[e.Row.RowIndex].TasaRetornoAccionistaMinimo)
                    {
                        e.Row.CssClass = "grilla_error_tra";
                    }
                }
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox, bool seleccione)
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<Producto> combobox, bool seleccione)
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Producto item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }
    }
}