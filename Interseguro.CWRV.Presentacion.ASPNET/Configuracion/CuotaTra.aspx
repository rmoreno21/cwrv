<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CuotaTra.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Configuracion.CuotaTra" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            
            var permisoGrabarTra = ($('#HPerGrabar').val() == '1' ? true : false);

            var periodo = $('#Periodo').val();
            var mes  = $('#Mes').val();
            CargarTablaCuotasTra(periodo, mes);
            
            $('.numerico_int').numeric();

            //Cuando cambia de mes
            $('#Mes').live('change', function () {
                var periodo = $('#Periodo').val();
                var mes = $('#Mes').val();
                CargarTablaCuotasTra(periodo, mes);
            });

            //Cuando cambia de Periodo - Año
            $('#Periodo').live('keyup keydown keypress change', function (e) {
                if (e.which == 13) {
                    var periodo = $('#Periodo').val();
                    var mes = $('#Mes').val();
                    CargarTablaCuotasTra(periodo, mes);
                }
            });

            /*BEGIN: deshabilitar boton segun permiso*/
            if (!permisoGrabarTra) {
                $('#GrabarTRA').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            /*END: deshabilitar boton segun permiso*/

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e;">
        <h1 class="simple" style="width: 230px">Configuración de cuotas TRA</h1>

        <asp:HiddenField  id="HPerGrabar" value="1" runat="server" ClientIDMode="Static"/>

        <div class="formLinea">
            <label id="LabPeriodo" for="Periodo" class="formLabel formLabel2Izq">Periodo:</label>
            <asp:TextBox ID="Periodo" runat="server" MaxLength="4" Style="text-align: right" CssClass="formTextbox  numerico_int" Width="188" ClientIDMode="Static"></asp:TextBox>

            <label id="LabMes" for="Mes" class="formLabel formLabel2Der">Mes:</label>
            <asp:DropDownList ID="Mes" runat="server" CssClass="formCombobox" Width="210" ClientIDMode="Static">
            </asp:DropDownList>
        </div>

        <div id="TablaCuotasTraCargando" align="center" style="height: 50px; padding: 82px 0">
            <asp:Image ID="icoTablaCuotasTraCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando las Cuotas Tra, espere por favor...</span>
        </div>

        <div id="TablaCuotasTraContenedor" style="display: none"></div>

        <p></p>
        <div class="formLinea" align="center">
            <asp:HyperLink ID="GrabarTRA" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" ClientIDMode="Static">Registrar</asp:HyperLink>

            <asp:HyperLink ID="CancelarTRA" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" ClientIDMode="Static">Cancelar</asp:HyperLink>
        </div>
        <br />

        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div id="ModalCuadroMensajeCuerpo">
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
            <div id="ModalCuadroMensajeCargando" align="center" style="display: none">
                <asp:Image ID="Image2" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Espere por favor...</span>
            </div>
            <div id="MCMBotonera" align="center">
                <a id="MCMAceptar" class="boton darkblue sharp" style="width: 80px;">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>

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
                <a id="MCAAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>


        
        <%--Inicio Modal Cotizando--%>
        <div id="ModalCotizando">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:40px;height:40px"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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


    </div>
</asp:Content>

