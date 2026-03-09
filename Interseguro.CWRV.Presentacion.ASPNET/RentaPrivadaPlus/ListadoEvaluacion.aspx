<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ListadoEvaluacion.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.ListadoEvaluacion1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rppevaluacion.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rppevaluacion.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    
    <%--<asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />--%>
    
     <asp:Panel ID="BusquedaAfiliados_RP" runat="server" ClientIDMode="Static" align="left" style="width:860px;padding:20px;background:rgba(255, 255, 255, 0.8); border:1px solid #00466e">
        <h1 class="simple" style="width:276px">Cotizaciones de Rentas Particulares</h1>
        
        <br />

        <fieldset>
            
            <legend>Listado de Solicitudes en Evaluación</legend>

            <div id="TablaSolicitudesCargando_RPP_Evaluacion" align="center" style="display:none">
                <asp:Image ID="icoTablaSolicitudesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando solicitudes, espere por favor...</span>
            </div>
            <div id="TablaSolicitudesContenedor_RPP_Evaluacion" style="display:none"></div>
            <div id="TablaSolicitudesError_RPP_Evaluacion" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar_RP">Intentar de nuevo</a>.</div>
                    
        </fieldset>
    </asp:Panel>

    <br />

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

</asp:Content>
