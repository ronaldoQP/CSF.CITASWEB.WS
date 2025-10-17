using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSF.CITASWEB.WS.BE;
using System.ServiceModel.Activation;
//using CSF.CITASWEB.WS.DA;
using System.ServiceModel.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using Newtonsoft.Json;
using System.ServiceModel.Channels;
using System.Web.Script.Serialization;
using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace CSF.CITASWEB.WS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Usuario : IUsuario
    {
        public Usuario()
        {
            Env.Load(ClasesGenericas.GetSetting("RutaEnv"));
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
        public RespuestaSimpleBE ActualizarTokenPush(string tipoDocumento, string numeroDocumento, string tokenPush)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(tokenPush))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 101,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            //if (tokenPush.Length > 500)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro tokenPush no puede tener más de 500 caracteres",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    new UsuarioDA().ActualizarTokenPush(tipoDocumento, numeroDocumento, tokenPush);
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = null
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaBE<AutenticacionBE> Autenticar(string tipoDocumento, string numeroDocumento, string password,
                                                        string tipoDispositivo, string imei, string tokenPush, string tokenPushHms)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(tipoDispositivo)
                || string.IsNullOrEmpty(imei) || (string.IsNullOrEmpty(tokenPush) && string.IsNullOrEmpty(tokenPushHms)))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            password = password.Trim();
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDispositivo.ToLower() != "android" && tipoDispositivo.ToLower() != "ios" && tipoDispositivo.ToLower() != "huawei")
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"android\", \"iOS\" o \"huawei\" en el parámetro tipoDispositivo",
                    data = null
                };
            }
            //2017-12-09 - Corrección de campo numeroDocumento por imei
            if (imei.Length > 50)
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 104,
                    mensaje = "El parámetro imei no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (tokenPush.Length > 500)
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 105,
                    mensaje = "El parámetro tokenPush no puede tener más de 500 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<AutenticacionBE> varRespuesta = new RespuestaBE<AutenticacionBE>();
            try
            {
                if (password.Length == 32)
                {
                    //MD5(Hex())
                    //32 caracteres es MD5
                    string sltPwd = ConfigurationManager.AppSettings["SltPwd"].ToString();
                    password = ClasesGenericas.CifrarSHA256Hex(tipoDocumento + numeroDocumento + sltPwd + password);
                }
                string userAgent = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "User-Agent");//PostmanRuntime/7.29.2
                userAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : "";
                string ipCliente = GetClientAddress();

                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    password = password,
                    tipoDispositivo = tipoDispositivo,
                    imei = imei,
                    tokenPush = tokenPush,
                    tokenPushHms = tokenPushHms,
                    userAgent = userAgent,
                    ipCliente = ipCliente
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/Autenticar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<AutenticacionBE>>(response);
                if (varRespuesta == null)
                {
                    return new RespuestaBE<AutenticacionBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("HTTP-400:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.BadRequest);
                //}
                //else if (ex.Message.StartsWith("HTTP-401:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.Unauthorized);
                //}
                //else
                //{
                //return new ErrorDA().RegistrarError<AutenticacionBE>(ex, "WS", "Usuario.svc/Autenticar");
                //}
            }
            return varRespuesta;
        }
        public RespuestaBE<AutenticacionBE> AutenticarWeb(string tipoDocumento, string numeroDocumento, string password)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(password))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            password = password.Trim();
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<AutenticacionBE>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<AutenticacionBE> varRespuesta = new RespuestaBE<AutenticacionBE>();
            try
            {
                if (password.Length == 32)
                {
                    //MD5(Hex())
                    //32 caracteres es MD5
                    string sltPwd = ConfigurationManager.AppSettings["SltPwd"].ToString();
                    password = ClasesGenericas.CifrarSHA256Hex(tipoDocumento + numeroDocumento + sltPwd + password);
                }
                string userAgent = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "User-Agent");//PostmanRuntime/7.29.2
                userAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : "";
                string ipCliente = GetClientAddress();
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    password = password,
                    userAgent = userAgent,
                    ipCliente = ipCliente
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/AutenticarWeb/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<AutenticacionBE>>(response);
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("HTTP-400:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.BadRequest);
                //}
                //else if (ex.Message.StartsWith("HTTP-401:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.Unauthorized);
                //}
                //else
                //{
                //    return new ErrorDA().RegistrarError<AutenticacionBE>(ex, "WS", "Usuario.svc/AutenticarWeb");
                //}
            }
            return varRespuesta;
        }
        public RespuestaBE<UsuarioBE> ValidarExistenciaUsuario(string data)//string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            #endregion
            RespuestaBE<UsuarioBE> varRespuesta = new RespuestaBE<UsuarioBE>();
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                data = ClasesGenericas.DecryptStringAES(data, ClasesGenericas.GetSetting("KeyAES"));
                if (String.IsNullOrEmpty(data)) 
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                String[] aData = data.Split('|');//(new Date()).getTime()|yyyy-MM-dd HH:mm:ss.fff|tipoDocumento|SltPwd|navigator.userAgent|numeroDocumento
                if (aData.Length != 6)
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                //DateTime oFechaHoraActual = DateTime.Now.AddSeconds(-5); //El tipo y número de documento (AES) en consulta tendrán una vigencia de 5 segundos como máximo
                string tiempoVigenciaSolicitudAES = ClasesGenericas.GetSetting("VigenciaSolicitudAES");
                double varTiempoVigenciaSolicitudAES;
                bool esTiempoValido = double.TryParse(tiempoVigenciaSolicitudAES, out varTiempoVigenciaSolicitudAES);
                varTiempoVigenciaSolicitudAES = esTiempoValido ? varTiempoVigenciaSolicitudAES : 5; //La consulta tendrá una vigencia del parámetro configurado, donde por defecto es 5 segundos como máximo
                //DateTime oFechaHoraActual = DateTime.Now.AddSeconds(-5); //El tipo y número de documento (AES) en consulta tendrán una vigencia de 5 segundos como máximo
                DateTime oFechaHoraActual = DateTime.Now.AddSeconds(-varTiempoVigenciaSolicitudAES);
                DateTime oFechaHoraConsulta;
                if (DateTime.TryParse(aData[1], out oFechaHoraConsulta)) 
                {
                    if (oFechaHoraConsulta <= oFechaHoraActual)
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                } 
                else
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                string tipoDocumento = aData[2], 
                       numeroDocumento = aData[4];
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarExistenciaUsuario/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioBE>>(response);
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("INFO:"))
                //{
                //    //INFO:El usuario no existe
                //    return new RespuestaBE<UsuarioBE>()
                //    {
                //        rpt = 0,
                //        mensaje = "Validación realizada"
                //    };
                //}
                //else if (ex.Message.StartsWith("ERRFU:"))
                //{
                //    //ERRFU:El tipo y número de documento ya tiene un usuario.
                //    return new RespuestaBE<UsuarioBE>()
                //    {
                //        rpt = 1,
                //        mensaje = "Validación realizada"
                //    };
                //}
                //else
                //{
                //    return new ErrorDA().RegistrarError<UsuarioBE>(ex, "WS", "Usuario.svc/ValidarExistenciaUsuario");
                //}
            }
            return varRespuesta;
        }
        public RespuestaBE<UsuarioBE> ValidarExistenciaUsuarioPaciente(string data)
        {
            #region Validacion de Parámetros
            #endregion
            RespuestaBE<UsuarioBE> varRespuesta = new RespuestaBE<UsuarioBE>();
            try
            {
                //Base: ValidarExistenciaUsuario
                //Objetivo: Devolver datos de un paciente (No usuario)
                WebOperationContext oContext = WebOperationContext.Current;
                if (ClasesGenericas.GetSetting("DescifrarAES").Equals("1"))
                {
                    data = ClasesGenericas.DecryptStringAES(data, ClasesGenericas.GetSetting("KeyAES"));
                }
                if (String.IsNullOrEmpty(data))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                String[] aData = data.Split('|');//(new Date()).getTime()|yyyy-MM-dd HH:mm:ss.fff|tipoDocumento|SltPwd|navigator.userAgent|numeroDocumento
                if (aData.Length != 6)
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                string fechaHoraConsulta = aData[1],
                       tipoDocumento = aData[2],
                       numeroDocumento = aData[4];

                string tiempoVigenciaSolicitudAES = ClasesGenericas.GetSetting("VigenciaSolicitudAES");
                double varTiempoVigenciaSolicitudAES;
                bool esTiempoValido = double.TryParse(tiempoVigenciaSolicitudAES, out varTiempoVigenciaSolicitudAES);
                varTiempoVigenciaSolicitudAES = esTiempoValido ? varTiempoVigenciaSolicitudAES : 5; //La consulta tendrá una vigencia del parámetro configurado, donde por defecto es 5 segundos como máximo
                DateTime oFechaHoraActual = DateTime.Now.AddSeconds(-varTiempoVigenciaSolicitudAES); //El tipo y número de documento (AES) en consulta tendrán una vigencia de 5 segundos como máximo
                DateTime oFechaHoraConsulta;
                if (DateTime.TryParse(fechaHoraConsulta, out oFechaHoraConsulta))
                {
                    if (oFechaHoraConsulta <= oFechaHoraActual)
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                }
                else
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarExistenciaUsuarioPaciente/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioBE>>(response);
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("INFO:"))
                //{
                //    //INFO:El usuario no existe
                //    return new RespuestaBE<UsuarioBE>()
                //    {
                //        rpt = 0,
                //        mensaje = "Validación realizada"
                //    };
                //}
                //else if (ex.Message.StartsWith("ERRFU:"))
                //{
                //    //ERRFU:El tipo y número de documento ya tiene un usuario.
                //    return new RespuestaBE<UsuarioBE>()
                //    {
                //        rpt = 1,
                //        mensaje = "Validación realizada"
                //    };
                //}
                //else
                //{
                //    return new ErrorDA().RegistrarError<UsuarioBE>(ex, "WS", "Usuario.svc/ValidarExistenciaUsuarioPaciente");
                //}
            }
            return varRespuesta;
        }
        public RespuestaBE<UsuarioBE> ValidarExistenciaPaciente(string tipoDocumento, string numeroDocumento)
        {
            RespuestaBE<UsuarioBE> varRespuesta = new RespuestaBE<UsuarioBE>();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarExistenciaPaciente/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<UsuarioBE>(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE RecuperarPassword(string tipoDocumento, string numeroDocumento, string metodo, string correo, string password,
            string codigoOTP, string tipoOTP, string origen)
        {
            #region Validacion de Parámetros
            metodo = "m";
            if (string.IsNullOrWhiteSpace(tipoDocumento) || string.IsNullOrWhiteSpace(numeroDocumento) || string.IsNullOrWhiteSpace(correo)
                 || string.IsNullOrWhiteSpace(password))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            password = password.Trim();
            bool isEmail = Regex.IsMatch(correo, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 100,
                    mensaje = "Correo inválido",
                    data = null
                };
            }
            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                string claveGenerada = "";
                bool esClaveGenerada = false;
                tipoOTP = !string.IsNullOrWhiteSpace(tipoOTP) ? tipoOTP.Trim() : "generico"; //tipo de OTP: recuperar-contrasenia, cambiar-contrasenia, politica-contrasenia o generico
                if (tipoOTP != "recuperar-contrasenia" && tipoOTP != "cambiar-contrasenia" && tipoOTP != "politica-contrasenia" && tipoOTP != "generico")
                {
                    throw new WebFaultException(HttpStatusCode.BadRequest);
                }
                
                if (metodo == "m")
                {
                    if (password.Length == 32)
                    {
                        //MD5(Hex())
                        //32 caracteres es MD5
                        string sltPwd = ConfigurationManager.AppSettings["SltPwd"].ToString();
                        password = ClasesGenericas.CifrarSHA256Hex(tipoDocumento + numeroDocumento + sltPwd + password);
                    }
                    var oRequest = new
                    {
                        tipoDocumento = tipoDocumento,
                        numeroDocumento = numeroDocumento,
                        password = password,
                        metodo = metodo,
                        correo = correo,
                        codigoOTP = codigoOTP,
                        tipoOTP = tipoOTP,
                        origen = origen
                    };
                    string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                    string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/RecuperarPassword/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                    varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
                    string varEmail;
                    if (varRespuesta.rpt == 0 && !string.IsNullOrEmpty(varRespuesta.data)) 
                    {
                        varEmail = varRespuesta.data;
                        string[] aUsuario = varEmail.Split('|');
                        string nombreUsuario = aUsuario[1],
                            correoUsuario = aUsuario[0];

                        Dictionary<string, string> varParametros = new Dictionary<string, string>();
                        varParametros.Add("nombre", nombreUsuario);
                        varParametros.Add("numeroDocumento", numeroDocumento);
                        //if (esClaveGenerada)
                        //{
                        //    varParametros.Add("contraseña", claveGenerada);
                        //    varParametros.Add("estiloClave", "");
                        //}
                        //else
                        //{
                        //    varParametros.Add("contraseña", password);
                        //    varParametros.Add("estiloClave", "display:none;");
                        //}
                        bool statusEnvioEmail = ClasesGenericas.EnviarCorreo(correoUsuario, "Usuario_RecordarPassword", varParametros);
                        if (statusEnvioEmail)
                        {
                            return new RespuestaSimpleBE()
                            {
                                rpt = 0,
                                mensaje = "Mensaje enviado",
                                data = varEmail.Split('|')[0]
                            };
                        }
                        else
                        {
                            return new RespuestaSimpleBE()
                            {
                                rpt = 103,
                                mensaje = "Alerta, no se pudo enviar el mensaje",
                                data = varEmail.Split('|')[0]
                            };
                        }
                    }
                }
                else
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 104,
                        mensaje = "No existe método configurado",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("HTTP-400:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.BadRequest);
                //}
                //else if (ex.Message.StartsWith("HTTP-401:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.Unauthorized);
                //}
                //else
                //{
                //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc/RecuperarPassword");
                //}
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE RegistrarUsuario(string tipoDocumento, string numeroDocumento, string nombres,
                                            string apellidoPaterno, string apellidoMaterno, string genero, string fechaNacimiento,
                                            string email, string celular, string seguroPlanSalud, string password, string finalidadesAdiciona,
                                            string direccion = "", string numero = "", string departamento = "", string referencia = "",
                                            string latlong = "", string origen = "", string nombreArchivo = "", string archivo = "",
                                            string telefono = "", string tipoPaciente = "", string observacion = "",
                                            string idPais = "", string idDepartamento = "", string idProvincia = "", string idDistrito = "",
                                            string idPaisNac = "", string idDepartamentoNac = "", string idProvinciaNac = "", string idDistritoNac = "",
                                            bool esIntranet = false, string codigoOTP = "", string tipoOTP = "", string finesAdicionales = "")
        {
            if (string.IsNullOrEmpty(genero))
                genero = "N";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidoPaterno) || string.IsNullOrEmpty(fechaNacimiento))
                //|| string.IsNullOrWhiteSpace(password))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            //password = password.Trim();
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 104,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            nombres = nombres.Trim();
            if (nombres.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 105,
                    mensaje = "El parámetro nombres no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(nombres))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 106,
                    mensaje = "El parámetro nombres solo debe contener letras.",
                    data = null
                };
            }
            apellidoPaterno = apellidoPaterno.Trim();
            if (apellidoPaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "El parámetro apellidoPaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(apellidoPaterno))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 108,
                    mensaje = "El parámetro apellidoPaterno solo debe contener letras.",
                    data = null
                };
            }
            apellidoMaterno = apellidoMaterno.Trim();
            if (apellidoMaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 109,
                    mensaje = "El parámetro apellidoMaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!string.IsNullOrEmpty(apellidoMaterno))
            {
                if (!fnValidarSoloTexto(apellidoMaterno))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 110,
                        mensaje = "El parámetro apellidoMaterno solo debe contener letras.",
                        data = null
                    };
                }
            }
            if (genero != "N" && genero != "F" && genero != "M")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 111,
                    mensaje = "El parámetro genero sólo acepta N, F o M",
                    data = null
                };
            }
            //DateTime varFecha;
            //if (!DateTime.TryParse(fechaNacimiento, out varFecha))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 107,
            //        mensaje = "El parámetro fechaNacimiento no tiene el formato correcto",
            //        data = null
            //    };
            //}
            if (email.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 112,
                    mensaje = "El parámetro email no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (celular.Length > 9)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 113,
                    mensaje = "El parámetro celular no puede tener más de 9 caracteres",
                    data = null
                };
            }
            //if (!string.IsNullOrEmpty(seguroPlanSalud) && seguroPlanSalud.Length != 11)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 110,
            //        mensaje = "El parámetro seguroPlanSalud debe tener 11 caracteres",
            //        data = null
            //    };
            //}
            if (string.IsNullOrEmpty(finalidadesAdiciona))
                finalidadesAdiciona = "true";

            bool varBool;
            if (!bool.TryParse(finalidadesAdiciona, out varBool))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 114,
                    mensaje = "El parámetro finalidadesAdiciona debe ser \"true\"(1) o \"false\"(0)",
                    data = null
                };
            }
            bool varBoolFinesAdicionales;
            if (!bool.TryParse(finesAdicionales, out varBoolFinesAdicionales))
            {
                finesAdicionales = null;
                //return new RespuestaSimpleBE()
                //{
                //    rpt = 115,
                //    mensaje = "El parámetro finesAdicionales debe ser \"true\" o \"false\"",
                //    data = null
                //};
            }
            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                //Cambio para permitir agregar el password a la creación de clientes
                string claveGenerada = "";
                bool esClaveGenerada = false;
                if (!esIntranet)
                {
                    WebOperationContext oContext = WebOperationContext.Current;
                    password = !String.IsNullOrEmpty(password) ? password.Trim() : "";
                    if (string.IsNullOrEmpty(password))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                    tipoOTP = !string.IsNullOrWhiteSpace(tipoOTP) ? tipoOTP.Trim() : "generico"; //tipo de OTP: registro o generico
                    if (tipoOTP != "registro" && tipoOTP != "generico")
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                    if (password.Length == 32)
                    {
                        //MD5(Hex())
                        //32 caracteres es MD5
                        string sltPwd = ConfigurationManager.AppSettings["SltPwd"].ToString();
                        password = ClasesGenericas.CifrarSHA256Hex(tipoDocumento + numeroDocumento + sltPwd + password);
                    }
                }
                else
                {
                    password = "";
                }

                string nombreGenerado = "";
                if (!string.IsNullOrEmpty(nombreArchivo) && !string.IsNullOrEmpty(archivo))
                {
                    string[] aNom = nombreArchivo.Split('.');
                    byte[] buffer = Convert.FromBase64String(archivo);
                    string ruta = ConfigurationManager.AppSettings["rutaUsuarioImagen"];
                    nombreGenerado = DateTime.Now.ToString("ddMMyyyHHmmss") + "." + aNom[aNom.Length - 1];
                    string rutaGenerada = Path.Combine(ruta, nombreGenerado);
                    File.WriteAllBytes(rutaGenerada, buffer);
                }

                var oRequest = new
                {
                    tipoDocumento = tipoDocumento, numeroDocumento = numeroDocumento, nombres = nombres,
                    apellidoPaterno = apellidoPaterno, apellidoMaterno = apellidoMaterno, fechaNacimiento = fechaNacimiento,
                    genero = genero, email = email, celular = celular,
                    seguroPlanSalud = seguroPlanSalud, password = password, finalidadesAdiciona = finalidadesAdiciona,
                    direccion = direccion, numero = numero, departamento = departamento,
                    referencia = referencia, latlong = latlong, origen = origen,
                    nombreArchivo = nombreArchivo, nombreGenerado = nombreGenerado, telefono = telefono,
                    tipoPaciente = tipoPaciente, observacion = observacion, idPais = idPais,
                    idDepartamento = idDepartamento, idProvincia = idProvincia, idDistrito = idDistrito, idPaisNac = idPaisNac,
                    idDepartamentoNac = idDepartamentoNac, idProvinciaNac = idProvinciaNac, idDistritoNac = idDistritoNac,
                    codigoOTP = codigoOTP, tipoOTP = tipoOTP, esIntranet = esIntranet,
                    finesAdicionales = finesAdicionales
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/RegistrarUsuario/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<RegistrarUsuarioBE> varRespuesta2 = new JavaScriptSerializer().Deserialize<RespuestaBE<RegistrarUsuarioBE>>(response);

                if (varRespuesta2.rpt == 0)
                {
                    bool statusEnvioEmail = false;
                    if (!esIntranet)
                    {
                        try
                        {
                            Dictionary<string, string> varParametros = new Dictionary<string, string>();
                            varParametros.Add("nombre", nombres.ToUpper());
                            //if (esClaveGenerada)
                            //{
                            //    //{estiloClave}
                            //    varParametros.Add("contraseña", claveGenerada);
                            //    varParametros.Add("estiloClave", "");
                            //}
                            //else
                            //{
                            //    varParametros.Add("contraseña", password);
                            //    varParametros.Add("estiloClave", "display:none;");
                            //}

                            statusEnvioEmail = ClasesGenericas.EnviarCorreo(email, "Usuario_RegistrarUsuario", varParametros);
                        }
                        catch (Exception)
                        {
                        }

                        if (varRespuesta2.data != null)
                        {
                            // Notificación a titular que tenía a la persona (familiar) como pendiente de aprobación
                            // para indicarle que se anuló su solicitud
                            string varRespuestaFcm = "";
                            try
                            {
                                // Notificar al titular
                                string serverKey = GetAccessTokenAsync().Result;
                                string varJson = "{" +
                                            "\"message\": {" +
                                                "\"token\":\"" + varRespuesta2.data.fcmTokenNotificacion + "\"," +
                                                "\"notification\": {" +
                                                    "\"title\": \"" + varRespuesta2.data.tituloNotificacion + "\"," +
                                                    "\"body\": \"" + varRespuesta2.data.textoNotificacion + "\"" +
                                                //"}" + 
                                                "}," +
                                                "\"data\":{" +
                                                    //"\"titulo\":\"" + "Título de prueba" + "\"," +
                                                    //"\"detalle\":\"" + "Detalle de prueba" + "\"," +
                                                    "\"fecha\":\"" + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + "\"" +
                                                "}" +
                                            "}" +
                                        "}";
                                // varRespuesta2.data.textoNotificacion; // Texto para la notificación
                                string urlBaseFCM = Env.GetString("fcm_url_api");
                                HttpWebRequest varWebRequest = (HttpWebRequest)WebRequest.Create(urlBaseFCM);
                                varWebRequest.ContentType = "application/json";

                                string authorization = "Bearer " + serverKey;
                                varWebRequest.Headers["Authorization"] = authorization;
                                varWebRequest.Method = "POST";
                                GrabarLog(string.Format("Request de FCM \r\ntipoDocumento: {0} - numeroDocumento: {1}",
                                        tipoDocumento, numeroDocumento), "WS", "Usuario.svc/RegistrarUsuario",
                                        string.Format("UrlBaseFCM: {0} - Authorization: {1}",
                                        urlBaseFCM, authorization), varJson);

                                using (var streamWriter = new StreamWriter(varWebRequest.GetRequestStream()))
                                {
                                    streamWriter.Write(varJson);
                                    streamWriter.Flush();
                                }

                                using (WebResponse responseFcm = varWebRequest.GetResponse())
                                {
                                    Stream varRespuestaStream = responseFcm.GetResponseStream();
                                    if (varRespuestaStream != null)
                                    {
                                        using (StreamReader varReader = new StreamReader(varRespuestaStream))
                                        {
                                            varRespuestaFcm = varReader.ReadToEnd();
                                            varReader.Close();
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                varRespuestaFcm = ex.Message;
                                ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/RegistrarUsuario");
                            }
                        }
                    }
                    else
                    {
                        statusEnvioEmail = true;
                    }
                    return new RespuestaSimpleBE()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = statusEnvioEmail ? (esIntranet ? "" : null) : "Alerta, no se pudo enviar el email"
                    };
                }
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/RegistrarUsuario");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE RegistrarMedicoFavorito(string tipoDocumento, string numeroDocumento, string cmp, string idEspecialidad)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(cmp) || string.IsNullOrEmpty(idEspecialidad))
            if (string.IsNullOrEmpty(cmp) || string.IsNullOrEmpty(idEspecialidad))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 101,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            if (cmp.Length > 10)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "El parámetro cmp no puede tener más de 10 caracteres",
                    data = null
                };
            }
            int varIDEspecialidad;
            if (!int.TryParse(idEspecialidad, out varIDEspecialidad))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "El parámetro idEspecialidad debe ser numérico",
                    data = null
                };
            }
            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    cmp = cmp,
                    idEspecialidad = idEspecialidad
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/RegistrarMedicoFavorito/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE EliminarMedicoFavorito(string idMedicoFavorito)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(idMedicoFavorito))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIDMedicoFavorito;
            if (!int.TryParse(idMedicoFavorito, out varIDMedicoFavorito))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "El parámetro idMedico debe ser numérico",
                    data = null
                };
            }
            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                var oRequest = new
                {
                    idMedicoFavorito = idMedicoFavorito
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/EliminarMedicoFavorito/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE AgregarFamiliar(string tipoDocumento, string numeroDocumento, string tipoDocumentoTitular,
                                            string numeroDocumentoTitular, string nombres,
                                            string apellidoPaterno, string apellidoMaterno, string genero,
                                            string fechaNacimiento, string email, string celular,
                                            string seguroPlanSalud, string tipoParentesco, string direccion = "", string numeroDireccion = "", string numeroDepartamento = "",
                                            string referencia = "", string latlong = "", string nombreArchivo = "", string archivo = "",
                                            string telefono = "", string tipoPaciente = "", string observacion = "",
                                            string idPais = "", string idDepartamento = "", string idProvincia = "", string idDistrito = "",
                                            string idPaisNac = "", string idDepartamentoNac = "", string idProvinciaNac = "", string idDistritoNac = "",
                                            string finalidadesAdiciona = "", string codEstadoSolicitud = "", string adjunto = "", string nombreAdjunto = "",
                                            string origen = "")
        {
            if (string.IsNullOrEmpty(finalidadesAdiciona))
                finalidadesAdiciona = "true";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(tipoDocumentoTitular) || string.IsNullOrEmpty(numeroDocumentoTitular) ||
                string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidoPaterno) || string.IsNullOrEmpty(fechaNacimiento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(genero))
                genero = "N";
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumentoTitular != "1" && tipoDocumentoTitular != "2" && tipoDocumentoTitular != "3")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumentoTitular.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 104,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 105,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 106,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            nombres = nombres.Trim();
            if (nombres.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "El parámetro nombres no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(nombres))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 108,
                    mensaje = "El parámetro nombres solo debe contener letras.",
                    data = null
                };
            }
            apellidoPaterno = apellidoPaterno.Trim();
            if (apellidoPaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 109,
                    mensaje = "El parámetro apellidoPaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(apellidoPaterno))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 110,
                    mensaje = "El parámetro apellidoPaterno solo debe contener letras.",
                    data = null
                };
            }
            apellidoMaterno = apellidoMaterno.Trim();
            if (apellidoMaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 111,
                    mensaje = "El parámetro apellidoMaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!string.IsNullOrEmpty(apellidoMaterno))
            {
                if (!fnValidarSoloTexto(apellidoMaterno))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 112,
                        mensaje = "El parámetro apellidoMaterno solo debe contener letras.",
                        data = null
                    };
                }
            }
            if (genero != "N" && genero != "F" && genero != "M")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 113,
                    mensaje = "El parámetro genero sólo acepta N, F o M",
                    data = null
                };
            }
            //DateTime varFecha;
            //if (!DateTime.TryParse(fechaNacimiento, out varFecha))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 109,
            //        mensaje = "El parámetro fechaNacimiento no tiene el formato correcto",
            //        data = null
            //    };
            //}
            if (email.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 114,
                    mensaje = "El parámetro email no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (celular.Length > 9)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 115,
                    mensaje = "El parámetro celular no puede tener más de 9 caracteres",
                    data = null
                };
            }
            //if (!string.IsNullOrEmpty(seguroPlanSalud) && seguroPlanSalud.Length != 11)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 112,
            //        mensaje = "El parámetro seguroPlanSalud debe tener 11 caracteres",
            //        data = null
            //    };
            //}
            if (tipoDocumento == tipoDocumentoTitular && numeroDocumento == numeroDocumentoTitular)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 116,
                    mensaje = "No se puede agregar al titular como familiar",
                    data = null
                };
            }
            if (tipoParentesco.Length > 3)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 117,
                    mensaje = "El parámetro tipoParentesco no puede tener más de 3 caracteres",
                    data = null
                };
            }
            bool varBool;
            if (!bool.TryParse(finalidadesAdiciona, out varBool))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 118,
                    mensaje = "El parámetro finalidadesAdiciona debe ser \"true\"(1) o \"false\"(0)",
                    data = null
                };
            }
            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                //codEstadoSolicitud: 1 (Pendiente de aprobación)
                if (codEstadoSolicitud == "1" && (string.IsNullOrEmpty(adjunto) || string.IsNullOrEmpty(nombreAdjunto)))
                {
                    throw new WebFaultException(HttpStatusCode.BadRequest);
                }

                string nombreGenerado = "";
                if (!string.IsNullOrEmpty(nombreArchivo) && !string.IsNullOrEmpty(archivo))
                {
                    string[] aNom = nombreArchivo.Split('.');
                    byte[] buffer = Convert.FromBase64String(archivo);
                    string ruta = ConfigurationManager.AppSettings["rutaUsuarioImagen"];
                    nombreGenerado = DateTime.Now.ToString("ddMMyyyHHmmss") + "." + aNom[aNom.Length - 1];
                    string rutaGenerada = Path.Combine(ruta, nombreGenerado);
                    File.WriteAllBytes(rutaGenerada, buffer);
                }
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    tipoDocumentoTitular = tipoDocumentoTitular,
                    numeroDocumentoTitular = numeroDocumentoTitular,
                    nombres = nombres,
                    apellidoPaterno = apellidoPaterno,
                    apellidoMaterno = apellidoMaterno,
                    genero = genero,
                    fechaNacimiento = fechaNacimiento,
                    email = email,
                    celular = celular,
                    seguroPlanSalud = seguroPlanSalud,
                    direccion = direccion,
                    numeroDireccion = numeroDireccion,
                    numeroDepartamento = numeroDepartamento,
                    referencia = referencia,
                    latlong = latlong,
                    tipoParentesco = tipoParentesco,
                    nombreArchivo = nombreArchivo,
                    nombreGenerado = nombreGenerado,
                    telefono = telefono,
                    tipoPaciente = tipoPaciente,
                    observacion = observacion,
                    idPais = idPais,
                    idDepartamento = idDepartamento,
                    idProvincia = idProvincia,
                    idDistrito = idDistrito,
                    idPaisNac = idPaisNac,
                    idDepartamentoNac = idDepartamentoNac,
                    idProvinciaNac = idProvinciaNac,
                    idDistritoNac = idDistritoNac,
                    finalidadesAdiciona = finalidadesAdiciona,
                    codEstadoSolicitud = codEstadoSolicitud,
                    origen = origen
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/AgregarFamiliar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<AgregarFamiliarBE> varRespuesta2 = new JavaScriptSerializer().Deserialize<RespuestaBE<AgregarFamiliarBE>>(response);

                if (varRespuesta2 == null)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 1,
                        mensaje = "Solicitud no procesada",
                        data = null
                    };
                }
                if (varRespuesta2.rpt != 0)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 1,
                        mensaje = varRespuesta2.mensaje,
                        data = null
                    };
                }

                //codEstadoSolicitud: 1 (Pendiente de aprobación)
                if (codEstadoSolicitud == "1" && varRespuesta2.data != null)
                {
                    AgregarFamiliarBE oAgregarFamiliarBE = varRespuesta2.data;
                    PlantillaCorreoBE oPlantillaCorreoBE = oAgregarFamiliarBE.plantillaCorreo;
                    oPlantillaCorreoBE.asunto = oPlantillaCorreoBE.asunto.Replace("{NombreCompletoPaciente}", oAgregarFamiliarBE.nombreCompleto);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{NombreCompletoPaciente}", oAgregarFamiliarBE.nombreCompleto);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{FechaSolicitud}", oAgregarFamiliarBE.fechaSolicitud);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{HoraSolicitud}", oAgregarFamiliarBE.horaSolicitud);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{TipoDocumentoDes}", oAgregarFamiliarBE.tipoDocumentoDes);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{NumeroDocumento}", numeroDocumento);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{NombreCompletoTitular}", oAgregarFamiliarBE.nombreCompletoTitular);
                    bool exito = ClasesGenericas.EnviarCorreoPlantillaHTML(oPlantillaCorreoBE.para, oPlantillaCorreoBE.asunto,
                        oPlantillaCorreoBE.cuerpo, null, "SMTP", oPlantillaCorreoBE.cc, ';', oPlantillaCorreoBE.cco, ';',
                        adjunto, nombreAdjunto);
                    if (exito)
                    {
                        varRespuesta = new RespuestaSimpleBE()
                        {
                            rpt = 0,
                            mensaje = oPlantillaCorreoBE.mensajeAlerta,
                            data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/AgregarFamiliar");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE ModificarFamiliar(string tipoDocumento, string numeroDocumento, string tipoDocumentoTitular,
                                    string numeroDocumentoTitular, string genero, string fechaNacimiento,
                                    string email, string celular, string seguroPlanSalud, string tipoParentesco, string direccion = "", 
                                    string numeroDireccion = "", string numeroDepartamento = "", string referencia = "", string latlong = "", 
                                    string nombres = "", string apellidoPaterno = "", string apellidoMaterno = "", string nombreArchivo = "", 
                                    string archivo = "", string telefono = "", string tipoPaciente = "", string observacion = "",
                                    string idPais = "", string idDepartamento = "", string idProvincia = "", string idDistrito = "",
                                    string idPaisNac = "", string idDepartamentoNac = "", string idProvinciaNac = "", string idDistritoNac = "",
                                    string finalidadesAdiciona = "", string origen = "", bool esIntranet = false)
        {
            if (string.IsNullOrEmpty(genero))
                genero = "N";
            if (string.IsNullOrEmpty(finalidadesAdiciona))
                finalidadesAdiciona = "true";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(fechaNacimiento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            nombres = nombres.Trim();
            if (nombres.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "El parámetro nombres no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(nombres))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 108,
                    mensaje = "El parámetro nombres solo debe contener letras.",
                    data = null
                };
            }
            apellidoPaterno = apellidoPaterno.Trim();
            if (apellidoPaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 109,
                    mensaje = "El parámetro apellidoPaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(apellidoPaterno))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 110,
                    mensaje = "El parámetro apellidoPaterno solo debe contener letras.",
                    data = null
                };
            }
            apellidoMaterno = apellidoMaterno.Trim();
            if (apellidoMaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 111,
                    mensaje = "El parámetro apellidoMaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!string.IsNullOrEmpty(apellidoMaterno))
            {
                if (!fnValidarSoloTexto(apellidoMaterno))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 112,
                        mensaje = "El parámetro apellidoMaterno solo debe contener letras.",
                        data = null
                    };
                }
            }
            if (genero != "N" && genero != "F" && genero != "M")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 106,
                    mensaje = "El parámetro genero sólo acepta N, F o M",
                    data = null
                };
            }
            DateTime varFecha;
            if (!DateTime.TryParse(fechaNacimiento, out varFecha))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "El parámetro fechaNacimiento no tiene el formato correcto",
                    data = null
                };
            }
            if (email.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 108,
                    mensaje = "El parámetro email no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (celular.Length > 9)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 109,
                    mensaje = "El parámetro celular no puede tener más de 9 caracteres",
                    data = null
                };
            }
            //if (!string.IsNullOrEmpty(seguroPlanSalud) && seguroPlanSalud.Length != 11)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 110,
            //        mensaje = "El parámetro seguroPlanSalud debe tener 11 caracteres",
            //        data = null
            //    };
            //}

            if (!string.IsNullOrEmpty(numeroDocumento) && !string.IsNullOrEmpty(numeroDocumentoTitular) &&
                !numeroDocumento.Equals(numeroDocumentoTitular) &&
                string.IsNullOrEmpty(tipoParentesco))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 110,
                    mensaje = "El parámetro tipoParentesco es requerido",
                    data = null
                };
            }

            if (!string.IsNullOrEmpty(numeroDocumento) && !string.IsNullOrEmpty(numeroDocumentoTitular) &&
                  !numeroDocumento.Equals(numeroDocumentoTitular) &&
                tipoParentesco.Length > 3)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 111,
                    mensaje = "El parámetro tipoParentesco debe tener 3 caracteres",
                    data = null
                };
            }
            bool varBool;
            if (!bool.TryParse(finalidadesAdiciona, out varBool))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 112,
                    mensaje = "El parámetro finalidadesAdiciona debe ser \"true\"(1) o \"false\"(0)",
                    data = null
                };
            }

            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                string nombreGenerado = "";
                if (!string.IsNullOrEmpty(nombreArchivo) && !string.IsNullOrEmpty(archivo))
                {
                    string[] aNom = nombreArchivo.Split('.');
                    byte[] buffer = Convert.FromBase64String(archivo);
                    string ruta = ConfigurationManager.AppSettings["rutaUsuarioImagen"];
                    nombreGenerado = DateTime.Now.ToString("ddMMyyyHHmmss") + "." + aNom[aNom.Length - 1];
                    string rutaGenerada = Path.Combine(ruta, nombreGenerado);
                    File.WriteAllBytes(rutaGenerada, buffer);

                }

                if (String.IsNullOrEmpty(nombreGenerado)) nombreArchivo = "";

                var oRequest = new
                {
                    tipoDocumento = tipoDocumento, numeroDocumento = numeroDocumento, tipoDocumentoTitular = tipoDocumentoTitular,
                    numeroDocumentoTitular = numeroDocumentoTitular, genero = genero, fechaNacimiento = fechaNacimiento,
                    email = email, celular = celular, seguroPlanSalud = seguroPlanSalud,
                    tipoParentesco = tipoParentesco, direccion = direccion, numeroDireccion = numeroDireccion,
                    numeroDepartamento = numeroDepartamento, referencia = referencia, latlong = latlong,
                    nombres = nombres, apellidoPaterno = apellidoPaterno, apellidoMaterno = apellidoMaterno,
                    nombreArchivo = nombreArchivo, nombreGenerado = nombreGenerado,
                    telefono = telefono, tipoPaciente = tipoPaciente, observacion = observacion,
                    idPais = idPais, idDepartamento = idDepartamento, idProvincia = idProvincia,
                    idDistrito = idDistrito, idPaisNac = idPaisNac, idDepartamentoNac = idDepartamentoNac,
                    idProvinciaNac = idProvinciaNac, idDistritoNac = idDistritoNac, finalidadesAdiciona = finalidadesAdiciona
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ModificarFamiliar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE EliminarFamiliar(string tipoDocumento, string numeroDocumento,
                                            string tipoDocumentoTitular, string numeroDocumentoTitular, bool esIntranet, 
                                            string motivo)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                string.IsNullOrEmpty(tipoDocumentoTitular) || string.IsNullOrEmpty(numeroDocumentoTitular))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumentoTitular != "1" && tipoDocumentoTitular != "2" && tipoDocumentoTitular != "3")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumentoTitular",
                    data = null
                };
            }
            if (numeroDocumentoTitular.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumentoTitular no puede tener más de 20 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    tipoDocumentoTitular = tipoDocumentoTitular,
                    numeroDocumentoTitular = numeroDocumentoTitular,
                    esIntranet = esIntranet
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/EliminarFamiliar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<EliminarFamiliarBE> varRespuesta2 = new JavaScriptSerializer().Deserialize<RespuestaBE<EliminarFamiliarBE>>(response);

                if (varRespuesta2 == null)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 1,
                        mensaje = "Solicitud no procesada",
                        data = null
                    };
                }
                if (varRespuesta2.rpt != 0)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 1,
                        mensaje = varRespuesta2.mensaje,
                        data = null
                    };
                }
                if (varRespuesta2.data != null)
                {
                    EliminarFamiliarBE oEliminarFamiliarBE = varRespuesta2.data;
                    PlantillaCorreoBE oPlantillaCorreoBE = oEliminarFamiliarBE.plantillaCorreo;
                    motivo = !string.IsNullOrEmpty(motivo) ? motivo.Trim() : "";
                    oPlantillaCorreoBE.asunto = oPlantillaCorreoBE.asunto.Replace("{NombreCompletoPaciente}", oEliminarFamiliarBE.nombreCompleto);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{NombreCompletoPaciente}", oEliminarFamiliarBE.nombreCompleto);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{FechaSolicitud}", oEliminarFamiliarBE.fechaSolicitud);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{HoraSolicitud}", oEliminarFamiliarBE.horaSolicitud);
                    oPlantillaCorreoBE.cuerpo = oPlantillaCorreoBE.cuerpo.Replace("{MotivoRechazo}", motivo);
                    bool exito = ClasesGenericas.EnviarCorreoPlantillaHTML(oPlantillaCorreoBE.para, oPlantillaCorreoBE.asunto,
                        oPlantillaCorreoBE.cuerpo, null, "SMTP", oPlantillaCorreoBE.cc, ';', oPlantillaCorreoBE.cco, ';');
                    if (exito)
                    {
                        varRespuesta = new RespuestaSimpleBE()
                        {
                            rpt = 0,
                            mensaje = "",//oPlantillaCorreoBE.mensajeAlerta,
                            data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/EliminarFamiliar");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<FamiliarBE>> ConsultarFamiliar(string tipoDocumentoTitular, string numeroDocumentoTitular)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumentoTitular) || string.IsNullOrEmpty(numeroDocumentoTitular))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumentoTitular != "1" && tipoDocumentoTitular != "2" && tipoDocumentoTitular != "3")
            {
                return new RespuestaBE<List<FamiliarBE>>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumentoTitular.Length > 20)
            {
                return new RespuestaBE<List<FamiliarBE>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<FamiliarBE>> varRespuesta = new RespuestaBE<List<FamiliarBE>>();
            try
            {
                var oRequest = new
                {
                    tipoDocumentoTitular = tipoDocumentoTitular,
                    numeroDocumentoTitular = numeroDocumentoTitular
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ConsultarFamiliar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<FamiliarBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<FamiliarBE>>(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<UsuarioFamiliaresBE> DatosUsuariosFamiliares(string tipoDocumento, string numeroDocumento)
        {
            RespuestaBE<UsuarioFamiliaresBE> varRespuesta = new RespuestaBE<UsuarioFamiliaresBE>();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/DatosUsuariosFamiliares/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioFamiliaresBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<UsuarioFamiliaresBE>(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE EliminarCuenta(string tipoDocumento, string numeroDocumento, string origen)
        {
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    origen = origen
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/EliminarCuenta/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE CifrarClave(string clave)
        {
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            if (string.IsNullOrEmpty(clave))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            try
            {
                string indicadoCifradoClave = ConfigurationManager.AppSettings["indicadoCifradoClave"];
                if (indicadoCifradoClave.Equals("0"))
                {
                    string claveHexa = ToHexString(clave);
                    string tokenSesion = ClasesGenericas.ObtenerMD5(claveHexa);
                    varRespuesta.rpt = 0;
                    varRespuesta.data = tokenSesion;
                }
                else
                {
                    var oRequest = new
                    {
                        clave = clave
                    };
                    string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                    string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/CifrarClave/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                    varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public UsuarioDatosContactoMFABE DatosContactoMFA(string codigoEmpresa, string codigoAplicativo, string usuario,
            string dispositivo, string tokenAutenticacion)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrWhiteSpace(codigoEmpresa) || string.IsNullOrWhiteSpace(codigoAplicativo) || string.IsNullOrWhiteSpace(usuario)
                || string.IsNullOrWhiteSpace(dispositivo) || (string.IsNullOrWhiteSpace(tokenAutenticacion)))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            string pUsuarioEmpresaMFA = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "Usuario");
            string pClaveEmpresaMFA = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "Contrasena");
            if (string.IsNullOrWhiteSpace(pUsuarioEmpresaMFA) || string.IsNullOrWhiteSpace(pClaveEmpresaMFA))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            string usuarioEmpresaMFA = ClasesGenericas.GetSetting("UsuarioEmpresaMFA");
            string claveEmpresaMFA = ClasesGenericas.GetSetting("ClaveEmpresaMFA");
            if (pUsuarioEmpresaMFA != usuarioEmpresaMFA
                || pClaveEmpresaMFA != claveEmpresaMFA)
            {
                throw new WebFaultException(HttpStatusCode.Unauthorized);
            }
            string[] aUsuario = usuario.Split('|');
            string tipoDocumento = "", numeroDocumento = "";
            if (aUsuario.Length != 2)
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            else
            {
                tipoDocumento = aUsuario[0];
                numeroDocumento = aUsuario[1];
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new UsuarioDatosContactoMFABE()
                {
                    codigo = "01",
                    mensaje = "Ingresar parámetros requeridos"//"Tipo de documento inválido"
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new UsuarioDatosContactoMFABE()
                {
                    codigo = "01",
                    mensaje = "Ingresar parámetros requeridos"//"Número de documento inválido"
                };
            }
            if (tipoDocumento.Equals("1") && numeroDocumento.Length != 8)
            {
                return new UsuarioDatosContactoMFABE()
                {
                    codigo = "01",
                    mensaje = "Ingresar parámetros requeridos"//"Número de documento inválido"
                };
            }
            int varCodigoEmpresa;
            if (!int.TryParse(codigoEmpresa, out varCodigoEmpresa))
            {
                return new UsuarioDatosContactoMFABE()
                {
                    codigo = "01",
                    mensaje = "Ingresar parámetros requeridos"//"Código de empresa inválido"
                };
            }
            int varCodigoAplicativo;
            if (!int.TryParse(codigoAplicativo, out varCodigoAplicativo))
            {
                return new UsuarioDatosContactoMFABE()
                {
                    codigo = "01",
                    mensaje = "Ingresar parámetros requeridos"//"Código de aplicativo inválido"
                };
            }
            int varTipoDocumento;
            if (!int.TryParse(tipoDocumento, out varTipoDocumento))
            {
                return new UsuarioDatosContactoMFABE()
                {
                    codigo = "01",
                    mensaje = "Ingresar parámetros requeridos"//"Tipo de documento inválido"
                };
            }
            #endregion
            UsuarioDatosContactoMFABE varRespuesta = new UsuarioDatosContactoMFABE();
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                var oRequest = new
                {
                    codigoEmpresa = codigoEmpresa,
                    codigoAplicativo = codigoAplicativo,
                    usuario = usuario,
                    dispositivo = dispositivo,
                    tokenAutenticacion = tokenAutenticacion
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/DatosContactoMFA/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<UsuarioDatosContactoMFABE>(response);
                if (varRespuesta == null)
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("HTTP-400:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.BadRequest);
                //}
                //else if (ex.Message.StartsWith("HTTP-401:"))
                //{
                //    throw new WebFaultException(HttpStatusCode.Unauthorized);
                //}
                //else
                //{
                //    return new UsuarioDatosContactoMFABE()
                //    {
                //        codigo = "04",
                //        mensaje = "Error en el servicio"
                //    };
                //    //return new ErrorDA().RegistrarError<UsuarioDatosContactoMFABE>(ex, "WS", "Usuario.svc/DatosContactoMFA");
                //}
            }
            return varRespuesta;
        }
        public RespuestaBE<AutenticarMFABE> AutenticarMFA(string codigoAplicacion, string codigoEquipo, string so, bool esReenvio,
            string hashDispositivo, string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrWhiteSpace(codigoAplicacion) || string.IsNullOrWhiteSpace(codigoEquipo) || string.IsNullOrWhiteSpace(so))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            bool flgWSDisponible = false;
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                AutenticarMFABE oAutenticarMFA = new AutenticarMFABE();

                //string tipoDocumento = "", numeroDocumento = "";
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    codigoAplicacion = codigoAplicacion
                };
                string strRequestIntra = new JavaScriptSerializer().Serialize(oRequest);
                string strResponseIntra = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ObtenerConfiguracionMFA/", strRequestIntra, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<ConfiguracionMFABE> varRptaConfiguracionMFA = new JavaScriptSerializer().Deserialize<RespuestaBE<ConfiguracionMFABE>>(strResponseIntra);
                if (varRptaConfiguracionMFA == null)
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                ConfiguracionMFABE oConfiguracionMFA = varRptaConfiguracionMFA.data;
                if (ClasesGenericas.GetSetting("Indicador_MFA_Flujo" + oConfiguracionMFA.tipoFlujo) != "1")
                {
                    return new RespuestaBE<AutenticarMFABE>()
                    {
                        rpt = 0,
                        mensaje = "Desactivado para este flujo",
                        data = new AutenticarMFABE()
                        {
                            requiereMFA = false
                        }
                    };
                }
                string urlBase = ClasesGenericas.GetSetting("UrlBaseMFA"), urlMetodo = "",
                    strRequest = "", strResponse = "";
                string codigoUsuario = oConfiguracionMFA.tipoDocumento + "|" + oConfiguracionMFA.numeroDocumento;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(urlBase);
                    //HTTP POST
                    urlMetodo = "mfa/autenticar";
                    MFAAutenticarBE oAutenticar = new MFAAutenticarBE()
                    {
                        codigoEmpresa = oConfiguracionMFA.codigoEmpresa,
                        codigoAplicativo = oConfiguracionMFA.codigoAplicativo,
                        clave = oConfiguracionMFA.claveAplicativo,
                        usuario = codigoUsuario,
                        dispositivo = oConfiguracionMFA.codigoDispositivo
                    };
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CSF_WS/" + ClasesGenericas.GetSetting("_VersionApp"));
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    strRequest = JsonConvert.SerializeObject(oAutenticar).ToString();
                    var content = new StringContent(strRequest, Encoding.UTF8, "application/json");

                    flgWSDisponible = false;
                    var responseWS = client.PostAsync(urlMetodo, content);
                    responseWS.Wait();
                    var responseAutenticar = responseWS.Result;
                    flgWSDisponible = true;

                    if (responseAutenticar.IsSuccessStatusCode)
                    {
                        var response = responseAutenticar.Content.ReadAsStringAsync();
                        response.Wait();
                        strResponse = response.Result;
                        MFAAutenticarResponseBE oResponse = JsonConvert.DeserializeObject<MFAAutenticarResponseBE>(strResponse);
                        if (oResponse != null)
                        {
                            if (oResponse.codigo.Equals("00"))
                            {
                                var oRequest2 = new
                                {
                                    codigoEmpresa = oConfiguracionMFA.codigoEmpresa,
                                    codigoAplicativo = oConfiguracionMFA.codigoAplicativo,
                                    codigoUsuario = codigoUsuario,
                                    codigoDispositivo = oConfiguracionMFA.codigoDispositivo,
                                    token = oResponse.token,
                                    expira = oResponse.expira,
                                    tipoDocumento = oConfiguracionMFA.tipoDocumento,
                                    numeroDocumento = oConfiguracionMFA.numeroDocumento
                                };
                                string strRequestIntra2 = new JavaScriptSerializer().Serialize(oRequest2);
                                string strResponseIntra2 = ClasesGenericas.PostAsyncIntranet("Usuario.svc/GrabarTokenMFA/", strRequestIntra2, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                                RespuestaSimpleBE oRespuestaSimpleBE = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(strResponseIntra2);
                                urlMetodo = "mfa/enviarcodigo";
                                MFAEnviarCodigoBE oEnviarCodigo = new MFAEnviarCodigoBE()
                                {
                                    codigoEmpresa = oConfiguracionMFA.codigoEmpresa,
                                    codigoAplicativo = oConfiguracionMFA.codigoAplicativo,
                                    usuario = codigoUsuario,
                                    dispositivo = oConfiguracionMFA.codigoDispositivo,
                                    viaEnvio = ClasesGenericas.GetSetting("ViaEnvioPorDefectoMFA"),
                                    identificadorEquipo = codigoEquipo,
                                    sistemaOperativo = so,
                                    hashDispositivo = hashDispositivo
                                };
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Add("User-Agent", "CSF_WS/" + ClasesGenericas.GetSetting("_VersionApp"));
                                client.DefaultRequestHeaders.Add("Accept", "*/*");
                                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                                strRequest = JsonConvert.SerializeObject(oEnviarCodigo).ToString();

                                string rutaArchivo = ConfigurationManager.AppSettings["_LogPath"];
                                string nombreArchivo = "AutenticarMFA.txt";
                                DateTime oFechaActual = DateTime.Now;
                                using (FileStream stream = new FileStream(rutaArchivo + nombreArchivo, FileMode.Append, FileAccess.Write, FileShare.Write))
                                {
                                    using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Default))
                                    {
                                        streamWriter.WriteLine(oFechaActual.ToString("dd/MM/yyyy HH:mm:ss.fff"));
                                        streamWriter.WriteLine("URL: " + urlBase + urlMetodo);
                                        streamWriter.WriteLine("Request: " + strRequest);
                                        streamWriter.WriteLine("");
                                        streamWriter.WriteLine(new string('_', 50));
                                    }
                                }
                                content = new StringContent(strRequest, Encoding.UTF8, "application/json");
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + oResponse.token);

                                flgWSDisponible = false;
                                responseWS = client.PostAsync(urlMetodo, content);
                                responseWS.Wait();
                                var responseEnviarCodigo = responseWS.Result;
                                flgWSDisponible = true;

                                if (responseEnviarCodigo.IsSuccessStatusCode)
                                {
                                    response = responseEnviarCodigo.Content.ReadAsStringAsync();
                                    response.Wait();
                                    strResponse = response.Result;
                                    MFAEnviarCodigoResponseBE oResponse2 = JsonConvert.DeserializeObject<MFAEnviarCodigoResponseBE>(strResponse);
                                    if (oResponse2 != null)
                                    {
                                        oAutenticarMFA = new AutenticarMFABE();
                                        if (oResponse2.codigo.Equals("00"))
                                        {
                                            oAutenticarMFA.requiereMFA = true;
                                            oAutenticarMFA.tokenAutenticacion = oResponse.token;
                                            oAutenticarMFA.expiracionTokenAutenticacion = oResponse.expira;
                                            oAutenticarMFA.expiracionCodigoMFA = oResponse2.expira;
                                            oAutenticarMFA.minutosVigenciaCodigoMFA = oResponse2.minutosVigencia != null ? oResponse2.minutosVigencia : ClasesGenericas.GetSetting("MinutosPorDefectoCodigoMFA");
                                            return new RespuestaBE<AutenticarMFABE>()
                                            {
                                                rpt = 0,
                                                mensaje = esReenvio ? "Te acabamos de enviar un nuevo código de seguridad a tu celular" : "",
                                                data = oAutenticarMFA
                                            };
                                        }
                                        else if (oResponse2.codigo.Equals("01"))
                                        {
                                            return new RespuestaBE<AutenticarMFABE>()
                                            {
                                                rpt = 3,
                                                mensaje = "Has agotado los intentos para la validación. Vuelve a intentarlo en unos minutos."
                                            };
                                        }
                                        else if (oResponse2.codigo.Equals("09"))
                                        {
                                            return new RespuestaBE<AutenticarMFABE>()
                                            {
                                                rpt = 0,
                                                mensaje = esReenvio ? "Te acabamos de enviar un nuevo código de seguridad a tu celular" : "",
                                                data = oAutenticarMFA
                                            };
                                        }
                                        return new RespuestaBE<AutenticarMFABE>()
                                        {
                                            rpt = 5,
                                            mensaje = "Error en envío de código: " + oResponse2.codigo
                                        };
                                    }
                                    return new RespuestaBE<AutenticarMFABE>()
                                    {
                                        rpt = 4,
                                        mensaje = "Envío de código no procesado"
                                    };
                                }
                                else if (responseEnviarCodigo.StatusCode.Equals(HttpStatusCode.BadRequest))
                                {
                                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                                }
                                else if (responseEnviarCodigo.StatusCode.Equals(HttpStatusCode.Unauthorized))
                                {
                                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                                }
                                else
                                {
                                    return new RespuestaBE<AutenticarMFABE>()
                                    {
                                        rpt = 0,
                                        mensaje = "Servicio MFA no disponible",
                                        data = new AutenticarMFABE()
                                        {
                                            requiereMFA = false
                                        }
                                    };
                                }
                                return null;
                            }
                            return new RespuestaBE<AutenticarMFABE>()
                            {
                                rpt = 2,
                                mensaje = "Error en autenticación: " + oResponse.codigo
                            };
                        }
                        return new RespuestaBE<AutenticarMFABE>()
                        {
                            rpt = 1,
                            mensaje = "Autenticación no procesada"
                        };
                    }
                    else if (responseAutenticar.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    }
                    else if (responseAutenticar.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        return new RespuestaBE<AutenticarMFABE>()
                        {
                            rpt = 0,
                            mensaje = "Servicio MFA no disponible",
                            data = new AutenticarMFABE()
                            {
                                requiereMFA = false
                            }
                        };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!flgWSDisponible)
                {
                    return new RespuestaBE<AutenticarMFABE>()
                    {
                        rpt = 0,
                        mensaje = "Servicio MFA no disponible: " + ex.Message,
                        data = new AutenticarMFABE()
                        {
                            requiereMFA = false
                        }
                    };
                }
                else
                {
                    if (ex.Message.StartsWith("HTTP-400:"))
                    {
                        throw new WebFaultException(HttpStatusCode.BadRequest);
                    }
                    else if (ex.Message.StartsWith("HTTP-401:"))
                    {
                        throw new WebFaultException(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        //return new ErrorDA().RegistrarError<AutenticarMFABE>(ex, "WS", "Usuario.svc/AutenticarMFA");
                        return new RespuestaBE<AutenticarMFABE>()
                        {
                            rpt = 1,
                            mensaje = ex.Message,
                            data = new AutenticarMFABE()
                            {
                                requiereMFA = false
                            }
                        };
                    }
                }
            }
        }
        public RespuestaBE<ValidarCodigoMFABE> ValidarCodigoMFA(string codigoAplicacion, string tokenAtenticacion, string codigoMFA,
            string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrWhiteSpace(codigoAplicacion) || string.IsNullOrWhiteSpace(tokenAtenticacion) || string.IsNullOrWhiteSpace(codigoMFA))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            bool flgWSDisponible = false;
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                ValidarCodigoMFABE oValidarCodigoMFA = new ValidarCodigoMFABE();

                //string tipoDocumento = "", numeroDocumento = "";
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    codigoAplicacion = codigoAplicacion
                };
                string strRequestIntra = new JavaScriptSerializer().Serialize(oRequest);
                string strResponseIntra = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ObtenerConfiguracionMFA/", strRequestIntra, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<ConfiguracionMFABE> varRptaConfiguracionMFA = new JavaScriptSerializer().Deserialize<RespuestaBE<ConfiguracionMFABE>>(strResponseIntra);
                if (varRptaConfiguracionMFA == null)
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                ConfiguracionMFABE oConfiguracionMFA = varRptaConfiguracionMFA.data;

                string urlBase = ClasesGenericas.GetSetting("UrlBaseMFA"), urlMetodo = "",
                    strRequest = "", strResponse = "";
                string codigoUsuario = oConfiguracionMFA.tipoDocumento + "|" + oConfiguracionMFA.numeroDocumento;

                #region Saltar validación MFA si el usuario (temporal) es el de prueba
                string tipoDocumentoTmp = ClasesGenericas.GetSetting("UsuarioGenericoMFA_TipoDocumento");
                string numeroDocumentoTmp = ClasesGenericas.GetSetting("UsuarioGenericoMFA_NumeroDocumento");
                string codigoMFATmp = ClasesGenericas.GetSetting("UsuarioGenericoMFA_CodigoMFA");
                if (!string.IsNullOrEmpty(tipoDocumentoTmp) && !string.IsNullOrEmpty(numeroDocumentoTmp)
                    && !string.IsNullOrEmpty(codigoMFATmp)) { 
                    if (oConfiguracionMFA.tipoDocumento.Equals(tipoDocumentoTmp) && oConfiguracionMFA.numeroDocumento.Equals(numeroDocumentoTmp))
                    {
                        if (codigoMFA.Equals(codigoMFATmp))
                        {
                            return new RespuestaBE<ValidarCodigoMFABE>()
                            {
                                rpt = 0,
                                mensaje = ""
                            };
                        }
                    }
                }
                #endregion

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(urlBase);
                    //HTTP POST
                    urlMetodo = "mfa/validarcodigo";
                    MFAValidarCodigoBE oValidarCodigo = new MFAValidarCodigoBE()
                    {
                        codigoEmpresa = oConfiguracionMFA.codigoEmpresa,
                        codigoAplicativo = oConfiguracionMFA.codigoAplicativo,
                        usuario = codigoUsuario,
                        dispositivo = oConfiguracionMFA.codigoDispositivo,
                        codigoMFA = codigoMFA
                    };
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CSF_WS/" + ClasesGenericas.GetSetting("_VersionApp"));
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    strRequest = JsonConvert.SerializeObject(oValidarCodigo).ToString();
                    var content = new StringContent(strRequest, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenAtenticacion);

                    flgWSDisponible = false;
                    var responseWS = client.PostAsync(urlMetodo, content);
                    responseWS.Wait();
                    var responseValidarCodigo = responseWS.Result;
                    flgWSDisponible = true;

                    if (responseValidarCodigo.IsSuccessStatusCode)
                    {
                        var response = responseValidarCodigo.Content.ReadAsStringAsync();
                        response.Wait();
                        strResponse = response.Result;
                        MFAValidarCodigoResponseBE oResponse = JsonConvert.DeserializeObject<MFAValidarCodigoResponseBE>(strResponse);
                        if (oResponse != null)
                        {
                            if (oResponse.codigo.Equals("00"))
                            {
                                return new RespuestaBE<ValidarCodigoMFABE>()
                                {
                                    rpt = 0,
                                    mensaje = ""
                                };
                            }
                            else if (oResponse.codigo.Equals("04"))//Equals("01"))
                            {
                                oValidarCodigoMFA = new ValidarCodigoMFABE();
                                oValidarCodigoMFA.indicadorBloqueado = !string.IsNullOrEmpty(oResponse.indicadorBloqueado) ? oResponse.indicadorBloqueado.Equals("1") : false;
                                oValidarCodigoMFA.expiracionBloqueo = !string.IsNullOrEmpty(oResponse.expiraBloqueo) ? oResponse.expiraBloqueo : "";
                                if (oValidarCodigoMFA.indicadorBloqueado)
                                {
                                    return new RespuestaBE<ValidarCodigoMFABE>()
                                    {
                                        rpt = 3,
                                        mensaje = "Has agotado los intentos para la validación. Vuelve a intentarlo en unos minutos.",
                                        data = oValidarCodigoMFA
                                    };
                                }
                            }
                            return new RespuestaBE<ValidarCodigoMFABE>()
                            {
                                rpt = 2,
                                mensaje = "Error en validación de código: " + oResponse.codigo
                            };
                        }
                        new RespuestaBE<ValidarCodigoMFABE>()
                        {
                            rpt = 1,
                            mensaje = "Validación de código no procesada"
                        };
                    }
                    else if (responseValidarCodigo.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    }
                    else if (responseValidarCodigo.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        return new RespuestaBE<ValidarCodigoMFABE>()
                        {
                            rpt = 0,
                            mensaje = "Servicio MFA no disponible"
                        };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!flgWSDisponible)
                {
                    return new RespuestaBE<ValidarCodigoMFABE>()
                    {
                        rpt = 0,
                        mensaje = "Servicio MFA no disponible: " + ex.Message
                    };
                }
                else
                {
                    if (ex.Message.StartsWith("HTTP-400:"))
                    {
                        throw new WebFaultException(HttpStatusCode.BadRequest);
                    }
                    else if (ex.Message.StartsWith("HTTP-401:"))
                    {
                        throw new WebFaultException(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        //return new ErrorDA().RegistrarError<ValidarCodigoMFABE>(ex, "WS", "Usuario.svc/ValidarCodigoMFA");
                        return new RespuestaBE<ValidarCodigoMFABE>()
                        {
                            rpt = 1,
                            mensaje = ex.Message
                        };
                    }
                }
            }
        }
        public RespuestaSimpleBE ValidarClave(string data)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (ClasesGenericas.GetSetting("DescifrarAES").Equals("1"))
                {
                    data = ClasesGenericas.DecryptStringAES(data, ClasesGenericas.GetSetting("KeyAES"));
                }
                if (String.IsNullOrEmpty(data))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                String[] aData = data.Split('|');
                if (aData.Length != 4)
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                string tipoDocumento = aData[0],
                       numeroDocumento = aData[1],
                       passwordSinCifrar = aData[2],
                       fechaHoraConsulta = aData[3]; // dd/MM/yyyy HH:mm:ss

                string tiempoVigenciaSolicitudAES = ClasesGenericas.GetSetting("VigenciaSolicitudAES");
                double varTiempoVigenciaSolicitudAES;
                bool esTiempoValido = double.TryParse(tiempoVigenciaSolicitudAES, out varTiempoVigenciaSolicitudAES);
                varTiempoVigenciaSolicitudAES = esTiempoValido ? varTiempoVigenciaSolicitudAES : 5; //La consulta tendrá una vigencia del parámetro configurado, donde por defecto es 5 segundos como máximo
                DateTime oFechaHoraActual = DateTime.Now.AddSeconds(-varTiempoVigenciaSolicitudAES);
                DateTime oFechaHoraConsulta;
                if (DateTime.TryParse(fechaHoraConsulta, out oFechaHoraConsulta))
                {
                    if (oFechaHoraConsulta <= oFechaHoraActual)
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                }
                else
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }

                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    passwordSinCifrar = passwordSinCifrar
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarClave/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaSimpleBE varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
                if (varRespuesta == null)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                return varRespuesta;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc/ValidarClave");
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/ValidarClave");
            }
        }
        public RespuestaBE<VD_ListarDeudaResponsePresBE> ValidarDeuda(string tipoDocumento, string numeroDocumento, string tipoDocumentoPaciente,
            string numeroDocumentoPaciente, string origen)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                origen = !string.IsNullOrEmpty(origen) ? origen : "";
                string tipoMensaje; // 1 (Mensaje para Paciente) y 2 (Mensaje para Counter)
                if (origen.ToLower().Equals("agenda"))
                {
                    tipoMensaje = "2";
                }
                else
                {
                    tipoMensaje = "1";
                }
                RespuestaBE<VD_ListarDeudaResponsePresBE> oResponse = new RespuestaBE<VD_ListarDeudaResponsePresBE>();
                string tokenSesion = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token");
                //UsuarioDocumentoBE oUsuario = new UsuarioDA().ObtenerPorToken(tokenSesion, tipoDocumento, numeroDocumento, true);
                var oRequestTmp = new
                {
                    tokenSesion = tokenSesion,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    validarFamiliar = true
                };
                string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                string responseTmp = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ObtenerPorToken/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<UsuarioDocumentoBE> varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioDocumentoBE>>(responseTmp);

                if (varRespuestaTmp == null)
                {
                    return new RespuestaBE<VD_ListarDeudaResponsePresBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuestaTmp.rpt != 0)
                {
                    return new RespuestaBE<VD_ListarDeudaResponsePresBE>()
                    {
                        rpt = varRespuestaTmp.rpt,
                        mensaje = varRespuestaTmp.mensaje,
                        data = null
                    };
                }
                UsuarioDocumentoBE oUsuario = varRespuestaTmp.data;

                WebOperationContext oContext = WebOperationContext.Current;
                if (!(oUsuario.tipoDocumento == tipoDocumento
                    && oUsuario.numeroDocumento == numeroDocumento))
                {
                    return new RespuestaBE<VD_ListarDeudaResponsePresBE>()
                    {
                        rpt = 102,
                        mensaje = "Usuario inválido",
                        data = null
                    };
                    //oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    //return null;
                }
                #region Integración
                QR_RespuestaSimpleBE oRespuestaSimpleBE = new QR_RespuestaSimpleBE();
                QR_RespuestaBE<VD_ListarDeudaResponseBE> oRespuestaQRBE = new QR_RespuestaBE<VD_ListarDeudaResponseBE>();

                string urlBase = "", urlMetodo = "";
                string token = "";
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                using (var client = new HttpClient())
                {
                    urlBase = ClasesGenericas.GetSetting("WS_QR").ToString();
                    client.BaseAddress = new Uri(urlBase);
                    QR_LoginBE oRequest = new QR_LoginBE();
                    oRequest.usuario = ClasesGenericas.GetSetting("WS_QR_Usuario");
                    oRequest.password = ClasesGenericas.GetSetting("WS_QR_Clave");
                    var content = new StringContent(JsonConvert.SerializeObject(oRequest).ToString(), Encoding.UTF8, "application/json");
                    urlMetodo = "usuario/login";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CSF_WS/" + ClasesGenericas.GetSetting("_VersionApp"));
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    var responseTaskOne = client.PostAsync(urlMetodo, content);
                    responseTaskOne.Wait();

                    var resultOne = responseTaskOne.Result;

                    string rptaTaskOne = "";
                    if (resultOne.IsSuccessStatusCode)
                    {
                        var readTaskOne = resultOne.Content.ReadAsStringAsync();
                        readTaskOne.Wait();
                        rptaTaskOne = readTaskOne.Result;
                        oRespuestaSimpleBE = JsonConvert.DeserializeObject<QR_RespuestaSimpleBE>(rptaTaskOne);
                        if (oRespuestaSimpleBE.success)
                        {
                            token = oRespuestaSimpleBE.result;
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                            VD_ListarDeudaBE oVD_ListarDeudaBE = new VD_ListarDeudaBE();
                            oVD_ListarDeudaBE.TipoDocumento = tipoDocumentoPaciente;
                            oVD_ListarDeudaBE.NumeroDocumento = numeroDocumentoPaciente;
                            oVD_ListarDeudaBE.TipoMensaje = tipoMensaje;
                            content = new StringContent(JsonConvert.SerializeObject(oVD_ListarDeudaBE).ToString(), Encoding.UTF8, "application/json");
                            urlMetodo = "PacienteCita/ListarDeuda";
                            var responseTaskTwo = client.PostAsync(urlMetodo, content);
                            responseTaskTwo.Wait();

                            var resultTwo = responseTaskTwo.Result;

                            string rptaTaskTwo = "";
                            if (resultTwo.IsSuccessStatusCode)
                            {
                                var readTaskTwo = resultTwo.Content.ReadAsStringAsync();
                                readTaskTwo.Wait();
                                rptaTaskTwo = readTaskTwo.Result;
                                oRespuestaQRBE = JsonConvert.DeserializeObject<QR_RespuestaBE<VD_ListarDeudaResponseBE>>(rptaTaskTwo);
                                if (oRespuestaQRBE.success)
                                {
                                    oResponse.rpt = 0;
                                    oResponse.mensaje = "";
                                    oResponse.data = new VD_ListarDeudaResponsePresBE();
                                    oResponse.data.indicadorBloqueo = oRespuestaQRBE.result.flg_bloquearatencion;
                                    oResponse.data.mensaje = oRespuestaQRBE.result.mensaje;
                                }
                                else
                                {
                                    oResponse.rpt = 4;
                                    oResponse.mensaje = oRespuestaQRBE.message;
                                }
                            }
                            else
                            {
                                oResponse.rpt = 3;
                                oResponse.mensaje = "ListarDeuda: Error en comunicación";
                            }
                        }
                        else
                        {
                            oResponse.rpt = 2;
                            oResponse.mensaje = oRespuestaSimpleBE.message;
                        }
                    }
                    else
                    {
                        oResponse.rpt = 1;
                        oResponse.mensaje = "Autenticación: Error en comunicación";
                    }
                }
                #endregion

                #region Data constante
                //oResponse.data = new VD_ListarDeudaResponsePresBE()
                //{
                //    indicadorBloqueo = "0",
                //    mensaje = ""
                //};
                #endregion

                return oResponse;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<VD_ListarDeudaResponsePresBE>(ex, "WS", "Usuario.svc/ValidarDeuda");
                return ClasesGenericas.RegistrarErrorIntranet<VD_ListarDeudaResponsePresBE>(ex, "WS", "Usuario.svc/ValidarDeuda");
            }
        }
        public RespuestaBE<PersonaRegistroBE> ValidarDocumentoPersona(string tipoDocumento, string numeroDocumento, string origen)
        {
            origen = !string.IsNullOrEmpty(origen) ? origen.ToLower().Trim() : "";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                string.IsNullOrEmpty(origen))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaBE<PersonaRegistroBE>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<PersonaRegistroBE>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaBE<PersonaRegistroBE>()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaBE<PersonaRegistroBE>()
                    {
                        rpt = 104,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            #endregion
            RespuestaBE<PersonaRegistroBE> varRespuesta = new RespuestaBE<PersonaRegistroBE>();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    origen = origen
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarDocumentoPersona/", strRequest);
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<PersonaRegistroBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<UsuarioBE>(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE ValidarRegistroPersona(string tipoDocumento, string numeroDocumento, string nombres,
                                            string apellidoPaterno, string apellidoMaterno, string genero, string fechaNacimiento,
                                            string origen)
        {
            if (string.IsNullOrEmpty(genero))
                genero = "N";
            origen = !string.IsNullOrEmpty(origen) ? origen.ToLower().Trim() : "";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidoPaterno) || string.IsNullOrEmpty(fechaNacimiento) ||
                string.IsNullOrEmpty(origen))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 104,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            nombres = nombres.Trim();
            if (nombres.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 105,
                    mensaje = "El parámetro nombres no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(nombres))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 106,
                    mensaje = "El parámetro nombres solo debe contener letras.",
                    data = null
                };
            }
            apellidoPaterno = apellidoPaterno.Trim();
            if (apellidoPaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "El parámetro apellidoPaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!fnValidarSoloTexto(apellidoPaterno))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 108,
                    mensaje = "El parámetro apellidoPaterno solo debe contener letras.",
                    data = null
                };
            }
            apellidoMaterno = apellidoMaterno != null ? apellidoMaterno.Trim() : "";
            if (apellidoMaterno.Length > 50)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 109,
                    mensaje = "El parámetro apellidoMaterno no puede tener más de 50 caracteres",
                    data = null
                };
            }
            if (!string.IsNullOrEmpty(apellidoMaterno))
            {
                if (!fnValidarSoloTexto(apellidoMaterno))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 110,
                        mensaje = "El parámetro apellidoMaterno solo debe contener letras.",
                        data = null
                    };
                }
            }
            if (genero != "N" && genero != "F" && genero != "M")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 111,
                    mensaje = "El parámetro genero sólo acepta N, F o M",
                    data = null
                };
            }
            DateTime varFecha;
            if (!DateTime.TryParse(fechaNacimiento, out varFecha))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 112,
                    mensaje = "El parámetro fechaNacimiento no tiene el formato correcto",
                    data = null
                };
            }
            #endregion
            RespuestaSimpleBE varRespuesta;
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    nombres = nombres,
                    apellidoPaterno = apellidoPaterno,
                    apellidoMaterno = apellidoMaterno,
                    fechaNacimiento = fechaNacimiento,
                    genero = genero,
                    origen = origen
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarRegistroPersona/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/ValidarRegistroPersona");
            }
            return varRespuesta;
        }
        public RespuestaBE<ValidarPersonaSitedsBE> ValidarPersonaSiteds(string tipoDocumentoTitular, string numeroDocumentoTitular, string tipoDocumento,
            string numeroDocumento, string nombres, string apellidoPaterno, string apellidoMaterno, string codigoTipoParentesco, 
            string origen)
        {
            origen = !string.IsNullOrEmpty(origen) ? origen.ToLower().Trim() : "";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumentoTitular) || string.IsNullOrEmpty(numeroDocumentoTitular) || 
                string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(nombres) ||
                string.IsNullOrEmpty(apellidoPaterno) || //string.IsNullOrEmpty(apellidoMaterno) ||
                string.IsNullOrEmpty(codigoTipoParentesco) || string.IsNullOrEmpty(origen))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            apellidoMaterno = !string.IsNullOrEmpty(apellidoMaterno) ? apellidoMaterno : "";
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaBE<ValidarPersonaSitedsBE>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<ValidarPersonaSitedsBE>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaBE<ValidarPersonaSitedsBE>()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaBE<ValidarPersonaSitedsBE>()
                    {
                        rpt = 104,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            if (tipoDocumentoTitular != "1" && tipoDocumentoTitular != "2" && tipoDocumentoTitular != "3" && tipoDocumentoTitular != "4")
            {
                return new RespuestaBE<ValidarPersonaSitedsBE>()
                {
                    rpt = 105,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumentoTitular",
                    data = null
                };
            }
            if (numeroDocumentoTitular.Length > 20)
            {
                return new RespuestaBE<ValidarPersonaSitedsBE>()
                {
                    rpt = 106,
                    mensaje = "El parámetro numeroDocumentoTitular no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumentoTitular == "1" && numeroDocumentoTitular.Length != 8)
            {
                return new RespuestaBE<ValidarPersonaSitedsBE>()
                {
                    rpt = 107,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            if (tipoDocumentoTitular == "1" && numeroDocumentoTitular.Length == 8)
            {
                if (!int.TryParse(numeroDocumentoTitular, out varEntero))
                {
                    return new RespuestaBE<ValidarPersonaSitedsBE>()
                    {
                        rpt = 108,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            #endregion
            RespuestaBE<ValidarPersonaSitedsBE> varRespuesta;
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumentoTitular,
                    numeroDocumento = numeroDocumentoTitular
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ObtenerDatosUsuario/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<UsuarioBE> varRespuesta2 = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioBE>>(response);

                if (varRespuesta2 == null)
                {
                    varRespuesta = new RespuestaBE<ValidarPersonaSitedsBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuesta2.rpt == 0)
                {
                    UsuarioBE oUsuarioBE = varRespuesta2.data;
                    //CONFIGURACION SITEDS
                    var oRequest2 = new
                    {
                        tipoDocumento = tipoDocumento,
                        numeroDocumento = numeroDocumento,
                        origen = origen
                    };
                    string strRequest2 = new JavaScriptSerializer().Serialize(oRequest2);
                    string response2 = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ObtenerConfiguracionSiteds/", strRequest2, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                    RespuestaBE<ConfiguracionSitedsBE> varRespuesta3 = new JavaScriptSerializer().Deserialize<RespuestaBE<ConfiguracionSitedsBE>>(response2);
                    if (varRespuesta3 == null)
                    {
                        return new RespuestaBE<ValidarPersonaSitedsBE>()
                        {
                            rpt = -1,
                            mensaje = "No se logró procesar la solicitud",
                            data = null
                        };
                    }
                    if (varRespuesta3.rpt != 0)
                    {
                        return new RespuestaBE<ValidarPersonaSitedsBE>()
                        {
                            rpt = 2,
                            mensaje = varRespuesta3.mensaje,
                            data = null
                        };
                    }
                    ConfiguracionSitedsBE oConfiguracionSitedsBE = varRespuesta3.data;

                    if (oUsuarioBE.codTipoPaciente.Equals("1") && !string.IsNullOrEmpty(oUsuarioBE.iafas))
                    {
                        /*  codigoTipoParentesco
                            C	CONYUGE
                            H	HIJO(A)
                            O	OTRO
                         */
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = (object se, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslerror) => true;
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        List<ConsultAsegNomResponse> listResponseOne = new List<ConsultAsegNomResponse>();
                        List<ConsultAsegNomResponse> listResponseOneFiltro = new List<ConsultAsegNomResponse>();

                        ConsultAsegNomResponse responseOne = new ConsultAsegNomResponse();
                        ConsultAsegCodResponse responseTwo = new ConsultAsegCodResponse();

                        int tipoDocumentoPaciente = Convert.ToInt32(oUsuarioBE.tipoDocumento);
                        bool indDNI = tipoDocumentoPaciente == 1;
                        bool indVolverLlamar = false; //Volver a llamar al método de productos

                        string urlBase = "", urlMetodo = "";
                        using (var client = new HttpClient())
                        {
                            urlBase = oConfiguracionSitedsBE.urlBase;
                            client.BaseAddress = new Uri(urlBase);
                            //HTTP POST
                            ConsultAsegNom rqst = new ConsultAsegNom();
                            rqst.CodTipoDocumentoAfiliado = indDNI ? tipoDocumento : "";
                            rqst.NumeroDocumentoAfiliado = indDNI ? numeroDocumento : "";
                            rqst.RUC = oConfiguracionSitedsBE.rucClinica;
                            rqst.SUNASA = oConfiguracionSitedsBE.sunasa;
                            rqst.IAFAS = oUsuarioBE.iafas;
                            rqst.NombresAfiliado = indDNI ? "" : nombres;
                            rqst.ApellidoPaternoAfiliado = indDNI ? "" : apellidoPaterno;
                            rqst.ApellidoMaternoAfiliado = indDNI ? "" : apellidoMaterno;
                            rqst.CodEspecialidad = oUsuarioBE.iafas.Equals("30004") ? "028" : "";
                            Debug.WriteLine("Request");
                            Debug.WriteLine(JsonConvert.SerializeObject(rqst).ToString());
                            var content = new StringContent(JsonConvert.SerializeObject(rqst).ToString(), Encoding.UTF8, "application/json");
                            urlMetodo = "Sistema/ConsultaAsegNom";
                            var responseTaskOne = client.PostAsync(urlMetodo, content);
                            responseTaskOne.Wait();

                            var resultOne = responseTaskOne.Result;
                            // CodEstado: 1 (VIGENTE)
                            // CodParentesco: 1 (TITULAR)
                            string rptaTaskOne = "";
                            if (resultOne.IsSuccessStatusCode)
                            {
                                var readTaskOne = resultOne.Content.ReadAsStringAsync();
                                readTaskOne.Wait();
                                if (readTaskOne.Result.Length > 2 && readTaskOne.Result != "[]")
                                {
                                    Debug.WriteLine("Response");
                                    Debug.WriteLine(readTaskOne.Result);
                                    rptaTaskOne = readTaskOne.Result;
                                    listResponseOne = JsonConvert.DeserializeObject<List<ConsultAsegNomResponse>>(rptaTaskOne);
                                    int nRegistros = listResponseOne.Count;
                                    if (nRegistros >= 25 && oUsuarioBE.iafas.Equals("20001"))
                                    {
                                        indVolverLlamar = true;
                                    }

                                    if (!indVolverLlamar)
                                    {
                                        int nRegistrosVigentes = listResponseOne.Where(x => (x.CodEstado == "1")).ToList().Count;
                                        if (nRegistrosVigentes > 0)
                                        {
                                            responseOne = listResponseOne.FirstOrDefault(x => (x.CodEstado == "1")
                                                        && x.NumeroDocumentoAfiliado == numeroDocumento
                                                        && (
                                                            (codigoTipoParentesco.Equals("C") && x.CodParentesco.Equals("2"))
                                                            || (codigoTipoParentesco.Equals("H") && (x.CodParentesco.Equals("5") || x.CodParentesco.Equals("6")))
                                                        )
                                                    );
                                            if (responseOne == null)
                                            {
                                                responseOne = listResponseOne.FirstOrDefault(
                                                    x => (x.CodEstado == "1")
                                                    && x.NombresAfiliado == nombres
                                                    && x.ApellidoPaternoAfiliado == apellidoPaterno
                                                    && x.ApellidoMaternoAfiliado == apellidoMaterno
                                                    && (
                                                        (codigoTipoParentesco.Equals("C") && x.CodParentesco.Equals("2"))
                                                        || (codigoTipoParentesco.Equals("H") && (x.CodParentesco.Equals("5") || x.CodParentesco.Equals("6")))
                                                    )
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                            if (indVolverLlamar)
                            {
                                //HTTP POST
                                rqst = new ConsultAsegNom();
                                rqst.CodTipoDocumentoAfiliado = "";
                                rqst.NumeroDocumentoAfiliado = "";
                                rqst.RUC = oConfiguracionSitedsBE.rucClinica;
                                rqst.SUNASA = oConfiguracionSitedsBE.sunasa;
                                rqst.IAFAS = oUsuarioBE.iafas;
                                rqst.NombresAfiliado = nombres;
                                rqst.ApellidoPaternoAfiliado = apellidoPaterno;
                                rqst.ApellidoMaternoAfiliado = apellidoMaterno;
                                rqst.CodEspecialidad = oUsuarioBE.iafas.Equals("30004") ? "028" : "";
                                Debug.WriteLine("Request");
                                Debug.WriteLine(JsonConvert.SerializeObject(rqst).ToString());
                                content = new StringContent(JsonConvert.SerializeObject(rqst).ToString(), Encoding.UTF8, "application/json");
                                urlMetodo = "Sistema/ConsultaAsegNom";
                                responseTaskOne = client.PostAsync(urlMetodo, content);
                                responseTaskOne.Wait();

                                resultOne = responseTaskOne.Result;

                                if (resultOne.IsSuccessStatusCode)
                                {
                                    var readTaskOne = resultOne.Content.ReadAsStringAsync();
                                    readTaskOne.Wait();
                                    if (readTaskOne.Result.Length > 2 && readTaskOne.Result != "[]")
                                    {
                                        Debug.WriteLine("Response");
                                        Debug.WriteLine(readTaskOne.Result);
                                        rptaTaskOne = readTaskOne.Result;
                                        listResponseOne = JsonConvert.DeserializeObject<List<ConsultAsegNomResponse>>(rptaTaskOne);
                                        int nRegistros = listResponseOne.Count;
                                        int nRegistrosVigentes = listResponseOne.Where(x => (x.CodEstado == "1")).ToList().Count;
                                        if (nRegistrosVigentes > 0)
                                        {
                                            responseOne = listResponseOne.FirstOrDefault(x => (x.CodEstado == "1")
                                                        && x.NumeroDocumentoAfiliado == numeroDocumento
                                                        && (
                                                            (codigoTipoParentesco.Equals("C") && x.CodParentesco.Equals("2"))
                                                            || (codigoTipoParentesco.Equals("H") && (x.CodParentesco.Equals("5") || x.CodParentesco.Equals("6")))
                                                        )
                                                    );
                                            if (responseOne == null)
                                            {
                                                responseOne = listResponseOne.FirstOrDefault(
                                                    x => (x.CodEstado == "1")
                                                    && x.NombresAfiliado == nombres
                                                    && x.ApellidoPaternoAfiliado == apellidoPaterno
                                                    && x.ApellidoMaternoAfiliado == apellidoMaterno
                                                    && (
                                                        (codigoTipoParentesco.Equals("C") && x.CodParentesco.Equals("2"))
                                                        || (codigoTipoParentesco.Equals("H") && (x.CodParentesco.Equals("5") || x.CodParentesco.Equals("6")))
                                                    )
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                            listResponseOneFiltro = listResponseOne.FindAll(x => (
                                (x.CodEstado == "1")
                                &&
                                (
                                    (
                                        x.NumeroDocumentoAfiliado == numeroDocumento
                                        && (
                                            (codigoTipoParentesco.Equals("C") && x.CodParentesco.Equals("2"))
                                            || (codigoTipoParentesco.Equals("H") && (x.CodParentesco.Equals("5") || x.CodParentesco.Equals("6")))
                                        )
                                    )
                                    ||
                                    (
                                        x.NombresAfiliado == nombres
                                        && x.ApellidoPaternoAfiliado == apellidoPaterno
                                        && x.ApellidoMaternoAfiliado == apellidoMaterno
                                        && (
                                            (codigoTipoParentesco.Equals("C") && x.CodParentesco.Equals("2"))
                                            || (codigoTipoParentesco.Equals("H") && (x.CodParentesco.Equals("5") || x.CodParentesco.Equals("6")))
                                        )
                                    )
                                )
                            ));
                            ConsultAsegCod rqstTwo = new ConsultAsegCod();
                            bool existeParentesco = false;
                            foreach (ConsultAsegNomResponse responseOneTmp in listResponseOneFiltro)
                            {
                                rqstTwo.SUNASA = oConfiguracionSitedsBE.sunasa;
                                rqstTwo.IAFAS = oUsuarioBE.iafas;
                                rqstTwo.RUC = oConfiguracionSitedsBE.rucClinica;
                                rqstTwo.NombresAfiliado = responseOneTmp.NombresAfiliado;
                                rqstTwo.ApellidoPaternoAfiliado = responseOneTmp.ApellidoPaternoAfiliado;
                                rqstTwo.ApellidoMaternoAfiliado = responseOneTmp.ApellidoMaternoAfiliado;
                                rqstTwo.CodigoAfiliado = responseOneTmp.CodigoAfiliado;
                                rqstTwo.CodTipoDocumentoAfiliado = responseOneTmp.CodTipoDocumentoAfiliado;
                                rqstTwo.NumeroDocumentoAfiliado = responseOneTmp.NumeroDocumentoAfiliado;
                                rqstTwo.CodProducto = responseOneTmp.CodProducto;
                                rqstTwo.DesProducto = responseOneTmp.DesProducto;
                                rqstTwo.NumeroPlan = responseOneTmp.NumeroPlan;
                                rqstTwo.CodTipoDocumentoContratante = responseOneTmp.CodTipoDocumentoContratante;
                                rqstTwo.NumeroDocumentoContratante = responseOneTmp.NumeroDocumentoContratante;
                                rqstTwo.NombreContratante = responseOneTmp.NombreContratante;
                                rqstTwo.CodParentesco = responseOneTmp.CodParentesco;
                                rqstTwo.TipoCalificadorContratante = responseOneTmp.TipoCalificadorContratante;
                                rqstTwo.CodEspecialidad = oUsuarioBE.iafas.Equals("30004") ? "028" : "";
                                var contentTwo = new StringContent(JsonConvert.SerializeObject(rqstTwo).ToString(), Encoding.UTF8, "application/json");
                                urlMetodo = "Sistema/ConsultaAsegCod";
                                var responseTaskTwo = client.PostAsync(urlMetodo, contentTwo);
                                responseTaskTwo.Wait();

                                var resultTwo = responseTaskTwo.Result;
                                string rptaTaskTwo = "";
                                if (resultTwo.IsSuccessStatusCode)
                                {
                                    var readTaskTwo = resultTwo.Content.ReadAsStringAsync();
                                    readTaskTwo.Wait();
                                    
                                    rptaTaskTwo = readTaskTwo.Result;

                                    if (rptaTaskTwo.Length > 2 && rptaTaskTwo != "[]")
                                    {
                                        responseTwo = JsonConvert.DeserializeObject<ConsultAsegCodResponse>(rptaTaskTwo);
                                        if (responseTwo.DatosAfiliado != null
                                            && responseTwo.DatosAfiliado.CodTipoDocumentoTitular == tipoDocumentoTitular
                                            && responseTwo.DatosAfiliado.NumeroDocumentoTitular == numeroDocumentoTitular)
                                        {
                                            existeParentesco = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (existeParentesco)
                            {
                                varRespuesta = new RespuestaBE<ValidarPersonaSitedsBE>()
                                {
                                    rpt = 0,
                                    mensaje = "",
                                    data = new ValidarPersonaSitedsBE()
                                    {
                                        formatos = null,
                                        codEstadoSolicitud = "2" //Aprobado
                                    }
                                };
                            }
                            else
                            {
                                varRespuesta = new RespuestaBE<ValidarPersonaSitedsBE>()
                                {
                                    rpt = 1,
                                    mensaje = "",
                                    data = new ValidarPersonaSitedsBE()
                                    {
                                        formatos = oConfiguracionSitedsBE.formatos,
                                        codEstadoSolicitud = "1" //Pendiente de aprobación
                                    }
                                };
                            }
                        }
                    }
                    else
                    {
                        varRespuesta = new RespuestaBE<ValidarPersonaSitedsBE>()
                        {
                            rpt = 1,
                            mensaje = "",
                            data = new ValidarPersonaSitedsBE()
                            {
                                formatos = oConfiguracionSitedsBE.formatos,
                                codEstadoSolicitud = "1" //Pendiente de aprobación
                            }
                        };
                    }
                }
                else
                {
                    varRespuesta = new RespuestaBE<ValidarPersonaSitedsBE>()
                    {
                        rpt = varRespuesta2.rpt,
                        mensaje = varRespuesta2.mensaje,
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet<ValidarPersonaSitedsBE>(ex, "WS", "Usuario.svc/ValidarPersonaSiteds");
            }
            return varRespuesta;
        }
        public RespuestaBE<ValidaFamiliarBE> ValidarDocumentoFamiliar(string tipoDocumentoTitular, string numeroDocumentoTitular,
            string tipoDocumento, string numeroDocumento, string origen)
        {
            origen = !string.IsNullOrEmpty(origen) ? origen.ToLower().Trim() : "";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumentoTitular) || string.IsNullOrEmpty(numeroDocumentoTitular) || 
                string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                string.IsNullOrEmpty(origen))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaBE<ValidaFamiliarBE>()
                    {
                        rpt = 104,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            if (tipoDocumentoTitular != "1" && tipoDocumentoTitular != "2" && tipoDocumentoTitular != "3" && tipoDocumentoTitular != "4")
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 105,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumentoTitular",
                    data = null
                };
            }
            if (numeroDocumentoTitular.Length > 20)
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 106,
                    mensaje = "El parámetro numeroDocumentoTitular no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumentoTitular == "1" && numeroDocumentoTitular.Length != 8)
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 107,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            if (tipoDocumentoTitular == "1" && numeroDocumentoTitular.Length == 8)
            {
                if (!int.TryParse(numeroDocumentoTitular, out varEntero))
                {
                    return new RespuestaBE<ValidaFamiliarBE>()
                    {
                        rpt = 108,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            if (tipoDocumento == tipoDocumentoTitular && numeroDocumento == numeroDocumentoTitular)
            {
                return new RespuestaBE<ValidaFamiliarBE>()
                {
                    rpt = 109,
                    mensaje = "No se puede agregar al titular como familiar",
                    data = null
                };
            }
            #endregion
            RespuestaBE<ValidaFamiliarBE> varRespuesta = new RespuestaBE<ValidaFamiliarBE>();
            try
            {
                var oRequest = new
                {
                    tipoDocumentoTitular = tipoDocumentoTitular,
                    numeroDocumentoTitular = numeroDocumentoTitular,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    origen = origen
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ValidarDocumentoFamiliar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<ValidaFamiliarBE>>(response);
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet<ValidaFamiliarBE>(ex, "WS", "Usuario.svc/ValidarDocumentoFamiliar");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE ConfirmarParentescoFamiliar(string tipoDocumentoTitular, string numeroDocumentoTitular,
            string tipoDocumento, string numeroDocumento, string origen)
        {
            origen = !string.IsNullOrEmpty(origen) ? origen.ToLower().Trim() : "";
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumentoTitular) || string.IsNullOrEmpty(numeroDocumentoTitular) ||
                string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                string.IsNullOrEmpty(origen))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3" && tipoDocumento != "4")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento == "1" && numeroDocumento.Length != 8)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            int varEntero;
            if (tipoDocumento == "1" && numeroDocumento.Length == 8)
            {
                if (!int.TryParse(numeroDocumento, out varEntero))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 104,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            if (tipoDocumentoTitular != "1" && tipoDocumentoTitular != "2" && tipoDocumentoTitular != "3" && tipoDocumentoTitular != "4")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 105,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE), \"3\" (PAS) o \"4\" (RN) en el parámetro tipoDocumentoTitular",
                    data = null
                };
            }
            if (numeroDocumentoTitular.Length > 20)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 106,
                    mensaje = "El parámetro numeroDocumentoTitular no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumentoTitular == "1" && numeroDocumentoTitular.Length != 8)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            if (tipoDocumentoTitular == "1" && numeroDocumentoTitular.Length == 8)
            {
                if (!int.TryParse(numeroDocumentoTitular, out varEntero))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 108,
                        mensaje = "El DNI debe tener 8 dígitos",
                        data = null
                    };
                }
            }
            if (tipoDocumento == tipoDocumentoTitular && numeroDocumento == numeroDocumentoTitular)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 109,
                    mensaje = "No se puede agregar al titular como familiar",
                    data = null
                };
            }
#endregion
            RespuestaSimpleBE varRespuesta;
            try
            {
                var oRequest = new
                {
                    tipoDocumentoTitular = tipoDocumentoTitular,
                    numeroDocumentoTitular = numeroDocumentoTitular,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    origen = origen
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ConfirmarParentescoFamiliar/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
            }
            catch (Exception ex)
            {
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Usuario.svc/ConfirmarParentescoFamiliar");
            }
            return varRespuesta;
        }

        #region Helpers
        private string ToHexString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char t in str)
            {
                //Note: X for upper, x for lower case letters
                sb.Append(Convert.ToInt32(t).ToString("x2"));
            }
            return sb.ToString();


        }
        private bool fnValidarSoloTexto(string texto, bool conEspacios = true)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                for (int i = 0; i < texto.Length; i++)
                {
                    if (!(char.IsLetter(texto[i]) || (char.IsWhiteSpace(texto[i]) && conEspacios)))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        private string GetClientAddress()
        {
            // creating object of service when request comes   
            OperationContext context = OperationContext.Current;
            //Getting Incoming Message details   
            MessageProperties prop = context.IncomingMessageProperties;
            //Getting client endpoint details from message header   
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpoint.Address;
        }
        private static async Task<string> GetAccessTokenAsync()
        {
            try
            {
                string jsonString = "{" +
                        "\"type\": \"" + Env.GetString("fcm_type") + "\", " +
                        "\"project_id\": \"" + Env.GetString("fcm_project_id") + "\", " +
                        "\"private_key_id\": \"" + Env.GetString("fcm_private_key_id") + "\", " +
                        "\"private_key\": \"" + Env.GetString("fcm_private_key") + "\", " +
                        "\"client_email\": \"" + Env.GetString("fcm_client_email") + "\", " +
                        "\"client_id\": \"" + Env.GetString("fcm_client_id") + "\", " +
                        "\"auth_uri\": \"" + Env.GetString("fcm_auth_uri") + "\", " +
                        "\"token_uri\": \"" + Env.GetString("fcm_token_uri") + "\", " +
                        "\"auth_provider_x509_cert_url\": \"" + Env.GetString("fcm_auth_provider_x509_cert_url") + "\", " +
                        "\"client_x509_cert_url\": \"" + Env.GetString("fcm_client_x509_cert_url") + "\", " +
                        "\"universe_domain\": \"" + Env.GetString("fcm_universe_domain") + "\"" +
                    "}";

                byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    GoogleCredential credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(Env.GetString("fcm_google_apis"));
                    string token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                    return token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private void GrabarLog(string titulo, string origen, string servicio, string mensajeLog = "", string request = "")
        {
            var oRequest = new
            {
                titulo = titulo,
                origen = origen,
                servicio = servicio,
                mensajeLog = mensajeLog,
                request = request
            };
            string strRequest = new JavaScriptSerializer().Serialize(oRequest);
            string response = ClasesGenericas.PostAsyncIntranet("Cita.svc/GrabarLog/", strRequest);
            RespuestaSimpleBE varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(response);
        }
        #endregion

        //private string getHeader(WebHeaderCollection aHeader, string nombreHeader)
        //{
        //    string valor = aHeader[nombreHeader];
        //    valor = !string.IsNullOrEmpty(valor) ? valor : "";
        //    return valor;
        //}

        #region Métodos deshabilitados
        public RespuestaSimpleBE RecordarPassword(string tipoDocumento, string numeroDocumento)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //NO SE USA
            //try
            //{
            //    string password = ClasesGenericas.NumeroAutogenerado();
            //    string varEmail = new UsuarioDA().RecordarPassword(tipoDocumento, numeroDocumento, ClasesGenericas.ObtenerMD5(password), "m", "");
            //    Dictionary<string, string> varParametros = new Dictionary<string, string>();
            //    varParametros.Add("nombre", varEmail.Split('|')[1]);
            //    varParametros.Add("numeroDocumento", numeroDocumento);
            //    varParametros.Add("contraseña", password);
            //    bool statusEnvioEmail = ClasesGenericas.EnviarCorreo(varEmail.Split('|')[0], "Usuario_RecordarPassword", varParametros);
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = statusEnvioEmail ? "Mensaje enviado al email" : "Alerta, no se pudo enviar el email",
            //        data = varEmail.Split('|')[0]
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaBE<DataPassword> RecordarPasswordDatos(string tipoDocumento, string numeroDocumento)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //NO SE USA
            //try
            //{
            //    DataPassword varRespuesta = new UsuarioDA().RecordarPasswordDatos(tipoDocumento, numeroDocumento);
            //    //Debug.WriteLine(varRespuesta);
            //    DataPassword verify_strings = new DataPassword();

            //    string email_verify = varRespuesta.email;
            //    string celular_verify = varRespuesta.celular;

            //    bool isEmail = Regex.IsMatch(email_verify, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);


            //    if (isEmail)
            //    {
            //        verify_strings.email = email_verify;
            //    }
            //    else
            //    {
            //        verify_strings.email = null;
            //    }

            //    if (celular_verify.Length == 9)
            //    {
            //        if (celular_verify.Substring(0, 1) == "9")
            //        {
            //            verify_strings.celular = celular_verify;
            //        }
            //        else
            //        {
            //            verify_strings.celular = null;
            //        }

            //    }
            //    else
            //    {
            //        verify_strings.celular = null;
            //    }
            //    Debug.WriteLine(celular_verify.Length);

            //    return new RespuestaBE<DataPassword>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new DataPassword() { celular = verify_strings.celular, email = verify_strings.email }
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<DataPassword>(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaSimpleBE CambiarPassword(string password)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //NO SE USA
            //try
            //{
            //    string token = WebOperationContext.Current.IncomingRequest.Headers["token"];

            //    new UsuarioDA().CambiarPassword(token, ClasesGenericas.ObtenerMD5(password));
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = ""
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaSimpleBE CargarImagen(string tipoDocumento, string numeroDocumento, string imagen)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 101,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    string rutaLocal = ConfigurationManager.AppSettings["RutaImagenes"].ToString();
            //    string urlBase = ConfigurationManager.AppSettings["URLImagenes"].ToString();
            //    var nombreImagen = tipoDocumento + "_" + numeroDocumento;

            //    try
            //    {
            //        if (File.Exists(Path.Combine(rutaLocal, nombreImagen + ".jpg")))
            //        {
            //            File.Delete(Path.Combine(rutaLocal, nombreImagen + ".jpg"));
            //            Debug.WriteLine("Delete JPG");
            //        }
            //        else if (File.Exists(Path.Combine(rutaLocal, nombreImagen + ".png")))
            //        {
            //            File.Delete(Path.Combine(rutaLocal, nombreImagen + ".png"));
            //            Debug.WriteLine("Delete PNG");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine("Error " + e);

            //    }


            //    string varImagenData = Regex.Match(imagen, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            //    byte[] varImagenDesencriptada = Convert.FromBase64String(varImagenData);
            //    Image varImagen;
            //    using (MemoryStream varStream = new MemoryStream(varImagenDesencriptada))
            //    {
            //        if (varStream.Length >= int.Parse(ConfigurationManager.AppSettings["PesoMaximo"].ToString()))
            //        {
            //            return new RespuestaSimpleBE()
            //            {
            //                rpt = 103,
            //                mensaje = "El tamaño de la imagen (" + varStream.Length + ") supera el tamaño máximo permitido (" + ConfigurationManager.AppSettings["PesoMaximo"].ToString() + ")",
            //                data = null
            //            };
            //        }

            //        varImagen = new Bitmap(varStream);
            //    }




            //    string varExtension = "jpg";
            //    if (varImagen.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
            //        varExtension = "png";
            //    else if (!varImagen.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            //        return new RespuestaSimpleBE()
            //        {
            //            rpt = 104,
            //            mensaje = "La imagen no tiene los formatos soportados (\"jpg\" o \"png\")",
            //            data = null
            //        };

            //    string varNombreArchivo = tipoDocumento + "_" + numeroDocumento + "." + varExtension;
            //    File.WriteAllBytes(Path.Combine(ConfigurationManager.AppSettings["RutaImagenes"].ToString(), varNombreArchivo), varImagenDesencriptada);
            //    Debug.WriteLine("Img nueva : " + varNombreArchivo);

            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = ConfigurationManager.AppSettings["URLImagenes"].ToString() + varNombreArchivo
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaSimpleBE PruebaEmail()
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //try
            //{
            //    Dictionary<string, string> varParametrosCorreo = new Dictionary<string, string>();
            //    //varParametrosCorreo["Email"] = "jerson.huaytac@gmail.com";
            //    varParametrosCorreo.Add("Email", "jerson.huaytac@gmail.com");
            //    bool statusEnvioEmail = ClasesGenericas.EnviarCorreo(varParametrosCorreo["Email"], "Cita_RegistrarCitaVirtual", varParametrosCorreo, null, "SMTPVirtual");

            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = (statusEnvioEmail) ? "SUCCESS" : "ERROR",
            //        data = null
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaBE<PacienteCovid> EncuestaPacienteCovid(string tipoDocumento, string numeroDocumento)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //bool pacienteCovid = new UsuarioDA().EncuestaPacienteCovid(tipoDocumento, numeroDocumento);
            //var respuesta = new PacienteCovid();
            //if (pacienteCovid)
            //{
            //    respuesta.esPacienteCovid = true;
            //    respuesta.backgroundBannerHex = "#f7ba00";
            //    respuesta.backgroundBannerRgb = new BackgroundBannerRgb()
            //    {
            //        red = 247,
            //        green = 186,
            //        blue = 0
            //    };
            //    respuesta.fontBannerHex = "#FFFFFF";
            //    respuesta.fontBannerRgb = new FontBannerRgb()
            //    {
            //        red = 255,
            //        green = 255,
            //        blue = 255
            //    };
            //    respuesta.mensaje = "Paciente Covid-19 >>> Registre sus sintomas";
            //}
            //else
            //{
            //    respuesta.backgroundBannerHex = "#FF0708";
            //    respuesta.backgroundBannerRgb = new BackgroundBannerRgb()
            //    {
            //        red = 255,
            //        green = 0,
            //        blue = 0
            //    };
            //    respuesta.fontBannerHex = "#FFFFFF";
            //    respuesta.fontBannerRgb = new FontBannerRgb()
            //    {
            //        red = 255,
            //        green = 255,
            //        blue = 255
            //    };
            //    respuesta.mensaje = "Paciente Covid-19 >>> Registre sus sintomas";
            //    respuesta.esPacienteCovid = false;
            //}
            //try
            //{

            //    return new RespuestaBE<PacienteCovid>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = respuesta
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<PacienteCovid>(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaSimpleBE ActualizarCorreoUsuario(string tipoDocumento, string numeroDocumento, string email)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(email))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 101,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            //if (email.Length > 50)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 110,
            //        mensaje = "El parámetro email no puede tener más de 50 caracteres",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    new UsuarioDA().ActualizarCorreoUsuario(tipoDocumento, numeroDocumento, email);
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = null
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaSimpleBE CambiarClave(string tipoDocumento, string numeroDocumento, string password)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //try
            //{
            //    string token = WebOperationContext.Current.IncomingRequest.Headers["token"];
            //    //Enviar correo
            //    new UsuarioDA().CambiarPassword(token, password);
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = ""
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Usuario.svc");
            //}
        }
        public RespuestaBE<List<MedicoFavoritoBE>> ListarMedicosFavoritos(string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<List<MedicoFavoritoBE>>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<List<MedicoFavoritoBE>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoFavoritoBE>> varRespuesta = new RespuestaBE<List<MedicoFavoritoBE>>();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ListarMedicosFavoritos/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoFavoritoBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoFavoritoBE>>(ex, "WS", "Usuario.svc");
            }
            return varRespuesta;
        }
        #endregion
    }
}
