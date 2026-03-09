using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Interseguro.CWRV.Infraestructura.General;
using System.Threading;
using System.ServiceModel;
using System.Configuration;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Dominio.Entidades;
using System.Web.Services;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Net;
using Microsoft.Reporting.WebForms;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;


namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class CompararCotizacionPlus : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CompararCotizacionPlus));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    HCompara.Value = "TRUE";
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteCotizacionPlus))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));


                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();
                            CargarInformacionPredeterminada();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteCotizacionPlus.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }


        private void CargarInformacionPredeterminada()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> parametros = servicioCotizador.ObtenerParametrosSimuladores();

            //divMaximo.Visible = false;
            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
            {
                BusAfiCUSPP.Text = Session["CUSPP"].ToString();
                HBusAfiCUSPP.Value = Session["CUSPP"].ToString();
                RepCotPlusBusAfiDatosCargados.Value = "1";
                //divMaximo.Visible = true;
            }
            else if (Session["CUSPP"] == null && Session["NroSolicitud"] != null)
            {
                BusAfiNroSolicitud.Text = Session["NroSolicitud"].ToString();
                HBusAfiNroSolicitud.Value = Session["NroSolicitud"].ToString();
                RepCotPlusBusAfiDatosCargados.Value = "1";
                //divMaximo.Visible = true;
            }

        }

        private void LimpiarFormularios()
        {
            CUSPP.Value = String.Empty;
            
        }

        private void CargarInformacionInicialPantalla()
        {
            
            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

            for (int i = 5; i <= 25; i++)
			{
                ModSolMaximo_RP.Items.Add(new ListItem { Value = i.ToString(), Text = i.ToString() });
			}
            
            // Permisos Modal Búsqueda de Afiliados
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.BusquedaAfiliadoConsultar))
            {
                PerBusAfiExaminarSolicitud.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiExaminarSolicitud);
                InhabilitarControl(ModBusAfiApellidoPaterno);
                InhabilitarControl(ModBusAfiApellidoMaterno);
                InhabilitarControl(ModBusAfiNombres);
                InhabilitarControl(ModBusAfiBuscar);
                PerBusAfiExaminarSolicitud.Value = "0";
            }

            // Permisos Consultar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
            {
                PerBusAfiBuscar.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiNroSolicitud);
                InhabilitarControl(BusAfiCUSPP);
                InhabilitarControl(RepCotPlusBusAfiBuscar);
                PerBusAfiBuscar.Value = "0";
            }
        }

        public void InhabilitarControl(Control control)
        {
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly";
            }
            if (control is DropDownList)
            {
                ((DropDownList)control).Enabled = false;
                ((DropDownList)control).CssClass = "formCombobox formComboboxReadOnly";
            }
            if (control is HyperLink)
            {
                ((HyperLink)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((HyperLink)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
            if (control is Button)
            {
                ((Button)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((Button)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
        }


        [WebMethod]
        public static string CargarTablaSolicitudes(string tokenUsuario, string cuspp)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesReportePlus)pagina.LoadControl("~/Controles/TablaSolicitudesReportePlus.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudRPPlus> solicitudes = servicioCotizador.ListarReporteCotizacionPlus(cuspp);

                            control.Solicitudes = solicitudes;

                            control.PermisoConsultar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar)) ? true : false;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusActualizar)) ? true : false;
                            control.PermisoCorreoElectronico = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusEnviarCorreo)) ? true : false;
                            control.PermisoExportarPDF = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusExportarPDF)) ? true : false;
                            
                            control.PermisoReporteEscenario = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios)) ? true : false; ;
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            
                        }
                        else
                        {
                            control.PermisoConsultar = false;
                        }

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
                    throw (ex);
                }
            }
        }


        [WebMethod]
        public static string GuardandoCheck(string[] solicitudes, string[] cotizaciones)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    HttpContext.Current.Session["SolicitudesReportePlusCheck"] = solicitudes;
                    HttpContext.Current.Session["CotizacionesReportePlusCheck"] = cotizaciones;
                    return "OK";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }


        //<INI.GTI_7012_2_1>
        [WebMethod]
        public static Respuesta GenerarReporteCotizacionPlus(string tokenUsuario, string cuspp, int num_maximo, string[] solicitudes, string[] cotizaciones  )
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteCotizacionPlus))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudRPPlus> lstSolicitudes = servicioCotizador.ListarReporteCotizacionPlus(cuspp);


                            // Generar archivo Excel
                            HSSFWorkbook wb;
                            HSSFSheet sh;

                            wb = new HSSFWorkbook();
                            sh = (HSSFSheet)wb.CreateSheet("Resumen Simulaciones RPP");



                            NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 0);
                            int fila = 13;
                            int col = 0;
                            IRow r;

                            

                            // Ancho de columnas
                            sh.SetColumnWidth(0, 3200);
                            sh.SetColumnWidth(1, 3200);
                            //sh.SetColumnWidth(2, 3000);
                            

                            //sh.SetColumnWidth(3, 3500);
                            //sh.SetColumnWidth(4, 2800);
                            
                            //sh.SetColumnWidth(5, 3200);
                            
                            //sh.SetColumnWidth(6, 3600);
                            
                            sh.SetColumnWidth(7, 1500);
                            sh.SetColumnWidth(9, 4000);
                            sh.SetColumnWidth(10, 4000);
                            //sh.SetColumnWidth(9, 3200);
                            
                            sh.SetColumnWidth(11, 5400);
                            sh.SetColumnWidth(12, 4000);

                            sh.SetColumnWidth(13, 4000);
                            sh.SetColumnWidth(14, 3700);
                            sh.SetColumnWidth(15, 4000);
                            sh.SetColumnWidth(16, 4000);
                            sh.SetColumnWidth(17, 4000);


                            ///ICellStyle detalleanho2 = wb.CreateCellStyle();
                            ////detalleanho2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            ////detalleanho2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            ////detalleanho2.Alignment = HorizontalAlignment.Center;
                            ////detalleanho2.VerticalAlignment = VerticalAlignment.Center;
                            ////detalleanho2.FillForegroundColor = IndexedColors.White.Index;
                            ////detalleanho2.FillPattern = FillPattern.SolidForeground;

                          

                            /*Estilos de Celdas*/
                            IFont negrita = wb.CreateFont();
                            negrita.FontName = "Arial";
                            negrita.Boldweight = (short)FontBoldWeight.Bold;

                            IFont negritaTitulo = wb.CreateFont();
                            negritaTitulo.FontName = "Arial";
                            negritaTitulo.Boldweight = (short)FontBoldWeight.Bold;
                            negritaTitulo.FontHeightInPoints = 18;

                            IFont fontLeyenda = wb.CreateFont();
                            fontLeyenda.FontName = "Arial";
                            fontLeyenda.FontHeightInPoints = 8;

                            ICellStyle tituloGeneral = wb.CreateCellStyle();
                            tituloGeneral.Alignment = HorizontalAlignment.Center;
                            tituloGeneral.VerticalAlignment = VerticalAlignment.Center;
                            tituloGeneral.WrapText = true;
                            tituloGeneral.SetFont(negritaTitulo);

                            ICellStyle tituloDato = wb.CreateCellStyle();
                            tituloDato.Alignment = HorizontalAlignment.Left;
                            tituloDato.VerticalAlignment = VerticalAlignment.Center;
                            tituloDato.WrapText = true;
                            tituloDato.SetFont(negrita);


                            ICellStyle titulo1 = wb.CreateCellStyle();
                            titulo1.Alignment = HorizontalAlignment.Center;
                            titulo1.VerticalAlignment = VerticalAlignment.Center;
                            titulo1.WrapText = true;
                            titulo1.SetFont(negrita);
                            titulo1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            HSSFColor marronLimon = setColor(wb, 83, 142, 213, 0);
                            titulo1.FillForegroundColor = marronLimon.Indexed;
                            titulo1.FillPattern = FillPattern.SolidForeground;

                            ICellStyle titulo2 = wb.CreateCellStyle();
                            titulo2.Alignment = HorizontalAlignment.Center;
                            titulo2.VerticalAlignment = VerticalAlignment.Center;
                            titulo2.WrapText = true;
                            titulo2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            HSSFColor marronClaro = setColor(wb, 184, 204, 228, 1);
                            titulo2.FillForegroundColor = marronClaro.Indexed;
                            titulo2.FillPattern = FillPattern.SolidForeground;

                            ICellStyle titulo2Amarillo = wb.CreateCellStyle();
                            titulo2Amarillo.Alignment = HorizontalAlignment.Center;
                            titulo2Amarillo.VerticalAlignment = VerticalAlignment.Center;
                            titulo2Amarillo.WrapText = true;
                            titulo2Amarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo2Amarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo2Amarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            titulo2Amarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            HSSFColor amarillo = setColor(wb, 255, 255, 0, 1);
                            titulo2Amarillo.FillForegroundColor = amarillo.Indexed;
                            titulo2Amarillo.FillPattern = FillPattern.SolidForeground;

                            ICellStyle monedaSoles = wb.CreateCellStyle();
                            monedaSoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");
                            monedaSoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            monedaSoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            monedaSoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            monedaSoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                            ICellStyle monedaDolares = wb.CreateCellStyle();
                            monedaDolares.DataFormat = wb.CreateDataFormat().GetFormat("$ #,###.00");
                            monedaDolares.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            monedaDolares.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            monedaDolares.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            monedaDolares.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                            ICellStyle numero = wb.CreateCellStyle();
                            numero.DataFormat = wb.CreateDataFormat().GetFormat("#,##0.00");
                            numero.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            numero.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            numero.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            numero.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                            ICellStyle porcentaje = wb.CreateCellStyle();
                            porcentaje.DataFormat = wb.CreateDataFormat().GetFormat("#,##0.00%");
                            porcentaje.Alignment = HorizontalAlignment.Center;
                            porcentaje.VerticalAlignment = VerticalAlignment.Center;
                            porcentaje.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            porcentaje.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            porcentaje.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            porcentaje.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                            ICellStyle centrarDetalle = wb.CreateCellStyle();
                            centrarDetalle.Alignment = HorizontalAlignment.Center;
                            centrarDetalle.VerticalAlignment = VerticalAlignment.Center;
                            centrarDetalle.VerticalAlignment = VerticalAlignment.Center;
                            centrarDetalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            centrarDetalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            centrarDetalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            centrarDetalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                            ICellStyle centrarTasa = wb.CreateCellStyle();
                            centrarTasa.DataFormat = wb.CreateDataFormat().GetFormat("#,##0.00");
                            centrarTasa.Alignment = HorizontalAlignment.Center;
                            centrarTasa.VerticalAlignment = VerticalAlignment.Center;
                            centrarTasa.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            centrarTasa.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            centrarTasa.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            centrarTasa.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                            ICellStyle celdaBorde = wb.CreateCellStyle();
                            celdaBorde.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            celdaBorde.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            celdaBorde.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            celdaBorde.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                            ICellStyle leyenda = wb.CreateCellStyle();
                            leyenda.VerticalAlignment = VerticalAlignment.Top;
                            leyenda.SetFont(fontLeyenda);
                            leyenda.WrapText = true;
                            //amarillo = setColor(wb, 255, 255, 0, 1);
                            //leyenda.FillForegroundColor = amarillo.Indexed;
                            //leyenda.FillPattern = FillPattern.SolidForeground;

                            ICellStyle leyendaSinFondo = wb.CreateCellStyle();
                            leyendaSinFondo.VerticalAlignment = VerticalAlignment.Top;
                            leyendaSinFondo.SetFont(fontLeyenda);
                            leyendaSinFondo.WrapText = true;


                            ICellStyle borderTop = wb.CreateCellStyle();
                            borderTop.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;

                            ICellStyle borderRight = wb.CreateCellStyle();
                            borderRight.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;

                            ICellStyle borderLeft = wb.CreateCellStyle();
                            borderLeft.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;

                            ICellStyle borderBottom = wb.CreateCellStyle();
                            borderBottom.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;


                            ICellStyle borderTopLeft = wb.CreateCellStyle();
                            borderTopLeft.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
                            borderTopLeft.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;

                            ICellStyle borderTopRight = wb.CreateCellStyle();
                            borderTopRight.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
                            borderTopRight.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;

                            ICellStyle borderBottomLeft = wb.CreateCellStyle();
                            borderBottomLeft.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
                            borderBottomLeft.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;

                            ICellStyle borderBottomRight = wb.CreateCellStyle();
                            borderBottomRight.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
                            borderBottomRight.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;

                            /*Titulos de Columnas Secundarias*/
                            col = 0;
                            fila = 13;

                            r = sh.CreateRow(fila - 1);
                            r = sh.CreateRow(fila);
                            r.Height = 700;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Propuesta");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("No. Cotización");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Tipo de Plan");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Monto Prima moneda(a)");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Años  PG     (1) ");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Temporalidad");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("% TRAMO");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Años Tramo");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;
                            
                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Moneda");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Renta Primer Tramo");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Renta Segundo Tramo");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Tasa Venta");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Total Rentas Garantizadas (2)");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            //<INI.GTI_7012_3>
                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Retorno de Renta / Prima Única");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;
                            //<FIN.GTI_7012_3>

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("% Devolución Prima");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Total a Recibir (3)");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Retorno de la Inversión(4)");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            col += 1;

                            r.CreateCell(col);
                            sh.GetRow(fila).GetCell(col).SetCellValue("Vigencia de la propuesta");
                            sh.GetRow(fila).GetCell(col).CellStyle = titulo2;
                            
                            col += 1;

                            fila = 14;

                            //Filtrando y Buscando solo las Solicitudes Marcadas
                            for (int i = 0; i < solicitudes.Count(); i++)
                            {
                                SolicitudRPPlus solicitud = new SolicitudRPPlus();
                                solicitud = lstSolicitudes.Find(p => p.Id == solicitudes[i]);

                                for (int iCot = 0; iCot < cotizaciones.Count(); iCot++)
                                {
                                    CotizacionRPPlus cotizacion = null;//new CotizacionRPPlus();
                                    cotizacion = solicitud.Cotizaciones.Find(p => p.Correlativo == Convert.ToInt64(cotizaciones[iCot]));

                                    if (cotizacion != null)
                                    {
                                        col = 0;

                                        r = sh.CreateRow(fila);

                                        //"Propuesta"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(solicitud.Id);
                                        sh.GetRow(fila).GetCell(col).CellStyle = celdaBorde;
                                        col += 1;

                                        //"No. Cotización"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.Correlativo);
                                        sh.GetRow(fila).GetCell(col).CellStyle = celdaBorde;
                                        col += 1;

                                        //"Tipo de Plan"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(solicitud.TipoPlan.Nombre);
                                        sh.GetRow(fila).GetCell(col).CellStyle = celdaBorde;
                                        col += 1;

                                        //"Monto Prima moneda(a)
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(solicitud.PrimaUnica);
                                        sh.GetRow(fila).GetCell(col).CellStyle = (solicitud.MonedaPrimaUnica.Id == "001") ? monedaSoles : monedaDolares;
                                        col += 1;

                                        //"Años  PG     (1)
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.PeriodoGarantizado);
                                        sh.GetRow(fila).GetCell(col).CellStyle = centrarDetalle;
                                        col += 1;

                                        //"Temporalidad"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue((solicitud.Temporalidad.Anhos == 0) ? solicitud.Temporalidad.Nombre : solicitud.Temporalidad.Anhos.ToString());
                                        sh.GetRow(fila).GetCell(col).CellStyle = centrarDetalle;
                                        col += 1;

                                        //"% TRAMO"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.PjePE);
                                        sh.GetRow(fila).GetCell(col).CellStyle = celdaBorde;
                                        col += 1;

                                        //"Años Tramo"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.PagoEscalonada);
                                        sh.GetRow(fila).GetCell(col).CellStyle = celdaBorde;
                                        col += 1;

                                        //"Moneda
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.Moneda.Nombre + "  " +
                                            (cotizacion.ValMonAju.ToString() == "-1" ? "" : cotizacion.ValMonAju.ToString() + "%"));
                                        sh.GetRow(fila).GetCell(col).CellStyle = celdaBorde;
                                        col += 1;

                                        //Renta Primer Tramo"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.PensionCiaMO);
                                        sh.GetRow(fila).GetCell(col).CellStyle = numero;
                                        col += 1;

                                        //Renta Segundo Tramo"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.Pension2doTramo);
                                        sh.GetRow(fila).GetCell(col).CellStyle = numero;
                                        col += 1;

                                        //"Tasa Venta"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.TasaVentaSbs);
                                        sh.GetRow(fila).GetCell(col).CellStyle = centrarTasa;
                                        col += 1;

                                        //"Total Rentas Garantizadas (2)"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.ValTotalPeriodoGarantizado);
                                        sh.GetRow(fila).GetCell(col).CellStyle = numero;
                                        col += 1;

                                        //<INI.GTI_7012_3>
                                        //"Retorno de Renta / Prima Unica"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.ValTotalPeriodoGarantizado / solicitud.PrimaUnica);
                                        sh.GetRow(fila).GetCell(col).CellStyle = porcentaje;
                                        col += 1;
                                        //<FIN.GTI_7012_3>

                                        //"% Devolución Prima"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(cotizacion.ValPjeDev);
                                        sh.GetRow(fila).GetCell(col).CellStyle = centrarDetalle;
                                        col += 1;

                                        //"Total a Recibir (3)"
                                        double totalRecibir = 0;
                                        if (cotizacion.ValPjeDev != 0)
                                            totalRecibir = cotizacion.ValTotalPeriodoGarantizado + (solicitud.PrimaUnica * (cotizacion.ValPjeDev / 100));

                                        r.CreateCell(col);
                                        if (totalRecibir!=0)
                                            sh.GetRow(fila).GetCell(col).SetCellValue(totalRecibir);

                                        sh.GetRow(fila).GetCell(col).CellStyle = numero;
                                        col += 1;

                                        //"Retorno de la Inversión(4)"
                                        double retornoInversion = 0;
                                        if (cotizacion.ValPjeDev != 0)
                                            retornoInversion = totalRecibir / solicitud.PrimaUnica;

                                        r.CreateCell(col);
                                        if (totalRecibir != 0)
                                            sh.GetRow(fila).GetCell(col).SetCellValue(retornoInversion);

                                        sh.GetRow(fila).GetCell(col).CellStyle = porcentaje;
                                        col += 1;

                                        //"Vigencia de la propuesta"
                                        r.CreateCell(col);
                                        sh.GetRow(fila).GetCell(col).SetCellValue(solicitud.FechaVigencia.Value.ToString("dd/MM/yyyy"));
                                        sh.GetRow(fila).GetCell(col).CellStyle = centrarDetalle;
                                        col += 1;

                                        fila += 1;
                                    }
                                }

                                

                            }

                            sh.AutoSizeColumn(2);
                            sh.AutoSizeColumn(3);
                            sh.AutoSizeColumn(4);
                            sh.AutoSizeColumn(5);
                            sh.AutoSizeColumn(6);
                            sh.AutoSizeColumn(8);
                            sh.AutoSizeColumn(11);
                            

                            /*Leyenda*/
                            fila = fila +2;
                            r = sh.CreateRow(fila);  ;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila+1, 0, 5);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(0);
                            r.CreateCell(1);
                            r.CreateCell(2);
                            r.CreateCell(3);
                            r.CreateCell(4);
                            r.CreateCell(5);
                            sh.GetRow(fila).GetCell(0).SetCellValue("(1) Al fallecimiento del titular dentro del PG aplica claúsula adicional correspondiente. En dicho caso la suma de rentas de los beneficiarios es igual al 100% de la renta del asegurado, de manera proporcional.");
                            sh.GetRow(fila).GetCell(0).CellStyle = leyenda;


                            fila = fila + 3;
                            r = sh.CreateRow(fila); ;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila + 2, 0, 5);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(0);
                            r.CreateCell(1);
                            r.CreateCell(2);
                            r.CreateCell(3);
                            r.CreateCell(4);
                            r.CreateCell(5);
                            sh.GetRow(fila).GetCell(0).SetCellValue("(2) Corresponde al total de rentas que se pagarían mensuamente l hasta el final de la vigencia de la cláusula adicional. En el caso de las monedas S/ ajustados y $ ajustados, considera el ajuste trimestral. En los casos de S/ indexados no considera proyección de inflación.");
                            sh.GetRow(fila).GetCell(0).CellStyle = leyendaSinFondo;


                            fila = fila + 4;
                            r = sh.CreateRow(fila); ;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila , 0, 5);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(0);
                            r.CreateCell(1);
                            r.CreateCell(2);
                            r.CreateCell(3);
                            r.CreateCell(4);
                            r.CreateCell(5);
                            sh.GetRow(fila).GetCell(0).SetCellValue("(3) Incluye el Total de Rentas Garantizadas y el Monto de la Clausula Adicional de  Devolución de Prima.");
                            sh.GetRow(fila).GetCell(0).CellStyle = leyenda;



                            fila = fila + 2;
                            r = sh.CreateRow(fila); ;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 5);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(0);
                            r.CreateCell(1);
                            r.CreateCell(2);
                            r.CreateCell(3);
                            r.CreateCell(4);
                            r.CreateCell(5);
                            sh.GetRow(fila).GetCell(0).SetCellValue("(4) Representa la relación entre el total a recibir y la prima original. Calculos no aplican a Vitalicia.");
                            sh.GetRow(fila).GetCell(0).CellStyle = leyenda;


                            /*Datos*/
                            fila = 6;//--------------------------------------------------------
                            r = sh.CreateRow(fila);

                            r.CreateCell(3);
                            r.CreateCell(14);
                            r.CreateCell(4);
                            sh.GetRow(fila).GetCell(4).SetCellValue("Asegurado:");
                            sh.GetRow(fila).GetCell(4).CellStyle = tituloDato;


                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 5, 9);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(5);
                            r.CreateCell(6);
                            r.CreateCell(7);
                            r.CreateCell(8);
                            r.CreateCell(9);
                            string ApellidosNombres = "";
                            ApellidosNombres = lstSolicitudes[0].Beneficiarios[0].ApellidoPaterno + " ";
                            ApellidosNombres += lstSolicitudes[0].Beneficiarios[0].ApellidoMaterno + " ";
                            ApellidosNombres += lstSolicitudes[0].Beneficiarios[0].Nombre;

                            sh.GetRow(fila).GetCell(5).SetCellValue(ApellidosNombres);


                            r.CreateCell(10);
                            sh.GetRow(fila).GetCell(10).SetCellValue("Agente IS:");
                            sh.GetRow(fila).GetCell(10).CellStyle = tituloDato;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 11, 13);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(11);
                            r.CreateCell(12);
                            r.CreateCell(13);
                            sh.GetRow(fila).GetCell(11).SetCellValue(lstSolicitudes[0].Agente.Nombre);


                            fila = 7;//--------------------------------------------------------
                            r = sh.CreateRow(fila);

                            r.CreateCell(3);
                            r.CreateCell(14);
                            r.CreateCell(4);
                            sh.GetRow(fila).GetCell(4).SetCellValue("Documento:");
                            sh.GetRow(fila).GetCell(4).CellStyle = tituloDato;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 5, 9);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(5);
                            r.CreateCell(6);
                            r.CreateCell(7);
                            r.CreateCell(8);
                            r.CreateCell(9);
                            sh.GetRow(fila).GetCell(5).SetCellValue(lstSolicitudes[0].Beneficiarios[0].Identificacion.GlosaTipo + " " + lstSolicitudes[0].Beneficiarios[0].Identificacion.Numero.ToString());

                            //r.CreateCell(9);
                            //sh.GetRow(fila).GetCell(9).SetCellValue("Agente IS:");
                            //sh.GetRow(fila).GetCell(9).CellStyle = tituloDato;

                            //cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 10, 12);
                            //sh.AddMergedRegion(cra);
                            //r.CreateCell(10);
                            //r.CreateCell(11);
                            //r.CreateCell(12);
                            //sh.GetRow(fila).GetCell(10).SetCellValue(lstSolicitudes[0].Agente.Nombre);

                            fila = 8;//---------------------------------------------------------
                            r = sh.CreateRow(fila);

                            r.CreateCell(3);
                            r.CreateCell(14);
                            r.CreateCell(4);
                            sh.GetRow(fila).GetCell(4).SetCellValue("CUSPP:");
                            sh.GetRow(fila).GetCell(4).CellStyle = tituloDato;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 5, 9);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(5);
                            r.CreateCell(6);
                            r.CreateCell(7);
                            r.CreateCell(8);
                            r.CreateCell(9);
                            sh.GetRow(fila).GetCell(5).SetCellValue(lstSolicitudes[0].Afiliado.CUSPP);
                            
                            /*Bordes*/
                            fila = 5;//---------------------------------------------------------
                            r = sh.CreateRow(fila);
                            for (int i = 3; i <= 14; i++)
                            {
                                r.CreateCell(i);
                                sh.GetRow(fila).GetCell(i).CellStyle = borderTop;
                            }
                            fila = 9;//---------------------------------------------------------
                            r = sh.CreateRow(fila);
                            for (int i = 3; i <= 14; i++)
                            {
                                r.CreateCell(i);
                                sh.GetRow(fila).GetCell(i).CellStyle = borderBottom;
                            }

                            sh.GetRow(5).GetCell(3).CellStyle = borderTopLeft;
                            sh.GetRow(6).GetCell(3).CellStyle = borderLeft;
                            sh.GetRow(7).GetCell(3).CellStyle = borderLeft;
                            sh.GetRow(8).GetCell(3).CellStyle = borderLeft;
                            sh.GetRow(9).GetCell(3).CellStyle = borderBottomLeft;

                            sh.GetRow(5).GetCell(14).CellStyle = borderTopRight;
                            sh.GetRow(6).GetCell(14).CellStyle = borderRight;
                            sh.GetRow(7).GetCell(14).CellStyle = borderRight;
                            sh.GetRow(8).GetCell(14).CellStyle = borderRight;
                            sh.GetRow(9).GetCell(14).CellStyle = borderBottomRight;

                            /*Titulos de Columnas Primarias*/
                            col = 0;
                            fila = 12;

                            r = sh.CreateRow(fila);
                            r.Height = 600;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(0);
                            r.CreateCell(1);
                            r.CreateCell(2);
                            r.CreateCell(3);
                            sh.GetRow(fila).GetCell(0).SetCellValue("DESCRIPCIÓN");
                            sh.GetRow(fila).GetCell(0).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(1).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(2).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(3).CellStyle = titulo1;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(4);
                            r.CreateCell(5);
                            sh.GetRow(fila).GetCell(4).SetCellValue("COBERTURAS");
                            sh.GetRow(fila).GetCell(4).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(5).CellStyle = titulo1;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 6, 11);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(6);
                            r.CreateCell(7);
                            r.CreateCell(8);
                            r.CreateCell(9);
                            r.CreateCell(10);
                            r.CreateCell(11);
                            sh.GetRow(fila).GetCell(6).SetCellValue("COTIZACIÓN");
                            sh.GetRow(fila).GetCell(6).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(7).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(8).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(9).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(10).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(11).CellStyle = titulo1;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 12, 16);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(12);
                            r.CreateCell(13);
                            r.CreateCell(14);
                            r.CreateCell(15);
                            r.CreateCell(16);
                            sh.GetRow(fila).GetCell(12).SetCellValue("CONDICIONES ADICIONALES");
                            sh.GetRow(fila).GetCell(12).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(12).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(13).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(14).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(15).CellStyle = titulo1;
                            sh.GetRow(fila).GetCell(16).CellStyle = titulo1;



                            
                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila+1, 17, 17);
                            sh.AddMergedRegion(cra);
                            r.CreateCell(17);
                            sh.GetRow(fila).GetCell(17).SetCellValue("Vigencia de la propuesta");
                            sh.GetRow(fila).GetCell(17).CellStyle = titulo2;
                            sh.GetRow(fila+1).GetCell(17).CellStyle = titulo2;
                            
                            /*Titulo general*/
                            fila = 2;

                            r = sh.CreateRow(fila);
                            r.Height = 500;

                            cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 17);
                            sh.AddMergedRegion(cra);

                            for (int i = 0; i <= 17; i++)
                            {
                                r.CreateCell(i);
                                sh.GetRow(fila).GetCell(0).CellStyle = tituloGeneral;
                            }

                            sh.GetRow(fila).GetCell(0).SetCellValue("RESUMEN COMPARATIVO: RENTA PARTICULAR PLUS");



                            HttpContext.Current.Session["ArchivoExcel"] = wb;
                            HttpContext.Current.Session["NombreArchivoExcel"] = "RESUMEN COMPARATIVO RPP";

                            respuesta.Estado = Constante.COD_OK;
                        }
                        else
                        {
                            //control.PermisoEjecutar = false;
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
                            Detalle = String.Format("CUSSP {0} simulada", cuspp)
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
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "La solicitud no contiene los datos suficientes para generar la simulación, por favor intente generando una nueva cotización." });
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

        public static HSSFColor setColor(HSSFWorkbook workbook, byte r, byte g, byte b, int i)
        {
            HSSFPalette palette = workbook.GetCustomPalette();
            HSSFColor hssfColor = null;
            try
            {
                hssfColor = palette.FindColor(r, g, b);
                if (hssfColor == null)
                {
                    if (i == 0)
                    {
                        palette.SetColorAtIndex(HSSFColor.Lavender.Index, r, g, b);
                        hssfColor = palette.GetColor(HSSFColor.Lavender.Index);
                    }
                    else
                    {
                        palette.SetColorAtIndex(HSSFColor.LightBlue.Index, r, g, b);
                        hssfColor = palette.GetColor(HSSFColor.LightBlue.Index);
                        //LIGHT_BLUE(48)
                    }

                }
            }
            catch (Exception e)
            {
                //logger.error(e);
            }

            return hssfColor;
        }
        //<FIN.GTI_7012_2_1>
    }
}