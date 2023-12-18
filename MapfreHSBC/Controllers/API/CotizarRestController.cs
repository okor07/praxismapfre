using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MapfreHSBC.Models;
using MapfreHSBC.Models.Cotizacion;
using System.Net.Mail;
using System.Configuration;
using MapfreHSBC.Models.General;

namespace MapfreHSBC.Controllers.API
{
    public class CotizarRestController : ApiController
    {

        [Route("api/rest/services/ws1")]
        [HttpPost]
        public DatoCotizacion.Respuesta DatosCotizacion(DatoCotizacion.Datos datos)
        {
            return new CotizarDao().getRespuestaWs1(datos);
        }

        [Route("api/rest/services/ws2")]
        [HttpPost]
        public DatoCotizacion.RespuestaWs2 getRespuestaWs2(DatoCotizacion.Datos datos)
        {
            return new CotizarDao().getRespuestaWs2(datos);
        }

        [Route("api/rest/services/ws3")]
        [HttpPost]
        public DatosContratante.Respuesta getRespuestaWs3(DatosContratante.Datos datos)
        {
            return new CotizarDao().getRespuestaWs3(datos);
        }

        [Route("api/rest/services/ws5")]
        [HttpPost]
        public ProcesoCobro.Respuesta getRespuestaWs5(ProcesoCobro.Datos datos)
        {
            return new CotizarDao().getRespuestaWs5(datos);
        }

        [Route("api/rest/services/ws7")]
        [HttpPost]
        public CotizacionMapfre.Respuesta getRespuestaWs7(CotizacionMapfre.Datos datos)
        {
            return new CotizarDao().getRespuestaWs7(datos);
        }


        [Route("api/rest/services/ws8")]
        [HttpPost]
        public InformacionCP.Respuesta getDatosCodigoPostal(InformacionCP.Datos datos)
        {
            return new ConsultaDao().getCP(datos.msgJson.idTransaccion, datos.msgJson.cp);
        }
        // GET: api/CotizarRest
        [Route("api/rest/services/fondos")]
        [HttpGet, HttpPost]
        public IEnumerable<Models.Cotizacion.DistribucionFondos> 
            getDistribucionFondos(double primaInicial, double pctDistribucion1, double pctDistribucion2,
                double pctDistribucion3,double pctDistribucion4, double pctDistribucion5, long idCotizacion, int cod_modalidad)
        {

            string parametros = "[cod_cia=1][cod_ramo=112][DVCOD_MODALIDAD=" + cod_modalidad + "][NUM_CONTRATO=" + (ConfigurationManager.AppSettings["NUM_CONTRATO"] != null ? ConfigurationManager.AppSettings["NUM_CONTRATO"].ToString() : "") + "]";

            return new CotizarDao().getDistribucionFondos(primaInicial, pctDistribucion1, pctDistribucion2, pctDistribucion3,pctDistribucion4,pctDistribucion5, idCotizacion, parametros);
        }

        [Route("api/rest/services/fondos/edit")]
        [HttpPost]
        //public IEnumerable<Models.Cotizacion.DistribucionFondos> getDistribucionFondos([FromBody]Models.Cotizacion.DistribucionFondos fondo, int primaInicial)
        public void getDistribucionFondos([FromBody]Models.Cotizacion.DistribucionFondos fondo, int primaInicial)
        {
            System.Diagnostics.Debug.WriteLine("primaInicial " + primaInicial);
            if (fondo != null)
            {                
                System.Diagnostics.Debug.WriteLine("pctDistribucion " + fondo.pctDistribucion);
                System.Diagnostics.Debug.WriteLine("tipoInversion " + fondo.tipoInversion);
                System.Diagnostics.Debug.WriteLine("idInversion " + fondo.idInversion);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Es Nulo ");

            }
            //return new CotizarDao().getDistribucionFondos(primaInicial, fondo.idInversion, fondo.pctDistribucion);
        }

        // GET: api/CotizarRest
        [Route("api/rest/services/perfiles")]
        [HttpGet]
        public IEnumerable<ComboBase> getPerfiles(int cod_cia, int cod_ramo, int cod_modalidad)
        {
            string fecha = DateTime.Now.ToString("ddMMyyyy");
            //fecha = "20122016";
            string param = "[dvcod_modalidad=" + cod_modalidad + "][fec_efec_poliza='" + fecha + "'][cod_cia=" + cod_cia + "][cod_ramo=" + cod_ramo + "]][num_contrato=" + (ConfigurationManager.AppSettings["NUM_CONTRATO"] != null ? ConfigurationManager.AppSettings["NUM_CONTRATO"].ToString() : "") + "]";
            return new CotizarDao().getPerfiles("TA899039", 2, param); // mandar 2 en lugar de 1
            //return new CotizarDao().getPerfiles("TA899039", 1, param); // mandar 2 en lugar de 1
        }
        
        [Route("api/rest/services/dias")]
        [HttpGet]
        public IEnumerable<String> getDiasCobro()
        {
            return new CotizarDao().getDiasCobro();
        }

        [Route("api/rest/services/monedas")]
        [HttpGet]
        public IEnumerable<ComboBase> getMoneda()
        {
            //return new CotizarDao().getMoneda(112,"99999",0);
            return new CotizarDao().getMoneda();
        }

        [Route("api/rest/services/modalidades")]
        [HttpGet]
        public IEnumerable<ComboBase> getModalidades()
        {
            string fecha = DateTime.Now.ToString("ddMMyyyy");
            //string param = "[fec_efec_poliza='" + fecha + "'][cod_cia=1][cod_ramo=112]";
            string param = "[fec_efec_poliza='" + fecha + "'][cod_cia=1][cod_ramo=112][num_contrato=" + (ConfigurationManager.AppSettings["NUM_CONTRATO"] != null ? ConfigurationManager.AppSettings["NUM_CONTRATO"].ToString() : "") + "]";

            return new CotizarDao().getModalidad("G2990004", 13, param);
        }

        [Route("api/rest/services/periodicidades")]
        [HttpGet]
        public IEnumerable<ComboBase> getPeriodicidades(int codModalidad)
        {
            //return new CotizarDao().getPeriodicidades(codModalidad);

            string fecha = DateTime.Now.ToString("ddMMyyyy");
            //fecha = "20122016";
            string param = "[dvcod_modalidad=" + codModalidad + "][fec_efec_poliza='" + fecha + "'][cod_cia=1][cod_ramo=112]";

            return new CotizarDao().getPeriodicidad("A1001402", 10, param);//  .getPerfiles("TA899039", 1, param);


        }

        [Route("api/rest/services/plazos")]
        [HttpGet]
        //public IEnumerable<ComboBase> getPlazos(int cod_moneda)
        public IEnumerable<Plazo> getPlazos(string modalidad)
        {
            //string param = "[COD_CIA=1][COD_RAMO=112][COD_MON=" + cod_moneda + "][TIP_REGULARIZA_SUMA=0]";
            return new CotizarDao().getPlazos(modalidad);
        }

        [Route("api/rest/services/coberturas")]
        [HttpGet]
        public IEnumerable<Models.Cotizacion.Cobertura> getCoberturas()
        {
            return new CotizarDao().getCoberturas();
        }

        [Route("api/rest/services/calculamonto")]
        [HttpGet]
        public IEnumerable<Models.Cotizacion.PrecioSeguro> getCalculaMonto(double prima, long idCotizacion)
        {
            return new CotizarDao().getCalculaMonto(prima,idCotizacion);
        }

        [Route("api/rest/services/calculaprecio")]
        [HttpGet]
        public IEnumerable<Models.Cotizacion.PrecioSeguro> getCalculaPrecio(double precio)
        {
            return new CotizarDao().getCalculaPrecio(precio);
        }

        [Route("api/rest/services/idcotizacion")]
        [HttpGet, HttpPost]
        public long getIdCotizacion([FromBody]AltaCotizacion datosAlta)
        {
            string[] fecha = datosAlta.fechaNacimiento.Split('/');
            DateTime fecNac = new DateTime(Convert.ToInt16(fecha[2]), Convert.ToInt16(fecha[1]), Convert.ToInt16(fecha[0]));
            DateTime fecActual = DateTime.Now;
            var dateSpan = DateTimeSpan.CompareDates(fecNac, fecActual);
            int anios = dateSpan.Years;

            int edadAlcanzada = Convert.ToInt32(datosAlta.plazo);

            string plazo = (edadAlcanzada - anios).ToString();
            datosAlta.plazo = plazo;
            
            datosAlta.msgJson = datosAlta.msgJson.Replace("\"plazo\":\""+edadAlcanzada+"\"","\"plazo\":\""+plazo+"\"");

            string[] json = datosAlta.msgJson.Split(',');
            datosAlta.periodicidadText = json[13].Split(':')[1].Replace("\"","");

            long idCotizacion = new CotizarDao().getIdCotizacion(datosAlta);            
            return idCotizacion;
        }

        [Route("api/rest/services/edadValida")]
        [HttpGet, HttpPost]
        public string edadValida([FromBody]Edad edad)
        {

            string[] fecha = edad.fecNacimiento.Split('/');
            DateTime fecNac = new DateTime(Convert.ToInt16(fecha[2]), Convert.ToInt16(fecha[1]), Convert.ToInt16(fecha[0]));
            DateTime fecActual = DateTime.Now;
            var dateSpan = DateTimeSpan.CompareDates(fecNac, fecActual);
            int anios = dateSpan.Years;

            string edadMin = ConfigurationSettings.AppSettings["edadMinima"];
            string edadMax = ConfigurationSettings.AppSettings["edadMax"+edad.modalidad];
            string plazoMin = ConfigurationSettings.AppSettings["plazoMin" + edad.modalidad];



            int EA = Convert.ToInt32(edad.edadAlcanzada);

            if (anios < Convert.ToInt64(edadMin))
            {
                return "La edad minima de aceptación es de 18 años.";
            }

            if(anios > Convert.ToInt64(edadMax))
            {
                return "La edad maxima de aceptación es de "+edadMax+" años.";
            }

            if (EA - anios < Convert.ToInt64(plazoMin))
            {
                return "La edad del contratante no es valida para el plazo seleccionado";
            }
            else
            {
                return null;
            }

        }

        [Route("api/rest/services/invokeWs2")]
        [HttpGet, HttpPost]
        public string invokeWs2([FromBody]ConfirmacionCotizacion.Datos datos)
        {
            return  new CotizarDao().invokeWS2(datos);
        }

        [Route("api/rest/services/invokeWs4")]
        [HttpGet, HttpPost]
        public string invokeWs4([FromBody]IntentoCobro.Datos datos)
        {
            return new CotizarDao().invokeWS4(datos);
        }

        [Route("api/rest/services/invokeWs1")]

        [HttpGet, HttpPost]
        public void invokeWs1()
        {
            new CotizarDao().invokeWS1();

        }


        [Route("api/rest/services/recursos")]
        [HttpGet]
        public IEnumerable<ComboBase> getRecursosPago()
        {
            return new CotizarDao().getRecursosPago();
        }

        [Route("api/rest/services/formapago")]
        [HttpGet]
        public IEnumerable<ComboBase> getFormaPago()
        {
            //MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getFormaPago", null);
            return new CotizarDao().getFormaPago();
        }

        [Route("api/rest/services/mailing")]
        [HttpGet, HttpPost]
        public string envioCorreo([FromBody]Models.General.DatosCorreo datosCorreo)
        {
            String ret = "Correo enviado exitosamente";
            try
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En api/rest/services/mailing numCotizacion : " + datosCorreo.numCotizacion, null);
                
                SmtpClient client = new SmtpClient();
                client.Host = ConfigurationSettings.AppSettings["servidorCorreo"];
                client.EnableSsl = false;
               
                System.Diagnostics.Debug.WriteLine("datosCorreo: " + datosCorreo.ToString());
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosCorreo: " + datosCorreo.ToString(), null);

                byte[] arr = null;
                try
                {
                    arr = new CotizarDao().obtienePdfCotizacion(datosCorreo.modalidad,datosCorreo.numCotizacion, datosCorreo.fecNacimiento, datosCorreo.sexo, datosCorreo.correo, datosCorreo.moneda, datosCorreo.plazo,
                    datosCorreo.primaIni, datosCorreo.primaAdd, datosCorreo.frecuenciaPrima, datosCorreo.perfil, datosCorreo.pctInversion1, datosCorreo.pctInversion2, datosCorreo.pctInversion3);

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Obtuvo los bytes", null);
                }
                catch (Exception ex)
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("envioCorreo Excepcion: ", ex);
                    arr = new Cotizacion().GetPDF("");
                }
                if (arr == null)
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("No obtuvo los bytes", null);
                    arr = new Cotizacion().GetPDF("");
                }
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("SendEmail", null);

                MailMessage mailMessage = new MailMessage();
                MailAddress desde = new MailAddress(datosCorreo.remitente);
                mailMessage.From = desde;
                mailMessage.To.Add(datosCorreo.para);
                mailMessage.Subject = datosCorreo.asunto;
                mailMessage.Body = datosCorreo.mensaje;
                mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(arr), string.Format("Cotizacion_{0}_{1}.pdf", datosCorreo.idCotizacion.ToString(), DateTime.Now.Date.ToShortDateString())));//Response.OutputStream, Response.ContentType));
                client.Send(mailMessage);
                
            }
            catch (Exception ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Excepcion en mailing", ex);
                System.Diagnostics.Debug.WriteLine("Exception en envioCorreo" + ex.ToString());
                ret = "Error al enviar correo ";
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
