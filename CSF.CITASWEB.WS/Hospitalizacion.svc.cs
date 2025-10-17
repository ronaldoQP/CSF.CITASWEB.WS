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
using io=System.IO;
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
    public class Hospitalizacion : IHospitalizacion
    {
        String _urlBaseHospitalizacion = String.Empty;
        String _apiKeyHospitalizacion = String.Empty;
        public Hospitalizacion ()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _urlBaseHospitalizacion = ClasesGenericas.GetSetting("WS_Hospitalizacion");
            _apiKeyHospitalizacion = ClasesGenericas.GetSetting("ApiKey_Hospitalizacion");
        }
        public RespuestaBE<List<HOConsultarMedicamentosPresentacionBE>> ConsultarMedicamentos(string tipoDocumento, string numeroDocumento, string descripcion)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                descripcion = descripcion != null ? descripcion : "";
                String urlMetodo = "api/ConsultarMedicamentos";
                List<HOConsultarMedicamentosPresentacionBE> lMedicamentoPresentacion = new List<HOConsultarMedicamentosPresentacionBE>();

                HOConsultarMedicamentosBE oRequest = new HOConsultarMedicamentosBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.Descripcion = descripcion;

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    List<HOConsultarMedicamentosResponseBE> lMedicamento = new JavaScriptSerializer().Deserialize<List<HOConsultarMedicamentosResponseBE>>(oResponse.data);
                    if (lMedicamento != null)
                    {
                        HOConsultarMedicamentosPresentacionBE oMedicamentoPresentacion;
                        int i = 0, nMedicamentos = lMedicamento.Count;
                        for (; i < nMedicamentos; i++)
                        {
                            oMedicamentoPresentacion = new HOConsultarMedicamentosPresentacionBE();
                            oMedicamentoPresentacion.IdeAlergiaMedicamentosDet = lMedicamento[i].IdeAlergiaMedicamentosDet.ToString();
                            oMedicamentoPresentacion.Codigo = lMedicamento[i].Codigo;
                            oMedicamentoPresentacion.Descripcion = lMedicamento[i].Descripcion;
                            lMedicamentoPresentacion.Add(oMedicamentoPresentacion);
                        }
                    }
                    return new RespuestaBE<List<HOConsultarMedicamentosPresentacionBE>>()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = lMedicamentoPresentacion
                    };
                } 
                else
                {
                    return new RespuestaBE<List<HOConsultarMedicamentosPresentacionBE>>()
                    {
                        rpt = oResponse.code,
                        mensaje = oResponse.message
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<HOConsultarMedicamentosPresentacionBE>>(ex, "WS", "Hospitalizacion.svc/ConsultarMedicamentos");
                return ClasesGenericas.RegistrarErrorIntranet<List<HOConsultarMedicamentosPresentacionBE>>(ex, "WS", "Hospitalizacion.svc/ConsultarMedicamentos");
            }
        }

        public RespuestaBE<List<HOConsultarMedicacionItemsPresentacionBE>> ConsultarMedicacionItems(string tipoDocumento, string numeroDocumento, string orden)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(orden))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                String urlMetodo = "api/ConsultarMedicacionItems";
                List<HOConsultarMedicacionItemsPresentacionBE> lMedicamentoItemPresentacion = new List<HOConsultarMedicacionItemsPresentacionBE>();

                HOConsultarMedicacionItemsBE oRequest = new HOConsultarMedicacionItemsBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.Orden = int.Parse(orden);

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    List<HOConsultarMedicacionItemsResponseBE> lMedicamentoItem = new JavaScriptSerializer().Deserialize<List<HOConsultarMedicacionItemsResponseBE>>(oResponse.data);
                    if (lMedicamentoItem != null)
                    {
                        HOConsultarMedicacionItemsPresentacionBE oMedicamentoItemPresentacion;
                        int i = 0, nMedicamentoItem = lMedicamentoItem.Count;
                        for (; i < nMedicamentoItem; i++)
                        {
                            oMedicamentoItemPresentacion = new HOConsultarMedicacionItemsPresentacionBE();
                            oMedicamentoItemPresentacion.IdeMedicacionItemsMae = lMedicamentoItem[i].IdeMedicacionItemsMae.ToString();
                            oMedicamentoItemPresentacion.DscItem = lMedicamentoItem[i].DscItem;
                            oMedicamentoItemPresentacion.DscSubItem = lMedicamentoItem[i].DscSubItem;
                            lMedicamentoItemPresentacion.Add(oMedicamentoItemPresentacion);
                        }
                    }
                    return new RespuestaBE<List<HOConsultarMedicacionItemsPresentacionBE>>()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = lMedicamentoItemPresentacion
                    };
                }
                else
                {
                    return new RespuestaBE<List<HOConsultarMedicacionItemsPresentacionBE>>()
                    {
                        rpt = oResponse.code,
                        mensaje = oResponse.message
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<HOConsultarMedicacionItemsPresentacionBE>>(ex, "WS", "Hospitalizacion.svc/ConsultarMedicacionItems");
                return ClasesGenericas.RegistrarErrorIntranet<List<HOConsultarMedicacionItemsPresentacionBE>>(ex, "WS", "Hospitalizacion.svc/ConsultarMedicacionItems");
            }
        }

        public RespuestaBE<List<HOConsultarDatosHospitalizacionPresentacionBE>> ConsultarDatosHospitalizacion(string tipoDocumento, string numeroDocumento, string idAmbulatorio)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                bool indHospitalizacion = ConfigurationManager.AppSettings["indicadorConectar_Hospitalizacion"].ToString().Equals("1");
                if (indHospitalizacion)
                {
                    #region Integración
                    String urlMetodo = "api/ConsultarDatosHospitalizacion";
                    List<HOConsultarDatosHospitalizacionPresentacionBE> lHospitalizacionPresentacion = new List<HOConsultarDatosHospitalizacionPresentacionBE>();

                    HOConsultarDatosHospitalizacionBE oRequest = new HOConsultarDatosHospitalizacionBE();
                    oRequest.ApiKey = _apiKeyHospitalizacion;
                    oRequest.idAmbulatorio = idAmbulatorio;

                    HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                    if (oResponse.success)
                    {
                        List<HOConsultarDatosHospitalizacionResponseBE> lHospitalizacion = new JavaScriptSerializer().Deserialize<List<HOConsultarDatosHospitalizacionResponseBE>>(oResponse.data);
                        if (lHospitalizacion != null)
                        {
                            HOConsultarDatosHospitalizacionPresentacionBE oHospitalizacionPresentacion;
                            int i = 0, nHospitalizacion = lHospitalizacion.Count;
                            for (; i < nHospitalizacion; i++)
                            {
                                oHospitalizacionPresentacion = new HOConsultarDatosHospitalizacionPresentacionBE();
                                oHospitalizacionPresentacion.codAtencion = lHospitalizacion[i].codAtencion;
                                oHospitalizacionPresentacion.fechaHora = lHospitalizacion[i].fechaHora;
                                oHospitalizacionPresentacion.idAmbulatorio = lHospitalizacion[i].idAmbulatorio;
                                oHospitalizacionPresentacion.grupoFamiliar = lHospitalizacion[i].grupoFamiliar;
                                oHospitalizacionPresentacion.codTipoDocumento = lHospitalizacion[i].codTipoDocumento;
                                oHospitalizacionPresentacion.desTipoDocumento = lHospitalizacion[i].desTipoDocumento;
                                oHospitalizacionPresentacion.numeroDocumento = lHospitalizacion[i].numeroDocumento;
                                oHospitalizacionPresentacion.fechaNacimiento = lHospitalizacion[i].fechaNacimiento;
                                oHospitalizacionPresentacion.nombres = lHospitalizacion[i].nombres;
                                oHospitalizacionPresentacion.apellidoPaterno = lHospitalizacion[i].apellidoPaterno;
                                oHospitalizacionPresentacion.apellidoMaterno = lHospitalizacion[i].apellidoMaterno;
                                oHospitalizacionPresentacion.correo = lHospitalizacion[i].correo;
                                oHospitalizacionPresentacion.telefono = lHospitalizacion[i].telefono;
                                oHospitalizacionPresentacion.sexo = lHospitalizacion[i].sexo;
                                oHospitalizacionPresentacion.codMedico = lHospitalizacion[i].codMedico;
                                oHospitalizacionPresentacion.nomMedico = lHospitalizacion[i].nomMedico;
                                oHospitalizacionPresentacion.codEspecialidad = lHospitalizacion[i].codEspecialidad;
                                oHospitalizacionPresentacion.desEspecialidad = lHospitalizacion[i].desEspecialidad;
                                oHospitalizacionPresentacion.nivelAtencion = lHospitalizacion[i].nivelAtencion;
                                oHospitalizacionPresentacion.codEstado = lHospitalizacion[i].codEstado;
                                oHospitalizacionPresentacion.desEstado = lHospitalizacion[i].desEstado;
                                oHospitalizacionPresentacion.ultimoPaso = lHospitalizacion[i].ultimoPaso;
                                oHospitalizacionPresentacion.codFamiliar = lHospitalizacion[i].codFamiliar;
                                oHospitalizacionPresentacion.desFamiliar = lHospitalizacion[i].desFamiliar;
                                lHospitalizacionPresentacion.Add(oHospitalizacionPresentacion);
                            }
                        }
                        return new RespuestaBE<List<HOConsultarDatosHospitalizacionPresentacionBE>>()
                        {
                            rpt = 0,
                            mensaje = "",
                            data = lHospitalizacionPresentacion
                        };
                    }
                    else
                    {
                        return new RespuestaBE<List<HOConsultarDatosHospitalizacionPresentacionBE>>()
                        {
                            rpt = oResponse.code,
                            mensaje = oResponse.message
                        };
                    }
                    #endregion
                } 
                else
                {
                    return new RespuestaBE<List<HOConsultarDatosHospitalizacionPresentacionBE>>()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = new List<HOConsultarDatosHospitalizacionPresentacionBE>()
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<List<HOConsultarDatosHospitalizacionPresentacionBE>>(ex, "WS", "Hospitalizacion.svc/ConsultarDatosHospitalizacion");
                return ClasesGenericas.RegistrarErrorIntranet<List<HOConsultarDatosHospitalizacionPresentacionBE>>(ex, "WS", "Hospitalizacion.svc/ConsultarDatosHospitalizacion");
            }
        }

        public RespuestaBE<HOConsultarAlergiasRiesgosPresentacionBE> ConsultarAlergiasMedicacionRiesgoCasa(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio)
                || string.IsNullOrEmpty(codAtencion))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                String urlMetodo = "api/ConsultarAlergiasMedicacionRiesgoCasa";
                HOConsultarAlergiasRiesgosPresentacionBE oAlergiasRiesgosPresentacion = new HOConsultarAlergiasRiesgosPresentacionBE();

                HOConsultarAlergiasRiesgosBE oRequest = new HOConsultarAlergiasRiesgosBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.idAmbulatorio = idAmbulatorio;
                oRequest.codAtencion = codAtencion;

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    HOConsultarAlergiasRiesgosResponseBE oAlergiasRiesgos = new JavaScriptSerializer().Deserialize<HOConsultarAlergiasRiesgosResponseBE>(oResponse.data);
                    if (oAlergiasRiesgos != null)
                    {
                        int i, nRegistros;
                        oAlergiasRiesgosPresentacion.indAlergia = oAlergiasRiesgos.indAlergia;
                        oAlergiasRiesgosPresentacion.hospitalizado = oAlergiasRiesgos.hospitalizado;
                        oAlergiasRiesgosPresentacion.representante = oAlergiasRiesgos.representante;
                        oAlergiasRiesgosPresentacion.habitacion = oAlergiasRiesgos.habitacion;
                        if (oAlergiasRiesgos.medicamentos != null)
                        {
                            HOCAMMedicamentoBE oMedicamento;
                            HOCAMMedicamentoPresentacionBE oMedicamentoPresentacion;
                            nRegistros = oAlergiasRiesgos.medicamentos.Count;
                            for (i = 0; i < nRegistros; i++)
                            {
                                oMedicamento = oAlergiasRiesgos.medicamentos[i];
                                oMedicamentoPresentacion = new HOCAMMedicamentoPresentacionBE();
                                oMedicamentoPresentacion.IdeAlergiaMedicamentosDet = oMedicamento.IdeAlergiaMedicamentosDet.ToString();
                                oMedicamentoPresentacion.Codigo = oMedicamento.Codigo;
                                oMedicamentoPresentacion.Descripcion = oMedicamento.Descripcion;
                                oAlergiasRiesgosPresentacion.medicamentos.Add(oMedicamentoPresentacion);
                            }
                        }
                        oAlergiasRiesgosPresentacion.alimentos = oAlergiasRiesgos.alimentos;
                        oAlergiasRiesgosPresentacion.otros = oAlergiasRiesgos.otros;
                        oAlergiasRiesgosPresentacion.indAlergia = oAlergiasRiesgos.indAlergia;
                        if (oAlergiasRiesgos.medicamentosRiesgos != null)
                        {
                            HOCAMMedicamentoRiesgoBE oMedicamentoRiesgo;
                            HOCAMMedicamentoRiesgoPresentacionBE oMedicamentoRiesgoPresentacion;
                            nRegistros = oAlergiasRiesgos.medicamentosRiesgos.Count;
                            for (i = 0; i < nRegistros; i++)
                            {
                                oMedicamentoRiesgo = oAlergiasRiesgos.medicamentosRiesgos[i];
                                oMedicamentoRiesgoPresentacion = new HOCAMMedicamentoRiesgoPresentacionBE();
                                oMedicamentoRiesgoPresentacion.IdeMedicacionDet = oMedicamentoRiesgo.IdeMedicacionDet;
                                oMedicamentoRiesgoPresentacion.Codigo = oMedicamentoRiesgo.Codigo.ToString();
                                oMedicamentoRiesgoPresentacion.Indicador = oMedicamentoRiesgo.Indicador;
                                oMedicamentoRiesgoPresentacion.DscItem = oMedicamentoRiesgo.DscItem;
                                oMedicamentoRiesgoPresentacion.DscSubItem = oMedicamentoRiesgo.DscSubItem;
                                oAlergiasRiesgosPresentacion.medicamentosRiesgos.Add(oMedicamentoRiesgoPresentacion);
                            }
                        }
                        oAlergiasRiesgosPresentacion.idRegistro = oAlergiasRiesgos.idRegistro.ToString();
                    }
                    return new RespuestaBE<HOConsultarAlergiasRiesgosPresentacionBE>()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = oAlergiasRiesgosPresentacion
                    };
                }
                else
                {
                    return new RespuestaBE<HOConsultarAlergiasRiesgosPresentacionBE>()
                    {
                        rpt = oResponse.code,
                        mensaje = oResponse.message
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HOConsultarAlergiasRiesgosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/ConsultarAlergiasMedicacionRiesgoCasa");
                return ClasesGenericas.RegistrarErrorIntranet<HOConsultarAlergiasRiesgosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/ConsultarAlergiasMedicacionRiesgoCasa");
            }
        }

        public RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE> GrabarAlergiasMedicacionRiesgoCasa(string tipoDocumento, string numeroDocumento, string idAmbulatorio,
            string codAtencion, string indAlergia, string hospitalizado,
            string representante, string habitacion, List<HOGRAMedicamentoPresentacionBE> medicamentos,
            string alimentos, string otros, List<HOGRAMedicamentoRiesgoPresentacionBE> medicamentosRiesgos,
            string accion, string idRegistro, string respEnvio,
            string codTipoDocumentoPaciente, string desTipoDocumentoPaciente, string numeroDocumentoPaciente,
            string nombresPaciente, string apellidoPaternoPaciente, string apellidoMaternoPaciente)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio)
                || string.IsNullOrEmpty(codAtencion) || string.IsNullOrEmpty(indAlergia) || string.IsNullOrEmpty(accion)
                || string.IsNullOrEmpty(idRegistro) || string.IsNullOrEmpty(desTipoDocumentoPaciente) || string.IsNullOrEmpty(numeroDocumentoPaciente)
                || string.IsNullOrEmpty(nombresPaciente) || string.IsNullOrEmpty(apellidoPaternoPaciente))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                {
                    rpt = 100,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento"
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                {
                    rpt = 101,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres"
                };
            }
            if (tipoDocumento.Equals("1"))
            {
                int varNumeroDocumento;
                if (!int.TryParse(numeroDocumento, out varNumeroDocumento))
                {
                    return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                    {
                        rpt = 102,
                        mensaje = "El parámetro numeroDocumento debe ser numérico"
                    };
                }
            }
            indAlergia = indAlergia.ToUpper().Trim();
            if (indAlergia != "S" && indAlergia != "N")
            {
                return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"S\" (SI) o \"N\" (NO) en el parámetro indAlergia"
                };
            }
            accion = accion.ToUpper().Trim();
            if (accion != "I" && accion != "A")
            {
                return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                {
                    rpt = 104,
                    mensaje = "Sólo se soportan los valores \"I\" (INSERTAR) o \"A\" (ACTUALIZAR) en el parámetro accion"
                };
            }
            idRegistro = idRegistro.Trim();
            int varIdRegistro;
            if (!int.TryParse(idRegistro, out varIdRegistro))
            {
                return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                {
                    rpt = 105,
                    mensaje = "El parámetro idRegistro debe ser numérico"
                };
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                hospitalizado = hospitalizado != null ? hospitalizado : "";
                representante = representante != null ? representante : "";
                habitacion = habitacion != null ? habitacion : "";
                medicamentos = medicamentos != null ? medicamentos : new List<HOGRAMedicamentoPresentacionBE>();
                medicamentosRiesgos = medicamentosRiesgos != null ? medicamentosRiesgos : new List<HOGRAMedicamentoRiesgoPresentacionBE>();
                respEnvio = respEnvio != null ? respEnvio : "";

                bool indicadorGuardarPDF = ClasesGenericas.GetSetting("Hospitalizacion_GuardarPDF").Equals("1"); // 0: No | 1: Si
                string paciente = nombresPaciente + " " + apellidoPaternoPaciente + (!String.IsNullOrEmpty(apellidoMaternoPaciente) ? " " + apellidoMaternoPaciente : "");
                int i, nRegistros;

                List<HOGRAMedicamentoBE> lMedicamentoRequest = new List<HOGRAMedicamentoBE>();
                HOGRAMedicamentoBE oMedicamentoRequest;
                nRegistros = medicamentos.Count;
                int varEntero;
                for (i = 0; i < nRegistros; i++)
                {
                    if (string.IsNullOrEmpty(medicamentos[i].IdeAlergiaMedicamentosDet) || 
                        string.IsNullOrEmpty(medicamentos[i].Codigo) ||
                        string.IsNullOrEmpty(medicamentos[i].Descripcion))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                    if (!int.TryParse(medicamentos[i].IdeAlergiaMedicamentosDet, out varEntero))
                    {
                        return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                        {
                            rpt = 200,
                            mensaje = "El parámetro IdeAlergiaMedicamentosDet debe ser numérico"
                        };
                    }
                    oMedicamentoRequest = new HOGRAMedicamentoBE();
                    oMedicamentoRequest.IdeAlergiaMedicamentosDet = int.Parse(medicamentos[i].IdeAlergiaMedicamentosDet);
                    oMedicamentoRequest.Codigo = medicamentos[i].Codigo;
                    oMedicamentoRequest.Descripcion = medicamentos[i].Descripcion;
                    lMedicamentoRequest.Add(oMedicamentoRequest);
                }

                List<HOGRAMedicamentoRiesgoBE> lMedicamentoRiesgoRequest = new List<HOGRAMedicamentoRiesgoBE>();
                HOGRAMedicamentoRiesgoBE oMedicamentoRiesgoRequest;
                nRegistros = medicamentosRiesgos.Count;
                for (i = 0; i < nRegistros; i++)
                {
                    if (string.IsNullOrEmpty(medicamentosRiesgos[i].IdeMedicacionDet) ||
                        string.IsNullOrEmpty(medicamentosRiesgos[i].Codigo) ||
                        string.IsNullOrEmpty(medicamentosRiesgos[i].Indicador) ||
                        string.IsNullOrEmpty(medicamentosRiesgos[i].DscItem) ||
                        string.IsNullOrEmpty(medicamentosRiesgos[i].DscSubItem))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                    if (!int.TryParse(medicamentosRiesgos[i].IdeMedicacionDet, out varEntero))
                    {
                        return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                        {
                            rpt = 201,
                            mensaje = "El parámetro IdeMedicacionDet debe ser numérico"
                        };
                    }
                    medicamentosRiesgos[i].Indicador = medicamentosRiesgos[i].Indicador.ToUpper().Trim();
                    if (medicamentosRiesgos[i].Indicador != "S" && 
                        medicamentosRiesgos[i].Indicador != "N")
                    {
                        return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                        {
                            rpt = 202,
                            mensaje = "Sólo se soportan los valores \"S\" (SI) o \"N\" (NO) en el parámetro Indicador"
                        };
                    }
                    oMedicamentoRiesgoRequest = new HOGRAMedicamentoRiesgoBE();
                    oMedicamentoRiesgoRequest.IdeMedicacionDet = medicamentosRiesgos[i].IdeMedicacionDet;
                    oMedicamentoRiesgoRequest.Codigo = int.Parse(medicamentosRiesgos[i].Codigo);
                    oMedicamentoRiesgoRequest.Indicador = medicamentosRiesgos[i].Indicador;
                    oMedicamentoRiesgoRequest.DscItem = medicamentosRiesgos[i].DscItem;
                    oMedicamentoRiesgoRequest.DscSubItem = medicamentosRiesgos[i].DscSubItem;
                    lMedicamentoRiesgoRequest.Add(oMedicamentoRiesgoRequest);
                }

                HttpServerUtility oServer = HttpContext.Current.Server;
                string archivoLogo = oServer.MapPath("~/Resources/Logos/clinicasanfelipe.png");
                string base64Logo = io.File.Exists(archivoLogo) ? Convert.ToBase64String(io.File.ReadAllBytes(archivoLogo)) : "";
                string base64PDFAlergia, base64PDFRiesgo;

                #region Documento Alergia
                string archivoDeclaratoriaAlergia = oServer.MapPath("~/Resources/Templates/TextoPDF/Hospitalizacion/DeclaratoriaAlergia.txt");
                string htmlAlergia = io.File.Exists(archivoDeclaratoriaAlergia) ? io.File.ReadAllText(archivoDeclaratoriaAlergia) : "";

                string strMedicamentos = "";
                nRegistros = medicamentos.Count;
                for (i = 0; i < nRegistros; i++)
                {
                    strMedicamentos += medicamentos[i].Descripcion;
                    strMedicamentos += ", ";
                }
                if (strMedicamentos != "")
                {
                    strMedicamentos = strMedicamentos.Substring(0, strMedicamentos.Length - 2);
                }

                htmlAlergia = htmlAlergia.Replace("@Logo", base64Logo);

                string estiloMarcado = "background-color:black;color:white;";
                htmlAlergia = htmlAlergia.Replace("@IndAlergiaSi", indAlergia.Equals("S") ? estiloMarcado : "");
                htmlAlergia = htmlAlergia.Replace("@IndAlergiaNo", indAlergia.Equals("N") ? estiloMarcado : "");

                htmlAlergia = htmlAlergia.Replace("@Paciente", paciente);
                htmlAlergia = htmlAlergia.Replace("@TipoDocumento", desTipoDocumentoPaciente);
                htmlAlergia = htmlAlergia.Replace("@NumeroDocumento", numeroDocumento);
                htmlAlergia = htmlAlergia.Replace("@Medicamentos", String.IsNullOrEmpty(strMedicamentos) ? "&nbsp; " : strMedicamentos);
                htmlAlergia = htmlAlergia.Replace("@Alimentos", String.IsNullOrEmpty(alimentos) ? "&nbsp; " : alimentos);
                htmlAlergia = htmlAlergia.Replace("@Otros", String.IsNullOrEmpty(otros) ? "&nbsp; " : otros);
                htmlAlergia = htmlAlergia.Replace("@FechaHoraActual", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                
                byte[] bufferAlergia = HtmlToPdf.ObtenerBuffer(htmlAlergia, oServer);
                if (indicadorGuardarPDF)
                {
                    string rutaPDF = io.Path.Combine(oServer.MapPath("~/Resources/PDF/Hospitalizacion"), String.Format("{0}_{1}_{2}_{3}_01_Alergias.pdf", tipoDocumento, numeroDocumento, idAmbulatorio, codAtencion));
                    io.File.WriteAllBytes(rutaPDF, bufferAlergia);
                }
                base64PDFAlergia = bufferAlergia != null ? Convert.ToBase64String(bufferAlergia) : "";
                #endregion

                #region Documento Riesgo
                string archivoMedicacionRiesgo = oServer.MapPath("~/Resources/Templates/TextoPDF/Hospitalizacion/MedicacionRiesgo.txt");
                string htmlRiesgo = io.File.Exists(archivoMedicacionRiesgo) ? io.File.ReadAllText(archivoMedicacionRiesgo) : "";

                string tablaRiesgo = "";
                tablaRiesgo += "<table class='table' style='width:100%;border-collapse:collapse;'>";
                tablaRiesgo += "<thead>";
                tablaRiesgo += "<tr>";
                tablaRiesgo += "<td rowspan='2' style='text-align:center'>¿Está usando la siguiente medicación?</td>";
                tablaRiesgo += "<td colspan='2' style='text-align:center'>X = Seleccionado</td>";
                tablaRiesgo += "</tr>";
                tablaRiesgo += "<tr>";
                tablaRiesgo += "<td style='text-align:center'>SI</td>";
                tablaRiesgo += "<td style='text-align:center'>NO</td>";
                tablaRiesgo += "</tr>";
                tablaRiesgo += "</thead>";
                tablaRiesgo += "<tbody>";

                nRegistros = medicamentosRiesgos.Count;
                for (i = 0; i < nRegistros; i++)
                {
                    tablaRiesgo += "<tr>";

                    tablaRiesgo += "<td>";
                    tablaRiesgo += medicamentosRiesgos[i].DscItem;
                    tablaRiesgo += "<br/>";
                    tablaRiesgo += medicamentosRiesgos[i].DscSubItem;
                    tablaRiesgo += "</td>";
                    tablaRiesgo += "<td style='text-align:center'>";
                    tablaRiesgo += medicamentosRiesgos[i].Indicador == "S" ? "X" : "";
                    tablaRiesgo += "</td>";
                    tablaRiesgo += "<td style='text-align:center'>";
                    tablaRiesgo += medicamentosRiesgos[i].Indicador == "N" ? "X" : "";
                    tablaRiesgo += "</td>";

                    tablaRiesgo += "</tr>";
                }
                tablaRiesgo += "</tbody>";
                tablaRiesgo += "</table>";

                htmlRiesgo = htmlRiesgo.Replace("@Logo", base64Logo);

                htmlRiesgo = htmlRiesgo.Replace("@MedicacionRiesgo", tablaRiesgo);
                htmlRiesgo = htmlRiesgo.Replace("@FechaHoraActual", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                byte[] bufferRiesgo = HtmlToPdf.ObtenerBuffer(htmlRiesgo, oServer);
                if (indicadorGuardarPDF)
                {
                    string rutaPDF = io.Path.Combine(oServer.MapPath("~/Resources/PDF/Hospitalizacion"), String.Format("{0}_{1}_{2}_{3}_02_Riesgos.pdf", tipoDocumento, numeroDocumento, idAmbulatorio, codAtencion));
                    io.File.WriteAllBytes(rutaPDF, bufferRiesgo);
                }
                base64PDFRiesgo = bufferRiesgo != null ? Convert.ToBase64String(bufferRiesgo) : "";
                #endregion

                String urlMetodo = "api/GrabarAlergiasMedicacionRiesgoCasa";
                List<HOConsultarMedicamentosPresentacionBE> lMedicamentoPresentacion = new List<HOConsultarMedicamentosPresentacionBE>();

                HOGrabarAlergiasRiesgosBE oRequest = new HOGrabarAlergiasRiesgosBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.idAmbulatorio = idAmbulatorio;
                oRequest.codAtencion = codAtencion;
                oRequest.indAlergia = indAlergia;
                oRequest.hospitalizado = hospitalizado;
                oRequest.representante = representante;
                oRequest.habitacion = habitacion;
                oRequest.medicamentos = lMedicamentoRequest;
                oRequest.alimentos = alimentos;
                oRequest.otros = otros;
                oRequest.medicamentosRiesgos = lMedicamentoRiesgoRequest;
                oRequest.documentoAlergias = base64PDFAlergia;
                oRequest.documentoRiesgos = base64PDFRiesgo;
                oRequest.accion = accion;
                oRequest.IdRegistro = int.Parse(idRegistro);
                oRequest.RespEnvio = respEnvio;

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    HOGrabarAlergiasRiesgosResponseBE oRpta = new JavaScriptSerializer().Deserialize<HOGrabarAlergiasRiesgosResponseBE>(oResponse.data);
                    if (oRpta != null)
                    {
                        HOGrabarAlergiasRiesgosPresentacionBE oRptaPresentacion = new HOGrabarAlergiasRiesgosPresentacionBE();
                        oRptaPresentacion.codEstado = oRpta.codEstado;
                        oRptaPresentacion.desEstado = oRpta.desEstado;
                        return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                        {
                            rpt = 0,
                            mensaje = "",
                            data = oRptaPresentacion
                        };
                    }
                }
                return new RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE>()
                {
                    rpt = oResponse.code,
                    mensaje = oResponse.message
                };
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HOGrabarAlergiasRiesgosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/GrabarAlergiasMedicacionRiesgoCasa");
                return ClasesGenericas.RegistrarErrorIntranet<HOGrabarAlergiasRiesgosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/GrabarAlergiasMedicacionRiesgoCasa");
            }
        }

        public RespuestaBE<HOConsultarTerminosPresentacionBE> ConsultarDocumentoAdmisionTerminos(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio)
                || string.IsNullOrEmpty(codAtencion))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                String urlMetodo = "api/ConsultarDocumentoAdmisionTerminos";
                HOConsultarTerminosPresentacionBE oTerminosPresentacion = new HOConsultarTerminosPresentacionBE();

                HOConsultarTerminosBE oRequest = new HOConsultarTerminosBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.idAmbulatorio = idAmbulatorio;
                oRequest.codAtencion = codAtencion;

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    HOConsultarTerminosResponseBE oTerminos = new JavaScriptSerializer().Deserialize<HOConsultarTerminosResponseBE>(oResponse.data);
                    int i, nRegistros;
                    if (oTerminos != null)
                    {
                        oTerminosPresentacion.indTerminosAdmision = oTerminos.indTerminosAdmision;
                        oTerminosPresentacion.idRegistro = oTerminos.idRegistro;
                        //oTerminosPresentacion.flg_autorizacion1 = oTerminos.flg_autorizacion1;
                        //oTerminosPresentacion.flg_autorizacion2 = oTerminos.flg_autorizacion2;
                        oTerminosPresentacion.opciones = new List<HOCTOpcionesPresentacionBE>();

                        //TextoBE oTextoBE = new ParametrizacionDA().ObtenerTexto(ClasesGenericas.GetSetting("Hospitalizacion_CodigoTerminos"));
                        var oRequestTmp = new
                        {
                            codigo = ClasesGenericas.GetSetting("Hospitalizacion_CodigoTerminos")
                        };
                        string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                        string responseTmp = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerTexto/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                        RespuestaBE<TextoBE> varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaBE<TextoBE>>(responseTmp);
                        if (varRespuestaTmp == null)
                        {
                            return new RespuestaBE<HOConsultarTerminosPresentacionBE>()
                            {
                                rpt = -1,
                                mensaje = "No se logró procesar la solicitud",
                                data = null
                            };
                        }
                        if (varRespuestaTmp.rpt != 0)
                        {
                            return new RespuestaBE<HOConsultarTerminosPresentacionBE>()
                            {
                                rpt = varRespuestaTmp.rpt,
                                mensaje = varRespuestaTmp.mensaje,
                                data = null
                            };
                        }
                        TextoBE oTextoBE = varRespuestaTmp.data;

                        if (oTextoBE != null)
                        {
                            oTextoBE.textosAdicionales = oTextoBE.textosAdicionales != null ? oTextoBE.textosAdicionales : new List<TextoDetalleBE>();
                            nRegistros = oTextoBE.textosAdicionales.Count;
                            string codigoRegistro = "", codigoBoton = "";
                            for (i = 0; i < nRegistros; i++)
                            {
                                codigoRegistro = oTextoBE.textosAdicionales[i].codigo;
                                codigoBoton = null;
                                switch (codigoRegistro)
                                {
                                    case "01":
                                        if (oTerminos.flg_autorizacion1 != null) codigoBoton = oTerminos.flg_autorizacion1.Equals("1") ? "S" : "N";
                                        break;
                                    case "02":
                                        if (oTerminos.flg_autorizacion2 != null) codigoBoton = oTerminos.flg_autorizacion2.Equals("1") ? "S" : "N";
                                        break;
                                    default:
                                        //return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                                        //{
                                        //    rpt = 200,
                                        //    mensaje = "Código " + opciones[i].codigoBoton + " inválido"
                                        //};
                                        codigoBoton = null;
                                        break;
                                }
                                oTerminosPresentacion.opciones.Add(new HOCTOpcionesPresentacionBE
                                {
                                    codigoRegistro = codigoRegistro,
                                    codigoBoton = codigoBoton
                                });
                            }
                        }
                    }
                    return new RespuestaBE<HOConsultarTerminosPresentacionBE>()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = oTerminosPresentacion
                    };
                }
                else
                {
                    return new RespuestaBE<HOConsultarTerminosPresentacionBE>()
                    {
                        rpt = oResponse.code,
                        mensaje = oResponse.message
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HOConsultarTerminosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/ConsultarDocumentoAdmisionTerminos");
                return ClasesGenericas.RegistrarErrorIntranet<HOConsultarTerminosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/ConsultarDocumentoAdmisionTerminos");
            }
        }

        public RespuestaBE<HOGrabarTerminosPresentacionBE> GrabarDocumentoAdmisionTerminos(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion, string indTerminosAdmision, string dscTexto, 
            string dscRpta, List<HOGTextoAdicionalPresentacionBE> opciones, string accion,
            string idRegistro, string responsableEnvio, string codTipoDocumentoPaciente,
            string desTipoDocumentoPaciente, string numeroDocumentoPaciente, string nombresPaciente,
            string apellidoPaternoPaciente, string apellidoMaternoPaciente)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio)
                || string.IsNullOrEmpty(codAtencion) || string.IsNullOrEmpty(indTerminosAdmision) || string.IsNullOrEmpty(accion)
                || string.IsNullOrEmpty(idRegistro) || string.IsNullOrEmpty(desTipoDocumentoPaciente) || string.IsNullOrEmpty(numeroDocumentoPaciente)
                || string.IsNullOrEmpty(nombresPaciente) || string.IsNullOrEmpty(apellidoPaternoPaciente))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                {
                    rpt = 100,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento"
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                {
                    rpt = 101,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres"
                };
            }
            if (tipoDocumento.Equals("1"))
            {
                int varNumeroDocumento;
                if (!int.TryParse(numeroDocumento, out varNumeroDocumento))
                {
                    return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                    {
                        rpt = 102,
                        mensaje = "El parámetro numeroDocumento debe ser numérico"
                    };
                }
            }
            indTerminosAdmision = indTerminosAdmision.ToUpper().Trim();
            if (indTerminosAdmision != "S" && indTerminosAdmision != "N")
            {
                return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"S\" (SI) o \"N\" (NO) en el parámetro indAlergia"
                };
            }
            accion = accion.ToUpper().Trim();
            if (accion != "I" && accion != "A")
            {
                return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                {
                    rpt = 104,
                    mensaje = "Sólo se soportan los valores \"I\" (INSERTAR) o \"A\" (ACTUALIZAR) en el parámetro accion"
                };
            }
            idRegistro = idRegistro.Trim();
            int varIdRegistro;
            if (!int.TryParse(idRegistro, out varIdRegistro))
            {
                return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                {
                    rpt = 105,
                    mensaje = "El parámetro idRegistro debe ser numérico"
                };
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                opciones = opciones != null ? opciones : new List<HOGTextoAdicionalPresentacionBE>();

                bool indicadorGuardarPDF = ClasesGenericas.GetSetting("Hospitalizacion_GuardarPDF").Equals("1"); // 0: No | 1: Si
                string paciente = apellidoPaternoPaciente + (!String.IsNullOrEmpty(apellidoMaternoPaciente) ? " " + apellidoMaternoPaciente : "") + ", " + nombresPaciente,
                       htmlTextoAdicional = "";
                int flg_autorizacion1 = 0, flg_autorizacion2 = 0;
                int i, nRegistros;
                //TextoBE oTextoBE = new ParametrizacionDA().ObtenerTexto(ClasesGenericas.GetSetting("Hospitalizacion_CodigoTerminos")); //Contenido HTML de Admisión Términos
                var oRequestTmp = new
                {
                    codigo = ClasesGenericas.GetSetting("Hospitalizacion_CodigoTerminos")
                };
                string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                string responseTmp = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerTexto/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<TextoBE> varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaBE<TextoBE>>(responseTmp);
                if (varRespuestaTmp == null)
                {
                    return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuestaTmp.rpt != 0)
                {
                    return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                    {
                        rpt = varRespuestaTmp.rpt,
                        mensaje = varRespuestaTmp.mensaje,
                        data = null
                    };
                }
                TextoBE oTextoBE = varRespuestaTmp.data;

                nRegistros = opciones.Count;
                for (i = 0; i < nRegistros; i++)
                {
                    opciones[i].codigoRegistro = opciones[i].codigoRegistro != null ? opciones[i].codigoRegistro : "";
                    opciones[i].codigoBoton = opciones[i].codigoBoton != null ? opciones[i].codigoBoton : "N";
                    switch (opciones[i].codigoRegistro)
                    {
                        case "01":
                            flg_autorizacion1 = opciones[i].codigoBoton.Equals("S") ? 1 : 0;
                            break;
                        case "02":
                            flg_autorizacion2 = opciones[i].codigoBoton.Equals("S") ? 1 : 0;
                            break;
                        default:
                            return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                            {
                                rpt = 200,
                                mensaje = "Código " + opciones[i].codigoBoton + " inválido"
                            };
                    }
                    for (int j = 0; j < oTextoBE.textosAdicionales.Count; j++)
                    {
                        if (oTextoBE.textosAdicionales[j].codigo.Equals(opciones[i].codigoRegistro))
                        {
                            for (int k = 0; k < oTextoBE.textosAdicionales[j].botones.Count; k++)
                            {
                                if (oTextoBE.textosAdicionales[j].botones[k].codigo.Equals(opciones[i].codigoBoton))
                                {
                                    htmlTextoAdicional += oTextoBE.textosAdicionales[j].exportarPDF.Replace("@Respuesta", oTextoBE.textosAdicionales[j].botones[k].descripcionPDF);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                HttpServerUtility oServer = HttpContext.Current.Server;
                string archivoLogo = oServer.MapPath("~/Resources/Logos/clinicasanfelipe.png");
                string base64Logo = io.File.Exists(archivoLogo) ? Convert.ToBase64String(io.File.ReadAllBytes(archivoLogo)) : "";
                string base64PDFTerminos;

                #region Documento Términos
                string archivoTerminos = oServer.MapPath("~/Resources/Templates/TextoPDF/Hospitalizacion/Terminos.txt");
                string htmlTerminos = io.File.Exists(archivoTerminos) ? io.File.ReadAllText(archivoTerminos) : "";

                htmlTerminos = htmlTerminos.Replace("@Logo", base64Logo);
                htmlTerminos = htmlTerminos.Replace("@Terminos", oTextoBE.exportarPDF); //Primero
                htmlTerminos = htmlTerminos.Replace("@TextosAdicionales", htmlTextoAdicional); //Segundo
                htmlTerminos = htmlTerminos.Replace("@Paciente", paciente);
                htmlTerminos = htmlTerminos.Replace("@TipoDocumento", desTipoDocumentoPaciente);
                htmlTerminos = htmlTerminos.Replace("@NumeroDocumento", numeroDocumento);
                htmlTerminos = htmlTerminos.Replace("@NumeroAtencion", codAtencion);
                htmlTerminos = htmlTerminos.Replace("@FechaHoraActual", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                byte[] bufferTerminos = HtmlToPdf.ObtenerBuffer(htmlTerminos, oServer);
                if (indicadorGuardarPDF)
                {
                    string rutaPDF = io.Path.Combine(oServer.MapPath("~/Resources/PDF/Hospitalizacion"), String.Format("{0}_{1}_{2}_{3}_03_Terminos.pdf", tipoDocumento, numeroDocumento, idAmbulatorio, codAtencion));
                    io.File.WriteAllBytes(rutaPDF, bufferTerminos);
                }
                base64PDFTerminos = bufferTerminos != null ? Convert.ToBase64String(bufferTerminos) : "";
                #endregion

                String urlMetodo = "api/GrabarDocumentoAdmisionTerminos";

                HOGrabarTerminosBE oRequest = new HOGrabarTerminosBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.idAmbulatorio = idAmbulatorio;
                oRequest.codAtencion = codAtencion;
                oRequest.indTerminosAdmision = indTerminosAdmision;
                oRequest.documento = base64PDFTerminos;
                oRequest.DscTexto = dscTexto;
                oRequest.DscRpta = dscRpta;
                oRequest.flg_autorizacion1 = flg_autorizacion1;
                oRequest.flg_autorizacion2 = flg_autorizacion2;
                oRequest.accion = accion;
                oRequest.ResponsableEnvio = responsableEnvio;
                oRequest.idRegistro = int.Parse(idRegistro);
                
                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    HOGrabarAlergiasRiesgosResponseBE oRpta = new JavaScriptSerializer().Deserialize<HOGrabarAlergiasRiesgosResponseBE>(oResponse.data);
                    if (oRpta != null)
                    {
                        HOGrabarTerminosPresentacionBE oRptaPresentacion = new HOGrabarTerminosPresentacionBE();
                        oRptaPresentacion.codEstado = oRpta.codEstado;
                        oRptaPresentacion.desEstado = oRpta.desEstado;
                        return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                        {
                            rpt = 0,
                            mensaje = "",
                            data = oRptaPresentacion
                        };
                    }
                }
                return new RespuestaBE<HOGrabarTerminosPresentacionBE>()
                {
                    rpt = oResponse.code,
                    mensaje = oResponse.message
                };
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HOGrabarTerminosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/GrabarDocumentoAdmisionTerminos");
                return ClasesGenericas.RegistrarErrorIntranet<HOGrabarTerminosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/GrabarDocumentoAdmisionTerminos");
            }
        }

        public RespuestaBE<HOConsultarContactosPresentacionBE> ConsultarContactosAutorizadosPreFacturas(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio)
                || string.IsNullOrEmpty(codAtencion))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                HOConsultarContactosPresentacionBE oHOConsultarContactosPresentacionBE = new HOConsultarContactosPresentacionBE();
                oHOConsultarContactosPresentacionBE.idRegistroCab = "0";
                oHOConsultarContactosPresentacionBE.personas = new List<HOCContactosPersonaPreBE>();
                oHOConsultarContactosPresentacionBE.textos = new List<HOCCTextosPreBE>();
                int i, nRegistros;

                string parametrosContenido = "*¯CONTENIDO¯hospitalizacion-paso3-importante-superior¬hospitalizacion-paso3-importante-inferior";
                //List<ContenidoBE> lContenidoBE = new ParametrizacionDA().ObtenerContenidos(parametrosContenido);
                var oRequestTmp = new
                {
                    parametrosContenido = parametrosContenido
                };
                string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
                string responseTmp = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerContenidos/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<List<ContenidoBE>> varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ContenidoBE>>>(responseTmp);
                if (varRespuestaTmp == null)
                {
                    return new RespuestaBE<HOConsultarContactosPresentacionBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuestaTmp.rpt != 0)
                {
                    return new RespuestaBE<HOConsultarContactosPresentacionBE>()
                    {
                        rpt = varRespuestaTmp.rpt,
                        mensaje = varRespuestaTmp.mensaje,
                        data = null
                    };
                }
                List<ContenidoBE> lContenidoBE = varRespuestaTmp.data;

                nRegistros = lContenidoBE.Count;
                HOCCTextosPreBE oHOCCTextosPreBE;
                for (i = 0; i < lContenidoBE.Count; i++)
                {
                    oHOCCTextosPreBE = new HOCCTextosPreBE();
                    oHOCCTextosPreBE.codigo = lContenidoBE[i].codigo;
                    oHOCCTextosPreBE.descripcion = lContenidoBE[i].contenido;
                    oHOConsultarContactosPresentacionBE.textos.Add(oHOCCTextosPreBE);
                }

                parametrosContenido = "HOSPITALIZACION¯PASO_3_CONTACTOS¯";
                //lContenidoBE = new ParametrizacionDA().ObtenerContenidos(parametrosContenido);
                var oRequestTmp2 = new
                {
                    parametrosContenido = parametrosContenido
                };
                string strRequestTmp2 = new JavaScriptSerializer().Serialize(oRequestTmp2);
                string responseTmp2 = ClasesGenericas.PostAsyncIntranet("Parametrizacion.svc/ObtenerContenidos/", strRequestTmp2, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
                RespuestaBE<List<ContenidoBE>> varRespuestaTmp2 = new JavaScriptSerializer().Deserialize<RespuestaBE<List<ContenidoBE>>>(responseTmp2);
                if (varRespuestaTmp2 == null)
                {
                    return new RespuestaBE<HOConsultarContactosPresentacionBE>()
                    {
                        rpt = -1,
                        mensaje = "No se logró procesar la solicitud",
                        data = null
                    };
                }
                if (varRespuestaTmp2.rpt != 0)
                {
                    return new RespuestaBE<HOConsultarContactosPresentacionBE>()
                    {
                        rpt = varRespuestaTmp2.rpt,
                        mensaje = varRespuestaTmp2.mensaje,
                        data = null
                    };
                }
                lContenidoBE = varRespuestaTmp2.data;

                nRegistros = lContenidoBE.Count;
                HOCContactosPersonaPreBE oHOCContactosPersonaPreBE;
                for (i = 0; i < nRegistros; i++)
                {
                    oHOCContactosPersonaPreBE = new HOCContactosPersonaPreBE();
                    oHOCContactosPersonaPreBE.idRegistroDet = "0";
                    oHOCContactosPersonaPreBE.nro_orden = lContenidoBE[i].codigo;
                    oHOCContactosPersonaPreBE.descripcionContacto = lContenidoBE[i].contenido;
                    oHOConsultarContactosPresentacionBE.personas.Add(oHOCContactosPersonaPreBE);
                }

                String urlMetodo = "api/ConsultarContactosAutorizadosPreFacturas";

                HOConsultarContactosBE oRequest = new HOConsultarContactosBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.idAmbulatorio = idAmbulatorio;
                oRequest.codAtencion = codAtencion;

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    HOConsultarContactosResponseBE oContactosResponse = new JavaScriptSerializer().Deserialize<HOConsultarContactosResponseBE>(oResponse.data);
                    if (oContactosResponse != null)
                    {
                        oHOConsultarContactosPresentacionBE.idRegistroCab = oContactosResponse.IdRegistroCab;
                        if (oContactosResponse.Persona != null)
                        {
                            HOCContactosPersonaResBE oHOCContactosPersonaResBE;
                            //HOCAMMedicamentoPresentacionBE oMedicamentoPresentacion;
                            nRegistros = oContactosResponse.Persona.Count;
                            for (i = 0; i < nRegistros; i++)
                            {
                                oHOCContactosPersonaResBE = oContactosResponse.Persona[i];
                                for (int j = 0; j < 3; j++)
                                {
                                    if (oHOConsultarContactosPresentacionBE.personas[j].nro_orden.Equals(oHOCContactosPersonaResBE.nro_orden))
                                    {
                                        oHOConsultarContactosPresentacionBE.personas[j].idRegistroDet = oHOCContactosPersonaResBE.IdRegistroDet;
                                        oHOConsultarContactosPresentacionBE.personas[j].codTipoDocumento = oHOCContactosPersonaResBE.TipoDocumento;
                                        //oHOConsultarContactosPresentacionBE.personas[j].desTipoDocumento = oHOCContactosPersonaResBE.dni;
                                        oHOConsultarContactosPresentacionBE.personas[j].numeroDocumento = oHOCContactosPersonaResBE.dni;
                                        oHOConsultarContactosPresentacionBE.personas[j].apellidoPaterno = oHOCContactosPersonaResBE.apellidoPaterno;
                                        oHOConsultarContactosPresentacionBE.personas[j].apellidoMaterno = oHOCContactosPersonaResBE.apellidoMaterno;
                                        oHOConsultarContactosPresentacionBE.personas[j].nombres = oHOCContactosPersonaResBE.nombres;
                                        oHOConsultarContactosPresentacionBE.personas[j].correo = oHOCContactosPersonaResBE.correo;
                                        oHOConsultarContactosPresentacionBE.personas[j].celular = oHOCContactosPersonaResBE.telefono;
                                        oHOConsultarContactosPresentacionBE.personas[j].codParentesco = oHOCContactosPersonaResBE.codparentesco;
                                        oHOConsultarContactosPresentacionBE.personas[j].desParentesco = oHOCContactosPersonaResBE.parentesco;
                                        oHOConsultarContactosPresentacionBE.personas[j].flg_copia = oHOCContactosPersonaResBE.flg_copia;
                                        break;
                                    }
                                }
                            }
                        }
                        
                    }
                    return new RespuestaBE<HOConsultarContactosPresentacionBE>()
                    {
                        rpt = 0,
                        mensaje = "",
                        data = oHOConsultarContactosPresentacionBE
                    };
                }
                else
                {
                    return new RespuestaBE<HOConsultarContactosPresentacionBE>()
                    {
                        rpt = oResponse.code,
                        mensaje = oResponse.message
                    };
                }
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HOConsultarContactosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/ConsultarContactosAutorizadosPreFacturas");
                return ClasesGenericas.RegistrarErrorIntranet<HOConsultarContactosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/ConsultarContactosAutorizadosPreFacturas");
            }
        }

        public RespuestaBE<HOGrabarContactosPresentacionBE> GrabarContactosAutorizadosPreFacturas(string tipoDocumento, string numeroDocumento, string idAmbulatorio,
            string codAtencion, List<HOGContactosPersonaReqPresentacionBE> personas, string accion,
            string idRegistro, string responsableEnvio, string codTipoDocumentoPaciente,
            string desTipoDocumentoPaciente, string numeroDocumentoPaciente, string nombresPaciente,
            string apellidoPaternoPaciente, string apellidoMaternoPaciente)
        {
            #region Validacion de Parámetros
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(idAmbulatorio)
                || string.IsNullOrEmpty(codAtencion) || string.IsNullOrEmpty(accion) || string.IsNullOrEmpty(idRegistro)
                || string.IsNullOrEmpty(desTipoDocumentoPaciente) || string.IsNullOrEmpty(numeroDocumentoPaciente) || string.IsNullOrEmpty(nombresPaciente) 
                || string.IsNullOrEmpty(apellidoPaternoPaciente))
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
            if (tipoDocumento != "1" && tipoDocumento != "2" && tipoDocumento != "3")
            {
                return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                {
                    rpt = 100,
                    mensaje = "Sólo se soportan los valores \"1\" (DNI), \"2\" (CE) o \"3\" (PAS) en el parámetro tipoDocumento"
                };
            }
            if (numeroDocumento.Length > 20)
            {
                return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                {
                    rpt = 101,
                    mensaje = "El parámetro numeroDocumento no puede tener más de 20 caracteres"
                };
            }
            if (tipoDocumento.Equals("1"))
            {
                int varNumeroDocumento;
                if (!int.TryParse(numeroDocumento, out varNumeroDocumento))
                {
                    return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                    {
                        rpt = 102,
                        mensaje = "El parámetro numeroDocumento debe ser numérico"
                    };
                }
            }
            accion = accion.ToUpper().Trim();
            if (accion != "I" && accion != "A")
            {
                return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                {
                    rpt = 103,
                    mensaje = "Sólo se soportan los valores \"I\" (INSERTAR) o \"A\" (ACTUALIZAR) en el parámetro accion"
                };
            }
            idRegistro = idRegistro.Trim();
            int varIdRegistro;
            if (!int.TryParse(idRegistro, out varIdRegistro))
            {
                return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                {
                    rpt = 104,
                    mensaje = "El parámetro idRegistro debe ser numérico"
                };
            }
            #endregion
            try
            {
                WebOperationContext oContext = WebOperationContext.Current;
                if (!fnTokenSesionValido(oContext, tipoDocumento, numeroDocumento))
                {
                    oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                personas = personas != null ? personas : new List<HOGContactosPersonaReqPresentacionBE>();

                bool indicadorGuardarPDF = ClasesGenericas.GetSetting("Hospitalizacion_GuardarPDF").Equals("1"); // 0: No | 1: Si
                string paciente = nombresPaciente + " " + apellidoPaternoPaciente + (!String.IsNullOrEmpty(apellidoMaternoPaciente) ? " " + apellidoMaternoPaciente : ""),
                       htmlTablaContactoResponsable = "", htmlTablaContactosAdicionales = "";
                int i, nRegistros;

                List<HOGContactosPersonaReqBE> lPersonaRequest = new List<HOGContactosPersonaReqBE>();
                HOGContactosPersonaReqBE oPersonaRequest;
                nRegistros = personas.Count;
                int varEntero;
                for (i = 0; i < nRegistros; i++)
                {
                    if (string.IsNullOrEmpty(personas[i].idRegistroDet) ||
                        string.IsNullOrEmpty(personas[i].apellidoPaterno) ||
                        string.IsNullOrEmpty(personas[i].nombres) ||
                        string.IsNullOrEmpty(personas[i].numeroDocumento) ||
                        string.IsNullOrEmpty(personas[i].codTipoDocumento) ||
                        string.IsNullOrEmpty(personas[i].correo) ||
                        string.IsNullOrEmpty(personas[i].celular) ||
                        string.IsNullOrEmpty(personas[i].codParentesco) ||
                        string.IsNullOrEmpty(personas[i].desParentesco) ||
                        string.IsNullOrEmpty(personas[i].nro_orden) ||
                        string.IsNullOrEmpty(personas[i].flg_copia))
                    {
                        oContext.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                    if (!int.TryParse(personas[i].idRegistroDet, out varEntero))
                    {
                        return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                        {
                            rpt = 200,
                            mensaje = "El parámetro idRegistroDet debe ser numérico"
                        };
                    }
                    if (!int.TryParse(personas[i].celular, out varEntero))
                    {
                        return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                        {
                            rpt = 201,
                            mensaje = "El parámetro celular debe ser numérico"
                        };
                    }
                    if (!int.TryParse(personas[i].nro_orden, out varEntero))
                    {
                        return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                        {
                            rpt = 202,
                            mensaje = "El parámetro nro_orden debe ser numérico"
                        };
                    }
                    if (!int.TryParse(personas[i].flg_copia, out varEntero))
                    {
                        return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                        {
                            rpt = 203,
                            mensaje = "El parámetro flg_copia debe ser numérico"
                        };
                    }
                    oPersonaRequest = new HOGContactosPersonaReqBE();
                    oPersonaRequest.IdRegistroDet = int.Parse(personas[i].idRegistroDet);
                    oPersonaRequest.apellidos = personas[i].apellidoPaterno + (!String.IsNullOrEmpty(personas[i].apellidoMaterno) ? " " + personas[i].apellidoMaterno : "");
                    oPersonaRequest.apellidoPaterno = personas[i].apellidoPaterno;
                    oPersonaRequest.apellidoMaterno = personas[i].apellidoMaterno;
                    oPersonaRequest.nombres = personas[i].nombres;
                    oPersonaRequest.dni = personas[i].numeroDocumento;
                    oPersonaRequest.TipoDocumento = personas[i].codTipoDocumento;
                    oPersonaRequest.correo = personas[i].correo;
                    oPersonaRequest.telefono = personas[i].celular;
                    oPersonaRequest.codparentesco = personas[i].codParentesco;
                    oPersonaRequest.parentesco = personas[i].desParentesco;
                    oPersonaRequest.nro_orden = int.Parse(personas[i].nro_orden);
                    oPersonaRequest.flg_copia = int.Parse(personas[i].flg_copia);
                    lPersonaRequest.Add(oPersonaRequest);
                }

                HttpServerUtility oServer = HttpContext.Current.Server;
                string archivoLogo = oServer.MapPath("~/Resources/Logos/clinicasanfelipe.png");
                string base64Logo = io.File.Exists(archivoLogo) ? Convert.ToBase64String(io.File.ReadAllBytes(archivoLogo)) : "";
                string base64PDFContactoResponsable, base64PDFContactosAdicionales;

                #region Documento Contactos
                string archivoContactoResponsable = oServer.MapPath("~/Resources/Templates/TextoPDF/Hospitalizacion/ContactoResponsable.txt");
                string archivoContactosAdicionales = oServer.MapPath("~/Resources/Templates/TextoPDF/Hospitalizacion/ContactosAdicionales.txt");
                string htmlContactoResponsable = io.File.Exists(archivoContactoResponsable) ? io.File.ReadAllText(archivoContactoResponsable) : "";
                string htmlContactosAdicinales = io.File.Exists(archivoContactosAdicionales) ? io.File.ReadAllText(archivoContactosAdicionales) : "";

                htmlContactoResponsable = htmlContactoResponsable.Replace("@Logo", base64Logo);
                htmlContactosAdicinales = htmlContactosAdicinales.Replace("@Logo", base64Logo);

                htmlTablaContactoResponsable += "<table class='table' style='width:100%;border-collapse:collapse;'>";
                htmlTablaContactoResponsable += "<thead>";
                htmlTablaContactoResponsable += "<tr>";
                htmlTablaContactoResponsable += "<td  style='text-align:center'>Apellidos</td>";
                htmlTablaContactoResponsable += "<td  style='text-align:center'>Nombres</td>";
                htmlTablaContactoResponsable += "<td  style='text-align:center'>DNI</td>";
                htmlTablaContactoResponsable += "<td  style='text-align:center'>Parentesco</td>";
                htmlTablaContactoResponsable += "<td  style='text-align:center'>Correo</td>";
                htmlTablaContactoResponsable += "<td  style='text-align:center'>Celular</td>";
                htmlTablaContactoResponsable += "</tr>";
                htmlTablaContactoResponsable += "</thead>";
                htmlTablaContactoResponsable += "<tbody>";

                htmlTablaContactosAdicionales += "<table class='table' style='width:100%;border-collapse:collapse;'>";
                htmlTablaContactosAdicionales += "<thead>";
                htmlTablaContactosAdicionales += "<tr>";
                htmlTablaContactosAdicionales += "<td  style='text-align:center'>Apellidos</td>";
                htmlTablaContactosAdicionales += "<td  style='text-align:center'>Nombres</td>";
                htmlTablaContactosAdicionales += "<td  style='text-align:center'>DNI</td>";
                htmlTablaContactosAdicionales += "<td  style='text-align:center'>Parentesco</td>";
                htmlTablaContactosAdicionales += "<td  style='text-align:center'>Correo</td>";
                htmlTablaContactosAdicionales += "<td  style='text-align:center'>Celular</td>";
                htmlTablaContactosAdicionales += "</tr>";
                htmlTablaContactosAdicionales += "</thead>";
                htmlTablaContactosAdicionales += "<tbody>";

                nRegistros = personas.Count;
                for (i = 0; i < nRegistros; i++)
                {
                    if (i == 0)//personas[i].nro_orden.Equals("1"))
                    {
                        htmlTablaContactoResponsable += "<tr>";
                        htmlTablaContactoResponsable += "<td>";
                        htmlTablaContactoResponsable += personas[i].apellidoPaterno + (!String.IsNullOrEmpty(personas[i].apellidoMaterno) ? " " + personas[i].apellidoMaterno : "");
                        htmlTablaContactoResponsable += "</td>";
                        htmlTablaContactoResponsable += "<td >";
                        htmlTablaContactoResponsable += personas[i].nombres;
                        htmlTablaContactoResponsable += "</td>";
                        htmlTablaContactoResponsable += "<td >";
                        htmlTablaContactoResponsable += personas[i].numeroDocumento;
                        htmlTablaContactoResponsable += "</td>";
                        htmlTablaContactoResponsable += "<td >";
                        htmlTablaContactoResponsable += personas[i].desParentesco;
                        htmlTablaContactoResponsable += "</td>";
                        htmlTablaContactoResponsable += "<td >";
                        htmlTablaContactoResponsable += personas[i].correo;
                        htmlTablaContactoResponsable += "</td>";
                        htmlTablaContactoResponsable += "<td >";
                        htmlTablaContactoResponsable += personas[i].celular;
                        htmlTablaContactoResponsable += "</td>";
                        htmlTablaContactoResponsable += "</tr>";
                    }
                    else
                    {
                        htmlTablaContactosAdicionales += "<tr>";
                        htmlTablaContactosAdicionales += "<td>";
                        htmlTablaContactosAdicionales += personas[i].apellidoPaterno + (!String.IsNullOrEmpty(personas[i].apellidoMaterno) ? " " + personas[i].apellidoMaterno : "");
                        htmlTablaContactosAdicionales += "</td>";
                        htmlTablaContactosAdicionales += "<td >";
                        htmlTablaContactosAdicionales += personas[i].nombres;
                        htmlTablaContactosAdicionales += "</td>";
                        htmlTablaContactosAdicionales += "<td >";
                        htmlTablaContactosAdicionales += personas[i].numeroDocumento;
                        htmlTablaContactosAdicionales += "</td>";
                        htmlTablaContactosAdicionales += "<td >";
                        htmlTablaContactosAdicionales += personas[i].desParentesco;
                        htmlTablaContactosAdicionales += "</td>";
                        htmlTablaContactosAdicionales += "<td >";
                        htmlTablaContactosAdicionales += personas[i].correo;
                        htmlTablaContactosAdicionales += "</td>";
                        htmlTablaContactosAdicionales += "<td >";
                        htmlTablaContactosAdicionales += personas[i].celular;
                        htmlTablaContactosAdicionales += "</td>";
                        htmlTablaContactosAdicionales += "</tr>";
                    }


                }
                htmlTablaContactoResponsable += "</tbody>";
                htmlTablaContactoResponsable += "</table>";

                htmlTablaContactosAdicionales += "</tbody>";
                htmlTablaContactosAdicionales += "</table>";

                htmlContactoResponsable = htmlContactoResponsable.Replace("@Contactos", htmlTablaContactoResponsable);
                htmlContactoResponsable = htmlContactoResponsable.Replace("@FechaHoraActual", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                htmlContactosAdicinales = htmlContactosAdicinales.Replace("@Contactos", htmlTablaContactosAdicionales);
                htmlContactosAdicinales = htmlContactosAdicinales.Replace("@FechaHoraActual", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                byte[] bufferContactoResponsable = HtmlToPdf.ObtenerBuffer(htmlContactoResponsable, oServer);
                byte[] bufferContactosAdicionales = HtmlToPdf.ObtenerBuffer(htmlContactosAdicinales, oServer);
                if (indicadorGuardarPDF)
                {
                    string rutaPDF = io.Path.Combine(oServer.MapPath("~/Resources/PDF/Hospitalizacion"), String.Format("{0}_{1}_{2}_{3}_04_ContactoResponsable.pdf", tipoDocumento, numeroDocumento, idAmbulatorio, codAtencion));
                    io.File.WriteAllBytes(rutaPDF, bufferContactoResponsable);

                    rutaPDF = io.Path.Combine(oServer.MapPath("~/Resources/PDF/Hospitalizacion"), String.Format("{0}_{1}_{2}_{3}_05_ContactosAdicionales.pdf", tipoDocumento, numeroDocumento, idAmbulatorio, codAtencion));
                    io.File.WriteAllBytes(rutaPDF, bufferContactosAdicionales);
                }
                base64PDFContactoResponsable = bufferContactoResponsable != null ? Convert.ToBase64String(bufferContactoResponsable) : "";
                base64PDFContactosAdicionales = bufferContactosAdicionales != null ? Convert.ToBase64String(bufferContactosAdicionales) : "";
                #endregion

                String urlMetodo = "api/GrabarContactosAutorizadosPreFacturas";

                HOGrabarContactosRequestBE oRequest = new HOGrabarContactosRequestBE();
                oRequest.ApiKey = _apiKeyHospitalizacion;
                oRequest.idAmbulatorio = idAmbulatorio;
                oRequest.codAtencion = codAtencion;
                oRequest.personas = lPersonaRequest;
                oRequest.documento1 = base64PDFContactoResponsable;
                oRequest.documento2 = base64PDFContactosAdicionales;
                oRequest.accion = accion;
                oRequest.idRegistro = int.Parse(idRegistro);
                oRequest.ResponsableEnvio = responsableEnvio;

                HOGenericoBE oResponse = fnPostAsync(urlMetodo, oRequest);

                if (oResponse.success)
                {
                    HOGrabarContactosResponseBE oRpta = new JavaScriptSerializer().Deserialize<HOGrabarContactosResponseBE>(oResponse.data);
                    if (oRpta != null)
                    {
                        HOGrabarContactosPresentacionBE oRptaPresentacion = new HOGrabarContactosPresentacionBE();
                        oRptaPresentacion.codEstado = oRpta.codEstado;
                        oRptaPresentacion.desEstado = oRpta.desEstado;
                        return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                        {
                            rpt = 0,
                            mensaje = "",
                            data = oRptaPresentacion
                        };
                    }
                }
                return new RespuestaBE<HOGrabarContactosPresentacionBE>()
                {
                    rpt = oResponse.code,
                    mensaje = oResponse.message
                };
            }
            catch (Exception ex)
            {
                //return new ErrorDA().RegistrarError<HOGrabarContactosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/GrabarContactosAutorizadosPreFacturas");
                return ClasesGenericas.RegistrarErrorIntranet<HOGrabarContactosPresentacionBE>(ex, "WS", "Hospitalizacion.svc/GrabarContactosAutorizadosPreFacturas");
            }
        }

        private bool fnTokenSesionValido(WebOperationContext oContext, string tipoDocumento, string numeroDocumento)
        {
            bool esTokenValido = true;
            String tokenSesion = ClasesGenericas.GetHeader(oContext.IncomingRequest.Headers, "token");
            //UsuarioDocumentoBE oUsuarioDocumento = new UsuarioDA().ObtenerPorToken(tokenSesion, tipoDocumento, numeroDocumento);
            var oRequestTmp = new
            {
                tokenSesion = tokenSesion,
                tipoDocumento = tipoDocumento,
                numeroDocumento = numeroDocumento,
                validarFamiliar = false
            };
            string strRequestTmp = new JavaScriptSerializer().Serialize(oRequestTmp);
            string responseTmp = ClasesGenericas.PostAsyncIntranet("Usuario.svc/ObtenerPorToken/", strRequestTmp, ClasesGenericas.GetHeader(WebOperationContext.Current.IncomingRequest.Headers, "token"));
            RespuestaBE<UsuarioDocumentoBE> varRespuestaTmp = new JavaScriptSerializer().Deserialize<RespuestaBE<UsuarioDocumentoBE>>(responseTmp);
            if (varRespuestaTmp == null)
            {
                esTokenValido = false;
            } 
            else
            {
                UsuarioDocumentoBE oUsuarioDocumento = varRespuestaTmp.data;
                if (!(oUsuarioDocumento.tipoDocumento == tipoDocumento
                    && oUsuarioDocumento.numeroDocumento == numeroDocumento))
                {
                    esTokenValido = false;
                }
            }
            return esTokenValido;
        }

        private HOGenericoBE fnPostAsync(String urlMetodo, object oRequest)
        {
            HOGenericoBE oResponse = new HOGenericoBE();
            try
            {
                using (HttpClient oCliente = new HttpClient())
                {
                    oCliente.BaseAddress = new Uri(_urlBaseHospitalizacion);

                    String contenidoJson = new JavaScriptSerializer().Serialize(oRequest);
                    StringContent content = new StringContent(contenidoJson, Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseWS = oCliente.PostAsync(urlMetodo, content);
                    responseWS.Wait();

                    HttpResponseMessage result = responseWS.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        Task<String> response = result.Content.ReadAsStringAsync();
                        response.Wait();

                        oResponse.success = true;
                        oResponse.data = response.Result;
                    }
                    else
                    {
                        oResponse.code = 1;
                        oResponse.message = "Error en comunicación con el servicio de hospitalización";
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.code = 2;
                oResponse.message = "Error en comunicación con el servicio de hospitalización - " + ex.Message;
            }
            return oResponse;
        }
    }
}
