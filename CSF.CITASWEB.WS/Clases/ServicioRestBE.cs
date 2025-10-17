using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSF.CITASWEB.WS
{
    //Respuestas Tekton (deberían eliminarse)
    public class respClientBE
    {
        public Dictionary<string, string> client { get; set; }
        public string error { get; set; }
    }

    public class respRegisterBE
    {
        public bool success { get; set; }
        public string id_nuevo { get; set; }
        public Dictionary<string, string> clinic { get; set; }
    }

    public class respBookBE
    {
        public string error { get; set; }
        public string id_cita_clinica { get; set; }
    }
    //Respuestas Spring
    public class respRegistrarPacienteBE
    {
        public string Persona { get; set; }
        public string error { get; set; }
    }
}