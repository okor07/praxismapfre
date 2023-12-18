using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MapfreHSBC.Models;

namespace MapfreHSBC.Controllers.API
{
    public class DetalleRestController : ApiController
    {
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
