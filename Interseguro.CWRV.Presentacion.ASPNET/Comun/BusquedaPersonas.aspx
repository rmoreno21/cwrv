<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="BusquedaPersonas.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Comun.BusquedaPersonas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3" style="line-height:20px;margin-bottom:25px"><i class="small material-icons">person_search</i> Búsqueda de Personas</span>
                    <div class="row">
                        <div class="input-field col s12 m6">
                            <a name="ancla-solicitud"></a>
                            <i class="material-icons prefix">person</i>
                            <asp:TextBox ID="ApellidoPaterno" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <label for="ApellidoPaterno">Apellido Paterno</label>
                        </div>
                        <div class="input-field col s12 m6">
                            <a name="ancla-solicitud"></a>
                            <i class="material-icons prefix">person</i>
                            <asp:TextBox ID="ApellidoMaterno" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <label for="ApellidoMaterno">Apellido Materno</label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="input-field col s12">
                            <a name="ancla-solicitud"></a>
                            <i class="material-icons prefix">person</i>
                            <asp:TextBox ID="Nombres" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <label for="Nombres">Nombres</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col s12 center-align">
                            <asp:HyperLink ID="Buscar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">search</i>Buscar</asp:HyperLink>
                            <asp:HyperLink ID="Regresar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><span class="mdi mdi-magnify" style="font-size:1.3rem;line-height:inherit"></span>Regresar</asp:HyperLink>
                            <asp:HyperLink ID="HyperLink1" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">search</i>Regresar</asp:HyperLink>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col s12 center-align">
                            <div id="TablaPersonas" class="col s12 black-text center-align" style="display:none">
                            </div>
                            <div id="TablaPersonasCargando" class="col s12 valign-wrapper center-align" style="display:none">
                                <div class="preloader-wrapper small active" style="width:24px;height:24px">
                                    <div class="spinner-layer spinner-blue-only">
                                        <div class="circle-clipper left">
                                            <div class="circle"></div>
                                        </div><div class="gap-patch">
                                        <div class="circle"></div>
                                        </div><div class="circle-clipper right">
                                        <div class="circle"></div>
                                        </div>
                                    </div>
                                </div>
                                <span style="margin-left:10px">
                                    Cargando tabla de beneficiarios...
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $("#Buscar").click(function () {
            cargarTablaPersonas();
        });

        function cargarTablaPersonas() {
            $("#TablaPersonasCargando").show();
            $("#TablaPersonas").hide();
            $("#Buscar").attr("disabled", true);
            tablaPersonasCargada = false;

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                origen: "IFP",
                apellidoPaterno: $("#ApellidoPaterno").val(),
                apellidoMaterno: $("#ApellidoMaterno").val(),
                nombres: $("#Nombres").val()
            };

            $.ajax({
                type: "POST",
                url: "BusquedaPersonas.aspx/CargarTablaPersonas",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params),
                success: function (data) {
                    $("#TablaPersonas").html($(data.d).find("#ContenidoDinamico").html());
                    if (!$("#TabPersonasVacia").length){
                        $("#TabPersonas").DataTable({
                            searching: false,
                            lengthChange: false,
                            language: es_PE
                        });
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ManejarError(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function () {
                    configurarMascaras();
                    actualizarCombobox();

                    $("#TablaPersonas").show();
                    $("#TablaPersonasCargando").hide();
                    $("#Buscar").attr("disabled", false);
                }
            });
        }
    </script>
</asp:Content>
