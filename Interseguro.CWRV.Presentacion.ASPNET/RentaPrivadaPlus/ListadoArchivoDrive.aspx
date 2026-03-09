<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ListadoArchivoDrive.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.ListadoArchivoDrive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/lightbox.css")%>" />
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/google-drive.css")%>" />

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.google.drive.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rpparchivosdrive.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
      
    <script src="https://apis.google.com/js/api.js" type="text/javascript">

    </script>
    
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/lightbox.min.js")%>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/upload.js")%>"></script>

     <style type="text/css">
        #drive-box{
            margin-top:10px !important;
        }

        .formLabel2Izq {
            margin-left: 152px !important;
        }

        .formTextbox {
            margin-left: -165px !important;
        }

        .boton-Espacio {
            left: 6px!important;
        }

    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="ListadoArchivosRPP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 299px">Listado de Archivos Almacenados</h1>

        <div class="formLinea">
            <label id="LabSolNroSolicitud_RP" for="ModSolNroSolicitud_RP" class="formLabel">Nro. Solicitud:</label>
            <asp:Label ID="ModSolNroSolicitud_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />
        </div>

        <div class="formLinea">
            <label id="LabSolCuspp_RP" for="ModSolCuspp_RP" class="formLabel">Cuspp:</label>
            <asp:Label ID="ModSolCuspp_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

            <label id="LabSolNombre_RP" for="ModSolNombre_RP" class="formLabel">Titular:</label>
            <asp:Label ID="ModSolNombre_RP" runat="server" CssClass="formDato" ClientIDMode="Static" />
        </div>

        <div class="formLinea">
            <label id="LabSolEstadoOpe_RP" for="ModSolEstadoOpe_RP" class="formLabel">Estado Operaciones:</label>
            <asp:Label ID="ModSolEstadoOpe_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

            <label id="LabSolEstadoPlaft_RP" for="ModSolEstadoPlaft_RP" class="formLabel">Estado Plaft:</label>
            <asp:Label ID="ModSolEstadoPlaft_RP" runat="server" CssClass="formDato" ClientIDMode="Static" />
        </div>

        <asp:HiddenField ID="HNroSolicitud" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HEstadoPlaft" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="Hnombre" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="Hcuspp" runat="server" ClientIDMode="Static" Value="0" />

        <asp:HiddenField ID="HDescEstPlaft" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HDescEstOpe" runat="server" ClientIDMode="Static" Value="0" />

        <asp:HiddenField ID="HAprobarFlujoSolicitud" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HObservarFlujoSolicitud" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HRechazarFlujoSolicitud" runat="server" ClientIDMode="Static" Value="0" />

        <div>
            <div id="drive-box" class="hide">
	            <div id="drive-menu">
                    <div id="button-reload" title="Refrescar"></div>
                </div>
	            <div id="drive-content"></div>
	            <div id="error-message" class="flash hidden"></div>
	            <div id="status-message" class="flash hidden"></div>
            </div>
            <input type="file" id="fUpload" name="fUpload"  class="hide" multiple accept=".jpg, .jpeg, .pdf, .png"/>
        </div>

        
        <br />
        <div class="formLinea" align="center">
            <a id="ModSolObservar_RPP" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Observar</a>
            <a id="ModSolAprobar_RPP" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Aprobar</a>
            <a id="ModSolRechazar_RPP" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Rechazar</a>
            <a id="ModSolCancelar_RPP" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Cancelar</a>
            <%--<a id="ModSolObservar_RPP" href="javascript:void(0);" style="width: 180px; height: 22px" class="observacion boton darkblue sharp boton-Espacio">Observar</a>  --%>
        </div>
        <br />
        <div class="formLinea" id="divCargarObservacion">
            <label id="LabModSolObservacion_RP" for="observacion" class="formLabel">Observación:</label>
            <textarea class="formTextbox ColorNegro" id="observacion" cols="120" style="height: 28px" rows="6" onkeyup="auto_grow(this)"></textarea>      
            <%--<asp:TextBox ID="observacion" runat="server" CssClass="formTextbox ColorNegro" Width="180"  ClientIDMode="Static"></asp:TextBox>--%>
        </div>

        <%--<INI.GTI_26697>--%>
        <br />

        <fieldset>
            <legend>Lista de Documentos disponibles para descargar</legend>

            <div>
                <asp:GridView ID="TabDocumentos" runat="server" Width="80%" CellPadding="3" 
                    CellSpacing="1" GridLines="None"
                    ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" HorizontalAlign="Center">
                    <AlternatingRowStyle CssClass="grilla_alt2" />
                    <Columns>
                        <asp:BoundField DataField="Opcion"
                               HeaderText="Opcion" Visible="false">
                        </asp:BoundField>

                        <asp:BoundField DataField="Id"
                               HeaderText="Id" Visible="false">
                        </asp:BoundField>

                        <asp:BoundField DataField="FechaSolicitud"
                               HeaderText="FechaSolicitud" Visible="false">
                        </asp:BoundField>
                        
                        <asp:BoundField DataField="Documento"
                               HeaderText="Documento" >
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
            
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# "<a class=\"grilla_boton grilla_pdf\" data-solicitud=\"" + Eval("Id") + "\" data-opcion=\"" + Eval("Opcion") + "\" data-fechacotizacion=\"" + Eval("FechaSolicitud") + "\" title=\"Descargar\"></a>"%>
                            </ItemTemplate>
                            <ItemStyle Width="10%" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                
                    <EmptyDataTemplate>
                        <div class="grilla_info">
                            No se ha encontrado ningún registro de documentos.
                        </div>
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="grilla_cabecera" />
                    <RowStyle CssClass="grilla_alt1" />
                </asp:GridView>
            </div>
        </fieldset>
        <%--<FIN.GTI_26697>--%>

    </asp:Panel>

    <div id="float-box-text" class="float-box">
        <div class="info-form">
            <div class="close-x"><img id="imgCloseText" class="imgClose" src="<%=ResolveUrl("~/Estilos/images/button_close.png")%>" alt="close" /></div>
            <h3 class="clear">Text Content</h3>
            <div id="text-content"></div>
	        <button id="btnCloseText" value="Close" class="button btnClose">Close</button>
        </div>
    </div>

    <%--Inicio Modal Cuadro de mensajes--%>

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
            <a id="MCAAceptarFlujo_Plus" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
            <a id="MCACancelarFlujo_Plus" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
        </div>
    </div>

    <div id="ModalCuadroMensaje">
        <div>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="MCMIcono" style="width: 40px; height: 40px"></td>
                    <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                        <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                            
                            <asp:HiddenField ID="MCMEstadoIcono" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="MCMEstadoTitulo" runat="server" ClientIDMode="Static" />
                            <asp:Literal ID="MCMMensaje" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                    <asp:HiddenField ID="MCMEstado" runat="server" ClientIDMode="Static" Value="0" />
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

</asp:Content>
