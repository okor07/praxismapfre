using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MapfreHSBC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            string bandera = ConfigurationManager.AppSettings["Production"].ToString();

            if (bandera == "N")
            {
                routes.MapRoute(
                     name: "Default",
                     url: "{controller}/{action}/{id}",
                     defaults: new { controller = "DatosCotizacion", action = "Cotizacion", id = UrlParameter.Optional }

                 );
            }
            else
            {
                 routes.MapRoute(
                  name: "Default",
                  url: "{controller}/{action}/{id}", 
                  defaults: new { controller = "DatosCotizacion", action = "CotizacionVacia", id = UrlParameter.Optional }

              );  
            }
           


        }
    }
}
