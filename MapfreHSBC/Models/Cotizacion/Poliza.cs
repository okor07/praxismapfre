using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class Poliza
    {
        #region Contrusctor
        public Poliza()
        { }
        public Poliza(Datos datos)
        { }
        #endregion

        #region Clases
        public class Datos : GEN.General.AsSerializeable
        {
            #region Propiedades
            [Required]
            public Int64? idTransaccion
            { get; set; }

            [Required]
            public string idPromotor
            { get; set; }

            [Required]
            public string idCotizacionMapfre
            { get; set; }

            [Required]
            public string numeroPoliza
            { get; set; }

            [Required]
            public string inicioVigencia
            { get; set; }

            [Required]
            public string finVigencia
            { get; set; }

            [Required]
            public double? primaInicial
            { get; set; }

            [Required]
            public double? primaAdicionales
            { get; set; }
	
            [Required]
            public string frecuenciaAportaciones
            { get; set; }

            [Required]
            public string plan
            { get; set; }
            #endregion
        }

        public class Respuesta : GEN.General.AsSerializeable
        {
            public Respuesta()
            { }
            public int codigoError
            { get; set; }

            public string descripcionError
            { get; set; }
        }
        #endregion
    }
}