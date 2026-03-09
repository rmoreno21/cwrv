<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="SeleccionarBeneficiarioCierre.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.SeleccionarBeneficiarioCierre" %>

<%@ Register Src="~/Controles/TablaRviBenefi2Mat.ascx" TagPrefix="uc1" TagName="TablaRviBenefi2Mat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Selección de Beneficiario</span>

                    <blockquote id="Mensaje" class="info">
                        <span id="instrucciones">
                            Por favor seleccione el beneficiario a nombre de quien saldrá el formato VCTP
                            y asegúrese de que sus datos registrados en la pestaña de <b>Grupo Familiar</b>
                            (Parentesco, Sexo y Fecha de Nacimiento) coincidan con los que se han registrado
                            en la <b>Solictud Oficial</b>.<br>
                            <br>
                            En caso de tratarse de un menor de edad, asegúrese también de que se haya
                            completado correctamente la información de su respectivo apoderado en la pestaña
                            de <b>Grupo Familiar</b>.
                        </span>
                    </blockquote>
                    
                    <div class="row">
                        <div class="col s12">
                            <uc1:TablaRviBenefi2Mat runat="server" id="TablaBeneficiarios" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $(document).ready(function () {
            $("#TabRviBenefi a").click(function (e) {
                // Check if the link has the disabled-beneficiary-link class
                if ($(this).hasClass('disabled-beneficiary-link')) {
                    // Prevent the click event and don't show loading screen
                    e.preventDefault();
                    return false;
                }
                // Only show loading screen for enabled links
                abrirModalCargando("Cargando datos del cliente, por favor espere un momento...");
            });
        });
    </script>
</asp:Content>
