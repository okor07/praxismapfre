using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class ConfirmacionCotizacion
    { 
        #region Propiedades
        public const string API = "api/ConfirmarCotizacion/WS2";
        public const string APIPrecios = "api/ConfirmaCotizacion/GetPrecios";
        public const string APIPDF = "api/ConfirmaCotizacion/GetPDF";
        public const string APICotizacion = "api/ConfirmaCotizacion/GetIdCotizacion";
        public const string PAGE = "DatosCotizacion/CompletarCotizacion"; 
        public const string PARAM = "DatosP";
        public const string NameView = "CompletarCotizacion";
        #endregion

        #region Contrusctor
        public ConfirmacionCotizacion()
        { }
        public ConfirmacionCotizacion(Datos datos)
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
            public Int64? idConfirmacion
            { get; set; }

            [Required]
            public string idCotizacionMapfre
            { get; set; }

            public string descripcionError
            { get; set; }

            [Required]
            public string email
            { get; set; }

            public int codigoError
            { get; set; }

            public float primaTotal
            { get; set; }

            public string idListaCotizacion
            { get; set; }
            
            #endregion
        }

        public class Respuesta : GEN.General.Respuesta
        {
            public Respuesta()
            { }
        }

        public class Distribucion : GEN.General.AsSerializeable
        {
            [GEN.AttrProperty( Header = "Tipo de inversión")]
            public string tipoInversion
            { get; set; }

            [GEN.AttrProperty(Header = GEN.General.ANIO)]
            public string porcentajeAnual
            { get; set; }

            [GEN.AttrProperty(Header = "Porcentaje de distribución")]
            public string porcentaje
            { get; set; }

            [GEN.AttrProperty(Header = "Distribución inicial")]
            public string distribucion
            { get; set; }
        }

        public class Coberturas : GEN.General.AsSerializeable
        {
            [GEN.AttrProperty(Header = "Nombre")]
            public string nombre
            { get; set; }

            [GEN.AttrProperty(Header = "Porcentaje")]
            public string porcentaje
            { get; set; }
        }

        public class Precios : GEN.General.AsSerializeable
        {
            public string prima
            { get; set; }

            public string monto
            { get; set; }
        }
        #endregion
    }
}