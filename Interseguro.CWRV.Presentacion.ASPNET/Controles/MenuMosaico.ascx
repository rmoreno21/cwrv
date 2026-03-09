<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuMosaico.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.MenuMosaico" %>
<script type="text/javascript">
    $(document).ready(function () {
        var opcion = 0;
        var cambiando = false;
        var menu_activo = false;
        var total_botones = 4;
        var contador = 0;
        $("#MenuMosaico a").click(function () {
            if (!cambiando && !menu_activo) {
                cambiando = true;
                opcion = $(this).attr("id");
                var elegido = $(this).children("span");
                $("#MenuMosaico a").not(this).each(function () {
                    $(this).hide("fast", function () {
                        contador++;
                        if (contador == total_botones - 1) {
                            contador = 0;
                            elegido.animate({ "left": "0", "top": "0" }, function () {
                                $("#SubmenuMosaico" + opcion).fadeIn("fast", function () {
                                    cambiando = false;
                                    menu_activo = true;
                                });
                            });
                        }
                    });
                });
            }
        });

        $(".submenu_regresar").click(function () {
            if (!cambiando && menu_activo) {
                cambiando = true;
                $(this).parent().parent().fadeOut("fast", function () {
                    $("#" + opcion + " span").animate({ "left": ((opcion - 1) * 144) + "px", "top": "0" }, function () {
                        $("#MenuMosaico a").fadeIn("fast");
                        cambiando = false;
                        menu_activo = false;
                    });
                });
            }
        });
    });
</script>
<div id="labelcontador"></div>
<div id="cambiando"></div>
<div id="menuactivo"></div>
<div id="ContenedorMenuMosaico" style="position:relative;width:578px;overflow:hidden;border:1px solid #0F0">
    <div id="MenuMosaico" style="position:relative;width:576px;height:164px;overflow:hidden;border:1px solid #F00">
        <a id="1">
            <span class="menu_mosaico_boton" style="top:0;left:0;">
                <img alt="" style="margin:20px" src="/Imagenes/menu_cotizador_64.png" />
                <br />
                COTIZADOR
            </span>
        </a>

        <a id="2">
            <span class="menu_mosaico_boton" style="top:0;left:144px;">
                <img alt="" style="margin:20px" src="/Imagenes/menu_simuladores_64.png" />
                <br />
                SIMULADORES
            </span>
        </a>

        <a id="3">
            <span class="menu_mosaico_boton" style="top:0;left:288px;">
                <img alt="" style="margin:20px" src="/Imagenes/menu_reportes_64.png" />
                <br />
                REPORTES
            </span>
        </a>

        <a id="4">
            <span class="menu_mosaico_boton" style="top:0;left:432px;">
                <img alt="" style="margin:20px" src="/Imagenes/menu_configuracion_64.png" />
                <br />
                CONFIGURACIÓN
            </span>
        </a>
    </div>
    <div id="SubmenuMosaico1" class="submenuMosaico" style="position:relative;width:576px;overflow:hidden;border:1px solid #FF0">
        <ul>
            <li class="submenu_extraoficiales">Cotizaciones extraoficiales</li>
            <li class="submenu_oficiales">Cotizaciones oficiales</li>
            <li class="submenu_sbs">Interfaces con el MELER</li>
            <li class="submenu_regresar">Regresar</li>
        </ul>
    </div>
    <div id="SubmenuMosaico2" class="submenuMosaico" style="position:relative;width:576px;overflow:hidden;border:1px solid #F0F">
        <ul>
            <li class="submenu_simulador">Jubilarse Hoy o a Futuro</li>
            <li class="submenu_simulador">Renta Vitalicia o Retiro Programado</li>
            <li class="submenu_simulador">Inmediata o Diferida</li>
            <li class="submenu_simulador">Tipo de Moneda</li>
            <li class="submenu_simulador">¿Qué me conviene</li>
            <li class="submenu_regresar">Regresar</li>
        </ul>
    </div>
    <div id="SubmenuMosaico3" class="submenuMosaico" style="position:relative;width:576px;overflow:hidden;border:1px solid #0FF">
        <ul>
            <li class="submenu_regresar">Regresar</li>
        </ul>
    </div>
    <div id="SubmenuMosaico4" class="submenuMosaico" style="position:relative;width:576px;overflow:hidden;border:1px solid #ABA">
        <ul>
            <li class="submenu_regresar">Regresar</li>
        </ul>
    </div>

</div>

<asp:Literal ID="Menu" runat="server"></asp:Literal>