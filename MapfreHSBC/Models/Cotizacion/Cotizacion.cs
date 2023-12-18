using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MapfreHSBC.Models;
using GEN = MapfreHSBC.Models.General;
using MapfreHSBC.Models.General;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace MapfreHSBC.Models.Cotizacion
{
    public class Cotizacion : ICotizacion
    {
        #region Métodos Públicos
        public DatoCotizacion.Respuesta DatosCotizacion(DatoCotizacion.Datos datos)
        {
            DatoCotizacion.Respuesta respuesta = new DatoCotizacion.Respuesta();

            new TestData().TestCotizacion(datos, ref respuesta);

            return respuesta;
        }

        public ConfirmacionCotizacion.Respuesta ConfimacionCotizacion(ConfirmacionCotizacion.Datos datos)
        { 
            ConfirmacionCotizacion.Respuesta respuesta = new ConfirmacionCotizacion.Respuesta();

            new TestData().TestConfirmacionCotizacion(datos, ref respuesta);

            return respuesta;
        }

        public DatosContratante.Respuesta DatosContratante(DatosContratante.Datos datos)
        {
            DatosContratante.Respuesta respuesta = new DatosContratante.Respuesta();

            new TestData().TestDatosContratante(datos, ref respuesta);

            return respuesta;
        }

        public string GetIdCotizacion(JObject data)
        {
            DatoCotizacion.Datos datos = data["Datos"].ToObject<DatoCotizacion.Datos>();
            DatoCotizacion.Complemento comp = data["Complemento"].ToObject<DatoCotizacion.Complemento>();

            return new TestData().GetIdCotizacion(datos, comp);
        }

        public string getIdCotizacion(ConfirmacionCotizacion.Datos datos)
        {
            return new TestData().GetIdCotizacion();
        }

        public string GetPrecios(JObject data)
        {
            DatoCotizacion.Datos datos = data["Datos"].ToObject<DatoCotizacion.Datos>();
            DatoCotizacion.Complemento comp = data["Complemento"].ToObject<DatoCotizacion.Complemento>();

            List<object> result = new List<object>();
            result = new TestData().GetPrecios(datos, comp);

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

        public string getPrecios(ConfirmacionCotizacion.Datos datos)
        {

            List<object> result = new List<object>();
            result = new TestData().GetPrecios(datos);

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

        public string getPrecios2(ConfirmacionCotizacion.Datos datos)
        {

            List<object> result = new List<object>();
            result = new TestData().GetPrecios(datos);

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

        public byte[] GetPDF(string datos)
        {
            System.Diagnostics.Debug.WriteLine("En GetPDF ");
            if (datos != null)
            {
                System.Diagnostics.Debug.WriteLine("idCotizacion " + datos);

            }
            byte[] bytes = null;
            string dataFilePath = "~/Documents/Cotizacion.pdf";
            string path = HttpContext.Current.Server.MapPath(dataFilePath);
            System.Diagnostics.Debug.WriteLine("path " + path);
            bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }

        public byte[] GetPDFPolizaHard()
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Error: getPDFPolizaHard", null);
            string dataFilePath = "~/Documents/POLIZA_UNIT JUBILACIÓN_DRAF.pdf";
            string path = HttpContext.Current.Server.MapPath(dataFilePath);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }

        public DatoCotizacion.Datos GetCotizacion(string id)
        {
            DatoCotizacion.Datos respuesta = new DatoCotizacion.Datos();

            new TestData().TestGetCotizacion(Int32.Parse(id), ref respuesta);

            return respuesta;
        }

        public ProcesoCobro.Respuesta ProcesoCobro(ProcesoCobro.Datos datos)
        {
            ProcesoCobro.Respuesta respuesta = new ProcesoCobro.Respuesta();

           // new TestData().TestProcesoCobro(datos, ref respuesta);
            new CotizarDao().getRespuestaWs5(datos);

            return respuesta;
        }

        public CotizacionMapfre.Respuesta CotizacionMapfre(CotizacionMapfre.Datos datos)
        {

            CotizacionMapfre.Respuesta respuesta = new CotizacionMapfre.Respuesta();

            new TestData().TestCotizacionMapfre(datos, ref respuesta);

            return respuesta;
        }

        public string GetCotizaciones(JObject data)
        {
            //DatoCotizacion.Datos datos = data["Datos"].ToObject<DatoCotizacion.Datos>();
            //DatoCotizacion.Complemento comp = data["Complemento"].ToObject<DatoCotizacion.Complemento>();

            List<object> result = new List<object>();
            //result = new TestData().GetCotizaciones( datos,comp);
            //result = new TestData().GetCotizaciones();

            List<DataGrid.ColumnCustom> cols = new List<DataGrid.ColumnCustom>();
            cols.Add(new DataGrid.ColumnCustom() { Header = "ID Cotización", DataField = "idCotizacion" });
            cols.Add(new DataGrid.ColumnCustom() { Header = "Fecha de cotización", DataField = "fecCotizacion" });
            cols.Add(new DataGrid.ColumnCustom() { Header = "Fecha de Nacimiento", DataField = "fecNacimiento" });
            cols.Add(new DataGrid.ColumnCustom() { Header = "Monto de la aportación adicional", DataField = "montoAportacionAd" });
            cols.Add(new DataGrid.ColumnCustom() { Header = "Periodicidad de la aportacion adicional", DataField = "perioAportacionAd" });
            cols.Add(new DataGrid.ColumnCustom() { Header = "Detalle",DataField="detalle" });

            DataGrid dataGid = new DataGrid(cols);
            
            System.Web.Helpers.WebGrid grid = new System.Web.Helpers.WebGrid(source: result, rowsPerPage: 5, canPage: true);

            return grid.GetHtml(htmlAttributes: new { id = "gdvCotizaciones" },
                                     columns: dataGid.DataHeaderTable,
                                     tableStyle: "wgTable",
                                     alternatingRowStyle: "wgRowAlter",
                                     rowStyle: "wgRow").ToString()
                                     ;
          
        }
        public byte[] GetFolleto_Informativo()
        {            
            string dataFilePath = "~/Documents/Folleto informativo.pdf";
            string path = HttpContext.Current.Server.MapPath(dataFilePath);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }
        #endregion
    }
}