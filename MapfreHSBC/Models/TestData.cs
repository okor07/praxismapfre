using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MapfreHSBC.Models.Cotizacion;
using GEN = MapfreHSBC.Models.General;
using Newtonsoft.Json.Linq;
using MapfreHSBC.Models.General;

namespace MapfreHSBC.Models
{
    public class TestData
    {
        #region TestMethods
        public void TestCotizacion(DatoCotizacion.Datos datos, ref DatoCotizacion.Respuesta resp)
        {
            resp.codigoError = 0;
            resp.descripcionError = "OK";
            resp.msgJson.url = string.Format("{0}://{1}/{2}?{3}={4}", 
                HttpContext.Current.Request.Url.Scheme, 
                HttpContext.Current.Request.Url.Authority, 
                ConfirmacionCotizacion.PAGE, 
                ConfirmacionCotizacion.PARAM,
                System.Net.WebUtility.UrlEncode(datos.ToJson()));
            //resp.Encrypt("My-Sup3r-Secr3t-Key", resp);
        }

        public void TestConfirmacionCotizacion(ConfirmacionCotizacion.Datos datos, ref ConfirmacionCotizacion.Respuesta resp)
        {
            resp.codigoError = 1;
            resp.descripcionError = "Desc error, si existe";
        }

        public void TestDatosContratante(DatosContratante.Datos datos, ref DatosContratante.Respuesta resp)
        {
            if (datos.msgJson.datosBeneficiarios != null && datos.msgJson.datosBeneficiarios.Count() > 5)
            {
                resp.codigoError = 0;
                resp.descripcionError = "No se aceptan mas de 5 benericiarios.";
            }
            else
            {
                resp.codigoError = 1;
                if (datos.msgJson.datosBeneficiarios == null)
                {
                    resp.descripcionError = "Se recibieron datos del contratante";
            }
                else
                {
                    resp.descripcionError = "Se recibieron datos del contratante y " + datos.msgJson.datosBeneficiarios.Count() + " beneficiarios";
                }
            }
            
            resp.msgJson.url = "";
            //resp.url = string.Format("{0}://{1}/{2}/{3}",
            //    HttpContext.Current.Request.Url.Scheme,
            //    HttpContext.Current.Request.Url.Authority,
            //    ConfirmacionCotizacion.PAGE,
            //    System.Net.WebUtility.UrlEncode(datos.ToJson()));
        }

        public List<object> GetDistribucion(int tipo)
        {
            List<object> result = new List<object>();

            if (tipo == 1)
            {
                result.Add(new ConfirmacionCotizacion.Distribucion() { tipoInversion = "Inversion1", porcentajeAnual = "2.30%", porcentaje = "50%", distribucion = "560,000" });
                result.Add(new ConfirmacionCotizacion.Distribucion() { tipoInversion = "Inversion2", porcentajeAnual = "3.40%", porcentaje = "35%", distribucion = "392,000" });
                result.Add(new ConfirmacionCotizacion.Distribucion() { tipoInversion = "Inversion3", porcentajeAnual = "4.75%", porcentaje = "15%", distribucion = "168,000" });
                result.Add(new ConfirmacionCotizacion.Distribucion() { tipoInversion = "Inversion4", porcentajeAnual = "5.70%", porcentaje = "0%", distribucion = "-" });
                result.Add(new ConfirmacionCotizacion.Distribucion() { tipoInversion = "Inversion5", porcentajeAnual = "8.30%", porcentaje = "0%", distribucion = "-" });
            }


            return result;
        }

        public IEnumerable<DistribucionFondos> getDistribucionFondos()
        {
            IEnumerable<DistribucionFondos> fondos = new List<DistribucionFondos>()
            {
                new DistribucionFondos()
                {
                    tipoInversion="Inversión 1", anio="2016", pctAnio=2.5, pctDistribucion=50, distInicial = 500000
                },
                new DistribucionFondos()
                {
                    tipoInversion="Inversión 2", anio="2016", pctAnio=3.5, pctDistribucion=30, distInicial = 300000
                },
                new DistribucionFondos()
                {
                    tipoInversion="Inversión 3", anio="2016", pctAnio=4.7, pctDistribucion=20, distInicial = 200000
                },
            };

            return fondos;
        }

        public List<object> GetCoberturas(int tipo)
        {
            List<object> result = new List<object>();

            if (tipo == 1)
            {
                result.Add(new ConfirmacionCotizacion.Coberturas() { nombre = "Fallecimiento", porcentaje = "1%" });
                result.Add(new ConfirmacionCotizacion.Coberturas() { nombre = "Muerte accidental", porcentaje = "20%" });
            }

            return result;
        }

        public List<object> GetPrecios(DatoCotizacion.Datos datos, DatoCotizacion.Complemento comp)
        {
            List<object> result = new List<object>();

            result.Add(new ConfirmacionCotizacion.Precios() { prima = "Derecho de póliza", monto = "0.00" });
            result.Add(new ConfirmacionCotizacion.Precios() { prima = "Prima Total", monto = "10,000.00" });

                //result.ForEach(obj => total += decimal.Parse((obj as ConfirmacionCotizacion.Precios).monto));
            

            return result;
        }

        public List<object> GetPrecios(ConfirmacionCotizacion.Datos datos)
        {
            List<object> result = new List<object>();

            result.Add(new ConfirmacionCotizacion.Precios() { prima = "Derecho de póliza", monto = "0.00" });
            result.Add(new ConfirmacionCotizacion.Precios() { prima = "Prima Total", monto = "10,000.00" });

            //result.ForEach(obj => total += decimal.Parse((obj as ConfirmacionCotizacion.Precios).monto));


            return result;
        }

        public string GetIdCotizacion(DatoCotizacion.Datos datos, DatoCotizacion.Complemento comp)
        {
            return Guid.NewGuid().ToString();
        }

        public string GetIdCotizacion()
        {
            return Guid.NewGuid().ToString();
        }

        public DatosContratante.Datos GetContratante()
        {
            DatosContratante.Datos datos = new DatosContratante.Datos();
            datos.msgJson.tipoDePersona = "F";
            datos.msgJson.nombreDeContratante = "Juan";
            datos.msgJson.apellidoMaternoContrante = "Lopez";
            datos.msgJson.apellidoPaternoContrante = "Duran";

            return datos;
        }

        //public List<object> GetCotizaciones(/*DatoCotizacion.Datos datos, DatoCotizacion.Complemento comp*/)
        //{
        //    List<object> result = new List<object>();

        //    result.Add(new ConsultaCotizaciones.Cotizaciones() { idCotizacion = "123123", fecCotizacion="07/12/2016",fecNacimiento="11/07/1990",montoAportacionAd="12,345.00",perioAportacionAd ="Semestral" , detalle = "Detalle" });
        //    result.Add(new ConsultaCotizaciones.Cotizaciones() { idCotizacion = "123123", fecCotizacion = "07/12/2016", fecNacimiento = "11/07/1990", montoAportacionAd = "12,345.00", perioAportacionAd = "Trimestral", detalle = "Detalle" });
        //    result.Add(new ConsultaCotizaciones.Cotizaciones() { idCotizacion = "123123", fecCotizacion = "07/12/2016", fecNacimiento = "11/07/1990", montoAportacionAd = "12,345.00", perioAportacionAd = "Mesual", detalle = "Detalle" });

        //    return result;
        //}

        public void TestGetCotizacion(int id, ref DatoCotizacion.Datos resp)
        {
            resp.msgJson.correoElectronico = "asdf@asdf.com";
            resp.msgJson.fechaDeNacimiento= DateTime.Now.ToString("dd/MM/yyyy");
            resp.msgJson.idPromotor = "HSBC111";
            resp.msgJson.idTransaccion = 13241234;
            resp.msgJson.sexo = 1;
        }

        //WS5
        public void TestProcesoCobro(ProcesoCobro.Datos datos,ref ProcesoCobro.Respuesta resp)
        {
            if(datos.msgJson.idTransaccion == null){
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta idTransacción";
            }
            else if(datos.msgJson.idPromotor == null){
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta idPromotor";
            }
            else if(datos.msgJson.idCotizacionMapfre == null){
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta idCotizacionMapfre";
            }
            else if (datos.msgJson.codigoError == null)
            {
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta codigoError";
            }
            else if(datos.msgJson.descripcionError == null){
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta descripcionError";
            }
            else {
                resp.codigoError = 0;
                resp.descripcionError = "Datos recibidos correctamente";
            }
                
        }

        //WS7
        public void TestCotizacionMapfre(CotizacionMapfre.Datos datos,ref CotizacionMapfre.Respuesta resp)
        {

            if(datos.msgJson.idTransaccion == null){
                resp.msgJson.statusCotizacion = 1;
                resp.msgJson.statusMessaje = "Error, Falta capturar idTransaccion";
            }
            else if(datos.msgJson.idCotizacionMapfre == null){
                resp.msgJson.statusCotizacion = 1;
                resp.msgJson.statusMessaje = "Error, Falta capturar idCotizacionMapfre";
            }
            else
            {
                resp.msgJson.idTransaccion = (long)datos.msgJson.idTransaccion;
                resp.msgJson.idCotizacionMapfre = datos.msgJson.idCotizacionMapfre;
                resp.msgJson.statusCotizacion = 0;
                resp.msgJson.statusMessaje = "Datos recibidos correctamente";
            }

        }

        public List<object> ConsultaCotizacion()
        {
            List<object> consult = new List<object>();
            consult.Add(new ConsultaCotizacion.Resultados() { idCotizacion = "321654987", periodicidad = "Semestral", montoAportacion = "10,000", fechaCotizacion = DateTime.Now.AddYears(-2), fechaNacimiento = DateTime.Now.AddMonths(2).AddDays(20).AddYears(-30) });
            consult.Add(new ConsultaCotizacion.Resultados() { idCotizacion = "654987321", periodicidad = "Anual", montoAportacion = "5,000", fechaCotizacion = DateTime.Now.AddYears(-2), fechaNacimiento = DateTime.Now.AddMonths(6).AddDays(10).AddYears(-20) });
            consult.Add(new ConsultaCotizacion.Resultados() { idCotizacion = "794653200", periodicidad = "Timestral", montoAportacion = "700", fechaCotizacion = DateTime.Now.AddYears(-2), fechaNacimiento = DateTime.Now.AddMonths(-3).AddYears(-33) });
            consult.Add(new ConsultaCotizacion.Resultados() { idCotizacion = "951847620", periodicidad = "Mensaul", montoAportacion = "11,000", fechaCotizacion = DateTime.Now.AddYears(-2), fechaNacimiento = DateTime.Now.AddMonths(-6).AddDays(-2).AddYears(-53) });
            consult.Add(new ConsultaCotizacion.Resultados() { idCotizacion = "315094873", periodicidad = "Trimestral", montoAportacion = "13,500", fechaCotizacion = DateTime.Now.AddYears(-2), fechaNacimiento = DateTime.Now.AddMonths(1).AddDays(3).AddYears(-22) });

            return consult;
        }

        public string GetPreciosHTML(DatoCotizacion.Datos datos, DatoCotizacion.Complemento comp)
        {
            List<object> result = new List<object>();
           // result = new TestData().GetPrecios(datos, comp);

            List<DataGrid.ColumnCustom> cols = new List<DataGrid.ColumnCustom>();
            cols.Add(new DataGrid.ColumnCustom() { Header = "Prima", DataField = "prima" });
            cols.Add(new DataGrid.ColumnCustom() { Header = ((ConfirmacionCotizacion.Precios)result.Last()).monto, DataField = "monto" });

            DataGrid dataGid = new DataGrid(cols);


            System.Web.Helpers.WebGrid grid = new System.Web.Helpers.WebGrid(source: result, rowsPerPage: 5, canPage: true);

            return grid.GetHtml(htmlAttributes: new { id = "gdvPrecio" },
                                     columns: dataGid.DataHeaderTable,
                                     tableStyle: "wgTable",
                                     alternatingRowStyle: "wgRowAlter",
                                     rowStyle: "wgRow").ToString();
        }
        #endregion
    }
}