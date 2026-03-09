<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="GrupoFamiliarCierre.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.IFP.GrupoFamiliarCierre" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <div>
                        <span class="card-title blue-text text-darken-3">Datos generales</span>
                        <div class="row">
                            <div id="cotizador" class="col s12">
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamApellidoPaterno_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamApellidoPaterno_RP" for="ModGruFamApellidoPaterno_RP">Apellido Paterno:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <i class="material-icons prefix">schedule</i>
                                        <asp:TextBox ID="ModGruFamApellidoMaterno_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamApellidoMaterno_RP" for="ModGruFamApellidoMaterno_RP">Apellido Materno:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamNombres_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamNombres_RP" for="ModGruFamNombres_RP">Nombres:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamTipoIdentificacion_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamTipoIdentificacion_RP" for="ModGruFamTipoIdentificacion_RP">Tipo de Identificación:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamNumeroIdentificacion_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamNumeroIdentificacion_RP" for="ModGruFamNumeroIdentificacion_RP">Nro. de Identificación:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamParentesco_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamParentesco_RP" for="ModGruFamParentesco_RP">Parentesco*:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamEstadoCivil_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamEstadoCivil_RP" for="ModGruFamEstadoCivil_RP">Nro. de Identificación:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamSexo_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamSexo_RP" for="ModGruFamSexo_RP">Sexo*:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamFechaNacimiento_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamFechaNacimiento_RP" for="ModGruFamFechaNacimiento_RP">Fecha de Nacimiento*:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamIndInvalidez_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamIndInvalidez_RP" for="ModGruFamIndInvalidez_RP">Indicador de Invalidez*:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamTipoInvalidez_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamTipoInvalidez_RP" for="ModGruFamTipoInvalidez_RP">Tipo de Invalidez*:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamFechaInvalidez_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamFechaInvalidez_RP" for="ModGruFamFechaInvalidez_RP">Fecha de Invalidez<span id="AstModGruFamFechaInvalidez_RP"></span>:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamNacional_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamNacional_RP" for="ModGruFamNacional_RP">Nacionalidad:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamProfesion_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamProfesion_RP" for="ModGruFamProfesion_RP">Profesión:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamResidencia_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamResidencia_RP" for="ModGruFamResidencia_RP">Residencia:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamPEP_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamPEP_RP" for="ModGruFamPEP_RP">¿Es Pers. Exp. Polít.?</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamSO_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamSO_RP" for="ModGruFamSO_RP">¿Es sujeto obligado?</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamBanco_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamBanco_RP" for="ModGruFamBanco_RP">Banco:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamTipoCtaBanco_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="labModGruFamTipoCtaBanco_RP" for="ModGruFamTipoCtaBanco_RP">Tipo de Cuenta:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamNumeroBanco_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="labModGruFamNumeroBanco_RP" for="ModGruFamNumeroBanco_RP">Nro. de cuenta:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamComunicacion_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamLineaComunicacion_RP" for="ModGruFamComunicacion_RP">Mecan. de comunic.:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamConfidencialidadDatos_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabGruFamConfidencialidadDatos_RP" for="ModGruFamConfidencialidadDatos_RP">Comerc. de Datos Pers.:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamMail_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamMail_RP" for="ModGruFamMail_RP">Correo Electrónico:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamTelefono_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamTelefono_RP" for="ModGruFamTelefono_RP">Teléfono:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamCelular_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamCelular_RP" for="ModGruFamCelular_RP">Celular:</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div>
                        <span class="card-title blue-text text-darken-3">Datos para orígenes de fondo</span>
                        <div class="row">
                            <div id="cotizador" class="col s12">
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamCentrolaboral_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamCentrolaboral_RP" for="ModGruFamCentrolaboral_RP">Centro Laboral:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <i class="material-icons prefix">schedule</i>
                                        <asp:TextBox ID="ModGruFamCargo_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamCargo_RP" for="ModGruFamCargo_RP">Cargo:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamActividadeconomica_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamActividadeconomica_RP" for="ModGruFamActividadeconomica_RP">Actividad Económica:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:DropDownList ID="ModGruFamMonedaingreso_RP" runat="server" ClientIDMode="Static">
                                        </asp:DropDownList>
                                        <label ID="LabModGruFamMonedaingreso_RP" for="ModGruFamMonedaingreso_RP">Moneda de Ingreso:</label>
                                    </div>
                                    <div class="input-field col s12 m6">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <asp:TextBox ID="ModGruFamIngreso_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                        <label ID="LabModGruFamIngreso_RP" for="ModGruFamIngreso_RP">Ingreso Neto:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix">numbers</i>
                                        <label ID="LabModGruFamDestinoFondos_RP" for="ModGruFamDestinoFondos_RP">Descripción jurada de origen y/o destino de fondos:</label>
                                    </div>
                                    <div class="input-field col s12 m12">
                                        <a name="ancla-solicitud"></a>
                                        <i class="material-icons prefix"></i>
                                        <asp:TextBox ID="ModGruFamDestinoFondos_RP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col s12 center-align">
                                        <asp:HyperLink ID="Cotizar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">calculate</i>Cotizar</asp:HyperLink>
                                        <asp:HyperLink ID="DecargarDetalleSolicitud" Target="_blank" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2" style="display:none"><i class="material-icons left">picture_as_pdf</i>Descargar</asp:HyperLink>
                                        <asp:HyperLink ID="Regresar" NavigateUrl="~/RentaPrivadaPlus/Cotizador.aspx#datos_solicitud" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">arrow_back</i>Regresar</asp:HyperLink>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
