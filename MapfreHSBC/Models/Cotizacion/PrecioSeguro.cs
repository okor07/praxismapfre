using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapfreHSBC.Models.Cotizacion
{
    public class PrecioSeguro
    {
        public long prima { get; set; }
        public long derechoPoliza { get; set; }
        public long primaTotal { get; set; }
        public string clave { get; set; }
        public string valor { get; set; }
    }
}