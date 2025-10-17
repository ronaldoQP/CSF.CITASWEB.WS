using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSF.CITASWEB.WS.BE;
using System.ServiceModel.Activation;
//using CSF.CITASWEB.WS.DA;
using System.Web;
using System.Net;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

namespace CSF.CITASWEB.WS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ClinicasEspecialidades : IClinicasEspecialidades
    {
        public RespuestaBE<List<CiudadClinicaBE>> ListarClinicasConDetalle(string ciudad)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(ciudad))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (ciudad.ToLower() != "lima" && ciudad.ToLower() != "provincia" && ciudad.ToLower() != "todos")
            {
                return new RespuestaBE<List<CiudadClinicaBE>>()
                {
                    rpt = 101,
                    mensaje = "Sólo se soportan los valores \"lima\", \"provincia\" o \"todos\" en el parámetro ciudad",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<CiudadClinicaBE>> varRespuesta = new RespuestaBE<List<CiudadClinicaBE>>();
            try
            {
                var oRequest = new
                {
                    ciudad = ciudad
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("ClinicasEspecialidades.svc/ListarClinicasConDetalle/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<CiudadClinicaBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<CiudadClinicaBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<ClinicaSimpleBE>> ListarClinicas()
        {
            RespuestaBE<List<ClinicaSimpleBE>> varRespuesta = new RespuestaBE<List<ClinicaSimpleBE>>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("ClinicasEspecialidades.svc/ListarClinicas/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ClinicaSimpleBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<ClinicaSimpleBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<EspecialidadBE>> EspecialidadPorClinica(string idClinica)
        {
            #region Validacion de Parámetros
            /*if (string.IsNullOrEmpty(idClinica))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            
            int varIDClinica;
            if (!string.IsNullOrEmpty(idClinica) && !int.TryParse(idClinica, out varIDClinica))
            {
                return new RespuestaBE<List<EspecialidadBE>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro idClinica debe ser numérico",
                    data = null
                };
            }*/
            #endregion
            //idClinica = "";
            RespuestaBE<List<EspecialidadBE>> varRespuesta = new RespuestaBE<List<EspecialidadBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("ClinicasEspecialidades.svc/EspecialidadPorClinica/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<EspecialidadBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<EspecialidadBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<ClinicaSimpleBE>> ClinicaPorEspecialidad(string idEspecialidad)
        {
            #region Validacion de Parámetros
            if (!string.IsNullOrEmpty(idEspecialidad))
            {
                int varIDClinica;
                if (!int.TryParse(idEspecialidad, out varIDClinica))
                {
                    return new RespuestaBE<List<ClinicaSimpleBE>>()
                    {
                        rpt = 101,
                        mensaje = "El parámetro idEspecialidad debe ser numérico",
                        data = null
                    };
                }
            }
            #endregion
            RespuestaBE<List<ClinicaSimpleBE>> varRespuesta = new RespuestaBE<List<ClinicaSimpleBE>>();
            try
            {
                var oRequest = new
                {
                    idEspecialidad = idEspecialidad
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("ClinicasEspecialidades.svc/ClinicaPorEspecialidad/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ClinicaSimpleBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<ClinicaSimpleBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<ClinicaSimpleBE>> ListarClinicasVirtuales()
        {
            RespuestaBE<List<ClinicaSimpleBE>> varRespuesta = new RespuestaBE<List<ClinicaSimpleBE>>();
            try
            {
                string response = ClasesGenericas.PostAsyncIntranet("ClinicasEspecialidades.svc/ListarClinicasVirtuales/", "", ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ClinicaSimpleBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<ClinicaSimpleBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<EspecialidadBE>> EspecialidadPorClinicaVirtual(string idClinica)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}

            //int varIDClinica;
            //if (!string.IsNullOrEmpty(idClinica) && !int.TryParse(idClinica, out varIDClinica))
            //{
            //    return new RespuestaBE<List<EspecialidadBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser numérico",
            //        data = null
            //    };
            //}
            #endregion
            RespuestaBE<List<EspecialidadBE>> varRespuesta = new RespuestaBE<List<EspecialidadBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("ClinicasEspecialidades.svc/EspecialidadPorClinicaVirtual/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<EspecialidadBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<EspecialidadBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            }
            return varRespuesta;
        }

        #region Métodos deshabilitados
        public RespuestaBE<List<ClinicaSimpleBE>> ListarClinicasMedico(string cmp)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //try
            //{
            //    return new RespuestaBE<List<ClinicaSimpleBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new ClinicasEspecialidadesDA().ListarClinicasMedico(cmp).OrderBy(p => p.nombre).ToList()
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<ClinicaSimpleBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            //}
        }
        public RespuestaBE<List<EspecialidadBE>> EspecialidadPorClinicaVirtualEC()
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //try
            //{
            //    return new RespuestaBE<List<EspecialidadBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new ClinicasEspecialidadesDA().EspecialidadPorClinicaVirtualEC().OrderBy(p => p.especialidad).ToList()
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<EspecialidadBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            //}
        }
        public RespuestaBE<List<EspecialidadFrecuenteBE>> EspecialidadesFrecuentes(string idClinica, string tipoDocumento, string numeroDocumento, string tipoCita)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //#endregion
            //try
            //{
            //    return new RespuestaBE<List<EspecialidadFrecuenteBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        //data = new ClinicasEspecialidadesDA().EspecialidadesFrecuentes(idClinica, tipoDocumento, numeroDocumento, tipoCita).OrderBy(p => p.nombre).ToList()
            //        data = new ClinicasEspecialidadesDA().EspecialidadesFrecuentes(idClinica, tipoDocumento, numeroDocumento, tipoCita)
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<EspecialidadFrecuenteBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            //}
        }
        public RespuestaBE<List<ClinicaEspecialidadAgrupadoBE>> ClinicaEspecialidadAgrupado(string idClinica, string tipoCita)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //try
            //{
            //    string rpta = new ClinicasEspecialidadesDA().ClinicaEspecialidadAgrupado(idClinica, tipoCita);

            //    List<ClinicaEspecialidadAgrupadoBE> lstClinica = new List<ClinicaEspecialidadAgrupadoBE>();

            //    if (!string.IsNullOrEmpty(rpta))
            //    {

            //        string[] listas = rpta.Split('¬');
            //        int n = listas.Length;
            //        string[] campos;
            //        string[] camposDetalle;
            //        ClinicaEspecialidadAgrupadoBE oClinicaEspecialidadAgrupadoBE;
            //        ClinicaEspecialidadAgrupadoDetalleBE odClinicaEspecialidadAgrupadoBE;
            //        for (int i = 0; i < n; i++)
            //        {
            //            campos = listas[i].Split('¦');
            //            if (campos[6].Equals("0"))
            //            {

            //                oClinicaEspecialidadAgrupadoBE = new ClinicaEspecialidadAgrupadoBE();
            //                oClinicaEspecialidadAgrupadoBE.idEspecialidad = int.Parse(campos[0]);
            //                oClinicaEspecialidadAgrupadoBE.nombre = campos[1];
            //                oClinicaEspecialidadAgrupadoBE.genero = campos[2];
            //                oClinicaEspecialidadAgrupadoBE.edadMin = int.Parse(campos[3]);
            //                oClinicaEspecialidadAgrupadoBE.edadMax = int.Parse(campos[4]);
            //                oClinicaEspecialidadAgrupadoBE.icono = campos[5];
            //                oClinicaEspecialidadAgrupadoBE.idEspecialidadPadre = int.Parse(campos[6]);
            //                oClinicaEspecialidadAgrupadoBE.detalle = new List<ClinicaEspecialidadAgrupadoDetalleBE>();

            //                for (int j = 0; j < n; j++)
            //                {
            //                    camposDetalle = listas[j].Split('¦');
            //                    if (!camposDetalle[6].Equals("0") && campos[0].Equals(camposDetalle[6]))
            //                    {
            //                        odClinicaEspecialidadAgrupadoBE = new ClinicaEspecialidadAgrupadoDetalleBE();

            //                        odClinicaEspecialidadAgrupadoBE.idEspecialidad = int.Parse(camposDetalle[0]);
            //                        odClinicaEspecialidadAgrupadoBE.nombre = camposDetalle[1];
            //                        odClinicaEspecialidadAgrupadoBE.genero = camposDetalle[2];
            //                        odClinicaEspecialidadAgrupadoBE.edadMin = int.Parse(camposDetalle[3]);
            //                        odClinicaEspecialidadAgrupadoBE.edadMax = int.Parse(camposDetalle[4]);
            //                        odClinicaEspecialidadAgrupadoBE.icono = camposDetalle[5];
            //                        odClinicaEspecialidadAgrupadoBE.idEspecialidadPadre = int.Parse(camposDetalle[6]);

            //                        oClinicaEspecialidadAgrupadoBE.detalle.Add(odClinicaEspecialidadAgrupadoBE);

            //                    }
            //                }

            //                lstClinica.Add(oClinicaEspecialidadAgrupadoBE);
            //            }

            //        }

            //    }

            //    return new RespuestaBE<List<ClinicaEspecialidadAgrupadoBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = lstClinica
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<ClinicaEspecialidadAgrupadoBE>>(ex, "WS", "ClinicasEspecialidades.svc");
            //}
        }
        #endregion
    }
}
