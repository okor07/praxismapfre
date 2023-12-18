using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class ProcesoCobro
    {
        #region Propiedades
        public const string API = "api/Cotizacion/WS5";
      //  public const string NameView = "Cotizacion";
        #endregion

        #region Contrusctor
        public ProcesoCobro()
        { }
        public ProcesoCobro(Datos datos)
        { }
        #endregion

        #region Clases

        public class MsgJson
        {
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
            public int? codigoError
            { get; set; }

            [Required]
            public string descripcionError
            { get; set; }

            public override string ToString()
            {
                return String.Format("idTransaccion: {0}, idPromotor:{1}, idCotizacionMapfre:{2}, "
                    + "codigoError: {3}, descripcionError: {4} ",
                    idTransaccion, idPromotor, idCotizacionMapfre, codigoError, descripcionError);
            }
        }

        public class Datos : GEN.General.AsSerializeable
        {
            #region Propiedades
            public MsgJson msgJson { get; set; }

            
            public int codResponse
            { get; set; }

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
            public int? codigoError
            { get; set; }

            [Required]
            public string descripcionError
            { get; set; }

            #endregion
            public override string ToString()
            {
                return String.Format("idTransaccion: {0}, idPromotor:{1}, idCotizacionMapfre:{2}, "
                    + "codigoError: {3}, descripcionError: {4}, msgJson: {5}, codResponse: {6} ",
                    idTransaccion, idPromotor, idCotizacionMapfre, codigoError, descripcionError, msgJson, codResponse);
            }
        }

        public class Respuesta : GEN.General.Respuesta
        {
            public Respuesta()
            { }
            public String msgJson { get; set; }
            public override string ToString()
            {
                return String.Format("msgJson: {0}", msgJson);
            }
        }

        #endregion
    }
}