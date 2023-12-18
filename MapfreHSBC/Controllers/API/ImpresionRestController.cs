using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;





namespace MapfreHSBC.Controllers.API
{
    public class ImpresionRestController : ApiController
    
    {
        string idError;
        // GET: api/CotizarRest
        [Route("api/rest/services/documentos")]
        [HttpGet, HttpPost]

        public IEnumerable<ComboBase> getDocumentos()
        {
            return new ImpresionDao().getDocumentos();
        }

        //Genera la impresion de los documentos
        [Route("api/rest/services/impresion")]
        [HttpGet, HttpPost]
        public void XMLImpresionDoc(Impresion imp)
        {
            
        }

        [Route("api/rest/services/mail")]
        [HttpGet, HttpPost]
        public string envioCorreo(Impresion imp)
        {
            String ret = "Correo enviado exitosamente";

            string mensaje = imp.mensaje;
            string doc = imp.value;

            if (imp.value.Equals("XX"))
            {
                doc = "P";
            }

            try
            {
                
                //MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En api/rest/services/mailing numCotizacion : " + datosCorreo.numCotizacion, null);
                SmtpClient client = new SmtpClient();
                client.Host = ConfigurationSettings.AppSettings["servidorCorreo"];
                client.EnableSsl = false;
                //System.Diagnostics.Debug.WriteLine("datosCorreo: " + datosCorreo.ToString());
                //MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosCorreo: " + datosCorreo.ToString(), null);

                byte[] arr = null;

                try
                {
                    arr = new ImpresionDao().CreaPDFHPExstream("1", imp.noPoliza, doc);// String xmlHPExstream = obtenXmlHpExstream(codCia, noPoliza, document);

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Obtuvo los bytes", null);
                }
                catch (Exception ex)
                {
                    
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("envioCorreo Excepcion: ", ref idError, ex);
                    arr = new Cotizacion().GetPDFPolizaHard();
                }

                if (arr == null)
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("No obtuvo los bytes", null);
                    arr = new Cotizacion().GetPDF("");
                }
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("SendEmail", null);

                MailMessage mailMessage = new MailMessage();
                MailAddress desde = new MailAddress(imp.remitente);
                mailMessage.From = desde;
                mailMessage.To.Add(imp.destinatario);
                mailMessage.Subject = imp.asunto;
                mailMessage.Body = mensaje;

                if (imp.value.Equals("XX"))
                {
                    string dataFilePath;
                    string path;

                    //Obtiene los Bytes del triptico para adjuntarlo en el correo
                    byte[] triptico = null;
                    dataFilePath = "~/Documents/Triptico_beneficios_vida.pdf";
                    path = HttpContext.Current.Server.MapPath(dataFilePath);
                    triptico = System.IO.File.ReadAllBytes(path);

                    //Obtiene los Bytes del triptico para adjuntarlo en el correo
                    byte[] condGenerales = null;
                    dataFilePath = "~/Documents/CG-UNITINV-HSBC.pdf";
                    path = HttpContext.Current.Server.MapPath(dataFilePath);
                    condGenerales = System.IO.File.ReadAllBytes(path);

                    mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(arr), string.Format("Poliza{0}_{1}.pdf", imp.noPoliza, DateTime.Now.Date.ToShortDateString())));
                    mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(triptico), string.Format("Triptico{0}_{1}.pdf", imp.noPoliza, DateTime.Now.Date.ToShortDateString())));
                    mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(condGenerales), string.Format("Condiciones_Generales{0}_{1}.pdf", imp.noPoliza, DateTime.Now.Date.ToShortDateString())));
                    client.Send(mailMessage);
                }
                else if (imp.value.Equals("P"))
                {
                    mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(arr), string.Format("Poliza{0}_{1}.pdf", imp.noPoliza, DateTime.Now.Date.ToShortDateString())));
                    client.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Excepcion en mailing", ref idError, ex);
                System.Diagnostics.Debug.WriteLine("Exception en envioCorreo " + ex.ToString());
                ret = "Error al enviar correo : " + idError;
            }

        return ret;
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
