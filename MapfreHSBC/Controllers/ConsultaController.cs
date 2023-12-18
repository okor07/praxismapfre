using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapfreHSBC.Controllers
{
    public class ConsultaController : Controller
    {
        // GET: Cotizar
        public ActionResult BusquedaCotizaciones(string idListaCotizacion)
        {
            ViewBag.idListaCotizacion = !string.IsNullOrEmpty(idListaCotizacion) ? idListaCotizacion : "0";
            return View();
        }

        // GET: Cotizar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Cotizar/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cotizar/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("BusquedaCotizaciones");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cotizar/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Cotizar/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("BusquedaCotizaciones");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cotizar/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Cotizar/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("BusquedaCotizaciones");
            }
            catch
            {
                return View();
            }
        }
    }
}
