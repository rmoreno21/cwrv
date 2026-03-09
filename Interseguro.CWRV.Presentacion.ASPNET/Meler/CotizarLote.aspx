<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CotizarLote.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Meler.CotizarLote" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.meler.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <style type="text/css">
        .cssload-container {
	        margin: -15px 0 0 0;
	        width: 99px;
	        height: 74px;
	        position: absolute;
        }
        .cssload-dot {
	        background: rgb(0,155,207);
	        border-radius: 50%; 
	        width: 25px;
	        height: 25px;
	        position: absolute;
	        bottom: 25px;
	        left: 22px;
	        transform-origin: center bottom;
		        -o-transform-origin: center bottom;
		        -ms-transform-origin: center bottom;
		        -webkit-transform-origin: center bottom;
		        -moz-transform-origin: center bottom;
	        animation: cssload-dot 0.69s ease-in-out infinite;
		        -o-animation: cssload-dot 0.69s ease-in-out infinite;
		        -ms-animation: cssload-dot 0.69s ease-in-out infinite;
		        -webkit-animation: cssload-dot 0.69s ease-in-out infinite;
		        -moz-animation: cssload-dot 0.69s ease-in-out infinite;
        }


        .step {
	        position: absolute;
	        width: 25px;
	        height: 25px;
	        border-top: 2px solid rgb(0,96,170);
	        top: 0;
	        right:0;
        }



        #cssload-s1 { animation: cssload-anim 2.07s linear infinite;
		        -o-animation: cssload-anim 2.07s linear infinite;
		        -ms-animation: cssload-anim 2.07s linear infinite;
		        -webkit-animation: cssload-anim 2.07s linear infinite;
		        -moz-animation: cssload-anim 2.07s linear infinite; }
        #cssload-s2 { animation: cssload-anim 2.07s linear infinite -0.69s;
		        -o-animation: cssload-anim 2.07s linear infinite -0.69s;
		        -ms-animation: cssload-anim 2.07s linear infinite -0.69s;
		        -webkit-animation: cssload-anim 2.07s linear infinite -0.69s;
		        -moz-animation: cssload-anim 2.07s linear infinite -0.69s; }
        #cssload-s3 { animation: cssload-anim 2.07s linear infinite -1.38s;
		        -o-animation: cssload-anim 2.07s linear infinite -1.38s;
		        -ms-animation: cssload-anim 2.07s linear infinite -1.38s;
		        -webkit-animation: cssload-anim 2.07s linear infinite -1.38s;
		        -moz-animation: cssload-anim 2.07s linear infinite -1.38s; }

        @keyframes cssload-dot {
         0% { transform: scale(1,.7); }
         20% { transform: scale(.7,1.2); }
         40% { transform: scale(1,1);} 
         50% { bottom: 82px;} 
         46% { transform: scale(1,1);} 
         80% { transform: scale(.7,1.2);} 
         90% { transform: scale(.7,1.2);} 
         100% { transform: scale(1,0.7);}
        }

        @-o-keyframes cssload-dot {
         0% { -o-transform: scale(1,.7); }
         20% { -o-transform: scale(.7,1.2); }
         40% { -o-transform: scale(1,1);} 
         50% { bottom: 82px;} 
         46% { -o-transform: scale(1,1);} 
         80% { -o-transform: scale(.7,1.2);} 
         90% { -o-transform: scale(.7,1.2);} 
         100% { -o-transform: scale(1,0.7);}
        }

        @-ms-keyframes cssload-dot {
         0% { -ms-transform: scale(1,.7); }
         20% { -ms-transform: scale(.7,1.2); }
         40% { -ms-transform: scale(1,1);} 
         50% { bottom: 82px;} 
         46% { -ms-transform: scale(1,1);} 
         80% { -ms-transform: scale(.7,1.2);} 
         90% { -ms-transform: scale(.7,1.2);} 
         100% { -ms-transform: scale(1,0.7);}
        }

        @-webkit-keyframes cssload-dot {
         0% { -webkit-transform: scale(1,.7); }
         20% { -webkit-transform: scale(.7,1.2); }
         40% { -webkit-transform: scale(1,1);} 
         50% { bottom: 82px;} 
         46% { -webkit-transform: scale(1,1);} 
         80% { -webkit-transform: scale(.7,1.2);} 
         90% { -webkit-transform: scale(.7,1.2);} 
         100% { -webkit-transform: scale(1,0.7);}
        }

        @-moz-keyframes cssload-dot {
         0% { -moz-transform: scale(1,.7); }
         20% { -moz-transform: scale(.7,1.2); }
         40% { -moz-transform: scale(1,1);} 
         50% { bottom: 82px;} 
         46% { -moz-transform: scale(1,1);} 
         80% { -moz-transform: scale(.7,1.2);} 
         90% { -moz-transform: scale(.7,1.2);} 
         100% { -moz-transform: scale(1,0.7);}
        }

        @keyframes cssload-anim { 
	        0% { 
		        opacity: 0;
		        top: 0; 
		        right: 0; 
	        }
	        50% { opacity: 1; }
	        100% { 
		        top: 74px; 
		        right: 74px;
		        opacity: 0;
	        }
        }

        @-o-keyframes cssload-anim { 
	        0% { 
		        opacity: 0;
		        top: 0; 
		        right: 0; 
	        }
	        50% { opacity: 1; }
	        100% { 
		        top: 74px; 
		        right: 74px;
		        opacity: 0;
	        }
        }

        @-ms-keyframes cssload-anim { 
	        0% { 
		        opacity: 0;
		        top: 0; 
		        right: 0; 
	        }
	        50% { opacity: 1; }
	        100% { 
		        top: 74px; 
		        right: 74px;
		        opacity: 0;
	        }
        }

        @-webkit-keyframes cssload-anim { 
	        0% { 
		        opacity: 0;
		        top: 0; 
		        right: 0; 
	        }
	        50% { opacity: 1; }
	        100% { 
		        top: 74px; 
		        right: 74px;
		        opacity: 0;
	        }
        }

        @-moz-keyframes cssload-anim { 
	        0% { 
		        opacity: 0;
		        top: 0; 
		        right: 0; 
	        }
	        50% { opacity: 1; }
	        100% { 
		        top: 74px; 
		        right: 74px;
		        opacity: 0;
	        }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e">
        <h1 class="simple" style="width:105px">Cotizar Lote</h1>

        <div class="formLinea">
            <label id="LabNumeroLote" for="NumeroLote" class="formLabel">Número de Lote:</label>
            <asp:TextBox ID="NumeroLote" runat="server" CssClass="enteroPositivo formTextbox" Width="60" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
            <asp:HyperLink ID="VerSolicitudes" style="width:130px;height:22px;margin-left:-20px;" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Ver Solicitudes</asp:HyperLink>
        </div>

        <div id="TablaSolicitudesLoteCargando" align="center" style="display:none">
            <asp:Image ID="icoTablaSolicitudesLoteCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando solicitudes del lote, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesLoteContenedor" style="display:none"></div>
        <div id="TablaSolicitudesLoteError" class="grilla_error" style="display:none">No se ha podido cargar la lista de solicitudes del lote. <a id="TablaSolicitudesLoteReintentar">Intentar de nuevo</a>.</div>
    
        <input type="hidden" id="usuario_actual" value="<%= Session["Usuario"] %>" />
        <input type="hidden" id="rol_azman" value="<%= Session["RolAzman"] %>" />
		<input type="hidden" id="url_api_rentas_rv" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiCotizadorRvUrl"] %>" />
    </div>

    <div id="VentanasModales" style="display:none">
        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCMIcono" style="width:40px;height:40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <a id="MCMAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>

        <%--Inicio Modal Cotizando Lote--%>
        <div id="ModalCotizandoLote" style="overflow:hidden">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:100px">
                            <div class="cssload-container">
	                            <div class="cssload-dot"></div>
	                            <div class="step" id="cssload-s1"></div>
	                            <div class="step" id="cssload-s2"></div>
	                            <div class="step" id="cssload-s3"></div>
                            </div>
                        </td>
                        <td id="MCContenedorMensaje" valign="middle">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0;color:#0060a9">
                                <asp:HiddenField ID="MCEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Panel ID="MCMensaje" runat="server" ClientIDMode="Static" align="left" style="margin-bottom:8px"></asp:Panel>
                                <asp:Panel ID="MCAdvertencia" runat="server" ClientIDMode="Static" align="left" style="color:#F00;font-weight:bold;margin-bottom:8px"></asp:Panel>
                                <div class="BarraFondo"><div class="BarraProgreso" style="width:0px"></div></div>
                                <asp:Panel ID="MCProgreso" runat="server" ClientIDMode="Static" align="right" style="margin-right:25px;margin-top:5px"></asp:Panel>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Cotizando Lote--%>
    </div>
</asp:Content>
