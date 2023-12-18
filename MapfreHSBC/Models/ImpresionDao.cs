using MapfreHSBC.Models.Cotizacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MapfreHSBC.Models.Cotizacion;
using MapfreWebCore.Oracle;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Xml;
using System.Net;
using System.Configuration;
using System.IO;

namespace MapfreHSBC.Models
{
    public class ImpresionDao
    {
        public string sFileOutput;
        string idError;

        public IEnumerable<ComboBase> getDocumentos()
        {
            List<ComboBase> documentos = new List<ComboBase>();
            documentos.Add(new ComboBase() { etiqueta = "Póliza Cartula", valor = "1"});
            documentos.Add(new ComboBase() { etiqueta = "Factura", valor = "2" });
            documentos.Add(new ComboBase() { etiqueta = "Anexos", valor = "3" });
            documentos.Add(new ComboBase() { etiqueta = "Constancia de Pago", valor ="4"});
            documentos.Add(new ComboBase() { etiqueta = "Contrato PPR", valor = "5" });
            documentos.Add(new ComboBase() { etiqueta = "Solicitud", valor = "6" });
            documentos.Add(new ComboBase() { etiqueta = "Formato Autorizacion Cargo Bancario", valor = "7" });
            documentos.Add(new ComboBase() { etiqueta = "Condiciones Generales", valor = "8" });
            documentos.Add(new ComboBase() { etiqueta = "Formato FATCA", valor = "9" });
            documentos.Add(new ComboBase() { etiqueta = "Formato CRS", valor = "10" });
            documentos.Add(new ComboBase() { etiqueta = "Formato Articulo 492(Umbral) KYC", valor = "11" });
            //HHAC ini
            documentos.Add(new ComboBase() { etiqueta = "Folleto Informativo", valor="12"});
            //HHAC fin
            return documentos;
        }

        public IEnumerable<ChkBase> getDocuments(string sModalidad, string sNacionalidad){

            List<ChkBase> obj = new List<ChkBase>();

            obj.Add(new ChkBase { text = "Póliza Carátula", value = 1, isChecked = false, id = "P" });
            //obj.Add(new ChkBase { text = "Factura", value = 2, isChecked = false, id = "FACTURA" });
            //obj.Add(new ChkBase { text = "Anexos", value = 3, isChecked = false, id = "Triptico_beneficios_vida" });

            if (sModalidad != null && sModalidad.Equals("11203")) //solo para la modalidad  UNIT LINKED PPR 
            {
                obj.Add(new ChkBase { text = "Constancia de Pago", value = 4, isChecked = false, id = "CP" });
                obj.Add(new ChkBase { text = "Contrato PPR", value = 5, isChecked = false, id = "PPR" });
            }

            obj.Add(new ChkBase { text = "Solicitud", value = 6, isChecked = false, id = "ST" });
            //obj.Add(new ChkBase { text = "Formato Autorización Cargo Bancario", value = 7, isChecked = false, id = "FORMATO_AUTORIZACION_CARGO_BANCARIO" });
            obj.Add(new ChkBase { text = "Derechos del Asegurado", value = 8, isChecked = false, id = "TR_beneficios_vida " });
            //
            if (sNacionalidad != null)
            {
                if (sNacionalidad.Equals("USA") || sNacionalidad.Equals("PTO") || sNacionalidad.Equals("ASM") || sNacionalidad.Equals("GUM") ||
                        sNacionalidad.Equals("IVE") || sNacionalidad.Equals("MNP"))
                {
                    obj.Add(new ChkBase { text = "Formato FATCA", value = 9, isChecked = false, id = "CF" });
                }
            }
            //
                if (!(sNacionalidad.Equals("USA") || sNacionalidad.Equals("PTO") || sNacionalidad.Equals("ASM") || sNacionalidad.Equals("GUM") || 
                        sNacionalidad.Equals("IVE") || sNacionalidad.Equals("MNP") || sNacionalidad.Equals("MEX")))
            {
                obj.Add(new ChkBase { text = "Formato CRS", value = 10, isChecked = false, id = "CRS" });
            }
            
            
            obj.Add(new ChkBase { text = "Formato Articulo 492(Umbral) KYC", value = 11, isChecked = false, id = "KYC" });

            //HHAC ini
            obj.Add(new ChkBase { text="Folleto Informativo",value=12,isChecked=false,id="FI" });
            //HHAC fin
            return obj;

        }

        public ImpresionPoliza getNoPoliza()
        {
            ImpresionPoliza poliza = new ImpresionPoliza(){
                noPoliza = "1234567890123456"
            };

            return poliza;
        }

        public byte[] CreaPDFHPExstream(string codCia, string noPoliza, string document)
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("CreaPDFHPExstream: ", null);

            String xmlHPExstream = obtenXmlHpExstream(codCia, noPoliza, document);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("xmlHPExstream: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(xmlHPExstream, null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Poliza: " + noPoliza, null);

            byte[] yourByteArray = null;

            try
            {
                string sPubFile = "";

                if (!xmlHPExstream.Contains("Error:"))
                {
                    byte[] byt = System.Text.Encoding.UTF8.GetBytes(xmlHPExstream);
                    xmlHPExstream = Convert.ToBase64String(byt);

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("DOCUMENTO: " + document, null);

                    if (document.Equals("P"))
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("POLIZA ", null);
                        sPubFile = ConfigurationSettings.AppSettings["PubFilePolVida"];
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PLANTILLA: " + sPubFile, null);
                    }
                    else if (document.Equals("ST"))
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("SOLICITUD ", null);
                        sPubFile = ConfigurationSettings.AppSettings["PubFilePolVidaSolicitud"];
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PLANTILLA: " + sPubFile, null);
                    }
                    else if (document.Equals("CF"))
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("FATCA", null);
                        sPubFile = ConfigurationSettings.AppSettings["PubFilePolVidaFacta"];
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PLANTILLA: " + sPubFile, null);
                    }
                    else if (document.Equals("CRS"))
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("CRS ", null);
                        sPubFile = ConfigurationSettings.AppSettings["PubFilePolVidaCRS"];
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PLANTILLA: " + sPubFile, null);
                    }
                    else if (document.Equals("PPR"))
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PPR ", null);
                        sPubFile = ConfigurationSettings.AppSettings["PubFilePolVidaPPR"];
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PLANTILLA: " + sPubFile, null);
                    }
                    else if(document.Equals("CP"))
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("CP ", null);
                        sPubFile = ConfigurationManager.AppSettings["PubFilePolVidaCP"];
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("PLANTILLA: " + sPubFile, null);
                    }

                    string userHPExstream = ConfigurationSettings.AppSettings["UserHp"];
                    string paswHPExstream = ConfigurationSettings.AppSettings["PassHp"];
                    string emisionSector = ConfigurationSettings.AppSettings["EmisionSector"];

                    string RutaURL = ConfigurationManager.AppSettings["URLWSHpExtream"];//url servicio hp

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(RutaURL);
                    webRequest.Headers.Add(@"SOAP:Action");
                    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                    webRequest.Accept = "text/xml";
                    webRequest.Method = "POST";
                    HttpWebRequest request = webRequest;

                   // HttpWebRequest request = CreateWebRequest();
                    XmlDocument soapEnvelopeXml = new XmlDocument();

                    soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:eng=""urn:hpexstream-services/Engine"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                                                               <soapenv:Header>
                                                                  <wsse:Security soapenv:mustUnderstand=""0"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                                                                     <wsse:UsernameToken wsu:Id=""UsernameToken-1"">
                                                                        <wsse:Username>" + userHPExstream + @"</wsse:Username>
                                                                        <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">" + paswHPExstream + @"</wsse:Password>
                                                                        <wsse:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">9XbhOoet06M5XXD83sgM7Q==</wsse:Nonce>
                                                                        <wsu:Created>2015-07-21T08:23:30.207Z</wsu:Created>
                                                                     </wsse:UsernameToken>
                                                                  </wsse:Security>
                                                               </soapenv:Header>
                                                               <soapenv:Body>
                                                                  <eng:Compose>
                                                                     <EWSComposeRequest>
                                                                        <driver>
                                                                           <!--Fichero de datos en Base64-->
                                                                           <driver>" + xmlHPExstream + @"</driver>
                                                                           <fileName>INPUT</fileName>
                                                                        </driver>
                                                                        <engineOptions>
                                                                           <name>IMPORTDIRECTORY</name>
                                                                           <value>/var/opt/exstream/pubs</value>
                                                                        </engineOptions>
                                                                        <engineOptions>
                                                                           <!--Ruta donde se encuentra fichero de referencias-->
                                                                           <!--A su vez, el fichero contiene ruta a recursos-->
                                                                           <name>FILEMAP</name>
                                                                           <value>REFERENCIAS,/var/opt/exstream/pubs/" + emisionSector + @"/REFERENCIAS.ini</value>
                                                                        </engineOptions>
                                                                        <!--Optional:-->
                                                                        <pubFile>" + sPubFile + @"</pubFile>
                                                                     </EWSComposeRequest>
                                                                  </eng:Compose>
                                                               </soapenv:Body>
                                                            </soapenv:Envelope>");

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("soapEnvelopeXml: ", null);
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(soapEnvelopeXml.ToString(), null);

                    using (Stream stream = request.GetRequestStream())
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("request.GetRequestStream: ", null);
                        soapEnvelopeXml.Save(stream);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("save", null);
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("request.GetResponse()", null);

                        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("response.GetResponseStream()", null);

                            string soapResult = rd.ReadToEnd();

                            int iCadenaIni = soapResult.IndexOf("<fileOutput>") + 12;
                            int iCadenaFin = soapResult.IndexOf("</fileOutput>");

                            sFileOutput = soapResult.Substring(iCadenaIni, (iCadenaFin - iCadenaIni));

                            yourByteArray = Convert.FromBase64String(sFileOutput);
                            //string RutaImp = ConfigurationSettings.AppSettings["RutaImpresionArchivo"];
                            //genera el pdf en base a la cadena de 64 bits
                            //WriteByteArrayToPdf(sFileOutput, RutaImp, noPoliza + ".pdf");
                        }
                    }

                }
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("yourByteArray: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(yourByteArray.ToString(), null);


                return yourByteArray;
            }
            catch (Exception ex)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("HP: ", ref idError, ex);
                throw new Exception("Error : " + idError);
            }
        }


        public String obtenXmlHpExstream(String codCia, String sNumPoliza, String document)
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Obtiene XML: " + document, null);

            OracleConnection conexion = null;
            var cmd = new Comando();
            DataRow dr = null;

            decimal dNumSpto = 0;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();

                    cmd.Connection = conexion;

                    cmd.CommandText = "TRON2000.EM_K_IMP_GRAL_MMX.p_imprime_poliza";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Decimal, Convert.ToDecimal(codCia));
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, sNumPoliza);
                    cmd.AgregarInParametro("p_num_spto", OracleDbType.Decimal, dNumSpto);
                    cmd.AgregarInParametro("p_tip_emision", OracleDbType.Varchar2, document);
                    cmd.AgregarOutParametro("p_xml", OracleDbType.Clob, 32000);
                    cmd.AgregarOutParametro("p_error", OracleDbType.Varchar2, 500);

                    dr = cmd.EjecutarRegistroSP();

                    if (!string.IsNullOrEmpty(Convert.ToString(dr["p_error"])))
                        return "Error:  " + Convert.ToString(dr["p_error"]);
                    else
                        return Convert.ToString(dr["p_xml"]);
                }
            }
            catch (Exception ex)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Error: obtenXmlHpExstream", ref idError, null);
                throw new Exception("TWImpPoliza(" + sNumPoliza + ") : " + idError + " - " + ex.Message);
            }
        }

        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
        public HttpWebRequest CreateWebRequest()
        {
            string RutaURL = ConfigurationSettings.AppSettings["URLWSHpExtream"];
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(RutaURL);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public void WriteByteArrayToPdf(string inPDFByteArrayStream, string pdflocation, string fileName)
        {

            byte[] data = Convert.FromBase64String(inPDFByteArrayStream);

            if (Directory.Exists(pdflocation))
            {
                pdflocation = pdflocation + fileName;

                using (FileStream Writer = new System.IO.FileStream(pdflocation, FileMode.Create, FileAccess.Write))
                {
                    Writer.Write(data, 0, data.Length);
                }
            }
            else
            {
                throw new System.Exception("PDF Shared Location not found");
            }
        }

        public string getEmail(string noPoliza)
        {
            OracleConnection conexion = null;
            var cmd = new Comando();
            DataRow dr = null;

            decimal dNumSpto = 0;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();

                    cmd.Connection = conexion;

                    cmd.CommandText = "ev_k_112_unit.GET_EMAIL";
                    cmd.AgregarInParametro("P_NUM_POLIZA", OracleDbType.Varchar2, noPoliza);
                    cmd.AgregarOutParametro("P_EMAIL", OracleDbType.Varchar2, 30);


                    dr = cmd.EjecutarRegistroSP();

                        return Convert.ToString(dr["P_EMAIL"]);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("getEmail(" + noPoliza + ") : " + ex.Message);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getEmail(" + noPoliza + ") : " + ex.Message, ex);
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }
            }
            return null;
        }

        public String getModalidad(String sNumPoliza)
        {
            OracleConnection conexion = null;
            var cmd = new Comando();
            DataRow dr = null;

            decimal dNumSpto = 0;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();

                    cmd.Connection = conexion;

                    cmd.CommandText = "ev_k_112_unit.GET_MODALIDAD";
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, sNumPoliza);
                    cmd.AgregarOutParametro("p_cod_modalidad", OracleDbType.Varchar2, 10);

                    dr = cmd.EjecutarRegistroSP();

                    return dr["p_cod_modalidad"].ToString();
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("getModalidad(" + sNumPoliza + ") : " + ex.Message);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getModalidad(" + sNumPoliza + ") : " + ex.Message, ex);
            }
            return null;
        }

        public string getJsonBenef(string num_poliza)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getJsonBenef: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(num_poliza, null);

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "ev_k_112_unit.GET_MSG_JSON_BENF";
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, num_poliza);
                    cmd.AgregarOutParametro("p_msg_json_benef", OracleDbType.Clob, 10000);

                    objDataSet = cmd.EjecutarRefCursorSP();

                    return getNacionalidad(objDataSet.Tables[0]);
                }
            }
            catch (System.Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDetalle: ", _error);
                //throw;
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
            return null;
        }

        public string getNacionalidad(DataTable dt){

            string[] json;
            string nacionalidad = "";
            
            foreach (DataRow row in dt.Rows)
            {
                json = row[0].ToString().Split(',');

                nacionalidad = json[4].ToString().Split(':')[1].Replace("\"", "");
            }
            return nacionalidad;
        }

    }
}