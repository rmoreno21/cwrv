<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaBeneficiarioIFP_VIT.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaBeneficiarioIFP_VIT" %>

<form id="form1" runat="server" enableviewstate="False">

    <div id="ContenidoDinamico">

        <asp:GridView ID="TabBeneficiarios_VIT_Cierre" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabBeneficiarios_VIT_Cierre_RowDataBound">

            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>

                <asp:TemplateField HeaderText="N°">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                        <input type="hidden" id="benId_VIT" name="benId_VIT" value="<%#  Container.DataItemIndex + 1 %>" class="ModSolBenId_VIT">
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Apellidos y Nombres">
                    <ItemTemplate>
                        <div style="width: 400px; overflow: hidden; text-overflow: ellipsis;">
                            <asp:Label ID="ItemNombre_VIT" runat="server" Text='<%# Eval("ApellidoPaterno").ToString().ToUpper() + " " + Eval("ApellidoMaterno").ToString().ToUpper() + " " + Eval("Nombre").ToString().ToUpper() %>'></asp:Label>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Parentesco">
                    <ItemTemplate>
                        <asp:Label ID="ItemParentesco_VIT" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Sexo">
                    <ItemTemplate>
                        <asp:Label ID="ItemSexo_VIT" runat="server" Text='<%# Bind("Sexo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>

                <asp:BoundField DataField="FechaNacimiento"
                    HeaderText="Fec. Nacimiento" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" Width="104px" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="% Renta">
                    <ItemTemplate>
                        <asp:Label ID="ItemPorcentajeRenta_VIT" runat="server" Text='<%# Bind("Id") %>' CssClass="Porcentaje" Width="40"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <a class="grilla_boton grilla_editar" data-grupofamiliar="<%# Container.DataItemIndex + 1 %>" data-id="<%# Eval("Id") %>" data-tipoidentificacion="<%# Eval("Identificacion.IdTipo") %>" data-numeroidentificacion="<%# Eval("Identificacion.Numero") %>" data-pg="0"></a>
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

        <asp:Panel ID="ModSumaBenefLinea_VIT" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruSumaBenef_IFP_VIT" for="ModGrusumaBenef_IFP" class="formLabel" style="margin-left: 645px; width: 100px;">Suma de %:</label>
            <asp:TextBox ID="ModGruSumaBenef_IFP_VIT" runat="server" CssClass="formTextbox formTextboxReadOnly" Style="margin-left: -12px; text-align: center;" Width="40" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
        </asp:Panel>

    </div>

</form>