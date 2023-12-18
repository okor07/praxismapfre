using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class EstatusCotizacion
    {
        public class Datos : GEN.General.Key
        {
            public Datos()
            { }
        }

        public class Respuesta : GEN.General.Key
        {
            public Respuesta()
            { }

            [Required]
            public int? statusCotizacion
            { get; set; }

            [Required]
            public string statusMessaje
            { get; set; }
        }
    }
}