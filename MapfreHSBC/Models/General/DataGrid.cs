using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;

namespace MapfreHSBC.Models.General
{
    public class DataGrid
    {
        #region Constantes
        const string HEADERATTR = "Header";
        #endregion

        #region Variables
        private List<WebGridColumn> columns;
        private List<dynamic> dsGrid;
        #endregion

        #region Propiedades
        public List<dynamic> DataSourceTable
        {
            get { return dsGrid; }
        }

        public List<WebGridColumn> DataHeaderTable
        {
            get
            {
                return columns;
            }
        }
        #endregion

        #region Constructor
        public DataGrid(List<object> datos)
        {
            string attrHeader = string.Empty;

            columns = new List<WebGridColumn>();

            foreach (var prop in datos.First().GetType().GetProperties())
            {
                attrHeader = this.GetValueAttr(prop.GetCustomAttributes(true), HEADERATTR);
                this.CreateRowHeader(prop.Name, attrHeader, prop.PropertyType);
            }
        }

        public DataGrid(List<ColumnCustom> columnas)
        {
            columns = new List<WebGridColumn>();

            foreach (ColumnCustom col in columnas)
            {
                if(col.Header.Equals("Detalle")){
                    this.CreateRowHeaderButton(col.DataField, col.Header);
                }else
                this.CreateRowHeader(col.DataField, col.Header);
            }
                
        }

        #endregion

        #region Métodos Privados
        private string GetValueAttr(object[] attrs, string nameAttr)
        {
            string value = string.Empty;

            if (attrs == null || attrs.Count() == 0)
                return string.Empty;

            var attr = attrs.Single(a => a is AttrProperty);

            if (attr != null)
                value = ((AttrProperty)attr).Header;

            switch (value)
            { 
                case General.ANIO:
                    value = DateTime.Now.Year.ToString();
                    break;
            }

            return value;
        }

        private void CreateRowHeader(string dataField, string header, Type tipo)
        {
            columns.Add(new WebGridColumn() { ColumnName = dataField, Header = header });
        }

        private void CreateRowHeader(string dataField, string header)
        {
            columns.Add(new WebGridColumn() { ColumnName = dataField, Header = header});
        }

        private void CreateRowHeaderButton(string dataField, string header)
        {
            columns.Add(new WebGridColumn() { ColumnName = dataField, Header = header, Format = (item) => new HtmlString(String.Format("<a href=\"http://localhost:21371/DatosCotizacion/CompletarCotizacion?DatosP=%7B%22idTransaccion%22%3Anull%2C%22idPromotor%22%3A%22e%22%2C%22fechaDeNacimiento%22%3A%221990-11-07T00%3A00%3A00%22%2C%22sexo%22%3A0%2C%22correoElectronico%22%3A%22e%22%7D\" class=\"{2}\">{1}</a>", item.Detalle, item.Detalle, "")) });
        }

        private void CreateRow(string dataField, string header, Type tipo)
        {
            dsGrid.Add(new WebGridColumn() { ColumnName = dataField, Header = header });
        }

        private HtmlString FormatControl(string text)
        {
            string html = "<span>{0}</span>";

            HtmlString htmlS = new HtmlString(string.Format(html, text));

            return htmlS;
        }
        #endregion

        #region Clases
        public class ColumnCustom
        {
            public string Header
            { get; set; }

            public string DataField
            { get; set; }
        }
        #endregion
    }
}