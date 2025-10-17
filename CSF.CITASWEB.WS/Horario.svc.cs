using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using CSF.CITASWEB.WS.BE;
//using CSF.CITASWEB.WS.DA;

namespace CSF.CITASWEB.WS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Horario : IHorario
    {
        public RespuestaSimpleBE RegistrarHorario(string CMP, DateTime FechaDesde, DateTime FechaHasta, DateTime HoraDesde,
            DateTime HoraHasta, string Dias, int ConsultorioId, string Consultorio,
            int TiempoAtencion, int EspecialidadId, string EspecialidadNombre, string TipoEntidad, int TipoHorario,
            int CantidadAdicional, int TipoHorarioVirtual, bool IndicadorCompartido, int IDServicio, bool EsPrePago, string Origen,
            int IdClinica)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //HorarioDA oHorarioDA = new HorarioDA();
            //string rpta = oHorarioDA.RegistrarHorario(CMP, FechaDesde, FechaHasta, HoraDesde, HoraHasta, Dias, ConsultorioId,
            //    Consultorio, TiempoAtencion, EspecialidadId, EspecialidadNombre, TipoEntidad, TipoHorario,
            //    CantidadAdicional, TipoHorarioVirtual, IndicadorCompartido, IDServicio, EsPrePago, Origen,
            //    IdClinica);

            //RespuestaSimpleBE oRespuestaSimpleBE = new RespuestaSimpleBE();
            //oRespuestaSimpleBE.rpt = 0;
            //oRespuestaSimpleBE.mensaje = "";
            //oRespuestaSimpleBE.data = rpta;

            //return oRespuestaSimpleBE;
        }

        public RespuestaSimpleBE ActualizarHorario(int IDHorarioSpring, string CMP, DateTime FechaDesde, DateTime FechaHasta,
            DateTime HoraDesde, DateTime HoraHasta, string Dias, int ConsultorioId, string Consultorio, int TiempoAtencion,
            string EstadoRegistro, int EspecialidadId, string EspecialidadNombre, string TipoEntidad, int TipoHorario, 
            int CantidadAdicional, int TipoHorarioVirtual, bool IndicadorCompartido, int IDServicio, bool EsPrePago, string Origen,
            int IdClinica)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //HorarioDA oHorarioDA = new HorarioDA();
            //string rpta = oHorarioDA.ActualizarHorario(IDHorarioSpring, CMP, FechaDesde, FechaHasta, HoraDesde, HoraHasta, Dias,
            //    ConsultorioId, Consultorio, TiempoAtencion, EstadoRegistro, EspecialidadId, EspecialidadNombre, TipoEntidad, 
            //    TipoHorario, CantidadAdicional, TipoHorarioVirtual, IndicadorCompartido, IDServicio, EsPrePago, Origen,
            //    IdClinica);

            //RespuestaSimpleBE oRespuestaSimpleBE = new RespuestaSimpleBE();
            //oRespuestaSimpleBE.rpt = 0;
            //oRespuestaSimpleBE.mensaje = "";
            //oRespuestaSimpleBE.data = rpta;

            //return oRespuestaSimpleBE;

        }

        public RespuestaSimpleBE AnularHorario(int IDHorarioSpring, string CMP, DateTime FechaDesde, DateTime FechaHasta,
            DateTime HoraDesde, DateTime HoraHasta, string Dias, int ConsultorioId, string Consultorio, int TiempoAtencion,
            string EstadoRegistro, int EspecialidadId, string EspecialidadNombre, string TipoEntidad, int TipoHorario,
            int CantidadAdicional, int TipoHorarioVirtual, bool IndicadorCompartido, int IDServicio, bool EsPrePago, string Origen, 
            int IdClinica)
        {
            throw new WebFaultException(HttpStatusCode.BadRequest);
            //HorarioDA oHorarioDA = new HorarioDA();
            //string rpta = oHorarioDA.AnularHorario(IDHorarioSpring, CMP, FechaDesde, FechaHasta, HoraDesde, HoraHasta, Dias,
            //    ConsultorioId, Consultorio, TiempoAtencion, EstadoRegistro, EspecialidadId, EspecialidadNombre, TipoEntidad,
            //    TipoHorario, CantidadAdicional, TipoHorarioVirtual, IndicadorCompartido, IDServicio, EsPrePago, Origen, 
            //    IdClinica);

            //RespuestaSimpleBE oRespuestaSimpleBE = new RespuestaSimpleBE();
            //oRespuestaSimpleBE.rpt = 0;
            //oRespuestaSimpleBE.mensaje = "";
            //oRespuestaSimpleBE.data = rpta;

            //return oRespuestaSimpleBE;
        }


    }
}
