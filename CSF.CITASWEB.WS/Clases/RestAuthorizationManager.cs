using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.IO;
//using CSF.CITASWEB.WS.DA;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using CSF.CITASWEB.WS.BE;
using System.Net.Http.Headers;

namespace CSF.CITASWEB.WS
{
    public class RestAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            try
            {
                bool varPaginaPublica = false;
                
                foreach (string paginaPublica in ConfigurationManager.AppSettings["PaginasPublicas"].ToString().Split(';'))
                {
                    if (!string.IsNullOrEmpty(paginaPublica)){
                    
                        
                        if (System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.LocalPath.EndsWith(paginaPublica))
                        {
                            varPaginaPublica = true;
                            break;
                        }
                    }
                }
                if (varPaginaPublica)
                {
                    return true;
                }
                else
                {
                    string token = WebOperationContext.Current.IncomingRequest.Headers["token"];
                    if (token != null)//token != null && new UsuarioDA().ValidarToken(token))
                    {
                        string urlBase = "", urlMetodo = "";
                        using (var client = new HttpClient())
                        {
                            urlBase = ClasesGenericas.GetSetting("WS_Intranet");
                            client.BaseAddress = new Uri(urlBase);
                            //HTTP POST
                            var oRequest = new
                            {
                                tipoDocumento = "",
                                numeroDocumento = ""
                            };
                            var content = new StringContent(JsonConvert.SerializeObject(oRequest).ToString(), Encoding.UTF8, "application/json");
                            urlMetodo = "Usuario.svc/ValidarToken/";
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Add("User-Agent", "CSF_CITAS_WEB_WS/" + ClasesGenericas.GetSetting("_VersionApp"));
                            client.DefaultRequestHeaders.Add("Accept", "*/*");
                            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                            client.DefaultRequestHeaders.Accept.Add(
                                 new MediaTypeWithQualityHeaderValue("application/json")
                             );
                            client.DefaultRequestHeaders.Add("token", token);
                            var responseTask = client.PostAsync(urlMetodo, content);
                            responseTask.Wait();
                            var resultOne = responseTask.Result;
                            string response = "";
                            if (resultOne.StatusCode.Equals(HttpStatusCode.Unauthorized))
                            {
                                throw new WebFaultException(HttpStatusCode.Unauthorized);
                            } 
                            else if (resultOne.StatusCode.Equals(HttpStatusCode.BadRequest))
                            {
                                throw new WebFaultException(HttpStatusCode.BadRequest);
                            }
                            else if (resultOne.StatusCode.Equals(HttpStatusCode.InternalServerError))
                            {
                                throw new WebFaultException(HttpStatusCode.InternalServerError);
                            }
                            else if (resultOne.IsSuccessStatusCode)
                            {
                                var readTask = resultOne.Content.ReadAsStringAsync();
                                readTask.Wait();
                                response = readTask.Result;
                                RespuestaSimpleBE oResponse = JsonConvert.DeserializeObject<RespuestaSimpleBE>(response);
                                return true;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        throw new WebFaultException(HttpStatusCode.Unauthorized);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}