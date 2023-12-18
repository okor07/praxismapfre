using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapfreHSBC.Models.Cotizacion
{
    public class DistribucionFondos
    {

        public long idCotizacion { get; set; }
        public double primaInicial { get; set; }
        public int idInversion { get; set; }
        public string tipoInversion { get; set; }
        public double pctAnio { get; set; }
        public double pctDistribucion { get; set; }
        public double distInicial { get; set; }
        public string anio { get; set; }

    }
}