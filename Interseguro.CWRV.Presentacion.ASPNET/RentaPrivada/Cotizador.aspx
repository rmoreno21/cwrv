<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="Cotizador.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivada.Cotizador" %>



<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rentaprivada.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rentaprivada.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            if ($('#SaldoCIC').val() != 0) $('#SaldoCIC').val(formatearMonto($('#SaldoCIC').val()));
            if ($('#ModSolSaldoCIC').val() != 0) $('#ModSolSaldoCIC').val(formatearMonto($('#ModSolSaldoCIC').val()))

        });

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:Panel ID="BusquedaAfiliados_RP" runat="server" ClientIDMode="Static" align="left" style="width:860px;padding:20px;background:rgba(255, 255, 255, 0.8); border:1px solid #00466e">
        <h1 class="simple" style="width:222px">Cotizaciones Renta Privada</h1>
        
        <fieldset>
            <legend>Búsqueda de afiliados</legend>
            <div class="formLinea">
                <label id="LabBusAfiNroSolicitud_RP" for="BusAfiNroSolicitud_RP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                <asp:TextBox ID="BusAfiNroSolicitud_RP" runat="server" CssClass="formTextbox" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                <label id="LabBusAfiCUSPP_RP" for="BusAfiCUSPP_RP" class="formLabel formLabel2Der">CUSPP:</label>
                <asp:TextBox ID="BusAfiCUSPP_RP" runat="server" CssClass="formTextbox alfanumerico" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
            
                <asp:HyperLink ID="BusAfiExaminarSolicitud_RP" CssClass="boton darkblue sharp" style="width:22px;height:22px;position:relative;margin-left:5px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                <asp:HiddenField ID="PerBusAfiExaminarSolicitud_RP" runat="server" ClientIDMode="Static" />
            </div>

            <asp:Panel ID="LineaBuscarAfiliado_RP" runat="server" ClientIDMode="Static" CssClass="formLinea" align="center">
                <asp:Button ID="BusAfiBuscar_RP" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Buscar" onclick="BusAfiBuscar_RP_Click" 
                    ClientIDMode="Static" />
                <asp:HiddenField ID="PerBusAfiBuscar_RP" runat="server" ClientIDMode="Static" />
            </asp:Panel>
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
                <%------------------------------------------------------------------------------------------%>
                <div id="Pestanha1" align="left" style="display:none;width:792px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">

                    <div class="grilla_info">Para actualizar los Datos del Afiliado (con excepción de la AFP y la CIC) por favor use el vtiger.</div>
            
                    <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
                    <asp:Panel ID="LineaCUSPP_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCUSPP_RP" for="CUSPP_RP" class="formLabel formLabel2Izq">CUSPP:</label>
                        <asp:TextBox ID="CUSPP_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaApellidos_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabApellidoPaterno_RP" for="ApellidoPaterno_RP" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                        <asp:TextBox ID="ApellidoPaterno_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabApellidoMaterno_RP" for="ApellidoMaterno_RP" class="formLabel formLabel2Der">Apellido Materno:</label>
                        <asp:TextBox ID="ApellidoMaterno_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaNombres_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabNombres_RP" for="Nombres_RP" class="formLabel formLabel2Izq">Nombres:</label>
                        <asp:TextBox ID="Nombres_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="620" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="LineaNacimientoSexo_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabFechaNacimiento_RP" for="FechaNacimiento_RP" class="formLabel formLabel2Izq">Fecha de nacimiento:</label>
                        <asp:TextBox ID="FechaNacimiento_RP" runat="server" CssClass="formTextbox formCalendarReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabSexo_RP" for="Sexo_RP" class="formLabel formLabel2Der">Sexo:</label>
                        <asp:DropDownList ID="Sexo_RP" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                        </asp:DropDownList>
                    </asp:Panel>

                    <asp:Panel ID="LineaCorreoElectronico_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCorreoElectronico_RP" for="CorreoElectronico_RP" class="formLabel formLabel2Izq">Correo Electrónico*:</label>
                        <asp:TextBox ID="CorreoElectronico_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        <asp:HiddenField ID="CorreoElectronicoRegistrado_RP" runat="server" ClientIDMode="Static" />
                    </asp:Panel>

                    <asp:HiddenField ID="HCategoria_RP" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:Panel ID="LineaCategoria_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabCategoria_RP" for="Categoria_RP" class="formLabel formLabel2Izq">Categoría*:</label>
                        <asp:DropDownList ID="Categoria_RP" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                        </asp:DropDownList>
                    </asp:Panel>

                    <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:Panel ID="LineaAFP_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabAFP_RP" for="AFP_RP" class="formLabel formLabel2Izq">AFP*:</label>
                        <asp:DropDownList ID="AFP_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                        </asp:DropDownList>
                    </asp:Panel>

                    <asp:Panel ID="LineaSaldoCIC_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabSaldoCIC_RP" for="SaldoCIC_RP" class="formLabel formLabel2Izq">Saldo CIC*:</label>
                        <asp:TextBox ID="SaldoCIC_RP" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>
                    </asp:Panel>

            
                    <%--<asp:Panel ID="LineaListaCIC" runat="server" ClientIDMode="Static" CssClass="formLinea" Visible="False">
                        <label id="LabListaCIC" for="ListaCIC" class="formLabel formLabel2Izq">Saldo CIC*:</label>
                        <asp:DropDownList ID="ListaCIC" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                        </asp:DropDownList>
                    </asp:Panel>--%>

                    <br />

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

                    <asp:Panel ID="GrupoTelefono_RP" runat="server" ClientIDMode="Static">
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
                    </asp:Panel>

                    <asp:Panel ID="GrupoEmpresa_RP" runat="server" ClientIDMode="Static">
                    <div id="AgrupadorEmpresa_RP" class="agrupador">
                        <span class="agrupador_titulo_mas">Datos de la Empresa</span>
                    </div>

                    <div id="DatosEmpresa_RP" style="display:none">

                        <div class="formLinea">
                                <label id="LabNombreEmpresa_RP" for="NombreEmpresa" class="formLabel formLabel2Izq">Empresa:</label>
                            <asp:TextBox ID="NombreEmpresa_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="620" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                                <label id="LabDireccionEmpresa_RP" for="DireccionEmpresa" class="formLabel formLabel2Izq">Dirección:</label>
                            <asp:TextBox ID="DireccionEmpresa_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="620" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                                <label id="LabCiudadEmpresa_RP" for="CiudadEmpresa" class="formLabel formLabel2Izq">Ciudad:</label>
                            <asp:DropDownList ID="CiudadEmpresa_RP" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                            </asp:DropDownList>
                        </div>

                        <div class="formLinea">
                                <label id="LabComunaEmpresa_RP" for="ComunaEmpresa" class="formLabel formLabel2Izq">Comuna:</label>
                            <asp:DropDownList ID="ComunaEmpresa_RP" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                            </asp:DropDownList>
                        </div>

                        <div class="formLinea">
                                <label id="LabTelefonoEmpresa_RP" for="TelefonoEmpresa" class="formLabel formLabel2Izq">Teléfono:</label>
                            <asp:TextBox ID="TelefonoEmpresa_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>
                
                    </div>
                    </asp:Panel>

                    <asp:Panel ID="ContenedorGuardar_RP" CssClass="formLinea" align="center" runat="server" ClientIDMode="Static">
                        <asp:Button ID="Guardar_RP" CssClass="boton darkblue sharp" Width="80" 
                            Height="24" runat="server" Text="Guardar" ClientIDMode="Static" 
                            onclick="Guardar_Click" />
                        <asp:HiddenField ID="PerGuardar_RP" runat="server" ClientIDMode="Static" />
                    </asp:Panel>
                </div>

                <%------------------------------------------------------------------------------------------%>

                <div id="Pestanha2" align="left" style="display:none;width:792px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
                    <div class="grilla_info">Antes de agregar nuevos beneficiarios, asegúrese de que el Afiliado ya figure en la lista, para ello guarde los datos del afiliado en la pestaña <strong>Datos del Afiliado</strong>.</div>

                    <div class="formLinea" align="right">
                        <asp:HyperLink ID="NuevoBeneficiario_RP" CssClass="boton darkblue sharp" style="width:80px;height:22px" runat="server" ClientIDMode="Static" Visible="False">Nuevo</asp:HyperLink>
                        <asp:HiddenField ID="PerNuevoBeneficiario_RP" runat="server" ClientIDMode="Static" />
                    </div>

                    <div id="TablaGrupoFamiliarCargando_RP" align="center" style="display:none">
                        <asp:Image ID="icoTablaGrupoFamiliarCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando grupo familiar, espere por favor...</span>
                    </div>
                    <div id="TablaGrupoFamiliarContenedor_RP" style="display:none"></div>
                    <div id="TablaGrupoFamiliarError_RP" class="grilla_error" style="display:none">No se ha podido cargar la tabla de grupo familiar. <a id="TablaGrupoFamiliarReintentar_RP" >Intentar de nuevo</a>.</div>

                    <br />
                    <a name="grupo_familiar"></a>
                </div>

                <div id="Pestanha3" align="left" style="display:none;width:792px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
                    <a name="datos_solicitud"></a>
                    <div class="formLinea" align="right">
                        <asp:HyperLink ID="NuevaSolicitud_RP" CssClass="boton darkblue sharp" style="width:170px" runat="server" ClientIDMode="Static" Visible="False">Nueva Renta Privada</asp:HyperLink>
                        <asp:HiddenField ID="PerNuevaSolicitud_RP" runat="server" ClientIDMode="Static" />
                    </div>

                    <div id="TablaSolicitudesCargando_RP" align="center" style="display:none">
                        <asp:Image ID="icoTablaSolicitudesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando solicitudes, espere por favor...</span>
                    </div>
                    <div id="TablaSolicitudesContenedor_RP" style="display:none"></div>
                    <div id="TablaSolicitudesError_RP" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar_RP">Intentar de nuevo</a>.</div>

                    <%--<SRIINI06326>--%>
                    <div id="TablaReporteEscenarioCargado_RP" align="center" style="display:none">
                        <asp:Image ID="icoTablaReporteEscenarioCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando reporte de escenarios, espere por favor...</span>
                    </div>
                    <%--<SRIFIN06326>--%>
                    <br />
                </div>
            </div>
        </fieldset>
    </asp:Panel>

    <br />

    <%--Ventanas Modales--%> 
    <%--Ini Ya esta en controles--%>
    <div style="display:none">

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
                <asp:HyperLink ID="ModBusAfiBuscar_RP" NavigateUrl="#" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
                <a id="ModBusAfiCancelar_RP" href="#" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
            </div>

            <br />

            <div id="ModBusAfiCargando_RP" align="center" style="display:none">
                <asp:Image ID="icoModBusAfiCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Buscando afiliados, espere por favor...</span>
            </div>
            <div id="TablaAfiliadosError_RP" align="center" style="display:none">
                <div class="grilla_error" align="left" style="width:365px;">
                    No se ha podido cargar la tabla de afiliados. <a id="TablaAfiliadosReintentar_RP" href="#">Intentar de nuevo</a>.
                </div>
            </div>

            <div id="ModBusAfiTablaAfiliados_RP" runat="server" style="display:none" clientidmode="Static"></div>

            <asp:HiddenField ID="TabAfiliadosIndicePagina_RP" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosTamanhoPagina_RP" runat="server" ClientIDMode="Static" Value="10" />
            <asp:HiddenField ID="TabAfiliadosColumnaOrdenar_RP" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosDireccionOrdenar_RP" runat="server" ClientIDMode="Static" Value="A" />

            <%--Control de fecha: <asp:TextBox ID="EjemploFecha" runat="server" CssClass="formCalendar" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>--%>
        </div>
        <%--Fin Modal Búsqueda de Afiliados--%>
        <%--Fin Ya esta en controles--%>


        <%--Inicio Modal Envío de Correo--%>
        <div id="ModalEnvioCorreo" title="Enviar Correo Electrónico">
            <div id="ModEnvCorCargando" class="modalCargandoContenido" style="width:526px;height:350px"></div>
            <div id="ModEnvCorContenido" class="modalContenido">
                <asp:HiddenField ID="ModEnvCorModo" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModEnvCorDe" class="formLabel formLabel2Izq">De:</label>
                    <asp:TextBox ID="ModEnvCorDe" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorPara" class="formLabel formLabel2Izq">Para:</label>
                    <asp:TextBox ID="ModEnvCorPara" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAsunto" class="formLabel formLabel2Izq">Asunto:</label>
                    <asp:TextBox ID="ModEnvCorAsunto" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAdjunto" class="formLabel formLabel2Izq">Adjunto:</label>
                    <%--<asp:Image ID="ModEnvCorIconoPDF" ImageUrl="~/Imagenes/iconoPDF.png" style="margin-left:-188px" runat="server" />--%>
                    <label id="ModEnvCorAdjunto" class="formLabelAdjunto">Cotizacion.pdf (189kb)</label>
                </div>

                <div class="formLinea" style="height:210px">
                    <%--<label id="LabModCorMensaje" class="formLabel formLabel2Izq">Mensaje:</label>--%>
                    <asp:TextBox ID="ModEnvCorMensaje" runat="server" CssClass="formTextbox" style="margin-left:0px;height:200px" Width="560" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModEnvCorEnviar" style="width:80px;height:22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Envío de Correo--%>

        <%--Inicio Modal Ver Actividad--%>
        <div id="ModalActividad" title="Ver Actividad">
            <div id="ModActCargando" class="modalCargandoContenido" style="width:526px;height:250px"></div>
            <div id="ModActContenido" class="modalContenido">

                <div class="formLinea">
                    <label id="LabModActEvento" class="formLabel formLabel2Izq">Evento:</label>
                    <asp:TextBox ID="ModActEvento" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-168px" Width="470" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea" style="height:210px">
                    <label id="LabModActComentario" class="formLabel formLabel2Izq">Comentario:</label>
                    <asp:TextBox ID="ModActComentario" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-168px;height:200px" Width="470" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModActCerrar" style="width:80px;height:22px" class="boton darkblue sharp">Cerrar</a>
                </div>

            </div>
        </div>
        <%--Fin Modal ver Actividad--%>

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

        <%--Inicio Modal Cuadro de advertencia--%>
        <div id="ModalCuadroAdvertencia">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIcono" style="width:40px;height:40px"></td>
                        <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <a id="MCAAceptar_RP" class="boton darkblue sharp" style="width:80px">Aceptar</a>
                <a id="MCACancelar_RP" class="boton darkblue sharp" style="width:80px">Cancelar</a>
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

        <%--Inicio Modal Generando Reporte--%>
        <div id="ModalGenerandoReporte">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MGRIcono" style="width:40px;height:40px"></td>
                        <td id="MGRContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MGRContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <div id="TablaSeleccionAcomCargando" align="center" style="display:none">
                    <asp:Image ID="Image2" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                    <span class="texto">Cargando registros de ACOM, espere por favor...</span>
                </div>

                <div id="DivErrorAcom" class="grilla_error" style="display:none">
                    No se ha podido cargar la tabla de ACOM. <a id="A1">Intentar de nuevo</a>.
                </div>

                <div id="TablaSeleccionAcomContenedor" style="display:none;"></div>
            </div>
            <br />
            <div id="DivBotoneraAcom" align="center">
                <a id="MRVGuardarSeleccionAcom" class="boton darkblue sharp" style="width:80px">Aceptar</a>
                <a id="MRVCancelarSeleccionAcom" class="boton darkblue sharp" style="width:80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Seleccion de ACOM--%>
        
    </div>
</asp:Content>
