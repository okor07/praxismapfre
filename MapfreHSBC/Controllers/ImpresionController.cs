using iTextSharp.text;
using iTextSharp.text.pdf;
using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MapfreHSBC.Controllers
{
    public class ImpresionController : Controller
    {
        string idError;
        // GET: Cotizar
        public ActionResult Impresiones()
        {
            ImpresionDao obj = new ImpresionDao();
            string noPoliza = Request["idPoliza"];
            string email = obj.getEmail(noPoliza);

            ViewBag.textCorreo = "Apreciable cliente, \n\nAnexa se encuentrá la documentación del producto INVERSION RETIRO recien contratado.";

            ViewBag.emailEnvia = ConfigurationSettings.AppSettings["cuentaCorreo"];
            ViewBag.Poliza = noPoliza;
            ViewBag.emailRecibe = email;

            string modalidad = obj.getModalidad(noPoliza);
            string nacionalidad = "MEX";// obj.getJsonBenef(noPoliza);

            ViewBag.DocumentsList = obj.getDocuments(modalidad, nacionalidad);

            return View();
        }

        // GET: Cotizar/Details/5
        public ActionResult Impresion(string noPoliza, string idDocument)
        {
            //return View();
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("----------------- ", null);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Impresion: ", null);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En Impresion: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("noPoliza: " + noPoliza, null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("idDocument: " + idDocument, null);
            string codCia = "1";
            byte[] yourByteArray = null;
            try
            {
                if (idDocument != "FI")
                {
                    yourByteArray = new ImpresionDao().CreaPDFHPExstream(codCia, noPoliza, idDocument);
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Obtuvo los bytes", null);
                }
                else
                    yourByteArray = new Cotizacion().GetFolleto_Informativo();
            }
            catch (Exception ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Impresion Excepcion: ", ex);
                yourByteArray = new Cotizacion().GetPDFPolizaHard();
            }
            if (yourByteArray == null)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("No obtuvo los bytes", null);
                yourByteArray = new Cotizacion().GetPDFPolizaHard();
            }

            var response = new System.Net.Http.HttpResponseMessage();
            response.Content = new System.Net.Http.ByteArrayContent(yourByteArray);

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "application/octet-stream");
            Response.Buffer = true;
            Response.Clear();
            Response.BinaryWrite(yourByteArray);
            Response.End();

            return new FileStreamResult(Response.OutputStream, Response.ContentType);

        }

        public ActionResult ImpresionAll(string noPoliza, string idDocument, string allDoctos)
        {
            idDocument = idDocument.Substring(0, idDocument.Length - 1);
            List<byte[]> pdfs = new List<byte[]>();

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("----------------- ", null);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Impresion: ", null);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En Impresion: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("noPoliza: " + noPoliza, null);

            foreach (string idDoc in idDocument.Split('|'))
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("idDocument: " + idDoc, null);
                string codCia = "1";
                byte[] yourByteArray = null;

                try
                {
                    if (idDoc == "P" || idDoc == "ST" || idDoc == "CRS" || idDoc == "CF" || idDoc == "PPR" || idDoc == "CP")
                    {
                        yourByteArray = new ImpresionDao().CreaPDFHPExstream(codCia, noPoliza, idDoc);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Obtuvo los bytes", null);
                        pdfs.Add(yourByteArray);
                    }
                    //HHAC ini
                    if (idDoc == "FI")
                    {
                        yourByteArray = new Cotizacion().GetFolleto_Informativo();
                        pdfs.Add(yourByteArray);
                    }
                    //HHAC fin
                    else
                    {
                        string dataFilePath = "~/Documents/" + idDoc + ".pdf";
                        string path = System.Web.HttpContext.Current.Server.MapPath(dataFilePath);
                        yourByteArray = System.IO.File.ReadAllBytes(path);

                        pdfs.Add(yourByteArray);
                    }
                }
                catch (Exception ex)
                {
                    
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Impresion Excepcion: ", ref idError, ex);
                    yourByteArray = new Cotizacion().GetPDFPolizaHard();
                }

                if (yourByteArray == null)
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("No obtuvo los bytes", null);
                    yourByteArray = new Cotizacion().GetPDFPolizaHard();
                }
            }

            byte[] concatPDF = mergePdfs(pdfs);

            var response = new System.Net.Http.HttpResponseMessage();
            response.Content = new System.Net.Http.ByteArrayContent(concatPDF);

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "application/octet-stream");
            Response.Buffer = true;
            Response.Clear();
            Response.BinaryWrite(concatPDF);
            Response.End();

            return new FileStreamResult(Response.OutputStream, Response.ContentType);

        }

        protected static byte[] mergePdfs(List<byte[]> pdfs)
        {
            MemoryStream concatPDF = new MemoryStream();
            using (Document document = new Document())
            using (PdfCopy copy = new PdfCopy(document, concatPDF))
            {
                document.Open();

                foreach (byte[] pdf in pdfs)
                    copy.AddDocument(new PdfReader(pdf));
            }

            return concatPDF.ToArray();
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

                return RedirectToAction("Impresiones");
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

                return RedirectToAction("Impresiones");
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

                return RedirectToAction("Impresiones");
            }
            catch
            {
                return View();
            }
        }


    }
}
