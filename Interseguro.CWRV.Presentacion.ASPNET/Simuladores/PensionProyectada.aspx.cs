using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace Interseguro.CWRV.Presentacion.ASPNET.Simuladores
{
	public partial class PensionProyectada : System.Web.UI.Page
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(JubilarseHoyFuturo));
		private static IServicioCWRV servicioCotizador;

		protected void Page_Load(object sender, EventArgs e)
		{
			using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
			{
				try
				{
					HSSFWorkbook wb;
					wb = (HSSFWorkbook)Session["ReportePensionProyectada"];

					using (var exportData = new MemoryStream())
					{
						wb.Write(exportData);
						string nombreArchivo = String.Format("PensionProyectada-{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
						//Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						Response.ContentType = "application/vnd.ms-excel";
						Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", nombreArchivo));
						Response.Clear();
						Response.BinaryWrite(exportData.GetBuffer());
						Response.End();
					}  
				}
				catch (ThreadAbortException ex ) {
					log.Error(String.Format("Error de comunicación: [{0}: {1}]", ex.Source, ex.Message), ex);
				}
				catch (CommunicationException ex)
				{
					log.Error(String.Format("Error de comunicación: [{0}: {1}]", ex.Source, ex.Message), ex);
				}
				catch (Exception ex)
				{
					log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]", ex.Source, ex.Message, ex.StackTrace), ex);
				}
			}
		}

		[WebMethod]
		public static string HolaMundo(string tokenUsuario)
		{
			return tokenUsuario;
		}

		[WebMethod]
		public static Respuesta GenerarPensionProyectada(string tokenUsuario, List<GrupoFamiliar> beneficiarios, List<Cotizacion> cotizaciones, Int64 correlativo,
														 double cic, double tipoCambio, double rentabilidadAFP, double ipc, double ajuste)
		{
			using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
			{
				Respuesta respuesta = new Respuesta();
				try
				{
					if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
					{
						if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorJubilarseHoy))
						{
							// Obtener cotización
							Cotizacion cotizacion = cotizaciones.Find(c => c.Correlativo == correlativo);

							// Generar archivo Excel
							HSSFWorkbook wb;
							HSSFSheet sh;

							wb = new HSSFWorkbook();
							sh = (HSSFSheet)wb.CreateSheet("Pensión Proyectada");

							// Ancho de columnas
							sh.SetColumnWidth(0, 2800);
							sh.SetColumnWidth(1, 3200);
							sh.SetColumnWidth(2, 3600);
							sh.SetColumnWidth(3, 3000);
							sh.SetColumnWidth(4, 2800);
							sh.SetColumnWidth(5, 3200);
							sh.SetColumnWidth(6, 3600);
							sh.SetColumnWidth(7, 3000);
							sh.SetColumnWidth(8, 2800);
							sh.SetColumnWidth(9, 3200);
							sh.SetColumnWidth(10, 3600);
							sh.SetColumnWidth(11, 3000);

							// Formatos de celda
							ICellStyle monedaSoles = wb.CreateCellStyle();
							monedaSoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");
							ICellStyle monedaDolares = wb.CreateCellStyle();
							monedaDolares.DataFormat = wb.CreateDataFormat().GetFormat("$ #,###.00");
							ICellStyle porcentaje = wb.CreateCellStyle();
							porcentaje.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
							ICellStyle monto = wb.CreateCellStyle();
							monto.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

							ICellStyle titulo = wb.CreateCellStyle();
							titulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							titulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							titulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							titulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							titulo.Alignment = HorizontalAlignment.Left;
							titulo.VerticalAlignment = VerticalAlignment.Center;
							titulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
							titulo.FillPattern = FillPattern.SolidForeground;

							ICellStyle titulodato = wb.CreateCellStyle();
							titulodato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodato.Alignment = HorizontalAlignment.Left;
							titulodato.VerticalAlignment = VerticalAlignment.Center;
							titulodato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							titulodato.FillPattern = FillPattern.SolidForeground;

							ICellStyle totaltitulo = wb.CreateCellStyle();
							totaltitulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							totaltitulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							totaltitulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							totaltitulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							totaltitulo.Alignment = HorizontalAlignment.Left;
							totaltitulo.VerticalAlignment = VerticalAlignment.Center;
							totaltitulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
							totaltitulo.FillPattern = FillPattern.SolidForeground;

							ICellStyle totaldatosoles = wb.CreateCellStyle();
							totaldatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldatosoles.Alignment = HorizontalAlignment.Left;
							totaldatosoles.VerticalAlignment = VerticalAlignment.Center;
							totaldatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							totaldatosoles.FillPattern = FillPattern.SolidForeground;
							totaldatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

							ICellStyle totaldato = wb.CreateCellStyle();
							totaldato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							totaldato.Alignment = HorizontalAlignment.Left;
							totaldato.VerticalAlignment = VerticalAlignment.Center;
							totaldato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							totaldato.FillPattern = FillPattern.SolidForeground;

							ICellStyle titulodatomoneda = wb.CreateCellStyle();
							titulodatomoneda.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatomoneda.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatomoneda.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatomoneda.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatomoneda.Alignment = HorizontalAlignment.Left;
							titulodatomoneda.VerticalAlignment = VerticalAlignment.Center;
							titulodatomoneda.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							titulodatomoneda.FillPattern = FillPattern.SolidForeground;

							ICellStyle titulodatosoles = wb.CreateCellStyle();
							titulodatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							titulodatosoles.Alignment = HorizontalAlignment.Left;
							titulodatosoles.VerticalAlignment = VerticalAlignment.Center;
							titulodatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							titulodatosoles.FillPattern = FillPattern.SolidForeground;
							titulodatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

							ICellStyle cabecera = wb.CreateCellStyle();
							cabecera.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							cabecera.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							cabecera.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							cabecera.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							cabecera.Alignment = HorizontalAlignment.Center;
							cabecera.FillForegroundColor = IndexedColors.SkyBlue.Index;
							cabecera.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleanho1 = wb.CreateCellStyle();
							detalleanho1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho1.Alignment = HorizontalAlignment.Center;
							detalleanho1.VerticalAlignment = VerticalAlignment.Center;
							detalleanho1.FillForegroundColor = IndexedColors.White.Index;
							detalleanho1.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleanho2 = wb.CreateCellStyle();
							detalleanho2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho2.Alignment = HorizontalAlignment.Center;
							detalleanho2.VerticalAlignment = VerticalAlignment.Center;
							detalleanho2.FillForegroundColor = IndexedColors.White.Index;
							detalleanho2.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleanho3 = wb.CreateCellStyle();
							detalleanho3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanho3.Alignment = HorizontalAlignment.Center;
							detalleanho3.VerticalAlignment = VerticalAlignment.Center;
							detalleanho3.FillForegroundColor = IndexedColors.White.Index;
							detalleanho3.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalle = wb.CreateCellStyle();
							detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalle.Alignment = HorizontalAlignment.Center;
							detalle.VerticalAlignment = VerticalAlignment.Center;

							ICellStyle detalleanhodiferida1 = wb.CreateCellStyle();
							detalleanhodiferida1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida1.Alignment = HorizontalAlignment.Center;
							detalleanhodiferida1.VerticalAlignment = VerticalAlignment.Center;
							detalleanhodiferida1.FillForegroundColor = IndexedColors.Grey25Percent.Index;
							detalleanhodiferida1.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleanhodiferida2 = wb.CreateCellStyle();
							detalleanhodiferida2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida2.Alignment = HorizontalAlignment.Center;
							detalleanhodiferida2.VerticalAlignment = VerticalAlignment.Center;
							detalleanhodiferida2.FillForegroundColor = IndexedColors.Grey25Percent.Index;
							detalleanhodiferida2.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleanhodiferida3 = wb.CreateCellStyle();
							detalleanhodiferida3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhodiferida3.Alignment = HorizontalAlignment.Center;
							detalleanhodiferida3.VerticalAlignment = VerticalAlignment.Center;
							detalleanhodiferida3.FillForegroundColor = IndexedColors.Grey25Percent.Index;
							detalleanhodiferida3.FillPattern = FillPattern.SolidForeground;

							ICellStyle detallediferida = wb.CreateCellStyle();
							detallediferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detallediferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detallediferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detallediferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detallediferida.Alignment = HorizontalAlignment.Center;
							detallediferida.VerticalAlignment = VerticalAlignment.Center;
							detallediferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
							detallediferida.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleanhoprimerpago = wb.CreateCellStyle();
							detalleanhoprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhoprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhoprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleanhoprimerpago.Alignment = HorizontalAlignment.Center;
							detalleanhoprimerpago.VerticalAlignment = VerticalAlignment.Center;
							detalleanhoprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							detalleanhoprimerpago.FillPattern = FillPattern.SolidForeground;

							ICellStyle detalleprimerpago = wb.CreateCellStyle();
							detalleprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detalleprimerpago.Alignment = HorizontalAlignment.Center;
							detalleprimerpago.VerticalAlignment = VerticalAlignment.Center;
							detalleprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							detalleprimerpago.FillPattern = FillPattern.SolidForeground;

							ICellStyle detallepension = wb.CreateCellStyle();
							detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepension.Alignment = HorizontalAlignment.Right;
							detallepension.VerticalAlignment = VerticalAlignment.Center;
							detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

							ICellStyle detallepensiondiferida = wb.CreateCellStyle();
							detallepensiondiferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensiondiferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensiondiferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensiondiferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensiondiferida.Alignment = HorizontalAlignment.Right;
							detallepensiondiferida.VerticalAlignment = VerticalAlignment.Center;
							detallepensiondiferida.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
							detallepensiondiferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
							detallepensiondiferida.FillPattern = FillPattern.SolidForeground;

							ICellStyle detallepensionprimerpago = wb.CreateCellStyle();
							detallepensionprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensionprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensionprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensionprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							detallepensionprimerpago.Alignment = HorizontalAlignment.Right;
							detallepensionprimerpago.VerticalAlignment = VerticalAlignment.Center;
							detallepensionprimerpago.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
							detallepensionprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
							detallepensionprimerpago.FillPattern = FillPattern.SolidForeground;

							ICellStyle detallepensionacumulada = wb.CreateCellStyle();
							detallepensionacumulada.Alignment = HorizontalAlignment.Right;
							detallepensionacumulada.VerticalAlignment = VerticalAlignment.Center;
							detallepensionacumulada.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

							IFont negrita = wb.CreateFont();
							negrita.FontName = "Arial";
							negrita.Boldweight = (short)FontBoldWeight.Bold;
							titulo.SetFont(negrita);
							cabecera.SetFont(negrita);
							detallepensionacumulada.SetFont(negrita);
							detalleanhoprimerpago.SetFont(negrita);
							detalleanho1.SetFont(negrita);
							detalleanhodiferida1.SetFont(negrita);

							IFont negrita12 = wb.CreateFont();
							negrita12.FontName = "Arial";
							negrita12.Boldweight = (short)FontBoldWeight.Bold;
							negrita12.FontHeightInPoints = 12;
							totaltitulo.SetFont(negrita12);
							totaldato.SetFont(negrita12);
							totaldatosoles.SetFont(negrita12);

							double tasa = 1;
							double tasatrimestral;
							string moneda = String.Empty;
							string simbolo = String.Empty;
							string modalidad = String.Empty;
							ICellStyle estilomoneda = null;
							if (cotizacion.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
							{
								tasa = ajuste / 100.00;
								moneda = "SOLES AJUSTADOS";
								simbolo = "S/.";
								estilomoneda = monedaSoles;
							}
							else if (cotizacion.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue())
							{
								tasa = ajuste / 100.00;
								moneda = "DÓLARES AJUSTADOS";
								simbolo = "$";
								estilomoneda = monedaDolares;
								cotizacion.PensionCia = cotizacion.PensionCiaMO;
							}
							else if (cotizacion.Moneda.Id == Enums.Moneda.Soles.StringValue())
							{
								tasa = ipc / 100.00;
								moneda = "SOLES INDEXADOS";
								simbolo = "S/.";
								estilomoneda = monedaSoles;
							}
							else if (cotizacion.Moneda.Id == Enums.Moneda.Dolares.StringValue())
							{
								tasa = 1;
								moneda = "DÓLARES";
								simbolo = "$";
								estilomoneda = monedaDolares;
								cotizacion.PensionCia = cotizacion.PensionCiaMO;
							}
							totaldato.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
							titulodatomoneda.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
							tasatrimestral = Math.Pow(1 + tasa, 0.25) - 1;
							if (cotizacion.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue())
							{
								modalidad = "INMEDIATA";
							}
							else if (cotizacion.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
							{
								modalidad = "DIFERIDA";
							}
							else if (cotizacion.Modalidad.Id == Enums.Modalidad.Mixta.StringValue())
							{
								modalidad = "MIXTA";
							}
							else if (cotizacion.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
							{
								modalidad = "BIMONEDA";
							}
							else if (cotizacion.Modalidad.Id == Enums.Modalidad.Combinada.StringValue())
							{
								modalidad = "COMBINADA";
							}

							IRow r;

							GrupoFamiliar afiliado = beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

							int fila = 0;
							r = sh.CreateRow(fila);
							NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
							sh.AddMergedRegion(cra);
							r.CreateCell(0);
							r.CreateCell(1);
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(0).SetCellValue("CLIENTE");
							sh.GetRow(fila).GetCell(0).CellStyle = titulo;
							sh.GetRow(fila).GetCell(1).CellStyle = titulo;
							sh.GetRow(fila).GetCell(2).CellStyle = titulo;
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
							sh.AddMergedRegion(cra);
							r.CreateCell(3);
							r.CreateCell(4);
							r.CreateCell(5);
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(3).SetCellValue(afiliado.Nombre + " " + afiliado.ApellidoPaterno + " " + afiliado.ApellidoMaterno);
							sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
							fila++;

							r = sh.CreateRow(fila);
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
							sh.AddMergedRegion(cra);
							r.CreateCell(0);
							r.CreateCell(1);
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(0).SetCellValue("FONDO CIC");
							sh.GetRow(fila).GetCell(0).CellStyle = titulo;
							sh.GetRow(fila).GetCell(1).CellStyle = titulo;
							sh.GetRow(fila).GetCell(2).CellStyle = titulo;
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
							sh.AddMergedRegion(cra);
							r.CreateCell(3);
							r.CreateCell(4);
							r.CreateCell(5);
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(cic, 2));
							sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
							sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
							sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
							sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
							fila++;

							if (cotizacion.PeriodoDiferido > 0)
							{
								r = sh.CreateRow(fila);
								cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
								sh.AddMergedRegion(cra);
								r.CreateCell(0);
								r.CreateCell(1);
								r.CreateCell(2);
								sh.GetRow(fila).GetCell(0).SetCellValue("RENTA TEMPORAL");
								sh.GetRow(fila).GetCell(0).CellStyle = titulo;
								sh.GetRow(fila).GetCell(1).CellStyle = titulo;
								sh.GetRow(fila).GetCell(2).CellStyle = titulo;
								sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
								cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
								sh.AddMergedRegion(cra);
								r.CreateCell(3);
								r.CreateCell(4);
								r.CreateCell(5);
								r.CreateCell(6);
								sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(cotizacion.PensionAFP, 2));
								sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
								sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
								sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
								sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
								fila++;

								r = sh.CreateRow(fila);
								cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
								sh.AddMergedRegion(cra);
								r.CreateCell(0);
								r.CreateCell(1);
								r.CreateCell(2);
								sh.GetRow(fila).GetCell(0).SetCellValue("MODALIDAD DIFERIDA");
								sh.GetRow(fila).GetCell(0).CellStyle = titulo;
								sh.GetRow(fila).GetCell(1).CellStyle = titulo;
								sh.GetRow(fila).GetCell(2).CellStyle = titulo;
								sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
								cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
								sh.AddMergedRegion(cra);
								r.CreateCell(3);
								r.CreateCell(4);
								r.CreateCell(5);
								r.CreateCell(6);
								sh.GetRow(fila).GetCell(3).SetCellValue("A " + cotizacion.PeriodoDiferido + " AÑO" + (cotizacion.PeriodoDiferido != 1 ? "S" : String.Empty));
								sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
								sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
								sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
								sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
								fila++;
							}

							r = sh.CreateRow(fila);
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
							sh.AddMergedRegion(cra);
							r.CreateCell(0);
							r.CreateCell(1);
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(0).SetCellValue("PENSIÓN");
							sh.GetRow(fila).GetCell(0).CellStyle = titulo;
							sh.GetRow(fila).GetCell(1).CellStyle = titulo;
							sh.GetRow(fila).GetCell(2).CellStyle = titulo;
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
							sh.AddMergedRegion(cra);
							r.CreateCell(3);
							r.CreateCell(4);
							r.CreateCell(5);
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(cotizacion.PensionCia, 2));
							sh.GetRow(fila).GetCell(3).CellStyle = titulodatomoneda;
							sh.GetRow(fila).GetCell(4).CellStyle = titulodatomoneda;
							sh.GetRow(fila).GetCell(5).CellStyle = titulodatomoneda;
							sh.GetRow(fila).GetCell(6).CellStyle = titulodatomoneda;
							fila++;

							r = sh.CreateRow(fila);
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
							sh.AddMergedRegion(cra);
							r.CreateCell(0);
							r.CreateCell(1);
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(0).SetCellValue("MONEDA");
							sh.GetRow(fila).GetCell(0).CellStyle = titulo;
							sh.GetRow(fila).GetCell(1).CellStyle = titulo;
							sh.GetRow(fila).GetCell(2).CellStyle = titulo;
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
							sh.AddMergedRegion(cra);
							r.CreateCell(3);
							r.CreateCell(4);
							r.CreateCell(5);
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(3).SetCellValue(moneda + ((cotizacion.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue() || cotizacion.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) ? (" AL " + tasa * 100 + "%") : String.Empty));
							sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
							fila++;

							r = sh.CreateRow(fila);
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
							sh.AddMergedRegion(cra);
							r.CreateCell(0);
							r.CreateCell(1);
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE ANUAL");
							sh.GetRow(fila).GetCell(0).CellStyle = titulo;
							sh.GetRow(fila).GetCell(1).CellStyle = titulo;
							sh.GetRow(fila).GetCell(2).CellStyle = titulo;
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
							sh.AddMergedRegion(cra);
							r.CreateCell(3);
							r.CreateCell(4);
							r.CreateCell(5);
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(3).SetCellValue(tasa);
							sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
							sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
							sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
							sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
							fila++;

							r = sh.CreateRow(fila);
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
							sh.AddMergedRegion(cra);
							r.CreateCell(0);
							r.CreateCell(1);
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE TRIMESTRAL");
							sh.GetRow(fila).GetCell(0).CellStyle = titulo;
							sh.GetRow(fila).GetCell(1).CellStyle = titulo;
							sh.GetRow(fila).GetCell(2).CellStyle = titulo;
							cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
							sh.AddMergedRegion(cra);
							r.CreateCell(3);
							r.CreateCell(4);
							r.CreateCell(5);
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(3).SetCellValue(tasatrimestral);
							sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
							sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
							sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
							sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
							sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
							fila++;

							r = sh.CreateRow(fila);
							fila++;

							r = sh.CreateRow(fila);
							r.CreateCell(0);
							sh.GetRow(fila).GetCell(0).SetCellValue("Año");
							sh.GetRow(fila).GetCell(0).CellStyle = cabecera;
							r.CreateCell(1);
							sh.GetRow(fila).GetCell(1).SetCellValue("Mes");
							sh.GetRow(fila).GetCell(1).CellStyle = cabecera;
							r.CreateCell(2);
							sh.GetRow(fila).GetCell(2).SetCellValue("Pensión " + simbolo);
							sh.GetRow(fila).GetCell(2).CellStyle = cabecera;

							r.CreateCell(4);
							sh.GetRow(fila).GetCell(4).SetCellValue("Año");
							sh.GetRow(fila).GetCell(4).CellStyle = cabecera;
							r.CreateCell(5);
							sh.GetRow(fila).GetCell(5).SetCellValue("Mes");
							sh.GetRow(fila).GetCell(5).CellStyle = cabecera;
							r.CreateCell(6);
							sh.GetRow(fila).GetCell(6).SetCellValue("Pensión " + simbolo);
							sh.GetRow(fila).GetCell(6).CellStyle = cabecera;

							r.CreateCell(8);
							sh.GetRow(fila).GetCell(8).SetCellValue("Año");
							sh.GetRow(fila).GetCell(8).CellStyle = cabecera;
							r.CreateCell(9);
							sh.GetRow(fila).GetCell(9).SetCellValue("Mes");
							sh.GetRow(fila).GetCell(9).CellStyle = cabecera;
							r.CreateCell(10);
							sh.GetRow(fila).GetCell(10).SetCellValue("Pensión " + simbolo);
							sh.GetRow(fila).GetCell(10).CellStyle = cabecera;

							fila++;

							double pension = Math.Round(cotizacion.PensionCia, 2);
							int anho = 1;
							int mes = 1;
							bool primerpago;
							ICellStyle estilodetalleanho1 = detalleanhodiferida1;
							ICellStyle estilodetalleanho2 = detalleanhodiferida2;
							ICellStyle estilodetalleanho3 = detalleanhodiferida3;
							ICellStyle estilodetalle = detallediferida;
							ICellStyle estilodetallepension = detallepensiondiferida;
							double acumuladoanho = 0;
							double acumuladodiferido = 0;
							double acumuladogarantizado = 0;
							int contador = 0;
							int anhosgarantizados = cotizacion.PeriodoGarantizado;
							if (cotizacion.PeriodoDiferido == 0)
							{
								estilodetalleanho1 = detalleanhoprimerpago;
								estilodetalleanho2 = detalleanho2;
								estilodetalleanho3 = detalleanho3;
								estilodetalle = detalleprimerpago;
								estilodetallepension = detallepensionprimerpago;
								primerpago = true;
							}
							else
							{
								estilodetalleanho1 = detalleanhodiferida1;
								estilodetalleanho2 = detalleanhodiferida2;
								estilodetalleanho3 = detalleanhodiferida3;
								estilodetalle = detallediferida;
								estilodetallepension = detallepensiondiferida;
								primerpago = false;
							}
							// Bucle pensión proyectada - Columna 1
							for (int i = fila; i < fila + 36; i++)
							{
								r = sh.CreateRow(i);
								r.CreateCell(0);
								if (contador % 4 == 0)
								{
									sh.GetRow(i).GetCell(0).SetCellValue(anho);
									sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho1;
								}
								else if (contador % 4 == 1 || contador % 4 == 2)
								{
									sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho2;
								}
								else if (contador % 4 == 3)
								{
									sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho3;
								}
								r.CreateCell(1);
								sh.GetRow(i).GetCell(1).SetCellValue("Mes" + mes);
								sh.GetRow(i).GetCell(1).CellStyle = estilodetalle;
								r.CreateCell(2);
								sh.GetRow(i).GetCell(2).SetCellValue(Math.Round(pension, 2));
								sh.GetRow(i).GetCell(2).CellStyle = estilodetallepension;

								acumuladoanho += (pension * 3);

								if (primerpago)
								{
									estilodetalleanho1 = detalleanho1;
									estilodetalleanho2 = detalleanho2;
									estilodetalleanho3 = detalleanho3;
									estilodetalle = detalle;
									estilodetallepension = detallepension;
									primerpago = false;
								}

								mes += 3;
								if (mes > 12)
								{
									if (cotizacion.PeriodoDiferido > anho)
									{
										acumuladoanho = 0;
										acumuladodiferido += (cotizacion.PensionAFP * 12);
									}
									else if (cotizacion.PeriodoDiferido == anho)
									{
										primerpago = true;
										estilodetalleanho1 = detalleanhoprimerpago;
										estilodetalleanho2 = detalleanho2;
										estilodetalleanho3 = detalleanho3;
										estilodetalle = detalleprimerpago;
										estilodetallepension = detallepensionprimerpago;
										acumuladodiferido += (cotizacion.PensionAFP * 12);
										acumuladoanho = 0;
									}
									else if (cotizacion.PeriodoDiferido < anho)
									{
										r.CreateCell(3);
										sh.GetRow(i).GetCell(3).SetCellValue(acumuladoanho);
										sh.GetRow(i).GetCell(3).CellStyle = detallepensionacumulada;
										if (anhosgarantizados > 0)
										{
											acumuladogarantizado += acumuladoanho;
											anhosgarantizados--;
										}
										acumuladoanho = 0;
									}
									anho++;
									mes = 1;
								}
								pension *= (1 + tasatrimestral);
								//pension = Math.Round(pension, 2);
								contador++;
							}

							// Bucle pensión proyectada - Columna 2
							for (int i = fila; i < fila + 36; i++)
							{
								r = sh.GetRow(i);
								r.CreateCell(4);
								if (contador % 4 == 0)
								{
									sh.GetRow(i).GetCell(4).SetCellValue(anho);
									sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho1;
								}
								else if (contador % 4 == 1 || contador % 4 == 2)
								{
									sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho2;
								}
								else if (contador % 4 == 3)
								{
									sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho3;
								}
								r.CreateCell(5);
								sh.GetRow(i).GetCell(5).SetCellValue("Mes" + mes);
								sh.GetRow(i).GetCell(5).CellStyle = estilodetalle;
								r.CreateCell(6);
								sh.GetRow(i).GetCell(6).SetCellValue(Math.Round(pension, 2));
								sh.GetRow(i).GetCell(6).CellStyle = estilodetallepension;

								acumuladoanho += (pension * 3);

								if (primerpago)
								{
									estilodetalleanho1 = detalleanho1;
									estilodetalleanho2 = detalleanho2;
									estilodetalleanho3 = detalleanho3;
									estilodetalle = detalle;
									estilodetallepension = detallepension;
									primerpago = false;
								}

								mes += 3;
								if (mes > 12)
								{
									if (cotizacion.PeriodoDiferido > anho)
									{
										acumuladoanho = 0;
										acumuladodiferido += (cotizacion.PensionAFP * 12);
									}
									else if (cotizacion.PeriodoDiferido == anho)
									{
										primerpago = true;
										estilodetalleanho1 = detalleanhoprimerpago;
										estilodetalleanho2 = detalleanho2;
										estilodetalleanho3 = detalleanho3;
										estilodetalle = detalleprimerpago;
										estilodetallepension = detallepensionprimerpago;
										acumuladodiferido += (cotizacion.PensionAFP * 12);
										acumuladoanho = 0;
									}
									else if (cotizacion.PeriodoDiferido < anho)
									{
										r.CreateCell(7);
										sh.GetRow(i).GetCell(7).SetCellValue(acumuladoanho);
										sh.GetRow(i).GetCell(7).CellStyle = detallepensionacumulada;
										if (anhosgarantizados > 0)
										{
											acumuladogarantizado += acumuladoanho;
											anhosgarantizados--;
										}
										acumuladoanho = 0;
									}
									anho++;
									mes = 1;
								}
								pension *= (1 + tasatrimestral);
								//pension = Math.Round(pension, 2);
								contador++;
							}

							// Bucle pensión proyectada - Columna 3
							for (int i = fila; i < fila + 28; i++)
							{
								r = sh.GetRow(i);
								r.CreateCell(8);
								if (contador % 4 == 0)
								{
									sh.GetRow(i).GetCell(8).SetCellValue(anho);
									sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho1;
								}
								else if (contador % 4 == 1 || contador % 4 == 2)
								{
									sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho2;
								}
								else if (contador % 4 == 3)
								{
									sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho3;
								}
								r.CreateCell(9);
								sh.GetRow(i).GetCell(9).SetCellValue("Mes" + mes);
								sh.GetRow(i).GetCell(9).CellStyle = detalle;
								r.CreateCell(10);
								sh.GetRow(i).GetCell(10).SetCellValue(Math.Round(pension, 2));
								sh.GetRow(i).GetCell(10).CellStyle = detallepension;

								acumuladoanho += (pension * 3);

								if (primerpago)
								{
									estilodetalleanho1 = detalleanho1;
									estilodetalleanho2 = detalleanho2;
									estilodetalleanho3 = detalleanho3;
									estilodetalle = detalle;
									estilodetallepension = detallepension;
									primerpago = false;
								}

								mes += 3;
								if (mes > 12)
								{
									if (cotizacion.PeriodoDiferido > anho)
									{
										acumuladoanho = 0;
										acumuladodiferido += (cotizacion.PensionAFP * 12);
									}
									else if (cotizacion.PeriodoDiferido == anho)
									{
										primerpago = true;
										estilodetalleanho1 = detalleanhoprimerpago;
										estilodetalleanho2 = detalleanho2;
										estilodetalleanho3 = detalleanho3;
										estilodetalle = detalleprimerpago;
										estilodetallepension = detallepensionprimerpago;
										acumuladodiferido += (cotizacion.PensionAFP * 12);
										acumuladoanho = 0;
									}
									else if (cotizacion.PeriodoDiferido < anho)
									{
										r.CreateCell(11);
										sh.GetRow(i).GetCell(11).SetCellValue(acumuladoanho);
										sh.GetRow(i).GetCell(11).CellStyle = detallepensionacumulada;
										if (anhosgarantizados > 0)
										{
											acumuladogarantizado += acumuladoanho;
											anhosgarantizados--;
										}
										acumuladoanho = 0;
									}
									anho++;
									mes = 1;
								}
								pension *= (1 + tasatrimestral);
								//pension = Math.Round(pension, 2);
								contador++;
							}

							if (acumuladodiferido > 0 || acumuladogarantizado > 0)
							{
								fila += 36;
								r = sh.CreateRow(fila);

								if (acumuladodiferido > 0)
								{
									fila++;
									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									r.CreateCell(3);
									sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL RENTA TEMPORAL (" + cotizacion.PeriodoDiferido + " AÑO" + (cotizacion.PeriodoDiferido != 1 ? "S" : String.Empty) + ")");
									sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
									sh.AddMergedRegion(cra);
									r.CreateCell(4);
									r.CreateCell(5);
									sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido);
									sh.GetRow(fila).GetCell(4).CellStyle = totaldatosoles;
									sh.GetRow(fila).GetCell(5).CellStyle = totaldatosoles;
									fila++;
									r = sh.CreateRow(fila);
								}

								if (acumuladogarantizado > 0)
								{
									fila++;
									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									r.CreateCell(3);
									sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL PERÍODO GARANTIZADO (" + cotizacion.PeriodoGarantizado + " AÑO" + (cotizacion.PeriodoGarantizado != 1 ? "S" : String.Empty) + ")");
									sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
									sh.AddMergedRegion(cra);
									r.CreateCell(4);
									r.CreateCell(5);
									sh.GetRow(fila).GetCell(4).SetCellValue(acumuladogarantizado);
									sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
									sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
									fila++;
									r = sh.CreateRow(fila);
								}

								if (acumuladodiferido > 0 && acumuladogarantizado > 0 && (cotizacion.Moneda.Id == Enums.Moneda.Soles.StringValue() || cotizacion.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()))
								{
									fila++;
									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									r.CreateCell(3);
									sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL " + (cotizacion.PeriodoGarantizado + cotizacion.PeriodoDiferido) + " PRIMEROS AÑO" + (cotizacion.PeriodoGarantizado + cotizacion.PeriodoDiferido != 1 ? "S" : String.Empty));
									sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
									sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
									sh.AddMergedRegion(cra);
									r.CreateCell(4);
									r.CreateCell(5);
									sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido + acumuladogarantizado);
									sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
									sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
								}
							}

							HttpContext.Current.Session["ReportePensionProyectada"] = wb;

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
							Detalle = String.Format("Cotización {0} simulada", correlativo)
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

		[WebMethod]
		public static Respuesta GenerarPensionProyectada6(string tokenUsuario, List<GrupoFamiliar> beneficiarios, List<Cotizacion> cotizaciones,
														  Int64 correlativo1, Int64 correlativo2, Int64 correlativo3,
														  Int64 correlativo4, Int64 correlativo5, Int64 correlativo6,
														  double cic, double tipoCambio, double rentabilidadAFP, double ipc, double ajuste)
		{
			using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
			{
				Respuesta respuesta = new Respuesta();
				try
				{
					if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
					{
						if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorJubilarseHoy))
						{
							// Declarar arreglo de cotizaciones
							Cotizacion[] cotizacion = new Cotizacion[6];
							for (int i=0; i<6; i++)
							{
								cotizacion[i] = null;
							}

							// Obtener cotizaciones
							if (correlativo1 > 0) cotizacion[0] = cotizaciones.Find(c => c.Correlativo == correlativo1);
							if (correlativo2 > 0) cotizacion[1] = cotizaciones.Find(c => c.Correlativo == correlativo2);
							if (correlativo3 > 0) cotizacion[2] = cotizaciones.Find(c => c.Correlativo == correlativo3);
							if (correlativo4 > 0) cotizacion[3] = cotizaciones.Find(c => c.Correlativo == correlativo4);
							if (correlativo5 > 0) cotizacion[4] = cotizaciones.Find(c => c.Correlativo == correlativo5);
							if (correlativo6 > 0) cotizacion[5] = cotizaciones.Find(c => c.Correlativo == correlativo6);

							// Generar archivo Excel
							HSSFWorkbook wb;
							HSSFSheet sh;

							wb = new HSSFWorkbook();

							foreach (Cotizacion c in cotizacion)
							{
								// Pintar la Hoja Excel únicamente si se ha seleccionado una cotización
								if (c != null)
								{
									// Hallar la modalidad de la cotización
									string modalidad = String.Empty;
									if (c.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue())
									{
										modalidad = "Inmediata";
									}
									else if (c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
									{
										modalidad = "Diferida " + c.PeriodoDiferido + " año" + (c.PeriodoDiferido != 1 ? "s" : String.Empty);
									}
									else if (c.Modalidad.Id == Enums.Modalidad.Mixta.StringValue())
									{
										modalidad = "Mixta";
									}
									else if (c.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
									{
										modalidad = "Bimoneda";
									}
									else if (c.Modalidad.Id == Enums.Modalidad.Combinada.StringValue())
									{
										modalidad = "Combinada";
									}

									sh = (HSSFSheet)wb.CreateSheet(modalidad);

									// Ancho de columnas
									sh.SetColumnWidth(0, 2800);
									sh.SetColumnWidth(1, 3200);
									sh.SetColumnWidth(2, 3600);
									sh.SetColumnWidth(3, 3000);
									sh.SetColumnWidth(4, 2800);
									sh.SetColumnWidth(5, 3200);
									sh.SetColumnWidth(6, 3600);
									sh.SetColumnWidth(7, 3000);
									sh.SetColumnWidth(8, 2800);
									sh.SetColumnWidth(9, 3200);
									sh.SetColumnWidth(10, 3600);
									sh.SetColumnWidth(11, 3000);

									// Formatos de celda
									ICellStyle monedaSoles = wb.CreateCellStyle();
									monedaSoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");
									ICellStyle monedaDolares = wb.CreateCellStyle();
									monedaDolares.DataFormat = wb.CreateDataFormat().GetFormat("$ #,###.00");
									ICellStyle porcentaje = wb.CreateCellStyle();
									porcentaje.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									ICellStyle monto = wb.CreateCellStyle();
									monto.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									ICellStyle titulo = wb.CreateCellStyle();
									titulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.Alignment = HorizontalAlignment.Left;
									titulo.VerticalAlignment = VerticalAlignment.Center;
									titulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
									titulo.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodato = wb.CreateCellStyle();
									titulodato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.Alignment = HorizontalAlignment.Left;
									titulodato.VerticalAlignment = VerticalAlignment.Center;
									titulodato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodato.FillPattern = FillPattern.SolidForeground;

									ICellStyle totaltitulo = wb.CreateCellStyle();
									totaltitulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.Alignment = HorizontalAlignment.Left;
									totaltitulo.VerticalAlignment = VerticalAlignment.Center;
									totaltitulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
									totaltitulo.FillPattern = FillPattern.SolidForeground;

									ICellStyle totaldatosoles = wb.CreateCellStyle();
									totaldatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.Alignment = HorizontalAlignment.Left;
									totaldatosoles.VerticalAlignment = VerticalAlignment.Center;
									totaldatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									totaldatosoles.FillPattern = FillPattern.SolidForeground;
									totaldatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

									ICellStyle totaldato = wb.CreateCellStyle();
									totaldato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.Alignment = HorizontalAlignment.Left;
									totaldato.VerticalAlignment = VerticalAlignment.Center;
									totaldato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									totaldato.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodatomoneda = wb.CreateCellStyle();
									titulodatomoneda.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.Alignment = HorizontalAlignment.Left;
									titulodatomoneda.VerticalAlignment = VerticalAlignment.Center;
									titulodatomoneda.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodatomoneda.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodatosoles = wb.CreateCellStyle();
									titulodatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.Alignment = HorizontalAlignment.Left;
									titulodatosoles.VerticalAlignment = VerticalAlignment.Center;
									titulodatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodatosoles.FillPattern = FillPattern.SolidForeground;
									titulodatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

									ICellStyle cabecera = wb.CreateCellStyle();
									cabecera.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.Alignment = HorizontalAlignment.Center;
									cabecera.FillForegroundColor = IndexedColors.SkyBlue.Index;
									cabecera.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho1 = wb.CreateCellStyle();
									detalleanho1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.Alignment = HorizontalAlignment.Center;
									detalleanho1.VerticalAlignment = VerticalAlignment.Center;
									detalleanho1.FillForegroundColor = IndexedColors.White.Index;
									detalleanho1.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho2 = wb.CreateCellStyle();
									detalleanho2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho2.Alignment = HorizontalAlignment.Center;
									detalleanho2.VerticalAlignment = VerticalAlignment.Center;
									detalleanho2.FillForegroundColor = IndexedColors.White.Index;
									detalleanho2.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho3 = wb.CreateCellStyle();
									detalleanho3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.Alignment = HorizontalAlignment.Center;
									detalleanho3.VerticalAlignment = VerticalAlignment.Center;
									detalleanho3.FillForegroundColor = IndexedColors.White.Index;
									detalleanho3.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalle = wb.CreateCellStyle();
									detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.Alignment = HorizontalAlignment.Center;
									detalle.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle detalleanhodiferida1 = wb.CreateCellStyle();
									detalleanhodiferida1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida1.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida1.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida1.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhodiferida2 = wb.CreateCellStyle();
									detalleanhodiferida2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida2.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida2.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida2.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida2.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhodiferida3 = wb.CreateCellStyle();
									detalleanhodiferida3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida3.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida3.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida3.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallediferida = wb.CreateCellStyle();
									detallediferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.Alignment = HorizontalAlignment.Center;
									detallediferida.VerticalAlignment = VerticalAlignment.Center;
									detallediferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detallediferida.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhoprimerpago = wb.CreateCellStyle();
									detalleanhoprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.Alignment = HorizontalAlignment.Center;
									detalleanhoprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detalleanhoprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detalleanhoprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleprimerpago = wb.CreateCellStyle();
									detalleprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.Alignment = HorizontalAlignment.Center;
									detalleprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detalleprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detalleprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepension = wb.CreateCellStyle();
									detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.Alignment = HorizontalAlignment.Right;
									detallepension.VerticalAlignment = VerticalAlignment.Center;
									detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									ICellStyle detallepensiondiferida = wb.CreateCellStyle();
									detallepensiondiferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.Alignment = HorizontalAlignment.Right;
									detallepensiondiferida.VerticalAlignment = VerticalAlignment.Center;
									detallepensiondiferida.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensiondiferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detallepensiondiferida.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensionprimerpago = wb.CreateCellStyle();
									detallepensionprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.Alignment = HorizontalAlignment.Right;
									detallepensionprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detallepensionprimerpago.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensionprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detallepensionprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensionacumulada = wb.CreateCellStyle();
									detallepensionacumulada.Alignment = HorizontalAlignment.Right;
									detallepensionacumulada.VerticalAlignment = VerticalAlignment.Center;
									detallepensionacumulada.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									IFont negrita = wb.CreateFont();
									negrita.FontName = "Arial";
									negrita.Boldweight = (short)FontBoldWeight.Bold;
									titulo.SetFont(negrita);
									cabecera.SetFont(negrita);
									detallepensionacumulada.SetFont(negrita);
									detalleanhoprimerpago.SetFont(negrita);
									detalleanho1.SetFont(negrita);
									detalleanhodiferida1.SetFont(negrita);

									IFont negrita12 = wb.CreateFont();
									negrita12.FontName = "Arial";
									negrita12.Boldweight = (short)FontBoldWeight.Bold;
									negrita12.FontHeightInPoints = 12;
									totaltitulo.SetFont(negrita12);
									totaldato.SetFont(negrita12);
									totaldatosoles.SetFont(negrita12);

									double tasa = 1;
									double tasatrimestral;
									string moneda = String.Empty;
									string simbolo = String.Empty;
									ICellStyle estilomoneda = null;
									if (c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
									{
										tasa = ajuste / 100.00;
										moneda = "SOLES AJUSTADOS";
										simbolo = "S/.";
										estilomoneda = monedaSoles;
									}
									else if (c.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue())
									{
										tasa = ajuste / 100.00;
										moneda = "DÓLARES AJUSTADOS";
										simbolo = "$";
										estilomoneda = monedaDolares;
										c.PensionCia = c.PensionCiaMO;
									}
									else if (c.Moneda.Id == Enums.Moneda.Soles.StringValue())
									{
										tasa = ipc / 100.00;
										moneda = "SOLES INDEXADOS";
										simbolo = "S/.";
										estilomoneda = monedaSoles;
									}
									else if (c.Moneda.Id == Enums.Moneda.Dolares.StringValue())
									{
										tasa = 1;
										moneda = "DÓLARES";
										simbolo = "$";
										estilomoneda = monedaDolares;
										c.PensionCia = c.PensionCiaMO;
									}
									totaldato.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
									titulodatomoneda.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
									tasatrimestral = Math.Pow(1 + tasa, 0.25) - 1;

									IRow r;

									GrupoFamiliar afiliado = beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

									int fila = 0;
									r = sh.CreateRow(fila);
									NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("CLIENTE");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(afiliado.Nombre + " " + afiliado.ApellidoPaterno + " " + afiliado.ApellidoMaterno);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("FONDO CIC");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(cic, 2));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
									fila++;

									if (c.PeriodoDiferido > 0)
									{
										r = sh.CreateRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
										sh.AddMergedRegion(cra);
										r.CreateCell(0);
										r.CreateCell(1);
										r.CreateCell(2);
										sh.GetRow(fila).GetCell(0).SetCellValue("RENTA TEMPORAL");
										sh.GetRow(fila).GetCell(0).CellStyle = titulo;
										sh.GetRow(fila).GetCell(1).CellStyle = titulo;
										sh.GetRow(fila).GetCell(2).CellStyle = titulo;
										sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
										sh.AddMergedRegion(cra);
										r.CreateCell(3);
										r.CreateCell(4);
										r.CreateCell(5);
										r.CreateCell(6);
										sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PensionAFP, 2));
										sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
										sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
										sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
										sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
										fila++;

										r = sh.CreateRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
										sh.AddMergedRegion(cra);
										r.CreateCell(0);
										r.CreateCell(1);
										r.CreateCell(2);
										sh.GetRow(fila).GetCell(0).SetCellValue("MODALIDAD DIFERIDA");
										sh.GetRow(fila).GetCell(0).CellStyle = titulo;
										sh.GetRow(fila).GetCell(1).CellStyle = titulo;
										sh.GetRow(fila).GetCell(2).CellStyle = titulo;
										sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
										sh.AddMergedRegion(cra);
										r.CreateCell(3);
										r.CreateCell(4);
										r.CreateCell(5);
										r.CreateCell(6);
										sh.GetRow(fila).GetCell(3).SetCellValue("A " + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty));
										sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
										fila++;
									}

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("PENSIÓN");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PensionCia, 2));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodatomoneda;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("MONEDA");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(moneda + ((c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue() || c.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) ? (" AL " + tasa * 100 + "%") : String.Empty));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE ANUAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(tasa);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE TRIMESTRAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(tasatrimestral);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									r = sh.CreateRow(fila);
									r.CreateCell(0);
									sh.GetRow(fila).GetCell(0).SetCellValue("Año");
									sh.GetRow(fila).GetCell(0).CellStyle = cabecera;
									r.CreateCell(1);
									sh.GetRow(fila).GetCell(1).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(1).CellStyle = cabecera;
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(2).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(2).CellStyle = cabecera;

									r.CreateCell(4);
									sh.GetRow(fila).GetCell(4).SetCellValue("Año");
									sh.GetRow(fila).GetCell(4).CellStyle = cabecera;
									r.CreateCell(5);
									sh.GetRow(fila).GetCell(5).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(5).CellStyle = cabecera;
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(6).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(6).CellStyle = cabecera;

									r.CreateCell(8);
									sh.GetRow(fila).GetCell(8).SetCellValue("Año");
									sh.GetRow(fila).GetCell(8).CellStyle = cabecera;
									r.CreateCell(9);
									sh.GetRow(fila).GetCell(9).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(9).CellStyle = cabecera;
									r.CreateCell(10);
									sh.GetRow(fila).GetCell(10).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(10).CellStyle = cabecera;

									fila++;

									double pension = Math.Round(c.PensionCia, 2);
									int anho = 1;
									int mes = 1;
									bool primerpago;
									ICellStyle estilodetalleanho1 = detalleanhodiferida1;
									ICellStyle estilodetalleanho2 = detalleanhodiferida2;
									ICellStyle estilodetalleanho3 = detalleanhodiferida3;
									ICellStyle estilodetalle = detallediferida;
									ICellStyle estilodetallepension = detallepensiondiferida;
									double acumuladoanho = 0;
									double acumuladodiferido = 0;
									double acumuladogarantizado = 0;
									int contador = 0;
									int anhosgarantizados = c.PeriodoGarantizado;
									if (c.PeriodoDiferido == 0)
									{
										estilodetalleanho1 = detalleanhoprimerpago;
										estilodetalleanho2 = detalleanho2;
										estilodetalleanho3 = detalleanho3;
										estilodetalle = detalleprimerpago;
										estilodetallepension = detallepensionprimerpago;
										primerpago = true;
									}
									else
									{
										estilodetalleanho1 = detalleanhodiferida1;
										estilodetalleanho2 = detalleanhodiferida2;
										estilodetalleanho3 = detalleanhodiferida3;
										estilodetalle = detallediferida;
										estilodetallepension = detallepensiondiferida;
										primerpago = false;
									}
									// Bucle pensión proyectada - Columna 1
									for (int i = fila; i < fila + 36; i++)
									{
										r = sh.CreateRow(i);
										r.CreateCell(0);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(0).SetCellValue(anho);
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(1);
										sh.GetRow(i).GetCell(1).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(1).CellStyle = estilodetalle;
										r.CreateCell(2);
										sh.GetRow(i).GetCell(2).SetCellValue(Math.Round(pension, 2));
										sh.GetRow(i).GetCell(2).CellStyle = estilodetallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												acumuladoanho = 0;
												acumuladodiferido += (c.PensionAFP * 12);
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												acumuladodiferido += (c.PensionAFP * 12);
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(3);
												sh.GetRow(i).GetCell(3).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(3).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									// Bucle pensión proyectada - Columna 2
									for (int i = fila; i < fila + 36; i++)
									{
										r = sh.GetRow(i);
										r.CreateCell(4);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(4).SetCellValue(anho);
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(5);
										sh.GetRow(i).GetCell(5).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(5).CellStyle = estilodetalle;
										r.CreateCell(6);
										sh.GetRow(i).GetCell(6).SetCellValue(Math.Round(pension, 2));
										sh.GetRow(i).GetCell(6).CellStyle = estilodetallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												acumuladoanho = 0;
												acumuladodiferido += (c.PensionAFP * 12);
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												acumuladodiferido += (c.PensionAFP * 12);
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(7);
												sh.GetRow(i).GetCell(7).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(7).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									// Bucle pensión proyectada - Columna 3
									for (int i = fila; i < fila + 28; i++)
									{
										r = sh.GetRow(i);
										r.CreateCell(8);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(8).SetCellValue(anho);
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(9);
										sh.GetRow(i).GetCell(9).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(9).CellStyle = detalle;
										r.CreateCell(10);
										sh.GetRow(i).GetCell(10).SetCellValue(Math.Round(pension, 2));
										sh.GetRow(i).GetCell(10).CellStyle = detallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												acumuladoanho = 0;
												acumuladodiferido += (c.PensionAFP * 12);
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												acumuladodiferido += (c.PensionAFP * 12);
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(11);
												sh.GetRow(i).GetCell(11).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(11).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									if (acumuladodiferido > 0 || acumuladogarantizado > 0)
									{
										fila += 36;
										r = sh.CreateRow(fila);

										if (acumuladodiferido > 0)
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL RENTA TEMPORAL (" + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty) + ")");
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldatosoles;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldatosoles;
											fila++;
											r = sh.CreateRow(fila);
										}

										if (acumuladogarantizado > 0)
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL PERÍODO GARANTIZADO (" + c.PeriodoGarantizado + " AÑO" + (c.PeriodoGarantizado != 1 ? "S" : String.Empty) + ")");
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladogarantizado);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
											fila++;
											r = sh.CreateRow(fila);
										}

										if (acumuladodiferido > 0 && acumuladogarantizado > 0 && (c.Moneda.Id == Enums.Moneda.Soles.StringValue() || c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()))
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL " + (c.PeriodoGarantizado + c.PeriodoDiferido) + " PRIMEROS AÑO" + (c.PeriodoGarantizado + c.PeriodoDiferido != 1 ? "S" : String.Empty));
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido + acumuladogarantizado);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
										}
									}
								}
							}

							HttpContext.Current.Session["ReportePensionProyectada"] = wb;

							respuesta.Estado = Constante.COD_OK;
						}
						else
						{
							//control.PermisoEjecutar = false;
						}

						//string nombreTerminal = String.Empty;
						//try
						//{
						//    nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
						//}
						//catch (Exception)
						//{
						//    log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
						//        HttpContext.Current.Request.ServerVariables["remote_addr"]));
						//}

						//nombreTerminal += HttpContext.Current.Request.UserAgent;

						//servicioCotizador = LocalizadorProxy.ObtenerServicio();
						//servicioCotizador.RegistrarLog(new LogBD
						//{
						//    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
						//    NombreTerminal = nombreTerminal,
						//    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
						//    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
						//    IdTipoEvento = Enums.EventoLog.Reporte1.StringValue(),
						//    Detalle = String.Format("Cotización {0} simulada", correlativo)
						//});
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

		[WebMethod]
		public static Respuesta GenerarPensionProyectadaMoneda(string tokenUsuario, List<GrupoFamiliar> beneficiarios, List<Cotizacion> cotizaciones,
															   Int64 correlativo1, Int64 correlativo2, Int64 correlativo3, string modalidad, int periodoGarantizado, 
															   double cic, double tipoCambio, double rentabilidadAFP, double ipc, double ajusteTasa, double ajusteTC)
		{
			using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
			{
				Respuesta respuesta = new Respuesta();
				try
				{
					if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
					{
						if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorJubilarseHoy))
						{
							// Declaración de variables
							IRow r;
							int contadorCotizaciones = 0;
							int fila = 0;
							CellRangeAddress cra;

							// Declarar arreglo de cotizaciones
							Cotizacion[] cotizacion = new Cotizacion[6];
							for (int i = 0; i < 6; i++)
							{
								cotizacion[i] = null;
							}

							// Obtener cotizaciones
							if (correlativo1 > 0)
							{
								cotizacion[0] = cotizaciones.Find(c => c.Correlativo == correlativo1);
								contadorCotizaciones++;
							}
							if (correlativo2 > 0)
							{
								cotizacion[1] = cotizaciones.Find(c => c.Correlativo == correlativo2);
								contadorCotizaciones++;
							}
							if (correlativo3 > 0)
							{
								cotizacion[2] = cotizaciones.Find(c => c.Correlativo == correlativo3);
								contadorCotizaciones++;
							}

							// Generar archivo Excel
							HSSFWorkbook wb;
							HSSFSheet sh;

							wb = new HSSFWorkbook();

							foreach (Cotizacion c in cotizacion)
							{
								// Pintar la Hoja Excel únicamente si se ha seleccionado una cotización
								if (c != null)
								{
									// Formatos de celda
									ICellStyle monedaSoles = wb.CreateCellStyle();
									monedaSoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");
									ICellStyle monedaDolares = wb.CreateCellStyle();
									monedaDolares.DataFormat = wb.CreateDataFormat().GetFormat("$ #,###.00");
									ICellStyle porcentaje = wb.CreateCellStyle();
									porcentaje.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									ICellStyle monto = wb.CreateCellStyle();
									monto.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									double tasa = 1;
									double tasatrimestral;
									string moneda = String.Empty;
									string monedatab = String.Empty;
									string simbolo = String.Empty;
									ICellStyle estilomoneda = null;
									if (c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
									{
										tasa = ajusteTasa / 100.00;
										moneda = "SOLES AJUSTADOS";
										monedatab = "Soles Ajustados";
										simbolo = "S/.";
										estilomoneda = monedaSoles;
									}
									else if (c.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue())
									{
										tasa = ajusteTasa / 100.00;
										moneda = "DÓLARES AJUSTADOS";
										monedatab = "Dólares Ajustados";
										simbolo = "$";
										estilomoneda = monedaDolares;
										c.PensionCia = c.PensionCiaMO;
									}
									else if (c.Moneda.Id == Enums.Moneda.Soles.StringValue())
									{
										tasa = ipc / 100.00;
										moneda = "SOLES INDEXADOS";
										monedatab = "Soles Indexados";
										simbolo = "S/.";
										estilomoneda = monedaSoles;
									}
									else if (c.Moneda.Id == Enums.Moneda.Dolares.StringValue())
									{
										tasa = 1;
										moneda = "DÓLARES";
										monedatab = "Dólares";
										simbolo = "$";
										estilomoneda = monedaDolares;
										c.PensionCia = c.PensionCiaMO;
									}

									sh = (HSSFSheet)wb.CreateSheet(monedatab);

									// Ancho de columnas
									sh.SetColumnWidth(0, 2800);
									sh.SetColumnWidth(1, 3200);
									sh.SetColumnWidth(2, 3600);
									sh.SetColumnWidth(3, 3000);
									sh.SetColumnWidth(4, 2800);
									sh.SetColumnWidth(5, 3200);
									sh.SetColumnWidth(6, 3600);
									sh.SetColumnWidth(7, 3000);
									sh.SetColumnWidth(8, 2800);
									sh.SetColumnWidth(9, 3200);
									sh.SetColumnWidth(10, 3600);
									sh.SetColumnWidth(11, 3000);

									// Formatos de celda
									ICellStyle titulo = wb.CreateCellStyle();
									titulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.Alignment = HorizontalAlignment.Left;
									titulo.VerticalAlignment = VerticalAlignment.Center;
									titulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
									titulo.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodato = wb.CreateCellStyle();
									titulodato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.Alignment = HorizontalAlignment.Left;
									titulodato.VerticalAlignment = VerticalAlignment.Center;
									titulodato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodato.FillPattern = FillPattern.SolidForeground;

									ICellStyle totaltitulo = wb.CreateCellStyle();
									totaltitulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.Alignment = HorizontalAlignment.Left;
									totaltitulo.VerticalAlignment = VerticalAlignment.Center;
									totaltitulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
									totaltitulo.FillPattern = FillPattern.SolidForeground;

									ICellStyle totaldatosoles = wb.CreateCellStyle();
									totaldatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.Alignment = HorizontalAlignment.Left;
									totaldatosoles.VerticalAlignment = VerticalAlignment.Center;
									totaldatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									totaldatosoles.FillPattern = FillPattern.SolidForeground;
									totaldatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

									ICellStyle totaldato = wb.CreateCellStyle();
									totaldato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.Alignment = HorizontalAlignment.Left;
									totaldato.VerticalAlignment = VerticalAlignment.Center;
									totaldato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									totaldato.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodatomoneda = wb.CreateCellStyle();
									titulodatomoneda.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.Alignment = HorizontalAlignment.Left;
									titulodatomoneda.VerticalAlignment = VerticalAlignment.Center;
									titulodatomoneda.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodatomoneda.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodatosoles = wb.CreateCellStyle();
									titulodatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.Alignment = HorizontalAlignment.Left;
									titulodatosoles.VerticalAlignment = VerticalAlignment.Center;
									titulodatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodatosoles.FillPattern = FillPattern.SolidForeground;
									titulodatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

									ICellStyle cabecera = wb.CreateCellStyle();
									cabecera.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.Alignment = HorizontalAlignment.Center;
									cabecera.FillForegroundColor = IndexedColors.SkyBlue.Index;
									cabecera.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho1 = wb.CreateCellStyle();
									detalleanho1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.Alignment = HorizontalAlignment.Center;
									detalleanho1.VerticalAlignment = VerticalAlignment.Center;
									detalleanho1.FillForegroundColor = IndexedColors.White.Index;
									detalleanho1.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho2 = wb.CreateCellStyle();
									detalleanho2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho2.Alignment = HorizontalAlignment.Center;
									detalleanho2.VerticalAlignment = VerticalAlignment.Center;
									detalleanho2.FillForegroundColor = IndexedColors.White.Index;
									detalleanho2.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho3 = wb.CreateCellStyle();
									detalleanho3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.Alignment = HorizontalAlignment.Center;
									detalleanho3.VerticalAlignment = VerticalAlignment.Center;
									detalleanho3.FillForegroundColor = IndexedColors.White.Index;
									detalleanho3.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalle = wb.CreateCellStyle();
									detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.Alignment = HorizontalAlignment.Center;
									detalle.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle detalleanhodiferida1 = wb.CreateCellStyle();
									detalleanhodiferida1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida1.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida1.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida1.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhodiferida2 = wb.CreateCellStyle();
									detalleanhodiferida2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida2.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida2.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida2.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida2.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhodiferida3 = wb.CreateCellStyle();
									detalleanhodiferida3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida3.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida3.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida3.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallediferida = wb.CreateCellStyle();
									detallediferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.Alignment = HorizontalAlignment.Center;
									detallediferida.VerticalAlignment = VerticalAlignment.Center;
									detallediferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detallediferida.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhoprimerpago = wb.CreateCellStyle();
									detalleanhoprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.Alignment = HorizontalAlignment.Center;
									detalleanhoprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detalleanhoprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detalleanhoprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleprimerpago = wb.CreateCellStyle();
									detalleprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.Alignment = HorizontalAlignment.Center;
									detalleprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detalleprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detalleprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepension = wb.CreateCellStyle();
									detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.Alignment = HorizontalAlignment.Right;
									detallepension.VerticalAlignment = VerticalAlignment.Center;
									detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									ICellStyle detallepensiondiferida = wb.CreateCellStyle();
									detallepensiondiferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.Alignment = HorizontalAlignment.Right;
									detallepensiondiferida.VerticalAlignment = VerticalAlignment.Center;
									detallepensiondiferida.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensiondiferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detallepensiondiferida.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensionprimerpago = wb.CreateCellStyle();
									detallepensionprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.Alignment = HorizontalAlignment.Right;
									detallepensionprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detallepensionprimerpago.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensionprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detallepensionprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensionacumulada = wb.CreateCellStyle();
									detallepensionacumulada.Alignment = HorizontalAlignment.Right;
									detallepensionacumulada.VerticalAlignment = VerticalAlignment.Center;
									detallepensionacumulada.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									IFont negrita = wb.CreateFont();
									negrita.FontName = "Arial";
									negrita.Boldweight = (short)FontBoldWeight.Bold;
									titulo.SetFont(negrita);
									cabecera.SetFont(negrita);
									detallepensionacumulada.SetFont(negrita);
									detalleanhoprimerpago.SetFont(negrita);
									detalleanho1.SetFont(negrita);
									detalleanhodiferida1.SetFont(negrita);

									IFont negrita12 = wb.CreateFont();
									negrita12.FontName = "Arial";
									negrita12.Boldweight = (short)FontBoldWeight.Bold;
									negrita12.FontHeightInPoints = 12;
									totaltitulo.SetFont(negrita12);
									totaldato.SetFont(negrita12);
									totaldatosoles.SetFont(negrita12);

									totaldato.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
									titulodatomoneda.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
									tasatrimestral = Math.Pow(1 + tasa, 0.25) - 1;

									GrupoFamiliar afiliado = beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

									fila = 0;
									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("CLIENTE");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(afiliado.Nombre + " " + afiliado.ApellidoPaterno + " " + afiliado.ApellidoMaterno);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("FONDO CIC");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(cic, 2));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
									fila++;

									if (c.PeriodoDiferido > 0)
									{
										r = sh.CreateRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
										sh.AddMergedRegion(cra);
										r.CreateCell(0);
										r.CreateCell(1);
										r.CreateCell(2);
										sh.GetRow(fila).GetCell(0).SetCellValue("RENTA TEMPORAL");
										sh.GetRow(fila).GetCell(0).CellStyle = titulo;
										sh.GetRow(fila).GetCell(1).CellStyle = titulo;
										sh.GetRow(fila).GetCell(2).CellStyle = titulo;
										sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
										sh.AddMergedRegion(cra);
										r.CreateCell(3);
										r.CreateCell(4);
										r.CreateCell(5);
										r.CreateCell(6);
										sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PensionAFP, 2));
										sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
										sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
										sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
										sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
										fila++;

										r = sh.CreateRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
										sh.AddMergedRegion(cra);
										r.CreateCell(0);
										r.CreateCell(1);
										r.CreateCell(2);
										sh.GetRow(fila).GetCell(0).SetCellValue("MODALIDAD DIFERIDA");
										sh.GetRow(fila).GetCell(0).CellStyle = titulo;
										sh.GetRow(fila).GetCell(1).CellStyle = titulo;
										sh.GetRow(fila).GetCell(2).CellStyle = titulo;
										sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
										sh.AddMergedRegion(cra);
										r.CreateCell(3);
										r.CreateCell(4);
										r.CreateCell(5);
										r.CreateCell(6);
										sh.GetRow(fila).GetCell(3).SetCellValue("A " + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty));
										sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
										fila++;
									}

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("PENSIÓN");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PensionCia, 2));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodatomoneda;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("MONEDA");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(moneda + ((c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue() || c.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) ? (" AL " + tasa * 100 + "%") : String.Empty));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE ANUAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(tasa);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE TRIMESTRAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(tasatrimestral);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									r = sh.CreateRow(fila);
									r.CreateCell(0);
									sh.GetRow(fila).GetCell(0).SetCellValue("Año");
									sh.GetRow(fila).GetCell(0).CellStyle = cabecera;
									r.CreateCell(1);
									sh.GetRow(fila).GetCell(1).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(1).CellStyle = cabecera;
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(2).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(2).CellStyle = cabecera;

									r.CreateCell(4);
									sh.GetRow(fila).GetCell(4).SetCellValue("Año");
									sh.GetRow(fila).GetCell(4).CellStyle = cabecera;
									r.CreateCell(5);
									sh.GetRow(fila).GetCell(5).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(5).CellStyle = cabecera;
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(6).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(6).CellStyle = cabecera;

									r.CreateCell(8);
									sh.GetRow(fila).GetCell(8).SetCellValue("Año");
									sh.GetRow(fila).GetCell(8).CellStyle = cabecera;
									r.CreateCell(9);
									sh.GetRow(fila).GetCell(9).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(9).CellStyle = cabecera;
									r.CreateCell(10);
									sh.GetRow(fila).GetCell(10).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(10).CellStyle = cabecera;

									fila++;

									double pension = Math.Round(c.PensionCia, 2);
									int anho = 1;
									int mes = 1;
									bool primerpago;
									ICellStyle estilodetalleanho1 = detalleanhodiferida1;
									ICellStyle estilodetalleanho2 = detalleanhodiferida2;
									ICellStyle estilodetalleanho3 = detalleanhodiferida3;
									ICellStyle estilodetalle = detallediferida;
									ICellStyle estilodetallepension = detallepensiondiferida;
									double acumuladoanho = 0;
									double acumuladodiferido = 0;
									double acumuladogarantizado = 0;
									int contador = 0;
									int anhosgarantizados = c.PeriodoGarantizado;
									if (c.PeriodoDiferido == 0)
									{
										estilodetalleanho1 = detalleanhoprimerpago;
										estilodetalleanho2 = detalleanho2;
										estilodetalleanho3 = detalleanho3;
										estilodetalle = detalleprimerpago;
										estilodetallepension = detallepensionprimerpago;
										primerpago = true;
									}
									else
									{
										estilodetalleanho1 = detalleanhodiferida1;
										estilodetalleanho2 = detalleanhodiferida2;
										estilodetalleanho3 = detalleanhodiferida3;
										estilodetalle = detallediferida;
										estilodetallepension = detallepensiondiferida;
										primerpago = false;
									}
									// Bucle pensión proyectada - Columna 1
									for (int i = fila; i < fila + 36; i++)
									{
										r = sh.CreateRow(i);
										r.CreateCell(0);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(0).SetCellValue(anho);
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(1);
										sh.GetRow(i).GetCell(1).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(1).CellStyle = estilodetalle;
										r.CreateCell(2);
										sh.GetRow(i).GetCell(2).SetCellValue(Math.Round(pension, 2));
										sh.GetRow(i).GetCell(2).CellStyle = estilodetallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												acumuladoanho = 0;
												acumuladodiferido += (c.PensionAFP * 12);
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												acumuladodiferido += (c.PensionAFP * 12);
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(3);
												sh.GetRow(i).GetCell(3).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(3).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									// Bucle pensión proyectada - Columna 2
									for (int i = fila; i < fila + 36; i++)
									{
										r = sh.GetRow(i);
										r.CreateCell(4);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(4).SetCellValue(anho);
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(5);
										sh.GetRow(i).GetCell(5).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(5).CellStyle = estilodetalle;
										r.CreateCell(6);
										sh.GetRow(i).GetCell(6).SetCellValue(Math.Round(pension, 2));
										sh.GetRow(i).GetCell(6).CellStyle = estilodetallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												acumuladoanho = 0;
												acumuladodiferido += (c.PensionAFP * 12);
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												acumuladodiferido += (c.PensionAFP * 12);
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(7);
												sh.GetRow(i).GetCell(7).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(7).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									// Bucle pensión proyectada - Columna 3
									for (int i = fila; i < fila + 28; i++)
									{
										r = sh.GetRow(i);
										r.CreateCell(8);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(8).SetCellValue(anho);
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(9);
										sh.GetRow(i).GetCell(9).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(9).CellStyle = detalle;
										r.CreateCell(10);
										sh.GetRow(i).GetCell(10).SetCellValue(Math.Round(pension, 2));
										sh.GetRow(i).GetCell(10).CellStyle = detallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												acumuladoanho = 0;
												acumuladodiferido += (c.PensionAFP * 12);
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												acumuladodiferido += (c.PensionAFP * 12);
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(11);
												sh.GetRow(i).GetCell(11).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(11).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									if (acumuladodiferido > 0 || acumuladogarantizado > 0)
									{
										fila += 36;
										r = sh.CreateRow(fila);

										if (acumuladodiferido > 0)
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL RENTA TEMPORAL (" + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty) + ")");
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldatosoles;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldatosoles;
											fila++;
											r = sh.CreateRow(fila);
										}

										if (acumuladogarantizado > 0)
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL PERÍODO GARANTIZADO (" + c.PeriodoGarantizado + " AÑO" + (c.PeriodoGarantizado != 1 ? "S" : String.Empty) + ")");
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladogarantizado);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
											fila++;
											r = sh.CreateRow(fila);
										}

										if (acumuladodiferido > 0 && acumuladogarantizado > 0 && (c.Moneda.Id == Enums.Moneda.Soles.StringValue() || c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()))
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL " + (c.PeriodoGarantizado + c.PeriodoDiferido) + " PRIMEROS AÑO" + (c.PeriodoGarantizado + c.PeriodoDiferido != 1 ? "S" : String.Empty));
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido + acumuladogarantizado);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
										}
									}
								}
							}

							// HOJA: RESUMEN PENSIÓN ACUMUMLADA

							int comparaciones = 0;
							if (contadorCotizaciones == 1) comparaciones = 0;
							if (contadorCotizaciones == 2) comparaciones = 1;
							if (contadorCotizaciones == 3) comparaciones = 3;

							if (comparaciones > 0)
							{
								// Crear Hoja
								sh = (HSSFSheet)wb.CreateSheet("Resumen Pensión Acumulada");

								// Estilos
								ICellStyle titulo = wb.CreateCellStyle();
								titulo.Alignment = HorizontalAlignment.Center;
								titulo.VerticalAlignment = VerticalAlignment.Center;

								ICellStyle tituloturquesa = wb.CreateCellStyle();
								tituloturquesa.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.Alignment = HorizontalAlignment.Center;
								tituloturquesa.VerticalAlignment = VerticalAlignment.Center;
								tituloturquesa.FillForegroundColor = IndexedColors.SkyBlue.Index;
								tituloturquesa.FillPattern = FillPattern.SolidForeground;
								tituloturquesa.WrapText = true;

								ICellStyle tituloamarillo = wb.CreateCellStyle();
								tituloamarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloamarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloamarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloamarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloamarillo.Alignment = HorizontalAlignment.Center;
								tituloamarillo.VerticalAlignment = VerticalAlignment.Center;
								tituloamarillo.FillForegroundColor = IndexedColors.Yellow.Index;
								tituloamarillo.FillPattern = FillPattern.SolidForeground;
								tituloamarillo.WrapText = true;

								ICellStyle detalle = wb.CreateCellStyle();
								detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.Alignment = HorizontalAlignment.Center;
								detalle.VerticalAlignment = VerticalAlignment.Center;

								ICellStyle detalleamarillo = wb.CreateCellStyle();
								detalleamarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detalleamarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detalleamarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detalleamarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detalleamarillo.Alignment = HorizontalAlignment.Center;
								detalleamarillo.VerticalAlignment = VerticalAlignment.Center;
								detalleamarillo.FillForegroundColor = IndexedColors.Yellow.Index;
								detalleamarillo.FillPattern = FillPattern.SolidForeground;

								ICellStyle detalletotal = wb.CreateCellStyle();
								detalletotal.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detalletotal.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detalletotal.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detalletotal.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detalletotal.Alignment = HorizontalAlignment.Center;
								detalletotal.VerticalAlignment = VerticalAlignment.Center;
								detalletotal.FillForegroundColor = IndexedColors.Yellow.Index;
								detalletotal.FillPattern = FillPattern.SolidForeground;

								ICellStyle detallepension = wb.CreateCellStyle();
								detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.Alignment = HorizontalAlignment.Right;
								detallepension.VerticalAlignment = VerticalAlignment.Center;
								detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

								ICellStyle detallepensionamarillo = wb.CreateCellStyle();
								detallepensionamarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensionamarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensionamarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensionamarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensionamarillo.Alignment = HorizontalAlignment.Right;
								detallepensionamarillo.VerticalAlignment = VerticalAlignment.Center;
								detallepensionamarillo.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
								detallepensionamarillo.FillForegroundColor = IndexedColors.Yellow.Index;
								detallepensionamarillo.FillPattern = FillPattern.SolidForeground;

								ICellStyle detallepensiontotal = wb.CreateCellStyle();
								detallepensiontotal.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensiontotal.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensiontotal.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensiontotal.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepensiontotal.Alignment = HorizontalAlignment.Right;
								detallepensiontotal.VerticalAlignment = VerticalAlignment.Center;
								detallepensiontotal.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
								detallepensiontotal.FillForegroundColor = IndexedColors.Yellow.Index;
								detallepensiontotal.FillPattern = FillPattern.SolidForeground;

								ICellStyle amarillo = wb.CreateCellStyle();
								amarillo.Alignment = HorizontalAlignment.Left;
								amarillo.VerticalAlignment = VerticalAlignment.Center;
								amarillo.FillForegroundColor = IndexedColors.Yellow.Index;
								amarillo.FillPattern = FillPattern.SolidForeground;

								IFont negrita = wb.CreateFont();
								negrita.FontName = "Arial";
								negrita.Boldweight = (short)FontBoldWeight.Bold;
								titulo.SetFont(negrita);
								tituloturquesa.SetFont(negrita);
								tituloamarillo.SetFont(negrita);
								detalletotal.SetFont(negrita);
								detallepensiontotal.SetFont(negrita);
								amarillo.SetFont(negrita);

								// Hallar el número de columnas
								bool hayDolares;
								if (correlativo3 > 0)
								{
									hayDolares = true;
								}
								else
								{
									hayDolares = false;
								}

								int nroColumnas = 1 + contadorCotizaciones + (hayDolares ? 2 : 0) + comparaciones;

								fila = 0;
								r = sh.CreateRow(fila);
								cra = new CellRangeAddress(fila, fila, 0, nroColumnas - 1);
								sh.AddMergedRegion(cra);
								for (int i = 0; i < comparaciones; i++)
								{
									r.CreateCell(i);
								}
								sh.GetRow(fila).GetCell(0).SetCellValue("COMPARATIVO DE PENSIÓN ACUMULADA");
								sh.GetRow(fila).GetCell(0).CellStyle = titulo;
								fila++;

								r = sh.CreateRow(fila);
								fila++;

								r = sh.CreateRow(fila);
								cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
								sh.AddMergedRegion(cra);
								r.CreateCell(0);
								r.CreateCell(1);
								r.CreateCell(2);
								r.CreateCell(3);
								string nombreModalidad = String.Empty;
								switch (modalidad)
								{
									case "I":
										nombreModalidad = "MODALIDAD INMEDIATA";
										break;
									case "D1":
										nombreModalidad = "MODALIDAD DIFERIDA 1 AÑO";
										break;
									case "D2":
										nombreModalidad = "MODALIDAD DIFERIDA 2 AÑOS";
										break;
									case "D3":
										nombreModalidad = "MODALIDAD DIFERIDA 3 AÑOS";
										break;
									case "D4":
										nombreModalidad = "MODALIDAD DIFERIDA 4 AÑOS";
										break;
									case "D5":
										nombreModalidad = "MODALIDAD DIFERIDA 5 AÑOS";
										break;
								}
								sh.GetRow(fila).GetCell(0).SetCellValue(nombreModalidad);
								fila++;

								r = sh.CreateRow(fila);
								fila++;

								// Cabeceras
								r = sh.CreateRow(fila);
								for (int i = 0; i < nroColumnas; i++)
								{
									r.CreateCell(i);
								}
								int columna = 0;
								sh.SetColumnWidth(columna, 4000);
								sh.GetRow(fila).GetCell(columna).SetCellValue("Pensión Acumulada Años");
								sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
								columna++;
								if (correlativo1 > 0)
								{
									sh.SetColumnWidth(columna, 5000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Indexados");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
								}
								if (correlativo2 > 0)
								{
									sh.SetColumnWidth(columna, 5000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Ajustados");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
								}
								if (correlativo3 > 0)
								{
									sh.SetColumnWidth(columna, 5000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares Ajustados");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
									sh.SetColumnWidth(columna, 4500);
									sh.GetRow(fila).GetCell(columna).SetCellValue("TC (" + (ajusteTC >= 0 ? "+" : "-") + " " + ajusteTC + "% anual)");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
									sh.SetColumnWidth(columna, 5000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares al TC");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
									columna++;
								}
								if (correlativo1 > 0 && correlativo3 > 0)
								{
									sh.SetColumnWidth(columna, 3000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("$ Ajust. vs S/. Index.");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
									columna++;
								}
								if (correlativo1 > 0 && correlativo2 > 0)
								{
									sh.SetColumnWidth(columna, 3000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("$ Ajust. vs S/. Index.");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
									columna++;
								}
								if (correlativo2 > 0 && correlativo3 > 0)
								{
									sh.SetColumnWidth(columna, 3000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("$ Ajust. vs S/. Ajust.");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
									columna++;
								}
								columna = 0;
								fila++;

								// Columna: Pensión Acumulada Años
								int filaPagoRV = -1;
								int nroAnhosDiferidos = 0;
								switch (modalidad)
								{
									case "I":
										nroAnhosDiferidos = 0;
										break;
									case "D1":
										nroAnhosDiferidos = 1;
										break;
									case "D2":
										nroAnhosDiferidos = 2;
										break;
									case "D3":
										nroAnhosDiferidos = 3;
										break;
									case "D4":
										nroAnhosDiferidos = 4;
										break;
									case "D5":
										nroAnhosDiferidos = 5;
										break;
								}

								int diferidos = nroAnhosDiferidos;
								string cadena = String.Empty;
								for (int i = 1; i <= 25; i++)
								{
									cadena = i.ToString();
									if (diferidos > 0)
									{
										cadena += " Temporal AFP";
										diferidos--;
									}
									else if (diferidos == 0)
									{
										filaPagoRV = fila;
										diferidos--;
									}
									r = sh.CreateRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(cadena);
									if (fila != filaPagoRV)
										sh.GetRow(fila).GetCell(columna).CellStyle = detalle;
									else
										sh.GetRow(fila).GetCell(columna).CellStyle = detalleamarillo;
									fila++;
								}
								r = sh.CreateRow(fila);
								r.CreateCell(columna);
								sh.GetRow(fila).GetCell(columna).SetCellValue("Total");
								sh.GetRow(fila).GetCell(columna).CellStyle = detalletotal;
								columna++;
								fila -= 25;

								// Columna: Soles Indexados
								double monto = 0;
								double[] solesIndexados = new double[26];
								diferidos = nroAnhosDiferidos;
								if (correlativo1 > 0)
								{
									double pensionCiaSolesInd = cotizacion[0].PensionCia;
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											monto = pensionCiaSolesInd;
											if (!cotizacion[0].Gratificacion)
											{
												solesIndexados[i] = monto * 12;
											}
											else
											{
												solesIndexados[i] = monto * 14;
											}
											pensionCiaSolesInd *= (1 + ipc / 100.00);
										}
										else
										{
											solesIndexados[i] = cotizacion[0].PensionAFP * 12;
											diferidos--;
											pensionCiaSolesInd *= (1 + ipc / 100.00);
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(solesIndexados[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(solesIndexados.Sum());
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: Soles Ajustados
								monto = 0;
								double[] solesAjustados = new double[26];
								diferidos = nroAnhosDiferidos;
								if (correlativo2 > 0)
								{
									double pensionCiaSolesAju = cotizacion[1].PensionCia;
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											monto = pensionCiaSolesAju;
											if (!cotizacion[0].Gratificacion)
											{
												solesAjustados[i] = monto * 12;
											}
											else
											{
												solesAjustados[i] = monto * 14;
											}
											pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
										}
										else
										{
											solesAjustados[i] = cotizacion[1].PensionAFP * 12;
											diferidos--;
											pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(solesAjustados[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(solesAjustados.Sum());
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: Dólares Ajustados
								monto = 0;
								double[] dolaresAjustados = new double[26];
								diferidos = nroAnhosDiferidos;
								if (correlativo3 > 0)
								{
									double pensionCiaDolaresAju = cotizacion[2].PensionCiaMO;
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											monto = pensionCiaDolaresAju;
											if (!cotizacion[2].Gratificacion)
											{
												dolaresAjustados[i] = monto * 12;
											}
											else
											{
												dolaresAjustados[i] = monto * 12;
											}
											pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
										}
										else
										{
											dolaresAjustados[i] = cotizacion[2].PensionAFP * 12;
											diferidos--;
											pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresAjustados[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: TC
								double[] tc = new double[26];
								tc[0] = tipoCambio;
								if (correlativo3 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										tc[i] = tc[i - 1] * (1 + ajusteTC/100);
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(tc[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: Dólares al TC
								double[] dolaresTC = new double[26];
								diferidos = nroAnhosDiferidos;
								if (correlativo3 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											dolaresTC[i] = dolaresAjustados[i] * tc[i];
										}
										else
										{
											dolaresTC[i] = dolaresAjustados[i];
											diferidos--;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresTC[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresTC.Sum());
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: $ Ajust vs S/. Index
								double[] vs1 = new double[26];
								if (correlativo1 > 0 && correlativo3 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										vs1[i] = dolaresTC[i] - solesIndexados[i];
										sh.GetRow(fila).GetCell(columna).SetCellValue(vs1[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(vs1.Sum());
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: S/. Ajust vs S/. Index
								double[] vs2 = new double[26];
								if (correlativo1 > 0 && correlativo2 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										vs2[i] = solesAjustados[i] - solesIndexados[i];
										sh.GetRow(fila).GetCell(columna).SetCellValue(vs2[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(vs2.Sum());
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								// Columna: $ Ajust vs S/. Ajust
								double[] vs3 = new double[26];
								if (correlativo2 > 0 && correlativo3 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										vs3[i] = dolaresTC[i] - solesAjustados[i];
										sh.GetRow(fila).GetCell(columna).SetCellValue(vs3[i]);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
										fila++;
									}
									r = sh.GetRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(vs3.Sum());
									sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
									columna++;
									fila -= 25;
								}

								r = sh.GetRow(filaPagoRV);
								cra = new NPOI.SS.Util.CellRangeAddress(filaPagoRV, filaPagoRV, columna, columna + 3);
								sh.AddMergedRegion(cra);
								r.CreateCell(columna);
								r.CreateCell(columna + 1);
								r.CreateCell(columna + 2);
								r.CreateCell(columna + 3);
								sh.GetRow(filaPagoRV).GetCell(columna).SetCellValue("Aquí empieza la pensión vitalicia");
								sh.GetRow(filaPagoRV).GetCell(columna).CellStyle = amarillo;
								sh.GetRow(filaPagoRV).GetCell(columna + 1).CellStyle = amarillo;
								sh.GetRow(filaPagoRV).GetCell(columna + 2).CellStyle = amarillo;
								sh.GetRow(filaPagoRV).GetCell(columna + 3).CellStyle = amarillo;
							}

							// HOJA: COMPARATIVO DE PENSIÓN ANUAL
							if (comparaciones > 0)
							{
								// Crear Hoja
								sh = (HSSFSheet)wb.CreateSheet("Comparativo de Pensión Anual");

								// Estilos
								ICellStyle titulo = wb.CreateCellStyle();
								titulo.Alignment = HorizontalAlignment.Center;
								titulo.VerticalAlignment = VerticalAlignment.Center;

								ICellStyle tituloturquesa = wb.CreateCellStyle();
								tituloturquesa.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								tituloturquesa.Alignment = HorizontalAlignment.Center;
								tituloturquesa.VerticalAlignment = VerticalAlignment.Center;
								tituloturquesa.FillForegroundColor = IndexedColors.SkyBlue.Index;
								tituloturquesa.FillPattern = FillPattern.SolidForeground;
								tituloturquesa.WrapText = true;

								ICellStyle detalle = wb.CreateCellStyle();
								detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detalle.Alignment = HorizontalAlignment.Center;
								detalle.VerticalAlignment = VerticalAlignment.Center;

								ICellStyle detallepension = wb.CreateCellStyle();
								detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
								detallepension.Alignment = HorizontalAlignment.Right;
								detallepension.VerticalAlignment = VerticalAlignment.Center;
								detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

								ICellStyle amarillo = wb.CreateCellStyle();
								amarillo.Alignment = HorizontalAlignment.Left;
								amarillo.VerticalAlignment = VerticalAlignment.Center;
								amarillo.FillForegroundColor = IndexedColors.Yellow.Index;
								amarillo.FillPattern = FillPattern.SolidForeground;

								IFont negrita = wb.CreateFont();
								negrita.FontName = "Arial";
								negrita.Boldweight = (short)FontBoldWeight.Bold;
								titulo.SetFont(negrita);
								tituloturquesa.SetFont(negrita);
								amarillo.SetFont(negrita);

								// Hallar el número de columnas
								bool hayDolares;
								if (correlativo3 > 0)
								{
									hayDolares = true;
								}
								else
								{
									hayDolares = false;
								}

								int nroColumnas = 1 + (contadorCotizaciones * 2) + (hayDolares ? 1 : 0);

								fila = 0;
								r = sh.CreateRow(fila);
								cra = new CellRangeAddress(fila, fila, 0, nroColumnas - 1);
								sh.AddMergedRegion(cra);
								for (int i = 0; i < comparaciones; i++)
								{
									r.CreateCell(i);
								}
								sh.GetRow(fila).GetCell(0).SetCellValue("COMPARATIVO DE PENSIÓN ANUAL");
								sh.GetRow(fila).GetCell(0).CellStyle = titulo;
								fila++;

								r = sh.CreateRow(fila);
								fila++;

								r = sh.CreateRow(fila);
								cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
								sh.AddMergedRegion(cra);
								r.CreateCell(0);
								r.CreateCell(1);
								r.CreateCell(2);
								r.CreateCell(3);
								string nombreModalidad = String.Empty;
								switch (modalidad)
								{
									case "I":
										nombreModalidad = "MODALIDAD INMEDIATA";
										break;
									case "D1":
										nombreModalidad = "MODALIDAD DIFERIDA 1 AÑO";
										break;
									case "D2":
										nombreModalidad = "MODALIDAD DIFERIDA 2 AÑOS";
										break;
									case "D3":
										nombreModalidad = "MODALIDAD DIFERIDA 3 AÑOS";
										break;
									case "D4":
										nombreModalidad = "MODALIDAD DIFERIDA 4 AÑOS";
										break;
									case "D5":
										nombreModalidad = "MODALIDAD DIFERIDA 5 AÑOS";
										break;
								}
								sh.GetRow(fila).GetCell(0).SetCellValue(nombreModalidad);
								fila++;

								r = sh.CreateRow(fila);
								fila++;

								// Cabeceras
								r = sh.CreateRow(fila);
								for (int i = 0; i < nroColumnas; i++)
								{
									r.CreateCell(i);
								}
								int columna = 0;
								sh.SetColumnWidth(columna, 3500);
								sh.GetRow(fila).GetCell(columna).SetCellValue("Pensión Años");
								sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
								columna++;
								if (correlativo1 > 0)
								{
									sh.SetColumnWidth(columna, 3000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Indexados");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;

									sh.SetColumnWidth(columna, 1800);
									sh.GetRow(fila).GetCell(columna).SetCellValue(ipc.ToString() + "%");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
								}
								if (correlativo2 > 0)
								{
									sh.SetColumnWidth(columna, 3000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Ajustados");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;

									sh.SetColumnWidth(columna, 1800);
									sh.GetRow(fila).GetCell(columna).SetCellValue(ajusteTasa.ToString() + "%");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
								}
								if (correlativo3 > 0)
								{
									sh.SetColumnWidth(columna, 3000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares Ajustados");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;

									sh.SetColumnWidth(columna, 1800);
									sh.GetRow(fila).GetCell(columna).SetCellValue(ajusteTasa.ToString() + "%");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;

									sh.SetColumnWidth(columna, 4800);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares al TC");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
								}
								columna = 0;
								fila++;

								// Columna: Pensión Años
								int filaPagoRV = -1;
								int nroAnhosDiferidos = 0;
								switch (modalidad)
								{
									case "I":
										nroAnhosDiferidos = 0;
										break;
									case "D1":
										nroAnhosDiferidos = 1;
										break;
									case "D2":
										nroAnhosDiferidos = 2;
										break;
									case "D3":
										nroAnhosDiferidos = 3;
										break;
									case "D4":
										nroAnhosDiferidos = 4;
										break;
									case "D5":
										nroAnhosDiferidos = 5;
										break;
								}

								int diferidos = nroAnhosDiferidos;
								string cadena = String.Empty;
								for (int i = 1; i <= 25; i++)
								{
									cadena = i.ToString();
									if (diferidos > 0)
									{
										cadena += " Temporal AFP";
										diferidos--;
									}
									else if (diferidos == 0)
									{
										filaPagoRV = fila;
										diferidos--;
									}
									r = sh.CreateRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue(cadena);
									sh.GetRow(fila).GetCell(columna).CellStyle = detalle;
									fila++;
								}
								columna++;
								fila -= 25;

								// Columna: Soles Indexados
								double monto = 0;
								diferidos = nroAnhosDiferidos;
								if (correlativo1 > 0)
								{
									double pensionCiaSolesInd = cotizacion[0].PensionCia;
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											monto = pensionCiaSolesInd;
											pensionCiaSolesInd *= (1 + ipc / 100.00);
										}
										else
										{
											monto = cotizacion[0].PensionAFP;
											diferidos--;
											pensionCiaSolesInd *= (1 + ipc / 100.00);
										}
										r = sh.GetRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, columna, columna + 1);
										sh.AddMergedRegion(cra);
										r.CreateCell(columna);
										r.CreateCell(columna + 1);
										sh.GetRow(fila).GetCell(columna).SetCellValue(monto);
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										sh.GetRow(fila).GetCell(columna + 1).CellStyle = detallepension;
										fila++;
									}
									columna += 2;
									fila -= 25;
								}

								// Columna: Soles Ajustados
								monto = 0;
								diferidos = nroAnhosDiferidos;
								if (correlativo2 > 0)
								{
									double pensionCiaSolesAju = cotizacion[1].PensionCia;
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											monto = pensionCiaSolesAju;
											pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
										}
										else
										{
											monto = cotizacion[1].PensionAFP;
											diferidos--;
											pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
										}
										r = sh.GetRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, columna, columna + 1);
										sh.AddMergedRegion(cra);
										r.CreateCell(columna);
										r.CreateCell(columna + 1);
										sh.GetRow(fila).GetCell(columna).SetCellValue(monto);
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										sh.GetRow(fila).GetCell(columna + 1).CellStyle = detallepension;
										fila++;
									}
									columna += 2;
									fila -= 25;
								}

								// Columna: Dólares Ajustados
								double[] dolaresAjustados = new double[26];
								diferidos = nroAnhosDiferidos;
								if (correlativo3 > 0)
								{
									double pensionCiaDolaresAju = cotizacion[2].PensionCiaMO;
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											dolaresAjustados[i] = pensionCiaDolaresAju;
											pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
										}
										else
										{
											dolaresAjustados[i] = cotizacion[2].PensionAFP;
											diferidos--;
											pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
										}
										r = sh.GetRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, columna, columna + 1);
										sh.AddMergedRegion(cra);
										r.CreateCell(columna);
										r.CreateCell(columna + 1);
										sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresAjustados[i]);
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										sh.GetRow(fila).GetCell(columna + 1).CellStyle = detallepension;
										fila++;
									}
									columna += 2;
									fila -= 25;
								}

								// Tipo de Cambio
								double[] tc = new double[26];
								tc[0] = tipoCambio;
								if (correlativo3 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										tc[i] = tc[i - 1] * (1 + ajusteTC / 100.00);
									}
								}

								// Columna: Dólares al TC
								double[] dolaresTC = new double[26];
								diferidos = nroAnhosDiferidos;
								if (correlativo3 > 0)
								{
									for (int i = 1; i <= 25; i++)
									{
										if (diferidos <= 0)
										{
											dolaresTC[i] = dolaresAjustados[i] * tc[i];
										}
										else
										{
											dolaresTC[i] = dolaresAjustados[i];
											diferidos--;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresTC[i]);
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
										fila++;
									}
									columna++;
									fila -= 25;
								}

								r = sh.GetRow(filaPagoRV);
								cra = new NPOI.SS.Util.CellRangeAddress(filaPagoRV, filaPagoRV, columna, columna + 3);
								sh.AddMergedRegion(cra);
								r.CreateCell(columna);
								r.CreateCell(columna + 1);
								r.CreateCell(columna + 2);
								r.CreateCell(columna + 3);
								sh.GetRow(filaPagoRV).GetCell(columna).SetCellValue("Aquí empieza la pensión vitalicia");
								sh.GetRow(filaPagoRV).GetCell(columna).CellStyle = amarillo;
								sh.GetRow(filaPagoRV).GetCell(columna + 1).CellStyle = amarillo;
								sh.GetRow(filaPagoRV).GetCell(columna + 2).CellStyle = amarillo;
								sh.GetRow(filaPagoRV).GetCell(columna + 3).CellStyle = amarillo;
							}

							HttpContext.Current.Session["ReportePensionProyectada"] = wb;

							respuesta.Estado = Constante.COD_OK;
						}
						else
						{
							//control.PermisoEjecutar = false;
						}
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

		[WebMethod]
		public static Respuesta GenerarPensionProyectada3(string tokenUsuario, List<GrupoFamiliar> beneficiarios, List<Cotizacion> cotizaciones,
														  Int64 correlativo1, Int64 correlativo2, Int64 correlativo3,
														  double cic, double tipoCambio, double rentabilidadAFP, double ipc, double ajusteTasa)
		{
			using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
			{
				Respuesta respuesta = new Respuesta();
				try
				{
					if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
					{
						if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorJubilarseHoy))
						{
							// Declaración de variables
							IRow r;
							int contadorCotizaciones = 0;
							int fila = 0;
							CellRangeAddress cra;

							// Declarar arreglo de cotizaciones
							Cotizacion[] cotizacion = new Cotizacion[3];
							for (int i = 0; i < 3; i++)
							{
								cotizacion[i] = null;
							}

							// Obtener cotizaciones
							if (correlativo1 > 0)
							{
								cotizacion[0] = cotizaciones.Find(c => c.Correlativo == correlativo1);
								contadorCotizaciones++;
							}
							if (correlativo2 > 0)
							{
								cotizacion[1] = cotizaciones.Find(c => c.Correlativo == correlativo2);
								contadorCotizaciones++;
							}
							if (correlativo3 > 0)
							{
								cotizacion[2] = cotizaciones.Find(c => c.Correlativo == correlativo3);
								contadorCotizaciones++;
							}

							// Generar archivo Excel
							HSSFWorkbook wb;

							HSSFSheet sh;

							wb = new HSSFWorkbook();

							foreach (Cotizacion c in cotizacion)
							{
								// Pintar la Hoja Excel únicamente si se ha seleccionado una cotización
								if (c != null)
								{
									// Formatos de celda
									ICellStyle monedaSoles = wb.CreateCellStyle();
									monedaSoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");
									ICellStyle monedaDolares = wb.CreateCellStyle();
									monedaDolares.DataFormat = wb.CreateDataFormat().GetFormat("$ #,###.00");
									ICellStyle porcentaje = wb.CreateCellStyle();
									porcentaje.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									ICellStyle monto = wb.CreateCellStyle();
									monto.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									double tasa = 1;
									double tasatrimestral;
									string moneda = String.Empty;
									string monedatab = String.Empty;
									string simbolo = String.Empty;
									ICellStyle estilomoneda = null;
									if (c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
									{
										tasa = ajusteTasa / 100.00;
										moneda = "SOLES AJUSTADOS";
										monedatab = "Soles Ajustados";
										simbolo = "S/.";
										estilomoneda = monedaSoles;
									}
									else if (c.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue())
									{
										tasa = ajusteTasa / 100.00;
										moneda = "DÓLARES AJUSTADOS";
										monedatab = "Dólares Ajustados";
										simbolo = "$";
										estilomoneda = monedaDolares;
										c.PensionCia = c.PensionCiaMO;
									}
									else if (c.Moneda.Id == Enums.Moneda.Soles.StringValue())
									{
										tasa = ipc / 100.00;
										moneda = "SOLES INDEXADOS";
										monedatab = "Soles Indexados";
										simbolo = "S/.";
										estilomoneda = monedaSoles;
									}
									else if (c.Moneda.Id == Enums.Moneda.Dolares.StringValue())
									{
										tasa = 1;
										moneda = "DÓLARES";
										monedatab = "Dólares";
										simbolo = "$";
										estilomoneda = monedaDolares;
										c.PensionCia = c.PensionCiaMO;
									}

									sh = (HSSFSheet)wb.CreateSheet(monedatab);

									// Ancho de columnas
									sh.SetColumnWidth(0, 2800);
									sh.SetColumnWidth(1, 3200);
									sh.SetColumnWidth(2, 3600);
									sh.SetColumnWidth(3, 3000);
									sh.SetColumnWidth(4, 2800);
									sh.SetColumnWidth(5, 3200);
									sh.SetColumnWidth(6, 3600);
									sh.SetColumnWidth(7, 3000);
									sh.SetColumnWidth(8, 2800);
									sh.SetColumnWidth(9, 3200);
									sh.SetColumnWidth(10, 3600);
									sh.SetColumnWidth(11, 3000);

									// Formatos de celda
									ICellStyle titulo = wb.CreateCellStyle();
									titulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulo.Alignment = HorizontalAlignment.Left;
									titulo.VerticalAlignment = VerticalAlignment.Center;
									titulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
									titulo.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodato = wb.CreateCellStyle();
									titulodato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodato.Alignment = HorizontalAlignment.Left;
									titulodato.VerticalAlignment = VerticalAlignment.Center;
									titulodato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodato.FillPattern = FillPattern.SolidForeground;

									ICellStyle totaltitulo = wb.CreateCellStyle();
									totaltitulo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaltitulo.Alignment = HorizontalAlignment.Left;
									totaltitulo.VerticalAlignment = VerticalAlignment.Center;
									totaltitulo.FillForegroundColor = IndexedColors.SkyBlue.Index;
									totaltitulo.FillPattern = FillPattern.SolidForeground;

									ICellStyle totaldatosoles = wb.CreateCellStyle();
									totaldatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldatosoles.Alignment = HorizontalAlignment.Left;
									totaldatosoles.VerticalAlignment = VerticalAlignment.Center;
									totaldatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									totaldatosoles.FillPattern = FillPattern.SolidForeground;
									totaldatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

									ICellStyle totaldato = wb.CreateCellStyle();
									totaldato.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									totaldato.Alignment = HorizontalAlignment.Left;
									totaldato.VerticalAlignment = VerticalAlignment.Center;
									totaldato.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									totaldato.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodatomoneda = wb.CreateCellStyle();
									titulodatomoneda.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatomoneda.Alignment = HorizontalAlignment.Left;
									titulodatomoneda.VerticalAlignment = VerticalAlignment.Center;
									titulodatomoneda.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodatomoneda.FillPattern = FillPattern.SolidForeground;

									ICellStyle titulodatosoles = wb.CreateCellStyle();
									titulodatosoles.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									titulodatosoles.Alignment = HorizontalAlignment.Left;
									titulodatosoles.VerticalAlignment = VerticalAlignment.Center;
									titulodatosoles.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									titulodatosoles.FillPattern = FillPattern.SolidForeground;
									titulodatosoles.DataFormat = wb.CreateDataFormat().GetFormat("S/. #,###.00");

									ICellStyle cabecera = wb.CreateCellStyle();
									cabecera.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									cabecera.Alignment = HorizontalAlignment.Center;
									cabecera.FillForegroundColor = IndexedColors.SkyBlue.Index;
									cabecera.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho1 = wb.CreateCellStyle();
									detalleanho1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho1.Alignment = HorizontalAlignment.Center;
									detalleanho1.VerticalAlignment = VerticalAlignment.Center;
									detalleanho1.FillForegroundColor = IndexedColors.White.Index;
									detalleanho1.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho2 = wb.CreateCellStyle();
									detalleanho2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho2.Alignment = HorizontalAlignment.Center;
									detalleanho2.VerticalAlignment = VerticalAlignment.Center;
									detalleanho2.FillForegroundColor = IndexedColors.White.Index;
									detalleanho2.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanho3 = wb.CreateCellStyle();
									detalleanho3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanho3.Alignment = HorizontalAlignment.Center;
									detalleanho3.VerticalAlignment = VerticalAlignment.Center;
									detalleanho3.FillForegroundColor = IndexedColors.White.Index;
									detalleanho3.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalle = wb.CreateCellStyle();
									detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.Alignment = HorizontalAlignment.Center;
									detalle.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle detalleanhodiferida1 = wb.CreateCellStyle();
									detalleanhodiferida1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida1.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida1.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida1.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida1.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhodiferida2 = wb.CreateCellStyle();
									detalleanhodiferida2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida2.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida2.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida2.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida2.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhodiferida3 = wb.CreateCellStyle();
									detalleanhodiferida3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhodiferida3.Alignment = HorizontalAlignment.Center;
									detalleanhodiferida3.VerticalAlignment = VerticalAlignment.Center;
									detalleanhodiferida3.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detalleanhodiferida3.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallediferida = wb.CreateCellStyle();
									detallediferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallediferida.Alignment = HorizontalAlignment.Center;
									detallediferida.VerticalAlignment = VerticalAlignment.Center;
									detallediferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detallediferida.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleanhoprimerpago = wb.CreateCellStyle();
									detalleanhoprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleanhoprimerpago.Alignment = HorizontalAlignment.Center;
									detalleanhoprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detalleanhoprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detalleanhoprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalleprimerpago = wb.CreateCellStyle();
									detalleprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleprimerpago.Alignment = HorizontalAlignment.Center;
									detalleprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detalleprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detalleprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepension = wb.CreateCellStyle();
									detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.Alignment = HorizontalAlignment.Right;
									detallepension.VerticalAlignment = VerticalAlignment.Center;
									detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									ICellStyle detallepensiondiferida = wb.CreateCellStyle();
									detallepensiondiferida.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiondiferida.Alignment = HorizontalAlignment.Right;
									detallepensiondiferida.VerticalAlignment = VerticalAlignment.Center;
									detallepensiondiferida.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensiondiferida.FillForegroundColor = IndexedColors.Grey25Percent.Index;
									detallepensiondiferida.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensionprimerpago = wb.CreateCellStyle();
									detallepensionprimerpago.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionprimerpago.Alignment = HorizontalAlignment.Right;
									detallepensionprimerpago.VerticalAlignment = VerticalAlignment.Center;
									detallepensionprimerpago.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensionprimerpago.FillForegroundColor = IndexedColors.LightTurquoise.Index;
									detallepensionprimerpago.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensionacumulada = wb.CreateCellStyle();
									detallepensionacumulada.Alignment = HorizontalAlignment.Right;
									detallepensionacumulada.VerticalAlignment = VerticalAlignment.Center;
									detallepensionacumulada.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									IFont negrita = wb.CreateFont();
									negrita.FontName = "Arial";
									negrita.Boldweight = (short)FontBoldWeight.Bold;
									titulo.SetFont(negrita);
									cabecera.SetFont(negrita);
									detallepensionacumulada.SetFont(negrita);
									detalleanhoprimerpago.SetFont(negrita);
									detalleanho1.SetFont(negrita);
									detalleanhodiferida1.SetFont(negrita);

									IFont negrita12 = wb.CreateFont();
									negrita12.FontName = "Arial";
									negrita12.Boldweight = (short)FontBoldWeight.Bold;
									negrita12.FontHeightInPoints = 12;
									totaltitulo.SetFont(negrita12);
									totaldato.SetFont(negrita12);
									totaldatosoles.SetFont(negrita12);

									totaldato.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
									titulodatomoneda.DataFormat = wb.CreateDataFormat().GetFormat(simbolo + " #,###.00");
									tasatrimestral = Math.Pow(1 + tasa, 0.25) - 1;

									GrupoFamiliar afiliado = beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

									fila = 0;
									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("CLIENTE");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(afiliado.Nombre + " " + afiliado.ApellidoPaterno + " " + afiliado.ApellidoMaterno);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("FONDO CIC");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(cic, 2));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
									fila++;

									
									if (c.PeriodoDiferido > 0)
									{
										r = sh.CreateRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
										sh.AddMergedRegion(cra);
										r.CreateCell(0);
										r.CreateCell(1);
										r.CreateCell(2);
										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											sh.GetRow(fila).GetCell(0).SetCellValue("PENSIÓN 2° TRAMO");
										}
										else
										{
											sh.GetRow(fila).GetCell(0).SetCellValue("RENTA TEMPORAL");
										}
										//<FINGTI_2145>
										
										sh.GetRow(fila).GetCell(0).CellStyle = titulo;
										sh.GetRow(fila).GetCell(1).CellStyle = titulo;
										sh.GetRow(fila).GetCell(2).CellStyle = titulo;
										sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
										sh.AddMergedRegion(cra);
										r.CreateCell(3);
										r.CreateCell(4);
										r.CreateCell(5);
										r.CreateCell(6);
										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PrimeraPensionRVD / (1+tasa), 2));
										}
										else
										{
											sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PensionAFP, 2));
										}
									   
										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											//titulodatomoneda
											sh.GetRow(fila).GetCell(3).CellStyle = titulodatomoneda;
											sh.GetRow(fila).GetCell(4).CellStyle = titulodatomoneda;
											sh.GetRow(fila).GetCell(5).CellStyle = titulodatomoneda;
											sh.GetRow(fila).GetCell(6).CellStyle = titulodatomoneda;
										}
										else
										{
											sh.GetRow(fila).GetCell(3).CellStyle = titulodatosoles;
											sh.GetRow(fila).GetCell(4).CellStyle = titulodatosoles;
											sh.GetRow(fila).GetCell(5).CellStyle = titulodatosoles;
											sh.GetRow(fila).GetCell(6).CellStyle = titulodatosoles;
										}
										//<FINGTI_2145>

										fila++;

										r = sh.CreateRow(fila);
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
										sh.AddMergedRegion(cra);
										r.CreateCell(0);
										r.CreateCell(1);
										r.CreateCell(2);
										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											sh.GetRow(fila).GetCell(0).SetCellValue("MODALIDAD ESCALONADA");
										}
										else 
										{
											sh.GetRow(fila).GetCell(0).SetCellValue("MODALIDAD DIFERIDA");
										}
										//<FINGTI_2145>

										sh.GetRow(fila).GetCell(0).CellStyle = titulo;
										sh.GetRow(fila).GetCell(1).CellStyle = titulo;
										sh.GetRow(fila).GetCell(2).CellStyle = titulo;
										sh.GetRow(fila).GetCell(0).CellStyle.WrapText = true;
										cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
										sh.AddMergedRegion(cra);
										r.CreateCell(3);
										r.CreateCell(4);
										r.CreateCell(5);
										r.CreateCell(6);
										sh.GetRow(fila).GetCell(3).SetCellValue("A " + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty));

										
										sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
										sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
										fila++;
									}


									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("PENSIÓN");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(Math.Round(c.PensionCia, 2));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodatomoneda;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodatomoneda;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("MONEDA");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(moneda + ((c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue() || c.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) ? (" AL " + tasa * 100 + "%") : String.Empty));
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE ANUAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(tasa);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00%");
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 2);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(0).SetCellValue("TASA DE AJUSTE TRIMESTRAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									sh.GetRow(fila).GetCell(1).CellStyle = titulo;
									sh.GetRow(fila).GetCell(2).CellStyle = titulo;
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 3, 6);
									sh.AddMergedRegion(cra);
									r.CreateCell(3);
									r.CreateCell(4);
									r.CreateCell(5);
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(3).SetCellValue(tasatrimestral);
									sh.GetRow(fila).GetCell(3).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(4).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(5).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(6).CellStyle = titulodato;
									sh.GetRow(fila).GetCell(3).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(4).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(5).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									sh.GetRow(fila).GetCell(6).CellStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.0000%");
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									r = sh.CreateRow(fila);
									r.CreateCell(0);
									sh.GetRow(fila).GetCell(0).SetCellValue("Año");
									sh.GetRow(fila).GetCell(0).CellStyle = cabecera;
									r.CreateCell(1);
									sh.GetRow(fila).GetCell(1).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(1).CellStyle = cabecera;
									r.CreateCell(2);
									sh.GetRow(fila).GetCell(2).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(2).CellStyle = cabecera;

									r.CreateCell(4);
									sh.GetRow(fila).GetCell(4).SetCellValue("Año");
									sh.GetRow(fila).GetCell(4).CellStyle = cabecera;
									r.CreateCell(5);
									sh.GetRow(fila).GetCell(5).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(5).CellStyle = cabecera;
									r.CreateCell(6);
									sh.GetRow(fila).GetCell(6).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(6).CellStyle = cabecera;

									r.CreateCell(8);
									sh.GetRow(fila).GetCell(8).SetCellValue("Año");
									sh.GetRow(fila).GetCell(8).CellStyle = cabecera;
									r.CreateCell(9);
									sh.GetRow(fila).GetCell(9).SetCellValue("Mes");
									sh.GetRow(fila).GetCell(9).CellStyle = cabecera;
									r.CreateCell(10);
									sh.GetRow(fila).GetCell(10).SetCellValue("Pensión " + simbolo);
									sh.GetRow(fila).GetCell(10).CellStyle = cabecera;

									fila++;

									double pension = Math.Round(c.PensionCia, 2);
									int anho = 1;
									int mes = 1;
									bool primerpago;
									ICellStyle estilodetalleanho1 = detalleanhodiferida1;
									ICellStyle estilodetalleanho2 = detalleanhodiferida2;
									ICellStyle estilodetalleanho3 = detalleanhodiferida3;
									ICellStyle estilodetalle = detallediferida;
									ICellStyle estilodetallepension = detallepensiondiferida;
									double acumuladoanho = 0;
									double acumuladodiferido = 0;
									double acumuladogarantizado = 0;
									int contador = 0;
									int anhosgarantizados = c.PeriodoGarantizado;
									if (c.PeriodoDiferido == 0)
									{
										estilodetalleanho1 = detalleanhoprimerpago;
										estilodetalleanho2 = detalleanho2;
										estilodetalleanho3 = detalleanho3;
										estilodetalle = detalleprimerpago;
										estilodetallepension = detallepensionprimerpago;
										primerpago = true;
									}
									else
									{
										estilodetalleanho1 = detalleanhodiferida1;
										estilodetalleanho2 = detalleanhodiferida2;
										estilodetalleanho3 = detalleanhodiferida3;
										estilodetalle = detallediferida;
										estilodetallepension = detallepensiondiferida;
										primerpago = false;
									}
									// Bucle pensión proyectada - Columna 1
									for (int i = fila; i < fila + 36; i++)
									{
										r = sh.CreateRow(i);
										r.CreateCell(0);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(0).SetCellValue(anho);
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(0).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(1);
										sh.GetRow(i).GetCell(1).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(1).CellStyle = estilodetalle;
										r.CreateCell(2);
										
										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											if (anho - 1 == c.PeriodoDiferido && mes == 1)
											{
												pension = c.PrimeraPensionRVD;
											}
											else if (anho <= c.PeriodoDiferido)
											{
												acumuladodiferido += (pension * 3);
											}

											sh.GetRow(i).GetCell(2).SetCellValue(Math.Round(pension, 2));
										}
										else
										{
											sh.GetRow(i).GetCell(2).SetCellValue(Math.Round(pension, 2));
										}
										//sh.GetRow(i).GetCell(2).SetCellValue(Math.Round(pension, 2));
										//<FINGTI_2145>

										
										sh.GetRow(i).GetCell(2).CellStyle = estilodetallepension;

										acumuladoanho += (pension * 3);

									   
										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												//<INIGTI_2145>
												if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
												{
													r.CreateCell(3);
													sh.GetRow(i).GetCell(3).SetCellValue(acumuladoanho);
													sh.GetRow(i).GetCell(3).CellStyle = detallepensionacumulada;

													if (anhosgarantizados > 0)
													{
														acumuladogarantizado += acumuladoanho;
														anhosgarantizados--;
													}

												}
												else
												{
													acumuladodiferido += (c.PensionAFP * 12);
												}
												//acumuladodiferido += (c.PensionAFP * 12);
												//<FINGTI_2145>
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;

												//<INIGTI_2145>
												if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
												{
													r.CreateCell(3);
													sh.GetRow(i).GetCell(3).SetCellValue(acumuladoanho);
													sh.GetRow(i).GetCell(3).CellStyle = detallepensionacumulada;

													if (anhosgarantizados > 0)
													{
														acumuladogarantizado += acumuladoanho;
														anhosgarantizados--;
													}
												}
												else
												{
													acumuladodiferido += (c.PensionAFP * 12);
												}
												//acumuladodiferido += (c.PensionAFP * 12);
												//<FINGTI_2145>
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(3);
												sh.GetRow(i).GetCell(3).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(3).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									// Bucle pensión proyectada - Columna 2
									for (int i = fila; i < fila + 36; i++)
									{
										r = sh.GetRow(i);
										r.CreateCell(4);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(4).SetCellValue(anho);
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(4).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(5);
										sh.GetRow(i).GetCell(5).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(5).CellStyle = estilodetalle;
										r.CreateCell(6);
										

										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											if (anho - 1 == c.PeriodoDiferido && mes == 1)
											{
												pension = c.PrimeraPensionRVD;
											}
											else if (anho <= c.PeriodoDiferido)
											{
												acumuladodiferido += (pension * 3);
											}
											sh.GetRow(i).GetCell(6).SetCellValue(Math.Round(pension, 2));
										}
										else
										{
											sh.GetRow(i).GetCell(6).SetCellValue(Math.Round(pension, 2));
										}
										//sh.GetRow(i).GetCell(6).SetCellValue(Math.Round(pension, 2));
										//<FINGTI_2145>

										sh.GetRow(i).GetCell(6).CellStyle = estilodetallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												
												//<INIGTI_2145>
												if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
												{
													r.CreateCell(7);
													sh.GetRow(i).GetCell(7).SetCellValue(acumuladoanho);
													sh.GetRow(i).GetCell(7).CellStyle = detallepensionacumulada;

													if (anhosgarantizados > 0)
													{
														acumuladogarantizado += acumuladoanho;
														anhosgarantizados--;
													}
												}
												else
												{
													acumuladodiferido += (c.PensionAFP * 12);
												}
												//acumuladodiferido += (c.PensionAFP * 12);
												//<FINGTI_2145>                                              
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;

												//<INIGTI_2145>
												if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
												{
													r.CreateCell(7);
													sh.GetRow(i).GetCell(7).SetCellValue(acumuladoanho);
													sh.GetRow(i).GetCell(7).CellStyle = detallepensionacumulada;

													if (anhosgarantizados > 0)
													{
														acumuladogarantizado += acumuladoanho;
														anhosgarantizados--;
													}
												}
												else
												{
													acumuladodiferido += (c.PensionAFP * 12);
												}
												//acumuladodiferido += (c.PensionAFP * 12);
												//<FINGTI_2145> 
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(7);
												sh.GetRow(i).GetCell(7).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(7).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									// Bucle pensión proyectada - Columna 3
									for (int i = fila; i < fila + 28; i++)
									{
										r = sh.GetRow(i);
										r.CreateCell(8);
										if (contador % 4 == 0)
										{
											sh.GetRow(i).GetCell(8).SetCellValue(anho);
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho1;
										}
										else if (contador % 4 == 1 || contador % 4 == 2)
										{
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho2;
										}
										else if (contador % 4 == 3)
										{
											sh.GetRow(i).GetCell(8).CellStyle = estilodetalleanho3;
										}
										r.CreateCell(9);
										sh.GetRow(i).GetCell(9).SetCellValue("Mes" + mes);
										sh.GetRow(i).GetCell(9).CellStyle = detalle;
										r.CreateCell(10);
										

										//<INIGTI_2145>
										if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
										{
											if (anho - 1 == c.PeriodoDiferido && mes == 1)
											{
												pension = c.PrimeraPensionRVD;
											}
											else if (anho <= c.PeriodoDiferido)
											{
												acumuladodiferido += (pension * 3);
											}
											sh.GetRow(i).GetCell(10).SetCellValue(Math.Round(pension, 2));
										}
										else
										{
											sh.GetRow(i).GetCell(10).SetCellValue(Math.Round(pension, 2));
										}
										//sh.GetRow(i).GetCell(10).SetCellValue(Math.Round(pension, 2));
										//<FINGTI_2145>

										sh.GetRow(i).GetCell(10).CellStyle = detallepension;

										acumuladoanho += (pension * 3);

										if (primerpago)
										{
											estilodetalleanho1 = detalleanho1;
											estilodetalleanho2 = detalleanho2;
											estilodetalleanho3 = detalleanho3;
											estilodetalle = detalle;
											estilodetallepension = detallepension;
											primerpago = false;
										}

										mes += 3;
										if (mes > 12)
										{
											if (c.PeriodoDiferido > anho)
											{
												//<INIGTI_2145>
												if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
												{
													r.CreateCell(11);
													sh.GetRow(i).GetCell(11).SetCellValue(acumuladoanho);
													sh.GetRow(i).GetCell(11).CellStyle = detallepensionacumulada;
												}
												else
												{
													acumuladodiferido += (c.PensionAFP * 12);
												}
												//acumuladodiferido += (c.PensionAFP * 12);
												//<FINGTI_2145>
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido == anho)
											{
												primerpago = true;
												estilodetalleanho1 = detalleanhoprimerpago;
												estilodetalleanho2 = detalleanho2;
												estilodetalleanho3 = detalleanho3;
												estilodetalle = detalleprimerpago;
												estilodetallepension = detallepensionprimerpago;
												//<INIGTI_2145>
												if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
												{
													r.CreateCell(11);
													sh.GetRow(i).GetCell(11).SetCellValue(acumuladoanho);
													sh.GetRow(i).GetCell(11).CellStyle = detallepensionacumulada;
												}
												else
												{
													acumuladodiferido += (c.PensionAFP * 12);
												}
												//acumuladodiferido += (c.PensionAFP * 12);
												//<FINGTI_2145>
												acumuladoanho = 0;
											}
											else if (c.PeriodoDiferido < anho)
											{
												r.CreateCell(11);
												sh.GetRow(i).GetCell(11).SetCellValue(acumuladoanho);
												sh.GetRow(i).GetCell(11).CellStyle = detallepensionacumulada;
												if (anhosgarantizados > 0)
												{
													acumuladogarantizado += acumuladoanho;
													anhosgarantizados--;
												}
												acumuladoanho = 0;
											}
											anho++;
											mes = 1;
										}
										pension *= (1 + tasatrimestral);
										//pension = Math.Round(pension, 2);
										contador++;
									}

									if (acumuladodiferido > 0 || acumuladogarantizado > 0)
									{
										fila += 36;
										r = sh.CreateRow(fila);

										if (acumuladodiferido > 0)
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);

											//<INIGTI_2145>
											if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL RENTA 1° TRAMO (" + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty) + ")");
											}
											else
											{
												sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL RENTA TEMPORAL (" + c.PeriodoDiferido + " AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty) + ")");
											}
											//<FINGTI_2145>    
											
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldatosoles;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldatosoles;
											fila++;
											r = sh.CreateRow(fila);
										}

										if (acumuladogarantizado > 0)
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL PERÍODO GARANTIZADO (" + c.PeriodoGarantizado + " AÑO" + (c.PeriodoGarantizado != 1 ? "S" : String.Empty) + ")");
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladogarantizado);
											sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
											fila++;
											r = sh.CreateRow(fila);
										}

										if (acumuladodiferido > 0 && acumuladogarantizado > 0 && (c.Moneda.Id == Enums.Moneda.Soles.StringValue() || c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()))
										{
											fila++;
											r = sh.CreateRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
											sh.AddMergedRegion(cra);
											r.CreateCell(0);
											r.CreateCell(1);
											r.CreateCell(2);
											r.CreateCell(3);

											//<INIGTI_2145>
											if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL " + (c.PeriodoDiferido) + " PRIMEROS AÑO" + (c.PeriodoDiferido != 1 ? "S" : String.Empty));
											}
											else {
											sh.GetRow(fila).GetCell(0).SetCellValue("TOTAL " + (c.PeriodoGarantizado + c.PeriodoDiferido) + " PRIMEROS AÑO" + (c.PeriodoGarantizado + c.PeriodoDiferido != 1 ? "S" : String.Empty));
											}
											

											//<FINGTI_2145>
											sh.GetRow(fila).GetCell(0).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(1).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(2).CellStyle = totaltitulo;
											sh.GetRow(fila).GetCell(3).CellStyle = totaltitulo;
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 4, 5);
											sh.AddMergedRegion(cra);
											r.CreateCell(4);
											r.CreateCell(5);

											//<INIGTI_2145>
											if (c.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido);
											}
											else
											{
											sh.GetRow(fila).GetCell(4).SetCellValue(acumuladodiferido + acumuladogarantizado);
											}

											sh.GetRow(fila).GetCell(4).CellStyle = totaldato;
											sh.GetRow(fila).GetCell(5).CellStyle = totaldato;
										}
									}
								}
							}

							// HOJA: RESUMEN PENSIÓN ACUMUMLADA

							int comparaciones = 0;
							if (contadorCotizaciones == 1) comparaciones = 0;
							if (contadorCotizaciones == 2) comparaciones = 1;
							if (contadorCotizaciones == 3) comparaciones = 3;

							if (comparaciones > 0)
							{
								// Validar que todas las cotizaciones tengan el mismo período diferido
								bool mismoPD = true;
								if (cotizacion[0] != null && cotizacion[1] != null && cotizacion[2] != null)
								{
									int pd = cotizacion[0].PeriodoDiferido;
									if (cotizacion[1].PeriodoDiferido != pd || cotizacion[2].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}
								if (cotizacion[0] == null && cotizacion[1] != null && cotizacion[2] != null)
								{
									int pd = cotizacion[1].PeriodoDiferido;
									if (cotizacion[2].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}
								if (cotizacion[0] != null && cotizacion[1] == null && cotizacion[2] != null)
								{
									int pd = cotizacion[0].PeriodoDiferido;
									if (cotizacion[2].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}
								if (cotizacion[0] != null && cotizacion[1] != null && cotizacion[2] == null)
								{
									int pd = cotizacion[0].PeriodoDiferido;
									if (cotizacion[1].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}

								if (mismoPD)
								{
									string nombreModalidad = String.Empty;
									int nroAnhosDiferidos = 0;
									for (int i = 0; i < 3; i++)
									{
										if (cotizacion[i] != null)
										{
											switch (cotizacion[i].Modalidad.Id)
											{
												case "I":
													nombreModalidad = "MODALIDAD INMEDIATA";
													nroAnhosDiferidos = 0;
													break;
												case "D":
													nombreModalidad = "MODALIDAD DIFERIDA " + cotizacion[i].PeriodoDiferido + " AÑO" + (cotizacion[i].PeriodoDiferido != 1 ? "S" : String.Empty);
													nroAnhosDiferidos = cotizacion[i].PeriodoDiferido;
													break;
											}
											break;
										}
									}

									// Crear Hoja
									sh = (HSSFSheet)wb.CreateSheet("Resumen Pensión Acumulada");

									// Estilos
									ICellStyle titulo = wb.CreateCellStyle();
									titulo.Alignment = HorizontalAlignment.Center;
									titulo.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle tituloturquesa = wb.CreateCellStyle();
									tituloturquesa.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.Alignment = HorizontalAlignment.Center;
									tituloturquesa.VerticalAlignment = VerticalAlignment.Center;
									tituloturquesa.FillForegroundColor = IndexedColors.SkyBlue.Index;
									tituloturquesa.FillPattern = FillPattern.SolidForeground;
									tituloturquesa.WrapText = true;

									ICellStyle tituloamarillo = wb.CreateCellStyle();
									tituloamarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloamarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloamarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloamarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloamarillo.Alignment = HorizontalAlignment.Center;
									tituloamarillo.VerticalAlignment = VerticalAlignment.Center;
									tituloamarillo.FillForegroundColor = IndexedColors.Yellow.Index;
									tituloamarillo.FillPattern = FillPattern.SolidForeground;
									tituloamarillo.WrapText = true;

									ICellStyle detalle = wb.CreateCellStyle();
									detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.Alignment = HorizontalAlignment.Center;
									detalle.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle detalleamarillo = wb.CreateCellStyle();
									detalleamarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleamarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleamarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleamarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalleamarillo.Alignment = HorizontalAlignment.Center;
									detalleamarillo.VerticalAlignment = VerticalAlignment.Center;
									detalleamarillo.FillForegroundColor = IndexedColors.Yellow.Index;
									detalleamarillo.FillPattern = FillPattern.SolidForeground;

									ICellStyle detalletotal = wb.CreateCellStyle();
									detalletotal.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalletotal.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalletotal.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalletotal.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalletotal.Alignment = HorizontalAlignment.Center;
									detalletotal.VerticalAlignment = VerticalAlignment.Center;
									detalletotal.FillForegroundColor = IndexedColors.Yellow.Index;
									detalletotal.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepension = wb.CreateCellStyle();
									detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.Alignment = HorizontalAlignment.Right;
									detallepension.VerticalAlignment = VerticalAlignment.Center;
									detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									ICellStyle detallepensionamarillo = wb.CreateCellStyle();
									detallepensionamarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionamarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionamarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionamarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensionamarillo.Alignment = HorizontalAlignment.Right;
									detallepensionamarillo.VerticalAlignment = VerticalAlignment.Center;
									detallepensionamarillo.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensionamarillo.FillForegroundColor = IndexedColors.Yellow.Index;
									detallepensionamarillo.FillPattern = FillPattern.SolidForeground;

									ICellStyle detallepensiontotal = wb.CreateCellStyle();
									detallepensiontotal.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiontotal.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiontotal.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiontotal.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepensiontotal.Alignment = HorizontalAlignment.Right;
									detallepensiontotal.VerticalAlignment = VerticalAlignment.Center;
									detallepensiontotal.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");
									detallepensiontotal.FillForegroundColor = IndexedColors.Yellow.Index;
									detallepensiontotal.FillPattern = FillPattern.SolidForeground;

									ICellStyle amarillo = wb.CreateCellStyle();
									amarillo.Alignment = HorizontalAlignment.Left;
									amarillo.VerticalAlignment = VerticalAlignment.Center;
									amarillo.FillForegroundColor = IndexedColors.Yellow.Index;
									amarillo.FillPattern = FillPattern.SolidForeground;

									IFont negrita = wb.CreateFont();
									negrita.FontName = "Arial";
									negrita.Boldweight = (short)FontBoldWeight.Bold;
									titulo.SetFont(negrita);
									tituloturquesa.SetFont(negrita);
									tituloamarillo.SetFont(negrita);
									detalletotal.SetFont(negrita);
									detallepensiontotal.SetFont(negrita);
									amarillo.SetFont(negrita);

									// Hallar el número de columnas
									bool hayDolares;
									if (correlativo3 > 0)
									{
										hayDolares = true;
									}
									else
									{
										hayDolares = false;
									}

									int nroColumnas = 1 + contadorCotizaciones + (hayDolares ? 2 : 0) + comparaciones;

									fila = 0;
									r = sh.CreateRow(fila);
									cra = new CellRangeAddress(fila, fila, 0, nroColumnas - 1);
									sh.AddMergedRegion(cra);
									for (int i = 0; i < comparaciones; i++)
									{
										r.CreateCell(i);
									}
									sh.GetRow(fila).GetCell(0).SetCellValue("COMPARATIVO DE PENSIÓN ACUMULADA");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									r.CreateCell(3);
									sh.GetRow(fila).GetCell(0).SetCellValue(nombreModalidad);
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									// Cabeceras
									r = sh.CreateRow(fila);
									for (int i = 0; i < nroColumnas; i++)
									{
										r.CreateCell(i);
									}
									int columna = 0;
									sh.SetColumnWidth(columna, 4000);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Pensión Acumulada Años");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
									if (correlativo1 > 0)
									{
										sh.SetColumnWidth(columna, 5000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Indexados");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;
									}
									if (correlativo2 > 0)
									{
										sh.SetColumnWidth(columna, 5000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Ajustados");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;
									}
									if (correlativo3 > 0)
									{
										sh.SetColumnWidth(columna, 5000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares Ajustados");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;
										sh.SetColumnWidth(columna, 5000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares al TC");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
										columna++;
									}
									if (correlativo1 > 0 && correlativo3 > 0)
									{
										sh.SetColumnWidth(columna, 3000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("$ Ajust. vs S/. Index.");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
										columna++;
									}
									if (correlativo1 > 0 && correlativo2 > 0)
									{
										sh.SetColumnWidth(columna, 3000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("$ Ajust. vs S/. Index.");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
										columna++;
									}
									if (correlativo2 > 0 && correlativo3 > 0)
									{
										sh.SetColumnWidth(columna, 3000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("$ Ajust. vs S/. Ajust.");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloamarillo;
										columna++;
									}
									columna = 0;
									fila++;

									// Columna: Pensión Acumulada Años
									int filaPagoRV = -1;
									int diferidos = nroAnhosDiferidos;
									string cadena = String.Empty;
									for (int i = 1; i <= 25; i++)
									{
										cadena = i.ToString();
										if (diferidos > 0)
										{
											cadena += " Temporal AFP";
											diferidos--;
										}
										else if (diferidos == 0)
										{
											filaPagoRV = fila;
											diferidos--;
										}
										r = sh.CreateRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(cadena);
										if (fila != filaPagoRV)
											sh.GetRow(fila).GetCell(columna).CellStyle = detalle;
										else
											sh.GetRow(fila).GetCell(columna).CellStyle = detalleamarillo;
										fila++;
									}
									r = sh.CreateRow(fila);
									r.CreateCell(columna);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Total");
									sh.GetRow(fila).GetCell(columna).CellStyle = detalletotal;
									columna++;
									fila -= 25;

									// Columna: Soles Indexados
									double monto = 0;
									double[] solesIndexados = new double[26];
									diferidos = nroAnhosDiferidos;
									if (correlativo1 > 0)
									{
										double pensionCiaSolesInd = cotizacion[0].PensionCia;
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												monto = pensionCiaSolesInd;
												if (!cotizacion[0].Gratificacion)
												{
													solesIndexados[i] = monto * 12;
												}
												else
												{
													solesIndexados[i] = monto * 14;
												}
												pensionCiaSolesInd *= (1 + ipc / 100.00);
											}
											else
											{
												solesIndexados[i] = cotizacion[0].PensionAFP * 12;
												diferidos--;
												pensionCiaSolesInd *= (1 + ipc / 100.00);
											}
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											sh.GetRow(fila).GetCell(columna).SetCellValue(solesIndexados[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(solesIndexados.Sum());
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									// Columna: Soles Ajustados
									monto = 0;
									double[] solesAjustados = new double[26];
									diferidos = nroAnhosDiferidos;
									if (correlativo2 > 0)
									{
										double pensionCiaSolesAju = cotizacion[1].PensionCia;
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												monto = pensionCiaSolesAju;
												if (!cotizacion[0].Gratificacion)
												{
													solesAjustados[i] = monto * 12;
												}
												else
												{
													solesAjustados[i] = monto * 14;
												}
												pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
											}
											else
											{
												solesAjustados[i] = cotizacion[1].PensionAFP * 12;
												diferidos--;
												pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
											}
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											sh.GetRow(fila).GetCell(columna).SetCellValue(solesAjustados[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(solesAjustados.Sum());
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									// Columna: Dólares Ajustados
									monto = 0;
									double[] dolaresAjustados = new double[26];
									diferidos = nroAnhosDiferidos;
									if (correlativo3 > 0)
									{
										double pensionCiaDolaresAju = cotizacion[2].PensionCiaMO;
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												monto = pensionCiaDolaresAju;
												if (!cotizacion[2].Gratificacion)
												{
													dolaresAjustados[i] = monto * 12;
												}
												else
												{
													dolaresAjustados[i] = monto * 12;
												}
												pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
											}
											else
											{
												dolaresAjustados[i] = cotizacion[2].PensionAFP * 12;
												diferidos--;
												pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
											}
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresAjustados[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									// Columna: Dólares al TC
									double[] dolaresTC = new double[26];
									diferidos = nroAnhosDiferidos;
									if (correlativo3 > 0)
									{
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												dolaresTC[i] = dolaresAjustados[i] * tipoCambio;
											}
											else
											{
												dolaresTC[i] = dolaresAjustados[i];
												diferidos--;
											}
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresTC[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresTC.Sum());
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									// Columna: $ Ajust vs S/. Index
									double[] vs1 = new double[26];
									if (correlativo1 > 0 && correlativo3 > 0)
									{
										for (int i = 1; i <= 25; i++)
										{
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											vs1[i] = dolaresTC[i] - solesIndexados[i];
											sh.GetRow(fila).GetCell(columna).SetCellValue(vs1[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(vs1.Sum());
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									// Columna: S/. Ajust vs S/. Index
									double[] vs2 = new double[26];
									if (correlativo1 > 0 && correlativo2 > 0)
									{
										for (int i = 1; i <= 25; i++)
										{
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											vs2[i] = solesAjustados[i] - solesIndexados[i];
											sh.GetRow(fila).GetCell(columna).SetCellValue(vs2[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(vs2.Sum());
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									// Columna: $ Ajust vs S/. Ajust
									double[] vs3 = new double[26];
									if (correlativo2 > 0 && correlativo3 > 0)
									{
										for (int i = 1; i <= 25; i++)
										{
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											vs3[i] = dolaresTC[i] - solesAjustados[i];
											sh.GetRow(fila).GetCell(columna).SetCellValue(vs3[i]);
											if (fila != filaPagoRV)
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											else
												sh.GetRow(fila).GetCell(columna).CellStyle = detallepensionamarillo;
											fila++;
										}
										r = sh.GetRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(vs3.Sum());
										sh.GetRow(fila).GetCell(columna).CellStyle = detallepensiontotal;
										columna++;
										fila -= 25;
									}

									r = sh.GetRow(filaPagoRV);
									cra = new NPOI.SS.Util.CellRangeAddress(filaPagoRV, filaPagoRV, columna, columna + 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(columna);
									r.CreateCell(columna + 1);
									r.CreateCell(columna + 2);
									r.CreateCell(columna + 3);
									sh.GetRow(filaPagoRV).GetCell(columna).SetCellValue("Aquí empieza la pensión vitalicia");
									sh.GetRow(filaPagoRV).GetCell(columna).CellStyle = amarillo;
									sh.GetRow(filaPagoRV).GetCell(columna + 1).CellStyle = amarillo;
									sh.GetRow(filaPagoRV).GetCell(columna + 2).CellStyle = amarillo;
									sh.GetRow(filaPagoRV).GetCell(columna + 3).CellStyle = amarillo;
								}
							}

							// HOJA: COMPARATIVO DE PENSIÓN ANUAL
							if (comparaciones > 0)
							{
								// Validar que todas las cotizaciones tengan el mismo período diferido
								bool mismoPD = true;
								if (cotizacion[0] != null && cotizacion[1] != null && cotizacion[2] != null)
								{
									int pd = cotizacion[0].PeriodoDiferido;
									if (cotizacion[1].PeriodoDiferido != pd || cotizacion[2].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}
								if (cotizacion[0] == null && cotizacion[1] != null && cotizacion[2] != null)
								{
									int pd = cotizacion[1].PeriodoDiferido;
									if (cotizacion[2].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}
								if (cotizacion[0] != null && cotizacion[1] == null && cotizacion[2] != null)
								{
									int pd = cotizacion[0].PeriodoDiferido;
									if (cotizacion[2].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}
								if (cotizacion[0] != null && cotizacion[1] != null && cotizacion[2] == null)
								{
									int pd = cotizacion[0].PeriodoDiferido;
									if (cotizacion[1].PeriodoDiferido != pd)
									{
										mismoPD = false;
									}
								}

								if (mismoPD)
								{
									string nombreModalidad = String.Empty;
									int nroAnhosDiferidos = 0;
									for (int i = 0; i < 3; i++)
									{
										if (cotizacion[i] != null)
										{
											switch (cotizacion[i].Modalidad.Id)
											{
												case "I":
													nombreModalidad = "MODALIDAD INMEDIATA";
													nroAnhosDiferidos = 0;
													break;
												case "D":
													nombreModalidad = "MODALIDAD DIFERIDA " + cotizacion[i].PeriodoDiferido + " AÑO" + (cotizacion[i].PeriodoDiferido != 1 ? "S" : String.Empty);
													nroAnhosDiferidos = cotizacion[i].PeriodoDiferido;
													break;
											}
											break;
										}
									}

									// Crear Hoja
									sh = (HSSFSheet)wb.CreateSheet("Comparativo de Pensión Anual");

									// Estilos
									ICellStyle titulo = wb.CreateCellStyle();
									titulo.Alignment = HorizontalAlignment.Center;
									titulo.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle tituloturquesa = wb.CreateCellStyle();
									tituloturquesa.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									tituloturquesa.Alignment = HorizontalAlignment.Center;
									tituloturquesa.VerticalAlignment = VerticalAlignment.Center;
									tituloturquesa.FillForegroundColor = IndexedColors.SkyBlue.Index;
									tituloturquesa.FillPattern = FillPattern.SolidForeground;
									tituloturquesa.WrapText = true;

									ICellStyle detalle = wb.CreateCellStyle();
									detalle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detalle.Alignment = HorizontalAlignment.Center;
									detalle.VerticalAlignment = VerticalAlignment.Center;

									ICellStyle detallepension = wb.CreateCellStyle();
									detallepension.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
									detallepension.Alignment = HorizontalAlignment.Right;
									detallepension.VerticalAlignment = VerticalAlignment.Center;
									detallepension.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

									ICellStyle amarillo = wb.CreateCellStyle();
									amarillo.Alignment = HorizontalAlignment.Left;
									amarillo.VerticalAlignment = VerticalAlignment.Center;
									amarillo.FillForegroundColor = IndexedColors.Yellow.Index;
									amarillo.FillPattern = FillPattern.SolidForeground;

									IFont negrita = wb.CreateFont();
									negrita.FontName = "Arial";
									negrita.Boldweight = (short)FontBoldWeight.Bold;
									titulo.SetFont(negrita);
									tituloturquesa.SetFont(negrita);
									amarillo.SetFont(negrita);

									// Hallar el número de columnas
									bool hayDolares;
									if (correlativo3 > 0)
									{
										hayDolares = true;
									}
									else
									{
										hayDolares = false;
									}

									int nroColumnas = 1 + (contadorCotizaciones * 2) + (hayDolares ? 1 : 0);

									fila = 0;
									r = sh.CreateRow(fila);
									cra = new CellRangeAddress(fila, fila, 0, nroColumnas - 1);
									sh.AddMergedRegion(cra);
									for (int i = 0; i < comparaciones; i++)
									{
										r.CreateCell(i);
									}
									sh.GetRow(fila).GetCell(0).SetCellValue("COMPARATIVO DE PENSIÓN ANUAL");
									sh.GetRow(fila).GetCell(0).CellStyle = titulo;
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									r = sh.CreateRow(fila);
									cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, 0, 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(0);
									r.CreateCell(1);
									r.CreateCell(2);
									r.CreateCell(3);
									
									sh.GetRow(fila).GetCell(0).SetCellValue(nombreModalidad);
									fila++;

									r = sh.CreateRow(fila);
									fila++;

									// Cabeceras
									r = sh.CreateRow(fila);
									for (int i = 0; i < nroColumnas; i++)
									{
										r.CreateCell(i);
									}
									int columna = 0;
									sh.SetColumnWidth(columna, 3500);
									sh.GetRow(fila).GetCell(columna).SetCellValue("Pensión Años");
									sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
									columna++;
									if (correlativo1 > 0)
									{
										sh.SetColumnWidth(columna, 3000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Indexados");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;

										sh.SetColumnWidth(columna, 1800);
										sh.GetRow(fila).GetCell(columna).SetCellValue(ipc.ToString() + "%");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;
									}
									if (correlativo2 > 0)
									{
										sh.SetColumnWidth(columna, 3000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Soles Ajustados");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;

										sh.SetColumnWidth(columna, 1800);
										sh.GetRow(fila).GetCell(columna).SetCellValue(ajusteTasa.ToString() + "%");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;
									}
									if (correlativo3 > 0)
									{
										sh.SetColumnWidth(columna, 3000);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares Ajustados");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;

										sh.SetColumnWidth(columna, 1800);
										sh.GetRow(fila).GetCell(columna).SetCellValue(ajusteTasa.ToString() + "%");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;

										sh.SetColumnWidth(columna, 4800);
										sh.GetRow(fila).GetCell(columna).SetCellValue("Dólares al TC");
										sh.GetRow(fila).GetCell(columna).CellStyle = tituloturquesa;
										columna++;
									}
									columna = 0;
									fila++;

									// Columna: Pensión Años
									int filaPagoRV = -1;
									
									int diferidos = nroAnhosDiferidos;
									string cadena = String.Empty;
									for (int i = 1; i <= 25; i++)
									{
										cadena = i.ToString();
										if (diferidos > 0)
										{
											cadena += " Temporal AFP";
											diferidos--;
										}
										else if (diferidos == 0)
										{
											filaPagoRV = fila;
											diferidos--;
										}
										r = sh.CreateRow(fila);
										r.CreateCell(columna);
										sh.GetRow(fila).GetCell(columna).SetCellValue(cadena);
										sh.GetRow(fila).GetCell(columna).CellStyle = detalle;
										fila++;
									}
									columna++;
									fila -= 25;

									// Columna: Soles Indexados
									double monto = 0;
									diferidos = nroAnhosDiferidos;
									if (correlativo1 > 0)
									{
										double pensionCiaSolesInd = cotizacion[0].PensionCia;
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												monto = pensionCiaSolesInd;
												pensionCiaSolesInd *= (1 + ipc / 100.00);
											}
											else
											{
												monto = cotizacion[0].PensionAFP;
												diferidos--;
												pensionCiaSolesInd *= (1 + ipc / 100.00);
											}
											r = sh.GetRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, columna, columna + 1);
											sh.AddMergedRegion(cra);
											r.CreateCell(columna);
											r.CreateCell(columna + 1);
											sh.GetRow(fila).GetCell(columna).SetCellValue(monto);
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											sh.GetRow(fila).GetCell(columna + 1).CellStyle = detallepension;
											fila++;
										}
										columna += 2;
										fila -= 25;
									}

									// Columna: Soles Ajustados
									monto = 0;
									diferidos = nroAnhosDiferidos;
									if (correlativo2 > 0)
									{
										double pensionCiaSolesAju = cotizacion[1].PensionCia;
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												monto = pensionCiaSolesAju;
												pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
											}
											else
											{
												monto = cotizacion[1].PensionAFP;
												diferidos--;
												pensionCiaSolesAju *= (1 + ajusteTasa / 100.00);
											}
											r = sh.GetRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, columna, columna + 1);
											sh.AddMergedRegion(cra);
											r.CreateCell(columna);
											r.CreateCell(columna + 1);
											sh.GetRow(fila).GetCell(columna).SetCellValue(monto);
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											sh.GetRow(fila).GetCell(columna + 1).CellStyle = detallepension;
											fila++;
										}
										columna += 2;
										fila -= 25;
									}

									// Columna: Dólares Ajustados
									double[] dolaresAjustados = new double[26];
									diferidos = nroAnhosDiferidos;
									if (correlativo3 > 0)
									{
										double pensionCiaDolaresAju = cotizacion[2].PensionCiaMO;
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												dolaresAjustados[i] = pensionCiaDolaresAju;
												pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
											}
											else
											{
												dolaresAjustados[i] = cotizacion[2].PensionAFP;
												diferidos--;
												pensionCiaDolaresAju *= (1 + ajusteTasa / 100.00);
											}
											r = sh.GetRow(fila);
											cra = new NPOI.SS.Util.CellRangeAddress(fila, fila, columna, columna + 1);
											sh.AddMergedRegion(cra);
											r.CreateCell(columna);
											r.CreateCell(columna + 1);
											sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresAjustados[i]);
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											sh.GetRow(fila).GetCell(columna + 1).CellStyle = detallepension;
											fila++;
										}
										columna += 2;
										fila -= 25;
									}

									// Columna: Dólares al TC
									double[] dolaresTC = new double[26];
									diferidos = nroAnhosDiferidos;
									if (correlativo3 > 0)
									{
										for (int i = 1; i <= 25; i++)
										{
											if (diferidos <= 0)
											{
												dolaresTC[i] = dolaresAjustados[i] * tipoCambio;
											}
											else
											{
												dolaresTC[i] = dolaresAjustados[i];
												diferidos--;
											}
											r = sh.GetRow(fila);
											r.CreateCell(columna);
											sh.GetRow(fila).GetCell(columna).SetCellValue(dolaresTC[i]);
											sh.GetRow(fila).GetCell(columna).CellStyle = detallepension;
											fila++;
										}
										columna++;
										fila -= 25;
									}

									r = sh.GetRow(filaPagoRV);
									cra = new NPOI.SS.Util.CellRangeAddress(filaPagoRV, filaPagoRV, columna, columna + 3);
									sh.AddMergedRegion(cra);
									r.CreateCell(columna);
									r.CreateCell(columna + 1);
									r.CreateCell(columna + 2);
									r.CreateCell(columna + 3);
									sh.GetRow(filaPagoRV).GetCell(columna).SetCellValue("Aquí empieza la pensión vitalicia");
									sh.GetRow(filaPagoRV).GetCell(columna).CellStyle = amarillo;
									sh.GetRow(filaPagoRV).GetCell(columna + 1).CellStyle = amarillo;
									sh.GetRow(filaPagoRV).GetCell(columna + 2).CellStyle = amarillo;
									sh.GetRow(filaPagoRV).GetCell(columna + 3).CellStyle = amarillo;
								}
							}

							HttpContext.Current.Session["ReportePensionProyectada"] = wb;

							respuesta.Estado = Constante.COD_OK;
						}
						else
						{
							//control.PermisoEjecutar = false;
						}
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
	
	//<INIGTI_2145>
		[WebMethod]
		public static Respuesta GenerarPensionProyectadaPlazoFijo(string tokenUsuario, List<Cotizacion> cotizaciones, List<string> cotizacion, string depositoPlazo, string tea, // string tem, string interesDeposito, 
												string ajusteIDX, string ajusteAJS, string tipoCambio, string crecimiento,string anhos, string afiliado)
		{

			using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
			{
				//Generar archivo Excel
				HSSFWorkbook wb;
				//XSSFWorkbook wb;
				
				Respuesta respuesta = new Respuesta();

				try
				{
					string ruta = HttpContext.Current.Server.MapPath("~/Recursos/Plantilla.xls");
					
					FileStream file = new FileStream(ruta, FileMode.Open, FileAccess.Read);
					wb = new HSSFWorkbook(file);
					//wb = new XSSFWorkbook(file);

					double DepositoPlazo = 0, Tem = 0, InteresDeposito = 0, AjusteIDX = 0, AjusteAJS = 0, TipoCambio = 0, Crecimiento = 0, TipoCambioOrigen = 0;


					double MOD2201TotMenOrigen = 0, MOD2202TotMenOrigen = 0, MOD2203TotMenOrigen = 0, MOD2204TotMenOrigen = 0, MOD2205TotMenOrigen = 0;
					double MOD2206TotMenOrigen = 0, MOD2207TotMenOrigen = 0, MOD2208TotMenOrigen = 0, MOD2209TotMenOrigen = 0, MOD2210TotMenOrigen = 0;
					double MOD2211TotMenOrigen = 0, MOD2212TotMenOrigen = 0, MOD2213TotMenOrigen = 0, MOD2214TotMenOrigen = 0, MOD2215TotMenOrigen = 0;
					double MOD2216TotMenOrigen = 0, MOD2217TotMenOrigen = 0, MOD2218TotMenOrigen = 0, MOD2219TotMenOrigen = 0, MOD2220TotMenOrigen = 0;

					double MOD2301TotMenOrigen = 0, MOD2302TotMenOrigen = 0, MOD2303TotMenOrigen = 0, MOD2304TotMenOrigen = 0, MOD2305TotMenOrigen = 0;
					double MOD2306TotMenOrigen = 0, MOD2307TotMenOrigen = 0, MOD2308TotMenOrigen = 0, MOD2309TotMenOrigen = 0, MOD2310TotMenOrigen = 0;
					double MOD2311TotMenOrigen = 0, MOD2312TotMenOrigen = 0, MOD2313TotMenOrigen = 0, MOD2314TotMenOrigen = 0, MOD2315TotMenOrigen = 0;
					double MOD2316TotMenOrigen = 0, MOD2317TotMenOrigen = 0, MOD2318TotMenOrigen = 0, MOD2319TotMenOrigen = 0, MOD2320TotMenOrigen = 0;

					double MOD2401TotMenOrigen = 0, MOD2402TotMenOrigen = 0, MOD2403TotMenOrigen = 0, MOD2404TotMenOrigen = 0, MOD2405TotMenOrigen = 0;
					double MOD2406TotMenOrigen = 0, MOD2407TotMenOrigen = 0, MOD2408TotMenOrigen = 0, MOD2409TotMenOrigen = 0, MOD2410TotMenOrigen = 0;
					double MOD2411TotMenOrigen = 0, MOD2412TotMenOrigen = 0, MOD2413TotMenOrigen = 0, MOD2414TotMenOrigen = 0, MOD2415TotMenOrigen = 0;
					double MOD2416TotMenOrigen = 0, MOD2417TotMenOrigen = 0, MOD2418TotMenOrigen = 0, MOD2419TotMenOrigen = 0, MOD2420TotMenOrigen = 0;

					double Ajuste1 = 0, Ajuste2 = 0, Ajuste3 = 3;
					int añoMax;

					ISheet sheet1 = wb.GetSheet("Comparativo");

					//sheet1.ForceFormulaRecalculation = true;

					double Tea = 0;

					if (anhos.Equals(""))
					{
						anhos = "20";
					}

					añoMax = Convert.ToInt16(Convert.ToDouble(anhos, new CultureInfo("es-PE")));

					Tea = Convert.ToDouble(tea, new CultureInfo("es-PE"));
					var a = 1 + (Tea / 100.00);
					var b = 1 / 12.0;

					Tem =  (Math.Pow(a, b) - 1) * 100.0;

					DepositoPlazo = Convert.ToDouble(depositoPlazo, new CultureInfo("es-PE"));
					//Tem = Convert.ToDouble(tem, new CultureInfo("es-PE"));
					//InteresDeposito = Convert.ToDouble(interesDeposito, new CultureInfo("es-PE"));
					InteresDeposito = Math.Round(DepositoPlazo * (Tem / 100.00), 2, MidpointRounding.AwayFromZero);

					AjusteIDX = Convert.ToDouble(ajusteIDX, new CultureInfo("es-PE"));
					AjusteAJS = Convert.ToDouble(ajusteAJS, new CultureInfo("es-PE"));
					TipoCambio = Convert.ToDouble(tipoCambio, new CultureInfo("es-PE"));
					TipoCambioOrigen = TipoCambio;
					Crecimiento = Convert.ToDouble(crecimiento, new CultureInfo("es-PE"));


					if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
					{
						int año = 0;
						var pagina = new Page();

						SimuladorPlazoFijo control = (SimuladorPlazoFijo)pagina.LoadControl("~/Controles/SimuladorPlazoFijo.ascx");

						if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorRentasVitaliciasVsPlazoFijo))
						{
							control.PermisoEjecutar = true;

							Cotizacion cotizacion1 = null;
							Cotizacion cotizacion2 = null;
							Cotizacion cotizacion3 = null;

							List<GraficoLineal> Grafico1 = new List<GraficoLineal>();
							List<GraficoLineal> Grafico2 = new List<GraficoLineal>();
							List<GraficoLineal> Grafico3 = new List<GraficoLineal>();
							List<GraficoLineal> Grafico4 = new List<GraficoLineal>();

							cotizacion1 = cotizaciones.Where(c => c.Correlativo == Convert.ToInt32(cotizacion[0])).Select(c => new Cotizacion()
							{
								Correlativo = c.Correlativo,
								Moneda = c.Moneda,
								Producto = c.Producto,
								Modalidad = c.Modalidad,
								PeriodoDiferido = c.PeriodoDiferido,
								PorcentajeEntreRentas = c.PorcentajeEntreRentas,
								PeriodoGarantizado = c.PeriodoGarantizado,
								DerechoCrecer = c.DerechoCrecer,
								Gratificacion = c.Gratificacion,
								Capital = c.Capital,
								AjusteTRA = c.AjusteTRA,
								MontoCia = c.MontoCia,
								PensionCia = c.PensionCia,
								PensionCiaMO = c.PensionCiaMO,
								PuurCia = c.PuurCia,
								TasaAFP = c.TasaAFP,
								MontoAFP = c.MontoAFP,
								PensionAFP = c.PensionAFP,
								PuurAFP = c.PuurAFP,
								TasaVenta = c.TasaVenta,
								TasaVentaSbs = c.TasaVentaSbs,
								PrimeraPensionRVD = c.PrimeraPensionRVD
							}).First();

							if (cotizacion.Count > 1)
							{
								cotizacion2 = cotizaciones.Where(c => c.Correlativo == Convert.ToInt32(cotizacion[1])).Select(c => new Cotizacion()
								{
									Correlativo = c.Correlativo,
									Moneda = c.Moneda,
									Producto = c.Producto,
									Modalidad = c.Modalidad,
									PeriodoDiferido = c.PeriodoDiferido,
									PorcentajeEntreRentas = c.PorcentajeEntreRentas,
									PeriodoGarantizado = c.PeriodoGarantizado,
									DerechoCrecer = c.DerechoCrecer,
									Gratificacion = c.Gratificacion,
									Capital = c.Capital,
									AjusteTRA = c.AjusteTRA,
									MontoCia = c.MontoCia,
									PensionCia = c.PensionCia,
									PensionCiaMO = c.PensionCiaMO,
									PuurCia = c.PuurCia,
									TasaAFP = c.TasaAFP,
									MontoAFP = c.MontoAFP,
									PensionAFP = c.PensionAFP,
									PuurAFP = c.PuurAFP,
									TasaVenta = c.TasaVenta,
									TasaVentaSbs = c.TasaVentaSbs,
									PrimeraPensionRVD = c.PrimeraPensionRVD
								}).First();

							}

							if (cotizacion.Count > 2)
							{
								cotizacion3 = cotizaciones.Where(c => c.Correlativo == Convert.ToInt32(cotizacion[2])).Select(c => new Cotizacion()
								{
									Correlativo = c.Correlativo,
									Moneda = c.Moneda,
									Producto = c.Producto,
									Modalidad = c.Modalidad,
									PeriodoDiferido = c.PeriodoDiferido,
									PorcentajeEntreRentas = c.PorcentajeEntreRentas,
									PeriodoGarantizado = c.PeriodoGarantizado,
									DerechoCrecer = c.DerechoCrecer,
									Gratificacion = c.Gratificacion,
									Capital = c.Capital,
									AjusteTRA = c.AjusteTRA,
									MontoCia = c.MontoCia,
									PensionCia = c.PensionCia,
									PensionCiaMO = c.PensionCiaMO,
									PuurCia = c.PuurCia,
									TasaAFP = c.TasaAFP,
									MontoAFP = c.MontoAFP,
									PensionAFP = c.PensionAFP,
									PuurAFP = c.PuurAFP,
									TasaVenta = c.TasaVenta,
									TasaVentaSbs = c.TasaVentaSbs,
									PrimeraPensionRVD = c.PrimeraPensionRVD
								}).First();

							}

							//A4
							sheet1.GetRow(3).GetCell(0).SetCellValue(afiliado);

							//C7
							sheet1.GetRow(6).GetCell(2).SetCellValue(cotizacion1.MontoCia);

							//C9
							sheet1.GetRow(8).GetCell(2).SetCellValue(DepositoPlazo);

							//C10
							sheet1.GetRow(9).GetCell(2).SetCellValue(Tea/100);

							//C11
							sheet1.GetRow(10).GetCell(2).SetCellValue(añoMax);

							//C12
							sheet1.GetRow(11).GetCell(2).SetCellValue(Tem/100);

							//C13
							sheet1.GetRow(12).GetCell(2).SetCellValue(InteresDeposito);

							//F7
							sheet1.GetRow(6).GetCell(5).SetCellValue(AjusteIDX/100);

							//F9
							sheet1.GetRow(8).GetCell(5).SetCellValue(AjusteAJS/100);

							//F13
							sheet1.GetRow(12).GetCell(5).SetCellValue(TipoCambio);

							//F15
							sheet1.GetRow(14).GetCell(5).SetCellValue(Crecimiento/100);


							sheet1.ForceFormulaRecalculation = true;

							Ajuste1 = AjusteIDX;
							Ajuste2 = AjusteIDX;
							Ajuste3 = AjusteIDX;

							

							if (cotizacion1 != null)
							{
								if (cotizacion1.Moneda.Id == "013" || cotizacion1.Moneda.Id == "014")
									Ajuste1 = AjusteAJS;

								string Leyenda1 = cotizacion1.Modalidad.Id + " " + cotizacion1.Moneda.Nombre + (cotizacion1.PeriodoGarantizado != 0 ? " PG. " + cotizacion1.PeriodoGarantizado : "") + ((cotizacion1.Moneda.Id == "002" || cotizacion1.Moneda.Id == "014") ? " AL TC" : "");

								//A14
								sheet1.GetRow(13).GetCell(0).SetCellValue("Pensión Cia " + Leyenda1);

								//A31
								sheet1.GetRow(25).GetCell(0).SetCellValue(Leyenda1);

								//A37
								sheet1.GetRow(31).GetCell(0).SetCellValue("Depósito a Plazo por Año");

								//A38
								sheet1.GetRow(32).GetCell(0).SetCellValue(Leyenda1 + " por Año");

								//A44
								sheet1.GetRow(38).GetCell(0).SetCellValue(Leyenda1 + " Vs Plazo");

								//A52
								//sheet1.GetRow(51).GetCell(0).SetCellValue(Leyenda1 + " + Plazo");

								//A59
								sheet1.GetRow(47).GetCell(0).SetCellValue("Difer. " + Leyenda1 + " y Plazo");


								for (año = 1; año <= 20; año++)
								{

									if (año > 1)
									{
										TipoCambio = TipoCambio * (1 + (Crecimiento / 100.00));
									}

									switch (año)
									{
										case 1:

											control.MOD101Pension = DepositoPlazo;
											control.MOD2101TotalMensual = InteresDeposito;
											MOD2201TotMenOrigen = cotizacion1.PensionCiaMO; //* TipoCambio;
											
											control.MOD2201TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2201TotMenOrigen : MOD2201TotMenOrigen * TipoCambio;
											
											//B26
											sheet1.GetRow(20).GetCell(1).SetCellValue(control.MOD101Pension);
											//B30
											sheet1.GetRow(24).GetCell(1).SetCellValue(control.MOD2101TotalMensual);
											//B31
											sheet1.GetRow(25).GetCell(1).SetCellValue(control.MOD2201TotalMensual);

											//C14
											sheet1.GetRow(13).GetCell(2).SetCellValue(control.MOD2201TotalMensual);
											//D14
											if (cotizacion1.Moneda.Id == "002" || cotizacion1.Moneda.Id == "014")
											{
												sheet1.GetRow(13).GetCell(1).SetCellValue(cotizacion1.PensionCiaMO);
											}

											break;
										case 2:
											
											control.MOD102Pension = control.MOD101Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2102TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2202TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2202TotMenOrigen = MOD2201TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											
											control.MOD2202TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2202TotMenOrigen : MOD2202TotMenOrigen * TipoCambio;
											
											//C34 -26
											sheet1.GetRow(20).GetCell(2).SetCellValue(control.MOD102Pension);
											//C38 -30
											sheet1.GetRow(24).GetCell(2).SetCellValue(control.MOD2102TotalMensual);
											//C39 -31
											sheet1.GetRow(25).GetCell(2).SetCellValue(control.MOD2202TotalMensual);

											break;
										case 3:
											control.MOD103Pension = control.MOD102Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2103TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2203TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2203TotMenOrigen = MOD2202TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2203TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2203TotMenOrigen : MOD2203TotMenOrigen * TipoCambio;

											//D34 -26
											sheet1.GetRow(20).GetCell(3).SetCellValue(control.MOD103Pension);
											//D38 - 30
											sheet1.GetRow(24).GetCell(3).SetCellValue(control.MOD2103TotalMensual);
											//D39 - 31
											sheet1.GetRow(25).GetCell(3).SetCellValue(control.MOD2203TotalMensual);

											break;
										case 4:
											control.MOD104Pension = control.MOD103Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2104TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2204TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2204TotMenOrigen = MOD2203TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2204TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2204TotMenOrigen : MOD2204TotMenOrigen * TipoCambio;

											//E34-26
											sheet1.GetRow(20).GetCell(4).SetCellValue(control.MOD104Pension);
											//E38-30
											sheet1.GetRow(24).GetCell(4).SetCellValue(control.MOD2104TotalMensual);
											//E39-31
											sheet1.GetRow(25).GetCell(4).SetCellValue(control.MOD2204TotalMensual);

											break;
										case 5:
											control.MOD105Pension = control.MOD104Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2105TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2205TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2205TotMenOrigen = MOD2204TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}

											control.MOD2205TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2205TotMenOrigen : MOD2205TotMenOrigen * TipoCambio;

											//F34 - 26
											sheet1.GetRow(20).GetCell(5).SetCellValue(control.MOD105Pension);
											//F38 - 30
											sheet1.GetRow(24).GetCell(5).SetCellValue(control.MOD2105TotalMensual);
											//F39 - 31
											sheet1.GetRow(25).GetCell(5).SetCellValue(control.MOD2205TotalMensual);

											break;
										case 6:
											control.MOD106Pension = control.MOD105Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2106TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2206TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2206TotMenOrigen = MOD2205TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}

											control.MOD2206TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2206TotMenOrigen : MOD2206TotMenOrigen * TipoCambio;

											//G34 - 26
											sheet1.GetRow(20).GetCell(6).SetCellValue(control.MOD106Pension);
											//G38 - 30
											sheet1.GetRow(24).GetCell(6).SetCellValue(control.MOD2106TotalMensual);
											//G39 - 31
											sheet1.GetRow(25).GetCell(6).SetCellValue(control.MOD2206TotalMensual);

											break;
										case 7:
											control.MOD107Pension = control.MOD106Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2107TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2207TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2207TotMenOrigen = MOD2206TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}

											control.MOD2207TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2207TotMenOrigen : MOD2207TotMenOrigen * TipoCambio;

											//H34 - 26
											sheet1.GetRow(20).GetCell(7).SetCellValue(control.MOD106Pension);
											//H38 - 30
											sheet1.GetRow(24).GetCell(7).SetCellValue(control.MOD2106TotalMensual);
											//H39 - 31
											sheet1.GetRow(25).GetCell(7).SetCellValue(control.MOD2206TotalMensual);

											break;
										case 8:
											control.MOD108Pension = control.MOD107Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2108TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2208TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2208TotMenOrigen = MOD2207TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2208TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2208TotMenOrigen : MOD2208TotMenOrigen * TipoCambio;

											//I34 - 26
											sheet1.GetRow(20).GetCell(8).SetCellValue(control.MOD108Pension);
											//I38 - 30
											sheet1.GetRow(24).GetCell(8).SetCellValue(control.MOD2108TotalMensual);
											//I39 - 31
											sheet1.GetRow(25).GetCell(8).SetCellValue(control.MOD2208TotalMensual);

											break;
										case 9:
											control.MOD109Pension = control.MOD108Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2109TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2209TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2209TotMenOrigen = MOD2208TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2209TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2209TotMenOrigen : MOD2209TotMenOrigen * TipoCambio;

											//J34 - 26
											sheet1.GetRow(20).GetCell(9).SetCellValue(control.MOD109Pension);
											//J38 - 30
											sheet1.GetRow(24).GetCell(9).SetCellValue(control.MOD2109TotalMensual);
											//J39 - 31
											sheet1.GetRow(25).GetCell(9).SetCellValue(control.MOD2209TotalMensual);

											break;
										case 10:
											control.MOD110Pension = control.MOD109Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2110TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2210TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2210TotMenOrigen = MOD2209TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2210TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2210TotMenOrigen : MOD2210TotMenOrigen * TipoCambio;

											//K34 - 26
											sheet1.GetRow(20).GetCell(10).SetCellValue(control.MOD110Pension);
											//K38 - 30
											sheet1.GetRow(24).GetCell(10).SetCellValue(control.MOD2110TotalMensual);
											//K39 - 31
											sheet1.GetRow(25).GetCell(10).SetCellValue(control.MOD2210TotalMensual);

											break;
										case 11:
											control.MOD111Pension = control.MOD110Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2111TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2211TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2211TotMenOrigen = MOD2210TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2211TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2211TotMenOrigen : MOD2211TotMenOrigen * TipoCambio;

											//L34 -26
											sheet1.GetRow(20).GetCell(11).SetCellValue(control.MOD111Pension);
											//L38 - 30
											sheet1.GetRow(24).GetCell(11).SetCellValue(control.MOD2111TotalMensual);
											//L39 - 31
											sheet1.GetRow(25).GetCell(11).SetCellValue(control.MOD2211TotalMensual);

											break;
										case 12:
											control.MOD112Pension = control.MOD111Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2112TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2212TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2212TotMenOrigen = MOD2211TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}

											control.MOD2212TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2212TotMenOrigen : MOD2212TotMenOrigen * TipoCambio;

											//M34 - 26
											sheet1.GetRow(20).GetCell(12).SetCellValue(control.MOD112Pension);
											//M38 - 30
											sheet1.GetRow(24).GetCell(12).SetCellValue(control.MOD2112TotalMensual);
											//M39 -31
											sheet1.GetRow(25).GetCell(12).SetCellValue(control.MOD2212TotalMensual);

											break;
										case 13:
											control.MOD113Pension = control.MOD112Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2113TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2213TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2213TotMenOrigen = MOD2212TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2213TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2213TotMenOrigen : MOD2213TotMenOrigen * TipoCambio;

											//N34 - 26
											sheet1.GetRow(20).GetCell(13).SetCellValue(control.MOD113Pension);
											//N38 - 30
											sheet1.GetRow(24).GetCell(13).SetCellValue(control.MOD2113TotalMensual);
											//N39 - 31
											sheet1.GetRow(25).GetCell(13).SetCellValue(control.MOD2213TotalMensual);

											break;
										case 14:
											control.MOD114Pension = control.MOD113Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2114TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2214TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2214TotMenOrigen = MOD2213TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2214TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2214TotMenOrigen : MOD2214TotMenOrigen * TipoCambio;

											//O34 - 26
											sheet1.GetRow(20).GetCell(14).SetCellValue(control.MOD114Pension);
											//O38 - 30
											sheet1.GetRow(24).GetCell(14).SetCellValue(control.MOD2114TotalMensual);
											//O39 - 31
											sheet1.GetRow(25).GetCell(14).SetCellValue(control.MOD2214TotalMensual);

											break;
										case 15:
											control.MOD115Pension = control.MOD114Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2115TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2215TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else 
											{
												MOD2215TotMenOrigen = MOD2214TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											

											control.MOD2215TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2215TotMenOrigen : MOD2215TotMenOrigen * TipoCambio;

											//P34
											sheet1.GetRow(20).GetCell(15).SetCellValue(control.MOD115Pension);
											//P38                     
											sheet1.GetRow(24).GetCell(15).SetCellValue(control.MOD2115TotalMensual);
											//P39                     
											sheet1.GetRow(25).GetCell(15).SetCellValue(control.MOD2215TotalMensual);

											break;
										case 16:
											control.MOD116Pension = control.MOD115Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2116TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2216TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2216TotMenOrigen = MOD2215TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2216TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2216TotMenOrigen : MOD2216TotMenOrigen * TipoCambio;

											//Q34
											sheet1.GetRow(20).GetCell(16).SetCellValue(control.MOD116Pension);
											//Q38                     
											sheet1.GetRow(24).GetCell(16).SetCellValue(control.MOD2116TotalMensual);
											//Q39                     
											sheet1.GetRow(25).GetCell(16).SetCellValue(control.MOD2216TotalMensual);

											break;
										case 17:
											control.MOD117Pension = control.MOD116Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2117TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2217TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2217TotMenOrigen = MOD2216TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}

											control.MOD2217TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2217TotMenOrigen : MOD2217TotMenOrigen * TipoCambio;

											//R34
											sheet1.GetRow(20).GetCell(17).SetCellValue(control.MOD117Pension);
											//R38                     
											sheet1.GetRow(24).GetCell(17).SetCellValue(control.MOD2117TotalMensual);
											//R39                     
											sheet1.GetRow(25).GetCell(17).SetCellValue(control.MOD2217TotalMensual);

											break;
										case 18:
											control.MOD118Pension = control.MOD117Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2118TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2218TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2218TotMenOrigen = MOD2217TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											
											control.MOD2218TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2218TotMenOrigen : MOD2218TotMenOrigen * TipoCambio;

											//S34
											sheet1.GetRow(20).GetCell(18).SetCellValue(control.MOD118Pension);
											//S38                     
											sheet1.GetRow(24).GetCell(18).SetCellValue(control.MOD2118TotalMensual);
											//S39                     
											sheet1.GetRow(25).GetCell(18).SetCellValue(control.MOD2218TotalMensual);

											break;
										case 19:
											control.MOD119Pension = control.MOD118Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2119TotalMensual = InteresDeposito;

											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2219TotMenOrigen = cotizacion1.PrimeraPensionRVD;
											}
											else
											{
												MOD2219TotMenOrigen = MOD2218TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											

											control.MOD2219TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2219TotMenOrigen : MOD2219TotMenOrigen * TipoCambio;

											//T34
											sheet1.GetRow(20).GetCell(19).SetCellValue(control.MOD119Pension);
											//T38                     
											sheet1.GetRow(24).GetCell(19).SetCellValue(control.MOD2119TotalMensual);
											//T39                     
											sheet1.GetRow(25).GetCell(19).SetCellValue(control.MOD2219TotalMensual);

											break;
										case 20:
											control.MOD120Pension = control.MOD119Pension * (1 - (AjusteIDX / 100.00));
											control.MOD2120TotalMensual = InteresDeposito;
											if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2220TotMenOrigen = cotizacion1.PensionCiaMO * (cotizacion1.PorcentajeEntreRentas / 100.00);
											}
											else
											{
												MOD2220TotMenOrigen = MOD2219TotMenOrigen * (1 + (Ajuste1 / 100.00));
											}
											

											control.MOD2220TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2220TotMenOrigen : MOD2220TotMenOrigen * TipoCambio;

											//U34
											sheet1.GetRow(20).GetCell(20).SetCellValue(control.MOD120Pension);
											//U38                     
											sheet1.GetRow(24).GetCell(20).SetCellValue(control.MOD2120TotalMensual);
											//U39                     
											sheet1.GetRow(25).GetCell(20).SetCellValue(control.MOD2220TotalMensual);

											break;
										default:
											Console.WriteLine("Default case");
											break;
									}
								}

							}


							if (cotizacion2 != null)
							{
								TipoCambio = TipoCambioOrigen;
								if (cotizacion2.Moneda.Id == "013" || cotizacion2.Moneda.Id == "014")
									Ajuste2 = AjusteAJS;

								string Leyenda2 = cotizacion2.Modalidad.Id + " " + cotizacion2.Moneda.Nombre + (cotizacion2.PeriodoGarantizado != 0 ? " PG. " + cotizacion2.PeriodoGarantizado : "") + ((cotizacion2.Moneda.Id == "002" || cotizacion2.Moneda.Id == "014") ? " AL TC" : "");

								//A15
								sheet1.GetRow(14).GetCell(0).SetCellValue("Pensión Cia " + Leyenda2);

								//A32
								sheet1.GetRow(26).GetCell(0).SetCellValue(Leyenda2);
								
								//A39
								sheet1.GetRow(33).GetCell(0).SetCellValue(Leyenda2 + " por Año");

								//A45
								sheet1.GetRow(39).GetCell(0).SetCellValue(Leyenda2 + " Vs Plazo");

								//A53
								//sheet1.GetRow(47).GetCell(0).SetCellValue(Leyenda2 + " + Plazo");

								//A60
								sheet1.GetRow(48).GetCell(0).SetCellValue("Difer. " + Leyenda2 + " y Plazo");

								for (año = 1; año <= 20; año++)
								{

									if (año > 1)
									{
										TipoCambio = TipoCambio * (1 + (Crecimiento / 100.00));
									}

									switch (año)
									{
										case 1:
											MOD2301TotMenOrigen = cotizacion2.PensionCiaMO; //* TipoCambio;
											control.MOD2301TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2301TotMenOrigen : MOD2301TotMenOrigen * TipoCambio;
											
											//B40
											sheet1.GetRow(26).GetCell(1).SetCellValue(control.MOD2301TotalMensual);

											//C23
											sheet1.GetRow(14).GetCell(2).SetCellValue(control.MOD2301TotalMensual);
											//D23
											if (cotizacion2.Moneda.Id == "002" || cotizacion2.Moneda.Id == "014")
											{
												sheet1.GetRow(14).GetCell(1).SetCellValue(cotizacion2.PensionCiaMO);   
											}
												

											break;
										case 2:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2302TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else 
											{
												MOD2302TotMenOrigen = MOD2301TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}
											
											control.MOD2302TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2302TotMenOrigen : MOD2302TotMenOrigen * TipoCambio;

											//C40
											sheet1.GetRow(26).GetCell(2).SetCellValue(control.MOD2302TotalMensual);

											break;
										case 3:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2303TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2303TotMenOrigen = MOD2302TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											control.MOD2303TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2303TotMenOrigen : MOD2303TotMenOrigen * TipoCambio;

											//D40
											sheet1.GetRow(26).GetCell(3).SetCellValue(control.MOD2303TotalMensual);

											break;
										case 4:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2304TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2304TotMenOrigen = MOD2303TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2304TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2304TotMenOrigen : MOD2304TotMenOrigen * TipoCambio;

											//E40
											sheet1.GetRow(26).GetCell(4).SetCellValue(control.MOD2304TotalMensual);

											break;
										case 5:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2305TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2305TotMenOrigen = MOD2304TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											control.MOD2305TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2305TotMenOrigen : MOD2305TotMenOrigen * TipoCambio;

											//F40
											sheet1.GetRow(26).GetCell(5).SetCellValue(control.MOD2305TotalMensual);

											break;
										case 6:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2306TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2306TotMenOrigen = MOD2305TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2306TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2306TotMenOrigen : MOD2306TotMenOrigen * TipoCambio;

											//G40
											sheet1.GetRow(26).GetCell(6).SetCellValue(control.MOD2306TotalMensual);
											break;
										case 7:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2307TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2307TotMenOrigen = MOD2306TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2307TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2307TotMenOrigen : MOD2307TotMenOrigen * TipoCambio;

											//H40
											sheet1.GetRow(26).GetCell(7).SetCellValue(control.MOD2307TotalMensual);
											break;
										case 8:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2308TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else {
												MOD2308TotMenOrigen = MOD2307TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2308TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2308TotMenOrigen : MOD2308TotMenOrigen * TipoCambio;

											//I40
											sheet1.GetRow(26).GetCell(8).SetCellValue(control.MOD2308TotalMensual);
											break;
										case 9:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2309TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2309TotMenOrigen = MOD2308TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2309TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2309TotMenOrigen : MOD2309TotMenOrigen * TipoCambio;

											//J40
											sheet1.GetRow(26).GetCell(9).SetCellValue(control.MOD2309TotalMensual);
											break;
										case 10:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2310TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2310TotMenOrigen = MOD2309TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2310TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2310TotMenOrigen : MOD2310TotMenOrigen * TipoCambio;

											//K40
											sheet1.GetRow(26).GetCell(10).SetCellValue(control.MOD2310TotalMensual);
											break;
										case 11:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2311TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2311TotMenOrigen = MOD2310TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2311TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2311TotMenOrigen : MOD2311TotMenOrigen * TipoCambio;

											//L40
											sheet1.GetRow(26).GetCell(11).SetCellValue(control.MOD2311TotalMensual);
											break;
										case 12:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2312TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2312TotMenOrigen = MOD2311TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2312TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2312TotMenOrigen : MOD2312TotMenOrigen * TipoCambio;

											//M40
											sheet1.GetRow(26).GetCell(12).SetCellValue(control.MOD2312TotalMensual);
											break;
										case 13:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2313TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2313TotMenOrigen = MOD2312TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2313TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2313TotMenOrigen : MOD2313TotMenOrigen * TipoCambio;

											//N40
											sheet1.GetRow(26).GetCell(13).SetCellValue(control.MOD2313TotalMensual);
											break;
										case 14:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2314TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2314TotMenOrigen = MOD2313TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											control.MOD2314TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2314TotMenOrigen : MOD2314TotMenOrigen * TipoCambio;

											//O40
											sheet1.GetRow(26).GetCell(14).SetCellValue(control.MOD2314TotalMensual);
											break;
										case 15:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2315TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2315TotMenOrigen = MOD2314TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2315TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2315TotMenOrigen : MOD2315TotMenOrigen * TipoCambio;

											//P40
											sheet1.GetRow(26).GetCell(15).SetCellValue(control.MOD2315TotalMensual);
											break;
										case 16:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2316TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2316TotMenOrigen = MOD2315TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2316TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2316TotMenOrigen : MOD2316TotMenOrigen * TipoCambio;

											//Q40
											sheet1.GetRow(26).GetCell(16).SetCellValue(control.MOD2316TotalMensual);
											break;
										case 17:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2317TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2317TotMenOrigen = MOD2316TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2317TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2317TotMenOrigen : MOD2317TotMenOrigen * TipoCambio;

											//R40
											sheet1.GetRow(26).GetCell(17).SetCellValue(control.MOD2317TotalMensual);
											break;
										case 18:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2318TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2318TotMenOrigen = MOD2317TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2318TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2318TotMenOrigen : MOD2318TotMenOrigen * TipoCambio;

											//S40
											sheet1.GetRow(26).GetCell(18).SetCellValue(control.MOD2318TotalMensual);
											break;
										case 19:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2319TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2319TotMenOrigen = MOD2318TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											
											control.MOD2319TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2319TotMenOrigen : MOD2319TotMenOrigen * TipoCambio;

											//T40
											sheet1.GetRow(26).GetCell(19).SetCellValue(control.MOD2319TotalMensual);
											break;
										case 20:
											if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2320TotMenOrigen = cotizacion2.PrimeraPensionRVD;
											}
											else
											{
												MOD2320TotMenOrigen = MOD2319TotMenOrigen * (1 + (Ajuste2 / 100.00));
											}

											control.MOD2320TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2320TotMenOrigen : MOD2320TotMenOrigen * TipoCambio;

											//U40
											sheet1.GetRow(26).GetCell(20).SetCellValue(control.MOD2320TotalMensual);
											break;
										default:
											Console.WriteLine("Default case");
											break;
									}
								}

							}

							if (cotizacion3 != null)
							{
								TipoCambio = TipoCambioOrigen;
								if (cotizacion3.Moneda.Id == "013" || cotizacion3.Moneda.Id == "014")
									Ajuste3 = AjusteAJS;

								String Leyenda3 = cotizacion3.Modalidad.Id + " " + cotizacion3.Moneda.Nombre + (cotizacion3.PeriodoGarantizado != 0 ? " PG. " + cotizacion3.PeriodoGarantizado : "") + ((cotizacion3.Moneda.Id == "002" || cotizacion3.Moneda.Id == "014") ? " AL TC" : "");
								//A16
								sheet1.GetRow(15).GetCell(0).SetCellValue("Pensión Cia " + Leyenda3);

								//A33
								sheet1.GetRow(27).GetCell(0).SetCellValue(Leyenda3);

								//A40
								sheet1.GetRow(34).GetCell(0).SetCellValue(Leyenda3 + " por Año");

								//A46
								sheet1.GetRow(40).GetCell(0).SetCellValue(Leyenda3 + " Vs Plazo");

								//A54
								//sheet1.GetRow(48).GetCell(0).SetCellValue(Leyenda3 + " + Plazo");

								//A61
								sheet1.GetRow(49).GetCell(0).SetCellValue("Difer. " + Leyenda3 + " y Plazo");

								for (año = 1; año <= 20; año++)
								{

									if (año > 1)
									{
										TipoCambio = TipoCambio * (1 + (Crecimiento / 100.00));
									}

									switch (año)
									{
										case 1:
											MOD2401TotMenOrigen = cotizacion3.PensionCiaMO; //* TipoCambio;
											control.MOD2401TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2401TotMenOrigen : MOD2401TotMenOrigen * TipoCambio;

											//B41
											sheet1.GetRow(27).GetCell(1).SetCellValue(control.MOD2401TotalMensual);

											//C24
											sheet1.GetRow(15).GetCell(2).SetCellValue(control.MOD2401TotalMensual);
											//D24
											if (cotizacion3.Moneda.Id == "002" || cotizacion3.Moneda.Id == "014")
											{
												sheet1.GetRow(15).GetCell(1).SetCellValue(cotizacion3.PensionCiaMO);
											}
												

											break;
										case 2:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2402TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2402TotMenOrigen = MOD2401TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2402TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2402TotMenOrigen : MOD2402TotMenOrigen * TipoCambio;

											//C41
											sheet1.GetRow(27).GetCell(2).SetCellValue(control.MOD2402TotalMensual);

											break;
										case 3:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2403TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2403TotMenOrigen = MOD2402TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2403TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2403TotMenOrigen : MOD2403TotMenOrigen * TipoCambio;

											//D41
											sheet1.GetRow(27).GetCell(3).SetCellValue(control.MOD2403TotalMensual);
											break;
										case 4:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2404TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2404TotMenOrigen = MOD2403TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2404TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2404TotMenOrigen : MOD2404TotMenOrigen * TipoCambio;

											//E41
											sheet1.GetRow(27).GetCell(4).SetCellValue(control.MOD2404TotalMensual);

											break;
										case 5:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2405TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2405TotMenOrigen = MOD2404TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2405TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2405TotMenOrigen : MOD2405TotMenOrigen * TipoCambio;

											//F41
											sheet1.GetRow(27).GetCell(5).SetCellValue(control.MOD2405TotalMensual);

											break;
										case 6:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2406TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2406TotMenOrigen = MOD2405TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2406TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2406TotMenOrigen : MOD2406TotMenOrigen * TipoCambio;

											//G41
											sheet1.GetRow(27).GetCell(6).SetCellValue(control.MOD2406TotalMensual);
											break;
										case 7:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2407TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2407TotMenOrigen = MOD2406TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2407TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2407TotMenOrigen : MOD2407TotMenOrigen * TipoCambio;

											//H41
											sheet1.GetRow(27).GetCell(7).SetCellValue(control.MOD2407TotalMensual);
											break;
										case 8:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2408TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else {
												MOD2408TotMenOrigen = MOD2407TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2408TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2408TotMenOrigen : MOD2408TotMenOrigen * TipoCambio;

											//I41
											sheet1.GetRow(27).GetCell(8).SetCellValue(control.MOD2408TotalMensual);
											break;
										case 9:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2409TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2409TotMenOrigen = MOD2408TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2409TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2409TotMenOrigen : MOD2409TotMenOrigen * TipoCambio;

											//J41
											sheet1.GetRow(27).GetCell(9).SetCellValue(control.MOD2409TotalMensual);
											break;
										case 10:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2410TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else {
												MOD2410TotMenOrigen = MOD2409TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2410TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2410TotMenOrigen : MOD2410TotMenOrigen * TipoCambio;

											//K41
											sheet1.GetRow(27).GetCell(10).SetCellValue(control.MOD2410TotalMensual);
											break;
										case 11:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2411TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2411TotMenOrigen = MOD2410TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2411TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2411TotMenOrigen : MOD2411TotMenOrigen * TipoCambio;

											//L41
											sheet1.GetRow(27).GetCell(11).SetCellValue(control.MOD2411TotalMensual);
											break;
										case 12:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2412TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2412TotMenOrigen = MOD2411TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2412TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2412TotMenOrigen : MOD2412TotMenOrigen * TipoCambio;

											//M41
											sheet1.GetRow(27).GetCell(12).SetCellValue(control.MOD2412TotalMensual);
											break;
										case 13:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2413TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2413TotMenOrigen = MOD2412TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2413TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2413TotMenOrigen : MOD2413TotMenOrigen * TipoCambio;

											//N41
											sheet1.GetRow(27).GetCell(13).SetCellValue(control.MOD2413TotalMensual);
											break;
										case 14:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2414TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2414TotMenOrigen = MOD2413TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2414TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2414TotMenOrigen : MOD2414TotMenOrigen * TipoCambio;

											//O41
											sheet1.GetRow(27).GetCell(14).SetCellValue(control.MOD2414TotalMensual);
											break;
										case 15:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2415TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2415TotMenOrigen = MOD2414TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2415TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2415TotMenOrigen : MOD2415TotMenOrigen * TipoCambio;

											//P41
											sheet1.GetRow(27).GetCell(15).SetCellValue(control.MOD2415TotalMensual);
											break;
										case 16:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2416TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2416TotMenOrigen = MOD2415TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2416TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2416TotMenOrigen : MOD2416TotMenOrigen * TipoCambio;

											//Q41
											sheet1.GetRow(27).GetCell(16).SetCellValue(control.MOD2416TotalMensual);
											break;
										case 17:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2417TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2417TotMenOrigen = MOD2416TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2417TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2417TotMenOrigen : MOD2417TotMenOrigen * TipoCambio;

											//R41
											sheet1.GetRow(27).GetCell(17).SetCellValue(control.MOD2417TotalMensual);
											break;
										case 18:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2418TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2418TotMenOrigen = MOD2417TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2418TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2418TotMenOrigen : MOD2418TotMenOrigen * TipoCambio;

											//S41
											sheet1.GetRow(27).GetCell(18).SetCellValue(control.MOD2418TotalMensual);
											break;
										case 19:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2419TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2419TotMenOrigen = MOD2418TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2419TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2419TotMenOrigen : MOD2419TotMenOrigen * TipoCambio;

											//T41
											sheet1.GetRow(27).GetCell(19).SetCellValue(control.MOD2419TotalMensual);
											break;
										case 20:
											if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
											{
												MOD2420TotMenOrigen = cotizacion3.PrimeraPensionRVD;
											}
											else
											{
												MOD2420TotMenOrigen = MOD2419TotMenOrigen * (1 + (Ajuste3 / 100.00));
											}

											
											control.MOD2420TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2420TotMenOrigen : MOD2420TotMenOrigen * TipoCambio;

											//U41
											sheet1.GetRow(27).GetCell(20).SetCellValue(control.MOD2420TotalMensual);
											break;
										default:
											Console.WriteLine("Default case");
											break;
									}
								}

							}


							//////Eliminado Columna
							////ICell cell;
							////for (int i = 20; i > añoMax; i--)
							////{
							////    cell = sheet1.GetRow(24).GetCell(i);
							////    sheet1.GetRow(24).RemoveCell(cell);

							////    cell = sheet1.GetRow(25).GetCell(i);
							////    sheet1.GetRow(25).RemoveCell(cell);

							////    //29-33---------------------------
							////    cell = sheet1.GetRow(28).GetCell(i);
							////    sheet1.GetRow(28).RemoveCell(cell);

							////    cell = sheet1.GetRow(29).GetCell(i);
							////    sheet1.GetRow(29).RemoveCell(cell);

							////    cell = sheet1.GetRow(30).GetCell(i);
							////    sheet1.GetRow(30).RemoveCell(cell);

							////    cell = sheet1.GetRow(31).GetCell(i);
							////    sheet1.GetRow(31).RemoveCell(cell);

							////    cell = sheet1.GetRow(32).GetCell(i);
							////    sheet1.GetRow(32).RemoveCell(cell);

							////    //36-40---------------------------
							////    cell = sheet1.GetRow(35).GetCell(i);
							////    sheet1.GetRow(35).RemoveCell(cell);

							////    cell = sheet1.GetRow(36).GetCell(i);
							////    sheet1.GetRow(36).RemoveCell(cell);

							////    cell = sheet1.GetRow(37).GetCell(i);
							////    sheet1.GetRow(37).RemoveCell(cell);

							////    cell = sheet1.GetRow(38).GetCell(i);
							////    sheet1.GetRow(38).RemoveCell(cell);

							////    cell = sheet1.GetRow(39).GetCell(i);
							////    sheet1.GetRow(39).RemoveCell(cell);

							////    //43-46---------------------------
							////    cell = sheet1.GetRow(42).GetCell(i);
							////    sheet1.GetRow(42).RemoveCell(cell);

							////    cell = sheet1.GetRow(43).GetCell(i);
							////    sheet1.GetRow(43).RemoveCell(cell);

							////    cell = sheet1.GetRow(44).GetCell(i);
							////    sheet1.GetRow(44).RemoveCell(cell);

							////    cell = sheet1.GetRow(45).GetCell(i);
							////    sheet1.GetRow(45).RemoveCell(cell);

							////    //51-54----------------------------
							////    cell = sheet1.GetRow(50).GetCell(i);
							////    sheet1.GetRow(50).RemoveCell(cell);

							////    cell = sheet1.GetRow(51).GetCell(i);
							////    sheet1.GetRow(51).RemoveCell(cell);

							////    cell = sheet1.GetRow(52).GetCell(i);
							////    sheet1.GetRow(52).RemoveCell(cell);

							////    cell = sheet1.GetRow(53).GetCell(i);
							////    sheet1.GetRow(53).RemoveCell(cell);

							////    //58-61----------------------------
							////    cell = sheet1.GetRow(57).GetCell(i);
							////    sheet1.GetRow(57).RemoveCell(cell);

							////    cell = sheet1.GetRow(58).GetCell(i);
							////    sheet1.GetRow(58).RemoveCell(cell);

							////    cell = sheet1.GetRow(59).GetCell(i);
							////    sheet1.GetRow(59).RemoveCell(cell);

							////    cell = sheet1.GetRow(60).GetCell(i);
							////    sheet1.GetRow(60).RemoveCell(cell);

							////}


							ICellStyle estiloNaranja = wb.CreateCellStyle();
							estiloNaranja.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloNaranja.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloNaranja.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloNaranja.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloNaranja.Alignment = HorizontalAlignment.Right;
							estiloNaranja.VerticalAlignment = VerticalAlignment.Center;
							HSSFColor lightNaranja = setColor(wb, 234, 241, 221,0);
							estiloNaranja.FillForegroundColor = lightNaranja.Indexed;
							//estiloNaranja.FillForegroundColor = IndexedColors.Orange.Index;
							estiloNaranja.FillPattern = FillPattern.SolidForeground;
							estiloNaranja.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");


							ICellStyle estiloAmarillo = wb.CreateCellStyle();
							estiloAmarillo.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloAmarillo.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloAmarillo.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloAmarillo.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
							estiloAmarillo.Alignment = HorizontalAlignment.Right;
							estiloAmarillo.VerticalAlignment = VerticalAlignment.Center;
							//estiloAmarillo.FillForegroundColor = IndexedColors.Yellow.Index;
							HSSFColor lightAmarillo = setColor(wb, 219, 229, 241, 1);
							estiloAmarillo.FillForegroundColor = lightAmarillo.Indexed;
							estiloAmarillo.FillPattern = FillPattern.SolidForeground;
							estiloAmarillo.DataFormat = wb.CreateDataFormat().GetFormat("#,###.00");

							for (int i = 20; i > añoMax; i--)
							{
								
								sheet1.GetRow(20).GetCell(i).CellStyle = estiloNaranja;
								sheet1.GetRow(24).GetCell(i).CellStyle = estiloNaranja;
								sheet1.GetRow(31).GetCell(i).CellStyle = estiloNaranja;

								//25-28
								sheet1.GetRow(25).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(26).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(27).GetCell(i).CellStyle = estiloAmarillo;

								//32-35

								sheet1.GetRow(32).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(33).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(34).GetCell(i).CellStyle = estiloAmarillo;

								//39-41
								sheet1.GetRow(38).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(39).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(40).GetCell(i).CellStyle = estiloAmarillo;

								//48-50
								sheet1.GetRow(47).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(48).GetCell(i).CellStyle = estiloAmarillo;
								sheet1.GetRow(49).GetCell(i).CellStyle = estiloAmarillo;

							}


							//Eliminamos las filas de la plantilla
							//Cuadro 4
							IRow fila;
							
							if (cotizacion3 == null){
								fila =  sheet1.GetRow(49);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(49);
								fila.ZeroHeight = true;
							}

							if (cotizacion2 == null)
							{
								fila = sheet1.GetRow(48);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(48);
								fila.ZeroHeight = true;
							}

							
							//Cuadro 3
							if (cotizacion3 == null)
							{
								fila = sheet1.GetRow(40);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(40);
								fila.ZeroHeight = true;
							}
							if (cotizacion2 == null)
							{
								fila = sheet1.GetRow(39);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(39);
								fila.ZeroHeight = true;
							}

							//Cuadro 2
							if (cotizacion3 == null)
							{
								fila = sheet1.GetRow(34);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(34);
								fila.ZeroHeight = true;
							}
							if (cotizacion2 == null)
							{
								fila = sheet1.GetRow(33);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(33);
								fila.ZeroHeight = true;
							}

							//Cuadro 1
							if (cotizacion3 == null)
							{
								fila = sheet1.GetRow(27);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(27);
								fila.ZeroHeight = true;
							}
							if (cotizacion2 == null)
							{
								fila = sheet1.GetRow(26);
								sheet1.RemoveRow(fila);

								fila = sheet1.CreateRow(26);
								fila.ZeroHeight = true;   
							}

						}
						else
						{
							control.PermisoEjecutar = false;
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
							IdTipoEvento = Enums.EventoLog.SimuladorVitaliciaVsPlazoFijo.StringValue(),
							//Detalle = String.Format("Cotizaciones Nodalidad 1 [{0}] Nodalidad 2 [{1}] Nodalidad 3 [{2}] simuladas", correlativo1, correlativo2, correlativo3)

						});

						HttpContext.Current.Session["ReportePensionProyectada"] = wb;
						respuesta.Estado = Constante.COD_OK;

					}
					else
					{
						log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
						respuesta.Estado = Constante.COD_TOKEN;
					}
				}
				catch (FaultException ex)
				{
					log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
						ex.Source, ex.Message, ex.StackTrace));
					if (ex.InnerException != null)
					{
						log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
							ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
					}

					respuesta.Estado = Constante.COD_ERROR;
					respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
					respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
					respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { String.Format("La solicitud [{0}] no contiene los datos suficientes para generar la simulación, por favor intente generando una nueva cotización.") });
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

	//<FINGTI_2145>
	
	}
}