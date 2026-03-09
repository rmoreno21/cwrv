<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="Cotizador.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.Cotizador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiParametro.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorIFP.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/cerrarSolicitud.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            Utilitarios.ValidarPermiso(EnumsPermisos.CotizacionesIFP);
        });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:Panel ID="BusquedaAfiliados_RP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: rgba(255, 255, 255, 0.8); border: 1px solid #00466e">
        <h1 class="simple" style="width: 264px">Cotizaciones Ingreso Flexible Plus</h1>

        <fieldset>
            <legend>Búsqueda de personas</legend>

            <div id="PestanhasBusqueda">
                <ul>
                    <li id="PesBusquedaIFP" data-pestanha="1"><a href="#">Por CUSSP</a></li>
                    <li id="PesBusquedaInteligo" data-pestanha="2"><a href="#">Por Documento</a></li>
                </ul>

                <div id="PestanhaBusqueda1" align="left" style="display: none; width: 792px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <div class="formLinea">
                        <label id="LabBusAfiNroSolicitud_RP" for="BusAfiNroSolicitud_RP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                        <asp:TextBox ID="BusAfiNroSolicitud_RP" runat="server" CssClass="formTextbox" Style="text-transform: uppercase" Width="170" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                        <label id="LabBusAfiCUSPP_RP" for="BusAfiCUSPP_RP" class="formLabel formLabel2Der">CUSPP:</label>
                        <asp:TextBox ID="BusAfiCUSPP_RP" runat="server" CssClass="formTextbox alfanumerico" Style="text-transform: uppercase" Width="165" ClientIDMode="Static" MaxLength="12"></asp:TextBox>

                        <asp:HyperLink ID="BusAfiExaminarSolicitud_RP" CssClass="boton darkblue sharp" Style="width: 22px; height: 22px; position: relative; margin-left: 5px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                        <asp:HiddenField ID="PerBusAfiExaminarSolicitud_RP" runat="server" ClientIDMode="Static" />
                    </div>

                    <asp:Panel ID="LineaBuscarAfiliado_RP" runat="server" ClientIDMode="Static" CssClass="formLinea" align="center">
                        <asp:Button ID="BusAfiBuscar_RP" CssClass="boton darkblue sharp" Width="80"
                            Height="24" runat="server" Text="Buscar" OnClick="BusAfiBuscar_RP_Click"
                            ClientIDMode="Static" />
                        <asp:HiddenField ID="PerBusAfiBuscar_RP" runat="server" ClientIDMode="Static" />
                    </asp:Panel>
                </div>

                <div id="PestanhaBusqueda2" align="left" style="display: none; width: 792px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <div class="formLinea">
                        <label id="LabTipoDocumentoBusqueda_RP" for="TipoDocumentoBusqueda_RP" class="formLabel formLabel2Izq">Tipo de Documento:</label>
                        <asp:DropDownList ID="TipoDocumentoBusqueda_RP" runat="server" CssClass="formCombobox" Width="170" ClientIDMode="Static" Style="margin-left: -120px !important; width: 260px!important;"></asp:DropDownList>

                        <label id="LabNumeroDocumentoBusqueda_RP" for="NumeroDocumentoBusqueda_RP" class="formLabel" style="margin-left: 30px;">Número de Documento:</label>
                        <asp:TextBox ID="NumeroDocumentoBusqueda_RP" runat="server" CssClass="formTextbox" Width="165" ClientIDMode="Static"></asp:TextBox>

                        <asp:HyperLink ID="BusAfiExaminarSolicitudBusqueda_RP" CssClass="boton darkblue sharp" Style="width: 22px; height: 22px; position: relative; margin-left: 3px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                        <asp:HiddenField ID="hcusppInteligo" runat="server" ClientIDMode="Static" />
                        <%--<asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" />--%>
                    </div>

                    <asp:Panel ID="LineaBuscar2Afiliado_RP" runat="server" ClientIDMode="Static" CssClass="formLinea" align="center">
                        <asp:Button ID="BusAfiBuscar2_RP" CssClass="boton darkblue sharp" Width="80"
                            Height="24" runat="server" Text="Buscar"
                            ClientIDMode="Static" OnClick="BusAfiBuscar2_RP_Click" />
                        <%--<asp:HiddenField ID="HiddenField2" runat="server" ClientIDMode="Static" />--%>
                    </asp:Panel>
                    <asp:HiddenField ID="hindPestaniaActiva" runat="server" ClientIDMode="Static" />
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend>Datos para la cotización</legend>

            <div id="Pestanhas">
                <ul>
                    <li id="Pes1" data-pestanha="1"><a href="#">Datos del Afiliado</a></li>
                    <li id="Pes2" data-pestanha="2"><a href="#">Grupo Familiar</a></li>
                    <li id="Pes3" data-pestanha="3"><a href="#">Solicitudes</a></li>
                </ul>

                <asp:HiddenField ID="PestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />

                <div id="Pestanha1" align="left" style="display: none; width: 792px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

                    <div class="grilla_info" style="display: none;">Validar que todos los campos* estén ingresados</div>

                    <div style="height: 5px;"></div>

                    <asp:Panel ID="PanelConsentimiento" runat="server" ClientIDMode="Static">
                    </asp:Panel>
                    <br />

                    <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
                    <asp:Panel ID="LineaCUSPP_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCUSPP_RP" for="CUSPP_RP" class="formLabel formLabel2Izq">CUSPP:</label>
                        <asp:TextBox ID="CUSPP_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <asp:HiddenField ID="HToken_IFP" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="HConsentimiento" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="HidConsentimientoAsesoria" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="HidConfiguracion" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="HindConsentimiento" runat="server" ClientIDMode="Static" />

                    <asp:Panel ID="LineaTipoDocumento" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabTipoDocumento_RP" for="TipoDocumento_RP" class="formLabel formLabel2Izq">Tipo de Documento*:</label>
                        <asp:DropDownList ID="TipoDocumento_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False"></asp:DropDownList>

                        <label id="LabNumeroDocumento_RP" for="NumeroDocumento_RP" class="formLabel formLabel2Der">Número de Documento*:</label>
                        <asp:TextBox ID="NumeroDocumento_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaApellidos_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabApellidoPaterno_RP" for="ApellidoPaterno_RP" class="formLabel formLabel2Izq">Apellido Paterno*:</label>
                        <asp:TextBox ID="ApellidoPaterno_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True" Style="text-transform: uppercase"></asp:TextBox>

                        <label id="LabApellidoMaterno_RP" for="ApellidoMaterno_RP" class="formLabel formLabel2Der">Apellido Materno*:</label>
                        <asp:TextBox ID="ApellidoMaterno_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True" Style="text-transform: uppercase"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaNombres_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabNombres_RP" for="Nombres_RP" class="formLabel formLabel2Izq">Nombres*:</label>
                        <asp:TextBox ID="Nombres_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="620" ClientIDMode="Static" ReadOnly="True" Style="text-transform: uppercase"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaNacimientoSexo_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabFechaNacimiento_RP" for="FechaNacimiento_RP" class="formLabel formLabel2Izq">Fecha de nacimiento*:</label>
                        <asp:TextBox ID="FechaNacimiento_RP" runat="server" Enabled="false" CssClass="formTextbox formCalendar formCalendarReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabSexo_RP" for="Sexo_RP" class="formLabel formLabel2Der">Sexo*:</label>
                        <asp:DropDownList ID="Sexo_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                        </asp:DropDownList>
                    </asp:Panel>

                    <asp:Panel ID="LineaCorreoElectronico_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCorreoElectronico_RP" for="CorreoElectronico_RP" class="formLabel formLabel2Izq">Correo Electrónico*:</label>
                        <asp:TextBox ID="CorreoElectronico_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        <asp:HiddenField ID="CorreoElectronicoRegistrado_RP" runat="server" ClientIDMode="Static" />

                        <label id="LabEstadoCivil_RP" for="EstadoCivil_RP" class="formLabel formLabel2Der">Estado Civil*:</label>
                        <asp:DropDownList ID="EstadoCivil_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False"></asp:DropDownList>
                    </asp:Panel>

                    <%--<asp:HiddenField ID="HCategoria_RP" runat="server" ClientIDMode="Static" Value="0" />--%>
                    <%--<asp:Panel ID="LineaCategoria_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCategoria_RP" for="Categoria_RP" class="formLabel formLabel2Izq">Categoría*:</label>
                        <asp:DropDownList ID="Categoria_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                        </asp:DropDownList>
                    </asp:Panel>--%>
                    
                    <asp:HiddenField ID="HCategoria_RP" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:Panel ID="LineaCategoria_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCategoria_RP" for="Categoria_RP" class="formLabel formLabel2Izq">Categoría:</label>
                        <asp:DropDownList ID="Categoria_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                        </asp:DropDownList>
                    </asp:Panel>

                    <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:Panel ID="LineaAFP_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabAFP_RP" for="AFP_RP" class="formLabel formLabel2Izq">AFP:</label>
                        <asp:DropDownList ID="AFP_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                        </asp:DropDownList>
                    </asp:Panel>

                    <asp:Panel ID="LineaSaldoCIC_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabSaldoCIC_RP" for="SaldoCIC_RP" class="formLabel formLabel2Izq">Fondo Aproximado(CIC)*:</label>
                        <asp:TextBox ID="SaldoCIC_RP" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>

                        <label id="LabRangoInversion_RP" for="RangoInversion_RP" class="formLabel formLabel2Der">Rango de inversión:</label>
                        <asp:TextBox ID="RangoInversion_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaCentroLaboral_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCentroLaboral_RP" for="CentroLaboral_RP" class="formLabel formLabel2Izq">Centro Laboral:</label>
                        <asp:TextBox ID="CentroLaboral_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaTelefonos_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabTelefono_RP" for="Telefono_RP" class="formLabel formLabel2Izq">Teléfono:</label>
                        <asp:TextBox ID="Telefono_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono" Width="180" ClientIDMode="Static" MaxLength="12" ReadOnly="True"></asp:TextBox>

                        <label id="LabCelular_RP" for="Celular_RP" class="formLabel formLabel2Der">Celular:</label>
                        <asp:TextBox ID="Celular_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono" Width="180" ClientIDMode="Static" MaxLength="9" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaAgente" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabAgente" for="Agente" class="formLabel formLabel2Izq">Agente:</label>
                        <asp:TextBox ID="Agente" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        <asp:HiddenField ID="NombreAgente" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="NumeroAgente" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="Cartera" runat="server" ClientIDMode="Static" />
                    </asp:Panel>

                    <br />

                    <asp:Panel ID="ContenedorGuardar_RP" CssClass="formLinea" align="center" runat="server" ClientIDMode="Static">
                        <asp:Button ID="Guardar_RP" CssClass="boton darkblue sharp" Width="80"
                            Height="24" runat="server" Text="Guardar" ClientIDMode="Static"
                            OnClick="Guardar_Click" />
                        <asp:HiddenField ID="PerGuardar_RP" runat="server" ClientIDMode="Static" />
                    </asp:Panel>

                    <asp:Panel ID="GrupoDireccion_RP" runat="server" ClientIDMode="Static">
                        <asp:HiddenField ID="ModDirModo" runat="server" ClientIDMode="Static" />
                        <div id="AgrupadorDireccion_RP" class="agrupador">
                            <span class="agrupador_titulo_mas">Dirección</span>
                        </div>

                        <div id="DatosDireccion_RP" style="display: none">
                            <div class="formLinea" align="right">
                                <asp:HyperLink ID="NuevaDireccion_RP" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static" Visible="False">Nueva</asp:HyperLink>
                                <asp:HiddenField ID="PerNuevaDireccion_RP" runat="server" ClientIDMode="Static" />
                            </div>

                            <div id="TablaDireccionesCargando_RP" align="center" style="display: none">
                                <asp:Image ID="icoTablaDireccionesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                                <span class="texto">Cargando direcciones, espere por favor...</span>
                            </div>
                            <div id="TablaDireccionesContenedor_RP" style="display: none"></div>
                            <div id="TablaDireccionesError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de direcciones. <a id="TablaDireccionesReintentar">Intentar de nuevo</a>.</div>
                            <input id="TabDireccionesDesplegar_RP" type="hidden" value="0" />

                            <br />
                        </div>
                        <a name="nueva_direccion"></a>
                    </asp:Panel>

                    <%--<asp:Panel ID="GrupoTelefono_RP" runat="server" ClientIDMode="Static">
                        <div id="AgrupadorTelefono_RP" class="agrupador">
                            <span class="agrupador_titulo_mas">Teléfono</span>
                        </div>

                        <div id="DatosTelefono_RP" style="display: none">
                            <div class="formLinea" align="right">
                                <asp:HyperLink ID="NuevoTelefono_RP" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static" Visible="False">Nuevo</asp:HyperLink>
                                <asp:HiddenField ID="PerNuevoTelefono_RP" runat="server" ClientIDMode="Static" />
                            </div>

                            <div id="TablaTelefonosCargando_RP" align="center" style="display: none">
                                <asp:Image ID="icoTablaTelefonosCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                                <span class="texto">Cargando teléfonos, espere por favor...</span>
                            </div>
                            <div id="TablaTelefonosContenedor_RP" style="display: none"></div>
                            <div id="TablaTelefonosError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de teléfonos. <a id="TablaTelefonosReintentar">Intentar de nuevo</a>.</div>
                            <input id="TabTelefonosDesplegar_RP" type="hidden" value="0" />

                            <br />
                        </div>
                        <a name="nuevo_telefono"></a>
                    </asp:Panel>--%>

                    <%--<asp:Panel ID="GrupoEmpresa_RP" runat="server" ClientIDMode="Static">--%>
                    <%--<div id="AgrupadorEmpresa_RP" class="agrupador">
                            <span class="agrupador_titulo_mas">Datos de la Empresa</span>
                        </div>--%>

                    <%-- <div id="DatosEmpresa_RP" style="display: none">--%>

                    <%--<div class="formLinea">
                                <label id="LabNombreEmpresa_RP" for="NombreEmpresa" class="formLabel formLabel2Izq">Empresa:</label>
                                <asp:TextBox ID="NombreEmpresa_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="620" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                            </div>--%>

                    <%-- <div class="formLinea">
                                <label id="LabDireccionEmpresa_RP" for="DireccionEmpresa" class="formLabel formLabel2Izq">Dirección:</label>
                                <asp:TextBox ID="DireccionEmpresa_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="620" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                            </div>--%>

                    <%--<div class="formLinea">
                                <label id="LabCiudadEmpresa_RP" for="CiudadEmpresa" class="formLabel formLabel2Izq">Ciudad:</label>
                                <asp:DropDownList ID="CiudadEmpresa_RP" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                                </asp:DropDownList>
                            </div>--%>

                    <%--<div class="formLinea">
                                <label id="LabComunaEmpresa_RP" for="ComunaEmpresa" class="formLabel formLabel2Izq">Comuna:</label>
                                <asp:DropDownList ID="ComunaEmpresa_RP" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                                </asp:DropDownList>
                            </div>--%>

                    <%--<div class="formLinea">
                                <label id="LabTelefonoEmpresa_RP" for="TelefonoEmpresa" class="formLabel formLabel2Izq">Teléfono:</label>
                                <asp:TextBox ID="TelefonoEmpresa_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                            </div>--%>

                    <%--</div>--%>
                    <%--</asp:Panel>--%>
                </div>

                <%-----------------------------------------------------------------------------------%>

                <div id="Pestanha2" align="left" style="display: none; width: 792px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <div class="grilla_info" style="display: none;">Antes de agregar nuevos beneficiarios, asegúrese de que el Afiliado ya figure en la lista, para ello guarde los datos del afiliado en la pestaña <strong>Datos del Afiliado</strong>.</div>

                    <div class="formLinea" align="right">
                        <asp:HyperLink ID="NuevoBeneficiario_RP" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static" Visible="False">Nuevo</asp:HyperLink>
                        <asp:HiddenField ID="PerNuevoBeneficiario_RP" runat="server" ClientIDMode="Static" />
                    </div>

                    <div id="TablaGrupoFamiliarCargando_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaGrupoFamiliarCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando grupo familiar, espere por favor...</span>
                    </div>
                    <div id="TablaGrupoFamiliarContenedor_RP" style="display: none"></div>
                    <div id="TablaGrupoFamiliarError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de grupo familiar. <a id="TablaGrupoFamiliarReintentar_RP">Intentar de nuevo</a>.</div>

                    <br />
                    <a name="grupo_familiar"></a>
                </div>

                <div id="Pestanha3" align="left" style="display: none; width: 792px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <a name="datos_solicitud"></a>
                    <asp:HiddenField ID="hPuedeCotizar" runat="server" ClientIDMode="Static" />
                    <%if (hPuedeCotizar.Value != "")
                        {%>
                    <div id="divPermiteCotizar" class="grilla_infoLN"><% =hPuedeCotizar.Value%></div>
                    <% }%>
                    <div id="divListaNegra" style="display: none" class="grilla_infoLN">Por Validar</div>
                    <div class="formLinea">
                        <input type="checkbox" name="chkTodasSolicitudes" id="chkTodasSolicitudes" title="Mostrar Todos(Incluir no vigentes)" />
                        Mostrar Todos

                        <asp:HyperLink ID="NuevaSolicitud_IFP" CssClass="boton darkblue sharp" Style="width: 220px; left: 470px" runat="server" ClientIDMode="Static" Visible="False">Nuevo Ingreso Flexible Plus</asp:HyperLink>
                        <asp:HiddenField ID="PerNuevaSolicitud_RP" runat="server" ClientIDMode="Static" />
                    </div>

                    <%--prueba--%>
                    <%--<div class="formLinea">
                        <asp:HyperLink ID="NuevaSolicitud_IFP2" CssClass="boton blue lighten-4" Style="width: 220px; left: 470px" runat="server" ClientIDMode="Static">Nuevo Ingreso Flexible Plus Prueba</asp:HyperLink>
                    </div>--%>

                    <div id="TablaSolicitudesCargando_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaSolicitudesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando solicitudes, espere por favor...</span>
                    </div>
                    <div id="TablaSolicitudesContenedor_RP" style="display: none"></div>
                    <div id="TablaSolicitudesError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar_RP">Intentar de nuevo</a>.</div>

                    <div id="TablaReporteEscenarioCargado_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaReporteEscenarioCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando reporte de escenarios, espere por favor...</span>
                    </div>

                    <br />
                </div>
            </div>
        </fieldset>
    </asp:Panel>

    <br />

    <%--Ventanas Modales--%>
    <div style="display: none">

        <%--Inicio Modal Búsqueda de Afiliados--%>
        <div id="ModalBusquedaAfiliados_RP" title="Búsqueda de afiliados">
            <div class="formLinea">
                <label id="LabModBusAfiApellidoPaterno_RP" for="ModBusAfiApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                <asp:TextBox ID="ModBusAfiApellidoPaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>

                <label id="LabModBusAfiApellidoMaterno_RP" for="ModBusAfiApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
                <asp:TextBox ID="ModBusAfiApellidoMaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea">
                <label id="LabModBusAfiNombres_RP" for="ModBusAfiNombres" class="formLabel formLabel2Izq">Nombres:</label>
                <asp:TextBox ID="ModBusAfiNombres_RP" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea" align="center">
                <asp:HyperLink ID="ModBusAfiBuscar_RP" NavigateUrl="#" Style="width: 80px; height: 22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
                <a id="ModBusAfiCancelar_RP" href="#" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
            </div>

            <br />

            <div id="ModBusAfiCargando_RP" align="center" style="display: none">
                <asp:Image ID="icoModBusAfiCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Buscando afiliados, espere por favor...</span>
            </div>
            <div id="TablaAfiliadosError_RP" align="center" style="display: none">
                <div class="grilla_error" align="left" style="width: 365px;">
                    No se ha podido cargar la tabla de afiliados. <a id="TablaAfiliadosReintentar_RP" href="#">Intentar de nuevo</a>.
                </div>
            </div>

            <div id="ModBusAfiTablaAfiliados_RP" runat="server" style="display: none" clientidmode="Static"></div>

            <asp:HiddenField ID="TabAfiliadosIndicePagina_RP" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosTamanhoPagina_RP" runat="server" ClientIDMode="Static" Value="10" />
            <asp:HiddenField ID="TabAfiliadosColumnaOrdenar_RP" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosDireccionOrdenar_RP" runat="server" ClientIDMode="Static" Value="A" />

        </div>
        <%--Fin Modal Búsqueda de Afiliados--%>

        <%--Inicio Modal Envío de Correo--%>
        <div id="ModalEnvioCorreo" title="Enviar Correo Electrónico">
            <div id="ModEnvCorCargando" class="modalCargandoContenido" style="width: 526px; height: 350px"></div>
            <div id="ModEnvCorContenido" class="modalContenido">
                <asp:HiddenField ID="ModEnvCorModo" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModEnvCorDe" class="formLabel formLabel2Izq">De:</label>
                    <asp:TextBox ID="ModEnvCorDe" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorPara" class="formLabel formLabel2Izq">Para:</label>
                    <asp:TextBox ID="ModEnvCorPara" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAsunto" class="formLabel formLabel2Izq">Asunto:</label>
                    <asp:TextBox ID="ModEnvCorAsunto" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAdjunto" class="formLabel formLabel2Izq">Adjunto:</label>
                    <label id="ModEnvCorAdjunto" class="formLabelAdjunto">Cotizacion.pdf (189kb)</label>
                </div>

                <div class="formLinea" style="height: 210px">
                    <asp:TextBox ID="ModEnvCorMensaje" runat="server" CssClass="formTextbox" Style="margin-left: 0px; height: 200px" Width="560" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModEnvCorEnviar" style="width: 80px; height: 22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelar" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                </div>

            </div>
        </div>
        <%--Fin Modal Envío de Correo--%>

        <%--Inicio Modal Ver Actividad--%>
        <div id="ModalActividad" title="Ver Actividad">
            <div id="ModActCargando" class="modalCargandoContenido" style="width: 526px; height: 250px"></div>
            <div id="ModActContenido" class="modalContenido">

                <div class="formLinea">
                    <label id="LabModActEvento" class="formLabel formLabel2Izq">Evento:</label>
                    <asp:TextBox ID="ModActEvento" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -168px" Width="470" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea" style="height: 210px">
                    <label id="LabModActComentario" class="formLabel formLabel2Izq">Comentario:</label>
                    <asp:TextBox ID="ModActComentario" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -168px; height: 200px" Width="470" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModActCerrar" style="width: 80px; height: 22px" class="boton darkblue sharp">Cerrar</a>
                </div>

            </div>
        </div>
        <%--Fin Modal ver Actividad--%>

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
                <a id="MCAAceptar_Plus" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar_Plus" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="indNuevoAfiliado" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>

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

        <%--Inicio Modal Generando Reporte--%>
        <div id="ModalGenerandoReporte">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MGRIcono" style="width: 40px; height: 40px"></td>
                        <td id="MGRContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MGRContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MGREstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MGREstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MRGEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MGRMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Generando Reporte--%>

        <%--Inicio Modal Seleccion de ACOM--%>
        <div id="ModalSeleccionAcom">
            <div>
                <div id="TablaSeleccionAcomCargando" align="center" style="display: none">
                    <asp:Image ID="Image2" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                    <span class="texto">Cargando registros de ACOM, espere por favor...</span>
                </div>

                <div id="DivErrorAcom" class="grilla_error" style="display: none">
                    No se ha podido cargar la tabla de ACOM. <a id="A1">Intentar de nuevo</a>.
                </div>

                <div id="TablaSeleccionAcomContenedor" style="display: none;"></div>
            </div>
            <br />
            <div id="DivBotoneraAcom" align="center">
                <a id="MRVGuardarSeleccionAcom" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MRVCancelarSeleccionAcom" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Seleccion de ACOM--%>

        <%--Inicio Modal Consentimiento de Asesoría--%>
        <div id="ModalConsentimientoAsesoria_IFP">
            <div id="ModConsentimientoAsesoriaCargando_IFP" class="modalCargandoContenido" style="display: none; width: 645px; height: 100px"></div>

            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCConstmAsesIcono_IFP" style="width: 40px; height: 40px"></td>
                        <td id="MCConstmAsesContenedorMensaje_IFP" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCConstmAsesContenedor_IFP" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCConstmAsesEstado_IFP" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCConstmAsesEstadoIcono_IFP" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCConstmAsesEstadoTitulo_IFP" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCConstmAsesMensaje_IFP" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCConstmAsesBotonera_IFP" align="center">
                <a id="MCConstmAsesEnviar_IFP" class="boton darkblue sharp" style="width: 80px">Enviar</a>
                <a id="MCConstmAsesCancelar_IFP" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>

        </div>
        <%--Fin Modal Consentimiento de Asesoría--%>

        <%--Inicio Modal Consentimiento de Asesoría SMS--%>
        <div id="ModalConsentimientoAsesoria_IFP_SMS">
            <div class="modalCargandoContenido" style="display: none; width: 645px; height: 100px"></div>

            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCConstmAsesIcono_IFP_SMS" style="width: 40px; height: 40px"></td>
                        <td valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCConstmAsesContenedor_IFP_SMS" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="HiddenField2" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="HiddenField3" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div align="center">
                <a id="MCConstmAsesEnviar_IFP_SMS" class="boton darkblue sharp" style="width: 80px">Enviar</a>
                <a id="MCConstmAsesCancelar_IFP_SMS" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>

        </div>
        <%--Fin Modal Consentimiento de Asesoría SMS--%>

        <%--Inicio Modal Reenvío Formato Consentimiento de Asesoria--%>
        <div id="ModalReenvioConsentimientoAsesoria">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MRCAIcono" style="width: 40px; height: 40px"></td>
                        <td id="MRCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MRCAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MRCAEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MRCAEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MRCAEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MRCAMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MRCABotonera" align="center">
                <a id="MRCAEnviar_IFP" class="boton darkblue sharp" style="width: 80px">Enviar</a>
                <a id="MRCACancelar_IFP" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>

        </div>
        <%--Fin Modal Reenvío Formato Consentimiento de Asesoria--%>
    </div>
</asp:Content>
