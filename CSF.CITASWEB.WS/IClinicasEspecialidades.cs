using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using CSF.CITASWEB.WS.BE;

namespace CSF.CITASWEB.WS
{
    [ServiceContract]
    public interface IClinicasEspecialidades
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarClinicasConDetalle/")]
        RespuestaBE<List<CiudadClinicaBE>> ListarClinicasConDetalle(string ciudad);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarClinicas/")]
        RespuestaBE<List<ClinicaSimpleBE>> ListarClinicas();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EspecialidadPorClinica/")]
        RespuestaBE<List<EspecialidadBE>> EspecialidadPorClinica(string idClinica);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ClinicaPorEspecialidad/")]
        RespuestaBE<List<ClinicaSimpleBE>> ClinicaPorEspecialidad(string idEspecialidad);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarClinicasVirtuales/")]
        RespuestaBE<List<ClinicaSimpleBE>> ListarClinicasVirtuales();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EspecialidadPorClinicaVirtual/")]
        RespuestaBE<List<EspecialidadBE>> EspecialidadPorClinicaVirtual(string idClinica);

        #region Métodos deshabilitados
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarClinicasMedico/")]
        RespuestaBE<List<ClinicaSimpleBE>> ListarClinicasMedico(string cmp);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EspecialidadPorClinicaVirtualEC/")]
        RespuestaBE<List<EspecialidadBE>> EspecialidadPorClinicaVirtualEC();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EspecialidadesFrecuentes/")]
        RespuestaBE<List<EspecialidadFrecuenteBE>> EspecialidadesFrecuentes(string idClinica, string tipoDocumento, string numeroDocumento, string tipoCita);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ClinicaEspecialidadAgrupado/")]
        RespuestaBE<List<ClinicaEspecialidadAgrupadoBE>> ClinicaEspecialidadAgrupado(string idClinica, string tipoCita);
        #endregion
    }
}
