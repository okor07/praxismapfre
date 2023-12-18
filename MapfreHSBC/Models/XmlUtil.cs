using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text;
using MapfreHSBC.Models.Cotizacion;
using System.Data;
using System.Configuration;

namespace MapfreHSBC.Models
{
    public class XmlUtil
    {
        string idError;
        public String getXmlCotizacion(Hashtable parametros)
        {
            StringBuilder xml = new StringBuilder();
            try
            {

                string codCia = "1";
                string codRamo = "112";
                string fechaHoy = DateTime.Now.ToString("dd/MM/yyyy");
                string fechaVence = DateTime.Now.ToString("dd/MM/yyyy");
                int aniosMax = Int32.Parse(parametros["anios_max"].ToString());
                string codGestor = "5884";//
                string tipGestor = "AG";//

                string codGestorUsed = codGestor;//

                if (Int32.TryParse(parametros["anios_max"].ToString(), out aniosMax))
                {
                    fechaVence = DateTime.Now.AddYears(aniosMax).ToString("dd/MM/yyyy");
                }
                //header xml
                xml.Append("<XML ACCION=\"").Append(parametros["accion"] != null ? parametros["accion"] : "C").Append("\" COD_RAMO=\"").Append(codRamo).Append("\" NUM_POLIZA=\"\" COD_ORIGEN=\"1\">");
                //P2000030            
                xml.Append(getP2000030(parametros, codCia, codRamo, fechaHoy, fechaVence, tipGestor, codGestorUsed));
                //P2000031
                xml.Append(getP2000031(parametros, codCia, fechaHoy, fechaVence));
                //P2000020
                xml.Append(getP2000020(parametros, codCia, codRamo, aniosMax, null));
                //P2000025
                xml.Append(getP2000025(parametros, codCia));
                //P2000060
                xml.Append(get2000060(parametros, null));
                //P2000040 beneficiarios
                xml.Append(get2000040(parametros));
                //P2300060_MMX
                xml.Append(getP2300060_MMX(parametros));
                // cierra header xml
                // xml.Append(getP1001331(parametros));
                xml.Append("</XML>");
            }
            catch (Exception ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getXmlCotizacion: ", ref idError, ex);
                System.Diagnostics.Debug.WriteLine("ERROR XmlUtil.getXmlCotizacion() : " + idError + " - " + ex);
                throw new Exception("Error al generar XML" + ex.Message);
            }
            
            return xml.ToString();
        }

        public String getXmlEmision(Hashtable parametros, DatosContratante.Datos datos)
        {
            StringBuilder xml = new StringBuilder();
            try
            {

                string codCia = "1";
                string codRamo = "112";
                string fechaHoy = DateTime.Now.ToString("dd/MM/yyyy");
                string fechaVence = DateTime.Now.ToString("dd/MM/yyyy");
                int aniosMax = Int32.Parse(parametros["anios_max"].ToString());
                string codGestor = ConfigurationManager.AppSettings["CodGestorHSBC"]; //"5884";//
                string tipGestor = ConfigurationManager.AppSettings["TipGestorHSBC"]; //"AG";//

                string codGestorUsed = codGestor + codGestor; //codGestor;//

                string codDocum = new CotizarDao().getCodDocum();

                if (Int32.TryParse(parametros["anios_max"].ToString(), out aniosMax))
                {
                    fechaVence = DateTime.Now.AddYears(aniosMax).ToString("dd/MM/yyyy");
                }

                //Guarda numero exterior y numero interior del domicilio
                new CotizarDao().saveNoExterior(datos, codDocum);
                //header xml
                xml.Append("<XML ACCION=\"").Append(parametros["accion"] != null ? parametros["accion"] : "C").Append("\" COD_RAMO=\"").Append(codRamo).Append("\" NUM_POLIZA=\"\" COD_ORIGEN=\"1\">");
                //P2000030            
                xml.Append(getP2000030Emision(parametros, codCia, codRamo, fechaHoy, fechaVence, codDocum, tipGestor, codGestorUsed));
                //P2000031
                xml.Append(getP2000031(parametros, codCia, fechaHoy, fechaVence));
                //P2000020
                xml.Append(getP2000020(parametros, codCia, codRamo, aniosMax, datos));
                //P2000025
                xml.Append(getP2000025(parametros, codCia));
                //P2000060
                xml.Append(get2000060(parametros, codDocum));
                //P2000040 beneficiarios
                xml.Append(get2000040(parametros));
                //P2300060_MMX
                xml.Append(getP2300060_MMX(parametros));
                //Datos Contratante
                xml.Append(getP1001331(datos, codDocum, fechaHoy, parametros));

                // GESTOR DE COBRO 
                int? consecutivo = new CotizarDao().getConsecutivoCta(codCia, codDocum);

                xml.Append(getX2990708_MMX(consecutivo.Value, codDocum, codCia, codGestor, "", datos, parametros));
                xml.Append(getX2990709_MMX(consecutivo.Value, codDocum, codCia, "", tipGestor));
                // GESTOR DE COBRO

                // cierra header xml
                xml.Append("</XML>");
            }
            catch (Exception ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getXmlEmision: ", ref idError, ex);
                System.Diagnostics.Debug.WriteLine("ERROR XmlUtil.getXmlEmision() : " + idError + " - " + ex);
                throw new Exception("Error al generar XML " + ex.Message);
            }

            return xml.ToString();
        }

        public string getP2000030(Hashtable parametros, String codCia, String codRamo, String fechaHoy, String fechaVence, String tipGestor, String codGestor)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2000030\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"1\">");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<COD_AGT>5884</COD_AGT>");//mismo que millon
            //xml.Append("<COD_AGT>99998</COD_AGT>");//mismo que millon
            xml.Append("<PCT_AGT>100</PCT_AGT>");//mismo que millon
            xml.Append(string.Format("<TIP_GESTOR>{0}</TIP_GESTOR>", tipGestor));//mismo que millon //
            xml.Append(string.Format("<COD_GESTOR>{0}</COD_GESTOR>", codGestor));//mismo que millon
            xml.Append("<FEC_EFEC_POLIZA>").Append(parametros["fecha_inicio_poliza"] != null ? parametros["fecha_inicio_poliza"] : fechaHoy).Append("</FEC_EFEC_POLIZA>");//  <!--Fecha del dia en que se realiza la cotización--> 
            xml.Append("<FEC_VCTO_POLIZA>").Append(parametros["fecha_fin_poliza"] != null ? parametros["fecha_fin_poliza"] : fechaVence).Append("</FEC_VCTO_POLIZA>  ");// <!--Fecha del dia en que se realiza la cotización + 1 año-->
            xml.Append("<FEC_EFEC_SPTO>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EFEC_SPTO> ");//<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_VCTO_SPTO>").Append(parametros["fecha_fin_cotizacion"] != null ? parametros["fecha_fin_cotizacion"] : fechaVence).Append("</FEC_VCTO_SPTO>"); //<!--Fecha del dia en que se realiza la cotización + 1 año--> 
            xml.Append("<TIP_DOCUM>CLM</TIP_DOCUM>"); // <!--(mismo que millon vida)--> 
            xml.Append("<COD_DOCUM>PRUEBA</COD_DOCUM>"); // <!--(mismo que millon vida)-->
            xml.Append("<COD_CUADRO_COM>81</COD_CUADRO_COM>");//<!--(mismo que millon vida)--> 
            xml.Append("<MCA_IMPRESION>N</MCA_IMPRESION>"); // <!--(mismo que millon vida)--> 
            //xml.Append("<ANIOS_MAX_DURACION>").Append(parametros["anios_max"] != null ? parametros["anios_max"] : aniosMax).Append("</ANIOS_MAX_DURACION>"); // <!--(mismo que millon vida)-->
            xml.Append("<ANIOS_MAX_DURACION>1</ANIOS_MAX_DURACION>"); // <!--(mismo que millon vida)-->
            xml.Append("<COD_ENVIO>1</COD_ENVIO>"); // <!--(mismo que millon vida)--> 
            xml.Append("<COD_FRACC_PAGO>1</COD_FRACC_PAGO>"); //  <!-- Valor Combo periodicidad-->
            xml.Append("<COD_MON>1</COD_MON>"); // <!--(mismo que millon vida)--> 
            xml.Append("<TXT_MOTIVO_SPTO>C</TXT_MOTIVO_SPTO>"); // <!--(mismo que millon vida)--> 
            xml.Append("<COD_USR>APPSEGA</COD_USR>"); // <!--(mismo que millon vida)--> 
            xml.Append(string.Format("<NUM_POLIZA_GRUPO>{0}</NUM_POLIZA_GRUPO>", ConfigurationManager.AppSettings["NUM_POLIZA_GPO"] != null ? ConfigurationManager.AppSettings["NUM_POLIZA_GPO"].ToString() : ""));
            //xml.Append(string.Format("<NUM_CONTRATO>{0}</NUM_CONTRATO>", ConfigurationManager.AppSettings["NUM_CONTRATO"] != null ? ConfigurationManager.AppSettings["NUM_CONTRATO"].ToString() : ""));
            xml.Append(string.Format("<NUM_CONTRATO>{0}</NUM_CONTRATO>", GetContrato(parametros["prima"].ToString())));
            xml.Append("<TIP_REGULARIZA>0</TIP_REGULARIZA>");
            xml.Append("<PCT_REGULARIZA>0</PCT_REGULARIZA>");
            xml.Append("<COD_AGT2></COD_AGT2>");
            xml.Append("<PCT_AGT2></PCT_AGT2>");
            xml.Append("<COD_AGT3></COD_AGT3>");
            xml.Append("<PCT_AGT3></PCT_AGT3>");
            xml.Append("<COD_AGT4></COD_AGT4>");
            xml.Append("<PCT_AGT4></PCT_AGT4>");
            xml.Append("<TIP_DURACION>2</TIP_DURACION>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_SECTOR>1</COD_SECTOR>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_APLI>0</NUM_APLI>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>"); //<!--(mismo que millon vida)-->
            xml.Append("<FEC_EMISION>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EMISION>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_EMISION_SPTO>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EMISION_SPTO>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<NUM_RIESGOS>1</NUM_RIESGOS>"); //<!--(mismo que millon vida)-->
            xml.Append("<CANT_RENOVACIONES>0</CANT_RENOVACIONES>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_RENOVACIONES>0</NUM_RENOVACIONES>"); //<!--(mismo que millon vida)-->
            xml.Append("<TIP_COASEGURO>0</TIP_COASEGURO>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_SECU_GRUPO></NUM_SECU_GRUPO>"); //<!--(mismo que millon vida)-->
            xml.Append("<TIP_SPTO>XX</TIP_SPTO>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_REGULARIZA>S</MCA_REGULARIZA>"); //<!--(mismo que millon vida)-->
            xml.Append("<DURACION_PAGO_PRIMA>1</DURACION_PAGO_PRIMA>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_TOMADORES_ALT>N</MCA_TOMADORES_ALT>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_REASEGURO_MANUAL>N</MCA_REASEGURO_MANUAL>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_PRORRATA>S</MCA_PRORRATA>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_PRIMA_MANUAL>N</MCA_PRIMA_MANUAL>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_PROVISIONAL>N</MCA_PROVISIONAL>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_POLIZA_ANULADA>N</MCA_POLIZA_ANULADA>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_SPTO_ANULADO>N</MCA_SPTO_ANULADO>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_SPTO_TMP>N</MCA_SPTO_TMP>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_DATOS_MINIMOS>N</MCA_DATOS_MINIMOS>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_EXCLUSIVO>N</MCA_EXCLUSIVO>"); //<!--(mismo que millon vida)-->
            xml.Append("<FEC_VALIDEZ>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_VALIDEZ>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_ACTU>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_ACTU>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<MCA_REASEGURO_MARCO>N</MCA_REASEGURO_MARCO>"); //<!--(mismo que millon vida)-->
            xml.Append("<TIP_POLIZA_TR>F</TIP_POLIZA_TR>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_NIVEL1>").Append(parametros["cod_nivel1"] != null ? parametros["cod_nivel1"] : "").Append("</COD_NIVEL1>"); //
            xml.Append("<COD_NIVEL2>").Append(parametros["cod_nivel2"] != null ? parametros["cod_nivel2"] : "").Append("</COD_NIVEL2>"); //
            xml.Append("<COD_NIVEL3>").Append(parametros["cod_nivel3"] != null ? parametros["cod_nivel3"] : "").Append("</COD_NIVEL3>"); //
            xml.Append("<COD_NIVEL3_CAPTURA>").Append(parametros["cod_nivel3_captura"] != null ? parametros["cod_nivel3_captura"] : "4920").Append("</COD_NIVEL3_CAPTURA>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_COMPENSACION>1</COD_COMPENSACION>"); //<!--(mismo que millon vida)-->
            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }

        public string getP2000030Emision(Hashtable parametros, String codCia, String codRamo, String fechaHoy, String fechaVence, String codDocum, String tipGestor, String codGestor)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2000030\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"1\">");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<COD_AGT>").Append("5884").Append("</COD_AGT>");//mismo que millon
            xml.Append("<PCT_AGT>100</PCT_AGT>");//mismo que millon
            xml.Append(string.Format("<TIP_GESTOR>{0}</TIP_GESTOR>", tipGestor));//mismo que millon //
            xml.Append(string.Format("<COD_GESTOR>{0}</COD_GESTOR>", codGestor));//mismo que millon
            xml.Append("<FEC_EFEC_POLIZA>").Append(parametros["fecha_inicio_poliza"] != null ? parametros["fecha_inicio_poliza"] : fechaHoy).Append("</FEC_EFEC_POLIZA>");//  <!--Fecha del dia en que se realiza la cotización--> 
            xml.Append("<FEC_VCTO_POLIZA>").Append(parametros["fecha_fin_poliza"] != null ? parametros["fecha_fin_poliza"] : fechaVence).Append("</FEC_VCTO_POLIZA>  ");// <!--Fecha del dia en que se realiza la cotización + 1 año-->
            xml.Append("<FEC_EFEC_SPTO>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EFEC_SPTO> ");//<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_VCTO_SPTO>").Append(parametros["fecha_fin_cotizacion"] != null ? parametros["fecha_fin_cotizacion"] : fechaVence).Append("</FEC_VCTO_SPTO>"); //<!--Fecha del dia en que se realiza la cotización + 1 año--> 
            xml.Append("<TIP_DOCUM>CLM</TIP_DOCUM>"); // <!--(mismo que millon vida)--> 
            xml.Append("<COD_DOCUM>").Append(codDocum).Append("</COD_DOCUM>"); // <!--(mismo que millon vida)-->
            xml.Append("<COD_CUADRO_COM>81</COD_CUADRO_COM>");//<!--(mismo que millon vida)--> 
            xml.Append("<MCA_IMPRESION>N</MCA_IMPRESION>"); // <!--(mismo que millon vida)--> 
            //xml.Append("<ANIOS_MAX_DURACION>").Append(parametros["anios_max"] != null ? parametros["anios_max"] : aniosMax).Append("</ANIOS_MAX_DURACION>"); // <!--(mismo que millon vida)-->
            xml.Append("<ANIOS_MAX_DURACION>1</ANIOS_MAX_DURACION>"); // <!--(mismo que millon vida)-->
            xml.Append("<COD_ENVIO>1</COD_ENVIO>"); // <!--(mismo que millon vida)--> 
            xml.Append("<COD_FRACC_PAGO>1</COD_FRACC_PAGO>"); //  <!-- Valor Combo periodicidad-->
            xml.Append("<COD_MON>1</COD_MON>"); // <!--(mismo que millon vida)--> 
            xml.Append("<TXT_MOTIVO_SPTO>C</TXT_MOTIVO_SPTO>"); // <!--(mismo que millon vida)--> 
            xml.Append("<COD_USR>APPSEGA</COD_USR>"); // <!--(mismo que millon vida)--> 
            xml.Append(string.Format("<NUM_POLIZA_GRUPO>{0}</NUM_POLIZA_GRUPO>", ConfigurationManager.AppSettings["NUM_POLIZA_GPO"] != null ? ConfigurationManager.AppSettings["NUM_POLIZA_GPO"].ToString() : ""));
            //xml.Append(string.Format("<NUM_CONTRATO>{0}</NUM_CONTRATO>", ConfigurationManager.AppSettings["NUM_CONTRATO"] != null ? ConfigurationManager.AppSettings["NUM_CONTRATO"].ToString() : ""));
            xml.Append(string.Format("<NUM_CONTRATO>{0}</NUM_CONTRATO>", GetContrato(parametros["prima"].ToString())));
            xml.Append("<TIP_REGULARIZA>0</TIP_REGULARIZA>");
            xml.Append("<PCT_REGULARIZA>0</PCT_REGULARIZA>");
            xml.Append("<COD_AGT2></COD_AGT2>");
            xml.Append("<PCT_AGT2></PCT_AGT2>");
            xml.Append("<COD_AGT3></COD_AGT3>");
            xml.Append("<PCT_AGT3></PCT_AGT3>");
            xml.Append("<COD_AGT4></COD_AGT4>");
            xml.Append("<PCT_AGT4></PCT_AGT4>");
            xml.Append("<TIP_DURACION>2</TIP_DURACION>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_SECTOR>1</COD_SECTOR>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_APLI>0</NUM_APLI>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>"); //<!--(mismo que millon vida)-->
            xml.Append("<FEC_EMISION>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EMISION>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_EMISION_SPTO>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EMISION_SPTO>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<NUM_RIESGOS>1</NUM_RIESGOS>"); //<!--(mismo que millon vida)-->
            xml.Append("<CANT_RENOVACIONES>0</CANT_RENOVACIONES>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_RENOVACIONES>0</NUM_RENOVACIONES>"); //<!--(mismo que millon vida)-->
            xml.Append("<TIP_COASEGURO>0</TIP_COASEGURO>"); //<!--(mismo que millon vida)-->
            xml.Append("<NUM_SECU_GRUPO></NUM_SECU_GRUPO>"); //<!--(mismo que millon vida)-->
            xml.Append("<TIP_SPTO>XX</TIP_SPTO>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_REGULARIZA>S</MCA_REGULARIZA>"); //<!--(mismo que millon vida)-->
            xml.Append("<DURACION_PAGO_PRIMA>1</DURACION_PAGO_PRIMA>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_TOMADORES_ALT>N</MCA_TOMADORES_ALT>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_REASEGURO_MANUAL>N</MCA_REASEGURO_MANUAL>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_PRORRATA>S</MCA_PRORRATA>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_PRIMA_MANUAL>N</MCA_PRIMA_MANUAL>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_PROVISIONAL>N</MCA_PROVISIONAL>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_POLIZA_ANULADA>N</MCA_POLIZA_ANULADA>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_SPTO_ANULADO>N</MCA_SPTO_ANULADO>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_SPTO_TMP>N</MCA_SPTO_TMP>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_DATOS_MINIMOS>N</MCA_DATOS_MINIMOS>"); //<!--(mismo que millon vida)-->
            xml.Append("<MCA_EXCLUSIVO>N</MCA_EXCLUSIVO>"); //<!--(mismo que millon vida)-->
            xml.Append("<FEC_VALIDEZ>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_VALIDEZ>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_ACTU>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_ACTU>"); //<!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<MCA_REASEGURO_MARCO>N</MCA_REASEGURO_MARCO>"); //<!--(mismo que millon vida)-->
            xml.Append("<TIP_POLIZA_TR>F</TIP_POLIZA_TR>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_NIVEL1>").Append(parametros["cod_nivel1"] != null ? parametros["cod_nivel1"] : "").Append("</COD_NIVEL1>"); //
            xml.Append("<COD_NIVEL2>").Append(parametros["cod_nivel2"] != null ? parametros["cod_nivel2"] : "").Append("</COD_NIVEL2>"); //
            xml.Append("<COD_NIVEL3>").Append(parametros["cod_nivel3"] != null ? parametros["cod_nivel3"] : "").Append("</COD_NIVEL3>"); //
            xml.Append("<COD_NIVEL3_CAPTURA>").Append(parametros["cod_nivel3_captura"] != null ? parametros["cod_nivel3_captura"] : "4920").Append("</COD_NIVEL3_CAPTURA>"); //<!--(mismo que millon vida)-->
            xml.Append("<COD_COMPENSACION>1</COD_COMPENSACION>"); //<!--(mismo que millon vida)-->
            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }

        public string getP2000031(Hashtable parametros, String codCia, String fechaHoy, String fechaVence)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2000031\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"1\">");
            xml.Append("<NOM_RIESGO>").Append(parametros["nom_cliente"] != null ? parametros["nom_cliente"] : "RIESGO COTIZACION").Append("</NOM_RIESGO>");// <!--Nombre del Cliente HSBC-->
            xml.Append("<FEC_EFEC_RIESGO>").Append(parametros["fecha_inicio_cotizacion"] != null ? parametros["fecha_inicio_cotizacion"] : fechaHoy).Append("</FEC_EFEC_RIESGO>");// <!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<FEC_VCTO_RIESGO>").Append(parametros["fecha_fin_cotizacion"] != null ? parametros["fecha_fin_cotizacion"] : fechaVence).Append("</FEC_VCTO_RIESGO>");// <!--Fecha del dia en que se realiza la cotización-->
            xml.Append("<COD_MODALIDAD>").Append(parametros["cod_modalidad"] != null ? parametros["cod_modalidad"] : "11201").Append("</COD_MODALIDAD>");//<!--Ramo + 01-->
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");// <!--(mismo que millon vida)-->
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");//<!--(mismo que millon vida)-->
            xml.Append("<NUM_APLI>0</NUM_APLI>");//<!--(mismo que millon vida)-->
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");//<!--(mismo que millon vida)-->
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");//<!--(mismo que millon vida)-->
            xml.Append("<TIP_SPTO>XX</TIP_SPTO>");//<!--(mismo que millon vida)-->
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");//<!--(mismo que millon vida)-->
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");//<!--(mismo que millon vida)-->
            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");
            return xml.ToString();
        }

        public string getP2000020(Hashtable parametros, String codCia, String codRamo, int aniosMax, DatosContratante.Datos datos)
        {
            StringBuilder xml = new StringBuilder();

            xml.Append("<TABLE NAME=\"P2000020\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"2\">");
            xml.Append("<COD_CAMPO>ID_PROMOTOR</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["id_promotor"] != null ? parametros["id_promotor"] : "1234").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["id_promotor_cor"] != null ? parametros["id_promotor_cor"] : "0").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>2</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"3\">");
            xml.Append("<COD_CAMPO>TIPO_SEGURO</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["tipo_seguro"] != null ? parametros["tipo_seguro"] : "P").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["tipo_seguro_cor"] != null ? parametros["tipo_seguro_cor"] : "P").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>3</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"4\">");
            xml.Append("<COD_CAMPO>TIP_COMISION</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["tip_comision"] != null ? parametros["tip_comision"] : "1").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["tip_comision_cor"] != null ? parametros["tip_comision_cor"] : "1").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>4</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"5\">");
            xml.Append("<COD_CAMPO>TIP_IMPRESION</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["tip_impresion"] != null ? parametros["tip_impresion"] : "N").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["tip_impresion_cor"] != null ? parametros["tip_impresion_cor"] : "N").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>5</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            //xml.Append("<ROW num=\"6\">");
            //xml.Append("<COD_CAMPO>VAL_DURACION_SEGURO</COD_CAMPO>");
            //xml.Append("<VAL_CAMPO>").Append(parametros["anios_max"] != null ? parametros["anios_max"] : aniosMax).Append("</VAL_CAMPO>");
            //xml.Append("<VAL_COR_CAMPO>").Append(parametros["anios_max"] != null ? parametros["anios_max"] : aniosMax).Append("</VAL_COR_CAMPO>");
            //xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            //xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            //xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            //xml.Append("<NUM_SECU>6</NUM_SECU>");
            //xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            //xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            //xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            //xml.Append("<NUM_APLI>0</NUM_APLI>");
            //xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            //xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            //xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            //xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            //xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            //xml.Append("</ROW>");

            xml.Append("<ROW num=\"7\">");
            xml.Append("<COD_CAMPO>TIP_DEDUCIBLE</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["tip_deducible"] != null ? parametros["tip_deducible"] : "1").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["tip_deducible_cor"] != null ? parametros["tip_deducible_cor"] : "1").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>7</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"8\">");
            xml.Append("<COD_CAMPO>COD_MODALIDAD</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["cod_modalidad"] != null ? parametros["cod_modalidad"] : "11201").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["cod_modalidad"] != null ? parametros["cod_modalidad"] : "11201").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>1</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"9\">");
            xml.Append("<COD_CAMPO>VAL_FOLIO_SOLICITUD</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["val_folio"] != null ? parametros["val_folio"] : "1").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["val_folio_cor"] != null ? parametros["val_folio_cor"] : "1").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>0</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>3</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"10\">");
            xml.Append("<COD_CAMPO>FEC_NACIMIENTO</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["fecha_nacimiento_modif"] != null ? parametros["fecha_nacimiento_modif"] : "").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["fecha_nacimiento_modif"] != null ? parametros["fecha_nacimiento_modif"] : "").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : codRamo).Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>4</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"11\">");
            xml.Append("<COD_CAMPO>MCA_SEXO</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["sexo"] != null ? parametros["sexo"] : "0").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["sexo"] != null ? parametros["sexo"] : "0").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>5</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"12\">");
            xml.Append("<COD_CAMPO>DTO_AGENTE_DIR</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["dto_agente_dir"] != null ? parametros["dto_agente_dir"] : "99998").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["dto_agente_dir"] != null ? parametros["dto_agente_dir"] : "99998").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>3</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>1</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            //xml.Append("<ROW num=\"13\">");
            //xml.Append("<COD_CAMPO>IMP_PREMIO_VIDA</COD_CAMPO>");
            //xml.Append("<VAL_CAMPO>").Append(parametros["imp_premio_vida"] != null ? parametros["imp_premio_vida"] : "0").Append("</VAL_CAMPO>");
            //xml.Append("<VAL_COR_CAMPO>").Append(parametros["imp_premio_vida"] != null ? parametros["imp_premio_vida"] : "0").Append("</VAL_COR_CAMPO>");
            //xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            //xml.Append("<TIP_NIVEL>5</TIP_NIVEL>");
            //xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            //xml.Append("<NUM_SECU>2</NUM_SECU>");
            //xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            //xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            //xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            //xml.Append("<NUM_APLI>0</NUM_APLI>");
            //xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            //xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            //xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            //xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            //xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            //xml.Append("</ROW>");

            xml.Append("<ROW num=\"14\">");
            xml.Append("<COD_CAMPO>ANIOS_DURACION_POLIZA</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["anios_max"] != null ? parametros["anios_max"] : aniosMax).Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["anios_max"] != null ? parametros["anios_max"] : aniosMax).Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>1</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>6</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            //xml.Append("<ROW num=\"15\">");
            //xml.Append("<COD_CAMPO>FORMA_PAGO</COD_CAMPO>");
            //xml.Append("<VAL_CAMPO>").Append(parametros["forma_pago"] != null ? parametros["forma_pago"] : "1").Append("</VAL_CAMPO>");
            //xml.Append("<VAL_COR_CAMPO>").Append(parametros["forma_pago"] != null ? parametros["forma_pago"] : "1").Append("</VAL_COR_CAMPO>");
            //xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            //xml.Append("<TIP_NIVEL>5</TIP_NIVEL>");
            //xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            //xml.Append("<NUM_SECU>24</NUM_SECU>");
            //xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            //xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            //xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            //xml.Append("<NUM_APLI>0</NUM_APLI>");
            //xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            //xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            //xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            //xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            //xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            //xml.Append("</ROW>");

            //xml.Append("<ROW num=\"16\">");
            //xml.Append("<COD_CAMPO>TIP_PRIMA</COD_CAMPO>");
            //xml.Append("<VAL_CAMPO>").Append(parametros["tip_prima"] != null ? parametros["tip_prima"] : "UI").Append("</VAL_CAMPO>");
            //xml.Append("<VAL_COR_CAMPO>").Append(parametros["tip_prima"] != null ? parametros["tip_prima"] : "UI").Append("</VAL_COR_CAMPO>");
            //xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            //xml.Append("<TIP_NIVEL>5</TIP_NIVEL>");
            //xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            //xml.Append("<NUM_SECU>25</NUM_SECU>");
            //xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            //xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            //xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            //xml.Append("<NUM_APLI>0</NUM_APLI>");
            //xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            //xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            //xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            //xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            //xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            //xml.Append("</ROW>");

            //xml.Append("<ROW num=\"17\">");
            //xml.Append("<COD_CAMPO>IMP_PREMIO_VIDA_UI</COD_CAMPO>");
            //xml.Append("<VAL_CAMPO>").Append(parametros["imp_premio_vida_ui"] != null ? parametros["imp_premio_vida_ui"] : "0").Append("</VAL_CAMPO>");
            //xml.Append("<VAL_COR_CAMPO>").Append(parametros["imp_premio_vida_ui"] != null ? parametros["imp_premio_vida_ui"] : "0").Append("</VAL_COR_CAMPO>");
            //xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            //xml.Append("<TIP_NIVEL>5</TIP_NIVEL>");
            //xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            //xml.Append("<NUM_SECU>2</NUM_SECU>");
            //xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            //xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            //xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            //xml.Append("<NUM_APLI>0</NUM_APLI>");
            //xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            //xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            //xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            //xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            //xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            //xml.Append("</ROW>");

            xml.Append("<ROW num=\"18\">");
            xml.Append("<COD_CAMPO>TIP_PERFIL_INV</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["tip_perfil_inv"] != null ? parametros["tip_perfil_inv"] : "DEC").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["tip_perfil_inv"] != null ? parametros["tip_perfil_inv"] : "DEC").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>11</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"19\">");
            xml.Append("<COD_CAMPO>NUM_FUNDOS</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["num_fondos"] != null ? parametros["num_fondos"] : "3").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["num_fondos"] != null ? parametros["num_fondos"] : "3").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>22</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            //ccc
            xml.Append("<ROW num=\"20\">");
            xml.Append("<COD_CAMPO>TIP_PRIMAS</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["tip_primas"] != null ? parametros["tip_primas"] : "2").Append("</VAL_CAMPO>");
            xml.Append("<VAL_COR_CAMPO>").Append(parametros["tip_primas"] != null ? parametros["tip_primas"] : "2").Append("</VAL_COR_CAMPO>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<NUM_SECU>23</NUM_SECU>");
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            //Solo si es poliza inserta la ocupacion
            if (parametros["accion"].Equals("P"))
            {

                xml.Append("<ROW num=\"21\">");
                xml.Append("<COD_CAMPO>VAL_OCUPACION</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(datos.msgJson.ocupacionContratante).Append("</VAL_CAMPO>");
                xml.Append("<VAL_COR_CAMPO>").Append(datos.msgJson.ocupacionContratante).Append("</VAL_COR_CAMPO>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<TIP_NIVEL>2</TIP_NIVEL>");
                xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
                xml.Append("<NUM_SECU>23</NUM_SECU>");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("</ROW>");

            }



            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");
            return xml.ToString();
        }

        public string getP2000025(Hashtable parametros, String codCia)
        {
            DateTime fecHoy = DateTime.Now;
            string fecActual = fecHoy.ToString("ddMMyyyy");

            string fechaPeriodicidad = new CotizarDao().getFecAportacion(parametros["periodoText"].ToString().Trim(), (parametros["diaCobro"] != null ? parametros["diaCobro"].ToString().Trim() : ""));

            string montoAportaciones = parametros["imp_premio_vida_ad"].ToString();

            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2000025\">");
            xml.Append("<ROWSET>");

            int row = 1;
            int noOcurr = 1;

            if (parametros["pct001"] != null && parametros["pct001"].ToString() != "0")
            {
                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>1</NUM_SECU>");
                xml.Append("<COD_CAMPO>CODIGO_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>0001</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;

                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>2</NUM_SECU>");
                xml.Append("<COD_CAMPO>PERCENTAGEM_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["pct001"]).Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;
                noOcurr++;
            }

            if (parametros["pct002"] != null && parametros["pct002"].ToString() != "0")
            {
                xml.Append("<ROW num=\"3\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>1</NUM_SECU>");
                xml.Append("<COD_CAMPO>CODIGO_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>0002</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;

                xml.Append("<ROW num=\"4\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>2</NUM_SECU>");
                xml.Append("<COD_CAMPO>PERCENTAGEM_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["pct002"]).Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");
                
                row++;
                noOcurr++;
            }

            if (parametros["pct003"] != null && parametros["pct003"].ToString() != "0")
            {
                xml.Append("<ROW num=\"5\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>1</NUM_SECU>");
                xml.Append("<COD_CAMPO>CODIGO_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>0003</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;

                xml.Append("<ROW num=\"6\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>2</NUM_SECU>");
                xml.Append("<COD_CAMPO>PERCENTAGEM_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["pct003"]).Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;
                noOcurr++;
            }
            //HHAC ini
            if (parametros["pct004"] != null && parametros["pct004"].ToString() != "0")
            {
                xml.Append("<ROW num=\"7\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>1</NUM_SECU>");
                xml.Append("<COD_CAMPO>CODIGO_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>0004</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;

                xml.Append("<ROW num=\"8\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>2</NUM_SECU>");
                xml.Append("<COD_CAMPO>PERCENTAGEM_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["pct004"]).Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;
                noOcurr++;
            }
            if (parametros["pct005"] != null && parametros["pct005"].ToString() != "0")
            {
                xml.Append("<ROW num=\"9\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>1</NUM_SECU>");
                xml.Append("<COD_CAMPO>CODIGO_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>0005</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;

                xml.Append("<ROW num=\"10\">");
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>600</COD_LISTA>");
                xml.Append(string.Format("<NUM_OCURRENCIA>{0}</NUM_OCURRENCIA>", noOcurr));
                xml.Append("<NUM_SECU>2</NUM_SECU>");
                xml.Append("<COD_CAMPO>PERCENTAGEM_FUNDO</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["pct005"]).Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");

                row++;
                noOcurr++;
            }
            //HHAC fin
            xml.Append(string.Format("<ROW num=\"{0}\">", row));
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<COD_LISTA>601</COD_LISTA>");
            xml.Append("<NUM_OCURRENCIA>1</NUM_OCURRENCIA>");
            xml.Append("<NUM_SECU>1</NUM_SECU>");
            xml.Append("<COD_CAMPO>TIP_PRIMA</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>UI</VAL_CAMPO>");
            xml.Append("<TXT_CAMPO>UNICA INICIAL</TXT_CAMPO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
            xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
            xml.Append("</ROW>");

            row++;

            xml.Append(string.Format("<ROW num=\"{0}\">", row));
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<COD_LISTA>601</COD_LISTA>");
            xml.Append("<NUM_OCURRENCIA>1</NUM_OCURRENCIA>");
            xml.Append("<NUM_SECU>2</NUM_SECU>");
            xml.Append("<COD_CAMPO>FORMA_PAGO</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>1</VAL_CAMPO>");
            xml.Append("<TXT_CAMPO>CONTADO</TXT_CAMPO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
            xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
            xml.Append("</ROW>");

            row++;

            xml.Append(string.Format("<ROW num=\"{0}\">", row));
            xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<COD_LISTA>601</COD_LISTA>");
            xml.Append("<NUM_OCURRENCIA>1</NUM_OCURRENCIA>");
            xml.Append("<NUM_SECU>4</NUM_SECU>");
            xml.Append("<COD_CAMPO>IMP_PREMIO_VIDA</COD_CAMPO>");
            xml.Append("<VAL_CAMPO>").Append(parametros["imp_premio_vida"] != null ? parametros["imp_premio_vida"] : "0").Append("</VAL_CAMPO>");
            xml.Append("<TXT_CAMPO></TXT_CAMPO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
            xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
            xml.Append("</ROW>");

            //ccc
            if (!(montoAportaciones.Equals("0")))
            {
                row++;

                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>601</COD_LISTA>");
                xml.Append("<NUM_OCURRENCIA>2</NUM_OCURRENCIA>");
                xml.Append("<NUM_SECU>1</NUM_SECU>");
                xml.Append("<COD_CAMPO>TIP_PRIMA</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>AD</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO>ADICIONALES</TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");
            }

            //ccc
            if (!(montoAportaciones.Equals("0")))
            {
                row++;

                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>601</COD_LISTA>");
                xml.Append("<NUM_OCURRENCIA>2</NUM_OCURRENCIA>");
                xml.Append("<NUM_SECU>3</NUM_SECU>");
                xml.Append("<COD_CAMPO>FORMA_PAGO_AD</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["periodicidad"]).Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO>").Append(parametros["periodicidad"]).Append("</TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");
            }

            if (!(montoAportaciones.Equals("0")))
            {
                //ccc
                row++;

                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>601</COD_LISTA>");
                xml.Append("<NUM_OCURRENCIA>2</NUM_OCURRENCIA>");
                xml.Append("<NUM_SECU>5</NUM_SECU>");
                xml.Append("<COD_CAMPO>IMP_PREMIO_VIDA_AD</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["imp_premio_vida_ad"] != null ? parametros["imp_premio_vida_ad"] : "0").Append("</VAL_CAMPO>");
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");
            }

            if (!(montoAportaciones.Equals("0")))
            {
                //ccc
                row++;

                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>601</COD_LISTA>");
                xml.Append("<NUM_OCURRENCIA>2</NUM_OCURRENCIA>");
                xml.Append("<NUM_SECU>9</NUM_SECU>");
                xml.Append("<COD_CAMPO>FEC_EFEC_PRIMA_ADI</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(parametros["diaCobro"]).Append("</VAL_CAMPO>"); //DIA SUGERIDO DE COBRO --CCC--
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");
            }

            if (!(montoAportaciones.Equals("0")))
            {
                //ccc
                row++;

                xml.Append(string.Format("<ROW num=\"{0}\">", row));
                xml.Append("<COD_CIA>").Append(codCia).Append("</COD_CIA>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
                xml.Append("<COD_LISTA>601</COD_LISTA>");
                xml.Append("<NUM_OCURRENCIA>2</NUM_OCURRENCIA>");
                xml.Append("<NUM_SECU>7</NUM_SECU>");
                xml.Append("<COD_CAMPO>FEC_EFEC_PRIMA_AD</COD_CAMPO>");
                xml.Append("<VAL_CAMPO>").Append(fechaPeriodicidad).Append("</VAL_CAMPO>"); //DIA SUGERIDO DE COBRO --CCC--
                xml.Append("<TXT_CAMPO></TXT_CAMPO>");
                xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
                xml.Append("<MCA_BAJA_OCURRENCIA>N</MCA_BAJA_OCURRENCIA>");
                xml.Append("<IMP_OCURRENCIA>0</IMP_OCURRENCIA>");
                xml.Append("</ROW>");
            }

            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }

        public string get2000060(Hashtable parametros, String codDocumEmision)
        {

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("------------------- ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("get2000060: ", null);


            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2000060\">");
            xml.Append("<ROWSET>");
            DatosContratante.Datos datosContratante= null;
            if (parametros["accion"].Equals("P"))
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("ACCION P: ", null);

                datosContratante = (DatosContratante.Datos)parametros["datosContratante"];

                xml.Append("<ROW num=\"").Append(1).Append("\">");
                xml.Append("<TIP_BENEF>").Append(2).Append("</TIP_BENEF>");
                xml.Append("<NUM_SECU>").Append(1).Append("</NUM_SECU>");
                xml.Append("<TIP_DOCUM>").Append("CLM").Append("</TIP_DOCUM>");
                if (parametros["accion"].Equals("C"))
                {
                    xml.Append("<COD_DOCUM>").Append("PRUEBA").Append("</COD_DOCUM>");
                }else
                {
                    xml.Append("<COD_DOCUM>").Append(codDocumEmision).Append("</COD_DOCUM>");
                }
                xml.Append("<PCT_PARTICIPACION></PCT_PARTICIPACION>");
                xml.Append("<FEC_NACIMIENTO></FEC_NACIMIENTO>");
                xml.Append("<NUM_POLIZA></NUM_POLIZA>");
                xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<MCA_PRINCIPAL>N</MCA_PRINCIPAL>");
                xml.Append("<MCA_CALCULO>N</MCA_CALCULO>");
                xml.Append("<MCA_BAJA>N</MCA_BAJA>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("</ROW>");

                if (datosContratante != null && datosContratante.msgJson != null && datosContratante.msgJson.datosBeneficiarios != null)
                {
                    int row = 2;
                    foreach (DatosContratante.DatosBeneficiario ben in datosContratante.msgJson.datosBeneficiarios)
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("BENEFICIARIO: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(row.ToString() , null);
                        
                        String tipBenef = "6";
                        String tipDocum = "CLM";
                        String codDocum = "PRUEBA";
                        Hashtable retBenef = new CotizarDao().AltaBeneficiario(ben);
                        tipDocum = retBenef["tip_docum"].ToString();
                        codDocum = retBenef["cod_docum"].ToString();
                        xml.Append("<ROW num=\"").Append(row).Append("\">");
                        xml.Append("<TIP_BENEF>").Append(tipBenef).Append("</TIP_BENEF>");
                        xml.Append("<NUM_SECU>").Append(row).Append("</NUM_SECU>");
                        xml.Append("<TIP_DOCUM>").Append(tipDocum).Append("</TIP_DOCUM>");
                        xml.Append("<COD_DOCUM>").Append(codDocum).Append("</COD_DOCUM>");
                        xml.Append("<PCT_PARTICIPACION>").Append(ben.porcentaje).Append("</PCT_PARTICIPACION>");
                        xml.Append("<FEC_NACIMIENTO></FEC_NACIMIENTO>");
                        xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                        xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
                        xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                        xml.Append("<NUM_APLI>0</NUM_APLI>");
                        xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                        xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                        xml.Append("<MCA_PRINCIPAL>N</MCA_PRINCIPAL>");
                        xml.Append("<MCA_CALCULO>N</MCA_CALCULO>");
                        xml.Append("<MCA_BAJA>N</MCA_BAJA>");
                        xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                        xml.Append("</ROW>");
                        row++;
                    }
                }                
            }
            else
            {
                xml.Append("<ROW num=\"1\">");
                xml.Append("<TIP_BENEF>").Append(parametros["P2000060_tip_beneficiario"] != null ? parametros["P2000060_tip_beneficiario"] : "2").Append("</TIP_BENEF>");
                xml.Append("<NUM_SECU>").Append(parametros["P2000060_num_secu"] != null ? parametros["P2000060_num_secu"] : "1").Append("</NUM_SECU>");
                xml.Append("<TIP_DOCUM>CLM</TIP_DOCUM>");
                xml.Append("<COD_DOCUM>PRUEBA</COD_DOCUM>");
                xml.Append("<PCT_PARTICIPACION/>");
                xml.Append("<FEC_NACIMIENTO></FEC_NACIMIENTO>");
                xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
                xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
                xml.Append("<NUM_SPTO>0</NUM_SPTO>");
                xml.Append("<NUM_APLI>0</NUM_APLI>");
                xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
                xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
                xml.Append("<MCA_PRINCIPAL>N</MCA_PRINCIPAL>");
                xml.Append("<MCA_CALCULO>N</MCA_CALCULO>");
                xml.Append("<MCA_BAJA>N</MCA_BAJA>");
                xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
                xml.Append("</ROW>");
            }
            
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("FIN BENEFICIARIO: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("------------------ ", null);

            return xml.ToString();
        }

        public string get2000040(Hashtable parametros)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2000040\">");
            xml.Append("<ROWSET>");

            xml.Append("<ROW num=\"1\">");
            xml.Append("<COD_COB>").Append(parametros["cod_cob"] != null ? parametros["cod_cob"] : "1000").Append("</COD_COB>");
            xml.Append("<SUMA_ASEG>").Append(parametros["suma_asegurada1"] != null ? parametros["suma_asegurada1"] : "0.01").Append("</SUMA_ASEG>");
            xml.Append("<SUMA_ASEG_SPTO>").Append(parametros["suma_asegurada1"] != null ? parametros["suma_asegurada1"] : "0.01").Append("</SUMA_ASEG_SPTO>");
            xml.Append("<NUM_SECU>1</NUM_SECU>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<MCA_BAJA_COB>N</MCA_BAJA_COB>");
            xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<COD_FRANQUICIA/>");
            xml.Append("<COD_MON_CAPITAL>1</COD_MON_CAPITAL>");
            xml.Append("<TASA_COB>0</TASA_COB>");
            xml.Append("<IMP_AGR_SPTO>0</IMP_AGR_SPTO>");
            xml.Append("<IMP_AGR_REL_SPTO>0</IMP_AGR_REL_SPTO>");
            xml.Append("<COD_SECC_REAS>").Append(parametros["accion"].Equals("C") ? "13" : "0").Append("</COD_SECC_REAS>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"2\">");
            xml.Append("<COD_COB>").Append(parametros["cod_cob2"] != null ? parametros["cod_cob2"] : "1004").Append("</COD_COB>");
            xml.Append("<SUMA_ASEG>").Append(parametros["suma_asegurada2"] != null ? parametros["suma_asegurada2"] : "0.01").Append("</SUMA_ASEG>");
            xml.Append("<SUMA_ASEG_SPTO>").Append(parametros["suma_asegurada2"] != null ? parametros["suma_asegurada2"] : "0.01").Append("</SUMA_ASEG_SPTO>");
            xml.Append("<NUM_SECU>2</NUM_SECU>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<MCA_BAJA_COB>N</MCA_BAJA_COB>");
            xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<COD_FRANQUICIA/>");
            xml.Append("<COD_MON_CAPITAL>1</COD_MON_CAPITAL>");
            xml.Append("<TASA_COB>0</TASA_COB>");
            xml.Append("<IMP_AGR_SPTO>0</IMP_AGR_SPTO>");
            xml.Append("<IMP_AGR_REL_SPTO>0</IMP_AGR_REL_SPTO>");
            xml.Append("<COD_SECC_REAS>").Append(parametros["accion"].Equals("C") ? "13" : "0").Append("</COD_SECC_REAS>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("<ROW num=\"3\">");
            xml.Append("<COD_COB>").Append(parametros["cod_cob3"] != null ? parametros["cod_cob3"] : "1042").Append("</COD_COB>");
            xml.Append("<SUMA_ASEG>").Append(parametros["suma_asegurada3"] != null ? parametros["suma_asegurada3"] : "0").Append("</SUMA_ASEG>");
            xml.Append("<SUMA_ASEG_SPTO>").Append(parametros["suma_asegurada3"] != null ? parametros["suma_asegurada3"] : "0").Append("</SUMA_ASEG_SPTO>");
            xml.Append("<NUM_SECU>3</NUM_SECU>");
            xml.Append("<COD_RAMO>").Append(parametros["cod_ramo"] != null ? parametros["cod_ramo"] : "112").Append("</COD_RAMO>");
            xml.Append("<MCA_BAJA_COB>N</MCA_BAJA_COB>");
            xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<COD_FRANQUICIA/>");
            xml.Append("<COD_MON_CAPITAL>1</COD_MON_CAPITAL>");
            xml.Append("<TASA_COB>0</TASA_COB>");
            xml.Append("<IMP_AGR_SPTO>0</IMP_AGR_SPTO>");
            xml.Append("<IMP_AGR_REL_SPTO>0</IMP_AGR_REL_SPTO>");
            xml.Append("<COD_SECC_REAS>").Append(parametros["accion"].Equals("C") ? "13" : "0").Append("</COD_SECC_REAS>");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_PERIODO>1</NUM_PERIODO>");
            xml.Append("<MCA_BAJA_RIESGO>N</MCA_BAJA_RIESGO>");
            xml.Append("<MCA_VIGENTE>S</MCA_VIGENTE>");
            xml.Append("<MCA_VIGENTE_APLI>S</MCA_VIGENTE_APLI>");
            xml.Append("</ROW>");

            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");
            return xml.ToString();
        }

        public string getP2300060_MMX(Hashtable parametros)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P2300060_MMX\">");
            xml.Append("<ROWSET>");

            xml.Append("<ROW num=\"1\">");
            xml.Append("<NUM_RIESGO>1</NUM_RIESGO>");
            xml.Append("<NUM_POLIZA>").Append(parametros["num_poliza"] != null ? parametros["num_poliza"] : "").Append("</NUM_POLIZA>");
            xml.Append("<NUM_SPTO>0</NUM_SPTO>");
            xml.Append("<COD_CIA>").Append(parametros["cod_cia"] != null ? parametros["cod_cia"] : "1").Append("</COD_CIA>");
            xml.Append("<NUM_APLI>0</NUM_APLI>");
            xml.Append("<NUM_SECU>1</NUM_SECU>");
            xml.Append("<FEC_NACIMIENTO>").Append(parametros["fecha_nacimiento"] != null ? parametros["fecha_nacimiento"] : "").Append("</FEC_NACIMIENTO>");
            xml.Append("<NUM_SPTO_APLI>0</NUM_SPTO_APLI>");
            xml.Append("<MCA_FUMA>").Append(parametros["fumar"] != null ? parametros["fumar"] : "0").Append("</MCA_FUMA>");
            xml.Append("<MCA_SEXO>").Append(parametros["sexo"] != null ? parametros["sexo"] : "0").Append("</MCA_SEXO>");
            xml.Append("<TIP_BENEF>").Append(parametros["tip_beneficiario"] != null ? parametros["tip_beneficiario"] : "1").Append("</TIP_BENEF>");
            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }

        public string getP1001331(DatosContratante.Datos datos, string codDocum, String fechaHoy, Hashtable parametros)
        {
            DataRow dr = new CotizarDao().validaCP(datos.msgJson.codigoPostal, datos.msgJson.paisResidenciaFiscalContratante);
            
            string codEstado = dr.ItemArray[0].ToString();
            string codProv = dr.ItemArray[1].ToString();

            string sexo = "";
            string[] json;

            AltaCotizacion datosAlta = new CotizarDao().getCotizacion(1, 112, datos.msgJson.idCotizacionMapfre);

            json = datosAlta.msgJson.Split(',');

            sexo = json[5].Split(':')[1].Replace("\"","");

            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"P1001331\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"1\">");

            xml.Append("<COD_NACIONALIDAD>").Append(datos.msgJson.idNacionalidadMapfre).Append("</COD_NACIONALIDAD>"); 
            xml.Append("<COD_PROFESION>").Append(datos.msgJson.ocupacionContratante).Append("</COD_PROFESION>");
            xml.Append("<NOM_CONTACTO></NOM_CONTACTO>"); 
            xml.Append("<COD_IDENTIFICADOR>9998</COD_IDENTIFICADOR>");
            xml.Append("<COD_ESTADO>").Append(codEstado).Append("</COD_ESTADO>"); //Cambiar valida_cp
            xml.Append("<TXT_AUX3>").Append(datos.msgJson.curp).Append("</TXT_AUX3>");
            xml.Append("<NOM_TERCERO>").Append(datos.msgJson.nombreDeContratante != null ? datos.msgJson.nombreDeContratante.ToUpper() : "").Append("</NOM_TERCERO>");
            xml.Append("<TIP_ACT_ECONOMICA></TIP_ACT_ECONOMICA>");
            xml.Append("<TIP_CARGO></TIP_CARGO>");
            xml.Append("<COD_POSTAL>").Append(datos.msgJson.codigoPostal).Append("</COD_POSTAL>");
            xml.Append("<APE2_TERCERO>").Append(datos.msgJson.apellidoMaternoContrante != null ? datos.msgJson.apellidoMaternoContrante.ToUpper() : "").Append("</APE2_TERCERO>");
            xml.Append("<COD_PROV>").Append(codProv).Append("</COD_PROV>");//CAMBIAR valida_cp
            xml.Append("<COD_LOCALIDAD>").Append(codProv).Append("</COD_LOCALIDAD>");//CAMBIAR ----
            xml.Append("<NOM_LOCALIDAD>").Append("").Append("</NOM_LOCALIDAD>");//CAMBIAR----------DELEGACION 
            xml.Append("<MCA_FISICO>").Append("S").Append("</MCA_FISICO>");
            xml.Append("<NOM_DOMICILIO1>").Append(datos.msgJson.domicilioContratanteCalle != null ? datos.msgJson.domicilioContratanteCalle.ToUpper() : "").Append("</NOM_DOMICILIO1>");
            xml.Append("<TLF_MOVIL>").Append(datos.msgJson.telfonoCelular).Append("</TLF_MOVIL>");
            xml.Append("<COD_DOCUM>").Append(codDocum).Append("</COD_DOCUM>");
            xml.Append("<EMAIL>").Append(parametros["correoElectronico"]).Append("</EMAIL>");
            xml.Append("<CTA_CTE>").Append(datos.msgJson.cuentaClabeContratante).Append("</CTA_CTE>");
            xml.Append("<APE1_TERCERO>").Append(datos.msgJson.apellidoPaternoContrante != null ?datos.msgJson.apellidoPaternoContrante.ToUpper() : "").Append("</APE1_TERCERO>");
            xml.Append("<TIP_DOCUM>").Append("CLM").Append("</TIP_DOCUM>");

            if (sexo.Equals("M"))
            {
                xml.Append("<MCA_SEXO>").Append("1").Append("</MCA_SEXO>");
            }
            else
            {
                xml.Append("<MCA_SEXO>").Append("0").Append("</MCA_SEXO>");
            }

            xml.Append("<FEC_TRATAMIENTO>").Append(fechaHoy).Append("</FEC_TRATAMIENTO>");
            xml.Append("<TLF_NUMERO>").Append(datos.msgJson.telefono).Append("</TLF_NUMERO>");
            xml.Append("<FEC_NACIMIENTO>").Append(parametros["fecha_nacimiento"] != null ? parametros["fecha_nacimiento"] : "").Append("</FEC_NACIMIENTO>");
            xml.Append("<NOM_DOMICILIO2_COM>").Append(datos.msgJson.domicilioContratanteColonia.ToUpper()).Append("</NOM_DOMICILIO2_COM>");
            xml.Append("<NOM_DOMICILIO3>").Append(datos.msgJson.domicilioContratanteColonia.ToUpper()).Append("</NOM_DOMICILIO3>"); //NOM_DOMICILIO3 --> Colonia
            xml.Append("<COD_OCUPACION>").Append("4").Append("</COD_OCUPACION>"); // 4 Titular
            
            //if(datos.msgJson.estadoCivilContratante.Equals("001")){
            //    xml.Append("<COD_EST_CIVIL>").Append("C").Append("</COD_EST_CIVIL>");
            //}else if(datos.msgJson.estadoCivilContratante.Equals("002")){
            //    xml.Append("<COD_EST_CIVIL>").Append("S").Append("</COD_EST_CIVIL>");
            //}else if(datos.msgJson.estadoCivilContratante.Equals("003")){
            //    xml.Append("<COD_EST_CIVIL>").Append("V").Append("</COD_EST_CIVIL>");
            //}
            xml.Append("<COD_EST_CIVIL>").Append(datos.msgJson.estadoCivilContratante).Append("</COD_EST_CIVIL>");
            
            xml.Append("<COD_PAIS>").Append(datos.msgJson.paisResidenciaFiscalContratante).Append("</COD_PAIS>");
            xml.Append("<COD_CIA>").Append("1").Append("</COD_CIA>");
            xml.Append("<TIP_MVTO_BATCH>").Append("3").Append("</TIP_MVTO_BATCH>");
            xml.Append("<COD_ACT_TERCERO>").Append("1").Append("</COD_ACT_TERCERO>");//CAMBIAR
            xml.Append("<TXT_AUX1></TXT_AUX1>");
            xml.Append("<TXT_AUX2>").Append(datos.msgJson.rfc.ToUpper()).Append("</TXT_AUX2>"); 
            xml.Append("<COD_IDIOMA>").Append("ES").Append("</COD_IDIOMA>");

            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }

        public string getX2990708_MMX(int consecutivo, String codDocum, String codCia, String codEntidad, String poliza, DatosContratante.Datos datos, Hashtable parametros)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"X2990708_MMX\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"1\">");
            xml.Append(string.Format("<TIP_DOCUM>{0}</TIP_DOCUM>", "CLM"));
            xml.Append(string.Format("<COD_DOCUM>{0}</COD_DOCUM>", codDocum));
            xml.Append(string.Format("<TIP_MOVIMIENTO>{0}</TIP_MOVIMIENTO>", "A"));
            xml.Append(string.Format("<COD_IDENTIFICADOR>{0}</COD_IDENTIFICADOR>", "9998"));
            xml.Append(string.Format("<NUM_CONSEC_CUENTA>{0}</NUM_CONSEC_CUENTA>", consecutivo));
            xml.Append(string.Format("<COD_CIA>{0}</COD_CIA>", codCia));
            xml.Append(string.Format("<FEC_TRATAMIENTO>{0}</FEC_TRATAMIENTO>", DateTime.Now.ToString("dd/MM/yyyy")));
            xml.Append(string.Format("<COD_ENTIDAD>{0}</COD_ENTIDAD>", codEntidad));
            xml.Append(string.Format("<FEC_VALIDEZ>{0}</FEC_VALIDEZ>", DateTime.Now.ToString("dd/MM/yyyy")));
            xml.Append(string.Format("<NUM_POLIZA>{0}</NUM_POLIZA>", poliza));
            xml.Append(string.Format("<NUM_SPTO>{0}</NUM_SPTO>", 0));
            xml.Append(string.Format("<MCA_INH>{0}</MCA_INH>", "N"));
            xml.Append(string.Format("<NOM_TITULAR>{0}</NOM_TITULAR>", datos.msgJson.nombreDeContratante));
            xml.Append(string.Format("<APE1_TITULAR>{0}</APE1_TITULAR>", datos.msgJson.apellidoPaternoContrante));
            xml.Append(string.Format("<APE2_TITULAR>{0}</APE2_TITULAR>", datos.msgJson.apellidoMaternoContrante));
            xml.Append(string.Format("<MES_VCTO_TARJETA>{0}</MES_VCTO_TARJETA>", ""));
            xml.Append(string.Format("<ANO_VCTO_TARJETA>{0}</ANO_VCTO_TARJETA>", ""));
            xml.Append(string.Format("<NUM_CUENTA>{0}</NUM_CUENTA>", datos.msgJson.cuentaClabeContratante));
            xml.Append(string.Format("<TIP_CUENTA>{0}</TIP_CUENTA>", "D"));
            xml.Append(string.Format("<EMAIL>{0}</EMAIL>", parametros["correoElectronico"]));

            int dia = 0;

            if (parametros["diaCobro"] != null)
                Int32.TryParse(parametros["diaCobro"].ToString(), out dia);

            xml.Append(string.Format("<DIA_APLICACION_COBRO>{0}</DIA_APLICACION_COBRO>", dia != 0 ? dia.ToString() : ""));
            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }

        public string getX2990709_MMX(int consecutivo, String codDocum, String codCia, String poliza, String tipGestor)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TABLE NAME=\"X2990709_MMX\">");
            xml.Append("<ROWSET>");
            xml.Append("<ROW num=\"1\">");
            xml.Append(string.Format("<TIP_DOCUM>{0}</TIP_DOCUM>", "CLM"));
            xml.Append(string.Format("<COD_DOCUM>{0}</COD_DOCUM>", codDocum));
            xml.Append(string.Format("<COD_IDENTIFICADOR>{0}</COD_IDENTIFICADOR>", "9998"));
            xml.Append(string.Format("<NUM_POLIZA>{0}</NUM_POLIZA>", poliza));
            xml.Append(string.Format("<NUM_SPTO>{0}</NUM_SPTO>", 0));
            xml.Append(string.Format("<TIP_GESTOR>{0}</TIP_GESTOR>", tipGestor));
            //xml.Append(string.Format("<COD_IDENTIFICADOR>{0}</COD_IDENTIFICADOR>", "HSBC"));
            xml.Append(string.Format("<NUM_CONSEC_CUENTA>{0}</NUM_CONSEC_CUENTA>", consecutivo));
            xml.Append(string.Format("<COD_CIA>{0}</COD_CIA>", codCia));
            xml.Append(string.Format("<FEC_TRATAMIENTO>{0}</FEC_TRATAMIENTO>", DateTime.Now.ToString("dd/MM/yyyy")));
            xml.Append("</ROW>");
            xml.Append("</ROWSET>");
            xml.Append("</TABLE>");

            return xml.ToString();
        }
        protected static string GetContrato(string Prima_ini)
        {
            String contrato = "";            
            String[] Rangos_11202 = ConfigurationManager.AppSettings["Rango_11202"].ToString().Split(',');
            String[] Rangos_11215 = ConfigurationManager.AppSettings["Rango_11215"].ToString().Split(',');

            if (Convert.ToInt32(Rangos_11202[0]) < Convert.ToInt32(Prima_ini) && Convert.ToInt32(Prima_ini) < Convert.ToInt32(Rangos_11202[1]))
                    contrato =ConfigurationManager.AppSettings["NUM_CONTRATO"].ToString();

            if (Convert.ToInt32(Rangos_11215[0]) < Convert.ToInt32(Prima_ini) && Convert.ToInt32(Prima_ini) < Convert.ToInt32(Rangos_11215[1]))
                    contrato =ConfigurationManager.AppSettings["Num_contrato_r2"].ToString();

            if(Convert.ToInt32(Prima_ini)> Convert.ToInt32(Rangos_11215[1]))
                contrato = ConfigurationManager.AppSettings["Num_contrato_r3"].ToString();

            return contrato;
        }
    }
}