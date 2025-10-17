using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using CSF.CITASWEB.WS.BE;
using System.ServiceModel.Web;
using System.Web;
using System.Net;
//using CSF.CITASWEB.WS.DA;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace CSF.CITASWEB.WS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Medico : IMedico
    {
        public RespuestaBE<List<MedicoHorarioBE>> HorariosPorMedico(string idClinica, string idEspecialidad,
                                        string tipoDocumento, string numeroDocumento, string dia,
                                        string nombre)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(dia))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIntDummy;
            if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro idClinica debe ser un número",
                    data = null
                };
            }
            if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro idEspecialidad debe ser un número",
                    data = null
                };
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 104,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            DateTime varDia;
            if (!DateTime.TryParse(dia, out varDia))
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 105,
                    mensaje = "El parámetro dia debe ser una fecha válida",
                    data = null
                };
            }
            if (varDia < DateTime.Today)
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 106,
                    mensaje = "La fecha no puede ser menor a la fecha actual",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoHorarioBE>> varRespuesta = new RespuestaBE<List<MedicoHorarioBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    dia = dia,
                    nombre = nombre
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/HorariosPorMedico/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoHorarioBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoHorarioBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<MedicoHorarioSimpleWebBE>> HorariosPorNombreMedico(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos)
        {

            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            //if (string.IsNullOrEmpty(idClinica) && string.IsNullOrEmpty(idEspecialidad) && string.IsNullOrEmpty(nombre))
            //if (string.IsNullOrEmpty(nombre))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 100,
            //        //mensaje = "Se debe indicar el idClínica, el idEspecialidad o el nombre (al menos uno de ellos)",
            //        mensaje = "Se debe indicar el nombre (al menos uno de ellos)",
            //        data = null
            //    };
            //}
            int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            //if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
                {
                    rpt = 104,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoHorarioSimpleWebBE>> varRespuesta = new RespuestaBE<List<MedicoHorarioSimpleWebBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    nombre = nombre,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    soloMedicosFavoritos = soloMedicosFavoritos
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/HorariosPorNombreMedico/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoHorarioSimpleWebBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoHorarioSimpleWebBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<MedicoHorarioDisponibleBEV2>> FechasDisponiblesPorMedico(string idClinica, string idEspecialidad, string cmp)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(cmp))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIntDummy;
            if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            {
                return new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro idClinica debe ser un número",
                    data = null
                };
            }
            if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            {
                return new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro idEspecialidad debe ser un número",
                    data = null
                };
            }
            if (cmp.Length > 10)
            {
                return new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoHorarioDisponibleBEV2>> varRespuesta = new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    cmp = cmp
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/FechasDisponiblesPorMedico/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoHorarioDisponibleBEV2>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoHorarioDisponibleBEV2>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<DiasHorarioDisponibleBE>> FechasDisponibles(string idClinica, string idEspecialidad)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(idEspecialidad))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIntDummy;
            if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            {
                return new RespuestaBE<List<DiasHorarioDisponibleBE>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro idClinica debe ser un número",
                    data = null
                };
            }
            if (!int.TryParse(idEspecialidad, out varIntDummy))
            {
                return new RespuestaBE<List<DiasHorarioDisponibleBE>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro idEspecialidad debe ser un número",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<DiasHorarioDisponibleBE>> varRespuesta = new RespuestaBE<List<DiasHorarioDisponibleBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/FechasDisponibles/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<DiasHorarioDisponibleBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<DiasHorarioDisponibleBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<MedicoHorarioBE>> HorariosPorMedicoVirtual(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia, string idClinica = "17")
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(dia))
            if (string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(dia))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro idEspecialidad debe ser un número",
                    data = null
                };
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 102,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 103,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            DateTime varDia;
            if (!DateTime.TryParse(dia, out varDia))
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 104,
                    mensaje = "El parámetro dia debe ser una fecha válida",
                    data = null
                };
            }
            if (varDia < DateTime.Today)
            {
                return new RespuestaBE<List<MedicoHorarioBE>>()
                {
                    rpt = 105,
                    mensaje = "La fecha no puede ser menor a la fecha actual",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoHorarioBE>> varRespuesta = new RespuestaBE<List<MedicoHorarioBE>>();
            try
            {
                var oRequest = new
                {
                    idEspecialidad = idEspecialidad,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    dia = dia,
                    idClinica = idClinica
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/HorariosPorMedicoVirtual/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoHorarioBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoHorarioBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<MedicoHorarioSimpleWebBE>> HorariosPorNombreMedicoVirtual(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            //if (string.IsNullOrEmpty(idClinica) && string.IsNullOrEmpty(idEspecialidad) && string.IsNullOrEmpty(nombre))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 100,
            //        mensaje = "Se debe indicar el idClínica, el idEspecialidad o el nombre (al menos uno de ellos)",
            //        data = null
            //    };
            //}
            //int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            //if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
                    data = null
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
                {
                    rpt = 104,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoHorarioSimpleWebBE>> varRespuesta = new RespuestaBE<List<MedicoHorarioSimpleWebBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    nombre = nombre,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    soloMedicosFavoritos = soloMedicosFavoritos
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/HorariosPorNombreMedicoVirtual/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoHorarioSimpleWebBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoHorarioSimpleWebBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<InfoMedicoBE> ObtenerInformacionMedico(string cmp, string idEspecialidad)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(cmp) || string.IsNullOrEmpty(idEspecialidad))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (cmp.Length > 10)
            {
                return new RespuestaBE<InfoMedicoBE>()
                {
                    rpt = 101,
                    mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<InfoMedicoBE> varRespuesta = new RespuestaBE<InfoMedicoBE>();
            try
            {
                var oRequest = new
                {
                    cmp = cmp,
                    idEspecialidad = idEspecialidad
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/ObtenerInformacionMedico/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<InfoMedicoBE>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<InfoMedicoBE>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<DiasHorarioDisponibleBE>> FechasDisponiblesVirtual(string idClinica, string idEspecialidad)
        {

            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(idEspecialidad))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<DiasHorarioDisponibleBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            if (!int.TryParse(idEspecialidad, out varIntDummy))
            {
                return new RespuestaBE<List<DiasHorarioDisponibleBE>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro idEspecialidad debe ser un número",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<DiasHorarioDisponibleBE>> varRespuesta = new RespuestaBE<List<DiasHorarioDisponibleBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/FechasDisponiblesVirtual/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<DiasHorarioDisponibleBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<DiasHorarioDisponibleBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<MedicoHorarioDisponibleBEV2>> FechasDisponiblesPorMedicoVirtual(string idClinica, string idEspecialidad, string cmp)
        {
            idClinica = "";
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(cmp))
            if (string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(cmp))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            {
                return new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>()
                {
                    rpt = 102,
                    mensaje = "El parámetro idEspecialidad debe ser un número",
                    data = null
                };
            }
            if (cmp.Length > 10)
            {
                return new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>()
                {
                    rpt = 101,
                    mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
                    data = null
                };
            }
            #endregion
            RespuestaBE<List<MedicoHorarioDisponibleBEV2>> varRespuesta = new RespuestaBE<List<MedicoHorarioDisponibleBEV2>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    cmp = cmp
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/FechasDisponiblesPorMedicoVirtual/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<MedicoHorarioDisponibleBEV2>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<MedicoHorarioDisponibleBEV2>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<DirectorioMedicoBE>> DirectorioMedico(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos)
        {
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //int varIntDummy;
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaBE<List<DirectorioMedicoBE>>()
            //    {
            //        rpt = 103,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaBE<List<DirectorioMedicoBE>>()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            #endregion
            RespuestaBE<List<DirectorioMedicoBE>> varRespuesta = new RespuestaBE<List<DirectorioMedicoBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    nombre = nombre,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    soloMedicosFavoritos = soloMedicosFavoritos
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/DirectorioMedico/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<DirectorioMedicoBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<DirectorioMedicoBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }
        public RespuestaBE<List<DirectorioMedicoBE>> DirectorioMedicoVirtual(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos)
        {
            idClinica = null;
            #region Validacion de Parámetros
            //if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaBE<List<DirectorioMedicoBE>>()
            //    {
            //        rpt = 103,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaBE<List<DirectorioMedicoBE>>()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            #endregion
            RespuestaBE<List<DirectorioMedicoBE>> varRespuesta = new RespuestaBE<List<DirectorioMedicoBE>>();
            try
            {
                var oRequest = new
                {
                    idClinica = idClinica,
                    idEspecialidad = idEspecialidad,
                    nombre = nombre,
                    tipoDocumento = tipoDocumento,
                    numeroDocumento = numeroDocumento,
                    soloMedicosFavoritos = soloMedicosFavoritos
                };
                string strRequest = new JavaScriptSerializer().Serialize(oRequest);
                string response = ClasesGenericas.PostAsyncIntranet("Medico.svc/DirectorioMedicoVirtual/", strRequest, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                varRespuesta = new JavaScriptSerializer().Deserialize<RespuestaBE<List<DirectorioMedicoBE>>>(response);
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<DirectorioMedicoBE>>(ex, "WS", "Medico.svc");
            }
            return varRespuesta;
        }

        #region Métodos deshabilitados
        public RespuestaBE<MedicoBE> InformacionMedico(string cmp)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(cmp))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //if (cmp.Length > 10)
            //{
            //    return new RespuestaBE<MedicoBE>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaBE<MedicoBE>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new MedicoDA().InformacionMedico(cmp)
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<MedicoBE>(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaSimpleBE PedirMedicoDomicilio(string especialidad, string tipoDocumento, string numeroDocumento,
                                                string nombre, string celular, string direccion, string latitud,
                                                string longitud)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(especialidad) || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
            //    string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(celular) || string.IsNullOrEmpty(direccion))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            ////if (especialidad.Replace("í", "i").ToLower() != "pediatria" && especialidad.ToLower() != "medicina interna")
            ////{
            ////    return new RespuestaSimpleBE()
            ////    {
            ////        rpt = 101,
            ////        mensaje = "Sólo se soportan los valores \"Pediatría\" o \"Medicina Interna\" en el parámetro especialidad",
            ////        data = null
            ////    };
            ////}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 103,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            //if (nombre.Length > 152)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro nombre no puede tener más de 152 caracteres",
            //        data = null
            //    };
            //}
            //if (celular.Length != 9)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 105,
            //        mensaje = "El parámetro celular debe tener 9 dígitos",
            //        data = null
            //    };
            //}
            //decimal numeroCelular;
            //if (!decimal.TryParse(celular, out numeroCelular))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 106,
            //        mensaje = "El parámetro celular debe tener ser numérico",
            //        data = null
            //    };
            //}
            //if (direccion.Length > 500)
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 107,
            //        mensaje = "El parámetro direccion no puede tener más de 500 caracteres",
            //        data = null
            //    };
            //}
            //if (!string.IsNullOrEmpty(latitud))
            //{
            //    decimal posicionLatitud;
            //    if (!decimal.TryParse(latitud, out posicionLatitud))
            //    {
            //        return new RespuestaSimpleBE()
            //        {
            //            rpt = 108,
            //            mensaje = "El parámetro latitud debe tener ser numérico",
            //            data = null
            //        };
            //    }
            //}
            //if (!string.IsNullOrEmpty(longitud))
            //{
            //    decimal posicionLongitud;
            //    if (!decimal.TryParse(longitud, out posicionLongitud))
            //    {
            //        return new RespuestaSimpleBE()
            //        {
            //            rpt = 109,
            //            mensaje = "El parámetro longitud debe tener ser numérico",
            //            data = null
            //        };
            //    }
            //}
            //#endregion
            //try
            //{
            //    string varSeguro = "";
            //    try
            //    {
            //        varSeguro = new MedicoDA().GuardarMedicoDomicilio(especialidad, tipoDocumento, numeroDocumento,
            //                                                                nombre, celular, direccion, latitud,
            //                                                                longitud);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //    string rutaArchivo = ClasesGenericas.GenerarPeticionMedicoDomicilio(tipoDocumento + "_" + numeroDocumento, especialidad + "|" + tipoDocumento + "|" + numeroDocumento + "|" + nombre.ToUpper() + "|" + celular + "|" + direccion + "|" + (string.IsNullOrEmpty(latitud) ? "" : latitud) + "|" + (string.IsNullOrEmpty(longitud) ? "" : longitud));
            //    //2017-12-02: Correo para médicos a domicilio
            //    Dictionary<string, string> varParametrosCorreo = new Dictionary<string, string>();
            //    varParametrosCorreo.Add("Email", ConfigurationManager.AppSettings["correoMedicoDomicilio"] == null ? "drmas.atencionalcliente@sanna.pe" : ConfigurationManager.AppSettings["correoMedicoDomicilio"].ToString());
            //    varParametrosCorreo.Add("celular", celular);
            //    varParametrosCorreo.Add("numeroDocumento", numeroDocumento);
            //    varParametrosCorreo.Add("nombre", nombre);
            //    varParametrosCorreo.Add("fechaSolicitud", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            //    varParametrosCorreo.Add("direccion", direccion);
            //    varParametrosCorreo.Add("latitud", latitud);
            //    varParametrosCorreo.Add("longitud", longitud);
            //    varParametrosCorreo.Add("especialidad", especialidad);
            //    varParametrosCorreo.Add("seguro", varSeguro);

            //    bool statusEnvioEmail = false;
            //    try
            //    {
            //        statusEnvioEmail = ClasesGenericas.EnviarCorreo(varParametrosCorreo["Email"], "MedicoDomicilio", varParametrosCorreo, rutaArchivo);

            //    }
            //    catch (Exception)
            //    {
            //    }
            //    //2017-12-02: Correo para médicos a domicilio

            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = (statusEnvioEmail ? "" : "No se pudo notificar vía email"),
            //        data = null
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Medico.svc");
            //}
        }
        //public RespuestaBE<List<MedicoHorarioDisponibleBE>> FechasDisponiblesPorMedico(string idClinica, string idEspecialidad, string cmp)
        //{

        //    #region Validacion de Parámetros
        //    if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(cmp))
        //    {
        //        throw new WebFaultException(HttpStatusCode.BadRequest);
        //    }
        //    int varIntDummy;
        //    if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
        //    {
        //        return new RespuestaBE<List<MedicoHorarioDisponibleBE>>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro idClinica debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
        //    {
        //        return new RespuestaBE<List<MedicoHorarioDisponibleBE>>()
        //        {
        //            rpt = 102,
        //            mensaje = "El parámetro idEspecialidad debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (cmp.Length > 10)
        //    {
        //        return new RespuestaBE<List<MedicoHorarioDisponibleBE>>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
        //            data = null
        //        };
        //    }
        //    #endregion
        //    try
        //    {
        //        return new RespuestaBE<List<MedicoHorarioDisponibleBE>>()
        //        {
        //            rpt = 0,
        //            mensaje = "",
        //            data = new MedicoDA().FechasDisponiblesPorMedico(idClinica, idEspecialidad, cmp).OrderBy(p => p.fechaDate).ThenBy(p => p.horaInicio).ToList()

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ErrorDA().RegistrarError<List<MedicoHorarioDisponibleBE>>(ex, "WS", "Medico.svc");
        //    }
        //}
        //public RespuestaBE<List<CMPMedicoHorarioBE>> FechasProximasPorMedico(string idClinica, string idEspecialidad, string cmp, 
        //                                                                        string ejecucion, string registrosInicial, string registrosFinal, string tipoCita)
        //{

        //    #region Validacion de Parámetros
        //    if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad))// || string.IsNullOrEmpty(cmp))
        //    {
        //        throw new WebFaultException(HttpStatusCode.BadRequest);
        //    }
        //    int varIntDummy;
        //    if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
        //    {
        //        return new RespuestaBE<List<CMPMedicoHorarioBE>>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro idClinica debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
        //    {
        //        return new RespuestaBE<List<CMPMedicoHorarioBE>>()
        //        {
        //            rpt = 102,
        //            mensaje = "El parámetro idEspecialidad debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (cmp.Length > 10)
        //    {
        //        return new RespuestaBE<List<CMPMedicoHorarioBE>>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
        //            data = null
        //        };
        //    }
        //    if (registrosInicial != null && !int.TryParse(registrosInicial, out varIntDummy))
        //    {
        //        return new RespuestaBE<List<CMPMedicoHorarioBE>>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro registrosInicial debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (registrosFinal != null && !int.TryParse(registrosFinal, out varIntDummy))
        //    {
        //        return new RespuestaBE<List<CMPMedicoHorarioBE>>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro registrosFinal debe ser un número",
        //            data = null
        //        };
        //    }
        //    #endregion
        //    try
        //    {
        //        return new RespuestaBE<List<CMPMedicoHorarioBE>>()
        //        {
        //            rpt = 0,
        //            mensaje = "",
        //            //data = new MedicoDA().FechasProximasPorMedico(idClinica, idEspecialidad, cmp, ejecucion, registrosInicial, registrosFinal, tipoCita).OrderBy(p => p.fechaDate).ToList()
        //            data = new MedicoDA().FechasProximasPorMedico(idClinica, idEspecialidad, cmp, ejecucion, registrosInicial, registrosFinal, tipoCita).ToList()
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ErrorDA().RegistrarError<List<CMPMedicoHorarioBE>>(ex, "WS", "Medico.svc");
        //    }
        //}
        public RespuestaBE<HorarioCitaMasProxima> HorariosPorMedicoProximo(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia, string idClinica, string cmp, string tipo)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
        //    #region Validacion de Parámetros
        //    if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(dia))
        //    {
        //        throw new WebFaultException(HttpStatusCode.BadRequest);
        //    }
        //    int varIntDummy;
        //    if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro idClinica debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 101,
        //            mensaje = "El parámetro idEspecialidad debe ser un número",
        //            data = null
        //        };
        //    }
        //    if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 102,
        //            mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
        //            data = null
        //        };
        //    }
        //    if (numeroDocumento.Length > 20)
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 103,
        //            mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
        //            data = null
        //        };
        //    }
        //    DateTime varDia;
        //    if (!DateTime.TryParse(dia, out varDia))
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 104,
        //            mensaje = "El parámetro dia debe ser una fecha válida",
        //            data = null
        //        };
        //    }
        //    if (varDia < DateTime.Today)
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 105,
        //            mensaje = "La fecha no puede ser menor a la fecha actual",
        //            data = null
        //        };
        //    }
        //    #endregion
        //    var resp = new HorarioCitaMasProxima();

        //    if (tipo == "presencial")
        //    {
        //        var fechaFin = new MedicoDA().FechaFinalMedico(idClinica, idEspecialidad, cmp);

        //        //var resp = new HorarioCitaMasProxima();
        //        List<MedicoHorarioBE> response = new List<MedicoHorarioBE>();
        //        if (fechaFin <= DateTime.Now)
        //        {
        //            resp.disponibilidad = false;
        //        }
        //        else
        //        {
        //            for (DateTime date = DateTime.Now.AddDays(1); date.Date <= fechaFin; date = date.AddDays(1))
        //            {

        //                Debug.WriteLine(date);
        //                var responseAux = new MedicoDA().FechasDisponiblesPorMedicoE(idClinica, idEspecialidad, cmp, date);
        //                if (responseAux.Count > 0)
        //                {
        //                    response = new MedicoDA().HorariosPorMedico(idClinica, idEspecialidad, tipoDocumento, numeroDocumento, date, ((int)date.DayOfWeek).ToString(), "");
        //                    resp.fecha = date.ToString("dd/MM/yyyy");
        //                    resp.disponibilidad = true;
        //                    break;
        //                }

        //            }

        //            var aux = response.Find(x => x.cmp == cmp);
        //            response.Remove(response.Find(x => x.cmp == cmp));
        //            response.Insert(0, aux);

        //            resp.horarios = response;
        //            resp.clinica = new ClinicaObj();
        //            resp.clinica.idClinica = idClinica;
        //            resp.clinica.nombre = (aux != null) ? aux.centroAtencion : "";
        //            resp.especialidad = new EspecialidadObj();
        //            resp.especialidad.idEspecialidad = idEspecialidad;
        //            resp.especialidad.especialidad = (aux != null) ? aux.especialidad : "";


        //        }




        //    }
        //    else
        //    {
        //        try
        //        {
        //            var response = new MedicoDA().HorariosPorMedicoVirtualCmp(
        //idClinica, idEspecialidad, tipoDocumento, numeroDocumento,
        //varDia, ((int)varDia.DayOfWeek).ToString(), cmp).OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
        //            var medico = response[0];

        //            var fechafinal = medico.fechaFinal;

        //            medico.prioritario = true;
        //            medico.fechaProxima = varDia.ToString("dd/MM/yyyy");
        //            if (fechafinal <= DateTime.Now)
        //            {
        //                Debug.WriteLine("Clear1");
        //                medico.horarios.Clear();
        //                medico.horariosEnteros.Clear();
        //            }
        //            else
        //            {
        //                if (medico.horarios.Count <= 0)
        //                {
        //                    Debug.WriteLine("0 horarios");
        //                    for (DateTime date = DateTime.Today.AddDays(1); date.Date <= fechafinal; date = date.AddDays(1))
        //                    {
        //                        Debug.WriteLine(date);
        //                        var responseAux = new MedicoDA().HorariosPorMedicoVirtualCmp(idClinica, idEspecialidad, tipoDocumento, numeroDocumento,
        //                                     date, ((int)date.DayOfWeek).ToString(), cmp).OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
        //                        var medicoAux = responseAux[0];
        //                        if (medicoAux.horarios.Count > 0)
        //                        {
        //                            Debug.WriteLine("Rpta");
        //                            medico = medicoAux;
        //                            medico.fechaProxima = date.ToString("dd/MM/yyyy");
        //                            medico.prioritario = true;
        //                            break;
        //                        }

        //                    }
        //                }


        //            }
        //            List<MedicoHorarioBE> horarios = new List<MedicoHorarioBE>();
        //            horarios = new MedicoDA().HorariosPorMedicoVirtual(
        //                        idClinica, idEspecialidad, tipoDocumento, numeroDocumento,
        //                        Convert.ToDateTime(medico.fechaProxima), ((int)Convert.ToDateTime(medico.fechaProxima).DayOfWeek).ToString()).OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
        //            horarios.Remove(horarios.Find(x => x.cmp == cmp));
        //            horarios.Insert(0, medico);



        //            resp.clinica = new ClinicaObj();
        //            resp.clinica.idClinica = idClinica;
        //            resp.clinica.nombre = null;
        //            resp.especialidad = new EspecialidadObj();
        //            resp.especialidad.idEspecialidad = idEspecialidad;
        //            resp.especialidad.especialidad = null;
        //            resp.horarios = horarios;
        //            resp.fecha = medico.fechaProxima;
        //            resp.disponibilidad = (horarios.Find(x => x.cmp == cmp).horarios.Count == 0) ? false : true;

        //        }
        //        catch (Exception exx)
        //        {
        //            new ErrorDA().RegistrarError<HorarioCitaMasProxima>(exx, "WS", "Medico.svc");

        //            resp.clinica = new ClinicaObj();
        //            resp.clinica.idClinica = idClinica;
        //            resp.clinica.nombre = null;
        //            resp.especialidad = new EspecialidadObj();
        //            resp.especialidad.idEspecialidad = idEspecialidad;
        //            resp.especialidad.especialidad = null;
        //            resp.disponibilidad = false;

        //        }

        //    }



        //    try
        //    {
        //        return new RespuestaBE<HorarioCitaMasProxima>()
        //        {
        //            rpt = 0,
        //            mensaje = "",
        //            data = resp


        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ErrorDA().RegistrarError<HorarioCitaMasProxima>(ex, "WS", "Medico.svc");
        //    }
        }
        public RespuestaBE<HorarioCitaMasProxima> HorariosPorMedicoProximoPresencial(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia, string idClinica, string cmp)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(dia))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            //if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 102,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 103,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            //DateTime varDia;
            //if (!DateTime.TryParse(dia, out varDia))
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro dia debe ser una fecha válida",
            //        data = null
            //    };
            //}
            //if (varDia < DateTime.Today)
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 105,
            //        mensaje = "La fecha no puede ser menor a la fecha actual",
            //        data = null
            //    };
            //}
            //#endregion
            //var fechaFin = new MedicoDA().FechaFinalMedico(idClinica, idEspecialidad, cmp);

            //var resp = new HorarioCitaMasProxima();
            //List<MedicoHorarioBE> response = new List<MedicoHorarioBE>();
            //for (DateTime date = varDia.AddDays(1); date.Date <= fechaFin; date = date.AddDays(1))
            //{

            //    Debug.WriteLine(date);
            //    var responseAux = new MedicoDA().FechasDisponiblesPorMedicoE(idClinica, idEspecialidad, cmp, date);
            //    if (responseAux.Count > 0)
            //    {
            //        response = new MedicoDA().HorariosPorMedico(idClinica, idEspecialidad, tipoDocumento, numeroDocumento, date, ((int)date.DayOfWeek).ToString(), "");
            //        resp.fecha = date.ToString("dd/MM/yyyy");
            //        resp.disponibilidad = true;
            //        break;
            //    }

            //}
            //var aux = response.Find(x => x.cmp == cmp);
            //response.Remove(response.Find(x => x.cmp == cmp));
            //response.Insert(0, aux);
            //resp.horarios = response;










            //try
            //{
            //    return new RespuestaBE<HorarioCitaMasProxima>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = resp


            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<HorarioCitaMasProxima>(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaBE<List<MedicoHorarioSimpleWebBE>> BuscarMedicoWeb(string idClinica, string idEspecialidad, string nombre)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica) && string.IsNullOrEmpty(idEspecialidad) && string.IsNullOrEmpty(nombre))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 100,
            //        mensaje = "Se debe indicar el idClínica, el idEspecialidad o el nombre (al menos uno de ellos)",
            //        data = null
            //    };
            //}
            //int varIntDummy;
            //if (!string.IsNullOrEmpty(idClinica) && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            //if (!string.IsNullOrEmpty(idEspecialidad) && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new MedicoDA().BuscarMedicoWeb(idClinica, idEspecialidad, nombre).OrderBy(p => p.nombres).ToList()

            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<MedicoHorarioSimpleWebBE>>(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaBE<List<MedicoHorarioSimpleWebBE>> StaffMedicos(string idClinica, string idEspecialidad, string nombre)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica) && string.IsNullOrEmpty(idEspecialidad) && string.IsNullOrEmpty(nombre))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 100,
            //        mensaje = "Se debe indicar el idClínica, el idEspecialidad o el nombre (al menos uno de ellos)",
            //        data = null
            //    };
            //}
            //int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            //if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaBE<List<MedicoHorarioSimpleWebBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new MedicoDA().StaffMedicos(idClinica, idEspecialidad, nombre).OrderBy(p => p.nombres).ToList()

            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<MedicoHorarioSimpleWebBE>>(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaSimpleBE TurnosPorRangoHorario(string fechaInicio, string fechaFin)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //DateTime varFechaInicio;
            //if (!DateTime.TryParse(fechaInicio, out varFechaInicio))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro fechaInicio no tiene el formato correcto",
            //        data = null
            //    };
            //}
            //DateTime varFechaFin;
            //if (!DateTime.TryParse(fechaFin, out varFechaFin))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro fechaFin no tiene el formato correcto",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new MedicoDA().TurnosPorRangoHorario(varFechaInicio, varFechaFin)

            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaSimpleBE TurnosPorRangoHorarioV2(string fechaInicio, string fechaFin)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //DateTime varFechaInicio;
            //if (!DateTime.TryParse(fechaInicio, out varFechaInicio))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro fechaInicio no tiene el formato correcto",
            //        data = null
            //    };
            //}
            //DateTime varFechaFin;
            //if (!DateTime.TryParse(fechaFin, out varFechaFin))
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro fechaFin no tiene el formato correcto",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaSimpleBE()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new MedicoDA().TurnosPorRangoHorarioV2(varFechaInicio, varFechaFin)

            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaBE<List<CitaHistoricaVistaPreviaBE>> ListarCitasMedico(string cmp, string idClinica, string fecha)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(cmp) || string.IsNullOrEmpty(fecha))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //DateTime varDia;
            //if (!DateTime.TryParse(fecha, out varDia))
            //{
            //    return new RespuestaBE<List<CitaHistoricaVistaPreviaBE>>()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro dia debe ser una fecha válida",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaBE<List<CitaHistoricaVistaPreviaBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new CitaDA().ListarCitasMedico(cmp, idClinica, varDia)
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<CitaHistoricaVistaPreviaBE>>(ex, "WS", "Cita.svc");
            //}
        }
        public RespuestaBE<List<MedicoHorarioBE>> ListarMedicoHorarioPorEspecialidadEC(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(dia))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //int varIntDummy;
            //if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            //if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            //{
            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 102,
            //        mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento",
            //        data = null
            //    };
            //}
            //if (numeroDocumento.Length > 20)
            //{
            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 103,
            //        mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres",
            //        data = null
            //    };
            //}
            //DateTime varDia;
            //if (!DateTime.TryParse(dia, out varDia))
            //{
            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 104,
            //        mensaje = "El parámetro dia debe ser una fecha válida",
            //        data = null
            //    };
            //}
            //if (varDia < DateTime.Today)
            //{
            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 105,
            //        mensaje = "La fecha no puede ser menor a la fecha actual",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    var general = new MedicoDA().ListarMedicoHorarioPorEspecialidadEC(idEspecialidad, tipoDocumento, numeroDocumento,
            //            varDia, ((int)varDia.DayOfWeek).ToString()).OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();

            //    List<MedicoHorarioBE> llenos = new List<MedicoHorarioBE>();
            //    List<MedicoHorarioBE> vacios = new List<MedicoHorarioBE>();

            //    /*
            //    if (idClinica == "17")
            //    {
            //        foreach (var item in general)
            //        {
            //            if (item.horarios.Count > 0)
            //            {
            //                llenos.Add(item);
            //            }
            //            else
            //            {
            //                vacios.Add(item);
            //            }
            //        }
            //        llenos = llenos.OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
            //        vacios = vacios.OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
            //    }
            //    List<MedicoHorarioBE> response = (idClinica == "17") ? llenos.Concat(vacios).ToList() : general;
            //    */

            //    foreach (var item in general)
            //    {
            //        if (item.horarios.Count > 0)
            //        {
            //            llenos.Add(item);
            //        }
            //        else
            //        {
            //            vacios.Add(item);
            //        }
            //    }
            //    llenos = llenos.OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
            //    vacios = vacios.OrderByDescending(p => p.idMedicoFavorito).ThenBy(p => p.nombres).ToList();
            //    List<MedicoHorarioBE> response = llenos.Concat(vacios).ToList();

            //    return new RespuestaBE<List<MedicoHorarioBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = response


            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<MedicoHorarioBE>>(ex, "WS", "Medico.svc");
            //}
        }
        public RespuestaBE<List<MedicoHorarioProximaFechaBE>> FechasProximasPorMedico(string idClinica, string idEspecialidad, string cmp,
                                                                                string ejecucion, string registrosInicial, string registrosFinal, string tipoCita)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //#region Validacion de Parámetros
            //if (string.IsNullOrEmpty(idClinica) || string.IsNullOrEmpty(idEspecialidad) || string.IsNullOrEmpty(cmp))
            //{
            //    throw new WebFaultException(HttpStatusCode.BadRequest);
            //}
            //int varIntDummy;
            //if (idClinica != null && !int.TryParse(idClinica, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioProximaFechaBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro idClinica debe ser un número",
            //        data = null
            //    };
            //}
            //if (idEspecialidad != null && !int.TryParse(idEspecialidad, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioProximaFechaBE>>()
            //    {
            //        rpt = 102,
            //        mensaje = "El parámetro idEspecialidad debe ser un número",
            //        data = null
            //    };
            //}
            //if (cmp.Length > 10)
            //{
            //    return new RespuestaBE<List<MedicoHorarioProximaFechaBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro cmp, debe tener como máximo 10 caracteres",
            //        data = null
            //    };
            //}
            //if (registrosInicial != null && !int.TryParse(registrosInicial, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioProximaFechaBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro registrosInicial debe ser un número",
            //        data = null
            //    };
            //}
            //if (registrosFinal != null && !int.TryParse(registrosFinal, out varIntDummy))
            //{
            //    return new RespuestaBE<List<MedicoHorarioProximaFechaBE>>()
            //    {
            //        rpt = 101,
            //        mensaje = "El parámetro registrosFinal debe ser un número",
            //        data = null
            //    };
            //}
            //#endregion
            //try
            //{
            //    return new RespuestaBE<List<MedicoHorarioProximaFechaBE>>()
            //    {
            //        rpt = 0,
            //        mensaje = "",
            //        data = new MedicoDA().FechasProximasPorMedico(idClinica, idEspecialidad, cmp, ejecucion, registrosInicial, registrosFinal, tipoCita).OrderBy(p => p.fechaDate).ToList()
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorDA().RegistrarError<List<MedicoHorarioProximaFechaBE>>(ex, "WS", "Medico.svc");
            //}
        }
        #endregion
    }
}
