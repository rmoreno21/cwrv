<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaBeneficiarioIFP_PG.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaBeneficiarioIFP_PG" %>

<form id="form1" runat="server" enableviewstate="False">

    <div id="ContenidoDinamico">

        <asp:GridView ID="TabBeneficiarios_IFP" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabBeneficiarios_IFP_RowDataBound">

            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>

                <asp:TemplateField>
                    <%--<HeaderTemplate>
                        <%# "<input id=\"SeleccionarTodos\" type=\"checkbox\" name=\"SeleccionarTodos\" />" %>
                    </HeaderTemplate>--%>
                    <ItemTemplate>
                        <%-- "<input type=\"checkbox\" id=\"seleccion\" name=\"seleccion\" class=\"chklote\" value=\"" + Eval("Id") + "\" />" --%>
                        <asp:CheckBox ID="seleccion" runat="server" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="50" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="N°">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                        <input type="hidden" id="benId" name="benId" value="<%#  Container.DataItemIndex + 1 %>" class="ModSolBenId">
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Apellidos y Nombres">
                    <ItemTemplate>
                        <div style="width: 400px; overflow: hidden; text-overflow: ellipsis;">
                            <asp:Label ID="ItemNombre" runat="server" Text='<%# Eval("ApellidoPaterno").ToString().ToUpper() + " " + Eval("ApellidoMaterno").ToString().ToUpper() + " " + Eval("Nombre").ToString().ToUpper() %>'></asp:Label>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Parentesco">
                    <ItemTemplate>
                        <asp:Label ID="ItemParentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Sexo">
                    <ItemTemplate>
                        <asp:Label ID="ItemSexo" runat="server" Text='<%# Bind("Sexo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <%-- <asp:TemplateField HeaderText="Fecha Nac.">
                    <ItemTemplate>
                        <asp:Label ID="ItemNacimiento" runat="server"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>--%>

                <asp:BoundField DataField="FechaNacimiento"
                    HeaderText="Fec. Nacimiento" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" Width="104px" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="% Renta">
                    <ItemTemplate>
                        <asp:TextBox ID="ItemPorcentajeRenta" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid Porcentaje" Width="40" Style="text-align: right;"></asp:TextBox>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <a class="grilla_boton grilla_editar" data-grupofamiliar="<%# Container.DataItemIndex + 1 %>" data-id="<%# Eval("Id") %>" data-tipoidentificacion="<%# Eval("Identificacion.IdTipo") %>" data-numeroidentificacion="<%# Eval("Identificacion.Numero") %>"></a>
                    </ItemTemplate>
                    <ItemStyle Width="20px" HorizontalAlign="Center" />
                </asp:TemplateField>

            </Columns>

            <EmptyDataTemplate>
                <div class="grilla_info">
                    No se ha encontrado ningún registro de beneficiarios.
                </div>
            </EmptyDataTemplate>

            <HeaderStyle CssClass="grilla_cabecera" />

            <RowStyle CssClass="grilla_alt1" />

        </asp:GridView>

        <asp:Panel ID="ModSumaBenefLinea" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruSumaBenef_IFP" for="ModGrusumaBenef_IFP" class="formLabel" style="margin-left: 645px; width: 100px;">Suma de %:</label>
            <asp:TextBox ID="ModGruSumaBenef_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Style="margin-left: -12px; text-align: right;" Width="40" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
        </asp:Panel>

        <div class="formLinea" align="right">
            <asp:HyperLink ID="ModAgregarBeneficiarios" Style="width: 99px; height: 22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Agregar</asp:HyperLink>
        </div>

    </div>

</form>
