<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="SolicitudesEvaluacion.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaParticular.SolicitudesEvaluacion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Solicitudes en Evaluación</span>
                    <div class="row">
                        <div class="col s12">
                            <asp:GridView
                                ID="TablaSolicitudesEvaluacionRP" runat="server"
                                ViewStateMode="Disabled" ClientIDMode="Static"
                                AutoGenerateColumns="False" CssClass="highlight">    
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Solicitud" />

                                    <asp:BoundField DataField="FechaSolicitud"
                                        HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="CUSPP">
                                        <ItemTemplate>
                                            <asp:Label ID="Cuspp" runat="server" Text='<%# Bind("Afiliado.CUSPP") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Asegurado">
                                        <ItemTemplate>
                                            <asp:Label ID="Asegurado" runat="server" Text='<%# Bind("Afiliado.Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Moneda">
                                        <ItemTemplate>
                                            <asp:Label ID="ItemMoneda" runat="server" Text='<%# Bind("MonedaPrimaUnica.Simbolo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="PrimaUnica"
                                        HeaderText="Prima Única" DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Estado Solicitud">
                                        <ItemTemplate>
                                            <asp:Label ID="ItemEstadoSolicitud" runat="server" Text='<%# Bind("EstadoSolicitud") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Estado PLAFT">
                                        <ItemTemplate>
                                            <asp:Label ID="ItemEstadoPLAFT" runat="server" Text='<%# Bind("EstadoSolicitudPlaft") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="LN">
                                        <ItemTemplate>
                                            <asp:Label ID="ItemListaNegra" runat="server" Text='<%# (Convert.ToInt32(Eval("ListaNegra"))==0)?"No":"<strong>Si</strong>" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Acción" ItemStyle-CssClass="center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# "<a class=\"center\" href=\"" + ResolveUrl("~/RentaParticular/SolicitudEvaluacion.aspx?s=") + Eval("Id") + "\"><i class=\"material-icons pink-text darken-2-text\" title=\"Visualizar Solicitud en Evaluación\">search</i></a>"%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    <p style="font-size:1rem"><i class="material-icons light-blue-text" style="margin-right:10px">info</i> No se han encontrado solicitudes en evaluación.</p>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
