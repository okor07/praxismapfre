using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class CotizacionMapfre
    {

        #region Propiedades
        public const string API = "api/Cotizacion/WS7";
        //  public const string NameView = "Cotizacion";
        #endregion

        #region Contrusctor
        public CotizacionMapfre()
        { }
        public CotizacionMapfre(Datos datos)
        { }
        #endregion

        #region Clases

        public class MsgJson
        {
            [Required]
            public Int64? idTransaccion
            { get; set; }

            [Required]
            public string idCotizacionMapfre
            { get; set; }
        }


        public class Datos : GEN.General.AsSerializeable
        {
            public MsgJson msgJson { get; set; }
        }
        public class MsgJsonRespuesta
        {
            public Int64 idTransaccion
            { get; set; }

            public string idCotizacionMapfre
            { get; set; }

            public Int64 statusCotizacion
            { get; set; }

            public string statusMessaje
            { get; set; }
        }

        public class Respuesta : GEN.General.Respuesta
        {
            public MsgJsonRespuesta msgJson
            { get; set; }
        }

        #endregion


    }
}