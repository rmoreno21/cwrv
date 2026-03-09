<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaParametrosRentas.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaParametrosRentas" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <div id="ModoConsulta" class="row" style="display:none">
        <div class="col s12">
            <asp:GridView ID="TablaParametros" runat="server" ViewStateMode="Disabled" ClientIDMode="Static"
                AutoGenerateColumns="False" OnRowDataBound="TablaParametros_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="N°">
                        <ItemTemplate>
                            <span><%# Container.DataItemIndex + 1 %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="fec_ini_rango"
                           HeaderText="Inicio Vigencia" DataFormatString="{0:dd/MM/yyyy}" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="fec_fin_rango"
                           HeaderText="Fin Vigencia" DataFormatString="{0:dd/MM/yyyy}" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="cod_moneda"
                           HeaderText="Moneda" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="cod_tipo_temporalidad"
                           HeaderText="Temporalidad" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="num_tramo"
                           HeaderText="Tramo" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="val_parametro"
                           HeaderText="Valor" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="cod_tipo_pension"
                           HeaderText="Tipo Pensión" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="cod_departamento"
                           HeaderText="Departamento" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <asp:BoundField DataField="ind_origen"
                           HeaderText="Origen" >
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>

                    <%--<asp:TemplateField>
                        <ItemTemplate>
                            <%# DateTime.Now <= (DateTime)Eval("fec_fin_rango") ? "<a class=\"center modificar-parametro\" data-producto=\"" + Producto + "\" data-fechainicio=\"" +  Eval("fec_ini_rango") +"\" data-fechafin=\"" +  Eval("fec_fin_rango") +"\" data-fechainicio=\"" +  Eval("fec_ini_rango") +"\" data-parametro=\"" +  Eval("cod_parametro") +"\"><i class=\"material-icons pink-text darken-2-text\" title=\"Modificar Parámetro\">edit</i></a>": "" %>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
            
                </Columns>
                <EmptyDataTemplate>
                    <p style="font-size:1rem"><i class="material-icons blue-text text-darken-2" style="margin-right:10px">info</i> No se han encontrado parámetros.</p>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
    <div id="ModoEditar" style="display:none">
        <div class="row">
            <div class="col s12">
                <asp:GridView ID="TablaParametrosEditar" runat="server" ViewStateMode="Disabled" ClientIDMode="Static"
                    AutoGenerateColumns="False" OnRowDataBound="TablaParametrosEditar_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="N°">
                            <ItemTemplate>
                                <span><%# Container.DataItemIndex + 1 %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Inicio Vigencia">
                            <ItemTemplate>
                                <asp:TextBox ID="TabInicioVigencia" runat="server" ClientIDMode="Static" CssClass="fecha datepicker par-fechainicio"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Fin Vigencia">
                            <ItemTemplate>
                                <asp:TextBox ID="TabFinVigencia" runat="server" ClientIDMode="Static" CssClass="fecha datepicker par-fechafin"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Moneda">
                            <ItemTemplate>
                                <asp:DropDownList ID="TabMoneda" CssClass="par-moneda" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Temporalidad">
                            <ItemTemplate>
                                <asp:DropDownList ID="TabTemporalidad" CssClass="par-temporalidad" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Tramo">
                            <ItemTemplate>
                                <asp:TextBox ID="TabTramo" runat="server" ClientIDMode="Static" CssClass="entero inputgrilla par-tramo"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Valor">
                            <ItemTemplate>
                                <asp:TextBox ID="TabValor" runat="server" ClientIDMode="Static" CssClass="numerico inputgrilla par-valor" Text="0.00"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Tipo Pensión">
                            <ItemTemplate>
                                <asp:DropDownList ID="TabTipoPension" CssClass="par-tipo-pension" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Departamento">
                            <ItemTemplate>
                                <asp:DropDownList ID="TabDepartamento" CssClass="par-departamento" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Origen">
                            <ItemTemplate>
                                <asp:DropDownList ID="TabOrigen" CssClass="par-origen" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <p style="font-size:1rem"><i class="material-icons blue-text text-darken-2" style="margin-right:10px">info</i> No se han encontrado parámetros.</p>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
        <div class="row">
            <div class="col s12 right-align">
                <asp:HyperLink ID="AgregarParametro" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">add</i>Agregar Fila</asp:HyperLink>
            </div>
        </div>
        <div class="row">
            <div class="col s12 center-align">
                <asp:HyperLink ID="ModificarGuardar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">save</i>Guardar</asp:HyperLink>
                <asp:HyperLink ID="ModificarCancelar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">cancel</i>Cancelar</asp:HyperLink>
            </div>
        </div>
    </div>
</div>
</form>