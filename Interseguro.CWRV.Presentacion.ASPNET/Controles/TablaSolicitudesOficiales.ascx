<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesOficiales.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesOficiales" %>

<form id="form1" runat="server" enableviewstate="False">

<div id="ContenidoDinamico">

    <asp:GridView 
        ID="TabSolicitudOficial" 
        runat="server" 
        Width="100%" 
        CellPadding="1" 
        CellSpacing="1" 
        GridLines="None" 
        UseAccessibleHeader="True"
        ViewStateMode="Disabled" 
        ClientIDMode="Static" 
        AutoGenerateColumns="False"
	    DataKeyNames="FechaPresentacion"
	    OnRowDataBound="TabSolicitudOficial_OnRowDataBound">

        <AlternatingRowStyle CssClass="grilla_alt2" />
        
        <Columns>
            <%--Grupo Fecha--%>
            <asp:TemplateField ItemStyle-Width="16px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
			    <ItemTemplate>
				    <a id="hrfdiv<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>" href="JavaScript:divexpandcollapse('div<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>');">
					    <img alt="Clientes" id="imgdiv<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>" src="../Imagenes/plus-icon.png" width="15" height="15" class="toogleOficiales" />
				    </a>

                    <!-- Adding the Div container -->
                    <%--Grupo Clientes--%>
				    <div id="div<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>" style="display: none;">
					    <!-- Adding Child GridView -->
					    <asp:GridView
						    ID="TabCotizacionCliente"
						    runat="server" 
                            Width="100%" 
                            CellPadding="1" 
                            CellSpacing="1" 
                            GridLines="None" 
                            UseAccessibleHeader="True"
                            ViewStateMode="Disabled" 
                            ClientIDMode="Static" 
                            AutoGenerateColumns="False"
						    DataKeyNames="NumOperacion"
                            OnRowDataBound="TabCotizacionCliente_OnRowDataBound">

                            <AlternatingRowStyle CssClass="grilla_alt2" />

						    <Columns>
                                <asp:TemplateField ItemStyle-Width="16px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                    <ItemTemplate>
				                    <a id="hrfdiv<%# Eval("NumOperacion") %>" href="JavaScript:divexpandcollapse('div<%# Eval("NumOperacion") %>');">
					                    <img alt="Cotizaciones" id="imgdiv<%# Eval("NumOperacion") %>" src="../Imagenes/plus-icon.png" width="15" height="15" class="toogleOficiales" />
				                    </a>
                                        <!-- Adding the Div container -->
                                        <%--Grupo Cotizaciones--%>
				                        <div id="div<%# Eval("NumOperacion") %>" style="display: none;">
					                        <!-- Adding Child GridView -->
					                        <asp:GridView
						                        ID="TabCotizacionCotizaciones"
						                        runat="server" 
                                                Width="100%" 
                                                CellPadding="1" 
                                                CellSpacing="1" 
                                                GridLines="None" 
                                                UseAccessibleHeader="True"
                                                ViewStateMode="Disabled" 
                                                ClientIDMode="Static" 
                                                AutoGenerateColumns="False"
						                        DataKeyNames="NumOperacion"
                                                OnRowDataBound="TabCotizacionCotizaciones_OnRowDataBound">

                                                 <AlternatingRowStyle CssClass="grilla_alt2" />

						                        <Columns>
                                                    <asp:TemplateField HeaderText="Fec. Presentación" ItemStyle-Width="85px" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotFecPresentacionLabel" runat="server" Text='<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Num. Meler" ItemStyle-Width="75px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotNumOperacion" runat="server" Text='<%# Eval("NumOperacion") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Sel." ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotIndEstadoSeleccion" runat="server" Text='<%# (Eval("IndEstadoSeleccion").ToString() == "S" ? "Sí" : "No") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Solicitud" ItemStyle-Width="75px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotNumSolicitud" runat="server" Text='<%# Eval("NumSolicitud") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="%A" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotPjeAumentoComision" runat="server" Text='<%# String.Format("{0:0.00}", Eval("PjeAumentoComision")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="%D" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotCodPjeCesionComision" runat="server" Text='<%# String.Format("{0:0.00}", Eval("CodPjeCesionComision")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DTRA" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotTRA" runat="server" Text='<%# (Eval("IndCondicionEspecial").ToString() == "S" ? "Sí" : "No") %>'></asp:Label>
                                                            <%--<asp:Label ID="CotTRA" runat="server" Text='<%# "No" %>'></asp:Label>--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Categoría">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotNombreCategoria" runat="server" Text='<%# Eval("Categoria.Nombre") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Estado">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotEstado" runat="server" Text='<%# Eval("TipoMovimiento.Nombre") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Fec. Registro" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotFecRegistroEscenario" runat="server" Text='<%# Eval("FecRegistroEscenario") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="CUSPP" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CotCuspp" runat="server" Text='<%# Eval("Afiliado.CUSPP") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="165px" ItemStyle-HorizontalAlign="Left">
                                                        <ItemTemplate>
                                                            <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_nuevo\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-operacion=\"" + Eval("NumOperacion") + "\" title=\"Nuevo Escenario") : " grilla_nuevo_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                                                            <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-operacion=\"" + Eval("NumOperacion") + "\" title=\"Modificar Escenario") : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                                                            <%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-numagente=\"" + Eval("Agente.Id")  +   "\" data-tipocotizacion=OFICIAL  title=\"Reporte Detalle Cotización") : " grilla_pdf_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%> <%--<INIGTI_754> Se agrega el tipocotizacion=OFICIAL--%>
                                                            <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_copiar\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" data-operacion=\"" + Eval("NumOperacion") + "\" title=\"Generar Solicitud Extraoficial en base a esta") : " grilla_copiar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                                                            <%# ((Eval("IndEstadoSeleccion").ToString() == "S" && Convert.ToDouble(Eval("PjeAumentoComision")) > 0) ?  "<a class=\"grilla_boton" + (PermisoSolicitudAnticipo ? (" grilla_anticipo\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" title=\"Generar Solicitud de Anticipo") : " grilla_anticipo_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : String.Empty) %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>


						                        </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="grilla_info">
                                                        No se ha encontrado ningún registro de ventas.
                                                    </div>
                                                </EmptyDataTemplate>
                                                <HeaderStyle CssClass="grilla_cabecera" />
                                                <RowStyle CssClass="grilla_alt1" />
					                        </asp:GridView>
				                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Fec. Presentación" ItemStyle-Width="85px" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="CliFecPresentacionLabel" runat="server" Text='<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Num. Meler" ItemStyle-Width="75px">
                                    <ItemTemplate>
                                        <asp:Label ID="CliNumOperacion" runat="server" Text='<%# Eval("NumOperacion") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Agente">
                                    <ItemTemplate>
                                        <asp:Label ID="CliNomAgente" runat="server" Text='<%# Eval("Agente.Nombre") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vig." ItemStyle-Width="25px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="CliIndVigAgente" runat="server" Text='<%# ((Eval("IndVigenciaAgente").ToString() == "S") ? "Sí" : "No") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Supervisor">
                                    <ItemTemplate>
                                        <asp:Label ID="CliNomSupervisor" runat="server" Text='<%# Eval("Supervision.Supervisor") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CUSPP" ItemStyle-Width="95px">
                                    <ItemTemplate>
                                        <asp:Label ID="CliCuspp" runat="server" Text='<%# Eval("Afiliado.CUSPP") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cliente">
                                    <ItemTemplate>
                                        <asp:Label ID="CliCliente" runat="server" Text='<%# Eval("Afiliado.NombreEmpresa") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="130px">
                                    <ItemTemplate>
                                        <%--<%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_rep_escenario\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-cusspp=\"" + Eval("Afiliado.CUSPP") + "\" data-operacion=\"" + Eval("NumOperacion")) : " grilla_rep_escenario_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>--%>
                                        <%# (RedLocal) ? "<a class=\"grilla_boton" + (PermisoReporteEscenario ? (" grilla_rep_escenario\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-operacion=\"" + Eval("NumOperacion") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" title=\"Generar Reporte de Escenarios") : " grilla_rep_escenario_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>
                                        <%--<%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf2\" data-operacion=\"" + Eval("NumOperacion") + "\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" title=\"Reporte de solicitud de pensión - Meler") : "\"></a>"%>--%>
                                        <%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf2\" data-operacion=\"" + Eval("NumOperacion") + "\" data-fechacotizacion=\"" + Eval("FechaPresentacion", "{0:dd/MM/yyyy}") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" title=\"Reporte de solicitud de pensión - Meler") : " grilla_pdf_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                                        <%# (PermisoValidacionesACOM) ? "<a data-operacion=\"" + Eval("NumOperacion") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" class=\"grilla_boton grilla_valacom grilla_switch_" + (Convert.ToBoolean(Eval("ValidarACOM")) ? "on\" data-valor=\"N\" title=\"Validación de Citas y Precubo activada para ACOM\">" : "off\" data-valor=\"S\" title=\"Validación de Citas y Precubo desactivada para ACOM\">") + "</a>" : String.Empty %>
                                        <%# (PermisoValidacionesDTRA) ? "<a data-operacion=\"" + Eval("NumOperacion") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" class=\"grilla_boton grilla_valdtra grilla_switch_" + (Convert.ToBoolean(Eval("ValidarDTRA")) ? "on\" data-valor=\"N\" title=\"Validación de Citas y Precubo activada para DTRA\">" : "off\" data-valor=\"S\" title=\"Validación de Citas y Precubo desactivada para DTRA\">") + "</a>" : String.Empty %>
                                        

                                    </ItemTemplate>
                                </asp:TemplateField>
						    </Columns>
                            <EmptyDataTemplate>
                                <div class="grilla_info">
                                    No se ha encontrado ningún registro de ventas.
                                </div>
                            </EmptyDataTemplate>
                            <HeaderStyle CssClass="grilla_cabecera" />
                            <RowStyle CssClass="grilla_alt1" />
					    </asp:GridView>

				    </div>


			    </ItemTemplate>
		    </asp:TemplateField>

            <asp:TemplateField HeaderText="Fecha de Plazo AFP" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:Label ID="FecPresentacion" runat="server" Text='<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("dd/MM/yyyy") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tiempo para el cierre" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <span class="HoraCierre"><%# (Convert.ToDateTime(Eval("FechaCierreLote")) - DateTime.Now.AddTicks( - (DateTime.Now.Ticks % TimeSpan.TicksPerSecond))).TotalSeconds * 1000 %></span>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de ventas.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
    
</div>
</form>
