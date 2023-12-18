using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class DatoCotizacion
    {
        #region Propiedades
        public const string API = "api/Cotizacion/WS1";
        public const string NameView = "Cotizacion";
        #endregion

        #region Contrusctor
        public DatoCotizacion()
        { }
        public DatoCotizacion(Datos datos)
        { }
        #endregion

        #region Clases
        public class Datos : GEN.General.AsSerializeable
        {
            #region Propiedades

            public MsgJson msgJson { get; set; }
            
            #endregion
        }

        public class MsgJson
        {
            public long idTransaccion
            { get; set; }

            public string idPromotor
            { get; set; }

            public string fechaDeNacimiento
            { get; set; }

            public int? sexo
            { get; set; }

            public string correoElectronico
            { get; set; }
        }

        public class MsgJsonRespuesta
        {
            public string url
            { get; set; }
        }

        public class Complemento : GEN.General.AsSerializeable
        { 
            public string perfil
            { get; set; }

            public string modalidad
            { get; set; }

            public string moneda
            { get; set; }

            public string plazo
            { get; set; }

            public string primaInicial
            { get; set; }

            public string aportacionesPeriodicas
            { get; set; }

            public string periodicidad
            { get; set; }

            public string formaDePago
            { get; set; }

        }

        public class Respuesta : GEN.General.Respuesta
        {
            public Respuesta()
            { }

            public MsgJsonRespuesta msgJson
            { get; set; }
        }

        public class RespuestaWs2 : GEN.General.Respuesta
        {
            public RespuestaWs2()
            { }

            public long? idTransaccion { get; set; }
            public string idPromotor { get; set; }
            public long idConfirmacion { get; set; }
            public string idCotizacionMapfre { get; set; }
        }
        #endregion
    }
}