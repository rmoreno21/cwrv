﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IniciarSesion.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Seguridad.IniciarSesion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Interseguro | Control de Acceso a Cotizador Web de Rentas </title>
    <script src="https://code.jquery.com/jquery-3.4.1.min.js" integrity="sha256-CSXorXvZcTkaix6Yvo6HppcZGetbYMGWSFlBw8HfCJo=" crossorigin="anonymous"></script>

    <!-- Compiled and minified CSS -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/css/materialize.min.css" />

    <!-- Compiled and minified JavaScript -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/js/materialize.min.js"></script>

    <!-- Material Icons -->
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />

    <!--favicon-->
    <link type="image/x-icon" href="../Iconos/favicon.ico" rel="shortcut icon" />
	<link type="image/x-icon" href="../Iconos/favicon.ico" rel="icon" />
    <link rel="apple-touch-icon" sizes="57x57" href="../Iconos/apple-icon-57x57.png">
    <link rel="apple-touch-icon" sizes="60x60" href="../Iconos/apple-icon-60x60.png">
    <link rel="apple-touch-icon" sizes="72x72" href="../Iconos/apple-icon-72x72.png">
    <link rel="apple-touch-icon" sizes="76x76" href="../Iconos/apple-icon-76x76.png">
    <link rel="apple-touch-icon" sizes="114x114" href="../Iconos/apple-icon-114x114.png">
    <link rel="apple-touch-icon" sizes="120x120" href="../Iconos/apple-icon-120x120.png">
    <link rel="apple-touch-icon" sizes="144x144" href="../Iconos/apple-icon-144x144.png">
    <link rel="apple-touch-icon" sizes="152x152" href="../Iconos/apple-icon-152x152.png">
    <link rel="apple-touch-icon" sizes="180x180" href="../Iconos/apple-icon-180x180.png">
    <link rel="icon" type="image/png" sizes="192x192" href="../Iconos/android-icon-192x192.png">
    <link rel="icon" type="image/png" sizes="32x32" href="../Iconos/favicon-32x32.png">
    <link rel="icon" type="image/png" sizes="96x96" href="../Iconos/favicon-96x96.png">
    <link rel="icon" type="image/png" sizes="16x16" href="../Iconos/favicon-16x16.png">
    <link rel="manifest" href="../Iconos/manifest.json">
    <meta name="msapplication-TileColor" content="#ffffff">
    <meta name="msapplication-TileImage" content="../Iconos/ms-icon-144x144.png">
    <meta name="theme-color" content="#ffffff">

    <style>
		body {
			/*background: #004899 url(../Imagenes/bk_login.svg) no-repeat center center fixed;*/
			background: #004D9C url(../Imagenes/bk_login_gdh.svg) no-repeat bottom center fixed;

			height: 100%;
            font-family: Roboto, "Helvetica Neue", sans-serif;
		}

        .login-titulo {
            display:inline-block;
            text-align: center;
            padding: 10px 0;
            width: 330px;
            background: rgb(8,85,196);
            background: linear-gradient(90deg, rgba(8,85,196,0.85) 0%, rgba(0,173,238,0.85) 100%);
            box-shadow: 1px 1px 50px rgba(0,0,0,.78);
            transition: box-shadow 280ms cubic-bezier(.4,0,.2,1);
            color: rgba(255, 255, 255, 0.87);
            -webkit-border-top-left-radius: 40px;
            -webkit-border-bottom-right-radius: 40px;
               -moz-border-radius-topleft: 40px;
               -moz-border-radius-bottomright: 40px;
                    border-top-left-radius: 40px;
                    border-bottom-right-radius: 40px;
        }

        .login-contenedor {
            padding: 20px 0;
            margin: 8px 0;
            width: 330px;
            background-color: #FFF;
            transition: box-shadow 280ms cubic-bezier(.4,0,.2,1);
            color: rgba(0, 0, 0, 0.87);
            -webkit-border-top-left-radius: 40px;
            -webkit-border-bottom-right-radius: 40px;
               -moz-border-radius-topleft: 40px;
               -moz-border-radius-bottomright: 40px;
                    border-top-left-radius: 40px;
                    border-bottom-right-radius: 40px;
        }

        .btn {
            text-transform: none;
            font-family: Roboto, "Helvetica Neue", sans-serif;
            font-weight: bold;
            letter-spacing: 0;
        }

        input[type=submit] {
            color: #FFF;
            letter-spacing: 0px;
            font-family: Roboto, "Helvetica Neue", sans-serif;
            font-size: 14px;
            font-weight: 600;
        }

        /* label color */
        .input-field label {
            color: #999;
        }
        /* label focus color */
        .input-field input:focus + label {
            color: #1565c0 !important;
        }
        /* label underline focus color */
        .input-field input:focus {
            border-bottom: 1px solid #0d47a1 !important;
            box-shadow: 0 1px 0 0 #0d47a1 !important;
        }
        /* valid color */
        .input-field input.valid {
            border-bottom: 1px solid #0d47a1 !important;
            box-shadow: 0 1px 0 0 #0d47a1 !important;
        }
        /* invalid color */
        .input-field input.invalid {
            border-bottom: 1px solid red !important;
            box-shadow: 0 1px 0 0 #000;
        }
        /* icon prefix focus color */
        .input-field .prefix.active {
            color: #1565c0;
        }
	</style>

    <script>
        var API_AUTENTICACION_URL = '<asp:Literal ID="lbApiAutenticacionUrl" runat="server" />';

        console.log("API_AUTENTICACION_URL: ", API_AUTENTICACION_URL)
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <asp:HiddenField ID="reCAPTCHAToken" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="reCAPTCHASiteKey" runat="server" ClientIDMode="Static" />
        <div class="row valign-wrapper" style="position:absolute;top:0;left:0;width:100%;height:100%">
            <div class="col s12" style="text-align:center;">
                <div class="login-titulo">
                    <img src="../Imagenes/logo-interseguro.svg" style="width:170px" />
                    <section class="login-contenedor">
                        <span style="font-size:1.2em">Cotizador Web de Rentas</span>
                        <div style="padding:0 20px 0 5px;">
                            <div class="input-field">
                                <i class="material-icons prefix">person</i>
                                <asp:TextBox ID="Usuario" CssClass="validate" MaxLength="20" runat="server" ClientIDMode="Static" autocomplete="off" required="" aria-required="true"></asp:TextBox>
                                <label for="Usuario">Usuario</label>
                                <span class="helper-text" data-error="Por favor ingrese su usuario de red"></span>
                            </div>
                            <div class="input-field">
                                <i class="material-icons prefix">lock</i>
                                <asp:TextBox ID="Contrasenha" CssClass="validate" MaxLength="20" runat="server" TextMode="Password" ClientIDMode="Static" required="" aria-required="true" ></asp:TextBox>
                                <label for="Contrasenha">Contraseña</label>
                                <span class="helper-text" data-error="Por favor ingrese su contraseña"></span>
                            </div>
                        </div>
                        <asp:Label ID="Error" Text="" Visible="false" style="display:block;color:red;font-size:0.8rem;margin-bottom:15px"></asp:Label>
                        <asp:Button ID="IniSesion" CssClass="btn blue darken-3 waves-effect waves-light" style="color: #FFF;" runat="server" ClientIDMode="Static" Text="Iniciar Sesión" OnClientClick="Evaluar(event)" />
                    </section>
                    <div style="line-height:110%">
                        <asp:Label ID="VersionSistema" runat="server" style="font-size:0.8em"></asp:Label>
                    </div>
                    <div style="line-height:110%">
                        <span style="font-size:0.8em">2013 - <% Response.Write(DateTime.Now.Year); %> Tecnología de la Información</span>
                    </div>
                </div>
            </div>         
        </div>
    </form>

    <script src="https://www.google.com/recaptcha/api.js?render=<%= ConfigurationManager.AppSettings["reCAPTCHASiteKey"].ToString() %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/utilitarios.js") %>?v=<%= DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/apiAutenticacion.js") %>?v=<%= DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/session.js") %>?v=<%= DateTime.Now.ToString("yyyyMMdd") %>"></script>

    <script>
        var inicioSesion = false;
        var validarCaptcha = false;

        $(document).ready(function () {
            validar();
            $("input.validate").on("keyup", validar);
            $("input.validate").on("focus", validar);
            $("#Usuario").focus();
            document.getElementById("form1").addEventListener("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault(); // evita que se dispare la validación automática
                    $("#IniSesion").focus();
                    $("#IniSesion").click();
                }
            });
        });

        function validar() {
            if (!inicioSesion) {
                var inputsConValores = 0;

                // Obtener todos los campos input fields con excepción de los type='submit'
                var inputs = $("input.validate");

                inputs.each(function (e) {
                    // Si el input tiene un valor, incrementar el contador
                    if ($(this).val()) {
                        inputsConValores += 1;
                    }
                });

                if (inputsConValores == inputs.length) {
                    $("#IniSesion").prop("disabled", false).removeClass("disabled");
                    $("#IniSesion").parent().removeClass("disabled");
                }
                else {
                    $("#IniSesion").prop("disabled", true).addClass("disabled");
                    $("#IniSesion").parent().addClass("disabled");
                }
            }
        }

        function Evaluar(e) {
            if (!validarCaptcha) {
                if (!inicioSesion) {
                    if (!validarCaptcha) {
                        grecaptcha.ready(function () {
                            grecaptcha.execute($("#reCAPTCHASiteKey").val(), { action: "submit" }).then(function (token) {
                                validarCaptcha = true;
                                $("#reCAPTCHAToken").val(token);

                                $("#Error").fadeOut();
                                $("#IniSesion").addClass("disabled");
                                $("#IniSesion").parent().addClass("disabled");
                                inicioSesion = true;
                                $("#IniSesion").click();
                            });
                        });
                    }
                    if (!inicioSesion) {
                        e.preventDefault();
                        return false;
                    }
                }
                else {
                    e.preventDefault();
                    return false;
                }
            }
        }
    </script>
</body>
</html>