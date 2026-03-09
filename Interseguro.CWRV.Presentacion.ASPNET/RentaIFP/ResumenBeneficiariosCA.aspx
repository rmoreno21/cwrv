<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ResumenBeneficiariosCA.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.GrupoFamiliarCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/ResumenBeneficiariosCA.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/jquery.steps.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="../Scripts/Common.js"></script>
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/jquery.steps.css")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:Panel ID="Resumen_Beneficiarios" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 100px">Beneficiarios</h1>

        <asp:HiddenField ID="ModGruFamsolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruNumCorrelativo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSolicitudSerializado" runat="server" ClientIDMode="Static" Value="0" />

        <div id="pasosCierre"></div>

        <fieldset>
            <legend>Resumen de Beneficiarios CA</legend>

            <div id="TablaBeneficiariosCargando_IFP" align="center" style="height: 50px; padding: 82px 0">
                <asp:Image ID="icoTablaBeneficiariosCargando_IFP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando cotizaciones, espere por favor...</span>
            </div>

            <div id="TablaBeneficiariosContenedor_IFP" style="display: none"></div>
        </fieldset>

        <div class="formLinea" align="center">
            <a id="ModSiguiente" style="width: 180px; height: 22px" class="boton darkblue sharp">Siguiente</a>
            <a id="ModCancelar" style="width: 180px; height: 22px" class="boton darkblue sharp">Cancelar</a>
            <a id="ModRegresar" style="width: 180px; height: 22px" class="boton darkblue sharp">Regresar</a>
            <asp:Button ClientIDMode="Static" ID="ModSolEditarBen" Style="visibility: hidden" runat="server" PostBackUrl="~/RentaIFP/GrupoFamiliarAfiliadoCierre.aspx" />
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
