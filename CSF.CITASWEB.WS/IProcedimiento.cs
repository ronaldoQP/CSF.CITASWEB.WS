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
    public interface IProcedimiento
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarCpt/")]
        RespuestaBE<List<PROC_ListarCptResponseBE>> ListarCpt(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarAnestesiologo/")]
        RespuestaBE<List<PROC_ListarAnestesiologoResponseBE>> ListarAnestesiologo(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarProcedimiento/")]
        RespuestaSimpleBE RegistrarProcedimiento(string tipoDocumento, string numeroDocumento, string idClinica,
                                        string codMedico, string idSubespecialidad, string idServicio, 
                                        string codAtencion, string codTipoPaciente, string fecha, 
                                        string horaInicio, string horaFin, string duracion,
                                        string codCpt, string cpt, string codSegus, 
                                        string segus, string guarismo, string rucSeguro, 
                                        string iafas, bool flgCartaGarantia, string cartaGarantia,
                                        bool flgPresupuesto, string presupuesto, string celular,
                                        bool flgAlergia, string coaseguro, string correo, 
                                        string observacion, string origen, string codEstado,
                                        string idOrdenDetalle, string totalSesiones);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarProcedimiento/")]
        RespuestaSimpleBE ActualizarProcedimiento(string tipoDocumento, string numeroDocumento, string idOrdenDetalle,
                                        string codCpt, string cpt, string codSegus,
                                        string segus, string guarismo, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AnularProcedimiento/")]
        RespuestaSimpleBE AnularProcedimiento(string tipoDocumento, string numeroDocumento, string idOrdenDetalle,
            string origen);

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //RequestFormat = WebMessageFormat.Json,
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest,
        //UriTemplate = "/RegistrarCitaProcedimiento/")]
        //RespuestaSimpleBE RegistrarCitaProcedimiento(string tipoDocumento, string numeroDocumento, string idHorarioDetalle,
        //                                string fecha, string numeroTurno, string origen, string observaciones,
        //                                string tipoCita, string esChequeo, string tipoCobertura, string horaInicio = "", string duracion = "", string codigoComponente = "",
        //                                List<beRespuestaImagen> lRespuestaImagen = null, string archivoConsentimiento = "", string nombreArchivoConsentimiento = "", string idCitaOriginal = "",
        //                                string origenOpcion = "");
    }
}
