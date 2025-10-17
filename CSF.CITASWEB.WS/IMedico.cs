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
    public interface IMedico
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/HorariosPorMedico/")]
        RespuestaBE<List<MedicoHorarioBE>> HorariosPorMedico(string idClinica, string idEspecialidad,
                                        string tipoDocumento, string numeroDocumento, string dia, string nombre);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/InformacionMedico/")]
        RespuestaBE<MedicoBE> InformacionMedico(string cmp);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/PedirMedicoDomicilio/")]
        RespuestaSimpleBE PedirMedicoDomicilio(string especialidad, string tipoDocumento, string numeroDocumento,
                                                string nombre, string celular, string direccion, string latitud,
                                                string longitud);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/HorariosPorNombreMedico/")]
        RespuestaBE<List<MedicoHorarioSimpleWebBE>> HorariosPorNombreMedico(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/FechasDisponiblesPorMedico/")]
        RespuestaBE<List<MedicoHorarioDisponibleBEV2>> FechasDisponiblesPorMedico(string idClinica, string idEspecialidad, string cmp);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/FechasDisponibles/")]
        RespuestaBE<List<DiasHorarioDisponibleBE>> FechasDisponibles(string idClinica, string idEspecialidad);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/HorariosPorMedicoVirtual/")]
        RespuestaBE<List<MedicoHorarioBE>> HorariosPorMedicoVirtual(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia, string idClinica = "17");

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/HorariosPorMedicoProximo/")]
        RespuestaBE<HorarioCitaMasProxima> HorariosPorMedicoProximo(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia, string idClinica, string cmp, string tipo);
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/HorariosPorMedicoProximoPresencial/")]
        RespuestaBE<HorarioCitaMasProxima> HorariosPorMedicoProximoPresencial(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia, string idClinica, string cmp);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/BuscarMedicoWeb/")]
        RespuestaBE<List<MedicoHorarioSimpleWebBE>> BuscarMedicoWeb(string idClinica, string idEspecialidad, string nombre);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/StaffMedicos/")]
        RespuestaBE<List<MedicoHorarioSimpleWebBE>> StaffMedicos(string idClinica, string idEspecialidad, string nombre);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/TurnosPorRangoHorario/")]
        RespuestaSimpleBE TurnosPorRangoHorario(string fechaInicio, string fechaFin);
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/TurnosPorRangoHorarioV2/")]
        RespuestaSimpleBE TurnosPorRangoHorarioV2(string fechaInicio, string fechaFin);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarCitasMedico/")]
        RespuestaBE<List<CitaHistoricaVistaPreviaBE>> ListarCitasMedico(string cmp, string idClinica, string fecha);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ListarMedicoHorarioPorEspecialidadEC/")]
        RespuestaBE<List<MedicoHorarioBE>> ListarMedicoHorarioPorEspecialidadEC(string idEspecialidad, string tipoDocumento, string numeroDocumento, string dia);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/FechasProximasPorMedico/")]
        //RespuestaBE<List<CMPMedicoHorarioBE>> FechasProximasPorMedico(string idClinica, string idEspecialidad, string cmp, string ejecucion, string registrosInicial, string registrosFinal, string tipoCita);
        RespuestaBE<List<MedicoHorarioProximaFechaBE>> FechasProximasPorMedico(string idClinica, string idEspecialidad, string cmp, string ejecucion, string registrosInicial, string registrosFinal, string tipoCita);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/HorariosPorNombreMedicoVirtual/")]
        RespuestaBE<List<MedicoHorarioSimpleWebBE>> HorariosPorNombreMedicoVirtual(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ObtenerInformacionMedico/")]
        RespuestaBE<InfoMedicoBE> ObtenerInformacionMedico(string cmp, string idEspecialidad);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/FechasDisponiblesVirtual/")]
        RespuestaBE<List<DiasHorarioDisponibleBE>> FechasDisponiblesVirtual(string idClinica, string idEspecialidad);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/FechasDisponiblesPorMedicoVirtual/")]
        RespuestaBE<List<MedicoHorarioDisponibleBEV2>> FechasDisponiblesPorMedicoVirtual(string idClinica, string idEspecialidad, string cmp);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/DirectorioMedico/")]
        RespuestaBE<List<DirectorioMedicoBE>> DirectorioMedico(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos);
        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/DirectorioMedicoVirtual/")]
        RespuestaBE<List<DirectorioMedicoBE>> DirectorioMedicoVirtual(string idClinica, string idEspecialidad, string nombre,
                                        string tipoDocumento, string numeroDocumento, bool soloMedicosFavoritos);
    }
}
