using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class ConsultaCotizacion : GEN.General.AsSerializeable
    {
        public class Resultados : GEN.General.AsSerializeable
        {
            private DateTime? fechacotizacion;
            private DateTime? fechanacimiento;
            private string idcotizacion;

            #region Propiedades
            [GEN.AttrProperty(Header = "ID Cotización")]
            public object idCotizacion
            {
                get { return new HtmlString(string.Format("<a href='javascript:Reload({0})'>{0}</a>", idcotizacion)); }
                set { idcotizacion = value.ToString(); } 
            }

            [GEN.AttrProperty(Header = "Fecha de cotización")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public object fechaCotizacion
            { 
                get { return fechacotizacion.HasValue ? fechacotizacion.Value.ToShortDateString() : ""; }
                set { fechacotizacion = (DateTime)value; }
            }

            [GEN.AttrProperty(Header = "Fecha de nacimiento")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public object fechaNacimiento
            {
                get { return fechanacimiento.HasValue ? fechanacimiento.Value.ToShortDateString() : ""; }
                set { fechanacimiento = (DateTime)value; }
            }

            [GEN.AttrProperty(Header = "Monto de la aportación adicional")]
            public string montoAportacion
            { get; set; }

            [GEN.AttrProperty(Header = "Periodicidad de la aprotación adicional")]
            public string periodicidad
            { get; set; }
            #endregion
        }

        public class Datos
        {
            #region Constantes
            public const string NameView = "ConsultasCotizaciones";
            #endregion

            #region Propiedades
            public string producto
            { get; set; }

            public string noCotizacion
            { get; set; }

            public string correoElectroinico
            { get; set; }
            #endregion
        }
    }
}