<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesBandejaOficiales.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesBandejaOficiales" %>

<form id="form1" runat="server" enableviewstate="False">


    <div id="ContenidoDinamico">
        <%--<INIGTI_4081>--%>
        <asp:HiddenField ID="PerRechazarSolicitud" runat="server" ClientIDMode="Static" />
        <%--<FINGTI_4081>--%>

        <div class="formLinea" align="right">
            <asp:Label ID="CotEstadoMovimiento" runat="server" ClientIDMode="Static"></asp:Label>
        </div>


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
                <asp:TemplateField ItemStyle-Width="16px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" >
                   
                   <HeaderTemplate>
                        <%#
                                                "<input type=\"checkbox\"  ID=\"ChkSolicitudTodos\" />" 

                                            %>
                   </HeaderTemplate>

                    <ItemTemplate>
                        <%--<a id='xConex<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>' href="JavaScript:divexpandcollapse('div<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>');">
                            <img alt="Clientes" id='imgdiv<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>' src="../Imagenes/plus-icon.png" width="15" height="15" class="toogleOficiales" />
                        </a>--%>

                       
                                           
                                            
                                      
                        
                         <a id="hrfdiv<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>" href="JavaScript:divexpandcollapse('div<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>');">
					        <img alt="Clientes" id="imgdiv<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>" src="../Imagenes/plus-icon.png" width="15" height="15" class="toogleOficiales" />
				         </a>
                             

                        <!-- Adding the Div container -->
                        <%--Grupo Solicitudes--%>
                        <div id='div<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") %>' style="display: none;">
                            <!-- Adding Child GridView -->

                            <asp:GridView
                                ID="TabBandejaSolicitudOficial"
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
                                OnRowDataBound="TabBandejaSolicitudOficial_RowDataBound">
                                <%--onrowdatabound
                                    OnRowDataBound
                                        DataKeyNames="FechaPresentacion"
                                --%>

                                <AlternatingRowStyle CssClass="grilla_alt2" />

                                <Columns>

                                    <%--<INIGTI_4081>--%>
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <%#
                                                "<input class='ChkSolicitudes' type=\"checkbox\"  ID=\"ChkSolicitud\" data-solicitud=\"" + Eval("NumSolicitud") + "\"/>" 

                                            %>
                                            
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                    </asp:TemplateField>
                                    <%--<FINGTI_4081>--%>

                                    <%--<asp:TemplateField HeaderText="Plazo AFP" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" runat="server" ToolTip="Plazo AFP" Text="Plazo AFP"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CliFecPresentacionLabel" runat="server" Text='<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Num. Solicitud" ItemStyle-Width="64px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" runat="server" ToolTip="Número Solicitud" Text="Núm. Solicitud"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotNumSolicitud" runat="server" Text='<%# Eval("NumSolicitud") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nom. Agente" ItemStyle-Width="70px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2" runat="server" ToolTip="Nombre Agente" Text="Nom. Agente"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CliNomAgente" runat="server" Text='<%# Eval("Agente.Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Nom. Supervisor" ItemStyle-Width="70px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label3" runat="server" ToolTip="Nombre Supervisor" Text="Nom. Superv."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CliNomSupervisor" runat="server" Text='<%# Eval("Supervision.Supervisor") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- <asp:TemplateField HeaderText="CUSPP" ItemStyle-Width="85px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2"  runat="server" ToolTip="CUSPP" Text="CUSPP"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CliCuspp" runat="server" Text='<%# Eval("Afiliado.CUSPP") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente" ItemStyle-Width="85px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2"  runat="server" ToolTip="Cliente" Text="Cliente"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CliCliente" runat="server" Text='<%# Eval("Afiliado.NombreEmpresa") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Cliente" ItemStyle-Width="140px" ItemStyle-HorizontalAlign="Left">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label4" runat="server" ToolTip="Cliente" Text="Cliente"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text="CUSPP:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotCUSPP" runat="server" CssClass="numerico" Text='<%# Eval("Afiliado.CUSPP") %>'></asp:Label>
                                            </strong>
                                            <br />
                                            <asp:Label  ID="Label6" runat="server" Text="Cliente:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotCliente" runat="server" Text='<%#  Eval("Afiliado.NombreEmpresa") %>'></asp:Label>
                                            </strong>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Datos AFP" ItemStyle-Width="120px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label7" runat="server" ToolTip="Datos AFP" Text="Datos AFP"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text="Categoría:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotNombreCategoria" runat="server" Text='<%#  Eval("Categoria.Nombre") %>'></asp:Label>

                                            </strong>
                                            <br />
                                            <asp:Label ID="Label9" runat="server" Text="AFP:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotNombreAFP" runat="server" Text='<%# Eval("AFP.Nombre") %>'></asp:Label>

                                            </strong>
                                            <br />
                                            <asp:Label ID="Label11" runat="server" Text="CIC:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotValTotalCic" runat="server" Text='<%# String.Format("{0:0,0.00}", Eval("ValTotalCic")) %>'></asp:Label>

                                            </strong>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%--<INIGTI_4081>--%>
                                    <%--<asp:TemplateField HeaderText="AFP" ItemStyle-Width="80px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2"  runat="server" ToolTip="AFP" Text="AFP"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotNombreAFP" runat="server" Text='<%# Eval("AFP.Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <%--<asp:TemplateField HeaderText="CIC" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2"  runat="server" ToolTip="CIC" Text="CIC"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotValTotalCic" runat="server" Text='<%# String.Format("{0:0,0.00}", Eval("ValTotalCic")) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <%--<FINGTI_4081>--%>

                                    <asp:TemplateField HeaderText="Tasas" ItemStyle-Width="75px" ItemStyle-HorizontalAlign="Left">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label12" runat="server" ToolTip="Tasas" Text="Tasas"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label13" runat="server" Text="SBS:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotPjeTasaVentaSBS" runat="server" CssClass="numerico" Text='<%# String.Format("{0:0.00}", Eval("Cotizaciones[0].TasaVentaSbs")) %>'></asp:Label>
                                            </strong>
                                            <br />
                                            
                                            <asp:Label ID="LabCotPjeTasaVentaMaxima" runat="server" Text="Max:" ClientIDMode="Static"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotPjeTasaVentaMaxima" runat="server" CssClass="numerico" ClientIDMode="Static" Text='<%# String.Format("{0:0.00}", Eval("Cotizaciones[0].TasaVentaMaxima")) %>'></asp:Label>
                                            </strong>

                                            <br />
                                            <asp:Label ID="Label15" runat="server" Text="Esperada:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotTasaEsperada" runat="server" CssClass="numerico" Text='<%# String.Format("{0:0.00}", Eval("Cotizaciones[0].TasaVentaSbsObjetivo")) %>'></asp:Label>
                                            </strong>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="TRA" ItemStyle-Width="75px" ItemStyle-HorizontalAlign="Left">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label16"  runat="server" ToolTip="TRA" Text="TRA"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label17" runat="server" Text="TRA:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotTRA" runat="server" Text='<%# String.Format("{0:0.00}", Eval("Cotizaciones[0].TasaRetornoAccionista")) %>'></asp:Label>
                                            </strong>

                                            <br />
                                            <asp:Label ID="Label18"  runat="server" Text="Min:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotCodTasaRetornoAccionistaMinimo" runat="server" Text='<%# String.Format("{0:0.00}", Eval("Cotizaciones[0].TasaRetornoAccionistaMinimo")) %>'></asp:Label>
                                            </strong>

                                            <br />
                                            <asp:Label ID="Label19" runat="server" Text="Esperada:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotTRAEsperada" runat="server" Text='<%# String.Format("{0:0.00}", Eval("Cotizaciones[0].TasaRetornoAccionistaObjetivo")) %>'></asp:Label>
                                            </strong>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Parámetros Especiales" ItemStyle-Width="85px" ItemStyle-HorizontalAlign="Left">
                                        <HeaderTemplate>
                                            <asp:Label  runat="server" ToolTip="Parámetros Especiales" Text="Parámetros Especiales"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label20" runat="server" Text="ACOM:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotPjeAumentoComision" runat="server" CssClass="numerico" Text='<%# String.Format("{0:0.00}", Eval("PjeAumentoComision")) %>'></asp:Label>

                                            </strong>
                                            <br />
                                            <asp:Label ID="Label21" runat="server" Text="DCOM:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotCodPjeCesionComision" runat="server" Text='<%# String.Format("{0:0.00}", Eval("CodPjeCesionComision")) %>'></asp:Label>

                                            </strong>
                                            <br />
                                            <asp:Label ID="Label22" runat="server" Text="Dif. TRA:"></asp:Label>
                                            <strong>
                                                <asp:Label ID="CotValTasaAjusteTra" runat="server" Text='<%# String.Format("{0:0.00}", Eval("ValTasaAjusteTra")) %>'></asp:Label>

                                            </strong>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="D COM" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2"  runat="server" ToolTip="DCOM" Text="D COM"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotCodPjeCesionComision" runat="server" Text='<%# String.Format("{0:0.00}", Eval("CodPjeCesionComision")) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <%--<asp:TemplateField HeaderText="Dif. TRA" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2"  runat="server" ToolTip="Dif. TRA" Text="Dif. TRA"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotValTasaAjusteTra" runat="server" Text='<%# String.Format("{0:0.00}", Eval("ValTasaAjusteTra")) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <%--<INIGTI_4081>--%>
                                    <%--<asp:TemplateField HeaderText="Recotización" ItemStyle-Width="10px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label3" runat="server" ToolTip="Re-Cotización" Text="RC."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotRecotizacion" runat="server" Text='<%# Eval("Recotizacion") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Competencia" ItemStyle-Width="20px">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label23" runat="server" ToolTip="Competencia" Text="Comp."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="CotCompetencia" runat="server" Text='<%# Eval("Compania.Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <%--<FINGTI_4081>--%>


                                    <asp:TemplateField ItemStyle-Width="30px">
                                        <ItemTemplate>
                                            <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar_flujo\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-cusspp=\"" + Eval("Afiliado.CUSPP") + "\" data-operacion=\"" + Eval("NumOperacion")) : " grilla_editar_flujo_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                                            <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_flujo\" data-solicitud=\"" + Eval("NumSolicitud") + "\" data-cusspp=\"" + Eval("Afiliado.CUSPP") + "\" data-operacion=\"" + Eval("NumOperacion")) : " grilla_flujo_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
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
                        <%# "<a  class='FecPresentacionData' data-fecha='div" + Convert.ToDateTime(Eval("FechaPresentacion")).ToString("yyyyMMdd") +  "'></a>"%>
                        <asp:Label ID="FecPresentacion"  runat="server" Text='<%# Convert.ToDateTime(Eval("FechaPresentacion")).ToString("dd/MM/yyyy") %>'></asp:Label>
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
            <div class="grilla_error" align="left" style="width: 375px">
                Usted no cuenta con privilegios para visualizar esta información.
            </div>
        </asp:Panel>

        <br />
        <%--<INIGTI_4081>--%>
        <asp:Panel ID="panelBotonera" runat="server" ClientIDMode="Static" align="center">
            <a id="ModSolAprobarBandejaBloque" style="width: 130px; height: 22px" class="boton darkblue sharp">Aprobar y Enviar</a>
            <a id="ModSolRechazarBandejaBloque" style="width: 130px; height: 22px" class="boton darkblue sharp">Rechazar</a>

            <br />
            <br />
            <div class="grilla_infoDer"><span id="asterisco"><strong>*</strong></span> Recotiza</div>
            <br />
        </asp:Panel>
        <%--<FINGTI_4081>--%>
    </div>
</form>
