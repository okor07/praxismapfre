using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GEN = MapfreHSBC.Models.General;
using Newtonsoft.Json.Linq;
using MapfreHSBC.Models.Cotizacion;

namespace MapfreHSBC.Controllers.API
{
    public class CotizacionController : ApiController
    {
        #region Métodos
        // Retorna datos de la solicitud de la cotizacion
        [Route(DatoCotizacion.API)]
        [HttpPost]
        public DatoCotizacion.Respuesta DatosCotizacion(DatoCotizacion.Datos datos)
        {
            return new Cotizacion().DatosCotizacion(datos);
        }

        // Confirma cotizacion del cliente
        [Route(ConfirmacionCotizacion.API)]
        [HttpGet, HttpPost]
        public ConfirmacionCotizacion.Respuesta ConfirmCotizacion(ConfirmacionCotizacion.Datos datos)
        {
            return new Cotizacion().ConfimacionCotizacion(datos);
        }
        // Retorna id de confirmacion
        [Route(ConfirmacionCotizacion.APICotizacion)]
        [HttpGet, HttpPost]
        public string GetIdCotizacion(JObject data)
        {
            //DatoCotizacion.Datos datos = data["Datos"].ToObject<DatoCotizacion.Datos>();
            //DatoCotizacion.Complemento comp = data["Complemento"].ToObject<DatoCotizacion.Complemento>();

            return new Cotizacion().GetIdCotizacion(data);
        }

        [Route("api/ConfirmaCotizacion/GetIdCotizacion2")]
        [HttpGet, HttpPost]
        public string GetIdCotizacion(ConfirmacionCotizacion.Datos datos)
        {
            return new Cotizacion().getIdCotizacion(datos);
        }

        // Retorna los precios para cotizacion
        [Route("api/ConfirmaCotizacion/getPrecios2")]
        [HttpGet, HttpPost]
        public string GetPrecios(ConfirmacionCotizacion.Datos datos)
        {
            return new Cotizacion().getPrecios(datos);
        }

        // Retorna los precios para cotizacion
        [Route(ConfirmacionCotizacion.APIPrecios)]
        [HttpGet, HttpPost]
        public string getPrecios(JObject data)
        {
            return new Cotizacion().GetPrecios(data);
        }

        //[Route(ConsultaCotizaciones.APICotizaciones)]
        //[HttpGet, HttpPost]
        //public string GetCotizaciones(JObject data)
        //{
        //    return new Cotizacion().GetCotizaciones(data);
        //}

        [Route(ConfirmacionCotizacion.APIPDF)]
        [HttpGet, HttpPost]
        public byte[] GetPDF(string datos)
        {
            return new Cotizacion().GetPDF(datos);
        }

        [Route(DatosContratante.API)]
        [HttpPost]
        public DatosContratante.Respuesta DatContratant(DatosContratante.Datos datos)
        {
            return new Cotizacion().DatosContratante(datos);
        }

        // Retorna datos de la solicitud de ProcesoCobro
        [Route("api/Cotizacion/WS5")]
        [HttpPost]
        public ProcesoCobro.Respuesta ProcesoCobro(ProcesoCobro.Datos datos)
        {
            return new Cotizacion().ProcesoCobro(datos);
            
        }

        [Route("api/Cotizacion/WS7")]
        [HttpPost]
        public CotizacionMapfre.Respuesta CotizacionMapfre(CotizacionMapfre.Datos datos)
        {
            return new Cotizacion().CotizacionMapfre(datos);
        }


        //[Route("api/Cotizacion/WS1Get")]///{id}")]
        ////[Route("api/Cotizacion/GetCotizacion/{id}")]
        //[HttpGet]
        //public DatoCotizacion.Datos GetCotizacion(int id)
        //{
        //    return new Cotizacion().GetCotizacion(id);
        //}
        #endregion
    }
}