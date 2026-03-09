<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ResumenBeneficiarios.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.ResumenBeneficiarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/ResumenBeneficiarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Resumen_Beneficiarios" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 197px">Beneficiarios</h1>

        <asp:HiddenField ID="ModGruFamsolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruNumCorrelativo" runat="server" ClientIDMode="Static" />

        <fieldset>
            <legend>Resumen de Beneficiarios</legend>

            <div id="TablaBeneficiariosCargando_IFP" align="center" style="height: 50px; padding: 82px 0">
                <asp:Image ID="icoTablaBeneficiariosCargando_IFP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando cotizaciones, espere por favor...</span>
            </div>

            <div id="TablaBeneficiariosContenedor_IFP" style="display: none"></div>
        </fieldset>

        <div class="formLinea" align="center">
            <a id="ModResumenBenefAceptar_IFP" style="width: 180px; height: 22px" class="boton darkblue sharp">Cierre de Cotización</a>
        </div>

    </asp:Panel>

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
    <%--Fin Modal Cotizando--%>
</asp:Content>
