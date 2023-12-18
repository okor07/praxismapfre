using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapfreHSBC.Models.Cotizacion
{
    public interface ICotizacion
    {
        DatoCotizacion.Respuesta DatosCotizacion(DatoCotizacion.Datos datos);

        ConfirmacionCotizacion.Respuesta ConfimacionCotizacion(ConfirmacionCotizacion.Datos datos);

        DatosContratante.Respuesta DatosContratante(DatosContratante.Datos datos);

        string GetIdCotizacion(JObject datos);
        string GetPrecios(JObject datos);

        byte[] GetPDF(string datos);

        DatoCotizacion.Datos GetCotizacion(string id);
    }
}
