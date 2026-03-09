<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaPersonasVinculadasPEP.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaPersonasVinculadasPEP" %>

<form id="form2" runat="server" enableviewstate="False">

<div id="ContenidoDinamico">
    
    <asp:GridView ID="TabPEP" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabGrupoFamiliar_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />

        <Columns>

            <asp:BoundField DataField="Id"
                   HeaderText="ID" Visible="False" />
            
            <asp:TemplateField HeaderText="Ape.Paterno">
                <ItemTemplate>
                    <div style="width:135px;overflow:hidden;text-overflow:ellipsis;">
                        <asp:Label ID="ItemApellidoPaterno" runat="server" Text='<%# Bind("ApellidoPaterno") %>'></asp:Label>
                    </div>
                </ItemTemplate>
                <ItemStyle Width="150px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Ape.Materno">
                <ItemTemplate>
                    <div style="width:135px;overflow:hidden;text-overflow:ellipsis;">
                        <asp:Label ID="ItemApellidoMaterno" runat="server" Text='<%# Bind("ApellidoMaterno") %>'></asp:Label>
                    </div>
                </ItemTemplate>
                <ItemStyle Width="150px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Nombres">
                <ItemTemplate>
                    <div style="width:162px;overflow:hidden;text-overflow:ellipsis;">
                        <asp:Label ID="ItemNombre" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                    </div>
                </ItemTemplate>
                <ItemStyle Width="162px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Parentesco">
                <ItemTemplate>
                    <asp:Label ID="ItemParentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="84px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Tipo de documento">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoDocumento" runat="server" Text='<%# Bind("Identificacion.GlosaTipo") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="84px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Número de documento">
                <ItemTemplate>
                    <asp:Label ID="ItemNumeroDocumento" runat="server" Text='<%# Bind("Identificacion.Numero") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="84px" />
            </asp:TemplateField>

            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton grilla_editar\" data-idpersonavinculada=\"" + Eval("Id") + "\"></a>"%>
                    <%# "<a class=\"grilla_boton grilla_eliminar\" data-idpersonavinculada=\"" + Eval("Id") + "\" data-nombrepersonavinculada=\"" + Eval("Nombre") + " " + Eval("ApellidoPaterno") + " " + Eval("ApellidoMaterno") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="64px" HorizontalAlign="Center" />
            </asp:TemplateField>

        </Columns>
        
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de personas vinculadas al cliente PEP.
            </div>
        </EmptyDataTemplate>

        <HeaderStyle CssClass="grilla_cabecera" />
        
        <RowStyle CssClass="grilla_alt1" />

    </asp:GridView>

    <asp:Panel ID="Panel1" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>

</div>

</form>
