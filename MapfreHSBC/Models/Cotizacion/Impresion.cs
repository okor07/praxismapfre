using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace MapfreHSBC.Models.Cotizacion
{
    public class Impresion
    {
        public string value { get; set; }
        public string text { get; set; }
        public bool isChecked { get; set; }
        public string noPoliza { get; set; }
        
        //Datos Correo
        public string remitente { get; set; }
        public string destinatario { get; set; }
        public string asunto { get; set; }
        public string mensaje { get; set; }
    }
    public class ImpresionList
    {
        public List<Impresion> documents { get; set; }
    }

    public class ImpresionPoliza
    {
        public string noPoliza { get; set; }    
    }
}