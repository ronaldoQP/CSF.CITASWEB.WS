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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IParametrizacion" in both code and config file together.
    [ServiceContract]
    public interface IParametrizacion
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/IniciarAplicacion/")]
        RespuestaBE<VersionAplicacionBE> IniciarAplicacion(string tipoDispositivo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarParametros/")]
        RespuestaBE<List<ParametroBE>> ListarParametros();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarSeguros/")]
        RespuestaBE<List<SeguroBE>> ListarSeguros();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerTerminos/")]
        RespuestaBE<TerminosBE> ObtenerTerminos(string tipo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/DatosPorRuc/")]
        RespuestaBE<DataRuc> DatosPorRuc(string ruc);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerUbigeo/")]
        RespuestaBE<List<UbigeoBE>> ObtenerUbigeo(String distrito);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerIndicadorPopUpHome/")]
        RespuestaBE<IndicadorPopUpHomeBE> ObtenerIndicadorPopUpHome();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ParametrosRegistrarUsuario/")]
        RespuestaBE<RUDatosBE> ParametrosRegistrarUsuario();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarPaises/")]
        RespuestaBE<List<PaisBE>> ListarPaises();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarDepartamentos/")]
        RespuestaBE<List<DepartamentoBE>> ListarDepartamentos(string idPais);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarProvincias/")]
        RespuestaBE<List<ProvinciaBE>> ListarProvincias(string idPais, string idDepartamento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarDistritos/")]
        RespuestaBE<List<DistritoBE>> ListarDistritos(string idPais, string idDepartamento, string idProvincia);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerTexto/")]
        RespuestaBE<TextoBE> ObtenerTexto(string codigo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/DatosPorDocumento/")]
        RespuestaBE<DataDocumento> DatosPorDocumento(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerParametrosHospitalizacion/")]
        RespuestaBE<HODatosBE> ObtenerParametrosHospitalizacion();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerParametrosEnlaces/")]
        RespuestaBE<ENDatosBE> ObtenerParametrosEnlaces();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerDisponibilidadWS/")]
        RespuestaSimpleBE ObtenerDisponibilidadWS();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerContenido/")]
        RespuestaBE<ContenidoBE> ObtenerContenido(string codigo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerParametroSeguridad/")]
        RespuestaBE<ParametroSeguridadBE> ObtenerParametroSeguridad(string tipoRegistro);
    }
}
