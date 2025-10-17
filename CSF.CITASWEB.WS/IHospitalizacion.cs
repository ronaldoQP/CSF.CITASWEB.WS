using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSF.CITASWEB.WS.BE;
using System.ServiceModel.Web;
using System.IO;

namespace CSF.CITASWEB.WS
{
    [ServiceContract]
    public interface IHospitalizacion
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarMedicamentos/")]
        RespuestaBE<List<HOConsultarMedicamentosPresentacionBE>> ConsultarMedicamentos(string tipoDocumento, string numeroDocumento, string descripcion);
        
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarMedicacionItems/")]
        RespuestaBE<List<HOConsultarMedicacionItemsPresentacionBE>> ConsultarMedicacionItems(string tipoDocumento, string numeroDocumento, string orden);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarDatosHospitalizacion/")]
        RespuestaBE<List<HOConsultarDatosHospitalizacionPresentacionBE>> ConsultarDatosHospitalizacion(string tipoDocumento, string numeroDocumento, string idAmbulatorio);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarAlergiasMedicacionRiesgoCasa/")]
        RespuestaBE<HOConsultarAlergiasRiesgosPresentacionBE> ConsultarAlergiasMedicacionRiesgoCasa(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/GrabarAlergiasMedicacionRiesgoCasa/")]
        RespuestaBE<HOGrabarAlergiasRiesgosPresentacionBE> GrabarAlergiasMedicacionRiesgoCasa(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion, string indAlergia, string hospitalizado, 
            string representante, string habitacion, List<HOGRAMedicamentoPresentacionBE> medicamentos, 
            string alimentos, string otros, List<HOGRAMedicamentoRiesgoPresentacionBE> medicamentosRiesgos, 
            string accion, string idRegistro, string respEnvio,
            string codTipoDocumentoPaciente, string desTipoDocumentoPaciente, string numeroDocumentoPaciente,
            string nombresPaciente, string apellidoPaternoPaciente, string apellidoMaternoPaciente);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarDocumentoAdmisionTerminos/")]
        RespuestaBE<HOConsultarTerminosPresentacionBE> ConsultarDocumentoAdmisionTerminos(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/GrabarDocumentoAdmisionTerminos/")]
        RespuestaBE<HOGrabarTerminosPresentacionBE> GrabarDocumentoAdmisionTerminos(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion, string indTerminosAdmision, string dscTexto, 
            string dscRpta, List<HOGTextoAdicionalPresentacionBE> opciones, string accion,
            string idRegistro, string responsableEnvio, string codTipoDocumentoPaciente, 
            string desTipoDocumentoPaciente, string numeroDocumentoPaciente, string nombresPaciente, 
            string apellidoPaternoPaciente, string apellidoMaternoPaciente);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarContactosAutorizadosPreFacturas/")]
        RespuestaBE<HOConsultarContactosPresentacionBE> ConsultarContactosAutorizadosPreFacturas(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/GrabarContactosAutorizadosPreFacturas/")]
        RespuestaBE<HOGrabarContactosPresentacionBE> GrabarContactosAutorizadosPreFacturas(string tipoDocumento, string numeroDocumento, string idAmbulatorio, 
            string codAtencion, List<HOGContactosPersonaReqPresentacionBE> personas, string accion, 
            string idRegistro, string responsableEnvio, string codTipoDocumentoPaciente, 
            string desTipoDocumentoPaciente, string numeroDocumentoPaciente, string nombresPaciente, 
            string apellidoPaternoPaciente, string apellidoMaternoPaciente);

    }
}
