using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapfreHSBC.Models.Cotizacion
{
    public class DetalleCotizacion
    {
        public string perfil { get; set; }
        public string modalidad { get; set; }
        public string moneda { get; set; }
        public string plazo { get; set; }
        public string coberturas { get; set; }
        public string primaInicial { get; set; }
        public double aportacionesPeriodicas { get; set; }
        public string periodicidad { get; set; }
        public string formaPago { get; set; }
        public string diaCobro { get; set; }
        public string fondoUno { get; set; }
        public string fondoDos { get; set; }
        public string fondoTres { get; set; }
        //HHAC INI
        public string fondoCuatro { get; set; }
        public string fondoCinco { get; set; }
        //HHAC FIN
        public string precioSeguro { get; set; }
        public double derechoPoliza { get; set; }
        public string primaTotal { get; set; }
    }
}