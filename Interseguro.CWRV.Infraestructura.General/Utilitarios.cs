using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web;

namespace Interseguro.CWRV.Infraestructura.General
{
    public static class Utilitarios
    {
        public static string FormatearError(List<string> errores)
        {
            string cadena = "";
            int i;
            for (i = 0; i < errores.Count; i++)
            {
                cadena += "<div style=\"margin: 5px 0\">" + errores[i] + "</div>";
            }
            return cadena;
        }

        public static string FormatearErrorTexto(List<string> errores)
        {
            string cadena = "";
            int i;
            for (i = 0; i < errores.Count; i++)
            {
                cadena += errores[i] + "\n";
            }
            return cadena;
        }


        public static string FormatearBytes(long bytes, bool si)
        {
            int unit = si ? 1000 : 1024;
            if (bytes < unit) return bytes + " B";
            int exp = (int)(Math.Log(bytes) / Math.Log(unit));
            String pre = (si ? "kMGTPE" : "KMGTPE")[exp - 1].ToString(); // +(si ? "" : "i");
            return String.Format("{0:0.##} {1}B", bytes / Math.Pow(unit, exp), pre);
        }

        public static bool ValidarPermiso(object listaPermisos, Enums.OpcionesSistema permiso)
        {
            List<OpcionSistema> opciones = (List<OpcionSistema>)listaPermisos;
            if (opciones != null && opciones.Count != 0)
            {
                OpcionSistema opcion = opciones.Find(o => o.IdAzman == (int)permiso);
                return (opcion != null && opcion.Activa);
            }
            else
            {
                return false;
            }
        }

        public static int ObtenerEdad(DateTime fechaCalcular, DateTime fechaNacimiento)
        {
            return fechaCalcular.AddTicks(-fechaNacimiento.Ticks).Year - 1;
        }

        public static string EnmascararNombre(string nombre)
        {
            string nombreEnmasacarado = String.Empty;
            string[] palabras = nombre.Trim().Split(' ');
            for (int i = 0; i < palabras.Length; i++)
            {
                string palabraEnmascarada = String.Empty;
                int letrasEnmascaradas = (palabras[i].Length / 2) + (palabras[i].Length % 2);
                for (int j = 0; j < palabras[i].Length; j++)
                {
                    if (j < letrasEnmascaradas)
                    {
                        palabraEnmascarada += "*";
                    }
                    else
                    {
                        palabraEnmascarada += palabras[i][j];
                    }
                }
                palabras[i] = palabraEnmascarada;
            }
            nombreEnmasacarado = String.Join("*", palabras);
            return nombreEnmasacarado;
        }

        public static string PrimeraMayuscula(string palabra)
        {
            if (string.IsNullOrEmpty(palabra))
            {
                return string.Empty;
            }
            return char.ToUpper(palabra[0]) + palabra.Substring(1);
        }

        public static bool ValidarRedLocal(string ip)
        {
            bool permitida = false;
            foreach (string ipPermitida in ConfigurationManager.AppSettings["RangoIPsParametrosEspeciales"].Split(' '))
            {
                if (ip.StartsWith(ipPermitida))
                {
                    permitida = true;
                    break;
                }
            }
            return permitida;
        }

        public static bool ValidarIPPermitida(string ip)
        {
            bool permitida = false;
            foreach (string ipPermitida in ConfigurationManager.AppSettings["ListaIPsPermitidas"].Split(' '))
            {
                if (ip == ipPermitida)
                {
                    permitida = true;
                    break;
                }
            }
            return permitida;
        }

        public static string ObtenerNombreTerminal(HttpRequest Request)
        {
            string nombreTerminal = string.Empty;
            try
            {
                nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new char[] { '.' })[0].ToString());
            }
            catch (Exception) { }
            nombreTerminal += Request.UserAgent;
            return nombreTerminal;
        }

        public static string CadenaAleatoria(int longitud, string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (longitud < 0) throw new ArgumentOutOfRangeException("longitud", "La longitud no puede ser menor a cero.");
            if (string.IsNullOrEmpty(caracteresPermitidos)) throw new ArgumentException("El parámetro caracteresPermitidos no debe estar vacío.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(caracteresPermitidos).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("El parámetro caracteresPermitidos no puede contener más de {0} caracteres.", byteSize));

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < longitud)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < longitud; ++i)
                    {
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }

        public static string ConvertirNroLetras(object num, bool conDecimal = false)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;
            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }
            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2, MidpointRounding.AwayFromZero));
            if (decimales > 0)
            {
                if (conDecimal)
                    dec = " CON " + decimales.ToString() + "/100";
            }
            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }

        private static string toText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
            }
            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }
            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }

            return Num2Text;
        }

        public static byte[] ConsumirServicio(string url, string usuario, string token)
        {
            WebClient myWebClient = new WebClient();
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token));
            myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            byte[] formatoByteArray = myWebClient.DownloadData(url);
            myWebClient.Dispose();
            return formatoByteArray;
        }

        public static bool EsRolComercial(string rolAzman)
        {
            if (
                rolAzman == Enums.RolAzman.AgenteExterno.StringValue() ||
                rolAzman == Enums.RolAzman.AgenteLima.StringValue() ||
                rolAzman == Enums.RolAzman.AgenteProvincia.StringValue() ||
                rolAzman == Enums.RolAzman.SupervisorLima.StringValue() ||
                rolAzman == Enums.RolAzman.SupervisorProvincia.StringValue() ||
                rolAzman == Enums.RolAzman.JefeVentaLima.StringValue() ||
                rolAzman == Enums.RolAzman.JefeVentaProvincia.StringValue() ||
                rolAzman == Enums.RolAzman.AsistenteComercial.StringValue() ||
                rolAzman == Enums.RolAzman.GerenteDivision.StringValue()
            )
                return true;
            else
                return false;
        }

        public static bool EsRolOperaciones(string rolAzman)
        {
            if (
                rolAzman == Enums.RolAzman.AsistenteOperaciones.StringValue() ||
                rolAzman == Enums.RolAzman.AnalistaOperaciones.StringValue() ||
                rolAzman == Enums.RolAzman.JefeOperaciones.StringValue()
            )
                return true;
            else
                return false;
        }

        public static bool EsRolVerAgentesCesados(string rolAzman)
        {
            if (
                rolAzman == Enums.RolAzman.SupervisorLima.StringValue() ||
                rolAzman == Enums.RolAzman.SupervisorProvincia.StringValue() ||
                rolAzman == Enums.RolAzman.JefeVentaLima.StringValue() ||
                rolAzman == Enums.RolAzman.JefeVentaProvincia.StringValue() ||
                rolAzman == Enums.RolAzman.JefeOperaciones.StringValue() ||
                rolAzman == Enums.RolAzman.AnalistaOperaciones.StringValue() ||
                rolAzman == Enums.RolAzman.AsistenteOperaciones.StringValue() ||
                rolAzman == Enums.RolAzman.AsistenteComercial.StringValue() ||
                rolAzman == Enums.RolAzman.GerenteDivision.StringValue()
            )
                return true;
            else
                return false;
        }

        public static dynamic ObtenerConfiguracionSME(string proceso, DateTime fecha)
        {
            string urlEndpoint = string.Format("{0}/configuraciones/{1}/{2}", ConfigurationManager.AppSettings["url_envio_documentos_sme"], proceso, fecha.ToString("yyyy-MM-dd"));
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlEndpoint);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string responseBody = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject(responseBody);
            }
        }

        public static string CrearFilaComparativa(string nombreCampo, object valorPrevio, object valorActual)
        {
            string html = string.Empty;
            html += "<tr>";
            html += "<td style=\"padding-left:10px\">";
            html += string.Format("<span style=\"font-size:18px;color:#FFF;font-family:Arial,Helvetica,sans-serif;\">{0}</span>", nombreCampo);
            html += "</td>";
            html += "<td style=\"border-radius:10px;background:#EDF4FC;padding-right:40px;text-align:right;height:35px\">";
            html += string.Format("<span style=\"font-size:18px;color:#1F1F1F;font-family:Arial,Helvetica,sans-serif;\">{0}</span>", valorPrevio);
            html += "</td>";
            html += "<td width=\"25\">";
            html += "</td>";
            html += "<td style=\"border-radius:10px;background:#EDF4FC;padding-right:40px;text-align:right;height:35px\">";
            html += string.Format("<span style=\"font-size:18px;color:#1F1F1F;font-family:Arial,Helvetica,sans-serif;\">{0}</span>", valorActual);
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td style=\"height:40px\">";
            html += "</td>";
            html += "</tr>";
            return html;
        }

        public static string CrearTablaComparativa(FormatoSolicitud formatoActual, FormatoSolicitud formatoPrevio, List<FormatoSolicitudBeneficiario> formatoBeneficiariosActual, List<FormatoSolicitudBeneficiario> formatoBeneficiariosPrevio, List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasActual, List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasPrevio)
        {
            string html = string.Empty;
            html += "<table style=\"width:400px;height:auto;\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
            html += "<tbody>";
            html += "<tr>";
            html += "<td style=\"padding-left:10px\">";
            html += "<span style=\"font-size:18px;color:#FFF;font-family:Arial,Helvetica,sans-serif\"></span>";
            html += "</td>";
            html += "<td style=\"padding-right:50px;text-align:right;height:35px\">";
            html += "<span style=\"font-size:18px;color:#FFF;font-family:Arial,Helvetica,sans-serif\">Antes</span>";
            html += "</td>";
            html += "<td width=\"40\">";
            html += "</td>";
            html += "<td style=\"padding-right:50px;text-align:right;height:35px\">";
            html += "<span style=\"font-size:18px;color:#FFF;font-family:Arial,Helvetica,sans-serif\">Ahora</span>";
            html += "</td>";

            if (formatoActual != null && formatoPrevio != null)
            {
                if (formatoPrevio.NumeroCotizacion != formatoActual.NumeroCotizacion)
                {
                    html += CrearFilaComparativa("Número de Cotización", formatoPrevio.NumeroCotizacion, formatoActual.NumeroCotizacion);
                }

                if (formatoPrevio.TipoCotizacion.Nombre != formatoActual.TipoCotizacion.Nombre)
                {
                    html += CrearFilaComparativa("Tipo de Cotización", formatoPrevio.TipoCotizacion.Nombre, formatoActual.TipoCotizacion.Nombre);
                }

                if (formatoPrevio.CodigoPlan != formatoActual.CodigoPlan)
                {
                    html += CrearFilaComparativa("Plan", formatoPrevio.CodigoPlan, formatoActual.CodigoPlan);
                }

                if (formatoPrevio.CodigoTipoPlanRPP != formatoActual.CodigoTipoPlanRPP)
                {
                    html += CrearFilaComparativa("Plan RPP", formatoPrevio.CodigoTipoPlanRPP, formatoActual.CodigoTipoPlanRPP);
                }

                if (formatoPrevio.MonedaCIC.Nombre != formatoActual.MonedaCIC.Nombre)
                {
                    html += CrearFilaComparativa("Moneda de CIC", formatoPrevio.MonedaCIC.Nombre, formatoActual.MonedaCIC.Nombre);
                }

                if (formatoPrevio.CIC != formatoActual.CIC)
                {
                    html += CrearFilaComparativa("Moneda de CIC", formatoPrevio.CodigoTipoPlanRPP, formatoActual.CodigoTipoPlanRPP);
                }

                if (formatoPrevio.Temporalidad.Nombre != formatoActual.Temporalidad.Nombre)
                {
                    html += CrearFilaComparativa("Temporalidad", formatoPrevio.Temporalidad.Nombre, formatoActual.Temporalidad.Nombre);
                }

                if (formatoPrevio.Moneda.Nombre != formatoActual.Moneda.Nombre)
                {
                    html += CrearFilaComparativa("Moneda", formatoPrevio.Moneda.Nombre, formatoActual.Moneda.Nombre);
                }

                if (formatoPrevio.PorcentajeAjuste != formatoActual.PorcentajeAjuste)
                {
                    html += CrearFilaComparativa("Porcentaje de ajuste", formatoPrevio.PorcentajeAjuste, formatoActual.PorcentajeAjuste);
                }

                if (formatoPrevio.MesesTramo1 != formatoActual.MesesTramo1)
                {
                    html += CrearFilaComparativa("Meses del tramo 1", formatoPrevio.MesesTramo1, formatoActual.MesesTramo1);
                }

                if (formatoPrevio.PorcentajeTramo2 != formatoActual.PorcentajeTramo2)
                {
                    html += CrearFilaComparativa("Porcentaje del trampo 2", formatoPrevio.PorcentajeTramo2, formatoActual.PorcentajeTramo2);
                }

                if (formatoPrevio.MesesGarantizados != formatoActual.MesesGarantizados)
                {
                    html += CrearFilaComparativa("Meses de periodo garantizado", formatoPrevio.MesesGarantizados, formatoActual.MesesGarantizados);
                }

                if (formatoPrevio.IndCoberturaAdicionalFallecimiento != formatoActual.IndCoberturaAdicionalFallecimiento)
                {
                    html += CrearFilaComparativa("Cobertura adicional de fallecimiento", formatoPrevio.IndCoberturaAdicionalFallecimiento, formatoActual.IndCoberturaAdicionalFallecimiento);
                }

                if (formatoPrevio.PorcentajeDevolucionFallecimiento != formatoActual.PorcentajeDevolucionFallecimiento)
                {
                    html += CrearFilaComparativa("Porcentaje de devolución por fallecimiento", formatoPrevio.PorcentajeDevolucionFallecimiento, formatoActual.PorcentajeDevolucionFallecimiento);
                }

                if (formatoPrevio.IndCoberturaAdicionalDevolucion != formatoActual.IndCoberturaAdicionalDevolucion)
                {
                    html += CrearFilaComparativa("Cobertura adicional por sobrevivencia", formatoPrevio.IndCoberturaAdicionalDevolucion, formatoActual.IndCoberturaAdicionalDevolucion);
                }

                if (formatoPrevio.PorcentajeDevolucionSobrevivencia != formatoActual.PorcentajeDevolucionSobrevivencia)
                {
                    html += CrearFilaComparativa("Porcentaje de devolución", formatoPrevio.PorcentajeDevolucionSobrevivencia, formatoActual.PorcentajeDevolucionSobrevivencia);
                }

                if (formatoPrevio.IndSepelio != formatoActual.IndSepelio)
                {
                    html += CrearFilaComparativa("Sepelio", formatoPrevio.IndSepelio, formatoActual.IndSepelio);
                }

                if (formatoPrevio.Renta != formatoActual.Renta)
                {
                    html += CrearFilaComparativa("Renta", formatoPrevio.Renta, formatoActual.Renta);
                }

                if (formatoPrevio.RentaTramo2 != formatoActual.RentaTramo2)
                {
                    html += CrearFilaComparativa("Renta del tramo 2", formatoPrevio.RentaTramo2, formatoActual.RentaTramo2);
                }

                if (formatoPrevio.Agente.Nombre != formatoActual.Agente.Nombre)
                {
                    html += CrearFilaComparativa("Agente", formatoPrevio.Agente.Nombre, formatoActual.Agente.Nombre);
                }

                if (formatoPrevio.IndConsentimientoNecesario != formatoActual.IndConsentimientoNecesario)
                {
                    html += CrearFilaComparativa("Consentimiento necesario", formatoPrevio.IndConsentimientoNecesario, formatoActual.IndConsentimientoNecesario);
                }

                if (formatoPrevio.IndConsentimientoOpcional != formatoActual.IndConsentimientoOpcional)
                {
                    html += CrearFilaComparativa("Consentimiento opcional", formatoPrevio.IndConsentimientoOpcional, formatoActual.IndConsentimientoOpcional);
                }

                if (formatoPrevio.PorcentajeConyuge != formatoActual.PorcentajeConyuge)
                {
                    html += CrearFilaComparativa("Porcentaje del cónyuge", formatoPrevio.PorcentajeConyuge, formatoActual.PorcentajeConyuge);
                }

                if (formatoPrevio.FechaVigencia.ToString("dd/MM/yyyy") != formatoActual.FechaVigencia.ToString("dd/MM/yyyy"))
                {
                    html += CrearFilaComparativa("Fecha de vigencia", formatoPrevio.FechaVigencia.ToString("dd/MM/yyyy"), formatoActual.FechaVigencia.ToString("dd/MM/yyyy"));
                }

                if (formatoPrevio.Rescate != formatoActual.Rescate)
                {
                    html += CrearFilaComparativa("Rescate", formatoPrevio.Rescate ? "S" : "N", formatoActual.Rescate ? "S" : "N");
                }

                if (formatoPrevio.DeclaracionJurada != formatoActual.DeclaracionJurada)
                {
                    html += CrearFilaComparativa("Declaración Jurada", formatoPrevio.DeclaracionJurada, formatoActual.DeclaracionJurada);
                }

                if (formatoPrevio.Direccion.TipoVia.Nombre != formatoActual.Direccion.TipoVia.Nombre ||
                    formatoPrevio.Direccion.Glosa != formatoActual.Direccion.Glosa ||
                    formatoPrevio.Direccion.EspacioUrbano != formatoActual.Direccion.EspacioUrbano ||
                    formatoPrevio.Direccion.Departamento.Nombre != formatoActual.Direccion.Departamento.Nombre ||
                    formatoPrevio.Direccion.Ciudad.Nombre != formatoActual.Direccion.Ciudad.Nombre ||
                    formatoPrevio.Direccion.Comuna.Nombre != formatoActual.Direccion.Comuna.Nombre)
                {
                    html += CrearFilaComparativa("Dirección",
                        string.Format("{0} {1} {2} - {3}, {4}, {5}",
                            formatoPrevio.Direccion.TipoVia.Nombre,
                            formatoPrevio.Direccion.Glosa,
                            formatoPrevio.Direccion.EspacioUrbano,
                            formatoPrevio.Direccion.Departamento.Nombre,
                            formatoPrevio.Direccion.Ciudad.Nombre,
                            formatoPrevio.Direccion.Comuna.Nombre),
                        string.Format("{0} {1} {2} - {3}, {4}, {5}",
                            formatoActual.Direccion.TipoVia.Nombre,
                            formatoActual.Direccion.Glosa,
                            formatoActual.Direccion.EspacioUrbano,
                            formatoActual.Direccion.Departamento.Nombre,
                            formatoActual.Direccion.Ciudad.Nombre,
                            formatoActual.Direccion.Comuna.Nombre));
                }
            }

            if (formatoBeneficiariosActual.Count == formatoBeneficiariosPrevio.Count)
            {
                for (int i = 0; i < formatoBeneficiariosActual.Count; i++)
                {
                    if (formatoBeneficiariosPrevio[i].Nombres != formatoBeneficiariosActual[i].Nombres)
                    {
                        html += CrearFilaComparativa(string.Format("Nombre (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Nombres, formatoBeneficiariosActual[i].Nombres);
                    }

                    if (formatoBeneficiariosPrevio[i].ApellidoPaterno != formatoBeneficiariosActual[i].ApellidoPaterno)
                    {
                        html += CrearFilaComparativa(string.Format("Apellido Paterno (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].ApellidoPaterno, formatoBeneficiariosActual[i].ApellidoPaterno);
                    }

                    if (formatoBeneficiariosPrevio[i].ApellidoMaterno != formatoBeneficiariosActual[i].ApellidoMaterno)
                    {
                        html += CrearFilaComparativa(string.Format("Apellido Materno (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].ApellidoMaterno, formatoBeneficiariosActual[i].ApellidoMaterno);
                    }

                    if (formatoBeneficiariosPrevio[i].FechaNacimiento.ToString("dd/MM/yyyy") != formatoBeneficiariosActual[i].FechaNacimiento.ToString("dd/MM/yyyy"))
                    {
                        html += CrearFilaComparativa(string.Format("Fecha de Nacimiento (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].FechaNacimiento.ToString("dd/MM/yyyy"), formatoBeneficiariosActual[i].FechaNacimiento.ToString("dd/MM/yyyy"));
                    }

                    if (formatoBeneficiariosPrevio[i].Identificacion.GlosaTipo != formatoBeneficiariosActual[i].Identificacion.GlosaTipo)
                    {
                        html += CrearFilaComparativa(string.Format("Tipo de Identificación (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Identificacion.GlosaTipo, formatoBeneficiariosActual[i].Identificacion.GlosaTipo);
                    }

                    if (formatoBeneficiariosPrevio[i].Identificacion.Numero != formatoBeneficiariosActual[i].Identificacion.Numero)
                    {
                        html += CrearFilaComparativa(string.Format("Número de Identificación (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Identificacion.Numero, formatoBeneficiariosActual[i].Identificacion.Numero);
                    }

                    if (formatoBeneficiariosPrevio[i].Parentesco.Nombre != formatoBeneficiariosActual[i].Parentesco.Nombre)
                    {
                        html += CrearFilaComparativa(string.Format("Parentesco (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Parentesco.Nombre, formatoBeneficiariosActual[i].Parentesco.Nombre);
                    }

                    if (formatoBeneficiariosPrevio[i].Sexo != formatoBeneficiariosActual[i].Sexo)
                    {
                        html += CrearFilaComparativa(string.Format("Sexo (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Sexo, formatoBeneficiariosActual[i].Sexo);
                    }

                    if (formatoBeneficiariosPrevio[i].EstadoCivil != null && formatoBeneficiariosActual[i].EstadoCivil != null && formatoBeneficiariosPrevio[i].EstadoCivil.gls_estado_civil != formatoBeneficiariosActual[i].EstadoCivil.gls_estado_civil)
                    {
                        html += CrearFilaComparativa(string.Format("Estado Civil (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].EstadoCivil.gls_estado_civil, formatoBeneficiariosActual[i].EstadoCivil.gls_estado_civil);
                    }

                    if (formatoBeneficiariosPrevio[i].Nacionalidad != null && formatoBeneficiariosActual[i].Nacionalidad != null && formatoBeneficiariosPrevio[i].Nacionalidad.gls_nacionalidad != formatoBeneficiariosActual[i].Nacionalidad.gls_nacionalidad)
                    {
                        html += CrearFilaComparativa(string.Format("Nacionalidad (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Nacionalidad.gls_nacionalidad, formatoBeneficiariosActual[i].Nacionalidad.gls_nacionalidad);
                    }

                    if (formatoBeneficiariosPrevio[i].Residencia != null && formatoBeneficiariosActual[i].Residencia != null && formatoBeneficiariosPrevio[i].Residencia.Nombre != formatoBeneficiariosActual[i].Residencia.Nombre)
                    {
                        html += CrearFilaComparativa(string.Format("Residencia (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Residencia.Nombre, formatoBeneficiariosActual[i].Residencia.Nombre);
                    }

                    if (formatoBeneficiariosPrevio[i].Profesion != null && formatoBeneficiariosActual[i].Profesion != null && formatoBeneficiariosPrevio[i].Profesion.gls_profesion != formatoBeneficiariosActual[i].Profesion.gls_profesion)
                    {
                        html += CrearFilaComparativa(string.Format("Profesión (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Profesion.gls_profesion, formatoBeneficiariosActual[i].Profesion.gls_profesion);
                    }

                    if (formatoBeneficiariosPrevio[i].CentroLaboral != formatoBeneficiariosActual[i].CentroLaboral)
                    {
                        html += CrearFilaComparativa(string.Format("Centro Laboral (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].CentroLaboral, formatoBeneficiariosActual[i].CentroLaboral);
                    }

                    if (formatoBeneficiariosPrevio[i].Cargo != formatoBeneficiariosActual[i].Cargo)
                    {
                        html += CrearFilaComparativa(string.Format("Cargo (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Cargo, formatoBeneficiariosActual[i].Cargo);
                    }

                    if (formatoBeneficiariosPrevio[i].ActividadEconomica != formatoBeneficiariosActual[i].ActividadEconomica)
                    {
                        html += CrearFilaComparativa(string.Format("Actividad Económica (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].ActividadEconomica, formatoBeneficiariosActual[i].ActividadEconomica);
                    }

                    if (formatoBeneficiariosPrevio[i].MonedaIngreso != null && formatoBeneficiariosActual[i].MonedaIngreso != null && formatoBeneficiariosPrevio[i].MonedaIngreso.Nombre != formatoBeneficiariosActual[i].MonedaIngreso.Nombre)
                    {
                        html += CrearFilaComparativa(string.Format("Moneda de Ingreso (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].MonedaIngreso.Nombre, formatoBeneficiariosActual[i].MonedaIngreso.Nombre);
                    }

                    if (formatoBeneficiariosPrevio[i].IngresoNeto != formatoBeneficiariosActual[i].IngresoNeto)
                    {
                        html += CrearFilaComparativa(string.Format("Ingreso Neto Mensual (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].IngresoNeto, formatoBeneficiariosActual[i].IngresoNeto);
                    }

                    if (formatoBeneficiariosPrevio[i].Telefono != formatoBeneficiariosActual[i].Telefono)
                    {
                        html += CrearFilaComparativa(string.Format("Teléfono (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Telefono, formatoBeneficiariosActual[i].Telefono);
                    }

                    if (formatoBeneficiariosPrevio[i].Celular != formatoBeneficiariosActual[i].Celular)
                    {
                        html += CrearFilaComparativa(string.Format("Celular (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Celular, formatoBeneficiariosActual[i].Celular);
                    }

                    if (formatoBeneficiariosPrevio[i].Email != formatoBeneficiariosActual[i].Email)
                    {
                        html += CrearFilaComparativa(string.Format("Correo Electrónico (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].Email, formatoBeneficiariosActual[i].Email);
                    }

                    if (formatoBeneficiariosPrevio[i].PEP != formatoBeneficiariosActual[i].PEP)
                    {
                        html += CrearFilaComparativa(string.Format("Persona Expuesta Políticamente (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].PEP, formatoBeneficiariosActual[i].PEP);
                    }

                    if (formatoBeneficiariosPrevio[i].SujetoObligado != formatoBeneficiariosActual[i].SujetoObligado)
                    {
                        html += CrearFilaComparativa(string.Format("Sujeto Obligado (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].SujetoObligado, formatoBeneficiariosActual[i].SujetoObligado);
                    }

                    if (formatoBeneficiariosPrevio[i].PorcentajeRenta != formatoBeneficiariosActual[i].PorcentajeRenta)
                    {
                        html += CrearFilaComparativa(string.Format("Porcentaje de Renta (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].PorcentajeRenta, formatoBeneficiariosActual[i].PorcentajeRenta);
                    }

                    if (formatoBeneficiariosPrevio[i].NombreBanco != formatoBeneficiariosActual[i].NombreBanco)
                    {
                        html += CrearFilaComparativa(string.Format("Banco (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].NombreBanco, formatoBeneficiariosActual[i].NombreBanco);
                    }

                    if (formatoBeneficiariosPrevio[i].NombreTipoCuenta != formatoBeneficiariosActual[i].NombreTipoCuenta)
                    {
                        html += CrearFilaComparativa(string.Format("Tipo de Cuenta (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].NombreTipoCuenta, formatoBeneficiariosActual[i].NombreTipoCuenta);
                    }

                    if (formatoBeneficiariosPrevio[i].NumeroCuenta != formatoBeneficiariosActual[i].NumeroCuenta)
                    {
                        html += CrearFilaComparativa(string.Format("Número de Cuenta (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].NumeroCuenta, formatoBeneficiariosActual[i].NumeroCuenta);
                    }

                    if (formatoBeneficiariosPrevio[i].CodigoConfidencialidadDatos != formatoBeneficiariosActual[i].CodigoConfidencialidadDatos)
                    {
                        html += CrearFilaComparativa(string.Format("Confidencialidad de Datos (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].CodigoConfidencialidadDatos, formatoBeneficiariosActual[i].CodigoConfidencialidadDatos);
                    }

                    if (formatoBeneficiariosPrevio[i].CodigoComunicacion != formatoBeneficiariosActual[i].CodigoComunicacion)
                    {
                        html += CrearFilaComparativa(string.Format("Comunicación (Ben. {0})", formatoBeneficiariosPrevio[i].Item), formatoBeneficiariosPrevio[i].CodigoComunicacion, formatoBeneficiariosActual[i].CodigoComunicacion);
                    }
                }
            }
            else
            {
                html += CrearFilaComparativa("Cantidad de Beneficiarios", formatoBeneficiariosPrevio.Count, formatoBeneficiariosActual.Count);
            }

            if (formatoPersonasVinculadasPrevio.Count == formatoPersonasVinculadasActual.Count)
            {
                for (int i = 0; i < formatoPersonasVinculadasPrevio.Count; i++)
                {
                    if (formatoPersonasVinculadasPrevio[i].Nombres != formatoPersonasVinculadasActual[i].Nombres)
                    {
                        html += CrearFilaComparativa(string.Format("Nombre (P.V. {0})", formatoPersonasVinculadasPrevio[i].Item), formatoPersonasVinculadasPrevio[i].Nombres, formatoPersonasVinculadasActual[i].Nombres);
                    }

                    if (formatoPersonasVinculadasPrevio[i].ApellidoPaterno != formatoPersonasVinculadasActual[i].ApellidoPaterno)
                    {
                        html += CrearFilaComparativa(string.Format("Apellido Paterno (P.V. {0})", formatoPersonasVinculadasPrevio[i].Item), formatoPersonasVinculadasPrevio[i].ApellidoPaterno, formatoPersonasVinculadasActual[i].ApellidoPaterno);
                    }

                    if (formatoPersonasVinculadasPrevio[i].ApellidoMaterno != formatoPersonasVinculadasActual[i].ApellidoMaterno)
                    {
                        html += CrearFilaComparativa(string.Format("Apellido Materno (P.V. {0})", formatoPersonasVinculadasPrevio[i].Item), formatoPersonasVinculadasPrevio[i].ApellidoMaterno, formatoPersonasVinculadasActual[i].ApellidoMaterno);
                    }

                    if (formatoPersonasVinculadasPrevio[i].Parentesco.Nombre != formatoPersonasVinculadasActual[i].Parentesco.Nombre)
                    {
                        html += CrearFilaComparativa(string.Format("Parentesco (P.V. {0})", formatoPersonasVinculadasPrevio[i].Item), formatoPersonasVinculadasPrevio[i].Parentesco.Nombre, formatoPersonasVinculadasActual[i].Parentesco.Nombre);
                    }

                    if (formatoPersonasVinculadasPrevio[i].Identificacion.GlosaTipo != formatoPersonasVinculadasActual[i].Identificacion.GlosaTipo)
                    {
                        html += CrearFilaComparativa(string.Format("Tipo de Identificación (P.V. {0})", formatoPersonasVinculadasPrevio[i].Item), formatoPersonasVinculadasPrevio[i].Identificacion.GlosaTipo, formatoPersonasVinculadasActual[i].Identificacion.GlosaTipo);
                    }

                    if (formatoPersonasVinculadasPrevio[i].Identificacion.Numero != formatoPersonasVinculadasActual[i].Identificacion.Numero)
                    {
                        html += CrearFilaComparativa(string.Format("Número de Identificación (P.V. {0})", formatoPersonasVinculadasPrevio[i].Item), formatoPersonasVinculadasPrevio[i].Identificacion.Numero, formatoPersonasVinculadasActual[i].Identificacion.Numero);
                    }
                }
            }
            else
            {
                html += CrearFilaComparativa("Cantidad de Personas Vinculadas", formatoPersonasVinculadasPrevio.Count, formatoPersonasVinculadasActual.Count);
            }

            html += "</tbody>";
            html += "</table>";
            return html;
        }
    }
}
