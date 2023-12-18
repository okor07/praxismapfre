﻿using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapfreHSBC.Controllers
{
    public class DetalleController : Controller
    {
        string idError;
        // GET: Cotizar/Details/5
        public ActionResult Detalle(string id)
        {
            try
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En Detalle: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Poliza: " + id, null);
                
                    //Datos Contratatnte
                DatosContratante.Datos detalle2 = new DetalleDao().getJsonBenef(id);
                ViewBag.apMaterno = detalle2.msgJson.apellidoMaternoContrante != null ? detalle2.msgJson.apellidoMaternoContrante : "" ;// dc.msgJson.idTransaccion;


                if (detalle2.msgJson.tipoDePersona.Equals("F"))
                {
                    detalle2.msgJson.tipoDePersona = "Física";
                }
                else
                {
                    detalle2.msgJson.tipoDePersona = "Moral";
                }

                //Datos del producto
                DataTable dt = new DetalleDao().getJsonProd(id);

                viweBagDatos(detalle2, dt, id);

                return View(detalle2);
            }
            catch(Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Detalle: ", ref idError, _error);
                throw;
            }
        }

        public void viweBagDatos(DatosContratante.Datos dc, DataTable dt, String numPoliza)
        {

            string[] json ;

            double pctFondoUno;
            double pctFondoDos;
            double pctFondoTres;
            //HHAC ini
            double pctFondoCuatro;
            double pctFondoCinco;
            //HHAC fin
            int cod_modalidad = 0;
            string parametros = "";
            CotizarDao objCot = new CotizarDao();

            CotizarDao obj = new CotizarDao();

            DataTable dtFondos = obj.getDistribucionFondos(numPoliza);

            string rutaImpresion = ConfigurationSettings.AppSettings["RutaPantallaImpresion"];
            ViewBag.rutaImp = rutaImpresion;

            DetalleCotizacion detalle = new DetalleCotizacion();

            ViewBag.idTransaccion = dc.msgJson.idTransaccion;
            ViewBag.idPromotor = dc.msgJson.idPromotor;
            ViewBag.idCotizacionMapfre = dc.msgJson.idCotizacionMapfre;
            ViewBag.numeroPoliza = numPoliza;
            ViewBag.numeroReferencia = numPoliza;
            ViewBag.monto = 1000.02;
            ViewBag.descCargo = "Seg Unit Link "+ numPoliza;




            foreach (DataRow row in dt.Rows)
            {
                json = row[0].ToString().Replace('"', ' ').Split(',');

                pctFondoUno = Convert.ToInt32(json[17].Split(':')[1]);
                pctFondoDos = Convert.ToInt32(json[18].Split(':')[1]);
                pctFondoTres = Convert.ToInt32(json[19].Split(':')[1]);
                //HHAC ini
                pctFondoCuatro = Convert.ToInt32(json[20].Split(':')[1]);
                pctFondoCinco = Convert.ToInt32(json[21].Split(':')[1]);
                //HHAC fin
                cod_modalidad = Convert.ToInt32(json[9].Split(':')[1]);

                parametros = "[cod_cia=1][cod_ramo=112][DVCOD_MODALIDAD=" + cod_modalidad + "]";

                DataTable dtNomFondos = objCot.getLV("A2309022_MPT", 1, parametros);

                string nomFondo001 = "";
                string nomFondo002 = "";
                string nomFondo003 = "";
                //HHAC ini
                string nomFondo004 = "";
                string nomFondo005 = "";

                foreach (DataRow rowF in dtNomFondos.Rows)
                {
                    if (rowF[0].ToString().Equals("0001"))
                        nomFondo001 = rowF[1].ToString();
                    else if (rowF[0].ToString().Equals("0002"))
                        nomFondo002 = rowF[1].ToString();
                    else if (rowF[0].ToString().Equals("0003"))
                        nomFondo003 = rowF[1].ToString();
                    else if (rowF[0].ToString().Equals("0004"))
                        nomFondo004 = rowF[1].ToString();
                    else if (rowF[0].ToString().Equals("0005"))
                        nomFondo005 = rowF[1].ToString();
                }
                
                detalle.perfil = json[22].Split(':')[1];//20
                detalle.modalidad = json[23].Split(':')[1];//21
                detalle.moneda = json[24].Split(':')[1].Replace("}","");//22
                detalle.plazo = json[14].Split(':')[1] + " AÑOS";
                detalle.coberturas = "Fallecimiento 1% y Muerte Accidental 20%";
                detalle.primaInicial = json[8].Split(':')[1];
                detalle.aportacionesPeriodicas = Convert.ToDouble(json[10].Split(':')[1]);
                detalle.periodicidad = json[13].Split(':')[1];
                detalle.formaPago = "CONTADO";
                
                foreach (DataRow row1 in dtFondos.Rows)
                {
                    if (row1[0].ToString().Equals("0001"))
                    {
                        detalle.fondoUno = json[17].Split(':')[1] + " % - " + Convert.ToDouble(row1[2].ToString()).ToString("C");
                    }
                    else if (row1[0].ToString().Equals("0002"))
                    {
                        detalle.fondoDos = json[18].Split(':')[1] + " % - " + Convert.ToDouble(row1[2].ToString()).ToString("C");

                    }
                    else if (row1[0].ToString().Equals("0003"))
                    {
                        detalle.fondoTres = json[19].Split(':')[1] + " % - " + Convert.ToDouble(row1[2].ToString()).ToString("C");

                    }
                    else if(row1[0].ToString().Equals("0004"))
                    {
                        detalle.fondoCuatro = json[20].Split(':')[1] + " % -" + Convert.ToDouble(row1[2].ToString()).ToString("C");
                    }
                    else if(row1[0].ToString().Equals("0005"))
                    {
                        detalle.fondoCinco = json[21].Split(':')[1] + " % -" + Convert.ToDouble(row1[2].ToString()).ToString("C");
                    }
                }
                //HHAC fin
                //detalle.fondoUno = json[17].Split(':')[1] + " % - " + Convert.ToInt32(Convert.ToDouble(detalle.primaInicial) * (pctFondoUno / 100)).ToString("C");
                //detalle.fondoDos = json[18].Split(':')[1] + " % - " + Convert.ToInt32(Convert.ToDouble(detalle.primaInicial) * (pctFondoDos / 100)).ToString("C");
                //detalle.fondoTres = json[19].Split(':')[1] + " % - "+ Convert.ToInt32(Convert.ToDouble(detalle.primaInicial) * (pctFondoTres / 100)).ToString("C");
                
                detalle.precioSeguro = json[8].Split(':')[1];
                detalle.derechoPoliza = 600 ;
                detalle.primaTotal = (Convert.ToDouble(json[8].Split(':')[1])).ToString();

                //Caracteristicas del producto
                ViewBag.perfil = detalle.perfil;
                ViewBag.modalidad = detalle.modalidad;
                ViewBag.moneda = detalle.moneda;
                ViewBag.plazo = detalle.plazo;
                ViewBag.coberturas = detalle.coberturas.ToUpper();
                ViewBag.primaInicial =  Convert.ToDouble(detalle.primaInicial).ToString("C");
                ViewBag.aportacionesPeriodicas = detalle.aportacionesPeriodicas.ToString("C");
                ViewBag.periodicidad = detalle.periodicidad;
                ViewBag.formaPago = detalle.formaPago;
                ViewBag.fondoUno = detalle.fondoUno;
                ViewBag.fondoDos = detalle.fondoDos;
                ViewBag.fondoTres = detalle.fondoTres;
                // HHAC ini
                ViewBag.fondoCuatro = detalle.fondoCuatro;
                ViewBag.fondoCinco = detalle.fondoCinco;
                // HHAC fin
                ViewBag.precioSeguro = Convert.ToDouble(detalle.precioSeguro).ToString("C");
                ViewBag.derechoPoliza = detalle.derechoPoliza.ToString("C");
                ViewBag.primaTotal = Convert.ToDouble(detalle.primaTotal).ToString("C");
                ViewBag.monto = detalle.primaTotal;

                ViewBag.nomFondo1 = nomFondo001;
                ViewBag.nomFondo2 = nomFondo002;
                ViewBag.nomFondo3 = nomFondo003;
                //HHAC ini
                ViewBag.nomFondo4 = nomFondo004;
                ViewBag.nomFondo5 = nomFondo005;
                //HHAC fin
            }

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

                return RedirectToAction("Detalle");
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

                return RedirectToAction("Detalle");
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

                return RedirectToAction("Detalle");
            }
            catch
            {
                return View();
            }
        }
    }
}
