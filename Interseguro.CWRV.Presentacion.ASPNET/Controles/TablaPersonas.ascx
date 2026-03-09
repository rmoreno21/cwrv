<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaPersonas.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaPersonas" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabPersonas" runat="server" ViewStateMode="Disabled" CssClass="highlight"
        ClientIDMode="Static" AutoGenerateColumns="False" UseAccessibleHeader="True">
        <Columns>
             <asp:TemplateField HeaderText="CUSPP">
                <ItemTemplate>
                    <%# string.Format("<a href=\"{0}\">{1}</a>", ResolveUrl(string.Format("~/{0}/Principal.aspx?c={1}", Origen, Eval("CUSPP"))), Eval("CUSPP")) %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="130" />
            </asp:TemplateField>
            <asp:BoundField DataField="CUSPP" 
                   HeaderText="CUSPP" />
            <asp:BoundField DataField="ApellidoPaterno" 
                   HeaderText="Apellido Paterno" />
            <asp:BoundField DataField="ApellidoMaterno"
                   HeaderText="Apellido Materno" />
            <asp:BoundField DataField="Nombre" 
                   HeaderText="Nombre" />
        </Columns>
        <EmptyDataTemplate>
            <p id="TabPersonasVacia" class="center-align" style="font-size:1rem"><i class="material-icons blue-text text-darken-2" style="margin-right:10px">info</i> No se han encontrado personas.</p>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
</form>