using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using MapfreHSBC.Models.General;

namespace MapfreHSBC.Controllers.API
{
    public class ConsultaRestController : ApiController
    {

        [Route("api/rest/services/busqueda")]
        [HttpGet, HttpPost]
        public IEnumerable<ConsultasCotizaciones> getIdCotizaciones(int cod_cia, int cod_ramo, int cod_modalidad, string email)
        {
            try
            {
                if (String.IsNullOrEmpty(email))
                {
                    return null;//return new ConsultaDao().getConsultaCotizaciones(idCotizacion);
                }
                else
                {
                    return new ConsultaDao().getIdCotizaciones(cod_cia, cod_ramo, cod_modalidad, email);
                }

            }
            catch(Exception ex)
            {
                //List<string> datos = new List<string>();
                return null;
                //return datos.Add(ex.ToString());
            }
            
        }

        // GET: api/CotizarRest
        [Route("api/rest/services/paquetes")]
        [HttpGet, HttpPost]

        public IEnumerable<String> getPaquetes()
        {
            return new ConsultaDao().getPaquetes();
        }

        // GET: api/CotizarRest
        [Route("api/rest/services/cotizaciones")]
        [HttpGet]
        public IEnumerable<Models.Cotizacion.ConsultasCotizaciones> getConsultaCotizaciones(long idCotizacion, int cod_modalidad, string email)
        {
            return new ConsultaDao().getConsultaCotizaciones(idCotizacion, cod_modalidad, email);
        }


        // GET: api/CotizarRest/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CotizarRest
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CotizarRest/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CotizarRest/5
        public void Delete(int id)
        {
        }
    }
}
