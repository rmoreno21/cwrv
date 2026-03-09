<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="Cotizador.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.IFP.Cotizador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            font-size: 12px;
        }

        table input {
            font-size: 12px !important;
        }

        td select {
            font-size: 12px !important;
        }

        .preloader-parametros {
            display: none;
            position: fixed;
            bottom: 5px;
            left: 5px;
            color: #0d47a1;
            border: 1px solid #0d47a1;
            padding: 5px;
            background: rgba(255, 255, 255, 0.5);
            -webkit-border-radius: 5px;
               -moz-border-radius: 5px;
                    border-radius: 5px;
        }

        .no-cotizable {
            background-color: #ffcdd2;
        }

        .eliminar-cotizacion {
            cursor: pointer;
        }


        .title-nav {
            position: absolute;
            color: #039be5;
            display: inline-block;
            font-size: 2.1rem;
            padding: 0;
        }

        .card-header {
          padding: 15px 10px 20px 20px;
          /*background: #8e75ac;*/
          /*color: white;*/
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSeleccionada" runat="server" ClientIDMode="Static" Value="N" />
        <asp:HiddenField ID="HDiasVigencia" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HCopia" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HBloqueo" runat="server" ClientIDMode="Static" Value="TRUE" />
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HFechaActual" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HAgenteId" runat="server" ClientIDMode="Static" Value="0" />

    <div class="row">

        <div class="card">

                <div class="card-header">
                    <span class="card-title title-nav">Cotizar Ingreso Flexible Plus</span>
                    <%--<i class="close material-icons">X</i>--%>
                </div>

                <div class="card-content">

                    <div class="row">
                        <div class="input-field col s12 m3 l3">
                            <i class="material-icons prefix">numbers</i>
                            <asp:TextBox ID="IFPSolicitud" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                            <label for="IFPSolicitud">Nro. Solicitud</label>
                        </div>                        
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3 l3">
                            <i class="material-icons prefix">assignment_ind</i>
                            <asp:TextBox ID="IFPDNI" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                            <label for="IFPDNI">DNI</label>
                            <span class="helper-text" data-error="wrong" data-success="right">DNI del afiliado</span>
                        </div>
                        <div class="input-field col s12 m4 l4">
                            <i class="material-icons prefix">person_pin</i>
                            <asp:TextBox ID="IFPNombres" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                            <label for="IFPNombres">Nombres</label>
                            <span class="helper-text" data-error="wrong" data-success="right">Nombres del afiliado</span>
                        </div>
                        <div class="input-field col s12 m4 l4">
                            <i class="material-icons prefix">person_pin</i>
                            <asp:TextBox ID="IFPApellidos" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                            <label for="IFPApellidos">Apellidos</label>
                            <span class="helper-text" data-error="wrong" data-success="right">Apellidos del afiliado</span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3 l3">
                            <i class="material-icons prefix">date_range</i>
                            <asp:TextBox ID="IFPFechaCotizacion" runat="server" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                            <label for="IFPFechaCotizacion">Fecha de Cotización*</label>
                        </div>
                        <div class="input-field col s12 m4 l4">
                            <i class="material-icons prefix">date_range</i>
                            <asp:TextBox ID="IFPFechaVigencia" runat="server" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>
                            <label for="IFPFechaVigencia">Vigente Hasta*</label>
                        </div>
                        <div class="input-field col s12 m4 l4">
                            <i class="material-icons prefix">date_range</i>
                            <asp:TextBox ID="IFPFechaDevengue" runat="server" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>
                            <label for="IFPFechaDevengue">Fecha de Devengue*</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3 l3">
                            <i class="material-icons prefix">monetization_on</i>
                            <asp:DropDownList ID="IFPMonedaPrimaUnica" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="IFPMonedaPrimaUnica">Moneda de Prima Única*</label>
                        </div>
                        <div class="input-field col s12 m4 l4">
                            <i class="material-icons prefix">payments</i>
                            <asp:TextBox ID="IFPPrimaUnica" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <label for="IFPPrimaUnica">Prima Única*</label>
                        </div>

                         <div class="input-field col s11 m4 l4">
                            <i class="material-icons prefix">assignment</i>
                            <asp:DropDownList ID="IFPPlan" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="IFPPlan">Plan</label>
                        </div>

                        <div class="input-field col s1 m1 l1">
                            <a id="IFPAgregarPlan" href="javascript:void(0);" class="btn-floating btn-small disabled waves-effect waves-light"><i class="material-icons">add</i></a>
                        </div>

                    </div>

                    

                    <div class="row">

                        <div id="TablaCotizacionesContenedor_IFP_Plan1" style="display: none" class="col s12 black-text center-align"></div>
                        <br />
                        <div id="TablaCotizacionesContenedor_IFP_Plan2" style="display: none" class="col s12 black-text center-align"></div>
                        
                        <div id="TablaCotizacionesCargando" class="col s12 valign-wrapper">
                            <div class="preloader-wrapper small active" style="width: 24px; height: 24px">
                                <div class="spinner-layer spinner-blue-only">
                                    <div class="circle-clipper left">
                                        <div class="circle"></div>
                                    </div>
                                    <div class="gap-patch">
                                        <div class="circle"></div>
                                    </div>
                                    <div class="circle-clipper right">
                                        <div class="circle"></div>
                                    </div>
                                </div>
                            </div>
                            <span style="margin-left: 10px">Cargando cotizaciones...
                            </span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col s12 center-align">
                            <a id="IFPCotizar" href="javascript:void(0);" class="waves-effect waves-light btn blue darken-2"><i class="material-icons left">calculate</i>Cotizar</a>
                            <a id="IFPRegresar" href="javascript:void(0);" class="waves-effect waves-light btn blue darken-2"><i class="material-icons left">arrow_back</i>Cotizaciones</a>
                            <a id="IFPImprimir" href="javascript:void(0);" class="waves-effect waves-light btn blue darken-2"><i class="material-icons left">print</i>Imprimir</a>
                        </div>
                    </div>




                    <%--<div id="ManSolLeyenda">
                        <fieldset>
                            <legend>Leyenda</legend>
                            <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" ClientIDMode="Static" Visible="true" Style="font-family: Calibri; font-size: 13px; color: #0060A9;">
                                <span class="grilla_error_tra" style="width: 25px !important; height: auto; display: inline-block">&nbsp;</span><span style="padding-left: 5px">Cotización no alcanza el mínimo requerido, por lo cual no se simula.</span>
                            </asp:Panel>
                            <div id="TabCotizacionesLeyenda_diferido_RP" style="font-family: Calibri; font-size: 13px; color: #0060A9;">
                                <span style="padding-left: 30px">Para las cotizaciones con moneda ajustada y diferimiento, mostrarán la renta ajustada al primer pago entre paréntesis "()".</span>
                            </div>
                        </fieldset>
                    </div>--%>



                </div>

        </div>
    </div>

    


    
<%--    <asp:Panel ID="NuevaCotizacionRP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 264px">Cotizaciones Ingreso Flexible Plus</h1>

        <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSeleccionada" runat="server" ClientIDMode="Static" Value="N" />
        <asp:HiddenField ID="HDiasVigencia" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HCopia" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HBloqueo" runat="server" ClientIDMode="Static" Value="TRUE" />
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HFechaActual" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HAgenteId" runat="server" ClientIDMode="Static" Value="0" />

        <div id="ModSolContenido" class="modalContenido">

            <div id="ManSolPestanhas">

                <ul>
                    <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                </ul>

                <asp:HiddenField ID="ManSolPestanhaActiva_RP" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManSolTipoSolicitud_RP" runat="server" ClientIDMode="Static" />

                <div id="ManSolPestanha1" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

                    <div class="formLinea">
                        <label id="LabModSolNroSolicitud_IFP" for="ModSolNroSolicitud_IFP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                        <asp:TextBox ID="ModSolNroSolicitud_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolDNI_IFP" for="ModSolDNI_IFP" class="formLabel formLabel2Der">DNI:</label>
                        <asp:TextBox ID="ModSolDNI_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="193" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolNombres_IFP" for="ModSolNombres_IFP" class="formLabel formLabel2Izq">Nombres:</label>
                        <asp:TextBox ID="ModSolNombres_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolApellidos_IFP" for="ModSolApellidos_IFP" class="formLabel formLabel2Der">Apellidos:</label>
                        <asp:TextBox ID="ModSolApellidos_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="193" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolMonedaPrimaUnica_IFP" for="IFPMonedaPrimaUnica" class="formLabel formLabel2Izq">Moneda de Prima Única*:</label>
                        <asp:DropDownList ID="IFPMonedaPrimaUnica" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static"></asp:DropDownList>

                        <label id="LabIFPPrimaUnica" for="IFPPrimaUnica" class="formLabel formLabel2Der">Prima Única*:</label>
                        <asp:TextBox ID="IFPPrimaUnica" runat="server" CssClass="formTextbox numerico" Width="193" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_IFP" for="ModSolFechaCotizacion_IFP" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                        <asp:TextBox ID="ModSolFechaCotizacion_IFP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                        <asp:Label ID="LabModSolFechaVigencia_IFP" AssociatedControlID="ModSolFechaVigencia_IFP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <asp:TextBox ID="ModSolFechaVigencia_IFP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="193" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>
                    </div>

                    <asp:Panel runat="server" CssClass="formLinea">
                        <label id="LabModSolFechaDevengue_IFP" for="ModSolFechaDevengue_IFP" class="formLabel formLabel2Izq">Fecha de Devengue*:</label>
                        <asp:TextBox ID="ModSolFechaDevengue_IFP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>

                        <label id="LabModSolPlan_IFP" for="IFPPlan" class="formLabel formLabel2Der">Agregar Plan:</label>
                        <asp:DropDownList ID="IFPPlan" runat="server" CssClass="formCombobox" Width="199" ClientIDMode="Static"></asp:DropDownList>

                        <a id="IFPAgregarPlan" href="javascript:void(0);" style="width: 20px; height: 22px; margin-left: 3px;" class="botonDeshabilitado gris gris_sharp">+</a>

                    </asp:Panel>

                    <div class="formLinea">
                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio_IFP" for="ModSolTipoCambio_IFP" class="formLabel formLabel2Izq">Tipo Cambio:</label>
                            <asp:TextBox ID="ModSolTipoCambio_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </asp:Panel>
                    </div>

                    <div class="formLinea" id="ModSolLineaNumPoliza_RP" runat="server">
                        <label id="LabModSolNumPoliza_RP" for="ModSolNumPoliza_RP" class="formLabel formLabel2Izq">Nro. Póliza:</label>
                        <asp:TextBox ID="ModSolNumPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolEstadoPoliza_RP" for="ModSolEstadoPoliza_RP" class="formLabel formLabel2Der">Estado Póliza:</label>
                        <asp:TextBox ID="ModSolEstadoPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>

                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false" Style="padding: 0px 0px 22px 30px">
                        <asp:Label ID="LabMensaje" CssClass="formLabel2SinFondo" runat="server" ClientIDMode="Static" />

                        <div id="LabModLineaCausal" runat="server">
                            <label id="LabModSolCausalPoliza_RP" for="ModSolCausalPoliza_RP" class="formLabel" style="margin-left: 152px;">Causal:</label>
                            <asp:TextBox ID="ModSolCausalPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>
                    </asp:Panel>

                    <div id="TablaCotizacionesCargando_RP" align="center" style="height: 50px; padding: 82px 0">
                        <asp:Image ID="icoTablaCotizacionesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando cotizaciones, espere por favor...</span>
                    </div>

                    <div id="TablaCotizacionesContenedor_IFP_Plan1" style="display: none"></div>
                    <br />
                    <div id="TablaCotizacionesContenedor_IFP_Plan2" style="display: none"></div>
                    <br />
                    <div id="TablaCotizacionesContenedor_IFP_Plan3" style="display: none"></div>

                    <div class="formLinea" align="center">
                        <a id="IFPCotizar" href="javascript:void(0);" style="width: 90px; height: 22px" class="boton darkblue sharp">Cotizar</a>
                        <a id="ModSolCancelar_RP" href="javascript:void(0);" style="width: 90px; height: 22px" class="boton darkblue sharp">Cotizaciones</a>
                        <a id="ModSolImprimir_RP" href="javascript:void(0);" style="width: 90px; height: 22px" class="boton darkblue sharp">Imprimir</a>

                    </div>

                    <div id="ManSolLeyenda">
                        <fieldset>
                            <legend>Leyenda</legend>
                            <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" ClientIDMode="Static" Visible="true" Style="font-family: Calibri; font-size: 13px; color: #0060A9;">
                                <span class="grilla_error_tra" style="width: 25px !important; height: auto; display: inline-block">&nbsp;</span><span style="padding-left: 5px">Cotización no alcanza el mínimo requerido, por lo cual no se simula.</span>
                            </asp:Panel>
                            <div id="TabCotizacionesLeyenda_diferido_RP" style="font-family: Calibri; font-size: 13px; color: #0060A9;">
                                <span style="padding-left: 30px">Para las cotizaciones con moneda ajustada y diferimiento, mostrarán la renta ajustada al primer pago entre paréntesis "()".</span>
                            </div>
                        </fieldset>
                    </div>

                </div>

                <div id="ManSolPestanha2" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <div id="TablaBeneficiariosCargando_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaBeneficiariosCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando beneficiarios, espere por favor...</span>
                    </div>
                    <div id="TablaBeneficiariosContenedor_RP" style="display: none"></div>
                    <div id="TablaBeneficiariosError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de beneficiarios. <a id="TablaBeneficiariosReintentar_RP">Intentar de nuevo</a>.</div>

                    <br />

                    <div id="BeneficiariosOriginales_RP" align="left" style="display: none">
                        Beneficiarios originales de la Solicitud:
                    </div>

                    <div id="TablaRviBenefiCargando_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaRviBenefiCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando beneficiarios registrados originalmente en la solicitud, espere por favor...</span>
                    </div>
                    <div id="TablaRviBenefiContenedor_RP" style="display: none"></div>
                    <div id="TablaRviBenefiError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de beneficiarios originales. <a id="TablaRviBenefiReintentar_RP">Intentar de nuevo</a>.</div>
                </div>

            </div>

        </div>

    </asp:Panel>--%>



   <div style="display: none">

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
      
    </div>

        <%-- 
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

        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />

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
        --%>




</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        var Solicitud = null;
        //var idCotizacion;
        //var parametrosCargados = false;
        //var tablaCotizacionesCargada = false;
        var Plan = [];

        $(document).ready(function () {
            
             //url: "Cotizador.aspx/ObtenerSolicitud",



            /* Solicitud 
            $('#ModalSolicitud').dialog({
                autoOpen: false,
                resizable: false,
                width: 915,
                show: "fade",
                hide: "fade",
                modal: true
            });*/

            //Eventos

            //if ($('#ModSolModo').val() == "N") {
                NuevaSolicitud();
            //};

            function NuevaSolicitud() {
                $('#ModSolModo').val('N');
                //$('#BeneficiariosOriginales_RP').hide();
                //$('#TablaRviBenefiContenedor_RP').hide();
                //$('#ManSolPestanhas li:eq(0)').trigger('click');

                $('#ModSolCargando').show();
                //$('#ModalSolicitud').dialog('open');

                LimpiarFormularioSolicitud();
                $('#ManSolTipoSolicitud_RP').val('EXTRAOFICIAL');

                //$('#TabCotizacionesLeyenda_RP').hide();

                var params = {
                    tokenUsuario: $("#TokenUsuario").val()
                }

                $('#IFPImprimir').attr('class', 'waves-effect waves-light btn disabled blue darken-2'); //botonDeshabilitado gris gris_sharp

                $.ajax({
                    type: 'POST',
                    url: 'Cotizador.aspx/CrearDatosSolicitud',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: JSON.stringify(params),
                    success: function (data) {
                        /* Solicitud Creada */
                        Solicitud = data.d;
                        Solicitud.CoberturasAdicionales = [];
                        //$("#ModSolTipoCambioPanel").hide();

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            /* Sesión caducada */
                            document.location.reload(true);
                        }
                        else {
                            //$('#ModalSolicitud').dialog('close');

                            //$('#MCMIcono').attr('class', 'error');
                            //$('#MCMContenedor').html('Ha ocurrido un error al crear los datos de la solicitud.');
                            //$('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            //$('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            };

            /*Cambiando la moneda*/
            //IFPPrimaUnica - IFPPrimaUnica
            //IFPMonedaPrimaUnica - IFPMonedaPrimaUnica
            //IFPPlan - IFPPlan
            $(document).on("change", "#IFPMonedaPrimaUnica", function () {
                var valIFPPrimaUnica = false
                var valIFPMonedaPrimaUnica = false
                var valIFPPlan = false

                if ($.trim($('#IFPPrimaUnica').val()).length > 0) {
                    if ($('#IFPPrimaUnica').val() != 0.00) {
                        valIFPPrimaUnica = true;
                    }
                }

                if ($('#IFPMonedaPrimaUnica').val() != "0") {
                    valIFPMonedaPrimaUnica = true;
                }

                if ($('#IFPPlan').val() != "0") {
                    valIFPPlan = true;
                }

                esCorrecto = valIFPPrimaUnica & valIFPMonedaPrimaUnica & valIFPPlan;

                /*VALIDACION DE TOPE DE COTIZACIONES*/
                //IFPCotizar - IFPCotizar
                var valCantidadSolicitudes = false;

                if (valIFPPrimaUnica & valIFPMonedaPrimaUnica) {
                    valCantidadSolicitudes = validarCantidadSolicitudes($('#IFPMonedaPrimaUnica').val(), $('#IFPPrimaUnica').val())
                    if (!valCantidadSolicitudes) {
                        //$('#MCMIcono').attr('class', 'error');
                        //$('#MCMContenedor').html('No es posible cotizar, ha alcanzado el límite de cotizaciones.');
                        //$('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        //$('#ModalCuadroMensaje').dialog('open');
                        //$('#ModalCotizando').dialog('close');

                        $('#IFPCotizar').attr('class', 'waves-effect waves-light btn disabled blue darken-2');
                    } else {
                        $('#IFPCotizar').attr('class', 'waves-effect waves-light btn blue darken-2');
                    }
                }

                //IFPAgregarPlan - 
                if (esCorrecto && valCantidadSolicitudes) {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');
                    
                    var valFlagPlan = false;

                    for (var itemPlan = 0; itemPlan < Plan.length; itemPlan++) {
                        if (Plan[itemPlan] == $('#IFPPlan').val()) {
                            valFlagPlan = true;
                        }
                    }

                    if (valFlagPlan) {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                    }
                    else {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small waves-effect waves-light');
                    }

                } else {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light'); //boton darkblue sharp
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light'); //botonDeshabilitado gris gris_sharp
                    $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                }

                var valPlan1 = false;
                var valPlan2 = false;
                var valPlan3 = false;

                for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    Solicitud.Cotizaciones[i].Moneda.Id = "000";
                    if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN1') {
                        valPlan1 = true;
                    }
                    if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN2') {
                        valPlan2 = true;
                    }
                    if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN3') {
                        valPlan3 = true;
                    }
                }

                if (valPlan1) {
                    CargarTablaCotizaciones_RP(Solicitud, $('#IFPMonedaPrimaUnica').val(), 'PLAN1');
                }
                if (valPlan2) {
                    CargarTablaCotizaciones_RP(Solicitud, $('#IFPMonedaPrimaUnica').val(), 'PLAN2');
                }
                if (valPlan3) {
                    CargarTablaCotizaciones_RP(Solicitud, $('#IFPMonedaPrimaUnica').val(), 'PLAN3');
                }

            });
            
            /*cambio de Plan*/
            $(document).on("change", "#IFPPlan", function () {

                var valIFPPrimaUnica = false
                var valIFPMonedaPrimaUnica = false
                var valIFPPlan = false

                if ($.trim($('#IFPPrimaUnica').val()).length > 0) {
                    if ($('#IFPPrimaUnica').val() != 0.00) {
                        valIFPPrimaUnica = true;
                    }
                }

                if ($('#IFPMonedaPrimaUnica').val() != "0") {
                    valIFPMonedaPrimaUnica = true;
                }

                if ($('#IFPPlan').val() != "0") {
                    valIFPPlan = true;
                }

                esCorrecto = valIFPPrimaUnica & valIFPMonedaPrimaUnica & valIFPPlan;
                
                if (esCorrecto) {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');

                    var valFlagPlan = false;
                    
                    for (var itemPlan = 0; itemPlan < Plan.length; itemPlan++) {
                        if (Plan[itemPlan] == $('#IFPPlan').val()) {
                            valFlagPlan = true;
                        }
                    }

                    if (valFlagPlan) {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                    }
                    else {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small waves-effect waves-light');
                    }


                } else {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');

                    $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                }

            });

            /*cambio de PU*/
            $(document).on("change", "#IFPPrimaUnica", function () {

                var valIFPPrimaUnica = false
                var valIFPMonedaPrimaUnica = false
                var valIFPPlan = false

                if ($.trim($('#IFPPrimaUnica').val()).length > 0) {
                    if ($('#IFPPrimaUnica').val() != 0.00) {
                        valIFPPrimaUnica = true;
                    }
                }

                if ($('#IFPMonedaPrimaUnica').val() != "0") {
                    valIFPMonedaPrimaUnica = true;
                }

                if ($('#IFPPlan').val() != "0") {
                    valIFPPlan = true;
                }

                esCorrecto = valIFPPrimaUnica & valIFPMonedaPrimaUnica & valIFPPlan;

                if (esCorrecto) {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');

                    var valFlagPlan = false;

                    for (var itemPlan = 0; itemPlan < Plan.length; itemPlan++) {
                        if (Plan[itemPlan] == $('#IFPPlan').val()) {
                            valFlagPlan = true;
                        }
                    }

                    if (valFlagPlan) {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                    }
                    else {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small waves-effect waves-light');
                    }

                } else {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');

                    $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                }

            });

            $(document).on("keyup", "#IFPPrimaUnica", function () {

                var valIFPPrimaUnica = false
                var valIFPMonedaPrimaUnica = false
                var valIFPPlan = false

                if ($.trim($('#IFPPrimaUnica').val()).length > 0) {
                    if ($('#IFPPrimaUnica').val() != 0.00) {
                        valIFPPrimaUnica = true;
                    }
                }

                if ($('#IFPMonedaPrimaUnica').val() != "0") {
                    valIFPMonedaPrimaUnica = true;
                }

                if ($('#IFPPlan').val() != "0") {
                    valIFPPlan = true;
                }

                esCorrecto = valIFPPrimaUnica & valIFPMonedaPrimaUnica & valIFPPlan;

                if (esCorrecto) {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');

                    var valFlagPlan = false;

                    for (var itemPlan = 0; itemPlan < Plan.length; itemPlan++) {
                        if (Plan[itemPlan] == $('#IFPPlan').val()) {
                            valFlagPlan = true;
                        }
                    }

                    if (valFlagPlan) {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                    }
                    else {
                        $('#IFPAgregarPlan').addClass('btn-floating btn-small waves-effect waves-light');
                    }

                } else {
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small waves-effect waves-light');
                    $('#IFPAgregarPlan').removeClass('btn-floating btn-small disabled waves-effect waves-light');

                    $('#IFPAgregarPlan').addClass('btn-floating btn-small disabled waves-effect waves-light');
                }

            });

            function validarCantidadSolicitudes(valMonedaPrimaUnica, valPrimaUnica) {
                var resultado = false;

                var params = {
                    monedaPrimaUnica: valMonedaPrimaUnica
                    ,primaUnica: valPrimaUnica
                }

                $.ajax({
                    type: "POST",
                    url: "Cotizador.aspx/validarCantidadSolicitudes",
                    contentType: "application/json; charset=iso-8859-1",
                    data: JSON.stringify(params),
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        if (data.d == "TOPE") {
                            resultado = false;
                        }
                        else if (data.d == "OK") {
                            resultado = true;
                        }
                        else if (data.d.Estado == "ERROR") {
                            //$("#MCMIcono").attr("class", "error");
                            //$("#MCMContenedor").html(data.d.Mensaje);
                            //$("#ModalCuadroMensaje").dialog({ title: "Error" });
                            //$("#ModalCuadroMensaje").dialog("open");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            //$("#MCMIcono").attr("class", "error");
                            //$("#MCMContenedor").html("Ha ocurrido un error al validar solicitud.");
                            //$("#ModalCuadroMensaje").dialog({ title: "Error" });
                            //$("#ModalCuadroMensaje").dialog("open");
                        }
                    }
                });

                return resultado;
            }

            function LimpiarFormularioSolicitud() {
                $('#IFPSolicitud').removeClass('formTextboxError');
                //$('#IFPTipoCambio').removeClass('formTextboxError');
                $('#IFPFechaDevengue').removeClass('formTextboxError formCalendarError');
                $('#IFPFechaCotizacion').removeClass('formTextboxError formCalendarError');
                $('#IFPPrimaUnica').removeClass('formTextboxError');
                $('#IFPSolicitud').val('');
                //$('#IFPTipoCambio').val('');
                botonModSolAceptarBloqueado = false;
                $('#IFPCotizar').attr('class', 'waves-effect waves-light btn disabled blue darken-2'); //botonDeshabilitado gris gris_sharp
                //$('#TablaCotizacionesCargando_RP').hide();
            }

        });
            


            //cargarTablaCotizaciones(false);
            
            //$("#Cotizar").click(function () {
            //url: "Cotizador.aspx/CotizarRPP",

                    //url: "Cotizador.aspx/AgregarCotizacionASolicitud",
                   
            //$(document).on("click", "#TabCotizacionesRPP .eliminar-cotizacion", function () {
               
       

        /*function limpiarValidaciones() {
            $(".helper-text").text("");
        }

        function mostrarPreloaderParametros() {
            $(".preloader-parametros").fadeIn(1000);
        }

        function ocultarPreloaderParametros() {
            $(".preloader-parametros").fadeOut(1000);
        }*/

                //url: "Cotizador.aspx/CargarParametrosRPP",
               
                //url: "Cotizador.aspx/CargarTablaCotizaciones",
               
                //url: "Cotizador.aspx/CargarTablaBeneficiarios",
               
       
    </script>
</asp:Content>
