using CSF.CITASWEB.WS.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CSF.CITASWEB.WS
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IHorario" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IHorario
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          UriTemplate = "/RegistrarHorario/")]
        RespuestaSimpleBE RegistrarHorario(string CMP, DateTime FechaDesde, DateTime FechaHasta, DateTime HoraDesde,
          DateTime HoraHasta, string Dias, int ConsultorioId, string Consultorio, int TiempoAtencion,
          int EspecialidadId, string EspecialidadNombre, string TipoEntidad, int TipoHorario, int CantidadAdicional,
          int TipoHorarioVirtual, Boolean IndicadorCompartido, int IDServicio, Boolean EsPrePago, string Origen,
          int IdClinica);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/ActualizarHorario/")]
        RespuestaSimpleBE ActualizarHorario(int IDHorarioSpring, string CMP, DateTime FechaDesde, DateTime FechaHasta,
        DateTime HoraDesde, DateTime HoraHasta, string Dias, int ConsultorioId, string Consultorio,
        int TiempoAtencion, string EstadoRegistro, int EspecialidadId, string EspecialidadNombre,
        string TipoEntidad, int TipoHorario, int CantidadAdicional, int TipoHorarioVirtual,
        Boolean IndicadorCompartido, int IDServicio, Boolean EsPrePago, string Origen,
        int IdClinica);

        [OperationContract]
        [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/AnularHorario/")]
        RespuestaSimpleBE AnularHorario(int IDHorarioSpring, string CMP, DateTime FechaDesde, DateTime FechaHasta,
        DateTime HoraDesde, DateTime HoraHasta, string Dias, int ConsultorioId, string Consultorio,
        int TiempoAtencion, string EstadoRegistro, int EspecialidadId, string EspecialidadNombre,
        string TipoEntidad, int TipoHorario, int CantidadAdicional, int TipoHorarioVirtual,
        Boolean IndicadorCompartido, int IDServicio, Boolean EsPrePago, string Origen, 
        int IdClinica);
    }
}
