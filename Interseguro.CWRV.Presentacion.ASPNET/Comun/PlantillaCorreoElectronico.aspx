<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="PlantillaCorreoElectronico.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Comun.PlantillaCorreoElectronico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Envío de correo manual</span>

                    <div class="row">
                        <div class="input-field col s12">
                            <asp:TextBox ID="NombreProceso" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="NombreProceso">Proceso</label>
                            <span id="NombreProcesoHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12">
                            <asp:TextBox ID="Asunto" runat="server" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                            <label for="Asunto">Asunto</label>
                            <span id="AsuntoHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col s12 center">
                            <asp:HyperLink ID="Copiar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">content_copy</i>Copiar cuerpo del correo</asp:HyperLink>
                            <div id="success" style="display:none; border: 1px solid red; padding:10px; margin-top: 10px;"><strong>¡Copiado!</strong></div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col s12 center">
                            <div id="plantilla-email" style="width:710px; border-bottom: 1px solid #000; padding: 10px 0; margin-bottom: 10px;"> 
                                <asp:Literal ID="PlantillaHTML" runat="server"></asp:Literal>
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
        $(document).ready(function () {
            /* Botón Copiar */
            $("#Copiar").click(function () {
                var str = $("#plantilla-email").html();

                function listener(e) {
                    e.clipboardData.setData("text/html", str);
                    e.clipboardData.setData("text/plain", str);
                    e.preventDefault();
                }
                document.addEventListener("copy", listener);
                document.execCommand("copy");
                document.removeEventListener("copy", listener);

                //$temp.remove();
                $("#success").show();
                $("#success").slideDown("slow");
                return false;
            });
        });
    </script>
</asp:Content>
