<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="GestionVentas.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Reportes.GestionVentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e;">
        <h1 class="simple" style="width: 237px">Reporte de Gestión de Ventas</h1>

        <asp:Panel ID="ControlJefe" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabJefe" for="Jefe" class="formLabel formLabel2Izq">Jefe:</label>
            <asp:DropDownList ID="Jefe" runat="server" CssClass="formCombobox" Width="356" Enabled="False" ClientIDMode="Static">
            </asp:DropDownList>
            <asp:HiddenField ID="HJefe" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <asp:Panel ID="ControlSupervisor" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabSupervisor" for="Supervisor" class="formLabel formLabel2Izq">Supervisor:</label>
            <asp:DropDownList ID="Supervisor" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="356" Enabled="False" ClientIDMode="Static">
            </asp:DropDownList>
            <span id="CargandoSupervisor" class="paginador_cargando"></span>
            <asp:HiddenField ID="HSupervisor" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <asp:Panel ID="ControlAgente" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabAgente" for="Agente" class="formLabel formLabel2Izq">Agente:</label>
            <asp:DropDownList ID="Agente" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="356" Enabled="False" ClientIDMode="Static">
            </asp:DropDownList>
            <span id="CargandoAgente" class="paginador_cargando"></span>
            <asp:HiddenField ID="HAgente" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <div class="formLinea">
            <label id="LabCotizacionCerrada" for="CotizacionCerrada" class="formLabel formLabel2Izq">Cotización Cerrada*:</label>
            <asp:DropDownList ID="CotizacionCerrada" runat="server" CssClass="formCombobox" Width="50" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabCerrada" style="margin-left: -25px;">Fecha de Cierre Comercial</label>

            <asp:HiddenField ID="HCotizacionCerrada" runat="server" ClientIDMode="Static" />
        </div>

        <div class="formLinea">
            <label id="LabFechaDesde" for="FechaDesde" class="formLabel formLabel2Izq">Fecha desde*:</label>
            <asp:TextBox ID="FechaDesde" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
            <asp:HiddenField ID="HFechaDesde" runat="server" ClientIDMode="Static" />

            <label id="LabFechaHasta" for="FechaHasta" class="formLabel formLabel2Der">Fecha hasta*:</label>
            <asp:TextBox ID="FechaHasta" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
            <asp:HiddenField ID="HFechaHasta" runat="server" ClientIDMode="Static" />
        </div>



        <div class="formLinea">
            <label id="LabTipoCotizacion" for="TipoCotizacion" class="formLabel formLabel2Izq">Tipo de Cotización*:</label>
            <asp:DropDownList ID="TipoCotizacion" runat="server" CssClass="formCombobox" Width="180" ClientIDMode="Static">
            </asp:DropDownList>
            <asp:HiddenField ID="HTipoCotizacion" runat="server" ClientIDMode="Static" />


            <label id="LabCiaSeguro" for="CiaSeguro" class="formLabel formLabel2Der" style="margin-left: 102px;">Compañía de Seguro:</label>
            <asp:DropDownList ID="CiaSeguro" runat="server" CssClass="formCombobox" Width="180" ClientIDMode="Static">
            </asp:DropDownList>
            <asp:HiddenField ID="HCiaSeguro" runat="server" ClientIDMode="Static" />

        </div>

        <%--<div class="formLinea">
            <label id="LabCiaSeguro" for="CiaSeguro" class="formLabel formLabel2Izq">Compañía de Seguro:</label>
            <asp:DropDownList ID="CiaSeguro" runat="server" CssClass="formCombobox" Width="180" ClientIDMode="Static">
            </asp:DropDownList>
            <asp:HiddenField ID="HCiaSeguro" runat="server" ClientIDMode="Static" />
        </div>--%>

        <div class="formLinea" align="center">
            <asp:HyperLink ID="BuscarGestionVentas" Style="width: 110px; height: 22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
            <asp:HyperLink ID="EnviarExcelGestionVentas" Style="width: 110px; height: 22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Exportar Excel</asp:HyperLink>
            <asp:HyperLink ID="EnviarCorreoGestionVentas" Style="width: 110px; height: 22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static" Visible="false">Enviar Correo</asp:HyperLink>
        </div>

        <div id="TablaGestionVentasCargando" align="center" style="height: 50px; padding: 82px 0; display: none">
            <asp:Image ID="icoTablaGestionVentasCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando Consulta de Gestión de Ventas, espere por favor...</span>
        </div>

        <div id="TablaGestionVentasContenedor" style="display: none"></div>

        <br />



    </div>


    <div style="display: none">

        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCMIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCMEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCMEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCMEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCMMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCMBotonera" align="center">
                <a id="MCMAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>

        <%--Inicio Modal Cotizando--%>
        <%--<div id="ModalCargando">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:40px;height:40px"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
                                <asp:HiddenField ID="MCEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>--%>

        <div id="ModalCotizando">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <%--<div id="ModalCargando" title="Generando Excel" align="center" style="display:none">
            <asp:Image ID="Image1" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" />
            <span class="texto">Generando Excel, espere por favor.</span>
        </div>--%>
        <%--Fin Modal Cotizando--%>
    </div>
</asp:Content>
