<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BeneficiarioCierreRVI.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.BeneficiarioCierreRVI" %>
<form id="form1" runat="server" enableviewstate="False">
    <li class="step li-beneficiario" id="<%=Beneficiario.numCorrelativo%>">
        <div class="step-title waves-effect" Style="text-transform: uppercase"><%=Beneficiario.Parentesco.Nombre + " - " + Beneficiario.Nombre + " " + Beneficiario.ApellidoPaterno + " " + Beneficiario.ApellidoMaterno%></div>
        <div class="step-content">
            <div class="row">
                <div class="col s12">
                    <asp:HiddenField ID="ModEntidad" runat="server" ClientIDMode="Static" Value="B" />
                    <asp:TextBox ID="ModNumCorrelativo" runat="server" CssClass="ModNumCorrelativo" ClientIDMode="Static" Style="display: none" ></asp:TextBox>

                    <span class="card-title" style="font-size: 18px;"><%=Beneficiario.Parentesco.Id == "80" ? "Tipo de Pensión: " + GlosaTipoPension : "" %></span>
                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="ApellidoPaterno" name="ApellidoPaterno" runat="server" CssClass="formTextbox ApellidoPaterno" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
                            <label for="ApellidoPaterno<%=Beneficiario.numCorrelativo%>">Apellido Paterno <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="ApellidoPaterno<%=Beneficiario.numCorrelativo%>Helper" class="helper-text red-text ApellidoPaternoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="ApellidoMaterno" name="ApellidoMaterno" runat="server" CssClass="formTextbox ApellidoMaterno" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
                            <label for="ApellidoMaterno<%=Beneficiario.numCorrelativo%>">Apellido Materno <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="ApellidoMaterno<%=Beneficiario.numCorrelativo%>Helper" class="helper-text ApellidoMaternoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="Nombres" name="Nombres" runat="server" CssClass="formTextbox Nombres" Width="180" ClientIDMode="Static" MaxLength="80" Style="text-transform: uppercase"></asp:TextBox>
                            <label for="Nombres<%=Beneficiario.numCorrelativo%>">Nombres <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="Nombres<%=Beneficiario.numCorrelativo%>Helper" class="helper-text NombresHelper"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="TipoDocumento" name="TipoDocumento" runat="server" Width="260" ClientIDMode="Static" CssClass="TipoDocumento">
                            </asp:DropDownList>
                            <label for="TipoDocumento<%=Beneficiario.numCorrelativo%>">Tipo de Identificación <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="TipoDocumento<%=Beneficiario.numCorrelativo%>Helper" class="helper-text TipoDocumentoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="NumeroDocumento" name="NumeroDocumento" runat="server" CssClass="formTextbox enteroPositivo NumeroDocumento" Width="178" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
                            <label for="NumeroDocumento<%=Beneficiario.numCorrelativo%>">Nro. de Identificación <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <asp:Label ClientIDMode="Static" runat="server" ID="PersonaHelperRviadm" CssClass="helper-text blue-text text-darken-3 PersonaHelperRviadm">Persona existente en Sistema Rentas</asp:Label>
                            <span id="NumeroDocumento<%=Beneficiario.numCorrelativo%>Helper" class="helper-text NumeroDocumentoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="Parentesco" name="Parentesco" runat="server" CssClass="formCombobox Parentesco" Width="260" ClientIDMode="Static" disabled="disabled">
                            </asp:DropDownList>
                            <label for="Parentesco<%=Beneficiario.numCorrelativo%>">Parentesco</label>
                            <span id="Parentesco<%=Beneficiario.numCorrelativo%>Helper" class="helper-text ParentescoHelper"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="IndInvalidez" name="IndInvalidez" runat="server" CssClass="formCombobox IndInvalidez" Width="260" ClientIDMode="Static" disabled="disabled">
                            </asp:DropDownList>
                            <label for="IndInvalidez<%=Beneficiario.numCorrelativo%>">Invalidez</label>
                            <span id="IndInvalidez<%=Beneficiario.numCorrelativo%>Helper" class="helper-text IndInvalidezHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="TipoInvalidez" name="TipoInvalidez" runat="server" CssClass="formCombobox TipoInvalidez" Width="260" ClientIDMode="Static" disabled="disabled">
                            </asp:DropDownList>
                            <label for="TipoInvalidez<%=Beneficiario.numCorrelativo%>">Tipo Invalidez</label>
                            <span id="TipoInvalidez<%=Beneficiario.numCorrelativo%>Helper" class="helper-text TipoInvalidezHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="FechaInvalidez" name="FechaInvalidez" runat="server" CssClass="fecha formTextbox formCalendar FechaInvalidez" Width="178" MaxLength="10" ClientIDMode="Static" disabled="disabled"></asp:TextBox>
                            <label for="FechaInvalidez<%=Beneficiario.numCorrelativo%>">Fecha Invalidez</label>
                            <span id="FechaInvalidez<%=Beneficiario.numCorrelativo%>Helper" class="helper-text FechaInvalidezHelper"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="Sexo" name="Sexo" runat="server" CssClass="formCombobox Sexo" Width="260" ClientIDMode="Static" disabled="disabled">
                            </asp:DropDownList>
                            <label for="Sexo<%=Beneficiario.numCorrelativo%>">Sexo</label>
                            <span id="Sexo<%=Beneficiario.numCorrelativo%>Helper" class="helper-text SexoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="FechaNacimiento" name="FechaNacimiento" runat="server" CssClass="fecha formTextbox formCalendar FechaNacimiento" Width="178" MaxLength="10" ClientIDMode="Static" disabled="disabled"></asp:TextBox>
                            <label for="FechaNacimiento<%=Beneficiario.numCorrelativo%>">Fecha Nacimiento</label>
                            <span id="FechaNacimiento<%=Beneficiario.numCorrelativo%>Helper" class="helper-text FechaNacimientoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4" style="display: none" id="divFechaFallecimiento">
                            <asp:TextBox ID="FechaFallecimiento" name="FechaFallecimiento" runat="server" CssClass="fecha formTextbox formCalendar FechaFallecimiento" Width="178" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                            <label for="FechaFallecimiento<%=Beneficiario.numCorrelativo%>">Fecha Fallecimiento <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="FechaFallecimiento<%=Beneficiario.numCorrelativo%>Helper" class="helper-text FechaFallecimientoHelper"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="Nacionalidad" name="Nacionalidad" runat="server" CssClass="formCombobox Nacionalidad" Width="260" ClientIDMode="Static">
                            </asp:DropDownList>
                            <label for="Nacionalidad<%=Beneficiario.numCorrelativo%>">Nacionalidad <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="Nacionalidad<%=Beneficiario.numCorrelativo%>Helper" class="helper-text NacionalidadHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="PaisOrigen" name="PaisOrigen" runat="server" CssClass="formCombobox PaisOrigen" Width="260" ClientIDMode="Static">
                            </asp:DropDownList>
                            <label for="PaisOrigen<%=Beneficiario.numCorrelativo%>">País de Origen <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="PaisOrigen<%=Beneficiario.numCorrelativo%>Helper" class="helper-text PaisOrigenHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="Celular" name="Celular" runat="server" ClientIDMode="Static" MaxLength="30" CssClass="formTextbox Celular"></asp:TextBox>
                            <label for="Celular<%=Beneficiario.numCorrelativo%>">Celular</label>
                            <span id="Celular<%=Beneficiario.numCorrelativo%>Helper" class="helper-text CelularHelper"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="CorreoElectronico" name="CorreoElectronico" runat="server" CssClass="formTextbox CorreoElectronico" Width="178" ClientIDMode="Static" MaxLength="200"></asp:TextBox>
                            <label for="CorreoElectronico<%=Beneficiario.numCorrelativo%>">Correo Electrónico</label>
                            <span id="CorreoElectronico<%=Beneficiario.numCorrelativo%>Helper" class="helper-text CorreoElectronicoHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="VinculoFamiliar" name="VinculoFamiliar" runat="server" CssClass="formCombobox VinculoFamiliar" Width="260" ClientIDMode="Static">
                            </asp:DropDownList>
                            <label for="VinculoFamiliar<%=Beneficiario.numCorrelativo%>">Vínculo Familiar</label>
                            <span id="VinculoFamiliar<%=Beneficiario.numCorrelativo%>Helper" class="helper-text VinculoFamiliarHelper"></span>
                        </div>

                        <div class="input-field col s12 m4">
                            <asp:TextBox ID="NumeroVinculoFamiliar" name="NumeroVinculoFamiliar" runat="server" CssClass="formTextbox NumeroVinculoFamiliar" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
                            <label for="NumeroVinculoFamiliar<%=Beneficiario.numCorrelativo%>">Nro. Vínculo Familiar</label>
                            <span id="NumeroVinculoFamiliar<%=Beneficiario.numCorrelativo%>Helper" class="helper-text NumeroVinculoFamiliarHelper"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="ESSALUD" name="ESSALUD" runat="server" CssClass="formCombobox ESSALUD" Width="260" ClientIDMode="Static" ToolTip="Indica si le aplica o no el descuento de ESSALUD, por defecto siempre se le descuenta el 4%.">
                            </asp:DropDownList>
                            <label for="ESSALUD<%=Beneficiario.numCorrelativo%>">Descuento ESSALUD <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="ESSALUD<%=Beneficiario.numCorrelativo%>Helper" class="helper-text ESSALUDHelper"></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="input-field col s12 m12 black-text">
                            <blockquote id="Mensaje<%=Beneficiario.numCorrelativo%>" class="MensajeBeneficiario" style="display: none">
                                <i id="MensajeIcono<%=Beneficiario.numCorrelativo%>" class="material-icons icono">done</i>
                                <span id="MensajeDetalle<%=Beneficiario.numCorrelativo%>"></span>
                            </blockquote>
                        </div>
                    </div>
                </div>
            </div>
            <div class="step-actions">
                <button class="waves-effect waves-dark btn next-step" data-feedback="siguientePaso">CONTINUAR</button>
                <button class="waves-effect waves-dark btn-flat previous-step" id="<%="btnRegresar" + Beneficiario.Parentesco.Id%>">REGRESAR</button>
            </div>
        </div>
    </li>
</form>
