<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="SolicitudEvaluacion.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaParticular.SolicitudEvaluacion" %>

<%@ Register Src="~/Controles/DocumentosFirmaDigitalHistorico.ascx" TagPrefix="uc1" TagName="DocumentosFirmaDigitalHistorico" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/lightbox.css")%>" />
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/google-drive.css")%>" />

    <style type="text/css">
        #drive-box{
            margin-top:10px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="HEstadoPlaft" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="Hnombre" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="HCUSPP" runat="server" ClientIDMode="Static" Value="0" />

    <asp:HiddenField ID="HDescEstPlaft" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="HDescEstOpe" runat="server" ClientIDMode="Static" Value="0" />

    <asp:HiddenField ID="HAprobarFlujoSolicitud" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="HObservarFlujoSolicitud" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="HRechazarFlujoSolicitud" runat="server" ClientIDMode="Static" Value="0" />

    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Solicitud en Evaluación</span>
                    <div class="row">
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="NumeroSolicitud" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="NumeroSolicitud">N° de Solicitud</label>
                            <span id="NumeroSolicitudHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="CUSPP" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="CUSPP">CUSPP</label>
                            <span id="CUSPPHelper" class="helper-text"></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="input-field col s12">
                            <asp:TextBox ID="Titular" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="Titular">Titular</label>
                            <span id="TitularHelper" class="helper-text"></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="EstadoOperaciones" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="EstadoOperaciones">Estado Operaciones</label>
                            <span id="EstadoOperacionesHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="EstadoPLAFT" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="EstadoPLAFT">Estado PLAFT</label>
                            <span id="EstadoPLAFTHelper" class="helper-text"></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col s12">
                            <label style="font-size:0.8rem">Documentos Sustentatorios</label>
                            <div id="drive-box">
	                            <div id="drive-content"></div>
	                            <div id="error-message" class="flash hidden"></div>
	                            <div id="status-message" class="flash hidden"></div>
                            </div>
                            <input type="file" id="fUpload" name="fUpload" class="hide" multiple accept=".jpg, .jpeg, .pdf, .png"/>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col s12">
                            <label style="font-size:0.8rem">Documentos Firmados Digitalmente</label>
                        </div>
                        <uc1:DocumentosFirmaDigitalHistorico runat="server" ID="DocumentosFirmaDigitalHistorico" />
                    </div>
                    <asp:Panel ID="SeccionObservacion" CssClass="row" runat="server">
                        <div class="input-field col s12">
                            <textarea id="Observacion" class="materialize-textarea"></textarea>
                            <label for="Observacion">Observación</label>
                            <span id="ObservacionHelper" class="helper-error red-text"></span>
                        </div>
                    </asp:Panel>
                    <div class="row">
                        <div class="col s12 center">
                            <asp:HyperLink ID="Observar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">visibility</i>Observar</asp:HyperLink>
                            <asp:HyperLink ID="Aprobar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">check_circle</i>Aprobar</asp:HyperLink>
                            <asp:HyperLink ID="Rechazar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">do_not_disturb</i>Rechazar</asp:HyperLink>
                            <asp:HyperLink ID="Regresar" NavigateUrl="~/RentaParticular/SolicitudesEvaluacion.aspx" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">arrow_back</i>Regresar</asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.google.drive.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script src="https://apis.google.com/js/api.js" type="text/javascript"></script>

    <script>
        var tipoFlujo;

        $(document).ready(function () {
            ObtenerArchivos($("#NumeroSolicitud").val(), true);

            /* Botón Aprobar */
            $("#Aprobar").click(function () {
                limpiarErrores();
                tipoFlujo = 6;
                if ($("#Observacion").val().length == 0) {
                    $("#Observacion").val("Solicitud Aprobada");
                }
                abrirModalConfirmacion("Confirmación", "¿Confirma que desea aprobar esta Solicitud?", "report_problem", "#ffab00");
                return false;
	        });
    	
            /* Botón Rechazar */
            $("#Rechazar").click(function () {
                limpiarErrores();
                let esValido = true;

                // Validar que se haya colocado una observación
                if ($("#Observacion").val().length == 0) {
                    $("#ObservacionHelper").html("Campo obligtorio");
                    esValido = false;
                }

                if (esValido) {
                    tipoFlujo = 7;
                    abrirModalConfirmacion("Confirmación", "¿Confirma que desea rechazar esta Solicitud?", "report_problem", "#ffab00");
                }
	        });

            /* Botón Observar */
            $("#Observar").click(function () {
                limpiarErrores();
                let esValido = true;

                // Validar que se haya colocado una observación
                if ($("#Observacion").val().length == 0) {
                    $("#ObservacionHelper").html("Campo obligtorio");
                    esValido = false;
                }

                if (esValido) {
                    tipoFlujo = 5;
                    abrirModalConfirmacion("Confirmación", "¿Confirma que desea observar esta Solicitud?", "report_problem", "#ffab00");
                }
            });

            /* Botón Observar */
            $("#Regresar").click(function () {
                deshabilitarBotones();
            });

            $("#modal-confirmar-aceptar").click(function () {
                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    solicitud: $("#NumeroSolicitud").val(),
                    observacion: $("#Observacion").val(),
                    codEstadoRPP: tipoFlujo
                };

                abrirModalCargando("Procesando solicitud, por favor espere...");

                $.ajax({
                    type: "POST",
                    url: "SolicitudEvaluacion.aspx/ActualizarFlujoSolicitud",
                    contentType: "application/json; charset=utf8",
                    dataType: "json",
                    data: JSON.stringify(params),
                    success: function (data) {
                        if (data.d.Estado == "OK") {
                            abrirModalAlerta("Proceso exitoso", data.d.Mensaje, "check_circle", "#2e7d32")
                            setTimeout(function () {
                                window.location.href = "<%=ResolveUrl("~/RentaParticular/SolicitudesEvaluacion.aspx")%>";
                            }, 3000);
                        }
                        else if (data.d.Estado == "TOKEN") {
                            cerrarSesionExpirada();
                        }
                        else {
                            abrirModalAlerta(data.d.Titulo, data.d.Mensaje, "error", "#c62828")
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        cerrarModalCargando();
                    }
                });
            });
        });
    </script>
</asp:Content>
