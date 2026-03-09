using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Builder.ConsultasXLS
{
    public class ExportarNpoiXLS
    {

        private HSSFWorkbook _hssfworkbook;
        private string _nombrearchivo;
        public List<Seguimiento> _DetalleSeguimiento;
        public List<Supervision> _DetalleSupervision;
        public List<Anticipo> _DetalleSolicitudesAnticipo;

        int rowInicial = 0;
        int cellInicial = 0;

        public ExportarNpoiXLS(HSSFWorkbook workbook, string nombrearchivo, List<Seguimiento> dDetalle)
        {
            _hssfworkbook = workbook;
            _nombrearchivo = nombrearchivo;
            _DetalleSeguimiento = dDetalle;
            
        }

        public ExportarNpoiXLS(HSSFWorkbook workbook, string nombrearchivo, List<Supervision> dDetalle)
        {
            _hssfworkbook = workbook;
            _nombrearchivo = nombrearchivo;
            _DetalleSupervision = dDetalle;

        }

        public ExportarNpoiXLS(HSSFWorkbook workbook, string nombrearchivo, List<Anticipo> dDetalle)
        {
            _hssfworkbook = workbook;
            _nombrearchivo = nombrearchivo;
            _DetalleSolicitudesAnticipo = dDetalle;

        }

        public void InitializeWorkbook()
        {
            _hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "INTERSEGURO TEAM";
            _hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK INTERSEGURO";
            _hssfworkbook.SummaryInformation = si;
        }

        public MemoryStream GetExcelStream()
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            _hssfworkbook.Write(file);
            return file;
        }

        public void BuildSeguimiento()
        {

            rowInicial = 0;
            cellInicial = 0;
            
            //********************Estilos de Celdas******************//
            //*Estilo cabeceras*//
            ICellStyle cabecera_style = _hssfworkbook.CreateCellStyle();

            cabecera_style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.BottomBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.LeftBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.RightBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.TopBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.Alignment = HorizontalAlignment.Center;
            cabecera_style.VerticalAlignment = VerticalAlignment.Center;
            cabecera_style.FillForegroundColor =  HSSFColor.OliveGreen.Index;
            cabecera_style.FillPattern = FillPattern.BigSpots;

            IFont cabecera_font = _hssfworkbook.CreateFont();
            cabecera_font.Color = HSSFColor.White.Index;
            cabecera_font.Boldweight = (short)FontBoldWeight.Bold;
            cabecera_font.FontName = ("Calibri");
            cabecera_font.FontHeightInPoints = 10;
            cabecera_style.SetFont(cabecera_font);


            /*Estilo Registros*/
            ICellStyle registro_style = _hssfworkbook.CreateCellStyle();

            registro_style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.BottomBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.LeftBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.RightBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.TopBorderColor = HSSFColor.Grey25Percent.Index;

            registro_style.Alignment = HorizontalAlignment.Left;
            registro_style.VerticalAlignment = VerticalAlignment.Center;
            IFont registro_font = _hssfworkbook.CreateFont();
            registro_font.Color = HSSFColor.Black.Index;
            registro_font.FontName = ("Calibri");
            registro_font.FontHeightInPoints = 8;
            registro_style.SetFont(registro_font);

            ICellStyle titulo_style = _hssfworkbook.CreateCellStyle();
            IFont titulo_font = _hssfworkbook.CreateFont();
            titulo_font.Color = HSSFColor.Black.Index;
            titulo_font.Boldweight = (short)FontBoldWeight.Bold;
            titulo_font.FontName = ("Calibri");
            titulo_font.FontHeightInPoints = 18;
            titulo_style.SetFont(titulo_font);

            rowInicial = 0;
            cellInicial = 0;

            //var sheetCM = _hssfworkbook.CreateSheet(_nombrearchivo.ToString());
            ISheet sheetCM = _hssfworkbook.CreateSheet("ReporteSeguimiento_" + DateTime.Now.ToString("yyyyMMdd"));

            //seteamos el ancho de las columnas
            sheetCM.SetColumnWidth(0, 15 * 256); //Jefe
            sheetCM.SetColumnWidth(1, 20 * 256); //Supervisor
            sheetCM.SetColumnWidth(2, 20 * 256); //Agente
            sheetCM.SetColumnWidth(3, 15 * 256); //CUSPP
            sheetCM.SetColumnWidth(4, 30 * 256); //Persona
            sheetCM.SetColumnWidth(5, 8 * 256); //Origen
            sheetCM.SetColumnWidth(6, 12 * 256); //Teléfono
            sheetCM.SetColumnWidth(7, 12 * 256); //Saldo CIC
            sheetCM.SetColumnWidth(8, 12 * 256); //Solicitud
            sheetCM.SetColumnWidth(9, 20 * 256); //Categoría
            sheetCM.SetColumnWidth(10, 20 * 256); //Fecha de Ingreso
            sheetCM.SetColumnWidth(11, 20 * 256); //Fecha de Cierre
            sheetCM.SetColumnWidth(12, 20 * 256); //Compañía
            
            //titulo
            IRow tituloRowPPH = sheetCM.CreateRow(rowInicial);
            tituloRowPPH.HeightInPoints = 25;
            ICell cellTitulo = tituloRowPPH.CreateCell(0);
            //cellTitulo.SetCellValue(_nombrearchivo.ToString());
            cellTitulo.SetCellValue("Reporte de Seguimiento");
            cellTitulo.CellStyle = (titulo_style);

            //creamos las columnas de la cabecera head
            rowInicial += 2;

            IRow headerRowPPH = sheetCM.CreateRow(rowInicial);
            headerRowPPH.HeightInPoints = 30;

            ICell cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Jefe");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Supervisor");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Agente");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("CUSPP");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Persona");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Origen");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Teléfono");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Saldo CIC");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Solicitud");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Categoría");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Fecha de Ingreso");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Fecha de Cierre");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Compañía");
            cell.CellStyle = (cabecera_style);
            cellInicial = 0;

            foreach (var reg in _DetalleSeguimiento)
            {
                rowInicial += 1;
                IRow rowRegistroDetalle = sheetCM.CreateRow(rowInicial);
                rowRegistroDetalle.HeightInPoints = 15;

                ICell vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Jefe != null ? reg.Jefe.ToString() : String.Empty);// reg.NumeroLinea.ToString());
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Supervisor != null ? reg.Supervisor.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Agente != null ? reg.Agente.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.CUSPP != null ? reg.CUSPP.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Persona != null ? reg.Persona.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.SitioGenerado != null ? reg.SitioGenerado.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Telefono != null ? reg.Telefono.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.SaldoCIC != null ? reg.SaldoCIC.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.NroSolicitud != null ? reg.NroSolicitud.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Categoria != null ? reg.FechaIngreso.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.FechaIngreso != null ? reg.FechaIngreso.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.FechaCierre != null ? reg.FechaCierre.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Companhia != null ? reg.Companhia.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial = 0;
            }

   
        }

        public void BuildSupervision()
        {

            rowInicial = 0;
            cellInicial = 0;

            //********************Estilos de Celdas******************//
            //*Estilo cabeceras*//
            ICellStyle cabecera_style = _hssfworkbook.CreateCellStyle();

            cabecera_style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.BottomBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.LeftBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.RightBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.TopBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.Alignment = HorizontalAlignment.Center;
            cabecera_style.VerticalAlignment = VerticalAlignment.Center;
            cabecera_style.FillForegroundColor = HSSFColor.OliveGreen.Index;
            cabecera_style.FillPattern = FillPattern.BigSpots;

            IFont cabecera_font = _hssfworkbook.CreateFont();
            cabecera_font.Color = HSSFColor.White.Index;
            cabecera_font.Boldweight = (short)FontBoldWeight.Bold;
            cabecera_font.FontName = ("Calibri");
            cabecera_font.FontHeightInPoints = 10;
            cabecera_style.SetFont(cabecera_font);


            /*Estilo Registros*/
            ICellStyle registro_style = _hssfworkbook.CreateCellStyle();

            registro_style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.BottomBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.LeftBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.RightBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.TopBorderColor = HSSFColor.Grey25Percent.Index;

            registro_style.Alignment = HorizontalAlignment.Left;
            registro_style.VerticalAlignment = VerticalAlignment.Center;
            IFont registro_font = _hssfworkbook.CreateFont();
            registro_font.Color = HSSFColor.Black.Index;
            registro_font.FontName = ("Calibri");
            registro_font.FontHeightInPoints = 8;
            registro_style.SetFont(registro_font);

            ICellStyle titulo_style = _hssfworkbook.CreateCellStyle();
            IFont titulo_font = _hssfworkbook.CreateFont();
            titulo_font.Color = HSSFColor.Black.Index;
            titulo_font.Boldweight = (short)FontBoldWeight.Bold;
            titulo_font.FontName = ("Calibri");
            titulo_font.FontHeightInPoints = 18;
            titulo_style.SetFont(titulo_font);

            rowInicial = 0;
            cellInicial = 0;

            //var sheetCM = _hssfworkbook.CreateSheet(_nombrearchivo.ToString());
            ISheet sheetCM = _hssfworkbook.CreateSheet("ReporteSupervisión_" + DateTime.Now.ToString("yyyyMMdd"));

            //seteamos el ancho de las columnas
            sheetCM.SetColumnWidth(0, 15 * 256); //Jefe
            sheetCM.SetColumnWidth(1, 25 * 256); //Supervisor
            sheetCM.SetColumnWidth(2, 30 * 256); //Agente
            sheetCM.SetColumnWidth(3, 12 * 256); //Usuario
            sheetCM.SetColumnWidth(4, 20 * 256); //Fecha
            sheetCM.SetColumnWidth(5, 25 * 256); //Evento
            sheetCM.SetColumnWidth(6, 65 * 256); //Detalle

            //titulo
            IRow tituloRowPPH = sheetCM.CreateRow(rowInicial);
            tituloRowPPH.HeightInPoints = 25;
            ICell cellTitulo = tituloRowPPH.CreateCell(0);
            //cellTitulo.SetCellValue(_nombrearchivo.ToString());
            cellTitulo.SetCellValue("Reporte de Supervisión");
            cellTitulo.CellStyle = (titulo_style);

            //creamos las columnas de la cabecera head
            rowInicial += 2;

            IRow headerRowPPH = sheetCM.CreateRow(rowInicial);
            headerRowPPH.HeightInPoints = 30;

            ICell cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Jefe");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Supervisor");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Agente");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Usuario");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Fecha");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Evento");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Detalle");
            cell.CellStyle = (cabecera_style);
            cellInicial = 0;


            foreach (var reg in _DetalleSupervision)
            {
                rowInicial += 1;
                IRow rowRegistroDetalle = sheetCM.CreateRow(rowInicial);
                rowRegistroDetalle.HeightInPoints = 15;

                ICell vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Jefe != null ? reg.Jefe.ToString() : String.Empty);// reg.NumeroLinea.ToString());
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Supervisor != null ? reg.Supervisor.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Agente != null ? reg.Agente.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Usuario != null ? reg.Usuario.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.FechaEvento != null ? reg.FechaEvento.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Evento != null ? reg.Evento.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Detalle != null ? reg.Detalle.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial = 0;
            }


        }

        public void BuildSolicitudesAnticipo()
        {
            rowInicial = 0;
            cellInicial = 0;

            //******************** Estilos de Celdas ******************//
            //* Estilo cabeceras *//
            ICellStyle cabecera_style = _hssfworkbook.CreateCellStyle();

            cabecera_style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.BottomBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.LeftBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.RightBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
            cabecera_style.TopBorderColor = HSSFColor.Grey40Percent.Index;
            cabecera_style.Alignment = HorizontalAlignment.Center;
            cabecera_style.VerticalAlignment = VerticalAlignment.Center;
            cabecera_style.FillForegroundColor = HSSFColor.OliveGreen.Index;
            cabecera_style.FillPattern = FillPattern.BigSpots;

            IFont cabecera_font = _hssfworkbook.CreateFont();
            cabecera_font.Color = HSSFColor.White.Index;
            cabecera_font.Boldweight = (short)FontBoldWeight.Bold;
            cabecera_font.FontName = ("Calibri");
            cabecera_font.FontHeightInPoints = 10;
            cabecera_style.SetFont(cabecera_font);


            /* Estilo Registros */
            ICellStyle registro_style = _hssfworkbook.CreateCellStyle();

            registro_style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.BottomBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.LeftBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.RightBorderColor = HSSFColor.Grey25Percent.Index;
            registro_style.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
            registro_style.TopBorderColor = HSSFColor.Grey25Percent.Index;

            registro_style.Alignment = HorizontalAlignment.Left;
            registro_style.VerticalAlignment = VerticalAlignment.Center;
            IFont registro_font = _hssfworkbook.CreateFont();
            registro_font.Color = HSSFColor.Black.Index;
            registro_font.FontName = ("Calibri");
            registro_font.FontHeightInPoints = 8;
            registro_style.SetFont(registro_font);

            ICellStyle titulo_style = _hssfworkbook.CreateCellStyle();
            IFont titulo_font = _hssfworkbook.CreateFont();
            titulo_font.Color = HSSFColor.Black.Index;
            titulo_font.Boldweight = (short)FontBoldWeight.Bold;
            titulo_font.FontName = ("Calibri");
            titulo_font.FontHeightInPoints = 16;
            titulo_style.SetFont(titulo_font);

            rowInicial = 0;
            cellInicial = 0;

            ISheet sheetCM = _hssfworkbook.CreateSheet("ReporteSolicitudesAnticipo_" + DateTime.Now.ToString("yyyyMMdd"));

            // Seteamos el ancho de las columnas
            sheetCM.SetColumnWidth(0, 12 * 256); // N° Agente
            sheetCM.SetColumnWidth(1, 35 * 256); // Nombre Agente
            sheetCM.SetColumnWidth(2, 12 * 256); // N° Solicitud
            sheetCM.SetColumnWidth(3, 12 * 256); // Monto
            sheetCM.SetColumnWidth(4, 20 * 256); // Fecha Aceptación

            // Título
            IRow tituloRowPPH = sheetCM.CreateRow(rowInicial);
            tituloRowPPH.HeightInPoints = 20;
            ICell cellTitulo = tituloRowPPH.CreateCell(0);
            cellTitulo.SetCellValue("Solicitudes de Anticipo");
            cellTitulo.CellStyle = (titulo_style);

            // Creamos las columnas de la cabecera head
            rowInicial += 2;

            IRow headerRowPPH = sheetCM.CreateRow(rowInicial);
            headerRowPPH.HeightInPoints = 20;

            ICell cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("N° Agente");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Nombre Agente");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("N° Solicitud");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Monto");
            cell.CellStyle = (cabecera_style);
            cellInicial += 1;

            cell = headerRowPPH.CreateCell(cellInicial);
            cell.SetCellValue("Fecha Aceptación");
            cell.CellStyle = (cabecera_style);
            cellInicial = 0;


            foreach (var reg in _DetalleSolicitudesAnticipo)
            {
                rowInicial += 1;
                IRow rowRegistroDetalle = sheetCM.CreateRow(rowInicial);
                rowRegistroDetalle.HeightInPoints = 15;

                ICell vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Agente.Id != null ? reg.Agente.Id.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Agente.Nombre != null ? reg.Agente.Nombre.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Solicitud.Id != null ? reg.Solicitud.Id.ToString() : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.Monto != null ? String.Format("{0:#,#.00}", reg.Monto) : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial += 1;

                vcell = rowRegistroDetalle.CreateCell(cellInicial);
                vcell.SetCellValue(reg.FechaAceptacion != null ? reg.FechaAceptacion.ToString("dd/MM/yyyy hh:mm:ss tt") : String.Empty);
                vcell.CellStyle = (registro_style);
                cellInicial = 0;
            }
        }
    }
}