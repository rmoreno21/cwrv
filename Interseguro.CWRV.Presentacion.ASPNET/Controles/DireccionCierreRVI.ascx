<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DireccionCierreRVI.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.DireccionCierreRVI" %>
<form id="form1" runat="server" enableviewstate="False">
    <li class="step li-direccion" id="DireccionAfiliado">
        <div class="step-title waves-effect">Dirección</div>
        <div class="step-content">
            <div class="row">
                <div class="col s12">
                    <asp:HiddenField ID="ModIdDireccion" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="ModEntidad" runat="server" ClientIDMode="Static" Value="D" />

                    <span class="card-title blue-text text-darken-3">Dirección del afiliado</span>

                    <div class="row">
                        <div class="input-field col s12 m9">
                            <asp:TextBox ID="Direccion" runat="server" ClientIDMode="Static" MaxLength="250" disabled="disabled"></asp:TextBox>
                            <label for="Direccion">Dirección Informada en Cotizador Web de Rentas</label>
                            <span id="DireccionHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="EspacioUrbano" runat="server" ClientIDMode="Static" MaxLength="80" disabled="disabled"></asp:TextBox>
                            <label for="EspacioUrbano">N°/Mz/Lt/Dpto</label>
                            <span id="EspacioUrbanoHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="ModDirPrincipal" runat="server" CssClass="formCombobox" Width="110" ClientIDMode="Static" ReadOnly="True"></asp:DropDownList>
                            <label for="ModDirPrincipal">Principal</label>
                            <span id="ModDirPrincipalHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="TipoVia" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="TipoVia">Tipo de Vía</label>
                            <span id="TipoViaHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="NombreVia" runat="server" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
                            <label for="NombreVia">Nombre de Vía</label>
                            <span id="NombreViaHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="NumeroVia" runat="server" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                            <label for="NumeroVia">Número de Vía</label>
                            <span id="NumeroViaHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="NumeroInterior" runat="server" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                            <label for="NumeroInterior">Número Interior</label>
                            <span id="NumeroInteriorHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="TipoZona" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="TipoZona">Tipo de Zona</label>
                            <span id="TipoZonaHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="NombreZona" runat="server" ClientIDMode="Static" MaxLength="50"></asp:TextBox>
                            <label for="NombreZona">Nombre de Zona</label>
                            <span id="NombreZonaHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="NumeroDepartamento" runat="server" ClientIDMode="Static" MaxLength="4"></asp:TextBox>
                            <label for="NumeroDepartamento">Número departamento</label>
                            <span id="NumeroDepartamentoHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="Manzana" runat="server" ClientIDMode="Static" MaxLength="4"></asp:TextBox>
                            <label for="Manzana">Manzana</label>
                            <span id="ManzanaHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="NumeroLote" runat="server" ClientIDMode="Static" MaxLength="4"></asp:TextBox>
                            <label for="NumeroLote">Lote</label>
                            <span id="NumeroLoteHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="Kilometro" runat="server" ClientIDMode="Static" MaxLength="4"></asp:TextBox>
                            <label for="Kilometro">Kilómetro</label>
                            <span id="KilometroHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="Block" runat="server" ClientIDMode="Static" MaxLength="4"></asp:TextBox>
                            <label for="Block">Block</label>
                            <span id="BlockHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="Etapa" runat="server" ClientIDMode="Static" MaxLength="4"></asp:TextBox>
                            <label for="Etapa">Etapa</label>
                            <span id="EtapaHelper" class="helper-text"></span>
                        </div>

                        <div class="input-field col s12 m9">
                            <asp:TextBox ID="Referencia" runat="server" ClientIDMode="Static" MaxLength="70"></asp:TextBox>
                            <label for="Referencia">Referencia</label>
                            <span id="ReferenciaHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="Departamento" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="Departamento">Departamento <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="DepartamentoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="Provincia" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="Provincia">Provincia <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="ProvinciaHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="Distrito" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="Distrito">Distrito <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="DistritoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="LargaDistancia" runat="server" ClientIDMode="Static"></asp:DropDownList>
                            <label for="LargaDistancia">Larga Distancia Nacional <span style="font-weight: bold; color: red; font-size: 14px">*</span></label>
                            <span id="LargaDistanciaHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="step-actions">
                <button class="waves-effect waves-dark btn next-step" data-feedback="siguientePaso">CONTINUAR</button>
                <button class="waves-effect waves-dark btn-flat previous-step" id="btnRegresar">REGRESAR</button>
            </div>
        </div>
    </li>
</form>
