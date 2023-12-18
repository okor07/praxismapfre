using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapfreHSBC.Controllers
{
    public class CotizarController : Controller
    {
        const string IDLISTACOT = "idListaCotizacion";
        public long idCotizacionMapfre { get; set; }

        public CotizarController getCotizarController()
        {
            return this;
        }

        public void setIdCotizacionViewBag(long idCotizacion, String error)
        {
            ViewBag.idCotizacionMapfre = idCotizacion;
            ViewBag.errorMsg = null;
        }
        // GET: Cotizar
        public ActionResult Index()
        {

            ViewBag.textCorreo = "Apreciable cliente, \n\nAnexo encontrará la cotización referente al producto HSBC-MAPFRE Inversión Retiro. " +
                    " \n\nLe recordamos que para la contratación del seguro es necesario acudir a cualquier sucursal de HSBC. ";

            ViewBag.emailEnvia = ConfigurationSettings.AppSettings["cuentaCorreo"];
            ViewBag.showTables = "false";
            if (Request[ConfirmacionCotizacion.PARAM] != null)
            {
                DatoCotizacion.Datos datosParams =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<DatoCotizacion.Datos>
                    (System.Net.WebUtility.UrlDecode(Request[ConfirmacionCotizacion.PARAM]));
                if (datosParams.msgJson.sexo == 1)
                {
                    ViewBag.sexo = "M";
                }
                else
                {
                    ViewBag.sexo = "F";
                }
                ViewBag.idTransaccion = datosParams.msgJson.idTransaccion;
                ViewBag.idPromotor = datosParams.msgJson.idPromotor;
                ViewBag.fechaDeNacimiento = datosParams.msgJson.fechaDeNacimiento.ToString();
                ViewBag.fechaDeCotizacion = new DateTime().Date.ToString("dd/MM/yyyy");

                ViewBag.idListaCotizacion = Request[IDLISTACOT] != null ? Request[IDLISTACOT].ToString() : "0";

                ViewBag.correoElectronico = datosParams.msgJson.correoElectronico;
                
                if (idCotizacionMapfre <= 0)
                {
                    ViewBag.idCotizacionMapfre = 0;
                }
                else
                {
                    ViewBag.idCotizacionMapfre = idCotizacionMapfre;
                }

                ViewBag.primaInicial = 15000;
                ViewBag.primaInicialSt = "15,000.00";
                ViewBag.aportaciones = 0;
                ViewBag.aportacionesSt = "00.00";
                ViewBag.modalidad = 11201;
                ViewBag.pctDist1 = 0;
                ViewBag.pctDist2 = 0;
                ViewBag.pctDist3 = 0;
                //HHAC ini
                ViewBag.pctDist4 = 0;
                ViewBag.pctDist5 = 0;
                //HHAC fin
            }
            else
            {
                if (Request["numCotizacion"] != null)
                {
                    string idListaCotizacion = Request["idListaCotizacion"] != null ? Request["idListaCotizacion"] : "0";

                    string numCotizacion = Request["numCotizacion"];
                    string modalidad = Request["modalidad"] != null ? Request["modalidad"] : "11201";
                    AltaCotizacion altaCotizacion = new CotizarDao().getCotizacion(1, 112, Convert.ToInt32(modalidad), numCotizacion);
                    System.Diagnostics.Debug.WriteLine("altaCotizacion " + altaCotizacion);
                    altaCotizacion = decodificaAltacotizacion(altaCotizacion.msgJson);
                    System.Diagnostics.Debug.WriteLine("altaCotizacion 2 " + altaCotizacion);

                    ViewBag.idListaCotizacion = idListaCotizacion;//idListaCotizacion;
                    ViewBag.showTables = "true";
                    ViewBag.idCotizacionMapfre = numCotizacion;
                    ViewBag.pctDist1 = altaCotizacion.pctInversion1;
                    ViewBag.pctDist2 = altaCotizacion.pctInversion2;
                    ViewBag.pctDist3 = altaCotizacion.pctInversion3;
                    //HHAC ini
                    ViewBag.pctDist4 = altaCotizacion.pctInversion4;
                    ViewBag.pctDist5 = altaCotizacion.pctInversion5;
                    //HHAC fin
                    ViewBag.idTransaccion = altaCotizacion.idTransaccion;
                    ViewBag.idPromotor = altaCotizacion.idPromotor;
                    ViewBag.fechaDeNacimiento = altaCotizacion.fechaNacimiento;
                    ViewBag.fechaDeCotizacion = altaCotizacion.fechaCotizacion;
                    ViewBag.sexo = altaCotizacion.sexo;
                    ViewBag.correoElectronico = altaCotizacion.correoElectronico;
                    //Datos
                    ViewBag.modalidad = altaCotizacion.modalidad;
                    ViewBag.primaInicial = altaCotizacion.primaInicial;
                    ViewBag.primaInicialSt = altaCotizacion.primaInicial.ToString("C2");
                    ViewBag.perfil = altaCotizacion.perfil;
                    ViewBag.aportaciones = altaCotizacion.aportaciones;
                    ViewBag.aportacionesSt = altaCotizacion.aportaciones.ToString("C2");
                    ViewBag.moneda = altaCotizacion.moneda;
                    ViewBag.periodicidad = altaCotizacion.periodicidad;
                    ViewBag.plazo = altaCotizacion.plazo;
                    ViewBag.diaCobro = altaCotizacion.diaCobro;
                }
            }

            return View();
        }

        public AltaCotizacion decodificaAltacotizacion(string msgJson)
        {
            AltaCotizacion alta = null;

            if(!string.IsNullOrEmpty(msgJson))
                alta = Newtonsoft.Json.JsonConvert.DeserializeObject<AltaCotizacion>(System.Net.WebUtility.UrlDecode(msgJson));
            
            return alta;
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

                return RedirectToAction("Index");
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

                return RedirectToAction("Index");
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

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}