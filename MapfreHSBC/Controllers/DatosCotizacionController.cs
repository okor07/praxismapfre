using MapfreHSBC.Controllers.API;
using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using MapfreHSBC.Models.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Xml;


namespace MapfreHSBC.Controllers
{
    public class DatosCotizacionController : Controller
    {
        public const string SEXO = "VDSexo";
        string idError;
        public ActionResult Cotizacion()
        {
            ViewBag.Sexo = General.Sexo;

            return View(DatoCotizacion.NameView, new DatoCotizacion.Datos());
        }

        public ActionResult CompletarCotizacion(string idCotizacion)
        {
            //params
            if (Request[ConfirmacionCotizacion.PARAM] != null)
            {
                DatoCotizacion.Datos datosParams = Newtonsoft.Json.JsonConvert.DeserializeObject<DatoCotizacion.Datos>(System.Net.WebUtility.UrlDecode(Request[ConfirmacionCotizacion.PARAM]));
                ViewBag.sexo = datosParams.msgJson.sexo;
                ViewBag.idTransaccion = datosParams.msgJson.idTransaccion;
                ViewBag.idPromotor = datosParams.msgJson.idPromotor;
                ViewBag.fechaDeNacimiento = datosParams.msgJson.fechaDeNacimiento.ToString();
                ViewBag.correoElectronico = datosParams.msgJson.correoElectronico;
            }
            //
            ViewBag.Perfil = General.Perfil;
            ViewBag.Modalidad = General.Modalidad;
            ViewBag.Plazo = General.Plazo;
            ViewBag.Periodicidad = General.Periodicidad;
            ViewBag.Cobro = General.Cobro;

            //TestData datosPrueba = new TestData();
            //Fondos
            //List<DistribucionFondos> fondos = datosPrueba.getDistribucionFondos();

            // Distribucion
            List<object> result = new List<object>();
            result = new TestData().GetDistribucion(1);

            DataGrid dataGid = new DataGrid(result);

            ViewBag.DSDistribucionFI = result;
            ViewBag.GVDistribucionFI = dataGid.DataHeaderTable;
            
            // Coberturas
            result = new List<object>();
            result = new TestData().GetCoberturas(1);

            dataGid = new DataGrid(result);

            ViewBag.DSCoberturas = result;
            ViewBag.GVCoberturas = dataGid.DataHeaderTable;

            result = new List<object>();
            result.Add(new ConfirmacionCotizacion.Precios() { prima="x", monto="x" });

            List<DataGrid.ColumnCustom> columns = new List<DataGrid.ColumnCustom>();
            columns.Add(new DataGrid.ColumnCustom() { Header = "Prima", DataField = "prima" });
            columns.Add(new DataGrid.ColumnCustom() { Header = ((ConfirmacionCotizacion.Precios)result.Last()).monto, DataField = "monto" });

            dataGid = new DataGrid(columns);
            //ViewBag.DSPrecios = result;
            ViewBag.GVPrecios = dataGid.DataHeaderTable;
            //ViewBag.IsResponse = "OK";

            if (Request[ConfirmacionCotizacion.PARAM] != null && string.IsNullOrEmpty(idCotizacion))
                return View(ConfirmacionCotizacion.NameView, Newtonsoft.Json.JsonConvert.DeserializeObject<DatoCotizacion.Datos>(System.Net.WebUtility.UrlDecode(Request[ConfirmacionCotizacion.PARAM])));
            else
            {
                DatoCotizacion.Datos datos = new DatoCotizacion.Datos();

                if (!string.IsNullOrEmpty(idCotizacion))
                {
                    DatoCotizacion.Complemento comp = new DatoCotizacion.Complemento() { primaInicial = "10,000", plazo = "10 años", periodicidad = "trimestral", modalidad = "Unit Linked PPR", formaDePago = "Contado", perfil = "Moderado", moneda = "Nacional", aportacionesPeriodicas = "1,000" };

                    new TestData().TestGetCotizacion(Int32.Parse(idCotizacion), ref datos);

                    ViewBag.DatosC = datos;//new DatoCotizacion.Datos() { idPromotor = "P102342", correoElectronico = "aa@asdf.com", fechaDeNacimiento = DateTime.Now, idTransaccion = 44458, sexo = 'M' };
                    ViewBag.CompC = comp;
                    ViewBag.DSPrecios = new TestData().GetPreciosHTML(datos, comp);
                    ViewBag.IsConsult = true;
                    ViewBag.IdCotizacion = idCotizacion;
                }

                return View(ConfirmacionCotizacion.NameView, datos);
            }
        }

        public ActionResult CompletarCotizacionResp()
        {
            

            return PartialView(ConfirmacionCotizacion.NameView,Newtonsoft.Json.JsonConvert.DeserializeObject<DatoCotizacion.Datos>(System.Net.WebUtility.UrlDecode(Request[ConfirmacionCotizacion.PARAM])));
        }

        public ActionResult DownloadFile(string modalidad,string numCotizacion, string fecNacimiento, 
            string sexo, string correo, string moneda, string plazo, string primaIni, string primaAdd, 
            string frecuenciaPrima, string perfil,  string pctInversion1, string pctInversion2, string pctInversion3)
        {
            int descarga = 1;
            byte[] yourByteArray = null;
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En DownloadFile numCotizacion : " + numCotizacion, null);
            try
            {
                yourByteArray = new CotizarDao().obtienePdfCotizacion(modalidad ,numCotizacion, fecNacimiento, sexo, correo, moneda, plazo,
                    primaIni, primaAdd, frecuenciaPrima, perfil, pctInversion1, pctInversion2, pctInversion3);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Obtuvo los bytes", null);
            }
            catch (Exception ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("DownloadFile Excepcion: ", ex);
                yourByteArray = new Cotizacion().GetPDF("");
            }
            if (yourByteArray == null)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("No obtuvo los bytes", null);
                yourByteArray = new Cotizacion().GetPDF("");
            }
            var response = new HttpResponseMessage();
            response.Content = new ByteArrayContent(yourByteArray);

            //si se requiere descargar el archivo
            if (descarga == 1)
            {
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = numCotizacion + ".pdf";
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "application/octet-stream");
            Response.Buffer = true;
            Response.Clear();
            Response.BinaryWrite(yourByteArray);
            Response.End();

            return new FileStreamResult(Response.OutputStream, Response.ContentType);
            
        }

        public ActionResult VerificacionDatos()
        {
            ViewBag.Sexo = General.Sexo;
            ViewBag.DatosC = new DatoCotizacion.Datos() { msgJson = new DatoCotizacion.MsgJson() { idPromotor = "P102342", correoElectronico = "aa@asdf.com", fechaDeNacimiento = DateTime.Now.ToString("dd/MM/yyyy"), idTransaccion = 44458, sexo = 'M' } };
            ViewBag.CompC = new DatoCotizacion.Complemento() { primaInicial = "10,000", plazo = "10 años", periodicidad = "trimestral", modalidad = "Unit Linked PPR", formaDePago = "Contado", perfil = "Moderado", moneda = "Nacional", aportacionesPeriodicas = "1,000" };

            DatosContratante.Datos cont = new DatosContratante.Datos();
            cont = new TestData().GetContratante();

            return View(DatosContratante.NameView, cont);
        }

        public async Task<ActionResult> SendMail(General.Mail mail)
        {
            byte[] arr = new Cotizacion().GetPDF(null);
            bool isOK = false;
            string msj = "";

            var message = new MailMessage();
            message.To.Add(new MailAddress(mail.envia));
            message.From = new MailAddress(mail.recibe);
            message.Subject = mail.titulo;
            message.Body = mail.texto;
            message.Attachments.Add(new Attachment(new MemoryStream(arr), string.Format("Cotizacion_{0}.pdf", DateTime.Now.Date.ToShortDateString())));//Response.OutputStream, Response.ContentType));
            message.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();

            try
            {
                //await 
                smtp.Send(message);
                isOK = true;

            }
            catch (Exception e)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("sendMessaje : ", ref idError, e);
                throw new Exception("Error sendMessaje : " + idError, e.InnerException);
                msj = e.Message + " : " + idError;
            }
            

            return new JsonResult() { Data="{isOK:" + isOK.ToString() + ", msj:" + msj + "}"};
        }

        public ActionResult ResultadoCotizaciones(ConsultaCotizacion.Datos datos)
        {
            List<object> result = new List<object>();
            result = new TestData().ConsultaCotizacion();

            DataGrid dataGid = new DataGrid(result);

            ViewBag.DSConsulta = result;
            ViewBag.GVConsulta = dataGid.DataHeaderTable;

            return View();
        }

        public ActionResult ConsultasCotizaciones()
        {
            ViewBag.Producto = General.Productos;

            return View(ConsultaCotizacion.Datos.NameView, new ConsultaCotizacion.Datos());
        }
    }
}