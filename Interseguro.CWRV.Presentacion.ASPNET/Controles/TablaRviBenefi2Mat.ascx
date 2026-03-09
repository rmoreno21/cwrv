<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaRviBenefi2Mat.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaRviBenefi2Mat" %>
<div id="ContenidoDinamico">
    <div style="overflow-x: auto; width: 100%; margin-bottom: 10px;">
        <asp:GridView ID="TabRviBenefi" runat="server" Width="100%"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" OnRowDataBound="TabRviBenefi_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" Visible="False" />
                <asp:TemplateField HeaderText="Parentesco">
                    <ItemTemplate>
                        <asp:Label ID="ItemParentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="84px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Apellidos y Nombres">
                    <ItemTemplate>
                        <div style="overflow:hidden;text-overflow:ellipsis;">
                            <asp:HyperLink ID="NombreBeneficiario" runat="server" NavigateUrl='<%# string.Format("~/Cotizador/BeneficiarioCierre.aspx?s={0}&fc={1}&c={2}", Eval("numSolicitud"), Request.QueryString["fc"], Eval("numCorrelativo")) %>'>
                                <asp:Label ID="ItemNomnbre" runat="server" Text='<%# Eval("ApellidoPaterno") + " " + Eval("ApellidoMaterno") + ", " + Eval("Nombre") %>'></asp:Label>
                            </asp:HyperLink>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Sexo"
                       HeaderText="Sexo">
                    <ItemStyle HorizontalAlign="Center" Width="39px" />
                </asp:BoundField>
                <asp:BoundField DataField="FechaNacimiento" 
                       HeaderText="Fecha de Nacimiento" DataFormatString="{0:dd/MM/yyyy}" >
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Invalidez" SortExpression="Active">
                    <ItemTemplate>
                        <%# (Boolean.Parse(Eval("Invalido").ToString())) ? "Sí" : "No"%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Tipo de Invalidez">
                    <ItemTemplate>
                        <asp:Label ID="ItemTipoInvalidez" runat="server" Text='<%# Bind("TipoInvalidez.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaInvalidez" 
                       HeaderText="Fecha de Invalidez" DataFormatString="{0:dd/MM/yyyy}" >
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
            </Columns>
            <EmptyDataTemplate>
                <bloquote>
                    <i class="material-icons">info</i> No se ha encontrado ningún registro de beneficiarios.
                </bloquote>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    
    <asp:Panel id="AdvertenciaBeneficiarios" runat="server" visible="false" style="margin-top: 20px;">
        <blockquote class="warning" style="background-color: #fff3e0; border-left: 4px solid #f57c00; padding: 10px; margin: 10px 0;">
            <i class="material-icons" style="color: #f57c00; vertical-align: middle;">warning</i>
            <span style="color: #f57c00; font-weight: 500; margin-left: 5px;">
                Las filas en rojo comprenden Beneficiarios no encontrados en la sección <b>"Grupo Familiar"</b> o con información incorrecta o incompleta. Por favor verifique, corrija o complete la información y luego recargue esta página.
            </span>
        </blockquote>
    </asp:Panel>
</div>