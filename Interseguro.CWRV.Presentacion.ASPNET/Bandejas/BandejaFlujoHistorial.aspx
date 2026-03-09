<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="BandejaFlujoHistorial.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Bandejas.BandejaFlujoHistorial" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
<div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
    <h1 class="simple" style="width:240px">Historial de Flujo de Cambio</h1>
    <br />

    <%--<div id="ModFlujoSolCargando" class="modalCargandoContenido" style="width:848px;height:580px"></div>--%>
    <asp:Panel ID="ModFlujoSolCargando" class="modalCargandoContenido" runat="server" ClientIDMode="Static" ></asp:Panel>

    <%--<div id="TablaFlujoCotizacionesContenedor" style="display:none"></div>--%>
    <asp:Panel ID="TablaFlujoCotizacionesContenedor" runat="server" ClientIDMode="Static" ></asp:Panel>


    <div class="formLinea" align="center">
        <a id="ModFlujoSolCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
    </div>

    <%--
        <div id="TablaFlujoSolError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de movimientos de Cotización. <a id="TablaFlujoSolReintentar" >Intentar de nuevo</a>.</div>
        --%>

    <asp:Panel ID="TablaFlujoSolError" class="grilla_error" runat="server" ClientIDMode="Static" >
        No se ha podido cargar la tabla de movimientos de Cotización. <a id="TablaFlujoSolReintentar" >Intentar de nuevo</a>.
    </asp:Panel>

    <%--Inicio Modal Vista Flujo--%>
        <div id="ModalFlujoSolicitud" title="Datos de Flujo de Solicitud">
            
            <%--<div id="ModFlujoSolCargando" class="modalCargandoContenido" style="width:848px;height:580px"></div>--%>
            <div id="ModFlujoSolContenido" class="modalContenido">

                <asp:HiddenField ID="ManFlujoSolPestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManFlujoSolTipoSolicitud" runat="server" ClientIDMode="Static" />

                <div id="ManFlujoSolPestanha1" align="left" style="width:840px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
                        

                    

	            </div>

            </div>
        </div>
        <%--Fin Modal Vista Flujo--%>




        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCMIcono" style="width:40px;height:40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <a id="MCMAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>



    

</div>

</asp:Content>
