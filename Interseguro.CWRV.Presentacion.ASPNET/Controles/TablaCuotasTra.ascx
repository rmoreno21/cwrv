<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCuotasTra.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCuotasTra" %>


<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:HiddenField ID="HCuotasTra" runat="server" ClientIDMode="Static" />

    <asp:GridView ID="TabCuotasTra" runat="server" Width="100%" CellPadding="1" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" >
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField HeaderText="Id.">
                <ItemTemplate>
                    <asp:Label ID="TabCotIdAgente" ClientIDMode="Static" runat="server" Text='<%# Bind("Agente.Id") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="40px" />
            </asp:TemplateField>


            <asp:TemplateField HeaderText="Agente">
                <ItemTemplate>
                    <asp:Label ID="TabCotAgente" ClientIDMode="Static" runat="server" Text='<%# Bind("Agente.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="290px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Fecha de Inicio">
                <ItemTemplate>
                    <asp:TextBox ID="CotFecIniRangoText" runat="server" CssClass="fecha formTextboxGrid formCalendarReadOnly" Width="90" ClientIDMode="Static" MaxLength="10" Text='<%# Eval("FecInicioVigenciaStr") %>' ReadOnly="true" ></asp:TextBox>
                </ItemTemplate>
                <ItemStyle Width="110px" HorizontalAlign="Center" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Fecha de Término">
                <ItemTemplate>
                    <asp:TextBox  runat="server" CssClass="fecha formTextboxGrid formCalendar" Width="90" ClientIDMode="Static" MaxLength="10" Text='<%# Eval("FecFinVigenciaStr") %>'></asp:TextBox>
                </ItemTemplate>
                <ItemStyle Width="110px"  HorizontalAlign="Center"/>
            </asp:TemplateField>

           
            <asp:TemplateField HeaderText="Nro. Casos Total">
                <ItemTemplate>
                    <asp:TextBox ID="TabNroCasosTotal" runat="server"  ClientIDMode="Static" CssClass="formTextboxGrid numerico_int TabNroCasosTotal" style="text-align:right;" Width="95%" data-v-min="0" Text='<%# Eval("NroCasosTotal") %>'></asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="90px" />
            </asp:TemplateField>

            <asp:BoundField DataField="NroCasosSolicitados" HeaderText="Nro. Casos Solicitados">
                <ItemStyle HorizontalAlign="Right"  Width="90px"/>
            </asp:BoundField>

            <asp:BoundField DataField="NroCasosEfectivos" HeaderText="Nro. Casos Efectivos" >
                <ItemStyle HorizontalAlign="Right"  Width="90px" />
                
            </asp:BoundField>

        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de cuotas de TRA.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
</div>
</form>
