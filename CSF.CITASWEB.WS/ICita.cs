using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSF.CITASWEB.WS.BE;
using System.ServiceModel.Web;
using System.IO;
using System.Threading.Tasks;

namespace CSF.CITASWEB.WS
{
    [ServiceContract]
    public interface ICita
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarCita/")]
        RespuestaSimpleBE RegistrarCita(string tipoDocumento, string numeroDocumento, string idHorarioDetalle,
                                        string fecha, string numeroTurno, string origen, string observaciones,
                                        string tipoCita, string esChequeo, string tipoCobertura, string horaInicio = "", string duracion = "", string codigoComponente = "",
                                        List<beRespuestaImagen> lRespuestaImagen = null, string archivoConsentimiento = "", string nombreArchivoConsentimiento = "", string idCitaOriginal = "",
                                        string origenOpcion = "");


        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarCitaApp/")]
        RespuestaBE<RegistrarCitaBE> RegistrarCitaApp(string tipoDocumento, string numeroDocumento, string idHorarioDetalle,
                                        string fecha, string numeroTurno, string origen, string observaciones,
                                        string tipoCita, string esChequeo, string idCitaOriginal);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarCitaAdicional/")]
        RespuestaSimpleBE RegistrarCitaAdicional(string tipoDocumento, string numeroDocumento, string idHorarioDetalle,
                                        string fecha, string horaInicio, string origen, string observaciones, string esChequeo,
                                        string idCitaOriginal, string origenOpcion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarCitas/")]
        RespuestaBE<CitasListadoBE> ListarCitas(string tipoDocumento, string numeroDocumento, string año, string origen, string fechaCita, string indicadorHistoricas, string indicadorPendientes, bool soloTitular);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ReprogramarCita/")]
        RespuestaSimpleBE ReprogramarCita(string tipoDocumento, string numeroDocumento, string idCita,
                                        string idHorarioDetalle, string fecha, string numeroTurno, string origen,
                                        string observaciones, string tipoCita, string esChequeo, string tipoCobertura,
                                        string origenOpcion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ReprogramarCitaApp/")]
        RespuestaBE<RegistrarCitaBE> ReprogramarCitaApp(string tipoDocumento, string numeroDocumento, string idCita,
                                        string idHorarioDetalle, string fecha, string numeroTurno, string origen,
                                        string observaciones, string tipoCita, string esChequeo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AnularCita/")]
        RespuestaSimpleBE AnularCita(string tipoDocumento, string numeroDocumento, string idCita, string origen, bool indReprogramacion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarCitaVirtual/")]
        RespuestaBE<CitaVirtualBE> RegistrarCitaVirtual(string tipoDocumento, string numeroDocumento, string idHorarioDetalle,
                                        string fecha, string numeroTurno, string preguntaPaciente1, string respuestaPaciente1,
                                        string preguntaPaciente2, string respuestaPaciente2, string origen,
                                        string tieneAlergia, string descripcionAlergia, string horaInicio,
                                        string origenOpcion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AnularCitaVirtual/")]
        RespuestaSimpleBE AnularCitaVirtual(string tipoDocumento, string numeroDocumento, string idCitaVirtual, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ReprogramarCitaVirtual/")]
        RespuestaBE<CitaVirtualBE> ReprogramarCitaVirtual(string tipoDocumento, string numeroDocumento, string idCitaVirtual,
                                        string idHorarioDetalle, string fecha, string numeroTurno, string preguntaPaciente1, string respuestaPaciente1,
                                        string preguntaPaciente2, string respuestaPaciente2,
                                        string tieneAlergia, string descripcionAlergia, string origen,
                                        string origenOpcion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDatosPago/")]
        RespuestaBE<DatosPagoBE> ObtenerDatosPago(string idCitaPresencial, string idCita, string canal, string ruc, string idCobertura);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarPagoCita/")]
        RespuestaSimpleBE RegistrarPagoCita(string idCitaPresencial, string idCita, string codigo, string mensaje, string fecha,
                                                    string hora, long purchaseNumber, string transactionID, string numeroTarjeta,
                                                    string deseaBoleta, string ruc, string razonSocial, string direccion, string origen,
                                                    string monto, string IDUnico, string tokenEmail, string nombreVisa, string apellidoVisa,
                                                    string firma, string tipoTarjeta, string tipoDocumentoBoleta, string numeroDocumentoBoleta,
                                                    string nombresBoleta, string apellidoPaternoBoleta, string apellidoMaternoBoleta,
                                                    string direccionBoleta, string fechaNacimientoBoleta, string celularBoleta,
                                                    string emailBoleta,
                                                    string RUCSeguro, string codigoCobertura, string origenMonto,
                                                    string fechaPago, string codigoProducto, string IAFAS,
                                                    string codigoParentesco, string codigoAfiliado, string tipoDocumentoContratante,
                                                    string numeroDocumentoContratante, string codigoTipoPago, bool indNoProcesarSiteds,
                                                    string tipoDocumentoUsuario, string numeroDocumentoUsuario, string tokenTarjeta,
                                                    string nombresTarjeta, string apellidosTarjeta, string merchantBuyerId,
                                                    string payMethod, bool esIPN);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AnularPagoCita/")]
        RespuestaSimpleBE AnularPagoCita(string idCita, string idCitaVirtual, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/IniciarCitaVirtual/")]
        RespuestaBE<DatosRoomBE> IniciarCitaVirtual(string idCita);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarFilaEspera/")]
        RespuestaBE<List<FilaEsperaBE>> ConsultarFilaEspera(string idCitaPresencial, string idCitaVirtual);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EstadoChat/")]
        RespuestaBE<EstadoTeleconsulta> EstadoChat();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/PreguntasFrecuentes/")]
        RespuestaBE<List<PreguntaFrecuenteBE>> PreguntasFrecuentes();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/PreguntasTriaje/")]
        RespuestaBE<List<PreguntaTriajeBE>> PreguntasTriaje(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RespuestasTriaje/")]
        RespuestaBE<bool> RespuestasTriaje(List<RespuestaTriaje> respuestas, string tipoDocumento, string numeroDocumento, string idEspecialidad, string cmp);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultaSeguro/")]
        RespuestaBE<List<PreguntaFrecuenteBE>> ConsultaSeguro(int tipoDocumento, string numeroDocumento, string ruc, string sunasa, string iafas, string seguro);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/IndicadorVideoLlamada/")]
        RespuestaSimpleBE IndicadorVideoLlamada(string idCitaVirtual);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerResultados/")]
        RespuestaBE<List<DataResponseLaboratorio>> ObtenerResultados(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerResultadosDocumento/")]
        RespuestaBE<ResultadoLaboratorio> ObtenerResultadosDocumento(string peticion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistroPagoRoyal/")]
        bool RegistroPagoRoyal(string idCita, string trajeta, string tipoTarjeta, int tipo, string fechaOperacion = "", string horaOperacion = "");

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/WebServiceTest/")]
        bool WebServiceTest(string idCita);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarPagoCitaMontoCero/")]
        RespuestaSimpleBE RegistrarPagoCitaMontoCero(string idCitaPresencial, string idCita);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistroPagoRoyalVirtual/")]
        RespuestaSimpleBE RegistroPagoRoyalVirtual(string idCita, string firma);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerResultadosTabla/")]
        RespuestaBE<List<DataResponseLaboratorio>> ObtenerResultadosTabla(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistroSiteds/")]
        RespuestaSimpleBE RegistroSiteds(string data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/TipoCoberturaEspecialidadListar/")]
        RespuestaBE<List<DataResponseTipoCoberturaEspecialidad>> TipoCoberturaEspecialidadListar(string codigo, string rucSeguro);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ProcesarPagoSynap/")]
        string ProcesarPagoSynap(OrderPago order, PaymentPago payment, ResultPago result, string signature, string success, Message message);
        /*string ProcesarPagoSynap(
            string _type, beAnswer answer, string applicationProvider, 
            string applicationVersion, string metadata, string mode, 
            string serverDate, string serverUrl, string status,
            string ticket, string version, string webService
        );*/


        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerRecetaMedica/")]
        RespuestaBE<ResultadoLaboratorio> ObtenerRecetaMedica(string idCita, string numeroOrden);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerTokenPagoIzipay/")]
        RespuestaBE<TokenIzipayBE> ObtenerTokenPagoIzipay(
            string idCitaPresencial, string idCita, string canal, 
            string ruc, int formTokenVersion, string idCobertura,
            string monto, string origenMonto, string fechaPago,
            string codigoProducto, string IAFAS, string codigoParentesco,
            string codigoAfiliado, string tipoDocumentoContratante, string numeroDocumentoContratante,
            string deseaBoleta, string rucFactura, string razonSocialFactura, string direccionFactura,
            string tipoDocumentoBoleta, string numeroDocumentoBoleta, string nombresBoleta,
            string apellidoPaternoBoleta, string apellidoMaternoBoleta, string direccionBoleta,
            string fechaNacimientoBoleta, string celularBoleta, string emailBoleta,
            string origen, string tipoDocumentoUsuario, string numeroDocumentoUsuario,
            string tokenTarjeta, string merchantBuyerId
        );

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerTokenPagoIzipayNew/")]
        RespuestaBE<TokenIzipayBE> ObtenerTokenPagoIzipayOld(
            string idCitaPresencial, string idCita, string canal,
            string ruc, int formTokenVersion, string idCobertura,
            string monto, string origenMonto, string fechaPago,
            string codigoProducto, string IAFAS, string codigoParentesco,
            string codigoAfiliado, string tipoDocumentoContratante, string numeroDocumentoContratante,
            string deseaBoleta, string rucFactura, string razonSocialFactura, string direccionFactura,
            string tipoDocumentoBoleta, string numeroDocumentoBoleta, string nombresBoleta,
            string apellidoPaternoBoleta, string apellidoMaternoBoleta, string direccionBoleta,
            string fechaNacimientoBoleta, string celularBoleta, string emailBoleta,
            string origen
        );

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDatosIzipaySDK/")]
        RespuestaBE<IzipaySDKBE> ObtenerDatosIzipaySDK(string idCitaPresencial, string idCita, string canal);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerTokenPagoIzipayFarmacia/")]
        RespuestaBE<TokenIzipayBE> ObtenerTokenPagoIzipayFarmacia(string numeroPedido, string tipoDocumento, string numeroDocumento, string monto, string correo, string idClinica, string canal, int formTokenVersion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDatosIzipaySDKFarmacia/")]
        RespuestaBE<IzipaySDKBE> ObtenerDatosIzipaySDKFarmacia(string numeroPedido, string idClinica, string canal);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerProtocoloChequeoMedico/")]
        RespuestaBE<ProtocoloChequeoMedicoBE> ObtenerProtocoloChequeoMedico(string tipoDocumento, String numeroDocumento, string clinicaId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EnviarCorreoImagenes/")]
        RespuestaSimpleBE EnviarCorreoImagenes(String idCita, String archivoConsentimiento, String nombreArchivoConsentimiento);


        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //RequestFormat = WebMessageFormat.Json,
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest,
        //UriTemplate = "/CrearCalendarioCita/")]
        //RespuestaSimpleBE CrearCalendarioCita(string nombreArchivo, string fechaCreacion, string asunto, string fechaInicio, string fechaFin, string descripcion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ServicioInfoListar/")]
        RespuestaBE<List<ServicioInfo>> ServicioInfoListar(string idClinica);


        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EnviarOTP/")]
        RespuestaBE<EnviarOTPBE> EnviarOTP(string tipoDocumento, string numeroDocumento, string correo, 
            bool indicadorReenvio, string tipo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarOTP/")]
        RespuestaBE<ActualizarOTPBE> ActualizarOTP(string tipoDocumento, string numeroDocumento, string codigoOTP);

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //RequestFormat = WebMessageFormat.Json,
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest,
        //UriTemplate = "/ReenviarOTP/")]
        //RespuestaSimpleBE ReenviarOTP(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarOTP/")]
        RespuestaSimpleBE ValidarOTP(string tipoDocumento, string numeroDocumento, string codigoOTP,
            string tipo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarPagoCitaSynapsis/")]
        RespuestaSimpleBE RegistrarPagoCitaSynapsis(string idCitaPresencial, string idCita, string codigo, string mensaje, string fecha,
                                                    string hora, long purchaseNumber, string transactionID, string numeroTarjeta,
                                                    string deseaBoleta, string ruc, string razonSocial, string direccion, string origen,
                                                    string monto, string IDUnico, string tokenEmail, string nombreVisa, string apellidoVisa,
                                                    string firma, string tipoTarjeta);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDatosPorHorario/")]
        RespuestaBE<ClinicaConsultorioBE> ObtenerDatosPorHorario(string idHorario, string tipoAtencion, bool esAdicional,
            string idServicioHorario, bool esProcedimiento);


        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarAccionCita/")]
        RespuestaSimpleBE ValidarAccionCita(string idCitaPresencial, string idCita, string tipoAccion, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDatosVideollamada/")]
        RespuestaBE<VideollamadaBE> ObtenerDatosVideollamada(string idCita);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDatosVideollamadaMedico/")]
        RespuestaBE<VideollamadaMedicoBE> ObtenerDatosVideollamadaMedico(string room_name);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarNovedades/")]
        RespuestaBE<List<NovedadesBE>> ListarNovedades(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/CrearTurnoPaciente/")]
        RespuestaSimpleBE CrearTurnoPaciente(string idCitaPresencial, string idCitaVirtual, string origen, string origenBMatic, string idUsuario, string observaciones);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarOpcionesPago/")]
        RespuestaBE<ListarOpcionesPagoBE> ListarOpcionesPago(string tipoDocumento, string numeroDocumento, string idCitaPresencial, string idCitaVirtual, string tipoPaciente,
            string requierePasarela, string nuevaPasarela);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarRoomSid/")]
        RespuestaSimpleBE RegistrarRoomSid(string idCitaVirtual, string origen, string room_sid, bool esMedico);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultaVideollamadaPendiente/")]
        RespuestaBE<VideollamadaPendienteBE> ConsultaVideollamadaPendiente(string type, string status);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarEstadoVideollamada/")]
        RespuestaSimpleBE ActualizarEstadoVideollamada(string type, string status, string room_name, List<VideollamadaTwilioSidBE> twilio_room_sid);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ProcesarPagoIzipay/")]
        RespuestaSimpleBE ProcesarPagoIzipay(beAnswer krAnswer);

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest,
        //UriTemplate = "/ProcesarPagoIzipay/")]
        //RespuestaSimpleBE ProcesarPagoIzipay(Stream oStream);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarOpcionesPagoV2/")]
        RespuestaBE<ListarOpcionesPagoBE> ListarOpcionesPagoV2(string tipoDocumento, string numeroDocumento, string idCitaPresencial, string idCitaVirtual, string tipoPaciente);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ReprogramarCitaAdicional/")]
        RespuestaSimpleBE ReprogramarCitaAdicional(string tipoDocumento, string numeroDocumento, string idCita,
                                        string idHorarioDetalle, string fecha, string horaInicio, string origen,
                                        string observaciones, string tipoCita, string esChequeo, string tipoCobertura,
                                        string origenOpcion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarQR/")]
        RespuestaBE<List<ObjetoQRBE>> ListarQR(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerReporte/")]
        RespuestaSimpleBE ObtenerReporte(string tipoDocumento, string numeroDocumento, string codigoAtencion, string codigoPaciente, string tipoReporte, string codigoCpt);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerFlyer/")]
        RespuestaBE<IndicadorFlyerBE> ObtenerFlyer(string idCita, string idCitaVirtual);

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //RequestFormat = WebMessageFormat.Json,
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest,
        //UriTemplate = "/RegistrarLog/")]
        //RespuestaSimpleBE RegistrarLog(string idCita, string idCitaVirtual, string metodo, 
        //                                string accion, string mensaje);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarEstado/")]
        RespuestaSimpleBE ActualizarEstado(string tipoDocumento, string numeroDocumento, string idCita, string idCitaVirtual, string origen,
            string estado, string idUsuario, string observaciones);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerPorId/")]
        RespuestaBE<CitaChatBotBE> ObtenerPorId(string tipoDocumento, string numeroDocumento, string idCita, string idCitaVirtual);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerPeriodos/")]
        RespuestaBE<List<IM_ListarPeriodoResPresentacionBE>> ObtenerPeriodos(string tipoDocumento, string numeroDocumento, string idAmbulatorio);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarImagenes/")]
        RespuestaBE<List<IM_ListarImagenesResPresentacionBE>> ListarImagenes(string data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerResultadosImagenes/")]
        RespuestaSimpleBE ObtenerResultadosImagenes(string data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerVueMotion/")]
        RespuestaSimpleBE ObtenerVueMotion(string data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarResultadosLaboratorio/")]
        RespuestaBE<List<LB_ListarResultadosResPresentacionBE>> ListarResultadosLaboratorio(string tipoDocumento, string numeroDocumento, string tipoDocumentoPaciente, 
            string numeroDocumentoPaciente, string periodo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerResultadoLaboratorio/")]
        RespuestaSimpleBE ObtenerResultadoLaboratorio(string tipoDocumento, string numeroDocumento, string ordenAtencion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerHistoricoLaboratorio/")]
        RespuestaSimpleBE ObtenerHistoricoLaboratorio(string tipoDocumento, string numeroDocumento, string ordenAtencion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerTokenIzipay/")]
        RespuestaBE<IzipaySimpleBE> ObtenerTokenIzipay(string tipoDocumento, string numeroDocumento, string origen,
            string idCita, string idCitaVirtual);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarTarjetas/")]
        RespuestaBE<List<UsuarioTarjetaBE>> ListarTarjetas(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarTarjeta/")]
        RespuestaSimpleBE RegistrarTarjeta(string tipoDocumento, string numeroDocumento, string numeroTarjeta,
            string tokenTarjeta, string tipoTarjeta, string nombres, string apellidos, string correo, string merchantBuyerId, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarEstadoTarjeta/")]
        RespuestaSimpleBE ActualizarEstadoTarjeta(string tipoDocumento, string numeroDocumento, string idUsuarioTarjeta,
            string codigoEstado, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ProcesarPagoIzipayApp/")]
        //RespuestaSimpleBE ProcesarPagoIzipayApp(IPNIzipayBE oIPNIzipayBE); //Método no acepta un solo objeto
        RespuestaSimpleBE ProcesarPagoIzipayApp(string code, string message, string messageUser,
            string messageUserEng, IIResponseBE response, string payloadHttp,
            string signature, string transactionId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ProcesarPagoIzipayWeb/")]
        //RespuestaSimpleBE ProcesarPagoIzipayWeb(IPNIzipayBE oIPNIzipayBE);
        RespuestaSimpleBE ProcesarPagoIzipayWeb(string code, string message, string messageUser,
            string messageUserEng, IIResponseBE response, string payloadHttp,
            string signature, string transactionId);
    }
}
