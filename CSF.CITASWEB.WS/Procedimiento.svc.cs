using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSF.CITASWEB.WS.BE;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Net;
//using CSF.CITASWEB.WS.DA;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using CSF.CITASWEB.WS.wsprecisa;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Twilio.Jwt.AccessToken;
using System.Reflection;
using System.Threading;
using chn = System.ServiceModel.Channels;

namespace CSF.CITASWEB.WS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Procedimiento : IProcedimiento
    {
        public RespuestaBE<List<PROC_ListarCptResponseBE>> ListarCpt(string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || (string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            #endregion
            try
            {
                RespuestaBE<List<PROC_ListarCptResponseBE>> oResponse = new RespuestaBE<List<PROC_ListarCptResponseBE>>();
                string tokenSesion = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token");
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
                    return new RespuestaBE<List<PROC_ListarCptResponseBE>>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuestaTmp.rpt != 0)
                {
                    return new RespuestaBE<List<PROC_ListarCptResponseBE>>()
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
                    return new RespuestaBE<List<PROC_ListarCptResponseBE>>()
                    {
                        rpt = 102,
                        mensaje = "Usuario inválido",
                        data = null
                    };
                    //oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    //return null;
                }
                #region Integración con San Felipe
                QR_RespuestaSimpleBE oRespuestaSimpleBE = new QR_RespuestaSimpleBE();
                QR_RespuestaBE<List<PROC_ListarCptResponseBE>> oRespuestaQRBE = new QR_RespuestaBE<List<PROC_ListarCptResponseBE>>();

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
                            urlMetodo = "AgendaCita/ListarCpt";
                            var responseTaskTwo = client.GetAsync(urlMetodo);
                            responseTaskTwo.Wait();

                            var resultTwo = responseTaskTwo.Result;

                            string rptaTaskTwo = "";
                            if (resultTwo.IsSuccessStatusCode)
                            {
                                var readTaskTwo = resultTwo.Content.ReadAsStringAsync();
                                readTaskTwo.Wait();
                                rptaTaskTwo = readTaskTwo.Result;
                                oRespuestaQRBE = JsonConvert.DeserializeObject<QR_RespuestaBE<List<PROC_ListarCptResponseBE>>>(rptaTaskTwo);
                                if (oRespuestaQRBE.success)
                                {
                                    oResponse.rpt = 0;
                                    oResponse.data = oRespuestaQRBE.result;
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
                                oResponse.mensaje = "ListarCpt: Error en comunicación";
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

                return oResponse;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<PROC_ListarCptResponseBE>>(ex, "WS", "Cita.svc/ListarCpt");
                return ClasesGenericas.RegistrarErrorIntranet<List<PROC_ListarCptResponseBE>>(ex, "WS", "Procedimiento.svc/ListarCpt");
            }
        }

        public RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>> ListarAnestesiologo(string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || (string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            #endregion
            try
            {
                RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>> oResponse = new RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>>();
                string tokenSesion = ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token");
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
                    return new RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuestaTmp.rpt != 0)
                {
                    return new RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>>()
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
                    return new RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>>()
                    {
                        rpt = 102,
                        mensaje = "Usuario inválido",
                        data = null
                    };
                    //oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    //return null;
                }
                #region Integración con San Felipe
                QR_RespuestaSimpleBE oRespuestaSimpleBE = new QR_RespuestaSimpleBE();
                QR_RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>> oRespuestaQRBE = new QR_RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>>();

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
                            urlMetodo = "AgendaCita/ListarAnestesiologo";
                            var responseTaskTwo = client.GetAsync(urlMetodo);
                            responseTaskTwo.Wait();

                            var resultTwo = responseTaskTwo.Result;

                            string rptaTaskTwo = "";
                            if (resultTwo.IsSuccessStatusCode)
                            {
                                var readTaskTwo = resultTwo.Content.ReadAsStringAsync();
                                readTaskTwo.Wait();
                                rptaTaskTwo = readTaskTwo.Result;
                                oRespuestaQRBE = JsonConvert.DeserializeObject<QR_RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>>>(rptaTaskTwo);
                                if (oRespuestaQRBE.success)
                                {
                                    oResponse.rpt = 0;
                                    oResponse.data = oRespuestaQRBE.result;
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
                                oResponse.mensaje = "ListarAnestesiologo: Error en comunicación";
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

                return oResponse;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<PROC_ListarAnestesiologoResponseBE>>(ex, "WS", "Cita.svc/ListarAnestesiologo");
                return ClasesGenericas.RegistrarErrorIntranet<List<PROC_ListarAnestesiologoResponseBE>>(ex, "WS", "Procedimiento.svc/ListarAnestesiologo");
            }
        }

        public RespuestaSimpleBE RegistrarProcedimiento(string tipoDocumento, string numeroDocumento, string idClinica,
                                        string codMedico, string idSubespecialidad, string idServicio,
                                        string codAtencion, string codTipoPaciente, string fecha,
                                        string horaInicio, string horaFin, string duracion,
                                        string codCpt, string cpt, string codSegus,
                                        string segus, string guarismo, string rucSeguro,
                                        string iafas, bool flgCartaGarantia, string cartaGarantia,
                                        bool flgPresupuesto, string presupuesto, string celular,
                                        bool flgAlergia, string coaseguro, string correo,
                                        string observacion, string origen, string codEstado,
                                        string idOrdenDetalle, string totalSesiones)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idClinica)
                || string.IsNullOrEmpty(idSubespecialidad) || string.IsNullOrEmpty(idServicio) || string.IsNullOrEmpty(codAtencion)
                || string.IsNullOrEmpty(origen) || string.IsNullOrEmpty(codEstado) || string.IsNullOrEmpty(idOrdenDetalle))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            //idServicio: 6 (Medicina del deporte)
            if (idServicio == "6")
            {
                totalSesiones = !String.IsNullOrEmpty(totalSesiones) ? totalSesiones : "1";
                int varTotalSesiones;
                if (!int.TryParse(totalSesiones, out varTotalSesiones))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 103,
                        mensaje = "El parámetro totalSesiones debe ser numérico",
                        data = null
                    };
                }
                if (varTotalSesiones < 1)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 103,
                        mensaje = "El parámetro totalSesiones debe ser mayor a 0",
                        data = null
                    };
                }
            }
            else
            {
                totalSesiones = null;
                if (string.IsNullOrEmpty(codMedico) || string.IsNullOrEmpty(codCpt) || string.IsNullOrEmpty(cpt)
                    || string.IsNullOrEmpty(codSegus) || string.IsNullOrEmpty(segus))
                {
                    throw new WebFaultException(HttpStatusCode.BadRequest);
                }
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
            if (!string.IsNullOrEmpty(fecha))
            {
                DateTime varFecha;
                if (!DateTime.TryParse(fecha, out varFecha))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 103,
                        mensaje = "El parámetro fecha no tiene el formato correcto",
                        data = null
                    };
                }
            }
            if (!string.IsNullOrEmpty(guarismo))
            {
                int varGuarismo;
                if (!int.TryParse(guarismo, out varGuarismo))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 104,
                        mensaje = "El parámetro guarismo no tiene el formato correcto",
                        data = null
                    };
                }
            }
            int idEstado;
            if (!int.TryParse(codEstado, out idEstado))
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 105,
                    mensaje = "El parámetro codEstado no tiene el formato correcto",
                    data = null
                };
            }
            if (idEstado != 1)
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 106,
                    mensaje = "Sólo se soportan los valores \"1\" (Pendiente) en el parámetro codEstado",
                    data = null
                };
            }
            origen = origen.Trim().ToUpper();
            if (origen != "HCE")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 107,
                    mensaje = "Sólo se soportan los valores \"HCE\" (Historia Clínica Electrónica) en el parámetro origen",
                    data = null
                };
            }
            #endregion
            try
            {
                var oRequestTmp = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    idClinica = idClinica,
                    codMedico = codMedico,
                    idSubespecialidad = idSubespecialidad,
                    idServicio = idServicio,
                    codAtencion = codAtencion,
                    codTipoPaciente = codTipoPaciente,
                    fecha = fecha,
                    horaInicio = horaInicio,
                    horaFin = horaFin,
                    duracion = duracion,
                    codCpt = codCpt,
                    cpt = cpt,
                    codSegus = codSegus,
                    segus = segus,
                    guarismo = guarismo,
                    rucSeguro = rucSeguro,
                    iafas = iafas,
                    flgCartaGarantia = flgCartaGarantia,
                    cartaGarantia = cartaGarantia,
                    flgPresupuesto = flgPresupuesto,
                    presupuesto = presupuesto,
                    celular = celular,
                    flgAlergia = flgAlergia,
                    coaseguro = coaseguro,
                    correo = correo,
                    observacion = observacion,
                    origen = origen,
                    codEstado = codEstado,
                    idOrdenDetalle = idOrdenDetalle
                };
                string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                string responseTmp = ClasesGenericas.PostAsyncIntranet("Procedimiento.svc/RegistrarProcedimiento/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaSimpleBE varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(responseTmp);

                if (varRespuestaTmp == null)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }

                return varRespuestaTmp;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Cita.svc");
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Procedimiento.svc/RegistrarProcedimiento");
            }
        }

        public RespuestaSimpleBE ActualizarProcedimiento(string tipoDocumento, string numeroDocumento, string idOrdenDetalle,
                                        string codCpt, string cpt, string codSegus,
                                        string segus, string guarismo, string origen)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idOrdenDetalle)
                || string.IsNullOrEmpty(codCpt) || string.IsNullOrEmpty(cpt) || string.IsNullOrEmpty(codSegus)
                || string.IsNullOrEmpty(segus) || string.IsNullOrEmpty(origen))
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
            if (!string.IsNullOrEmpty(guarismo))
            {
                int varGuarismo;
                if (!int.TryParse(guarismo, out varGuarismo))
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = 103,
                        mensaje = "El parámetro guarismo no tiene el formato correcto",
                        data = null
                    };
                }
            }
            origen = origen.Trim().ToUpper();
            if (origen != "HCE")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 104,
                    mensaje = "Sólo se soportan los valores \"HCE\" (Historia Clínica Electrónica) en el parámetro origen",
                    data = null
                };
            }
            #endregion
            try
            {
                var oRequestTmp = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    idOrdenDetalle = idOrdenDetalle,
                    codCpt = codCpt,
                    cpt = cpt,
                    codSegus = codSegus,
                    segus = segus,
                    guarismo = guarismo,
                    origen = origen
                };
                string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                string responseTmp = ClasesGenericas.PostAsyncIntranet("Procedimiento.svc/ActualizarProcedimiento/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaSimpleBE varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(responseTmp);

                if (varRespuestaTmp == null)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }

                return varRespuestaTmp;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Cita.svc");
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Procedimiento.svc/ActualizarProcedimiento");
            }
        }


        public RespuestaSimpleBE AnularProcedimiento(string tipoDocumento, string numeroDocumento, string idOrdenDetalle,
                                        string origen)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idOrdenDetalle)
                || string.IsNullOrEmpty(origen))
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
            origen = origen.Trim().ToUpper();
            if (origen != "HCE")
            {
                return new RespuestaSimpleBE()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"HCE\" (Historia Clínica Electrónica) en el parámetro origen",
                    data = null
                };
            }
            #endregion
            try
            {
                var oRequestTmp = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    idOrdenDetalle = idOrdenDetalle,
                    origen = origen
                };
                string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                string responseTmp = ClasesGenericas.PostAsyncIntranet("Procedimiento.svc/AnularProcedimiento/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaSimpleBE varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaSimpleBE>(responseTmp);

                if (varRespuestaTmp == null)
                {
                    return new RespuestaSimpleBE()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }

                return varRespuestaTmp;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Cita.svc");
                return ClasesGenericas.RegistrarErrorIntranet(ex, "WS", "Procedimiento.svc/AnularProcedimiento");
            }
        }
    }
}
