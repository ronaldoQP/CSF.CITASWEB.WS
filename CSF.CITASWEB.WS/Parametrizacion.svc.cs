using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using CSF.CITASWEB.WS.BE;
//using CSF.CITASWEB.WS.DA;
using System.ServiceModel.Web;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Drawing;
using System.IO;
//using tessnet2;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Configuration;

namespace CSF.CITASWEB.WS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Parametrizacion : IParametrizacion
    {
        public RespuestaBE<VersionAplicacionBE> IniciarAplicacion(string tipoDispositivo)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDispositivo))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDispositivo.ToLower() != "android" && tipoDispositivo.ToLower() != "ios" && tipoDispositivo.ToLower() != "huawei")
            {
                return new RespuestaBE<VersionAplicacionBE>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"android\", \"iOS\" o \"huawei\" en el parámetro tipoDispositivo",
                    data = null
                };
            }
            #endregion
            RespuestaBE<VersionAplicacionBE> varRespuesta = new RespuestaBE<VersionAplicacionBE>();
            try
            {
                Debug.WriteLine("Incio Aplicacion");
                Console.WriteLine("inicio app");
                var oRequest = new
                {
                    tipoDispositivo = tipoDispositivo
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/IniciarAplicacion/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<VersionAplicacionBE>>(response);
            }
            catch (Exception ex)
            {
                // return new ErrorDA().RegistrarError<VersionAplicacionBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<ParametroBE>> ListarParametros()
        {
            RespuestaBE<List<ParametroBE>> varRespuesta = new RespuestaBE<List<ParametroBE>>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ListarParametros/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ParametroBE>>>(response);
            }
            catch (Exception ex)
            {
                // return new ErrorDA().RegistrarError<List<ParametroBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<SeguroBE>> ListarSeguros()
        {
            RespuestaBE<List<SeguroBE>> varRespuesta = new RespuestaBE<List<SeguroBE>>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ListarSeguros/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<SeguroBE>>>(response);
            }
            catch (Exception ex)
            {
                // return new ErrorDA().RegistrarError<List<SeguroBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<TerminosBE> ObtenerTerminos(string tipo)
        {
            RespuestaBE<TerminosBE> varRespuesta = new RespuestaBE<TerminosBE>();
            try
            {
                var oRequest = new
                {
                    tipo = tipo
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerTerminos/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<TerminosBE>>(response);
            }
            catch (Exception ex)
            {
                // return new ErrorDA().RegistrarError<TerminosBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<DataRuc> DatosPorRuc(string ruc)
        {
            RespuestaBE<DataRuc> varRespuesta = null;
            try
            {
                if (ruc.Length != 11)
                {
                    return new RespuestaBE<DataRuc>()
                    {
                        rpt = 1,
                        mensaje = "RUC debe tener 11 dígitos",
                        data = null
                    };
                }

                string urlBaseMigo = ConfigurationManager.AppSettings["WS_Migo"].ToString();
                string tokenMigoRUC = ConfigurationManager.AppSettings["Token_MigoRUC"].ToString();
                string urlMetodo = "ruc";

                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.BaseAddress = new Uri(urlBaseMigo);
                    //HTTP POST
                    RequestMigoRUCBE oRequestMigoRUCBE = new RequestMigoRUCBE();
                    oRequestMigoRUCBE.ruc = ruc;
                    oRequestMigoRUCBE.token = tokenMigoRUC;

                    string contenidoJson = new JavaScriptSerializer().Serialize(oRequestMigoRUCBE);
                    var content = new StringContent(contenidoJson, Encoding.UTF8, "application/json");
                    var responseWS = client.PostAsync(urlMetodo, content);
                    responseWS.Wait();

                    var responseMigo = responseWS.Result;

                    var response = responseMigo.Content.ReadAsStringAsync();
                    response.Wait();
                    string strResponse = response.Result;

                    if (responseMigo.IsSuccessStatusCode)
                    {
                        ResponseMigoRUCBE oResponseMigoRUCBE = new JavaScriptSerializer().Deserialize<ResponseMigoRUCBE>(strResponse);
                        if (oResponseMigoRUCBE != null)
                        {
                            if (oResponseMigoRUCBE.success)
                            {
                                DataRuc oDataRuc = new DataRuc();
                                oDataRuc.Ruc = ruc;
                                oDataRuc.RazonSocial = oResponseMigoRUCBE.nombre_o_razon_social;
                                oDataRuc.Estado = oResponseMigoRUCBE.estado_del_contribuyente;
                                oDataRuc.Condicion = oResponseMigoRUCBE.condicion_de_domicilio;
                                oDataRuc.Direccion = oResponseMigoRUCBE.direccion_simple;
                                oDataRuc.Departamento = oResponseMigoRUCBE.departamento;
                                oDataRuc.Provincia = oResponseMigoRUCBE.provincia;
                                oDataRuc.Distrito = oResponseMigoRUCBE.distrito;
                                varRespuesta = new RespuestaBE<DataRuc>()
                                {
                                    rpt = 0,
                                    mensaje = "",
                                    data = oDataRuc
                                };
                            }
                        }
                    }
                }
                if (varRespuesta == null)
                {
                    varRespuesta = new RespuestaBE<DataRuc>()
                    {
                        rpt = 1,
                        mensaje = "El RUC ingresado no existe, por favor verifique",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ClasesGenericas.RegistrarErrorIntranet<DataRuc>(ex, "WS", "Cita.svc/DatosPorRuc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<UbigeoBE>> ObtenerUbigeo(string distrito)
        {
            RespuestaBE<List<UbigeoBE>> varRespuesta = new RespuestaBE<List<UbigeoBE>>();
            try
            {
                var oRequest = new
                {
                    distrito = distrito
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerUbigeo/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<UbigeoBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<UbigeoBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<IndicadorPopUpHomeBE> ObtenerIndicadorPopUpHome()
        {
            try
            {
                //return new RespuestaBE<IndicadorPopUpHomeBE>()
                //{
                //    rpt = 0,
                //    mensaje = "",
                //    data = new ParametrizacionDA().ObtenerIndicadorPopUpHome()
                //};
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerIndicadorPopUpHome/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<IndicadorPopUpHomeBE> oResponse = new JavaScriptSerializer().Deserialize<RespuestaBE<IndicadorPopUpHomeBE>>(response);
                if (oResponse == null)
                {
                    return new RespuestaBE<IndicadorPopUpHomeBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                return oResponse;
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<IndicadorPopUpHomeBE>(ex, "WS", "Parametrizacion.svc");
                return ClasesGenericas.RegistrarErrorIntranet<IndicadorPopUpHomeBE>(ex, "WS", "Parametrizacion.svc/ObtenerIndicadorPopUpHome");
            }
        }
        public RespuestaBE<RUDatosBE> ParametrosRegistrarUsuario()
        {
            RespuestaBE<RUDatosBE> varRespuesta = new RespuestaBE<RUDatosBE>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ParametrosRegistrarUsuario/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<RUDatosBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<RUDatosBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<PaisBE>> ListarPaises()
        {
            RespuestaBE<List<PaisBE>> varRespuesta = new RespuestaBE<List<PaisBE>>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ListarPaises/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<PaisBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<PaisBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<DepartamentoBE>> ListarDepartamentos(string idPais)
        {
            RespuestaBE<List<DepartamentoBE>> varRespuesta = new RespuestaBE<List<DepartamentoBE>>();
            try
            {
                idPais = !String.IsNullOrEmpty(idPais) ? idPais : "000";
                var oRequest = new
                {
                    idPais = idPais
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ListarDepartamentos/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<DepartamentoBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<DepartamentoBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<ProvinciaBE>> ListarProvincias(string idPais, string idDepartamento)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(idDepartamento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            RespuestaBE<List<ProvinciaBE>> varRespuesta = new RespuestaBE<List<ProvinciaBE>>();
            try
            {
                idPais = !String.IsNullOrEmpty(idPais) ? idPais : "000";
                var oRequest = new
                {
                    idPais = idPais,
                    idDepartamento = idDepartamento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ListarProvincias/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ProvinciaBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<ProvinciaBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<DistritoBE>> ListarDistritos(string idPais, string idDepartamento, string idProvincia)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(idDepartamento) || string.IsNullOrEmpty(idProvincia))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            RespuestaBE<List<DistritoBE>> varRespuesta = new RespuestaBE<List<DistritoBE>>();
            try
            {
                idPais = !String.IsNullOrEmpty(idPais) ? idPais : "000";
                var oRequest = new
                {
                    idPais = idPais,
                    idDepartamento = idDepartamento,
                    idProvincia = idProvincia
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ListarDistritos/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<DistritoBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<DistritoBE>>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<TextoBE> ObtenerTexto(string codigo)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(codigo))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            RespuestaBE<TextoBE> varRespuesta = new RespuestaBE<TextoBE>();
            try
            {
                var oRequest = new
                {
                    codigo = codigo
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerTexto/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<TextoBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<TextoBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<DataDocumento> DatosPorDocumento(string tipoDocumento, string numeroDocumento)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<DataDocumento>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<DataDocumento>()
                {
                    rpt = 102,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            if (tipoDocumento.Equals("1") && numeroDocumento.Length != 8)
            {
                return new RespuestaBE<DataDocumento>()
                {
                    rpt = 103,
                    mensaje = "El DNI debe tener 8 dígitos",
                    data = null
                };
            }
            #endregion
            RespuestaBE<DataDocumento> varRespuesta = new RespuestaBE<DataDocumento>();
            try
            {
                var oRequest = new
                {
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/DatosPorDocumento/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<DataDocumento>>(response);
                if (varRespuesta.rpt != 0)
                {
                    string urlBasePersona = ConfigurationManager.AppSettings["WS_Persona"].ToString();
                    string urlMetodo = "usuario/login";

                    using (var client = new HttpClient())
                    {
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.BaseAddress = new Uri(urlBasePersona);
                        //HTTP POST
                        RequestLoginPersonaBE oRequestLogin = new RequestLoginPersonaBE();
                        oRequestLogin.usuario = ConfigurationManager.AppSettings["WS_Persona_Usuario"].ToString();
                        oRequestLogin.password = ConfigurationManager.AppSettings["WS_Persona_Clave"].ToString();

                        string contenidoJson = new JavaScriptSerializer().Serialize(oRequestLogin);
                        var content = new StringContent(contenidoJson, Encoding.UTF8, "application/json");
                        var responseWS = client.PostAsync(urlMetodo, content);
                        responseWS.Wait();

                        var responseMigo = responseWS.Result;

                        var response2 = responseMigo.Content.ReadAsStringAsync();
                        response2.Wait();
                        string strResponse = response2.Result;

                        if (responseMigo.IsSuccessStatusCode)
                        {
                            ResponseLoginPersonaBE oResponseLogin = new JavaScriptSerializer().Deserialize<ResponseLoginPersonaBE>(strResponse);
                            if (oResponseLogin != null)
                            {
                                if (oResponseLogin.success)
                                {
                                    urlMetodo = String.Format("AgendaCita/GetDatoPersona?Tipdocidentidad={0}&Docidentidad={1}", tipoDocumento, numeroDocumento);
                                    DataDocumento oDataDocumento = new DataDocumento();
                                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + oResponseLogin.result); //Token
                                    responseWS = client.GetAsync(urlMetodo);
                                    responseWS.Wait();

                                    var responsePersona = responseWS.Result;

                                    response2 = responsePersona.Content.ReadAsStringAsync();
                                    response2.Wait();
                                    strResponse = response2.Result;
                                    bool indPersonaEncontrada = false;

                                    if (responsePersona.IsSuccessStatusCode)
                                    {
                                        ResponsePersonaBE oResponsePersona = new JavaScriptSerializer().Deserialize<ResponsePersonaBE>(strResponse);
                                        if (oResponsePersona != null)
                                        {
                                            int i = 0, nPersonas = oResponsePersona.persona.Count;
                                            for (; i < nPersonas; i++)
                                            {
                                                oDataDocumento.nombres = oResponsePersona.persona[i].nombre;
                                                oDataDocumento.apellidoPaterno = oResponsePersona.persona[i].apPaterno;
                                                oDataDocumento.apellidoMaterno = oResponsePersona.persona[i].apMaterno;
                                                oDataDocumento.email = oResponsePersona.persona[i].correo;
                                                oDataDocumento.celular = oResponsePersona.persona[i].celular;
                                                oDataDocumento.direccion = oResponsePersona.persona[i].direccion;
                                                oDataDocumento.genero = "";
                                                oDataDocumento.fechaNacimiento = "";
                                                indPersonaEncontrada = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (indPersonaEncontrada)
                                    {
                                        return new RespuestaBE<DataDocumento>()
                                        {
                                            rpt = 0,
                                            mensaje = "",
                                            data = oDataDocumento
                                        };
                                    }
                                    else
                                    {
                                        return new RespuestaBE<DataDocumento>()
                                        {
                                            rpt = 1,
                                            mensaje = "No se encontraron datos de la persona",
                                            data = null
                                        };
                                    }
                                }
                                else
                                {
                                    return new RespuestaBE<DataDocumento>()
                                    {
                                        rpt = 1,
                                        mensaje = !String.IsNullOrEmpty(oResponseLogin.message) ? oResponseLogin.message : "No se pudo generar el token",
                                        data = null
                                    };
                                }
                            }
                        }
                    }
                    return new RespuestaBE<DataDocumento>()
                    {
                        rpt = 1,
                        mensaje = "No se encontraron datos con el tipo/número de documento",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<DataDocumento>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<HODatosBE> ObtenerParametrosHospitalizacion()
        {
            RespuestaBE<HODatosBE> varRespuesta = new RespuestaBE<HODatosBE>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerParametrosHospitalizacion/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<HODatosBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HODatosBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<ENDatosBE> ObtenerParametrosEnlaces()
        {
            RespuestaBE<ENDatosBE> varRespuesta = new RespuestaBE<ENDatosBE>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerParametrosEnlaces/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<ENDatosBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<ENDatosBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaSimpleBE ObtenerDisponibilidadWS()
        {
            RespuestaSimpleBE varRespuesta = new RespuestaSimpleBE();
            try
            {
                bool indDisponibilidadWS = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["indicadorDisponibilidadWS"]) ? "0" : ConfigurationManager.AppSettings["indicadorDisponibilidadWS"]).Equals("1");
                varRespuesta = new RespuestaSimpleBE()
                {
                    rpt = 0,
                    mensaje = indDisponibilidadWS ? "Servicio disponible" : "Servicio no disponible",
                    data = indDisponibilidadWS ? "1" : "0"
                };
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError(ex, "WS", "Parametrizacion.svc/ObtenerDisponibilidadWS/");
            }
            return varRespuesta;
        }
        public RespuestaBE<ContenidoBE> ObtenerContenido(string codigo)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(codigo))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            RespuestaBE<ContenidoBE> varRespuesta = new RespuestaBE<ContenidoBE>();
            try
            {
                var oRequest = new
                {
                    codigo = codigo
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerContenido/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<ContenidoBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<ContenidoBE>(ex, "WS", "Parametrizacion.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<ParametroSeguridadBE> ObtenerParametroSeguridad(string tipoRegistro)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoRegistro))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            tipoRegistro = !string.IsNullOrEmpty(tipoRegistro) ? tipoRegistro : "2"; //1: Parámetros de Seguridad y 2: Políticas de contraseña
            #endregion
            try
            {
                var oRequest = new
                {
                    tipoRegistro = tipoRegistro
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerParametroSeguridad/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<ParametroSeguridadBE> varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<ParametroSeguridadBE>>(response);
                if (varRespuesta == null)
                {
                    return new RespuestaBE<ParametroSeguridadBE>()
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
                //return new ErrorDA().RegistrarError<ParametroSeguridadBE>(ex, "WS", "Parametrizacion.svc");
                return ClasesGenericas.RegistrarErrorIntranet<ParametroSeguridadBE>(ex, "WS", "Parametrizacion.svc/ObtenerParametroSeguridad");
            }
        }

    }
}