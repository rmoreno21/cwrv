<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ResumenBeneficiariosPG.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.ResumenBeneficiariosPG" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/ResumenBeneficiariosPG.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/jquery.steps.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="../Scripts/Common.js"></script>
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/jquery.steps.css")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Resumen_Beneficiarios" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 110px">Beneficiarios</h1>

        <asp:HiddenField ID="ModGruFamsolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruNumCorrelativo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSolicitudSerializado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HCUSPP" runat="server" ClientIDMode="Static" />

        <asp:HiddenField ID="HNombres" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCorreo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HTipoDocumento" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HNumeroDocumento" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HTelefono" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HToken" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HPEP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSoloNombre" runat="server" ClientIDMode="Static" />

        <div id="pasosCierre"></div>
        <br />

        <fieldset>
            <legend>Resumen de Beneficiarios del Periodo Garantizado</legend>

            <div id="TablaBeneficiariosCargando_IFP" align="center" style="height: 50px; padding: 82px 0">
                <asp:Image ID="icoTablaBeneficiariosCargando_IFP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando beneficiarios, espere por favor...</span>
            </div>

            <div id="TablaBeneficiariosContenedor_IFP" style="display: none"></div>
        </fieldset>

        <br />

        <fieldset id="tabla_VIT" style="display:none">
            <legend>Resumen de Beneficiarios del Periodo No Garantizado</legend>

            <div id="TablaBeneficiariosCargando_IFP_VIT" align="center" style="height: 50px; padding: 82px 0">
                <asp:Image ID="Image1" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando beneficiarios, espere por favor...</span>
            </div>

            <div id="TablaBeneficiariosContenedor_IFP_VIT" style="display: none"></div>
        </fieldset>
        
        <br />

        <div class="formLinea" align="center">
            <a id="ModAceptarCierre" style="width: 150px; height: 22px" class="boton darkblue sharp">Enviar al cliente</a>
            <asp:HyperLink ID="ReenvioManual" Visible="false" ClientIDMode="Static" runat="server" style="width: 150px; height: 22px" CssClass="boton darkblue sharp">Reenvío manual</asp:HyperLink>
            <a id="ModVistaPrevia" style="width: 150px; height: 22px" class="boton darkblue sharp">Vista Previa</a>
            <a id="ModRegresar" style="width: 150px; height: 22px" class="boton darkblue sharp">Regresar</a>
            <a id="ModGruFamCancelar_RP" style="width: 150px; height: 22px" class="boton darkblue sharp">Cancelar</a>
            <asp:Button ID="ModSolSiguiente_RP" Style="display: none" runat="server" Text="Siguiente cierre" />
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

    <%--Inicio Modal Cuadro de advertencia--%>
    <div id="ModalCuadroAdvertencia">
        <div>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="MCAIcono" style="width: 40px; height: 40px"></td>
                    <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                        <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                            <asp:HiddenField ID="MCAEstado" runat="server" ClientIDMode="Static" Value="0" />
                            <asp:HiddenField ID="MCAEstadoIcono" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="MCAEstadoTitulo" runat="server" ClientIDMode="Static" />
                            <asp:Literal ID="MCAMensaje" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
        <div id="MCABotonera" align="center">
            <a id="MCASiBeneficiario" class="boton darkblue sharp" style="width: 80px">Si</a>
            <a id="MCANoBeneficiario" class="boton darkblue sharp" style="width: 80px">No</a>
        </div>
    </div>

    <asp:HiddenField ID="MCATablaPregunta" runat="server" ClientIDMode="Static" />
    <%--Fin Modal Cuadro de advertencia--%>
</asp:Content>
