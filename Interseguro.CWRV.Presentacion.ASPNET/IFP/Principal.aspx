<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.IFP.Principal" %>
<%@ Import Namespace="Interseguro.CWRV.Infraestructura.General" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <h1>Ingreso Flexible Plus</h1>
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3" style="line-height:20px;margin-bottom:25px"><i class="small material-icons">person_search</i> Búsqueda de Personas</span>
                    <div class="row">
                        <div class="col s12">
                            <ul class="tabs">
                                <li class="tab col s3"><a href="#por-cuspp">Por CUSPP</a></li>
                                <li class="tab col s3"><a href="#por-documento">Por Documento</a></li>
                            </ul>
                        </div>
                        <div id="por-cuspp" class="col s12">
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">numbers</i>
                                    <asp:TextBox ID="BusquedaSolicitud" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="BusquedaSolicitud">Nro. Solicitud</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">numbers</i>
                                    <asp:TextBox ID="BusquedaCUSPP" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="BusquedaCUSPP">CUSPP</label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col s12 center-align">
                                    <asp:HyperLink ID="BuscarPorCUSPP" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">search</i>Buscar</asp:HyperLink>
                                </div>
                            </div>

                            <div class="row" style="margin-bottom:0">
                                <div class="col s12 right-align">
                                    <asp:HyperLink ID="BusquedaAvanzada" NavigateUrl="~/Comun/BusquedaPersonas.aspx?o=IFP" runat="server">Búsqueda Avanzada</asp:HyperLink>
                                </div>
                            </div>

                        </div>
                        <div id="por-documento" class="col s12">
                            <div class="row">
                                <div class="col s12">
                                    
                                </div>
                            </div>

                            <div id="beneficiarios-solicitud" class="row" style="display:none">
                                <div class="col s12">
                                    <h6>Beneficiarios de la Solicitud</h6>
                                </div>
                                <div id="TablaBeneficiarios" class="col s12 black-text center-align">
                                </div>
                                <div id="TablaBeneficiariosCargando" class="col s12 valign-wrapper">
                                    <div class="preloader-wrapper small active" style="width:24px;height:24px">
                                        <div class="spinner-layer spinner-blue-only">
                                            <div class="circle-clipper left">
                                                <div class="circle"></div>
                                            </div><div class="gap-patch">
                                            <div class="circle"></div>
                                            </div><div class="circle-clipper right">
                                            <div class="circle"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <span style="margin-left:10px">
                                        Cargando tabla de beneficiarios...
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="DatosPersona" runat="server" CssClass="row" Visible="false">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3" style="line-height:20px;margin-bottom:25px">
                        <i class="small material-icons">person</i>
                        <asp:Label ID="TituloNombre" runat="server" Text=""></asp:Label>
                    </span>
                    <div class="row">
                        <div class="col s12">
                            <ul class="tabs">
                                <li class="tab col s3"><a href="#datos-personales">Datos Personales</a></li>
                                <li class="tab col s3"><a href="#grupo-familiar">Grupo Familiar</a></li>
                                <li class="tab col s3"><a href="#solicitudes">Solicitudes</a></li>
                            </ul>
                        </div>
                        <div id="datos-personales" class="col s12">
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">numbers</i>
                                    <asp:TextBox ID="CUSPP" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="CUSPP">CUSPP</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">event</i>
                                    <asp:TextBox ID="FechaNacimiento" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="FechaNacimiento">Fecha de Nacimiento</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">badge</i>
                                    <asp:DropDownList ID="TipoDocumento" runat="server" ClientIDMode="Static"></asp:DropDownList>
                                    <label for="TipoDocumento">Tipo de Documento</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">badge</i>
                                    <asp:TextBox ID="NumeroDocumento" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="NumeroDocumento">Número de Documento</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">person</i>
                                    <asp:TextBox ID="ApellidoPaterno" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="ApellidoPaterno">Apellido Paterno</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">person</i>
                                    <asp:TextBox ID="ApellidoMaterno" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="ApellidoMaterno">Apellido Materno</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">person</i>
                                    <asp:TextBox ID="Nombres" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="Nombres">Nombres</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">person</i>
                                    <asp:DropDownList ID="Sexo" runat="server" ClientIDMode="Static"></asp:DropDownList>
                                    <label for="Sexo">Sexo</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">call</i>
                                    <asp:TextBox ID="Telefono" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="Telefono">Teléfono</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">smartphone</i>
                                    <asp:TextBox ID="Celular" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="Celular">Celular</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">email</i>
                                    <asp:TextBox ID="CorreoElectronico" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="CorreoElectronico">Correo Electrónico</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">person</i>
                                    <asp:DropDownList ID="EstadoCivil" runat="server" ClientIDMode="Static"></asp:DropDownList>
                                    <label for="EstadoCivil">Estado Civil</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">corporate_fare</i>
                                    <asp:DropDownList ID="AFP" runat="server" ClientIDMode="Static"></asp:DropDownList>
                                    <label for="AFP">AFP</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">corporate_fare</i>
                                    <asp:TextBox ID="CentroLaboral" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="CentroLaboral">Centro Laboral</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">payments</i>
                                    <asp:TextBox ID="CIC" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="CIC">Fondo Aproximado (CIC)</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">payments</i>
                                    <asp:TextBox ID="RangoInversion" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="RangoInversion">Rango de Inversión</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">account_box</i>
                                    <asp:TextBox ID="Agente" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="Agente">Agente</label>
                                </div>
                            </div>

                            <ul class="collapsible">
                                <li>
                                    <div class="collapsible-header">
                                        <i class="material-icons">home</i>
                                        Direcciones
                                    </div>
                                    <div class="collapsible-body">
                                        <div class="row" style="margin-top:20px">
                                            <div class="col s12 right-align">
                                                <asp:HyperLink ID="NuevaDireccion" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">add_home</i>Nueva Dirección</asp:HyperLink>
                                            </div>
                                        </div>
                                        <asp:GridView
                                            ID="TablaDirecciones" runat="server"
                                            ViewStateMode="Disabled" ClientIDMode="Static"
                                            AutoGenerateColumns="False" CssClass="highlight">    
                                            <Columns>
                                                <asp:TemplateField HeaderText="Dirección" HeaderStyle-CssClass="vis-sm" ItemStyle-CssClass="vis-sm">
                                                    <ItemTemplate>
                                                        <asp:Label ID="DireccionCompleta" runat="server" Text='<%# string.Format("{0} {1} {2}<br>{3} - {4} - {5}", Eval("TipoVia.Glosa"), Eval("Glosa"), Eval("EspacioUrbano"), Eval("Comuna.Nombre"), Eval("Ciudad.Nombre"), Eval("Departamento.Nombre")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Dirección" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                                    <ItemTemplate>
                                                        <div style="width:240px;overflow:hidden;text-overflow:ellipsis;">
                                                            <%#  Eval("TipoVia.Glosa") + " " + Eval("Glosa") + " " + Eval("EspacioUrbano") %>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Distrito" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Distrito" runat="server" Text='<%# Bind("Comuna.Nombre") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Provincia" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Provincia" runat="server" Text='<%# Bind("Ciudad.Nombre") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Departamento" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Parentesco" runat="server" Text='<%# Bind("Departamento.Nombre") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="FechaIngreso"
                                                    HeaderText="Fecha de Registro" DataFormatString="{0:dd/MM/yyyy}"
                                                    HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Prncipal">
                                                    <ItemTemplate>
                                                        <%# (Boolean.Parse(Eval("Principal").ToString())) ? "Sí" : "No" %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <%# "<a class=\"center\" href=\"" + ResolveUrl(string.Format("~/Comun/MantenimientoDireccion.aspx?o=IFP&a=M&d={0}&c={1}", Eval("Id"), CUSPP.Text)) + "\"><i class=\"material-icons pink-text darken-2-text\" title=\"Modificar Dirección\">edit</i></a>" %>
                                                        <%# "<a class=\"center\" href=\"" + ResolveUrl(string.Format("~/Comun/MantenimientoDireccion.aspx?o=IFP&a=E&d={0}&c={1}", Eval("Id"), CUSPP.Text)) + "\"><i class=\"material-icons pink-text darken-2-text\" title=\"Eliminar Dirección\">delete</i></a>" %>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <p id="TabDireccionesVacia" class="center-align" style="font-size:1rem"><i class="material-icons blue-text text-darken-2" style="margin-right:10px">info</i> No se han encontrado direcciones.</p>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                        <div class="row" style="margin-top:20px">
                                            <div class="col s12 right-align">
                                                <asp:HyperLink ID="VerMasDirecciones" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">visibility</i>Ver más Direcciones</asp:HyperLink>
                                                <asp:HyperLink ID="VerMasReciente" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">visibility_off</i>Ver más reciente</asp:HyperLink>
                                            </div>
                                        </div>
                                    </div>
                                </li>
                            </ul>                        

                            <div class="row">
                                <div class="col s12 center-align">
                                    <asp:HyperLink ID="GuardarDatos" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">save</i>Guardar</asp:HyperLink>
                                </div>
                            </div>
                        </div>
                        <div id="grupo-familiar" class="col s12">
                            <div class="row" style="margin-top:20px">
                                <div class="col s12 right-align">
                                    <asp:HyperLink ID="NuevoBeneficiario" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">person_add</i>Nuevo Beneficiario</asp:HyperLink>
                                </div>
                            </div>

                            <asp:GridView
                                ID="TablaGrupoFamiliar" runat="server"
                                ViewStateMode="Disabled" ClientIDMode="Static"
                                AutoGenerateColumns="False" CssClass="highlight" OnRowDataBound="TablaGrupoFamiliar_RowDataBound">    
                                <Columns>
                                    <asp:TemplateField HeaderText="Nombre" HeaderStyle-CssClass="vis-sm" ItemStyle-CssClass="vis-sm">
                                        <ItemTemplate>
                                            <asp:Label ID="NombreCompleto" runat="server" Text='<%# string.Format("{0} {1}, {2}", Eval("ApellidoPaterno"), Eval("ApellidoMaterno"), Eval("Nombre")) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Apellido Paterno" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                        <ItemTemplate>
                                            <asp:Label ID="ApellidoPaterno" runat="server" Text='<%# Bind("ApellidoPaterno") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Apellido Materno" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                        <ItemTemplate>
                                            <asp:Label ID="ApellidoMaterno" runat="server" Text='<%# Bind("ApellidoMaterno") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Nombre" HeaderStyle-CssClass="vis-l" ItemStyle-CssClass="vis-l">
                                        <ItemTemplate>
                                            <asp:Label ID="Nombre" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Parentesco">
                                        <ItemTemplate>
                                            <asp:Label ID="Parentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Sexo" HeaderStyle-CssClass="vis-ml" ItemStyle-CssClass="vis-ml">
                                        <ItemTemplate>
                                            <asp:Label ID="Sexo" runat="server" Text='<%# Bind("Sexo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="FechaNacimiento"
                                        HeaderText="Fecha de Nacimiento" DataFormatString="{0:dd/MM/yyyy}">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <%# "<a class=\"center\" href=\"" + ResolveUrl("~/Comun/MantenimientoGrupoFamiliar.aspx?a=M&gf=") + Eval("Id") + "\"><i class=\"material-icons pink-text darken-2-text\" title=\"Modificar Beneficiario\">edit</i></a>"%>
                                            <%# (Eval("Parentesco.Id").ToString() != Enums.Parentesco.Afiliado.StringValue()) ? "<a class=\"center\" href=\"" + ResolveUrl("~/Comun/MantenimientoGrupoFamiliar.aspx?a=E&gf=") + Eval("Id") + "\"><i class=\"material-icons pink-text darken-2-text\" title=\"Eliminar Beneficiario\">delete</i></a>" : string.Empty %>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <p style="font-size:1rem"><i class="material-icons amber-text text-darken-2" style="margin-right:10px">warning</i> No se han encontrado beneficiarios.</p>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                        <div id="solicitudes" class="col s12">
                            <div class="row" style="margin-top:20px">
                                <div class="col s12 right-align">
                                    <asp:HyperLink ID="NuevaSolicitud" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">assignment_add</i>Nuevo Ingreso Flexible Plus</asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $(document).ready(function () {
            DeshabilitarCampos();

            $("#BuscarPorCUSPP").click(function (e) {
                let esCorrecto = true;
                let bCUSPP = false;
                let bSolicitud = false;

                // Validaciones
                if ($("#BusquedaCUSPP").val() != "") {
                    bCUSPP = true;
                }

                if ($("#BusquedaSolicitud").val() != "") {
                    bSolicitud= true;
                }

                if (bCUSPP & bSolicitud) {
                    cerrarModalCargando();
                    abrirModalAlerta("Error", "Seleccione sólo un criterio de búsqueda.", "error", "red");
                    esCorrecto = false;
                }

                if (esCorrecto) {
                    abrirModalCargando("Buscando, por favor espere...");

                    let url = "<%=ResolveUrl("~/IFP/Principal.aspx")%>";
                    if (bCUSPP) {
                        url = url + "?c=" + $("#BusquedaCUSPP").val();
                    }
                    if (bSolicitud) {
                        url = url + "?s=" + $("#BusquedaSolicitud").val();
                    }
                    window.location.href = url;
                }
            });

            function DeshabilitarCampos() {
                console.log("aaa");
                $("#TipoDocumento").prop("disabled", true);
                $("#Sexo").prop("disabled", true);
                $("#EstadoCivil").prop("disabled", true);
                actualizarCombobox();
            }
        });
    </script>
</asp:Content>
