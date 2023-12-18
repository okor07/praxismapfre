using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using MapfreHSBC.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MapfreHSBC.Controllers
{
    public class EditaCotizacionController : Controller
    {
        public const string SEXO = "VDSexo";
        public ActionResult EditaCotizacion()
        {
            ViewBag.Sexo = General.Sexo;

            ViewBag.Sexo = General.Sexo;
            ViewBag.Perfil = General.Perfil;
            ViewBag.Modalidad = General.Modalidad;
            ViewBag.Plazo = General.Plazo;
            ViewBag.Periodicidad = General.Periodicidad;


           // return View(  DatoCotizacion.NameView, new DatoCotizacion.Datos());

            return View(EditCotizacion.NameView, new EditCotizacion.Datos());

        }
	}
}