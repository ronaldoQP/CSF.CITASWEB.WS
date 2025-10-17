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
    public interface IUsuario
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarTokenPush/")]
        RespuestaSimpleBE ActualizarTokenPush(string tipoDocumento, string numeroDocumento, string tokenPush);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/Autenticar/")]
        RespuestaBE<AutenticacionBE> Autenticar(string tipoDocumento, string numeroDocumento, string password,
                                                        string tipoDispositivo, string imei, string tokenPush, string tokenPushHms);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AutenticarWeb/")]
        RespuestaBE<AutenticacionBE> AutenticarWeb(string tipoDocumento, string numeroDocumento, string password);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarExistenciaUsuario/")]
        RespuestaBE<UsuarioBE> ValidarExistenciaUsuario(string data);//string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarExistenciaUsuarioPaciente/")]
        RespuestaBE<UsuarioBE> ValidarExistenciaUsuarioPaciente(string data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarExistenciaPaciente/")]
        RespuestaBE<UsuarioBE> ValidarExistenciaPaciente(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RecuperarPassword/")]
        RespuestaSimpleBE RecuperarPassword(string tipoDocumento, string numeroDocumento, string metodo, string correo, string password, string codigoOTP,
            string tipoOTP, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarUsuario/")]
        RespuestaSimpleBE RegistrarUsuario(string tipoDocumento, string numeroDocumento, string nombres,
                                            string apellidoPaterno, string apellidoMaterno, string genero, string fechaNacimiento,
                                            string email, string celular, string seguroPlanSalud, string password, string finalidadesAdiciona,
                                            string direccion, string numero, string departamento, string referencia, 
                                            string latlong, string origen, string nombreArchivo, string archivo,
                                            string telefono, string tipoPaciente, string observacion,
                                            string idPais, string idDepartamento, string idProvincia, string idDistrito,
                                            string idPaisNac, string idDepartamentoNac, string idProvinciaNac, string idDistritoNac,
                                            bool esIntranet, string codigoOTP, string tipoOTP, string finesAdicionales);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RegistrarMedicoFavorito/")]
        RespuestaSimpleBE RegistrarMedicoFavorito(string tipoDocumento, string numeroDocumento, string cmp, string idEspecialidad);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EliminarMedicoFavorito/")]
        RespuestaSimpleBE EliminarMedicoFavorito(string idMedicoFavorito);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AgregarFamiliar/")]
        RespuestaSimpleBE AgregarFamiliar(string tipoDocumento, string numeroDocumento, string tipoDocumentoTitular,
                                            string numeroDocumentoTitular, string nombres,
                                            string apellidoPaterno, string apellidoMaterno, string genero,
                                            string fechaNacimiento, string email, string celular,
                                            string seguroPlanSalud, string tipoParentesco, string direccion, string numeroDireccion, string numeroDepartamento,
                                            string referencia, string latlong, string nombreArchivo, string archivo,
                                            string telefono, string tipoPaciente, string observacion,
                                            string idPais, string idDepartamento, string idProvincia, string idDistrito,
                                            string idPaisNac, string idDepartamentoNac, string idProvinciaNac, string idDistritoNac,
                                            string finalidadesAdiciona, string codEstadoSolicitud, string adjunto, string nombreAdjunto,
                                            string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ModificarFamiliar/")]
        RespuestaSimpleBE ModificarFamiliar(string tipoDocumento, string numeroDocumento, string tipoDocumentoTitular,
                                    string numeroDocumentoTitular, string genero, string fechaNacimiento,
                                    string email, string celular, string seguroPlanSalud, string tipoParentesco, string direccion, string numeroDireccion, string numeroDepartamento,
                                    string referencia, string latlong, string nombres, string apellidoPaterno,
                                    string apellidoMaterno, string nombreArchivo, string archivo,
                                    string telefono, string tipoPaciente, string observacion,
                                    string idPais, string idDepartamento, string idProvincia, string idDistrito,
                                    string idPaisNac, string idDepartamentoNac, string idProvinciaNac, string idDistritoNac,
                                    string finalidadesAdiciona, string origen, bool esIntranet);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EliminarFamiliar/")]
        RespuestaSimpleBE EliminarFamiliar(string tipoDocumento, string numeroDocumento,
                                            string tipoDocumentoTitular, string numeroDocumentoTitular, bool esIntranet, string motivo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConsultarFamiliar/")]
        RespuestaBE<List<FamiliarBE>> ConsultarFamiliar(string tipoDocumentoTitular, string numeroDocumentoTitular);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/CargarImagen/")]
        RespuestaSimpleBE CargarImagen(string tipoDocumento, string numeroDocumento, string imagen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/PruebaEmail/")]
        RespuestaSimpleBE PruebaEmail();

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EncuestaPacienteCovid/")]
        RespuestaBE<PacienteCovid> EncuestaPacienteCovid(string tipoDocumento, string numeroDocumento);


        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/DatosUsuariosFamiliares/")]
        RespuestaBE<UsuarioFamiliaresBE> DatosUsuariosFamiliares(string tipoDocumento, string numeroDocumento);


        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarCorreoUsuario/")]
        RespuestaSimpleBE ActualizarCorreoUsuario(string tipoDocumento, string numeroDocumento, string email);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/EliminarCuenta/")]
        RespuestaSimpleBE EliminarCuenta(string tipoDocumento, string numeroDocumento, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/CifrarClave/")]
        RespuestaSimpleBE CifrarClave(string clave);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/DatosContactoMFA/")]
        UsuarioDatosContactoMFABE DatosContactoMFA(string codigoEmpresa, string codigoAplicativo, string usuario, 
            string dispositivo, string tokenAutenticacion);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AutenticarMFA/")]
        RespuestaBE<AutenticarMFABE> AutenticarMFA(string codigoAplicacion, string codigoEquipo, string so, bool esReenvio, string hashDispositivo,
            string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarCodigoMFA/")]
        RespuestaBE<ValidarCodigoMFABE> ValidarCodigoMFA(string codigoAplicacion, string tokenAtenticacion, string codigoMFA,
            string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarDeuda/")]
        RespuestaBE<VD_ListarDeudaResponsePresBE> ValidarDeuda(string tipoDocumento, string numeroDocumento, string tipoDocumentoPaciente,
            string numeroDocumentoPaciente, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarClave/")]
        RespuestaSimpleBE ValidarClave(string data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarDocumentoPersona/")]
        RespuestaBE<PersonaRegistroBE> ValidarDocumentoPersona(string tipoDocumento, string numeroDocumento, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarRegistroPersona/")]
        RespuestaSimpleBE ValidarRegistroPersona(string tipoDocumento, string numeroDocumento, string nombres,
                                            string apellidoPaterno, string apellidoMaterno, string genero, string fechaNacimiento,
                                            string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarPersonaSiteds/")]
        RespuestaBE<ValidarPersonaSitedsBE> ValidarPersonaSiteds(string tipoDocumentoTitular, string numeroDocumentoTitular, string tipoDocumento,
            string numeroDocumento, string nombres, string apellidoPaterno, string apellidoMaterno, string codigoTipoParentesco,
            string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ValidarDocumentoFamiliar/")]
        RespuestaBE<ValidaFamiliarBE> ValidarDocumentoFamiliar(string tipoDocumentoTitular, string numeroDocumentoTitular,
            string tipoDocumento, string numeroDocumento, string origen);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ConfirmarParentescoFamiliar/")]
        RespuestaSimpleBE ConfirmarParentescoFamiliar(string tipoDocumentoTitular, string numeroDocumentoTitular,
            string tipoDocumento, string numeroDocumento, string origen);

        #region Métodos deshabilitados
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RecordarPassword/")]
        RespuestaSimpleBE RecordarPassword(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/RecordarPasswordDatos/")]
        RespuestaBE<DataPassword> RecordarPasswordDatos(string tipoDocumento, string numeroDocumento);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/CambiarPassword/")]
        RespuestaSimpleBE CambiarPassword(string password);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarMedicosFavoritos/")]
        RespuestaBE<List<MedicoFavoritoBE>> ListarMedicosFavoritos(string tipoDocumento, string numeroDocumento);
        #endregion
    }
}
