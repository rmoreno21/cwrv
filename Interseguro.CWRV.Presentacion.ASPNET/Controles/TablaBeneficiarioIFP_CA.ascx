<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaBeneficiarioIFP_CA.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaBeneficiarioIFP_CA" %>

<form id="form1" runat="server" enableviewstate="False">

    <div id="ContenidoDinamico">

        <asp:GridView ID="TabBeneficiarios_IFP" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabBeneficiarios_IFP_RowDataBound">

            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>
                <asp:TemplateField HeaderText="N°">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                        <%--<input type="hidden" id="benId" name="benId" value="<%#  Eval("Id").ToString() %>" class="ModSolBenId">--%>
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
                        <asp:Label ID="ItemParentesco" runat="server"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Sexo">
                    <ItemTemplate>
                        <asp:Label ID="ItemSexo" runat="server"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Fecha Nac.">
                    <ItemTemplate>
                        <asp:Label ID="ItemNacimiento" runat="server"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <%--<asp:TemplateField HeaderText="Tipo de Identif.">
                    <ItemTemplate>
                        <asp:Label ID="ItemTipoIdentificacion" runat="server" Text='<%# Bind("Identificacion.IdTipo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="N° de Identif.">
                    <ItemTemplate>
                        <asp:Label ID="ItemNumeroIdentificacion" runat="server" Text='<%# Bind("Identificacion.Numero") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>--%>

                <asp:TemplateField HeaderText="% Renta">
                    <ItemTemplate>
                        <%--<asp:TextBox ID="ItemPorcentajeRenta" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid Porcentaje" Width="40" Style="text-align: right;"></asp:TextBox>--%>
                        <asp:Label ID="ItemPorcentajeRenta" runat="server" Text='<%# Bind("ValPjeRentaCA") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%--<asp:Button CssClass="grilla_boton" runat="server" PostBackUrl="~/RentaIFP/GrupoFamiliarAfiliado.aspx" />--%>
                        <a class="grilla_boton grilla_editar" data-grupofamiliar="<%# Container.DataItemIndex + 1 %>"></a>
                    </ItemTemplate>
                    <ItemStyle Width="30px" HorizontalAlign="Center" />
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
        <%--<asp:Panel ID="ModSumaBenefLinea" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruSumaBenef_IFP" for="ModGrusumaBenef_IFP" class="formLabel" style="margin-left: 650px; width: 100px;">Suma de %:</label>
            <asp:TextBox ID="ModGruSumaBenef_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Style="margin-left: 10px; text-align: right;" Width="40" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
        </asp:Panel>--%>
    </div>
</form>
