using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.General
{
    public class InformacionCP
    {
        public InformacionCP()
        { }

        public class Datos : GEN.General.AsSerializeable
        {
            public MsgJson msgJson { get; set; }
        }
        public class MsgJson
        {
            [Required]
            public Int64 idTransaccion
            { get; set; }
            [Required]
            public string cp
            { get; set; }
        }

        public class MsgJsonRespuesta
        {
            public Int64 idTransaccion
            { get; set; }
            public string cp
            { get; set; }
            public int idPoblacion
            { get; set; }
            public string descPoblacion
            { get; set; }
            public int idMunicipio
            { get; set; }
            public string descMunicipio
            { get; set; }
            public int idEstado
            { get; set; }
            public string descEstado
            { get; set; }
            public string idPais
            { get; set; }
            public string descPais
            { get; set; }
        }

        public class Respuesta : GEN.General.Respuesta
        {
            public Respuesta()
            { }
            public MsgJsonRespuesta msgJson
            { get; set; }
        }
    }
}