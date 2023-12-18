using MapfreHSBC.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapfreHSBC.Controllers
{
    public class ConsultaCotizacionesController : Controller
    {
        const string IDLISTACOT = "idListaCotizacion";
        //
        // GET: /ConsultaCotizaciones/
        public ActionResult Busqueda()
        {

            //Combo Productos
           // ViewBag.Producto = General.Producto;
            ViewBag.idListaCotizacion = Request[IDLISTACOT] != null ? Request[IDLISTACOT].ToString() : "0";

            return View();
        }
	}
}