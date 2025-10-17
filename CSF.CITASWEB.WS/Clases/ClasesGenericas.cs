using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSF.CITASWEB.WS.BE;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.IO;
//using CSF.CITASWEB.WS.DA;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Web.Script.Serialization;

namespace CSF.CITASWEB.WS
{
    public static class ClasesGenericas
    {
        public static string ObtenerMD5(string texto)
        {
            using (MD5 hashMD5 = MD5.Create())
            {
                byte[] varDatos = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder varBuilder = new StringBuilder();
                for (int i = 0; i < varDatos.Length; i++)
                {
                    varBuilder.Append(varDatos[i].ToString("x2"));
                }
                return varBuilder.ToString();
            }
        }

        public static string TextoAutogenerado(int tamaño = 6)
        {
            Random varRandom = new Random();
            const string varCaracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(varCaracteres, tamaño)
              .Select(s => s[varRandom.Next(s.Length)]).ToArray());
        }

        public static string NumeroAutogenerado(int tamaño = 6)
        {
            Random varRandom = new Random();
            const string varCaracteres = "0123456789";
            return new string(Enumerable.Repeat(varCaracteres, tamaño)
              .Select(s => s[varRandom.Next(s.Length)]).ToArray());
        }

        public static bool EnviarCorreo(string para, string plantilla, Dictionary<string, string> parametros, string rutaAdjunto = null, string tagDe = "SMTP", string correoCopia = "", char correoCopiaSep = ';', string correoCopiaOculta = "", char correoCopiaOcultaSep = ';', string archivoAdjunto = "", string nombreArchivoAdjunto = "")
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            try
            {
                MailAddress smtpDe = new MailAddress(ConfigurationManager.AppSettings[tagDe + "Usuario"].ToString(), ConfigurationManager.AppSettings[tagDe + "UsuarioAlias"].ToString());
                string smtpPassword = ConfigurationManager.AppSettings[tagDe + "Password"].ToString();

                SmtpClient smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings[tagDe + "Servidor"].ToString(),
                    Port = int.Parse(ConfigurationManager.AppSettings[tagDe + "Puerto"].ToString()),
                    EnableSsl = (ConfigurationManager.AppSettings[tagDe + "Ssl"].ToString().Equals("1")?true:false) ,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpDe.Address, smtpPassword)
                };
                using (MailMessage message = new MailMessage())
                {
                    message.Subject = FormatearHeader(plantilla, parametros);
                    message.Body = FormatearBody(plantilla, parametros);
                    message.IsBodyHtml = true;

                    message.From = smtpDe;
                    Debug.WriteLine(para);
                    foreach (string correo in para.Split(';'))
                    {
                        
                        if (!string.IsNullOrEmpty(correo))
                        {
                            Debug.WriteLine(correo);
                            message.To.Add(new MailAddress(correo));
                        }
                        
                    }
                    String[] aCorreo;
                    int i, nCorreos;
                    if (!String.IsNullOrEmpty(correoCopia))
                    {
                        aCorreo = correoCopiaOculta.Split(correoCopiaSep);
                        nCorreos = aCorreo.Length;
                        for (i = 0; i < nCorreos; i++)
                        {
                            message.CC.Add(new MailAddress(aCorreo[i]));
                        }
                    }
                    if (!String.IsNullOrEmpty(correoCopiaOculta))
                    {
                        aCorreo = correoCopiaOculta.Split(correoCopiaOcultaSep);
                        nCorreos = aCorreo.Length;
                        for (i = 0; i < nCorreos; i++)
                        {
                            message.Bcc.Add(new MailAddress(aCorreo[i]));
                        }
                    }
                    if (!string.IsNullOrEmpty(rutaAdjunto))
                        message.Attachments.Add(new Attachment(rutaAdjunto));

                    MemoryStream ms = null;
                    if (!String.IsNullOrEmpty(archivoAdjunto) && !String.IsNullOrEmpty(nombreArchivoAdjunto)) 
                    {
                        byte[] buffer = Convert.FromBase64String(archivoAdjunto);
                        ms = new MemoryStream(buffer, 0, buffer.Length);
                        message.Attachments.Add(new Attachment(ms, nombreArchivoAdjunto));
                    }
                    smtp.Send(message);
                    if (ms != null) {
                        ms.Close();
                        ms.Flush();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                RegistrarErrorIntranet(ex, "WS", "EnviarEmail - "+para);
                return false;
            }
        }

        private static string FormatearHeader(string plantilla, Dictionary<string, string> parametros)
        {
            string textoPlantilla = ConfigurationManager.AppSettings[plantilla + "_Titulo"].ToString();
            foreach (KeyValuePair<string, string> parametro in parametros)
            {
                textoPlantilla = textoPlantilla.Replace("{" + parametro.Key + "}", parametro.Value);
            }
            return textoPlantilla.Replace("\\n", ". ");
        }

        private static string FormatearBody(string plantilla, Dictionary<string, string> parametros)
        {
            string textoPlantilla = ConfigurationManager.AppSettings[plantilla + "_Cuerpo"].ToString();
            foreach (KeyValuePair<string, string> parametro in parametros)
            {
                textoPlantilla = textoPlantilla.Replace("{" + parametro.Key + "}", parametro.Value);
            }
            return textoPlantilla.Replace("\\n", "<br/>");
        }

        public static string GenerarPeticionMedicoDomicilio(string numeroDocumento, string texto)
        {
            string rutaArchivo = ConfigurationManager.AppSettings["RutaMedicoFavorito"].ToString() + "medicoDomicilio_" + numeroDocumento + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmssfff") + ".txt";
            using (StreamWriter varEscritor = new StreamWriter(rutaArchivo))
            {
                try
                {
                    varEscritor.WriteLine(texto);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    varEscritor.Close();
                    varEscritor.Dispose();
                }
            }
            return rutaArchivo;
        }

        public static bool EnviarCorreoPlantillaHTML(string para, string asunto, string plantillaHTML,  string rutaAdjunto = null, string tagDe = "SMTP", string correoCopia = "", char correoCopiaSep = ';', string correoCopiaOculta = "", char correoCopiaOcultaSep = ';', string archivoAdjunto = "", string nombreArchivoAdjunto = "")
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            try
            {
                MailAddress smtpDe = new MailAddress(ConfigurationManager.AppSettings[tagDe + "Usuario"].ToString(), ConfigurationManager.AppSettings[tagDe + "UsuarioAlias"].ToString());
                string smtpPassword = ConfigurationManager.AppSettings[tagDe + "Password"].ToString();

                SmtpClient smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings[tagDe + "Servidor"].ToString(),
                    Port = int.Parse(ConfigurationManager.AppSettings[tagDe + "Puerto"].ToString()),
                    EnableSsl = (ConfigurationManager.AppSettings[tagDe + "Ssl"].ToString().Equals("1") ? true : false),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpDe.Address, smtpPassword)
                };
                using (MailMessage message = new MailMessage())
                {
                    message.Subject = asunto;
                    message.Body = plantillaHTML;
                    message.IsBodyHtml = true;

                    message.From = smtpDe;
                    Debug.WriteLine(para);
                    foreach (string correo in para.Split(';'))
                    {

                        if (!string.IsNullOrEmpty(correo))
                        {
                            Debug.WriteLine(correo);
                            message.To.Add(new MailAddress(correo));
                        }

                    }
                    String[] aCorreo;
                    int i, nCorreos;
                    if (!String.IsNullOrEmpty(correoCopia))
                    {
                        aCorreo = correoCopia.Split(correoCopiaSep);
                        nCorreos = aCorreo.Length;
                        for (i = 0; i < nCorreos; i++)
                        {
                            message.CC.Add(new MailAddress(aCorreo[i]));
                        }
                    }
                    if (!String.IsNullOrEmpty(correoCopiaOculta))
                    {
                        aCorreo = correoCopiaOculta.Split(correoCopiaOcultaSep);
                        nCorreos = aCorreo.Length;
                        for (i = 0; i < nCorreos; i++)
                        {
                            message.Bcc.Add(new MailAddress(aCorreo[i]));
                        }
                    }
                    if (!string.IsNullOrEmpty(rutaAdjunto))
                        message.Attachments.Add(new Attachment(rutaAdjunto));

                    MemoryStream ms = null;
                    if (!String.IsNullOrEmpty(archivoAdjunto) && !String.IsNullOrEmpty(nombreArchivoAdjunto))
                    {
                        byte[] buffer = Convert.FromBase64String(archivoAdjunto);
                        ms = new MemoryStream(buffer, 0, buffer.Length);
                        message.Attachments.Add(new Attachment(ms, nombreArchivoAdjunto));
                    }
                    smtp.Send(message);
                    if (ms != null)
                    {
                        ms.Close();
                        ms.Flush();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                RegistrarErrorIntranet(ex, "WS", "EnviarEmail - " + para);
                return false;
            }
        }

        public static string ObtenerHexadecimal(string texto)
        {
            byte[] ba = Encoding.Default.GetBytes(texto);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static string CifrarSHA256Hex(string mensaje)
        {
            string rpta = "";
            SHA256Managed sha = new SHA256Managed(); //32 bytes
            byte[] buffer = Encoding.Default.GetBytes(mensaje);
            byte[] hash = sha.ComputeHash(buffer);
            //Convertir cada byte en hexadecimal 32 x 2 = 64 bytes
            rpta = BitConverter.ToString(hash).Replace("-", "");
            return rpta.ToLower();
        }

        public static string GetHeader(WebHeaderCollection aHeader, string nombreHeader)
        {
            string valor = aHeader[nombreHeader];
            valor = !string.IsNullOrEmpty(valor) ? valor : "";
            return valor;
        }

        public static string HexToBase64(string strInput)
        {
            try
            {
                var bytes = new byte[strInput.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(strInput.Substring(i * 2, 2), 16);
                }
                return Convert.ToBase64String(bytes);
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        public static string Base64ToHex(string strInput)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(strInput);
                string hex = BitConverter.ToString(bytes);
                return hex.Replace("-", "");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.ToString();
            }
            return "";
        }

        public static string DecryptStringAES(string cipherText, string key)
        {
            string decriptedFromJavascript = null;
            try
            {
                //key debe ser de 16 caracteres
                var keybytes = Encoding.UTF8.GetBytes(key);
                var iv = Encoding.UTF8.GetBytes(key);

                var encrypted = Convert.FromBase64String(cipherText);
                decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            }
            catch (Exception ex)
            {
                decriptedFromJavascript = "";
            }
            return string.Format(decriptedFromJavascript);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    plaintext = "";
                }
            }
            return plaintext;
        }

        public static string ObtenerFormatoFecha(DateTime oFecha)
        {
            string valor = "";
            if (oFecha != null)
            {
                string[] aSemana = new string[] {
                    "lunes", "martes", "miércoles",
                    "jueves", "viernes", "sábado",
                    "domingo"
                };
                string[] aMes = new string[] {
                    "enero", "febrero", "marzo",
                    "abril", "mayo", "junio",
                    "julio", "agosto", "setiembre",
                    "octubre", "noviembre", "diciembre"
                };
                string fechaHora = oFecha.ToString("dd/MM/yyyy HH:mm");
                string[] aFechaHora = fechaHora.Split(' ');
                string[] aFecha = aFechaHora[0].Split('/');
                //string[] aHora = aFechaHora[0].Split(':');
                int diaSemana = (int)oFecha.DayOfWeek;
                diaSemana = diaSemana == 0 ? 7 : diaSemana;
                valor = aSemana[diaSemana - 1].ToString();
                valor += " " + aFecha[0] + " de ";
                valor += aMes[((int)oFecha.Month) - 1];
            }
            return valor;
        }

        public static string ObtenerFormatoHora(string strHora)
        {
            string valor = "";
            if (!String.IsNullOrEmpty(strHora))
            {
                string[] aHora = strHora.Split(':');
                int hora = int.Parse(aHora[0]);
                string sufijo = hora >= 12 ? "pm" : "am";
                if (hora >= 13)
                {
                    hora = hora - 12;
                }
                valor = hora.ToString().PadLeft(2, '0') + ":" + aHora[1] + " " + sufijo;
            }
            return valor;
        }

        public static string GetCapitalize(string texto, bool soloPrimeraPalabra = false)
        {
            List<String> lstPalabra = new List<String>();
            if (!String.IsNullOrWhiteSpace(texto))
            {
                texto = texto.Trim().ToLower();
                string[] lPalabra = texto.Split(' ');
                int i = 0, nPalabras = lPalabra.Length;
                string primerCaracter, palabraTmp;
                for (; i < nPalabras; i++)
                {
                    primerCaracter = lPalabra[i].Substring(0, 1).ToUpper();
                    palabraTmp = primerCaracter;
                    if (lPalabra[i].Length > 1)
                    {
                        palabraTmp += lPalabra[i].Substring(1, lPalabra[i].Length - 1);
                    }
                    lstPalabra.Add(palabraTmp);
                    if (soloPrimeraPalabra)
                    {
                        break;
                    }
                }
            }
            return String.Join(" ", lstPalabra.ToArray());
        }

        public static string PostAsyncIntranet(string url, string data = "", string credencial = "", bool validarCertificado = true)
        {
            string response = "";
            try
            {
                string urlBase = GetSetting("WS_Intranet");
                using (HttpClient oCliente = new HttpClient())
                {
                    oCliente.BaseAddress = new Uri(urlBase);
                    oCliente.Timeout = new TimeSpan(0, 2, 0);
                    if (!validarCertificado)
                    {
                        ServicePointManager.ServerCertificateValidationCallback = (object se, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslerror) => true;
                    }
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    oCliente.DefaultRequestHeaders.Accept.Clear();
                    oCliente.DefaultRequestHeaders.Add("User-Agent", "CSF_CITAS_WEB_WS/" + GetSetting("_VersionApp"));
                    oCliente.DefaultRequestHeaders.Add("Accept", "*/*");
                    oCliente.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    oCliente.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                    if (!string.IsNullOrEmpty(credencial))
                    {
                        oCliente.DefaultRequestHeaders.Add("token", credencial);
                    }
                    var responseTask = oCliente.PostAsync(
                        url,
                        new StringContent(data, Encoding.UTF8, "application/json")
                    );
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    //if (result.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        response = readTask.Result;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        /*public static string InvocarServicioRest(string metodo, string dataPost)
        {
            var request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["URLServiciosREST"].ToString() + "/" + metodo);

            request.Method = "POST";
            request.ContentLength = 0;
            request.ContentType = "application/json";

            var encoding = new UTF8Encoding();
            var bytes = Encoding.UTF8.GetBytes(dataPost);
            request.ContentLength = bytes.Length;

            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }

        public static string InvocarServicioRestTekton(string url, string dataPost)
        {
            StreamWriter varEscritor = null;
            try
            {
                string usuarioTekton = ConfigurationManager.AppSettings["usuarioTekton"].ToString();
                string passwordTekton = ConfigurationManager.AppSettings["passwordTekton"].ToString();

                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(usuarioTekton + ":" + passwordTekton));

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["rutaLogSpring"]))
                {
                    varEscritor = new StreamWriter(Path.Combine(ConfigurationManager.AppSettings["rutaLogSpring"].ToString(),"debugSpring_" + DateTime.Today.ToString("yyyyMMdd") + ".txt"));
                    varEscritor.WriteLine("--" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "--");
                    varEscritor.WriteLine("URL: " + url);
                    varEscritor.WriteLine("Request:\r\n" + dataPost);
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("Authorization", "Basic " + encoded);

                request.Method = "POST";
                request.ContentLength = 0;
                request.ContentType = "application/json";

                var encoding = new UTF8Encoding();
                var bytes = Encoding.UTF8.GetBytes(dataPost);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["rutaLogSpring"]))
                        {
                            varEscritor.WriteLine("Response:\r\n" + message);
                        }
                        throw new ApplicationException(message);
                    }

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }

                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["rutaLogSpring"]))
                    {
                        varEscritor.WriteLine("Response:\r\n" + responseValue + "\r\n\r\n");
                    }

                    return responseValue;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["rutaLogSpring"]))
                {
                    varEscritor.WriteLine("Response (catch):\r\n" + ex.Message + "\r\n\r\n");
                }
                throw;
            }
            finally
            {
                if (varEscritor != null)
                {
                    varEscritor.Flush();
                    varEscritor.Close();
                }
            }
        }*/

        public static RespuestaSimpleBE RegistrarErrorIntranet(Exception ex, string origen, string servicio)
        {
            if (ex.Message.StartsWith("INFO:"))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 0,
                    mensaje = ex.Message.Replace("INFO:", ""),
                    data = null
                };
            }
            else if (ex.Message.StartsWith("ERRFU:"))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 1,
                    mensaje = ex.Message.Replace("ERRFU:", ""),
                    data = null
                };
            }
            else
            {
                //int varIDError = 0;
                //ConexionUtil varConexion = new ConexionUtil();
                try
                {
                    string request = "";
                    try
                    {
                        request = OperationContext.Current.RequestContext.RequestMessage.ToString();
                    }
                    catch (Exception)
                    {
                    }
                    Exception varErrorDetallado = ex;
                    string mensaje = ex.Message;
                    string stackTrace = ex.StackTrace;
                    while (varErrorDetallado.InnerException != null)
                    {
                        mensaje = mensaje + "\r\n**\r\n" + varErrorDetallado.InnerException.Message;
                        stackTrace = stackTrace + "\r\n**\r\n" + varErrorDetallado.InnerException.StackTrace;
                        varErrorDetallado = varErrorDetallado.InnerException;
                    }

                    var oRequest = new
                    {
                        mensaje = string.IsNullOrEmpty(ex.Message) ? "" : (mensaje.Substring(0, ex.Message.Length > 5000 ? 5000 : mensaje.Length)),
                        ubicacion = string.IsNullOrEmpty(ex.StackTrace) ? "" : (stackTrace.Substring(0, ex.StackTrace.Length > 5000 ? 5000 : stackTrace.Length)),
                        origen = origen,
                        servicio = servicio,
                        request = request
                    };
                    string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                    string response = PostAsyncIntranet("Cita.svc/RegistrarErrorSimple/", strRequest);
                    RespuestaSimpleBE varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
                }
                catch (Exception)
                {
                }
                finally
                {
                    //varConexion.Desconectar();
                }
                return new RespuestaSimpleBE()
                {
                    rpt = 999,
                    mensaje = ex.Message,
                    data = null
                };
            }
        }
        public static RespuestaBE<T> RegistrarErrorIntranet<T>(Exception ex, string origen, string servicio)
        {
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            if (ex.Message.StartsWith("INFO:"))
            {
                return new RespuestaBE<T>()
                {
                    rpt = 0,
                    mensaje = ex.Message.Replace("INFO:", ""),
                    data = default(T)
                };
            }
            else if (ex.Message.StartsWith("ERRFU:"))
            {
                return new RespuestaBE<T>()
                {
                    rpt = 1,
                    mensaje = ex.Message.Replace("ERRFU:", ""),
                    data = default(T)
                };
            }
            else
            {
                //int varIDError = 0;
                //ConexionUtil varConexion = new ConexionUtil();
                try
                {
                    string request = "";
                    try
                    {
                        request = OperationContext.Current.RequestContext.RequestMessage.ToString();
                    }
                    catch (Exception)
                    {
                    }
                    
                    var oRequest = new
                    {
                        mensaje = string.IsNullOrEmpty(ex.Message) ? "" : (ex.Message).Substring(0, ex.Message.Length > 5000 ? 5000 : ex.Message.Length),
                        ubicacion = string.IsNullOrEmpty(ex.StackTrace) ? "" : ex.StackTrace.Substring(0, ex.StackTrace.Length > 5000 ? 5000 : ex.StackTrace.Length),
                        origen = origen,
                        servicio = servicio,
                        request = request
                    };
                    string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                    string response = PostAsyncIntranet("Cita.svc/RegistrarError/", strRequest);
                    varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
                }
                catch (Exception)
                {
                }
                finally
                {
                    //varConexion.Desconectar();
                }
                return new RespuestaBE<T>()
                {
                    rpt = 999,
                    mensaje = varRespuesta.mensaje,//ex.Message,
                    data = default(T)
                };
            }
        }
    }
}