<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaGestionVentas.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaGestionVentas" %>


<style type="text/css">
        
  </style>

<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">

        <%--   
    <div id="divScrooll" runat="server">

          

    </div>--%>


        <asp:GridView ID="TabGestionVentas" runat="server" CellPadding="1"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabGestionVentas_RowDataBound">


            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>

                <asp:TemplateField HeaderText="Nro. Meler">
                    <ItemTemplate>
                        <asp:Label Width="65px" ID="TabCotNumeroMeler" runat="server" Text='<%# Bind("NumeroMeler") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Fecha de Plazo AFP">
                    <ItemTemplate>
                        <asp:Label Width="65px" ID="TabCotFechaPlazoAFP" runat="server" Text='<%# Convert.ToDateTime(Eval("FechaPlazoAFP")).ToString("dd/MM/yyyy") %>'></asp:Label>
                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="AFP">
                    <ItemTemplate>
                        <asp:Label Width="68px" ID="TabCotNomAFP" runat="server" Text='<%# Bind("AFP.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CUSPP">
                    <ItemTemplate>
                        <asp:Label Width="90px" ID="TabCotCUSPP" runat="server" Text='<%# Bind("CUSPP") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>



                <asp:TemplateField HeaderText="Nombre Afiliado">
                    <ItemTemplate>
                        <asp:Label Width="150px" ID="TabCotNombreCliente" runat="server" Text='<%# Bind("NombreCliente") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Categoría">
                    <ItemTemplate>
                        <asp:Label Width="120px" ID="TabCotCategoria" runat="server" Text='<%# Bind("Categoria.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="CIC" HeaderText="Monto CIC" DataFormatString="{0:0,0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="Fecha de Cierre Comercial">
                    <ItemTemplate>
                        <asp:Label Width="65px" ID="TabCotFechaCierre" runat="server" Text='<%# (Eval("FechaCierre")!=null)?Convert.ToDateTime(Eval("FechaCierre")).ToString("dd/MM/yyyy"):string.Empty %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Re-cotización">
                    <ItemTemplate>
                        <asp:Label Width="65px" ID="TabCotRecotizacion" runat="server" Text='<%# Eval("Recotizacion") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Modalidad">
                    <ItemTemplate>
                        <asp:Label Width="65px" ID="TabCotModalidad" runat="server" Text='<%# Bind("Modalidad.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="PeriodoDiferido" HeaderText="P.Dif">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:BoundField DataField="PorcentajeRenta" HeaderText="Pje.Rent.">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:BoundField DataField="PeriodoGarantizado" HeaderText="P.Gar">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="CIA Ganadora">
                    <ItemTemplate>
                        <asp:Label Width="66px" ID="TabCotCompaniaGanadora" runat="server" Text='<%# Bind("CompaniaGanadora") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Nro. Cotización Ganadora">
                    <ItemTemplate>
                        <asp:Label Width="66px" ID="TabCotCotizacion" runat="server" Text='<%# Bind("NumeroCotizacion") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Moneda Ganadora">
                    <ItemTemplate>
                        <asp:Label Width="50px" ID="TabCotMoneda" runat="server" Text='<%# Bind("Moneda.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="ACOM" HeaderText="A" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:BoundField DataField="DCOM" HeaderText="D" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:BoundField DataField="DifTra" HeaderText="Solicitud Especial" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:BoundField DataField="TasaIS" HeaderText="Tasa IS Meler" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:BoundField DataField="TasaCiaGanadora" HeaderText="Tasa CIA Ganadora" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>



                <asp:TemplateField HeaderText="Fecha Cita">
                    <ItemTemplate>
                        <asp:Label Width="65px" ID="TabCotFechaCita" runat="server" Text='<%# Convert.ToDateTime(Eval("FechaCita")).ToString("dd/MM/yyyy") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Lugar de Cita">
                    <ItemTemplate>
                        <asp:Label Width="180px" ID="TabCotLugarCita" runat="server" Text='<%# Bind("LugarCita") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Nro. Agente">
                    <ItemTemplate>
                        <asp:Label Width="50px" ID="TabCotAgenteId" runat="server" Text='<%# Bind("Agente.Id") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Agente">
                    <ItemTemplate>
                        <asp:Label Width="100px" ID="TabCotAgenteNombre" runat="server" Text='<%# Bind("Agente.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Ubicación Agente">
                    <ItemTemplate>
                        <asp:Label Width="70px" ID="TabCotUbigeoAgente" runat="server" Text='<%# Bind("UbigeoAgente") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Agencia">
                    <ItemTemplate>
                        <asp:Label Width="100px" ID="TabCotNombreAgencia" runat="server" Text='<%# Bind("NombreAgencia") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Supervisor">
                    <ItemTemplate>
                        <asp:Label Width="100px" ID="TabCotSupervisor" runat="server" Text='<%# Bind("Supervisor") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Jefe">
                    <ItemTemplate>
                        <asp:Label Width="100px" ID="TabCotJefe" runat="server" Text='<%# Bind("Jefe") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>



            </Columns>
            <EmptyDataTemplate>
                <div class="grilla_info">
                    No se ha encontrado ningún registro de solicitud.
                </div>
            </EmptyDataTemplate>
            <HeaderStyle CssClass="grilla_cabecera" />
            <RowStyle CssClass="grilla_alt1" />
        </asp:GridView>

        <br />
        <asp:Panel ID="LabModSolLineaTotales" runat="server" CssClass="formLinea" ClientIDMode="Static">

            <label id="LabModSolTotalRegistro" for="ModSolTotalRegistro" class="formLabel formLabel2Izq">Cantidad Registros:</label>
            <asp:TextBox ID="ModSolTotalRegistro" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Style="text-align: right" Width="100" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>

            <label id="LabModSolTotalCIC" for="ModSolTotalCIC" class="formLabel formLabel2Der" style="margin-left: 182px">Total CIC:</label>
            <asp:TextBox ID="ModSolTotalCIC" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Style="text-align: right" Width="100" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>

        </asp:Panel>


    </div>
</form>
