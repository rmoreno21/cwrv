<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsentimientoMensaje.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ConsentimientoMensaje" %>

<%--<asp:Panel ID="consentimiento_asesoria_advertencia" class="mensaje_advertencia_amarillo" runat="server" Visible="false">
    <asp:Label ID="lblMensaje_advertencia" runat="server"></asp:Label>
</asp:Panel>--%>

<asp:Panel ID="consentimiento_asesoria" runat="server" Visible="false">
    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
</asp:Panel>

<%--<div id="consentimiento_asesoria_advertencia" class="mensaje_advertencia_amarillo" style="display: none;"></div>--%>

<%--<asp:Panel ID="consentimiento_asesoria_exito" class="grilla_exito" runat="server" Visible="false">
     <asp:Label ID="lblMensaje_exito" runat="server"></asp:Label>
</asp:Panel>--%>

<%--<div id="consentimiento_asesoria_exito" class="grilla_exito" style="display: none;"></div>--%>

<asp:Panel ID="consentimiento_asesoria_sobrevivencia" runat="server" Visible="false">
    Este es un caso de Sobrevivencia, por favor seleccione un contacto:
                <asp:Panel ID="Panel1" runat="server" ClientIDMode="Static" CssClass="formLinea" Style="margin-left: 98px;">
                    <asp:DropDownList ID="dlBeneficiarios" runat="server" CssClass="formCombobox" Width="350" ClientIDMode="Static"></asp:DropDownList>
                </asp:Panel>
    <div id="consentimiento_asesoria_sobrevivencia_pie" style="display: none;">
        <a id="LinkConsentimientoAsesoriaSobrevivencia"><span class="material-icons" style="font-size:20px;margin-right:5px;vertical-align:bottom">email</span><span>Enviar enlace de consentimiento al contacto seleccionado: <b><asp:Label ID="lblCorreo" runat="server"></asp:Label></b></span></a>
    </div>
    
</asp:Panel>

<%--<asp:DropDownList ID="ddlBeneficiarios" CssClass="formCombobox" runat="server"></asp:DropDownList>--%>

<%--<div style="display: none">--%>

   <%--Inicio Modal Consentimiento de Asesoria 26560--%>
   <%-- <div id="ModalConsentimientoAsesoria">
        <div id="ModConsentimientoAsesoriaCargando" class="modalCargandoContenido" style="display: none; width: 645px; height: 100px"></div>

        <div>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="MCConstmAsesIcono" style="width: 40px; height: 40px"></td>
                    <td id="MCConstmAsesContenedorMensaje" valign="middle" class="cuadroMensaje">
                        <asp:Panel ID="MCConstmAsesContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                            <asp:HiddenField ID="MCConstmAsesEstado" runat="server" ClientIDMode="Static" Value="0" />
                            <asp:HiddenField ID="MCConstmAsesEstadoIcono" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="MCConstmAsesEstadoTitulo" runat="server" ClientIDMode="Static" />
                            <asp:Literal ID="MCConstmAsesMensaje" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
        <div id="MCConstmAsesBotonera" align="center">
            <a id="MCConstmAsesEnviar" class="boton darkblue sharp" style="width: 80px">Enviar</a>
            <a id="MCConstmAsesCancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
        </div>

    </div>--%>
    <%--Fin Modal Consentimiento de Asesoria 26560--%>

<%--</div>--%>
