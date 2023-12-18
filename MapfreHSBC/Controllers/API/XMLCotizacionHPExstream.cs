using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Globalization;

namespace MapfreHSBC.Controllers.API
{
    public class XMLCotizacionHPExstream
    {
        //Genera el XML para la impresion de la cotización
        public string getXMLPrintCotizacionHP(string modalidad,string numCotizacion, string edad, string sexo, string correo, 
            string moneda, string plazo, string primaIni, string primaAdd, string frecuenciaPrima, 
            string perfil, string pctInversion1, string mntoInversion1, string pctInversion2, string mntoInversion2, 
            string pctInversion3, string mntoInversion3, string aportacionesAdd, string rendimientoProy, string fondoAcum) 
        {
            string strXMLHP = String.Empty;

            XmlDocument xmlDoc2 = new XmlDocument();
            XmlNode xmlRoot2;
     
            xmlRoot2 = xmlDoc2.CreateNode(XmlNodeType.Element, "ROOT", null);

            //Generacion de XML
            datosGenerales(xmlRoot2,xmlDoc2);
            datosDistribucion(xmlRoot2,xmlDoc2);
            datosCabeceraHP(xmlRoot2, xmlDoc2, numCotizacion, modalidad);
            datosGenerales2(xmlRoot2,xmlDoc2);
            datosAsegurado(xmlRoot2,xmlDoc2, edad, sexo, correo);//EXTRAER INFORMACION DE PANTALLAPLAZO_SEGURO
            datosCotizacion(xmlRoot2, xmlDoc2, moneda, plazo, primaIni, primaAdd, frecuenciaPrima);
            datosLeyendaCob(xmlRoot2, xmlDoc2);
            //Jose Marcos solicito elimiar el nodo <TASAS_INTERES>
            //datosTasaInversion(xmlRoot2,xmlDoc2, perfil,pctInversion1,mntoInversion1,pctInversion2,mntoInversion2,pctInversion3,mntoInversion3);
            avisos2(xmlRoot2, xmlDoc2);
            //Jose Marcos solicito elimiar el nodo FONDOS_AHORRO Y AVISOS
            //datosFondo(xmlRoot2, xmlDoc2, primaIni,aportacionesAdd,rendimientoProy,fondoAcum);
            //avisos(xmlRoot2,xmlDoc2);
            imagenes(xmlRoot2, xmlDoc2);

            strXMLHP = xmlRoot2.OuterXml;

            return System.Web.HttpUtility.HtmlDecode(strXMLHP);

        }

        //Muestra los datos generales del reporte
        public void datosGenerales(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            xmlRoot2.AppendChild(this.regresaNodoXml2("COMPANIA", "1", xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("RAMO", "112", xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("ACTIVIDAD", "COTIZACION", xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("IDIOMA", "mx_ES", xmlDoc2));
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosGenerales", null);

        }

        //Muestra los datos de distribucion del reporte
        public void datosDistribucion(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            XmlNode nodoDistribucion;

            nodoDistribucion = xmlDoc2.CreateNode(XmlNodeType.Element, "DISTRIBUCION", null);
            nodoDistribucion.AppendChild(this.regresaNodoXml2("DUPLEX_MODE","false",xmlDoc2));
            nodoDistribucion.AppendChild(this.regresaNodoXml2("LOCAL", "true", xmlDoc2));

            xmlRoot2.AppendChild(nodoDistribucion);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosDistribucion", null);

        }
        //Muestra los datos de cabecera del reporte
        public void datosCabeceraHP(XmlNode xmlRoot2, XmlDocument xmlDoc2, string strCotizacion, string modalidad)
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("modalidad: "+ modalidad, null);

            XmlNode nodoCabecera;

            nodoCabecera = xmlDoc2.CreateNode(XmlNodeType.Element, "CABECERA", null);

            if (modalidad.Equals("11201"))
            {
                nodoCabecera.AppendChild(this.regresaNodoXml2("PRODUCTO", "INVERSIÓN", xmlDoc2));
            }
            else if(modalidad.Equals("11202")){
                nodoCabecera.AppendChild(this.regresaNodoXml2("PRODUCTO", "JUBILACIÓN DIFERIDO", xmlDoc2));
            }
            else if(modalidad.Equals("11203")){
                nodoCabecera.AppendChild(this.regresaNodoXml2("PRODUCTO", "PPR", xmlDoc2));
            }

            nodoCabecera.AppendChild(this.regresaNodoXml2("NUMERO_COTIZACION", strCotizacion, xmlDoc2));
            nodoCabecera.AppendChild(this.regresaNodoXml2("FECHA_COTIZACION", DateTime.Today.ToString("dd/MM/yyyy"), xmlDoc2));

            xmlRoot2.AppendChild(nodoCabecera);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosCabeceraHP", null);

        }

        //Muestra los datos generales 2 del reporte
        public void datosGenerales2(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            //Obtiene la fecha y la formatea
            string dt = DateTime.Today.ToShortDateString();
            dt = Convert.ToDateTime(dt).ToString("dddd dd DE MMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX"));

            xmlRoot2.AppendChild(this.regresaNodoXml2("PIE", ConfigurationSettings.AppSettings["PIE"], xmlDoc2));

            xmlRoot2.AppendChild(this.regresaNodoXml2("VI_ESTADO", "", xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("VI_POBLACION", "", xmlDoc2));
            //xmlRoot2.AppendChild(this.regresaNodoXml2("VI_FECHA", DateTime.Today.ToShortDateString(), xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("VI_FECHA", dt.ToUpper(), xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("VI_DIAS_VALIDEZ", "30", xmlDoc2));

            xmlRoot2.AppendChild(this.regresaNodoXml2("REG_MERCANTIL", ConfigurationSettings.AppSettings["REGISTRO"], xmlDoc2));
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosGenerales2", null);

        }

        //Muestra los datos del asegurado del reporte
        public void datosAsegurado(XmlNode xmlRoot2, XmlDocument xmlDoc2, string strEdad, string strSexo, string strCorreo)
        {
            XmlNode nodoAsegurado;

            nodoAsegurado = xmlDoc2.CreateNode(XmlNodeType.Element, "ASEGURADO", null);
            nodoAsegurado.AppendChild(this.regresaNodoXml2("NOMBRE", "", xmlDoc2));
            nodoAsegurado.AppendChild(this.regresaNodoXml2("EDAD", strEdad, xmlDoc2));
            nodoAsegurado.AppendChild(this.regresaNodoXml2("SEXO", strSexo, xmlDoc2));
            nodoAsegurado.AppendChild(this.regresaNodoXml2("CORREO", strCorreo, xmlDoc2));

            xmlRoot2.AppendChild(nodoAsegurado);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosAsegurado", null);

        }

        //Muestra los datos de cotizacion del reporte
        public void datosCotizacion(XmlNode xmlRoot2, XmlDocument xmlDoc2, string strMoneda, string strPlazo, string strPrimaIni, string strPrimaAdd, string strFrecuenciaPrimAdd)
        {
            XmlNode nodoCotizacion;

            nodoCotizacion = xmlDoc2.CreateNode(XmlNodeType.Element, "DATOS_COTIZACION", null);
            nodoCotizacion.AppendChild(this.regresaNodoXml2("MONEDA",strMoneda,xmlDoc2));
            nodoCotizacion.AppendChild(this.regresaNodoXml2("PLAZO_SEGURO",strPlazo + " AÑOS",xmlDoc2));
            nodoCotizacion.AppendChild(this.regresaNodoXml2("PRIMA_INICIAL", Convert.ToDouble(strPrimaIni).ToString("C"),xmlDoc2));
            nodoCotizacion.AppendChild(this.regresaNodoXml2("PRIMA_ADICIONAL", Convert.ToDouble(strPrimaAdd).ToString("C"),xmlDoc2));
            nodoCotizacion.AppendChild(this.regresaNodoXml2("FRECUENCIA_PRIMA_ADICIONAL",strFrecuenciaPrimAdd,xmlDoc2));

            xmlRoot2.AppendChild(nodoCotizacion);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosCotizacion", null);

        }
        //Muestra los datos de Leyenda de Cobertura del reporte
        public void datosLeyendaCob(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            xmlRoot2.AppendChild(this.regresaNodoXml2("SUMA_COBERTURAS", ConfigurationSettings.AppSettings["SUMA_COB"], xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("VI_SUMA_MUERTE_NATURAL", "1", xmlDoc2));
            xmlRoot2.AppendChild(this.regresaNodoXml2("VI_SUMA_MUERTE_ACCIDENTAL", "20", xmlDoc2));

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosLeyendaCob", null);

        }
        //Genera Nodo de Tasa de Inversion
        public void datosTasaInversion(XmlNode xmlRoot2, XmlDocument xmlDoc2, string strperfil, string strPctInversion1, string strMntInicial1, string strPctInversion2, string strMntInicial2, string strPctInversion3, string strMntInicial3)
        {
            XmlNode nodoTasaInteres;
            XmlNode nodoInversion1;
            XmlNode nodoInversion2;
            XmlNode nodoInversion3;

            nodoTasaInteres = xmlDoc2.CreateNode(XmlNodeType.Element, "TASAS_INTERES", null);
            nodoTasaInteres.AppendChild(this.regresaNodoXml2("PERFIL_INVERSION", strperfil, xmlDoc2));

            //nodoInversion1 = xmlDoc2.CreateNode(XmlNodeType.Element, "INVERSION",null);
            //nodoInversion1.AppendChild(this.regresaNodoXml2("TIPO_INVERSION", "1", xmlDoc2));
            //nodoInversion1.AppendChild(this.regresaNodoXml2("PCT_INVERSION", strPctInversion1+"%", xmlDoc2));
            //nodoInversion1.AppendChild(this.regresaNodoXml2("MONTO_INICIAL", Convert.ToDouble(strMntInicial1).ToString("C"), xmlDoc2));

            //nodoInversion2 = xmlDoc2.CreateNode(XmlNodeType.Element, "INVERSION", null);
            //nodoInversion2.AppendChild(this.regresaNodoXml2("TIPO_INVERSION", "2", xmlDoc2));
            //nodoInversion2.AppendChild(this.regresaNodoXml2("PCT_INVERSION", strPctInversion2+"%", xmlDoc2));
            //nodoInversion2.AppendChild(this.regresaNodoXml2("MONTO_INICIAL", Convert.ToDouble(strMntInicial2).ToString("C"), xmlDoc2));

            //nodoInversion3 = xmlDoc2.CreateNode(XmlNodeType.Element, "INVERSION", null);
            //nodoInversion3.AppendChild(this.regresaNodoXml2("TIPO_INVERSION", "3", xmlDoc2));
            //nodoInversion3.AppendChild(this.regresaNodoXml2("PCT_INVERSION", strPctInversion3+"%", xmlDoc2));
            //nodoInversion3.AppendChild(this.regresaNodoXml2("MONTO_INICIAL", Convert.ToDouble(strMntInicial3).ToString("C"), xmlDoc2));


            xmlRoot2.AppendChild(nodoTasaInteres);
            //xmlRoot2.AppendChild(nodoInversion1);
            //xmlRoot2.AppendChild(nodoInversion2);
            //xmlRoot2.AppendChild(nodoInversion3);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosTasaInversion", null);

        }
        //Genera del Nodo de Avisos2
        public void avisos2(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            XmlNode nodoAvisos;

            nodoAvisos = xmlDoc2.CreateNode(XmlNodeType.Element, "AVISO_PRIVACIDAD", null);
            nodoAvisos.AppendChild(this.regresaNodoXml2("CONTENIDO",ConfigurationSettings.AppSettings["AVISO_PRIV"],xmlDoc2));

            xmlRoot2.AppendChild(nodoAvisos);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("avisos2", null);

        }
        //Genera el Nodo de Fondos
        public void datosFondo(XmlNode xmlRoot2, XmlDocument xmlDoc2, string strPrimaIni, string strAportacionesAdd, string strRendimientorProy, string strFondoAcum )
        {
            XmlNode nodoFondos;
            XmlNode nodoFondo1;
            XmlNode nodoFondo2;
            XmlNode nodoFondo3;
            XmlNode nodoFondo4;
            
            nodoFondos = xmlDoc2.CreateNode(XmlNodeType.Element, "FONDOS_AHORRO", null);
            
            nodoFondo1 = xmlDoc2.CreateNode(XmlNodeType.Element, "FONDO", null);
            nodoFondo1.AppendChild(this.regresaNodoXml2("CONCEPTO", "PRIMA INICIAL", xmlDoc2));
            nodoFondo1.AppendChild(this.regresaNodoXml2("MONTO_ESTIMADO", Convert.ToDouble(strPrimaIni).ToString("C"), xmlDoc2));

            nodoFondo2 = xmlDoc2.CreateNode(XmlNodeType.Element, "FONDO", null);
            nodoFondo2.AppendChild(this.regresaNodoXml2("CONCEPTO", "APORTACIONES ADICIONALES FUTURAS", xmlDoc2));
            nodoFondo2.AppendChild(this.regresaNodoXml2("MONTO_ESTIMADO", Convert.ToDouble(strAportacionesAdd).ToString("C"), xmlDoc2));
            /*
            nodoFondo3 = xmlDoc2.CreateNode(XmlNodeType.Element, "FONDO", null);
            nodoFondo3.AppendChild(this.regresaNodoXml2("CONCEPTO", "RENDIMIENTOS PROYECTADOS", xmlDoc2));
            nodoFondo3.AppendChild(this.regresaNodoXml2("MONTO_ESTIMADO", strRendimientorProy, xmlDoc2));

            nodoFondo4 = xmlDoc2.CreateNode(XmlNodeType.Element, "FONDO", null);
            nodoFondo4.AppendChild(this.regresaNodoXml2("CONCEPTO", "FONDO ACUMULADO AL VENCIMIENTO", xmlDoc2));
            nodoFondo4.AppendChild(this.regresaNodoXml2("MONTO_ESTIMADO", strFondoAcum, xmlDoc2));
            */

            nodoFondos.AppendChild(nodoFondo1);
            nodoFondos.AppendChild(nodoFondo2);
            //nodoFondos.AppendChild(nodoFondo3);
            //nodoFondos.AppendChild(nodoFondo4);

            xmlRoot2.AppendChild(nodoFondos);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosFondo", null);

        }
        //Genera el Nodo de Avisos
        public void avisos(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            XmlNode nodoAvisos;
            XmlNode nodoAviso;

            nodoAvisos = xmlDoc2.CreateNode(XmlNodeType.Element, "AVISOS", null);
            nodoAviso = xmlDoc2.CreateNode(XmlNodeType.Element, "AVISO", null);
            nodoAviso.AppendChild(this.regresaNodoXml2("CONTENIDO", ConfigurationSettings.AppSettings["CONTENIDO"], xmlDoc2));

            nodoAvisos.AppendChild(nodoAviso);
            xmlRoot2.AppendChild(nodoAvisos);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("avisos", null);

        }
        //Genera el Nodo de Imagenes
        public void imagenes(XmlNode xmlRoot2, XmlDocument xmlDoc2)
        {
            XmlNode nodoImagen;

            nodoImagen = xmlDoc2.CreateNode(XmlNodeType.Element, "IMAGENES", null);
            nodoImagen.AppendChild(this.regresaNodoXml2("PUBLICIDAD", ConfigurationSettings.AppSettings["PUBLI"],xmlDoc2));
            nodoImagen.AppendChild(this.regresaNodoXml2("VENTA_CRUZADA", ConfigurationSettings.AppSettings["VENTA"], xmlDoc2));

            xmlRoot2.AppendChild(nodoImagen);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("imagenes", null);

        }

        /*'**************************************************************************************************
        Crea y regresa un Nodo XML para ir construyendo el documento que servirá de entrada al generador
        de archivos PDF.
        PARÁMETROS:
        strNombreNodo: es el nombre del tag XML
        strContenido: es el valor que lleva ese nodo XML
        xmlDoc: es el documento que genera una nueva instancia de un nodo
        ***************************************************************************************************/
        private XmlNode regresaNodoXml2(string strNombreNodo ,string strContenido, XmlDocument xmlDoc2)
        {
            XmlNode xmlNodo;
            xmlNodo = xmlDoc2.CreateNode(XmlNodeType.Element, strNombreNodo, "");
            xmlNodo.InnerText = strContenido;
            return xmlNodo;
        }
    }
}