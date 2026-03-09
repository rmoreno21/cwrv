using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
//using System.ServiceModel;
using Microsoft.Reporting.WebForms;
using log4net;
using System.IO;
using System.Globalization;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Net;

using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;


namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class GestionVentas : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GestionVentas));
        private static IServicioCWRV servicioCotizador;
        private static IServicioAzman servicioAzman;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteGestionVentas))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteGestionVentas.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        private void CargarInformacionInicialPantalla()
        {
            switch ((string)Session["RolAzman"])
            {
                case "AST.RVI.COM":
                case "GTE.DIV.RVI":
                case "JEF.RVI.OPE":
                    ControlJefe.Visible = true;
                    CargarCombobox(Jefe, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.IdNivel == 1), true);
                    Jefe.Enabled = true;

                    ControlSupervisor.Visible = true;
                    CargarCombobox(Supervisor, new List<Agente>(), true);
                    Supervisor.Enabled = false;

                    ControlAgente.Visible = true;
                    CargarCombobox(Agente, new List<Agente>(), true);
                    Agente.Enabled = false;

                    //Habilitando el boton enviarCorreo
                    switch ((string)Session["RolAzman"])
                    {
                        case "AST.RVI.COM":
                            EnviarCorreoGestionVentas.Visible = true;
                            break;
                    }

                    break;
                case "JEF.VTA.LIM.RVI":
                case "JEF.VTA.PRO.RVI":
                    ControlJefe.Visible = true;
                    CargarCombobox(Jefe, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    Jefe.Enabled = false;

                    ControlSupervisor.Visible = true;

                    // Lista de supervisores
                    List<Agente> listaSupervisores = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 2 && a.IdPadre == Jefe.SelectedValue);

                    // Eliminar los duplicados
                    listaSupervisores =
                        listaSupervisores
                            .GroupBy(s => s.Id)
                            .Select(s => s.First())
                            .ToList();

                    CargarCombobox(Supervisor, listaSupervisores, true);
                    Supervisor.Enabled = true;

                    ControlAgente.Visible = true;
                    CargarCombobox(Agente, new List<Agente>(), true);
                    Agente.Enabled = false;
                    break;
                case "SPV.LIM.RVI":
                case "SPV.PRO.RVI":
                    ControlJefe.Visible = false;

                    ControlSupervisor.Visible = true;
                    CargarCombobox(Supervisor, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    Supervisor.Enabled = false;

                    ControlAgente.Visible = true;

                    // Lista de agentes
                    List<Agente> listaAgentes = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 3 && a.IdPadre == Supervisor.SelectedValue);

                    // Eliminar los duplicados
                    listaAgentes =
                        listaAgentes
                            .GroupBy(a => a.Id)
                            .Select(a => a.First())
                            .ToList();

                    CargarCombobox(Agente, listaAgentes, true);
                    Agente.CssClass = "formCombobox";
                    Agente.Enabled = true;
                    break;
                case "AGT.LIM.RVI":
                case "AGT.PRO.RVI":
                    ControlJefe.Visible = false;

                    ControlSupervisor.Visible = false;

                    ControlAgente.Visible = true;
                    CargarCombobox(Agente, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    Agente.Enabled = false;
                    break;
                    //ControlCUSPP.Visible = false;

            }

            CotizacionCerrada.Items.Add(new ListItem("Sí", "S"));
            CotizacionCerrada.Items.Add(new ListItem("No", "N"));

            TipoCotizacion.Items.Add(new ListItem("Oficiales", "O"));
            //TipoCotizacion.Items.Add(new ListItem("Extraoficiales", "E"));

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
            CargarCombobox(CiaSeguro, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Compania]);


            FechaDesde.Text = "01/" + DateTime.Today.Month.ToString("00") + "/" + DateTime.Today.Year.ToString("00");
            DateTime FecFinal = Convert.ToDateTime(FechaDesde.Text, new CultureInfo("es-PE"));
            FecFinal = FecFinal.AddMonths(1).AddDays(-1);
            FechaHasta.Text = FecFinal.ToString("dd/MM/yyyy");   //"01/" + DateTime.Today.Month.ToString("00") + "/" + DateTime.Today.Year.ToString("00");

        }

        private void CargarCombobox(DropDownList control, List<Agente> combobox, bool todos)
        {
            control.Items.Clear();
            if (todos)
            {
                control.Items.Add(new ListItem("«Todos»", "0"));
            }
            foreach (Agente item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void LimpiarFormularios()
        {
            Jefe.SelectedIndex = Jefe.Items.IndexOf(Jefe.Items.FindByValue("0"));
            HJefe.Value = "0";
            Supervisor.SelectedIndex = Supervisor.Items.IndexOf(Supervisor.Items.FindByValue("0"));
            HSupervisor.Value = "0";
            Agente.SelectedIndex = Agente.Items.IndexOf(Agente.Items.FindByValue("0"));
            HAgente.Value = "0";
            //FechaDesde.Text = String.Empty;
            //FechaHasta.Text = String.Empty;
            CotizacionCerrada.SelectedIndex = CotizacionCerrada.Items.IndexOf(CotizacionCerrada.Items.FindByValue("0"));
            HCotizacionCerrada.Value = "0";
            TipoCotizacion.SelectedIndex = TipoCotizacion.Items.IndexOf(TipoCotizacion.Items.FindByValue("0"));
            HTipoCotizacion.Value = "0";
            CiaSeguro.SelectedIndex = CiaSeguro.Items.IndexOf(CiaSeguro.Items.FindByValue("0"));
            HCiaSeguro.Value = "0";
        }


        [WebMethod]
        public static string CargarTablaGestionventas(string tokenUsuario, string fechaInicial, string fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        tipoCotizacion = (tipoCotizacion == "0") ? "" : tipoCotizacion;
                        codCiaSeguro = (codCiaSeguro == "0") ? "" : codCiaSeguro;

                        DateTime fecha_inicial = Convert.ToDateTime(fechaInicial, new CultureInfo("es-PE"));
                        DateTime fecha_final = Convert.ToDateTime(fechaFinal, new CultureInfo("es-PE"));

                        if (fecha_inicial > fecha_final)
                        {
                            throw new Exception("La fecha inicial no puede ser mayor que la fecha final.");
                        }

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<Dominio.Entidades.GestionVentas> lstGestionVentas = servicioCotizador.ConsultarGestionVentas(fecha_inicial, fecha_final, numJefe, numSuperv, numAgente, indCierre, tipoCotizacion, codCiaSeguro);

                        var pagina = new Page();
                        var control = (TablaGestionVentas)pagina.LoadControl("~/Controles/TablaGestionVentas.ascx");
                        control.lstGestionVentas = lstGestionVentas;
                        pagina.Controls.Add(control);

                        string html = "";
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }
                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    //return ex.Message.ToString();
                    throw (ex);
                }

            }
        }

        [WebMethod]
        public static Respuesta GenerarExcelGestionVentas(string tokenUsuario, string fechaInicial, string fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro, bool correo, List<Dominio.Entidades.GestionVentas> lstGVentas)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<Dominio.Entidades.GestionVentas> lstGestionVentas = new List<Dominio.Entidades.GestionVentas>();

                        tipoCotizacion = (tipoCotizacion == "0") ? "" : tipoCotizacion;
                        codCiaSeguro = (codCiaSeguro == "0") ? "" : codCiaSeguro;

                        DateTime fecha_inicial = Convert.ToDateTime(fechaInicial, new CultureInfo("es-PE"));
                        DateTime fecha_final = Convert.ToDateTime(fechaFinal, new CultureInfo("es-PE"));


                        if (fecha_inicial > fecha_final)
                        {
                            throw new Exception("La fecha inicial no puede ser mayor que la fecha final.");
                        }

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        if (correo)
                        {
                            lstGestionVentas = lstGVentas;
                        }
                        else
                        {
                            lstGestionVentas = servicioCotizador.ConsultarGestionVentas(fecha_inicial, fecha_final, numJefe, numSuperv, numAgente, indCierre, tipoCotizacion, codCiaSeguro);
                        }

                        if (lstGestionVentas.Count > 0)
                        {
                            // Generar archivo Excel
                            HSSFWorkbook wb;
                            HSSFSheet sh;

                            wb = new HSSFWorkbook();
                            sh = (HSSFSheet)wb.CreateSheet("Gestión de Ventas");

                            ICellStyle titulodato = wb.CreateCellStyle();
                            titulodato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulodato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulodato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulodato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulodato.Alignment = HorizontalAlignment.Center;
                            titulodato.VerticalAlignment = VerticalAlignment.Center;
                            titulodato.FillForegroundColor = IndexedColors.BlueGrey.Index;// .LightTurquoise.Index;
                            titulodato.FillPattern = FillPattern.SolidForeground;

                            IFont cabecera_font = wb.CreateFont();
                            cabecera_font.Color = HSSFColor.White.Index;
                            cabecera_font.Boldweight = (short)FontBoldWeight.Bold;
                            //cabecera_font.FontName = ("Calibri");
                            //cabecera_font.FontHeightInPoints = 10;
                            titulodato.SetFont(cabecera_font);

                            titulodato.WrapText = true;

                            ICellStyle detalleTexto = wb.CreateCellStyle();
                            detalleTexto.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleTexto.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleTexto.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleTexto.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleTexto.Alignment = HorizontalAlignment.Left;
                            detalleTexto.VerticalAlignment = VerticalAlignment.Center;

                            ICellStyle detalleNumero = wb.CreateCellStyle();
                            detalleNumero.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumero.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumero.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumero.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumero.Alignment = HorizontalAlignment.Right;
                            detalleNumero.VerticalAlignment = VerticalAlignment.Center;
                            detalleNumero.DataFormat = wb.CreateDataFormat().GetFormat("#,##0.00");

                            ICellStyle detalleFecha = wb.CreateCellStyle();
                            detalleFecha.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleFecha.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleFecha.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleFecha.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleFecha.Alignment = HorizontalAlignment.Left;
                            detalleFecha.VerticalAlignment = VerticalAlignment.Center;
                            detalleFecha.DataFormat = wb.CreateDataFormat().GetFormat("dd/MM/yyyy");

                            ICellStyle detalleNumeroEntero = wb.CreateCellStyle();
                            detalleNumeroEntero.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumeroEntero.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumeroEntero.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumeroEntero.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            detalleNumeroEntero.Alignment = HorizontalAlignment.Right;
                            detalleNumeroEntero.VerticalAlignment = VerticalAlignment.Center;
                            //detalleNumeroEntero.DataFormat = wb.CreateDataFormat().GetFormat("#,##0.00");

                            // Ancho de columnas
                            sh.SetColumnWidth(0, 2600);
                            sh.SetColumnWidth(1, 3000);
                            sh.SetColumnWidth(2, 4100);
                            sh.SetColumnWidth(3, 4500);
                            sh.SetColumnWidth(4, 9000);
                            sh.SetColumnWidth(5, 15500);
                            sh.SetColumnWidth(6, 3300);
                            sh.SetColumnWidth(7, 3000);
                            sh.SetColumnWidth(8, 2800);
                            sh.SetColumnWidth(9, 2900);
                            sh.SetColumnWidth(10, 2000);
                            sh.SetColumnWidth(11, 2000);
                            sh.SetColumnWidth(12, 2000);
                            sh.SetColumnWidth(13, 3800);
                            sh.SetColumnWidth(14, 3500);
                            sh.SetColumnWidth(15, 2800);
                            sh.SetColumnWidth(16, 2000);
                            sh.SetColumnWidth(17, 2000);
                            sh.SetColumnWidth(18, 2000);
                            sh.SetColumnWidth(19, 2000);
                            sh.SetColumnWidth(20, 2000);
                            sh.SetColumnWidth(21, 3000);
                            sh.SetColumnWidth(22, 14000);
                            sh.SetColumnWidth(23, 2000);

                            sh.SetColumnWidth(24, 8000);
                            sh.SetColumnWidth(25, 5000);
                            sh.SetColumnWidth(26, 5000);
                            sh.SetColumnWidth(27, 8000);

                            //<INIGTI_4081_3>
                            sh.SetColumnWidth(28, 8000);
                            //<FINGTI_4081_3>


                            int fila = 0;
                            IRow r;
                            r = sh.CreateRow(fila);
                            r.Height = 800;

                            r.CreateCell(0);
                            r.CreateCell(1);
                            r.CreateCell(2);
                            r.CreateCell(3);
                            r.CreateCell(4);
                            r.CreateCell(5);
                            r.CreateCell(6);
                            r.CreateCell(7);
                            r.CreateCell(8);
                            r.CreateCell(9);
                            r.CreateCell(10);
                            r.CreateCell(11);
                            r.CreateCell(12);
                            r.CreateCell(13);
                            r.CreateCell(14);
                            r.CreateCell(15);
                            r.CreateCell(16);
                            r.CreateCell(17);
                            r.CreateCell(18);
                            r.CreateCell(19);
                            r.CreateCell(20);
                            r.CreateCell(21);
                            r.CreateCell(22);
                            r.CreateCell(23);
                            r.CreateCell(24);
                            r.CreateCell(25);
                            r.CreateCell(26);
                            r.CreateCell(27);
                            //<INIGTI_4081_3>
                            r.CreateCell(28);
                            //<FINGTI_4081_3>

                            sh.GetRow(fila).GetCell(0).SetCellValue("Nro. Meler");
                            sh.GetRow(fila).GetCell(1).SetCellValue("Fecha de Plazo AFP");
                            sh.GetRow(fila).GetCell(2).SetCellValue("AFP");

                            sh.GetRow(fila).GetCell(3).SetCellValue("CUSPP");

                            sh.GetRow(fila).GetCell(4).SetCellValue("Nombre Afiliado");
                            sh.GetRow(fila).GetCell(5).SetCellValue("Categoría");
                            sh.GetRow(fila).GetCell(6).SetCellValue("Monto CIC");
                            sh.GetRow(fila).GetCell(7).SetCellValue("Fecha de Cierre Comercial");
                            sh.GetRow(fila).GetCell(8).SetCellValue("Re-cotización");

                            sh.GetRow(fila).GetCell(9).SetCellValue("Modalidad");

                            sh.GetRow(fila).GetCell(10).SetCellValue("P. Dif");
                            sh.GetRow(fila).GetCell(11).SetCellValue("Pje. Rent.");
                            sh.GetRow(fila).GetCell(12).SetCellValue("P. Gar.");
                            sh.GetRow(fila).GetCell(13).SetCellValue("CIA Ganadora");

                            sh.GetRow(fila).GetCell(14).SetCellValue("Nro. Cotización Ganadora");

                            sh.GetRow(fila).GetCell(15).SetCellValue("Moneda Ganadora");
                            sh.GetRow(fila).GetCell(16).SetCellValue("A");
                            sh.GetRow(fila).GetCell(17).SetCellValue("D");
                            sh.GetRow(fila).GetCell(18).SetCellValue("Solicitud Especial");
                            sh.GetRow(fila).GetCell(19).SetCellValue("Tasa IS Meler");
                            sh.GetRow(fila).GetCell(20).SetCellValue("Tasa CIA Ganadora");

                            sh.GetRow(fila).GetCell(21).SetCellValue("Fecha Cita");
                            sh.GetRow(fila).GetCell(22).SetCellValue("Lugar de Cita");

                            sh.GetRow(fila).GetCell(23).SetCellValue("Nro. Agente");
                            sh.GetRow(fila).GetCell(24).SetCellValue("Agente");

                            sh.GetRow(fila).GetCell(25).SetCellValue("Ubicación Agente");
                            sh.GetRow(fila).GetCell(26).SetCellValue("Agencia");

                            sh.GetRow(fila).GetCell(27).SetCellValue("Supervisor");

                            //<INIGTI_4081_3>
                            sh.GetRow(fila).GetCell(28).SetCellValue("Jefe");
                            //<FINGTI_4081_3>

                            sh.GetRow(fila).GetCell(0).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(1).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(2).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(7).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(8).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(9).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(10).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(11).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(12).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(13).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(14).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(15).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(16).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(17).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(18).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(19).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(20).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(21).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(22).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(23).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(24).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(25).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(26).CellStyle = titulodato;
                            sh.GetRow(fila).GetCell(27).CellStyle = titulodato;
                            //<INIGTI_4081_3>
                            sh.GetRow(fila).GetCell(28).CellStyle = titulodato;
                            //<FINGTI_4081_3>

                            for (int i = 0; i < lstGestionVentas.Count; i++)
                            {
                                Dominio.Entidades.GestionVentas gstVentas = lstGestionVentas[i];
                                fila = i + 1;
                                r = sh.CreateRow(fila);
                                r.CreateCell(0);
                                r.CreateCell(1);
                                r.CreateCell(2);
                                r.CreateCell(3);
                                r.CreateCell(4);
                                r.CreateCell(5);
                                r.CreateCell(6);
                                r.CreateCell(7);
                                r.CreateCell(8);
                                r.CreateCell(9);
                                r.CreateCell(10);
                                r.CreateCell(11);
                                r.CreateCell(12);
                                r.CreateCell(13);
                                r.CreateCell(14);
                                r.CreateCell(15);
                                r.CreateCell(16);
                                r.CreateCell(17);
                                r.CreateCell(18);
                                r.CreateCell(19);
                                r.CreateCell(20);
                                r.CreateCell(21);
                                r.CreateCell(22);
                                r.CreateCell(23);
                                r.CreateCell(24);
                                r.CreateCell(25);
                                r.CreateCell(26);
                                r.CreateCell(27);
                                //<INIGTI_4081_3>
                                r.CreateCell(28);
                                //<FINGTI_4081_3>

                                sh.GetRow(fila).GetCell(0).SetCellValue(gstVentas.NumeroMeler);
                                sh.GetRow(fila).GetCell(0).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(1).SetCellValue(gstVentas.FechaPlazoAFP);
                                sh.GetRow(fila).GetCell(1).CellStyle = detalleFecha;

                                sh.GetRow(fila).GetCell(2).SetCellValue(gstVentas.AFP.Nombre);
                                sh.GetRow(fila).GetCell(2).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(3).SetCellValue(gstVentas.CUSPP);
                                sh.GetRow(fila).GetCell(3).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(4).SetCellValue(gstVentas.NombreCliente);
                                sh.GetRow(fila).GetCell(4).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(5).SetCellValue(gstVentas.Categoria.Nombre);
                                sh.GetRow(fila).GetCell(5).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(6).SetCellValue(gstVentas.CIC);
                                sh.GetRow(fila).GetCell(6).CellStyle = detalleNumero;

                                sh.GetRow(fila).GetCell(7).SetCellValue((gstVentas.FechaCierre != null) ? gstVentas.FechaCierre.Value.ToString("dd/MM/yyyy") : string.Empty);
                                sh.GetRow(fila).GetCell(7).CellStyle = detalleFecha;

                                sh.GetRow(fila).GetCell(8).SetCellValue(gstVentas.Recotizacion);
                                sh.GetRow(fila).GetCell(8).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(9).SetCellValue(gstVentas.Modalidad.Nombre);
                                sh.GetRow(fila).GetCell(9).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(10).SetCellValue(gstVentas.PeriodoDiferido);
                                sh.GetRow(fila).GetCell(10).CellStyle = detalleNumeroEntero;

                                sh.GetRow(fila).GetCell(11).SetCellValue(gstVentas.PorcentajeRenta);
                                sh.GetRow(fila).GetCell(11).CellStyle = detalleNumeroEntero;

                                sh.GetRow(fila).GetCell(12).SetCellValue(gstVentas.PeriodoGarantizado);
                                sh.GetRow(fila).GetCell(12).CellStyle = detalleNumeroEntero;

                                sh.GetRow(fila).GetCell(13).SetCellValue(gstVentas.CompaniaGanadora);
                                sh.GetRow(fila).GetCell(13).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(14).SetCellValue(gstVentas.NumeroCotizacion);
                                sh.GetRow(fila).GetCell(14).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(15).SetCellValue(gstVentas.Moneda.Nombre);
                                sh.GetRow(fila).GetCell(15).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(16).SetCellValue(gstVentas.ACOM);
                                sh.GetRow(fila).GetCell(16).CellStyle = detalleNumero;

                                sh.GetRow(fila).GetCell(17).SetCellValue(gstVentas.DCOM);
                                sh.GetRow(fila).GetCell(17).CellStyle = detalleNumero;

                                sh.GetRow(fila).GetCell(18).SetCellValue(gstVentas.DifTra);
                                sh.GetRow(fila).GetCell(18).CellStyle = detalleNumero;

                                sh.GetRow(fila).GetCell(19).SetCellValue(gstVentas.TasaIS);
                                sh.GetRow(fila).GetCell(19).CellStyle = detalleNumero;

                                sh.GetRow(fila).GetCell(20).SetCellValue(gstVentas.TasaCiaGanadora);
                                sh.GetRow(fila).GetCell(20).CellStyle = detalleNumero;

                                sh.GetRow(fila).GetCell(21).SetCellValue(gstVentas.FechaCita);
                                sh.GetRow(fila).GetCell(21).CellStyle = detalleFecha;

                                sh.GetRow(fila).GetCell(22).SetCellValue(gstVentas.LugarCita);
                                sh.GetRow(fila).GetCell(22).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(23).SetCellValue(gstVentas.Agente.Id);
                                sh.GetRow(fila).GetCell(23).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(24).SetCellValue(gstVentas.Agente.Nombre);
                                sh.GetRow(fila).GetCell(24).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(25).SetCellValue(gstVentas.UbigeoAgente);
                                sh.GetRow(fila).GetCell(25).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(26).SetCellValue(gstVentas.NombreAgencia);
                                sh.GetRow(fila).GetCell(26).CellStyle = detalleTexto;

                                sh.GetRow(fila).GetCell(27).SetCellValue(gstVentas.Supervisor);
                                sh.GetRow(fila).GetCell(27).CellStyle = detalleTexto;

                                //<INIGTI_4081_3>
                                sh.GetRow(fila).GetCell(28).SetCellValue(gstVentas.Jefe);
                                sh.GetRow(fila).GetCell(28).CellStyle = detalleTexto;
                                //<FINGTI_4081_3>
                            }

                            using (MemoryStream ms = new MemoryStream())
                            {
                                wb.Write(ms);
                                byte[] excelBytes = ms.ToArray();

                                if (correo)
                                {
                                    HttpContext.Current.Session["ExcelGestionVentasBytes" + lstGestionVentas[0].Num_Supervisor.ToString()] = excelBytes;
                                    respuesta.Contenido = "ReporteGestionVentas" + lstGestionVentas[0].Num_Supervisor.ToString();
                                }
                                else
                                {
                                    HttpContext.Current.Session["ExcelGestionVentasBytes"] = excelBytes;
                                }
                            }

                            respuesta.Estado = Constante.COD_OK;
                        }


                        string nombreTerminal = String.Empty;
                        try
                        {
                            nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                        }
                        catch (Exception)
                        {
                            log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                HttpContext.Current.Request.ServerVariables["remote_addr"]));
                        }

                        nombreTerminal += HttpContext.Current.Request.UserAgent;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        servicioCotizador.RegistrarLog(new LogBD
                        {
                            IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                            NombreTerminal = nombreTerminal,
                            IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                            NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                            IdTipoEvento = Enums.EventoLog.Reporte1.StringValue(),
                            //Detalle = String.Format("Cotización {0} simulada", correlativo)
                        });
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }
                catch (FaultException ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message.ToString() });
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta EnviarExcelCorreoGestionVentas(string tokenUsuario, string fechaInicial, string fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro, bool correo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<Dominio.Entidades.GestionVentas> lstGestionVentas = new List<Dominio.Entidades.GestionVentas>();

                        tipoCotizacion = (tipoCotizacion == "0") ? "" : tipoCotizacion;
                        codCiaSeguro = (codCiaSeguro == "0") ? "" : codCiaSeguro;

                        DateTime fecha_inicial = Convert.ToDateTime(fechaInicial, new CultureInfo("es-PE"));
                        DateTime fecha_final = Convert.ToDateTime(fechaFinal, new CultureInfo("es-PE"));

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        lstGestionVentas = servicioCotizador.ConsultarGestionVentas(fecha_inicial, fecha_final, numJefe, numSuperv, numAgente, indCierre, tipoCotizacion, codCiaSeguro);

                        //Seleccionamos los supervisores
                        List<int> lstSupervisorVentas = lstGestionVentas.Select(x => x.Num_Supervisor).Distinct().ToList();

                        List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];

                        for (int i = 0; i < lstSupervisorVentas.Count(); i++)
                        {
                            //Seleccionamos las Gestion de Ventas por supervisor
                            List<Dominio.Entidades.GestionVentas> lstGestionVtasSupervisor =
                                lstGestionVentas.FindAll(x => x.Num_Supervisor == lstSupervisorVentas[i]);

                            Respuesta rsptaExcel = new Respuesta();

                            //Obtenemos la respuestade la creacion del Excel y nombre del archivo en rsptaExcel.Contenido
                            rsptaExcel = GenerarExcelGestionVentas(tokenUsuario, fechaInicial, fechaFinal, numJefe, numSuperv, numAgente, indCierre, tipoCotizacion, codCiaSeguro, true, lstGestionVtasSupervisor);

                            if (rsptaExcel.Estado == Constante.COD_OK)
                            {
                                //Obtenermos el Usuario de los Supervisores
                                List<Usuario> listaUsuariosCorreo = new List<Usuario>();
                                Agente agente = new Agente();
                                agente = listaAgentes.Find(x => x.Id == lstSupervisorVentas[i].ToString());

                                if (agente != null)
                                {
                                    listaUsuariosCorreo.Add(new Usuario { NombreUsuario = agente.Usuario });
                                }
                                //.ForEach(p =>
                                //                listaUsuariosCorreo.Add(new Usuario { NombreUsuario=p.Usuario})
                                //            );

                                //listaUsuariosCorreo = servicioCotizador.ListarUsuario(String.Empty, lstSupervisorVentas[i].ToString());

                                //Recuperamos el Excel de la Session
                                byte[] excelBytes = (byte[])HttpContext.Current.Session["ExcelGestionVentasBytes" + lstSupervisorVentas[i].ToString()];
                                if (excelBytes == null || excelBytes.Length == 0)
                                {
                                    log.Error($"No se encontraron datos Excel en la sesión para el supervisor {lstSupervisorVentas[i]}");
                                    continue; // Pasar al siguiente supervisor
                                }

                                //Recorremos la lista de Usuario
                                CorreoElectronico Correo = null;
                                foreach (Usuario usuarioCorreo in listaUsuariosCorreo)
                                {
                                    try
                                    {
                                        //Obtenermos los datos del Correo del Usuario
                                        servicioAzman = LocalizadorProxy.ObtenerServicioSeguridad();
                                        BEUsuario datosUsuario =
                                            servicioAzman.ObtenerDatosUsuarioSinClave(
                                                ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                ConfigurationManager.AppSettings["DominioRed"],
                                                usuarioCorreo.NombreUsuario);

                                        //Configuramos el Correo
                                        SeccionCorreo config = (SeccionCorreo)ConfigurationManager.GetSection("correo");

                                        Correo = new CorreoElectronico
                                        {
                                            De = "no-responder@interseguro.com.pe",
                                            DeNombre = "Interseguro - Cotizador Web de Rentas ",
                                            Para = datosUsuario.Correo,
                                            ParaNombre = datosUsuario.NombreCompleto,
                                            Asunto = config.AsuntoGestionVentas.Texto
                                                        .Replace("{FechaDesde}", fechaInicial)
                                                        .Replace("{FechaHasta}", fechaFinal),
                                            Mensaje = config.MensajeGestionVentas.Texto
                                                            .Replace("{Nombre}", datosUsuario.Nombres + " " + datosUsuario.Apellidos),
                                            Html = true,
                                            // BinarioAdjunto = (byte[])buffer,
                                            BinarioAdjunto = excelBytes,
                                            Adjunto = rsptaExcel.Contenido + ".xls"
                                        };
                                        servicioCotizador.EnviarCorreoElectronicoAsincrono(Correo);

                                        respuesta.Estado = Constante.COD_OK;
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(String.Format("Error al enviar el correo de notificación. Usuario[{0}]", usuarioCorreo.NombreUsuario), ex);
                                    }
                                }
                                HttpContext.Current.Session.Remove("ExcelGestionVentasBytes" + lstSupervisorVentas[i].ToString());
                            }
                        }

                        respuesta.Estado = Constante.COD_OK;

                        string nombreTerminal = String.Empty;
                        try
                        {
                            nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                        }
                        catch (Exception)
                        {
                            log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                HttpContext.Current.Request.ServerVariables["remote_addr"]));
                        }

                        nombreTerminal += HttpContext.Current.Request.UserAgent;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        servicioCotizador.RegistrarLog(new LogBD
                        {
                            IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                            NombreTerminal = nombreTerminal,
                            IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                            NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                            IdTipoEvento = Enums.EventoLog.Reporte1.StringValue(),
                            //Detalle = String.Format("Cotización {0} simulada", correlativo)
                        });
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }
                catch (FaultException ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message.ToString() });
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

    }
}
