using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;


namespace MapfreHSBC.Models.Cotizacion
{
    public class EditCotizacion
    {
        #region Propiedades
        public const string API = "api/Cotizacion/WS1";
        public const string NameView = "EditaCotizacion";


        #endregion

        #region Contrusctor
        public EditCotizacion()
        { }
        public EditCotizacion(Datos datos)
        { }
        #endregion

        #region Clases
        public class Datos : GEN.General.AsSerializeable
        {
            #region Propiedades
            [Required]
            public Int64? noCotizacion
            { get; set; }

            [Required]
            [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
            public DateTime? fechaDeNacimiento
            { get; set; }

            [Required]
            public int sexo
            { get; set; }

            public string correoElectronico
            { get; set; }

            [Required]
            public Int64? idTransaccion
            { get; set; }

            [Required]
            public string idPromotor
            { get; set; }

            #endregion
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

        #endregion
    }

}