using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class IntentoCobro
    {
        #region Contrusctor
        public IntentoCobro()
        { }
        public IntentoCobro(Datos datos)
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
            public Int64? numeroPoliza
            { get; set; }
	
            [Required]
            public string numeroReferencia
            { get; set; }

            [Required]
            public double? monto
            { get; set; }

            [Required]
            public string descCargo
            { get; set; }
            #endregion
        }
        #endregion
    }
}