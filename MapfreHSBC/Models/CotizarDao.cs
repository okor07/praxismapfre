﻿using MapfreHSBC.Models.Cotizacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using MapfreWebCore.Oracle;
using System.Configuration;
using System.Collections;
using System.Net.Http;
using System.Net;
using System.Xml;
using System.IO;
using MapfreHSBC.Controllers.API;
using DalSoft.RestClient;
using MapfreMMX.clm;
using MapfreHSBC.Models.General;

namespace MapfreHSBC.Models
{
    public class CotizarDao
    {

        string strPaqueteVida = ConfigurationManager.AppSettings["EsquemaVida"].ToString();
        string strPaqueteUnit = ConfigurationManager.AppSettings["EsquemaUnit"].ToString();
        string strPaqueteLV = ConfigurationManager.AppSettings["EsquemaLV"].ToString();
        string strEsquemaValidaciones = ConfigurationManager.AppSettings["EsquemaValidaciones"].ToString();
        string strPaqueteParaWM = ConfigurationManager.AppSettings["PaqueteParaWM"].ToString();
        string idError;

        public IEnumerable<ComboBase> getMoneda()
        {
            List<ComboBase> monedas = new List<ComboBase>();
            monedas.Add(new ComboBase() { etiqueta = "PESOS", valor = "1" });
            return monedas;
        }

        /// <summary>
        /// Método para obtener los tipos de Moneda.
        /// </summary>
        /// <param name="CodigoRamo">Parámetro de tipo entero para almacenar el código del Ramo.</param>
        /// <param name="contrato">Parámetro de tipo cadena para almacenar el contrato.</param>
        /// <returns>Regresa un objeto de tipo DataTable con la Información de los Tipos de Moneda.</returns>
        public IEnumerable<ComboBase> getMoneda(int CodigoRamo, string contrato, int tipoPlan)
        {
            DataSet objDataSet = new DataSet();

            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteVida + ".p_obtiene_moneda";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, 1);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, 112);
                    cmd.AgregarInParametro("p_num_contrato", OracleDbType.Double, 99999);
                    cmd.AgregarInParametro("p_tip_plan", OracleDbType.Int32, 0);
                    cmd.AgregarOutParametro("p_cod_mon", OracleDbType.RefCursor, 2000);

                    // var items = cmd.EjecutarRegistroSP();

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                return getDatos(objDataSet.Tables[0]);
                //}
            }
            catch (System.Exception _error)
            {
                //MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("",_error);

                throw;
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public IEnumerable<ComboBase> getFormasPago(int codModalidad)
        {
            //DataTable objDataSet = new DataTable();
            DataSet objDataSet = new DataSet();

            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    //cmd.CommandText = "SELECT COD_VALOR, NOM_VALOR FROM G1010031 WHERE COD_CAMPO='FORMA_PAGO_VIDA' " +
                    //  " AND COD_IDIOMA='ES' ORDER BY NUM_ORDEN";

                    cmd.CommandText = strEsquemaValidaciones + strPaqueteParaWM + ".p_devuelve_forma_pago";
                    cmd.AgregarInParametro("p_cod_portal", OracleDbType.Int32, 2);
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, 1);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, 112);
                    cmd.AgregarInParametro("p_cod_modalidad", OracleDbType.Int32, codModalidad);
                    cmd.AgregarInParametro("p_num_contrato", OracleDbType.Double, 99999);
                    cmd.AgregarInParametro("p_fecha_validez", OracleDbType.Date, DateTime.Today);
                    cmd.AgregarOutParametro("p_forma_pago", OracleDbType.RefCursor, 0);
                    //objDataSet = cmd.EjecutarMultiRegistro();
                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                return getDatos(objDataSet.Tables[0]);
            }
            catch (System.Exception _error)
            {
                System.Diagnostics.Debug.WriteLine("_error " + _error);
                throw;
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }


        public IEnumerable<ComboBase> getPeriodicidades(int codModalidad)
        {
            return getFormasPago(codModalidad);
        }
        /// <summary>
        /// Método para obtener los tipos de Modalidad.
        /// </summary>
        /// <param name="cod_cia">Parámetro de tipo entero para almacenar el codigo de compañia.</param>
        /// <param name="cod_ramo">Parámetro de tipo entero para almacenar el codigo de ramo.</param>
        /// <returns>Regresa un objeto de tipo DataTable con la Información de los Tipos de Modalidad.</returns>
        public IEnumerable<ComboBase> getModalidad(string tabla, int cod_version, string param)
        {
            return getLV(tabla, cod_version, param, 1);

            //DataSet objDataSet = new DataSet();
            //OracleConnection conexion = null;

            //try
            //{
            //    Conexion _Conexion = new Conexion();

            //    using (conexion = _Conexion.GetConexionId("ConnectionTW"))
            //    {
            //        conexion.Open();
            //        var cmd = new Comando();

            //        cmd.Connection = conexion;
            //        cmd.CommandText = strPaqueteUnit + ".GET_MODALIDADES";
            //        cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, cod_cia);
            //        cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, cod_ramo);
            //        cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

            //        objDataSet = cmd.EjecutarRefCursorSP();
            //    }
            //    return getDatos(objDataSet.Tables[0]);
            //    //}
            //}
            //catch (System.Exception _error)
            //{
            //    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(_error.ToString(), _error);

            //    throw;
            //}
            //finally
            //{
            //    if (conexion != null && conexion.State != ConnectionState.Closed)
            //    {
            //        conexion.Close();
            //        conexion.Dispose();
            //    }

            //}
        }


        public IEnumerable<ComboBase> getDatos(DataTable tabla)
        {

            List<ComboBase> datos = new List<ComboBase>();

            foreach (DataRow row in tabla.Rows)
            {

                ComboBase elemento = new ComboBase();

                elemento.valor = row[0].ToString();
                elemento.etiqueta = row[1].ToString();
                datos.Add(elemento);
            }

            return datos;
        }

        public IEnumerable<ComboBase> getDatos2(DataTable tabla)
        {

            List<ComboBase> datos = new List<ComboBase>();

            foreach (DataRow row in tabla.Rows)
            {

                if (!(row[0].ToString().Equals("4")))
                {
                    ComboBase elemento = new ComboBase();

                    elemento.valor = row[1].ToString();
                    elemento.etiqueta = row[0].ToString();
                    datos.Add(elemento);
                }

            }

            return datos;
        }


        public DatoCotizacion.Respuesta getRespuestaWs1(DatoCotizacion.Datos datos)
        {
            StringBuilder errores = new StringBuilder();
            Boolean faltanDatos = false;
            errores.Append("No se recibieron los siguientes campos: ");
            DatoCotizacion.Respuesta resp = new DatoCotizacion.Respuesta();
            DatoCotizacion.MsgJsonRespuesta msgjResp = new DatoCotizacion.MsgJsonRespuesta();
            resp.msgJson = msgjResp;
            if (datos != null && datos.msgJson != null)
            {
                if (datos.msgJson.idTransaccion <= 0)
                {
                    faltanDatos = true;
                    errores.Append("idTransaccion, ");
                }
                if (datos.msgJson.idPromotor == null || datos.msgJson.idPromotor == "")
                {
                    faltanDatos = true;
                    errores.Append("idPromotor, ");
                }
                if (datos.msgJson.sexo == null)
                {
                    faltanDatos = true;
                    errores.Append("sexo, ");
                }
                else
                {
                    if (datos.msgJson.sexo < 0 || datos.msgJson.sexo > 1)
                    {
                        faltanDatos = true;
                        errores.Append("se recibio valor incorrecto en el campo sexo, ");
                    }
                }

                if (datos.msgJson.fechaDeNacimiento == null || datos.msgJson.fechaDeNacimiento == "")
                {
                    faltanDatos = true;
                    errores.Append("fechaDeNacimiento, ");
                }
                else
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(datos.msgJson.fechaDeNacimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (dt == null)
                        {
                            faltanDatos = true;
                            errores.Append("fechaDeNacimiento, ");
                        }
                    }
                    catch (Exception ex)
                    {
                        faltanDatos = true;
                        errores.Append("fechaDeNacimiento no esta en el formato correcto, ");
                    }
                }

            }
            else
            {
                faltanDatos = true;
                errores.Append("msgJson ");
            }
            if (faltanDatos)
            {
                resp.codigoError = 1;
                resp.descripcionError = errores.ToString();
                resp.msgJson.url = "";

            }
            else
            {
                resp.codigoError = 0;
                resp.descripcionError = "Datos recibidos correctamente";

                resp.msgJson.url = string.Format("{0}://{1}{2}/{3}?{4}={5}",
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Authority,
                    HttpContext.Current.Request.ApplicationPath,
                    "Cotizar/Index",
                    ConfirmacionCotizacion.PARAM,
                    System.Net.WebUtility.UrlEncode(datos.ToJson()));
            }

            return resp;
        }

        public DatoCotizacion.RespuestaWs2 getRespuestaWs2(DatoCotizacion.Datos datos)
        {
            StringBuilder errores = new StringBuilder();
            Boolean faltanDatos = false;
            errores.Append("No se recibieron los siguientes campos: ");
            DatoCotizacion.RespuestaWs2 resp = new DatoCotizacion.RespuestaWs2();
            if (datos != null)
            {
                if (datos.msgJson.idTransaccion <= 0)
                {
                    faltanDatos = true;
                    errores.Append("idTransaccion, ");
                }
                if (datos.msgJson.idPromotor == null || datos.msgJson.idPromotor == "")
                {
                    faltanDatos = true;
                    errores.Append("idPromotor, ");
                }
            }
            if (faltanDatos)
            {
                resp.idTransaccion = datos.msgJson.idTransaccion;
                resp.idPromotor = datos.msgJson.idPromotor;
                resp.codigoError = 1;
                resp.descripcionError = errores.ToString();
                resp.idConfirmacion = 1;

            }
            else
            {
                resp.idTransaccion = datos.msgJson.idTransaccion;
                resp.idPromotor = datos.msgJson.idPromotor;
                resp.codigoError = 0;
                resp.descripcionError = "Datos recibidos correctamente";
                resp.idCotizacionMapfre = getIdCotizacion().ToString();
                resp.idConfirmacion = 0;
            }

            return resp;
        }

        public DatosContratante.Respuesta getRespuestaWs3(DatosContratante.Datos datos)
        {

            StringBuilder errores = new StringBuilder();
            Boolean faltanDatos = false;
            errores.Append("No se recibieron los siguientes campos: ");
            DatosContratante.Respuesta resp = new DatosContratante.Respuesta();
            DatosContratante.MsgJsonRespuesta msgjResp = new DatosContratante.MsgJsonRespuesta();
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En getRespuestaWs3: ", null);
            try
            {

                resp.msgJson = msgjResp;
                if (datos != null)
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("DatosContratante.Datos no es nulo " + datos, null);
                    if (datos.msgJson.idTransaccion <= 0)
                    {
                        faltanDatos = true;
                        errores.Append("idTransaccion, ");
                    }
                    if (datos.msgJson.idPromotor == null || datos.msgJson.idPromotor == "")
                    {
                        faltanDatos = true;
                        errores.Append("idPromotor, ");
                    }
                    if (datos.msgJson.idCotizacionMapfre == null || datos.msgJson.idCotizacionMapfre == "")
                    {
                        faltanDatos = true;
                        errores.Append("idCotizacionMapfre, ");
                    }
                    if (datos.msgJson.rfc == null || datos.msgJson.rfc == "")
                    {
                        faltanDatos = true;
                        errores.Append("rfc, ");
                    }
                    if (datos.msgJson.idNacionalidadMapfre == null || datos.msgJson.idNacionalidadMapfre == "")
                    {
                        faltanDatos = true;
                        errores.Append("idNacionalidadMapfre, ");
                    }
                    if (datos.msgJson.domicilioContratanteCalle == null || datos.msgJson.domicilioContratanteCalle == "")
                    {
                        faltanDatos = true;
                        errores.Append("domicilioContratanteCalle, ");
                    }
                    //if (datos.msgJson.domicilioContratanteNumInt == null || datos.msgJson.domicilioContratanteNumInt == "")
                    //{
                    //    faltanDatos = true;
                    //    errores.Append("domicilioContratanteNumInt, ");
                    //}
                    if (datos.msgJson.domicilioContratanteNumExt == null || datos.msgJson.domicilioContratanteNumExt == "")
                    {
                        faltanDatos = true;
                        errores.Append("domicilioContratanteNumExt, ");
                    }
                    if (datos.msgJson.domicilioContratanteColonia == null || datos.msgJson.domicilioContratanteColonia == "")
                    {
                        faltanDatos = true;
                        errores.Append("domicilioContratanteColonia, ");
                    }
                    if (datos.msgJson.paisResidencia == null || datos.msgJson.paisResidencia == "")
                    {
                        faltanDatos = true;
                        errores.Append("paisResidencia, ");
                    }
                    if (datos.msgJson.codigoPostal == null || datos.msgJson.codigoPostal == "")
                    {
                        faltanDatos = true;
                        errores.Append("codigoPostal, ");
                    }
                    if (datos.msgJson.nombreDeContratante == null || datos.msgJson.nombreDeContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("nombreDeContratante, ");
                    }

                    if (datos.msgJson.apellidoPaternoContrante == null || datos.msgJson.apellidoPaternoContrante == "")
                    {
                        faltanDatos = true;
                        errores.Append("apellidoPaternoContrante, ");
                    }
                    //if (datos.msgJson.apellidoMaternoContrante == null || datos.msgJson.apellidoMaternoContrante == "")
                    //{
                    //    faltanDatos = true;
                    //    errores.Append("apellidoMaternoContrante, ");
                    //}
                    if (datos.msgJson.tipoDePersona == null || datos.msgJson.tipoDePersona.ToString() == "")
                    {
                        faltanDatos = true;
                        errores.Append("tipoDePersona, ");
                    }

                    if (datos.msgJson.curp == null || datos.msgJson.curp == "")
                    {
                        faltanDatos = true;
                        errores.Append("curp, ");
                    }
                    if (datos.msgJson.paisOrigen == null || datos.msgJson.paisOrigen == "")
                    {
                        faltanDatos = true;
                        errores.Append("paisOrigen, ");
                    }
                    if (datos.msgJson.telefono == null || datos.msgJson.telefono == "")
                    {
                        faltanDatos = true;
                        errores.Append("paisOrigen, ");
                    }
                    if (datos.msgJson.tipoIdentificacion == null || datos.msgJson.tipoIdentificacion == "")
                    {
                        faltanDatos = true;
                        errores.Append("tipoIdentificacion, ");
                    }
                    if (datos.msgJson.numeroIdentificacion == null || datos.msgJson.numeroIdentificacion == "")
                    {
                        faltanDatos = true;
                        errores.Append("numeroIdentificacion, ");
                    }
                    if (datos.msgJson.cuentaClabeContratante == null || datos.msgJson.cuentaClabeContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("cuentaClabeContratante, ");
                    }
                    if (datos.msgJson.paisNacimientoContratante == null || datos.msgJson.paisNacimientoContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("paisNacimientoContratante, ");
                    }
                    if (datos.msgJson.cuidadNacimientoContratante == null || datos.msgJson.cuidadNacimientoContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("cuidadNacimientoContratante, ");
                    }
                    if (datos.msgJson.paisResidenciaFiscalContratante == null || datos.msgJson.paisResidenciaFiscalContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("paisResidenciaFiscalContratante, ");
                    }
                    if (datos.msgJson.estadoCivilContratante == null || datos.msgJson.estadoCivilContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("estadoCivilContratante, ");
                    }
                    if (datos.msgJson.ocupacionContratante == null || datos.msgJson.ocupacionContratante == "")
                    {
                        faltanDatos = true;
                        errores.Append("ocupacionContratante, ");
                    }
                    if (datos.msgJson.statusPEPS < 0 && datos.msgJson.statusPEPS > 1)
                    {
                        faltanDatos = true;
                        errores.Append("statusPEPS, ");
                    }
                    if (datos.msgJson.datosBeneficiarios == null || datos.msgJson.datosBeneficiarios.Count() == 0)
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Beneficiarios nulos ó 0 ", null);
                        faltanDatos = true;
                        errores.Append("datosBeneficiarios (se debe indicar por lo menos 1 beneficiario), ");
                    }
                    else
                    {
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Beneficiarios no son nulos", null);
                        foreach (DatosContratante.DatosBeneficiario ben in datos.msgJson.datosBeneficiarios)
                        {
                            if (ben.idTransaccion <= 0)
                            {
                                faltanDatos = true;
                                errores.Append("idTransaccion del beneficiario, ");
                            }
                            if (ben.idPromotor == null || ben.idPromotor == "")
                            {
                                faltanDatos = true;
                                errores.Append("idPromotor del beneficiario, ");
                            }
                            if (ben.idCotizacionMapfre == null || ben.idCotizacionMapfre == "")
                            {
                                faltanDatos = true;
                                errores.Append("idCotizacionMapfre del beneficiario, ");
                            }
                            if (ben.idBeneficiario <= 0)
                            {
                                faltanDatos = true;
                                errores.Append("idBeneficiario del beneficiario, ");
                            }
                            if (ben.nombreDeBeneficiario == null || ben.nombreDeBeneficiario == "")
                            {
                                faltanDatos = true;
                                errores.Append("nombreDeBeneficiario del beneficiario, ");
                            }
                            if (ben.apellidoPaternoBeneficiario == null || ben.apellidoPaternoBeneficiario == "")
                            {
                                faltanDatos = true;
                                errores.Append("apellidoPaternoBeneficiario del beneficiario, ");
                            }
                            if (ben.tipoPersona == 0 || ben.tipoPersona.ToString() == "")
                            {
                                faltanDatos = true;
                                errores.Append("tipoPersona del beneficiario, ");
                            }
                            if (ben.idParentescoBeneficiarioMapfre == null || ben.idParentescoBeneficiarioMapfre == "")
                            {
                                faltanDatos = true;
                                errores.Append("idParentescoBeneficiarioMapfre del beneficiario, ");
                            }
                            if (ben.fechaNacimientoBeneficiario == null || ben.fechaNacimientoBeneficiario == "")
                            {
                                faltanDatos = true;
                                errores.Append("fechaNacimientoBeneficiario del beneficiario, ");
                            }
                            else
                            {
                                try
                                {
                                    DateTime dt = DateTime.ParseExact(ben.fechaNacimientoBeneficiario, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    if (dt == null)
                                    {
                                        faltanDatos = true;
                                        errores.Append("fechaNacimientoBeneficiario, ");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Error en formato de fecha (fechaNacimientoBeneficiario)", ex);
                                    faltanDatos = true;
                                    errores.Append("fechaNacimientoBeneficiario no esta en el formato correcto, ");
                                }
                            }
                            if (ben.domicilioBeneficiarioCalle == null || ben.domicilioBeneficiarioCalle == "")
                            {
                                faltanDatos = true;
                                errores.Append("domicilioBeneficiarioCalle del beneficiario, ");
                            }
                            //if (ben.domicilioBeneficiarioNumInt == null || ben.domicilioBeneficiarioNumInt == "")
                            //{
                            //    faltanDatos = true;
                            //    errores.Append("domicilioBeneficiarioNumInt del beneficiario, ");
                            //}
                            if (ben.domicilioBeneficiarioNumExt == null || ben.domicilioBeneficiarioNumExt == "")
                            {
                                faltanDatos = true;
                                errores.Append("domicilioBeneficiarioNumExt del beneficiario, ");
                            }
                            if (ben.domicilioBeneficiarioNomColonia == null || ben.domicilioBeneficiarioNomColonia == "")
                            {
                                faltanDatos = true;
                                errores.Append("domicilioBeneficiarioNomColonia del beneficiario, ");
                            }
                            if (ben.cpBeneficiario == null || ben.cpBeneficiario == "")
                            {
                                faltanDatos = true;
                                errores.Append("cpBeneficiario del beneficiario, ");
                            }
                            if (ben.telefono == null || ben.telefono == "")
                            {
                                faltanDatos = true;
                                errores.Append("telefono del beneficiario, ");
                            }
                            if (ben.genero < 0 || ben.genero > 1)
                            {
                                faltanDatos = true;
                                errores.Append("genero del beneficiario, ");
                            }
                            if (ben.nacionalidadBeneficiario == null || ben.nacionalidadBeneficiario == "")
                            {
                                faltanDatos = true;
                                errores.Append("nacionalidadBeneficiario del beneficiario, ");
                            }
                            if (ben.genero == null || ben.porcentaje > 100)
                            {
                                faltanDatos = true;
                                errores.Append("porcentaje del beneficiario, ");
                            }
                            if (ben.tipoBeneficiario == null || ben.tipoBeneficiario.Trim() == "")
                            {
                                faltanDatos = true;
                                errores.Append("tipoBeneficiario del beneficiario, ");
                            }
                        }
                    }
                }
                else
                {
                    faltanDatos = true;
                    errores.Append("Error en los campos que definen el mensaje, msgJson");
                }
                if (faltanDatos)
                {

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Hicieron falta datos en WS3", ref idError, null);
                    resp.codigoError = 1;
                    resp.msgJson.url = "";
                    resp.descripcionError = errores.ToString() + " : " + idError;
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("errores " + errores.ToString(), null);
                }
                else
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Los datos del contratante son correctos ", null);
                    if ((datos.msgJson.datosBeneficiarios == null) || (datos.msgJson.datosBeneficiarios != null
                            && datos.msgJson.datosBeneficiarios.Count() > 5))
                    {
                        resp.codigoError = 1;
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Los datos de los beneficiarios son incorrectos ", null);
                        if (datos.msgJson.datosBeneficiarios == null)
                        {
                            resp.descripcionError = "No se han recibido beneficiarios.";
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(resp.descripcionError, null);
                        }
                        else
                        {
                            resp.descripcionError = "No se aceptan mas de 5 benericiarios.";
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(resp.descripcionError, null);
                        }
                    }
                    else
                    {
                        //Emision
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Los datos de los beneficiarios son correctos, se procede a emisión ", null);
                        Double dato;
                        Int32 datoInt;

                        AltaCotizacion datosAlta = getCotizacion(1, 112, datos.msgJson.idCotizacionMapfre);
                        string[] json = datosAlta.msgJson.Replace('"', ' ').Split(',');

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("--------------------------------", null);

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("json: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.msgJson, null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("--------------------------------", null);

                        //idpromotor
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("idPromotor: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[0].Split(':')[1].Trim(), null);
                        //periodicidad
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("periodicidad: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[12].Split(':')[1].Trim(), null);
                        //modalidad
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("modalidad: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[9].Split(':')[1].Trim(), null);
                        //fec_nacimiento
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("fechaNacimiento: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[4].Split(':')[1].Trim(), null);
                        //prima inicial
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("primaInicial: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[8].Split(':')[1], null);
                        //plazo
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("plazo: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[14].Split(':')[1].Trim(), null);
                        //correo
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("correo: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[6].Split(':')[1].Trim(), null);

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("--------------------------------", null);


                        datosAlta.idPromotor = json[0].Split(':')[1].Trim();

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("idPromotor: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.idPromotor, null);

                        datosAlta.periodicidad = json[12].Split(':')[1].Trim();

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("periodicidad: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.periodicidad, null);

                        datosAlta.modalidad = json[9].Split(':')[1].Trim();

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("modalidad: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.modalidad, null);

                        datosAlta.fechaNacimiento = json[4].Split(':')[1].Trim().ToString();

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("fechaNacimiento: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.fechaNacimiento, null);

                        if (!string.IsNullOrEmpty(json[8].Split(':')[1].Trim()) && Double.TryParse(json[8].Split(':')[1].Trim(), out dato))
                            datosAlta.primaInicial = dato;
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("primaInicial: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[8].Split(':')[1].Trim(), null);
                        }
                        //datosAlta.primaInicial = !string.IsNullOrEmpty(json[8].Split(':')[1]) ? Convert.ToDouble(json[8].Split(':')[1]) : 0.0;

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("primaInicial: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.primaInicial.ToString(), null);

                        datosAlta.plazo = json[14].Split(':')[1].Trim();

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("plazo: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.plazo, null);

                        datosAlta.correoElectronico = json[6].Split(':')[1].Trim();

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("correo: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(datosAlta.correoElectronico, null);

                        long idCotizacion = 0;
                        if (!string.IsNullOrEmpty(json[17].Split(':')[1].Trim()) && Double.TryParse(json[17].Split(':')[1].Trim(), out dato))
                            datosAlta.pctInversion1 = dato;
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("pctInversion1: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[17].Split(':')[1].Trim(), null);
                        }
                        
                        if (!string.IsNullOrEmpty(json[18].Split(':')[1].Trim()) && Double.TryParse(json[18].Split(':')[1].Trim(), out dato))
                            datosAlta.pctInversion2 = dato;
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("pctInversion2: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[18].Split(':')[1].Trim(), null);
                        }
                        //datosAlta.pctInversion2 = Convert.ToDouble(json[18].Split(':')[1].Trim());
                        if (!string.IsNullOrEmpty(json[19].Split(':')[1].Trim()) && Double.TryParse(json[19].Split(':')[1].Trim(), out dato))
                            datosAlta.pctInversion3 = dato;
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("pctInversion3: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[19].Split(':')[1].Trim(), null);
                        }
                        //datosAlta.pctInversion3 = Convert.ToDouble(json[19].Split(':')[1].Trim());
                        //HHAC
                        if(!string.IsNullOrEmpty(json[20].Split(':')[1].Trim()) && Double.TryParse(json[20].Split(':')[1].Trim(), out dato))
                        {
                            datosAlta.pctInversion4 = dato;
                        }
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("pctInversion4: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[20].Split(':')[1].Trim(), null);
                        }
                        if (!string.IsNullOrEmpty(json[21].Split(':')[1].Trim()) && Double.TryParse(json[21].Split(':')[1].Trim(), out dato))
                        {
                            datosAlta.pctInversion5 = dato;
                        }
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("pctInversion5: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[21].Split(':')[1].Trim(), null);
                        }

                        if (!string.IsNullOrEmpty(json[16].Split(':')[1].Trim()) && Int32.TryParse(json[16].Split(':')[1].Trim(), out datoInt))
                            datosAlta.diaCobro = datoInt;
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("diaCobro: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[16].Split(':')[1].Trim(), null);
                        }
                        //datosAlta.diaCobro = Convert.ToInt32(json[16].Split(':')[1]);
                        if (!string.IsNullOrEmpty(json[10].Split(':')[1].Trim()) && Double.TryParse(json[10].Split(':')[1].Trim(), out dato))
                            datosAlta.aportaciones = dato;
                        else
                        {
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("aportaciones: ", null);
                            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json[10].Split(':')[1].Trim(), null);
                        }
                        //datosAlta.aportaciones = Convert.ToDouble(json[10].Split(':')[1]);

                        datosAlta.periodicidadText = json[13].Split(':')[1];
                        datosAlta.perfil = json[7].Split(':')[1].Trim();
                        datosAlta.perfilText = json[22].Split(':')[1].Trim();//20

                        //String xmlCotiza = getXmlCotizacion(getParametrosXmlCotizacion(datosAlta, "P", datos));

                        String xmlEmision = getXmlEmision(getParametrosXmlCotizacion(datosAlta, "P", datos), datos);


                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("EstablecerEmision XML Emision: ", null);
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(xmlEmision, null);


                        String numPoliza = EstablecerEmision(xmlEmision);

                        if (numPoliza != null)
                        {
                            if (long.TryParse(numPoliza, out idCotizacion))
                            {
                                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("POLIZA:  ", null);
                                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(idCotizacion.ToString(), null);


                                //Respuesta
                                resp.codigoError = 0;
                                resp.descripcionError = "Se recibieron datos del contratante y " + datos.msgJson.datosBeneficiarios.Count() + " beneficiarios";
                                resp.msgJson.url = string.Format("{0}://{1}{2}/{3}?{4}={5}",
                                    HttpContext.Current.Request.Url.Scheme,
                                    HttpContext.Current.Request.Url.Authority,
                                    HttpContext.Current.Request.ApplicationPath,
                                    "Detalle/Detalle",
                                    "id",
                                    //datos.msgJson.idCotizacionMapfre);
                                    idCotizacion);

                                //INSERT JSON_BENEF
                                insert_P2300067(datosAlta, idCotizacion.ToString());
                                updateJsonBenef(datos.ToJson(), idCotizacion.ToString());
                                //UPDATE Número de Poliza Emitida
                                updatePolizaEmitida(datosAlta.numCotizacion.ToString(), numPoliza);

                            }
                            else
                            {

                                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("codigoError: 1", ref idError, null);
                                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("ERROR al realizar la emisión : " + idError + " - No. Póliza" + numPoliza, null);

                                resp.codigoError = 1;
                                resp.descripcionError = "ERROR al realizar la emisión : " + idError;

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getRespuestaWs3:  ", ref idError, ex);
                resp.descripcionError = "ERROR al realizar la emisión : " + idError;
                resp.codigoError = 1;
            }
            return resp;
        }

        public ProcesoCobro.Respuesta getRespuestaWs5(ProcesoCobro.Datos datos)
        {
            ProcesoCobro.Respuesta resp = new ProcesoCobro.Respuesta();
            resp.msgJson = null;
            resp.codigoError = 0;
            resp.descripcionError = "Datos recibidos correctamente";
            /*
            if (datos.msgJson.idTransaccion == null)
            {
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta idTransacción";
            }
            else if (datos.msgJson.idPromotor == null)
            {
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta idPromotor";
            }
            else if (datos.msgJson.idCotizacionMapfre == null)
            {
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta idCotizacionMapfre";
            }
            else if (datos.msgJson.codigoError == null)
            {
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta codigoError";
            }
            
            else if (datos.msgJson.descripcionError == null)
            {
                resp.codigoError = 1;
                resp.descripcionError = "Error... Falta descripcionError";
            }
            else
            {
                resp.codigoError = 0;
                resp.descripcionError = "Datos recibidos correctamente";
            }
            */
            if (datos.msgJson.codigoError == 1)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Respuesta obtenida: No se realizo cobro: ", null);
                marcaPolizaProvisional(datos);
            }
            else
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Respuesta obtenida: Si se realizo cobro: ", null);
                Hashtable map = emitePoliza(datos);

                if (map != null)
                {
                    if (map["codigoError"] != null)
                    {
                        resp.codigoError = Convert.ToInt32(map["codigoError"]);
                    }
                    if (map["codigoError"] != null)
                    {
                        resp.descripcionError = map["codigoError"].ToString();
                    }
                }

            }

            return resp;
        }

        public void marcaPolizaProvisional(ProcesoCobro.Datos datos)
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En marcaPolizaProvisional: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("idCotizacionMapfre: " + datos.msgJson.idCotizacionMapfre, null);
            DataTable dt = getDatosPoliza(datos.msgJson.idCotizacionMapfre);
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    String numPoliza = row[0].ToString();
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("numPoliza: " + numPoliza, null);
                    marcaProvisional(numPoliza, "1");
                }
            }
            catch (Exception ex)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Exception en marcaPolizaProvisional: ", ref idError, ex);
            }
        }

        public Hashtable emitePoliza(ProcesoCobro.Datos datos)
        {

            Hashtable map = new Hashtable();
            map.Add("codigoError", "0");
            map.Add("descripcionError", "");
            string[] json;
            DateTime inicioVigencia;
            DateTime finVigencia;

            Poliza.Datos datosPoliza = new Poliza.Datos();


            DataTable dt = getDatosPoliza(datos.msgJson.idCotizacionMapfre);

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    //Inicio de vigencia de la poliza
                    inicioVigencia = Convert.ToDateTime(row[1].ToString());

                    json = row[2].ToString().Split(',');

                    //Obtiene fin de vigencia
                    finVigencia = inicioVigencia.AddYears(Convert.ToInt32(json[14].ToString().Split(':')[1].Replace("\"", "")));

                    datosPoliza.idTransaccion = datos.msgJson.idTransaccion;
                    datosPoliza.idPromotor = datos.msgJson.idPromotor;
                    datosPoliza.idCotizacionMapfre = datos.msgJson.idCotizacionMapfre;
                    datosPoliza.numeroPoliza = row[0].ToString();
                    datosPoliza.inicioVigencia = inicioVigencia.ToString("dd/MM/yyyy");
                    datosPoliza.finVigencia = finVigencia.ToString("dd/MM/yyyy");
                    datosPoliza.primaInicial = Convert.ToDouble(json[8].ToString().Split(':')[1].Replace("\"", ""));
                    datosPoliza.primaAdicionales = Convert.ToDouble(json[10].ToString().Split(':')[1].Replace("\"", ""));
                    datosPoliza.frecuenciaAportaciones = json[13].ToString().Split(':')[1].Replace("\"", "");
                    datosPoliza.plan = json[23].ToString().Split(':')[1].Replace("\"", "");

                }

            }
            catch (Exception ex)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Exception en emitePoliza: ", ref idError, ex);
                map.Add("codigoError", "1");
                map.Add("descripcionError", "Error al generar poliza : " + idError);
            }
            invokeWS6(datosPoliza);
            return map;
        }

        public string invokeWS2(ConfirmacionCotizacion.Datos datosAlta)
        {
            StringBuilder json = new StringBuilder();
            json.Append("{ \"msgJson\":{\"idTransaccion\":");
            json.Append(datosAlta.idTransaccion).Append(",");
            json.Append("\"idPromotor\":\"").Append(datosAlta.idPromotor).Append("\"").Append(",");
            json.Append("\"idConfirmacion\":").Append(datosAlta.idConfirmacion).Append(",");
            json.Append("\"descripcionError\":\"").Append(datosAlta.descripcionError).Append("\"").Append(",");
            json.Append("\"idCotizacionMapfre\":\"").Append(datosAlta.idCotizacionMapfre).Append("\"").Append(",");
            json.Append("\"email\":\"").Append(datosAlta.email).Append("\"").Append(",");
            json.Append("\"primaTotal\":").Append(datosAlta.primaTotal).Append(",");
            json.Append("\"idQuotationList\":").Append(datosAlta.idListaCotizacion).Append("}}");

            string URL = ConfigurationManager.AppSettings["HSBCWs2"];

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("post: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(json.ToString(), null);
            try
            {
                if (datosAlta.idCotizacionMapfre.Equals("0"))
                {
                    return null;
                }
                else
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = json.ToString().Length;
                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                    requestWriter.Write(json);
                    requestWriter.Close();

                    WebResponse webResponse = request.GetResponse();
                    Stream webStream = webResponse.GetResponseStream();
                    StreamReader responseReader = new StreamReader(webStream);
                    string response = responseReader.ReadToEnd();
                    Console.Out.WriteLine(response);
                    responseReader.Close();

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("response: ", null);
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(response, null);

                    return response;
                }
            }
            catch (Exception e)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Exception: ", ref idError, e);
                return "error : " + idError;
                //throw new Exception("Error : " + idError, e.InnerException);
            }
        }

        public string invokeWS4(IntentoCobro.Datos poliza)
        {
            string mensajeReferenciaBancaria = ConfigurationManager.AppSettings["mensajeReferenciaBancaria"];
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En invokeWS4 Cotizar Dao: ", null);

            //Numero Identificador para la referencia
            string identificador = getNumIdentificador(poliza.numeroPoliza.ToString());
            //string identificador = getNumIdentificador(poliza.idCotizacionMapfre.ToString());

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Identificador: " + identificador, null);

            StringBuilder json = new StringBuilder();
            json.Append("{ \"msgJson\":");
            json.Append("{\"idTransaccion\":").Append(poliza.idTransaccion).Append(",");
            json.Append("\"idPromotor\":\"").Append(poliza.idPromotor).Append("\"").Append(",");
            json.Append("\"idCotizacionMapfre\":\"").Append(poliza.idCotizacionMapfre).Append("\"").Append(",");
            json.Append("\"numeroPoliza\":").Append(poliza.numeroPoliza).Append(",");
            json.Append("\"numeroReferencia\":\"").Append(poliza.numeroReferencia + identificador).Append("\"").Append(",");
            json.Append("\"monto\":").Append(poliza.monto).Append(",");
            json.Append("\"descCargo\":\"").Append(mensajeReferenciaBancaria).Append("\"}}");

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Json Ws4 " + json.ToString(), null);

            string URL = ConfigurationManager.AppSettings["HSBCWs4"];
            String response = "";

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("URL WS4: " + URL, null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("post WS4: ", null);
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = json.ToString().Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(json);
                requestWriter.Close();

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
                responseReader.Close();

                //response = "{\"codResponse\":0,\"msgJson\":null,\"codigoError\":0,\"descripcionError\":\"sin valor\",\"idTransaccion\":2168,\"idPromotor\":\"B9099\",\"idCotizacionMapfre\":\"1611200005091\"}";

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("response: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(response, null);
                ProcesoCobro.Datos datos =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<ProcesoCobro.Datos>
                    (response);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("ProcesoCobro.Datos: " + datos, null);
                if (datos.msgJson.codigoError == 0)
                {
                    response = "Cargo Exitoso=0";
                }
                else
                {
                    response = "No se ha realizado el cargo=1";
                }
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(response, null);
                ProcesoCobro.Respuesta respuesta = getRespuestaWs5(datos);
                return response;
            }
            catch (Exception e)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Exception: ", ref idError, e);
                response = "Error al invocar " + URL + " : " + idError + " - " + e.ToString();
            }
            return response;
        }

        public async void invokeWS1()
        {
            var msgJsonVar = new
            {
                idTransaccion = 1096,
                idPromotor = "D0916",
                fechaDeNacimiento = "11/07/1990",
                sexo = 1,
                correoElectronico = "ccc@bwmeill.net"
            };
            var post = new
            {
                msgJson = msgJsonVar
            };
            string URL = "http://localhost:21371/api/rest/services/ws1";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = post.ToString().Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(post);
            requestWriter.Close();
            string response;

            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
                responseReader.Close();

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("response: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(response, null);

            }
            catch (Exception e)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Exception: ", ref idError, e);
                response = "Error al invocar " + URL + " : " + idError + " - " + e.ToString();
            }

        }


        public String invokeWS6(Poliza.Datos poliza)
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En invokeWS6 Cotizar Dao: ", null);

            StringBuilder json = new StringBuilder();
            json.Append("{ \"msgJson\":");
            json.Append("{\"idTransaccion\":").Append(poliza.idTransaccion).Append(",");
            json.Append("\"idPromotor\":\"").Append(poliza.idPromotor).Append("\"").Append(",");
            json.Append("\"idCotizacionMapfre\":\"").Append(poliza.idCotizacionMapfre).Append("\"").Append(",");
            json.Append("\"numeroPoliza\":").Append(poliza.numeroPoliza).Append(",");
            json.Append("\"inicioVigencia\":\"").Append(poliza.inicioVigencia).Append("\"").Append(",");
            json.Append("\"finVigencia\":\"").Append(poliza.finVigencia).Append("\"").Append(",");
            json.Append("\"primaInicial\":").Append(poliza.primaInicial).Append(",");
            json.Append("\"primaAdicionales\":").Append(poliza.primaAdicionales).Append(",");
            json.Append("\"frecuenciaAportaciones\":\"").Append(poliza.frecuenciaAportaciones).Append("\"").Append(",");
            json.Append("\"plan\":\"").Append(poliza.plan).Append("\"}}");

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Json Ws6 " + json.ToString(), null);

            string URL = ConfigurationManager.AppSettings["HSBCWs6"];
            String response = "";

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("URL WS6: " + URL, null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("post WS6: ", null);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = json.ToString().Length;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(json);
                requestWriter.Close();

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
                responseReader.Close();

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("response: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(response, null);

                return response;
            }
            catch (Exception e)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Exception: ", ref idError, e);
                response = "Error al invocar " + URL + " : " + idError + " - " + e.ToString();
            }
            return response;
        }

        public CotizacionMapfre.Respuesta getRespuestaWs7(CotizacionMapfre.Datos datos)
        {
            CotizacionMapfre.Respuesta resp = new CotizacionMapfre.Respuesta();
            CotizacionMapfre.MsgJsonRespuesta msgjResp = new CotizacionMapfre.MsgJsonRespuesta();
            resp.msgJson = msgjResp;
            if (datos.msgJson.idTransaccion == null)
            {
                resp.msgJson.statusCotizacion = 1;
                resp.msgJson.statusMessaje = "Error, Falta capturar idTransaccion";
            }
            else if (datos.msgJson.idCotizacionMapfre == null)
            {
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

            return resp;
        }

        public IEnumerable<DistribucionFondos> getDistribucionFondos(double primaInicial, double pctDistribucion1,
                double pctDistribucion2, double pctDistribucion3,Double pctDistribucion4,Double pctDistribucion5, long idCotizacion, string parametros)
        {
            System.Diagnostics.Debug.WriteLine("getDistribucionFondos " + idCotizacion);
            System.Diagnostics.Debug.WriteLine("pctDistribucion1 " + pctDistribucion1);
            System.Diagnostics.Debug.WriteLine("pctDistribucion2 " + pctDistribucion2);
            System.Diagnostics.Debug.WriteLine("pctDistribucion3 " + pctDistribucion3);
            double distInicial1 = 0;
            double distInicial2 = 0;
            double distInicial3 = 0;
            //HHAC ini
            double distInicial4 = 0;
            double distInicial5 = 0;
            //HHAC fin
            if (idCotizacion > 0)
            {

                DataTable dtFondos = getDistribucionFondos(idCotizacion.ToString());

                dtFondos = getDistribucionFondos(idCotizacion.ToString());


                foreach (DataRow row in dtFondos.Rows)
                {
                    if (row[0].ToString().Equals("0001"))
                    {
                        distInicial1 = Convert.ToDouble(row[2].ToString());
                    }
                    else if (row[0].ToString().Equals("0002"))
                    {
                        distInicial2 = Convert.ToDouble(row[2].ToString());

                    }
                    else if (row[0].ToString().Equals("0003"))
                    {
                        distInicial3 = Convert.ToDouble(row[2].ToString());

                    }
                    //HHAC ini
                    else if (row[0].ToString().Equals("0004"))
                    {
                        distInicial4 = Convert.ToDouble(row[2].ToString());
                    }
                    else if (row[0].ToString().Equals("0005"))
                    {
                        distInicial5 = Convert.ToDouble(row[2].ToString());
                    }
                    //HHAC fin
                }

            }


            DataTable dtNomFondos = getLV("A2309022_MPT", 1, parametros);

            string nomFondo001 = "";
            string nomFondo002 = "";
            string nomFondo003 = "";
            //HHAC ini
            string nomFondo004 = "";
            string nomFondo005 = "";
            //HHAC fin

            foreach (DataRow row in dtNomFondos.Rows)
            {
                if (row[0].ToString().Equals("0001"))
                    nomFondo001 = row[1].ToString();
                else if (row[0].ToString().Equals("0002"))
                    nomFondo002 = row[1].ToString();
                else if (row[0].ToString().Equals("0003"))
                    nomFondo003 = row[1].ToString();
                else if (row[0].ToString().Equals("0004"))
                    nomFondo004 = row[1].ToString();
                else if (row[0].ToString().Equals("0005"))
                    nomFondo005 = row[1].ToString();
            }


            List<DistribucionFondos> fondos = new List<DistribucionFondos>();
            DistribucionFondos fondo1 = new DistribucionFondos();
            fondo1.idInversion = 1;
            fondo1.idCotizacion = idCotizacion;
            fondo1.tipoInversion = nomFondo001;
            fondo1.pctDistribucion = pctDistribucion1;
            fondo1.distInicial = distInicial1;
            fondos.Add(fondo1);
            System.Diagnostics.Debug.WriteLine("distInicial1 " + distInicial1);

            DistribucionFondos fondo2 = new DistribucionFondos();
            fondo2.idInversion = 2;
            fondo2.idCotizacion = idCotizacion;
            fondo2.tipoInversion = nomFondo002;
            fondo2.pctDistribucion = pctDistribucion2;
            fondo2.distInicial = distInicial2;
            fondos.Add(fondo2);
            System.Diagnostics.Debug.WriteLine("distInicial2 " + distInicial2);

            DistribucionFondos fondo3 = new DistribucionFondos();
            fondo3.idInversion = 3;
            fondo3.idCotizacion = idCotizacion;
            fondo3.tipoInversion = nomFondo003;
            fondo3.pctDistribucion = pctDistribucion3;
            fondo3.distInicial = distInicial3;
            System.Diagnostics.Debug.WriteLine("distInicial3 " + distInicial3);
            fondos.Add(fondo3);
            //HHAC ini
            DistribucionFondos fondo4 = new DistribucionFondos();
            fondo4.idInversion = 4;
            fondo4.idCotizacion = idCotizacion;
            fondo4.tipoInversion = nomFondo004;
            fondo4.pctDistribucion = pctDistribucion4;
            fondo4.distInicial = distInicial4;
            fondos.Add(fondo4);

            DistribucionFondos fondo5 = new DistribucionFondos();
            fondo5.idInversion = 5;
            fondo5.idCotizacion = idCotizacion;
            fondo5.tipoInversion = nomFondo005;
            fondo5.pctDistribucion = pctDistribucion5;
            fondo5.distInicial = distInicial5;
            fondos.Add(fondo5);
            //HHAC fin
            return fondos;
        }


        public IEnumerable<ComboBase> getPerfiles(string tabla, int cod_version, string parametros)
        {
            return getLV(tabla, cod_version, parametros, 1);
        }

        public IEnumerable<ComboBase> getPeriodicidad(string tabla, int cod_version, string parametros)
        {
            return getLV(tabla, cod_version, parametros, 1);
        }

        public IEnumerable<ComboBase> getLV(string tabla, int cod_version, string parametros, int orden)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;
            System.Diagnostics.Debug.WriteLine("En GetListasValor getLV " + parametros);

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteLV + ".getLOV";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, 1);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, null);
                    cmd.AgregarInParametro("p_cod_campo", OracleDbType.Varchar2, null);
                    cmd.AgregarInParametro("p_nom_tabla", OracleDbType.Varchar2, tabla);
                    cmd.AgregarInParametro("p_cod_version", OracleDbType.Int32, cod_version);
                    cmd.AgregarInParametro("p_pramaetros", OracleDbType.Varchar2, parametros);

                    cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                if (orden == 1)
                {
                    return getDatos(objDataSet.Tables[0]);
                }
                else
                {
                    return getDatos2(objDataSet.Tables[0]);

                }
            }
            catch (System.Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getLV: ", ref idError, _error);

                throw new Exception(_error.Message + " : " + idError, _error.InnerException);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }
            }
        }

        public DataTable getLV(string tabla, int cod_version, string parametros)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;
            System.Diagnostics.Debug.WriteLine("En GetListasValor getLV " + parametros);

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteLV + ".getLOV";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, 1);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, null);
                    cmd.AgregarInParametro("p_cod_campo", OracleDbType.Varchar2, null);
                    cmd.AgregarInParametro("p_nom_tabla", OracleDbType.Varchar2, tabla);
                    cmd.AgregarInParametro("p_cod_version", OracleDbType.Int32, cod_version);
                    cmd.AgregarInParametro("p_pramaetros", OracleDbType.Varchar2, parametros);

                    cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();
                    return objDataSet.Tables[0];
                }

            }
            catch (System.Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getLV: ", ref idError, _error);

                throw new Exception(_error.Message + " : " + idError, _error.InnerException);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }
            }
        }

        //public IEnumerable<ComboBase> getModalidades()
        //{
        //    List<ComboBase> modalidades = new List<ComboBase>();
        //    modalidades.Add(new ComboBase() { etiqueta = "Unit Linked PPR", valor = 0 });
        //    modalidades.Add(new ComboBase() { etiqueta = "Inversión", valor = 1 });
        //    modalidades.Add(new ComboBase() { etiqueta = "Jubilación", valor = 2 });

        //    return modalidades;
        //}

        /*
        public IEnumerable<ComboBase> getPeriodicidades()
        {
            List<ComboBase> periodicidades = new List<ComboBase>();
            periodicidades.Add(new ComboBase() { etiqueta = "Unica", valor = "0" });
            periodicidades.Add(new ComboBase() { etiqueta = "Mensual", valor = "1" });
            periodicidades.Add(new ComboBase() { etiqueta = "Semestral", valor = "2" });
            periodicidades.Add(new ComboBase() { etiqueta = "Anual", valor = "3" });

            return periodicidades;
        }
        */

        public IEnumerable<String> getDiasCobro()
        {
            List<String> dias = new List<String>();
            for (int i = 1; i <= 31; i++)
            {
                dias.Add(i.ToString());
            }

            return dias;
        }

        public IEnumerable<Plazo> getPlazos(string modalidad)
        //public IEnumerable<ComboBase> getPlazos(string tabla, int cod_version, string parametros)
        {
            string plazos = ConfigurationManager.AppSettings["EA" + modalidad];
            string[] arrPlazo = plazos.Split(',');

            List<Plazo> plazo = new List<Plazo>();
            for(int i=Convert.ToInt32(arrPlazo[0]);i<= Convert.ToInt32(arrPlazo[1]); i++)
            {
                plazo.Add(new Cotizacion.Plazo() { idPlazo = Convert.ToInt64(i), nombre = "EA " + i });
            }
            //foreach (string p in arrPlazo)
            //{
            //    plazo.Add(new Cotizacion.Plazo() { idPlazo = Convert.ToInt64(p), nombre = "EA " + p });
            //}

            return plazo;
        }

        public IEnumerable<Cobertura> getCoberturas()
        {
            List<Cobertura> coberturas = new List<Cobertura>();
            coberturas.Add(new Cotizacion.Cobertura() { idCobertura = 1, nombre = "Fallecimiento", porcentaje = " 1%" });
            coberturas.Add(new Cotizacion.Cobertura() { idCobertura = 2, nombre = "Muerte accidental", porcentaje = "20%" });

            return coberturas;
        }

        public IEnumerable<PrecioSeguro> getCalculaPrecio(double precio)
        {
            List<PrecioSeguro> calculo = new List<PrecioSeguro>();
            calculo.Add(new Cotizacion.PrecioSeguro() { clave = "Prima Inicial", valor = String.Format("{0:n}", precio) });
            long derechoPoliza = getDerechoPoliza(precio);
            calculo.Add(new Cotizacion.PrecioSeguro() { clave = "Derecho de Póliza", valor = String.Format("{0:n}", derechoPoliza) });
            double primaTotal = precio + derechoPoliza;
            calculo.Add(new Cotizacion.PrecioSeguro() { clave = "Prima Total", valor = String.Format("{0:n}", primaTotal) });

            return calculo;
        }

        public IEnumerable<PrecioSeguro> getCalculaMonto(double prima, long idCotizacion)
        {
            List<PrecioSeguro> calculo = new List<PrecioSeguro>();

            double deducciones = 0;
            double totalInvertir = 0;

            calculo.Add(new Cotizacion.PrecioSeguro() { clave = "Prima Inicial", valor = String.Format("{0:n}", prima) });

            if ((idCotizacion) > 0)
            {
                deducciones = getDeducciones(idCotizacion);
            }

            calculo.Add(new Cotizacion.PrecioSeguro() { clave = "Deducciones", valor = String.Format("{0:n}", deducciones) });

            if (deducciones > 0)
            {
                totalInvertir = prima - deducciones;
            }

            calculo.Add(new Cotizacion.PrecioSeguro() { clave = "Total a Invertir", valor = String.Format("{0:n}", totalInvertir) });

            return calculo;
        }


        public IEnumerable<ComboBase> getRecursosPago()
        {
            List<ComboBase> plazos = new List<ComboBase>();
            plazos.Add(new ComboBase() { etiqueta = "Ahorro", valor = "0" });
            plazos.Add(new ComboBase() { etiqueta = "Herencia", valor = "1" });
            plazos.Add(new ComboBase() { etiqueta = "Sueldo", valor = "2" });
            plazos.Add(new ComboBase() { etiqueta = "Premio", valor = "3" });
            plazos.Add(new ComboBase() { etiqueta = "Pensión", valor = "4" });
            plazos.Add(new ComboBase() { etiqueta = "Otro", valor = "5" });

            return plazos;
        }

        public IEnumerable<ComboBase> getFormaPago()
        {
            List<ComboBase> plazos = new List<ComboBase>();
            plazos.Add(new ComboBase() { etiqueta = "CONTADO", valor = "0" });

            return plazos;
        }

        public long getDerechoPoliza(double precio)
        {
            //Random rnd = new Random();
            //return rnd.Next(0, precio);
            return 600;
        }

        public double getDeducciones(double prima)
        {
            return prima * 0.16;
        }

        public long getIdCotizacion(AltaCotizacion datosAlta)
        {
            long idCotizacion = new Random().Next(0, 99999999);
            System.Diagnostics.Debug.WriteLine("datosAlta " + datosAlta.ToString());

            System.Diagnostics.Debug.WriteLine("idCotizacion " + idCotizacion);

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("-- getIdCotizacion -- ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("datosAlta" + datosAlta.ToString(), null);
            String xmlCotiza = getXmlCotizacion(getParametrosXmlCotizacion(datosAlta, "C", null));
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("EstablecerCotizacion: ", null);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("XML: " + xmlCotiza, null);
            String numPoliza = EstablecerEmision(xmlCotiza);
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("No. Cotizacion: " + numPoliza, null);

            if (datosAlta.numCotizacion != 0)
            {
                datosAlta.msgJson = datosAlta.msgJson.Replace(datosAlta.numCotizacion.ToString(), numPoliza.ToString());
            }


            if (numPoliza != null)
            {
                if (long.TryParse(numPoliza, out idCotizacion))
                {


                    insert_P2300067(datosAlta, idCotizacion.ToString());

                    //getDeducciones
                    //double dtDeducciones = getDeducciones(idCotizacion.ToString());
                    //getDistribucionFondos
                    DataTable dtDistribucionFon = getDistribucionFondos(idCotizacion.ToString());

                }
            }

            return idCotizacion;
        }

        public long getIdCotizacion()
        {
            return new Random().Next(0, 99999999);
        }

        public Hashtable getParametrosXmlCotizacion(AltaCotizacion datosAlta, string accion, DatosContratante.Datos datosContratante)
        {
            Hashtable parametros = new Hashtable();

            //Cotizacion o Emision
            if (accion.Equals("C"))
            {
                parametros.Add("accion", "C");
            }
            else
            {
                parametros.Add("accion", "P");
            }
            parametros.Add("cod_ramo", "112");
            parametros.Add("id_promotor", datosAlta.idPromotor);
            parametros.Add("periodicidad", datosAlta.periodicidad);
            parametros.Add("nom_cliente", "RIESGO COTIZACION");
            parametros.Add("cod_modalidad", datosAlta.modalidad);
            parametros.Add("fecha_nacimiento", datosAlta.fechaNacimiento);
            parametros.Add("fecha_nacimiento_modif", datosAlta.fechaNacimiento.Replace("/", ""));
            parametros.Add("prima", datosAlta.primaInicial);
            parametros.Add("plazo", datosAlta.plazo);
            parametros.Add("anios_max", datosAlta.plazo);

            parametros.Add("imp_premio_vida", datosAlta.primaInicial);
            parametros.Add("imp_premio_vida_ad", datosAlta.aportaciones);
            parametros.Add("imp_premio_vida_ui", datosAlta.aportaciones);
            parametros.Add("datosContratante", datosContratante);
            parametros.Add("pct001", datosAlta.pctInversion1);
            parametros.Add("pct002", datosAlta.pctInversion2);
            parametros.Add("pct003", datosAlta.pctInversion3);
            //HHAC ini
            parametros.Add("pct004",datosAlta.pctInversion4);
            parametros.Add("pct005", datosAlta.pctInversion5);
            
            int num_fondos = 0;

            if (datosAlta.pctInversion1 > 0)
                num_fondos++;
            if (datosAlta.pctInversion2 > 0)
                num_fondos++;
            if (datosAlta.pctInversion3 > 0)
                num_fondos++;
            if (datosAlta.pctInversion4 > 0)
                num_fondos++;
            if (datosAlta.pctInversion5 > 0)
                num_fondos++;
            //HHAC fin
            parametros.Add("num_fondos", num_fondos);

            parametros.Add("correoElectronico", datosAlta.correoElectronico);
            parametros.Add("diaCobro", datosAlta.diaCobro);
            parametros.Add("periodoText", datosAlta.periodicidadText);

            parametros.Add("tip_perfil_inv", datosAlta.perfil);

            if (datosAlta.aportaciones == 0)
            {
                parametros.Add("tip_primas", "1");
            }
            else
            {
                parametros.Add("tip_primas", "2");
            }

            return parametros;
        }

        public DataTable getJsonProd(string num_poliza)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".GET_MSG_JSON_PROD";
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, num_poliza);
                    cmd.AgregarOutParametro("p_msg_json", OracleDbType.Clob, 10000);

                    objDataSet = cmd.EjecutarRefCursorSP();

                    return objDataSet.Tables[0];
                }
            }
            catch (System.Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDetalle: ", ref idError, _error);

                throw new Exception(_error.Message + " : " + idError, _error.InnerException);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public String getXmlCotizacion(Hashtable parametros)
        {
            return new XmlUtil().getXmlCotizacion(parametros);
        }

        public String getXmlEmision(Hashtable parametros, DatosContratante.Datos datos)
        {
            return new XmlUtil().getXmlEmision(parametros, datos);
        }

        protected string EstablecerEmision(string _XML)
        {
            OracleConnection conexion = new OracleConnection();
            DataRow row;
            DataSet ds;
            string result = string.Empty;

            try
            {
                Conexion _Conexion = new Conexion();
                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();
                    cmd.Connection = conexion;

                    cmd.CommandText = "EM_K_GEN_WS.P_LANZA_PROCESO2_XML";
                    cmd.AgregarInParametro("pcadena", OracleDbType.Clob, _XML);
                    cmd.AgregarOutParametro("pnum_poliza", OracleDbType.Varchar2, 13);
                    cmd.AgregarOutParametro("ptxt_error", OracleDbType.Varchar2, 2000);

                    row = cmd.EjecutarRegistroSP();

                    if (!string.IsNullOrEmpty(row["pnum_poliza"].ToString()))
                    {
                        result = row["pnum_poliza"].ToString();
                    }
                    else
                    {
                        result = "ERROR - " + row["ptxt_error"].ToString();
                        //throw new Exception(result);

                    }
                }
            }
            catch (Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("EstablecerEmision: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR CotizacionDA.EstablecerEmision() : " + idError + " - " + _error);
                result = _error.Message + " : " + idError;
                //throw new Exception("ERROR CotizacionDA.EstablecerEmision() : " + _error.Message);
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }
            }

            return result;
        }

        protected void insert_P2300067(AltaCotizacion datosAlta, string num_Poliza)
        {
            OracleConnection conexion = new OracleConnection();
            System.Diagnostics.Debug.WriteLine("En  insert_P2300067");
            try
            {
                //string[] fecha = datosAlta.fechaNacimiento.Split('/');
                //DateTime fecNac = new DateTime(Convert.ToInt16(fecha[2]), Convert.ToInt16(fecha[1]), Convert.ToInt16(fecha[0]));
                //DateTime fecActual = DateTime.Now;
                //var dateSpan = DateTimeSpan.CompareDates(fecNac, fecActual);
                //int anios = dateSpan.Years;

                //int edadAlcanzada = Convert.ToInt32(datosAlta.plazo);
                //string plazo = (edadAlcanzada - anios).ToString();

                //datosAlta.plazo = plazo;

                Conexion _Conexion = new Conexion();
                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();
                    cmd.Connection = conexion;

                    cmd.CommandText = "ev_k_112_unit.INSERT_P2300067";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, 1);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, 112);
                    cmd.AgregarInParametro("p_cod_modalidad", OracleDbType.Int32, datosAlta.modalidad);
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, num_Poliza);
                    cmd.AgregarInParametro("p_email", OracleDbType.Varchar2, datosAlta.correoElectronico);
                    cmd.AgregarInParametro("p_json", OracleDbType.Varchar2, datosAlta.msgJson != null ? datosAlta.msgJson : "");

                    cmd.EjecutarSP();

                }
            }
            catch (Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("EstablecerEmision: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR CotizacionDA.insert_P2300067() : " + idError + " - " + _error);
                throw new Exception("ERROR CotizacionDA.insert_P2300067 : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }
            }
        }


        public AltaCotizacion getCotizacion(int cod_cia, int cod_ramo, int cod_modalidad, string numCotizacion)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".GET_POLIZAS_P2300067_POLIZA";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, cod_cia);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, cod_ramo);
                    // cmd.AgregarInParametro("p_cod_modalidad", OracleDbType.Int32, cod_modalidad);
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, numCotizacion);
                    cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                return getDatosCotizacion(objDataSet.Tables[0]);
                //}
            }
            catch (System.Exception _error)
            {
                //MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("",_error);
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getCotizacion: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getCotizacion() : " + idError + " - " + _error);
                throw new Exception("ERROR getCotizacion : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public AltaCotizacion getCotizacion(int cod_cia, int cod_ramo, string numCotizacion)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".GET_POLIZAS_P2300067_POLIZA";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, cod_cia);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, cod_ramo);
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, numCotizacion);
                    cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                return getDatosCotizacionEmision(objDataSet.Tables[0]);
                //}
            }
            catch (System.Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getCotizacion: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getCotizacion() : " + idError + " - " + _error);
                throw new Exception("ERROR getCotizacion : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }


        public AltaCotizacion getDatosCotizacion(DataTable tabla)
        {
            try
            {

                AltaCotizacion datos = new AltaCotizacion();
                foreach (DataRow row in tabla.Rows)
                {
                    datos.numCotizacion = Convert.ToInt64(row[0].ToString());
                    datos.fechaCotizacion = row[1].ToString();
                    datos.msgJson = row[2].ToString();
                }

                return datos;
            }
            catch (Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDatosCotizacion: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getDatosCotizacion() : " + idError + " - " + _error);
                throw new Exception("ERROR getDatosCotizacion : " + idError + " - " + _error.Message);
            }
        }

        public AltaCotizacion getDatosCotizacionEmision(DataTable tabla)
        {
            try
            {

                AltaCotizacion datos = new AltaCotizacion();
                foreach (DataRow row in tabla.Rows)
                {
                    datos.numCotizacion = Convert.ToInt64(row[0].ToString());
                    datos.fechaCotizacion = row[1].ToString();
                    datos.msgJson = row[2].ToString();
                }

                return datos;
            }
            catch (Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDatosCotizacionEmision: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getDatosCotizacionEmision() : " + idError + " - " + _error);
                throw new Exception("ERROR getDatosCotizacionEmision : " + idError + " - " + _error.Message);
            }
        }

        public void updateJsonBenef(string json, string num_poliza)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".UPDATE_JSON_BENEF";
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Int32, num_poliza);
                    cmd.AgregarInParametro("p_msg_json_benef", OracleDbType.Clob, json);
                    cmd.AgregarOutParametro("p_msg_result", OracleDbType.Varchar2, 10);

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
            }
            catch (System.Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("updateJsonBenef: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR updateJsonBenenf() : " + idError + " - " + _error);
                throw new Exception("ERROR updateJsonBenef : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }


        public byte[] obtienePdfCotizacion(string modalidad, string numCotizacion, string fecNacimiento,
            string sexo, string correo, string moneda, string plazo, string primaIni, string primaAdd,
            string frecuenciaPrima, string perfil, string pctInversion1, string pctInversion2, string pctInversion3)
        {
            //string aportacionesAdd = "0";
            string rendimientoProy = "0";
            string fondoAcum = "0";
            string mntoInversion = primaIni;
            string edad = "";
            string sex = "";

            try
            {

                if (sexo.Equals("M"))
                {
                    sex = "MASCULINO";
                }
                else
                {
                    sex = "FEMENINO";
                }

                string[] fecha = fecNacimiento.Split('/');

                DateTime fecNac = new DateTime(Convert.ToInt16(fecha[2]), Convert.ToInt16(fecha[1]), Convert.ToInt16(fecha[0]));
                DateTime fecActual = DateTime.Now;

                //int anios = fecActual.Year - fecNac.Year;
                var dateSpan = DateTimeSpan.CompareDates(fecNac, fecActual);
                int anios = dateSpan.Years;

                edad = anios.ToString();
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("fecNac: " + fecNac, null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("fecActual: " + fecActual, null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("calculo edad: " + edad, null);

                string montoInv1 = (Convert.ToDouble(primaIni) * (Convert.ToDouble(pctInversion1) / 100)).ToString();
                string montoInv2 = (Convert.ToDouble(primaIni) * (Convert.ToDouble(pctInversion2) / 100)).ToString();
                string montoInv3 = (Convert.ToDouble(primaIni) * (Convert.ToDouble(pctInversion3) / 100)).ToString();

                //var response = new HttpResponseMessage();
                byte[] yourByteArray = null;

                XMLCotizacionHPExstream _XMLCotizacionHPExstream = new Controllers.API.XMLCotizacionHPExstream();
                string Xml64cc = _XMLCotizacionHPExstream.getXMLPrintCotizacionHP(modalidad, numCotizacion, edad, sex, correo, moneda, plazo, primaIni, primaAdd, frecuenciaPrima, perfil, pctInversion1, montoInv1, pctInversion2, montoInv2, pctInversion3, montoInv3, primaAdd, rendimientoProy, fondoAcum);

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("XML: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(Xml64cc, null);

                yourByteArray = System.Text.Encoding.UTF8.GetBytes(Xml64cc);
                Xml64cc = Convert.ToBase64String(yourByteArray);

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("XML BASE64: ", null);
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir(Xml64cc, null);

                //configuracion HP
                string sPubFile = ConfigurationManager.AppSettings["PubFileCotAut"];
                string referencias = ConfigurationManager.AppSettings["refImpAut"];

                string RutaURL = ConfigurationManager.AppSettings["URLWSHpExtream"];//url servicio hp

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(RutaURL);
                webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                HttpWebRequest request = webRequest;

                XmlDocument soapEnvelopeXml = new XmlDocument();
                //<?xml version=""1.0"" encoding=""utf-8""?>
                soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:eng=""urn:hpexstream-services/Engine"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                                       <soapenv:Header>
                                          <wsse:Security soapenv:mustUnderstand=""0"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                                             <wsse:UsernameToken wsu:Id=""UsernameToken-1"">
                                                <wsse:Username>" +
                                                ConfigurationManager.AppSettings["userHP"] +
                                                @"</wsse:Username>
                                                <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">" +
                                                ConfigurationManager.AppSettings["passHP"] +
                                                @"</wsse:Password>
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
                                                   <driver>" + Xml64cc + @"</driver>
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
                                                   <value>" + referencias + @"</value>
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

                using (WebResponse wresponse = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(wresponse.GetResponseStream()))
                    {

                        string soapResult = rd.ReadToEnd();

                        int iCadenaIni = soapResult.IndexOf("<fileOutput>") + 12;
                        int iCadenaFin = soapResult.IndexOf("</fileOutput>");

                        string sFileOutput = soapResult.Substring(iCadenaIni, (iCadenaFin - iCadenaIni));

                        //genera el pdf en base a la cadena de 64 bits
                        yourByteArray = Convert.FromBase64String(sFileOutput);
                    }
                }
                return yourByteArray;
            }
            catch (Exception ex)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("HP: ", ref idError, ex);
                System.Diagnostics.Debug.WriteLine("ERROR HP : " + idError + " - " + ex);
                throw new Exception("Error : " + idError + " - " + ex.Message);
            }
        }

        public Hashtable AltaBeneficiario(DatosContratante.DatosBeneficiario datosBeneficiario)
        {
            CLMFisica objPersonaFisica = null;
            CLMMoral objPersonaMoral = null;
            Hashtable datosBenef = null;

            string tipoPersona = datosBeneficiario.tipoPersona.ToString();

            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario: ", null);

            if (datosBeneficiario != null)
            {
                // Asignar valores al objeto objPersona
                if (datosBeneficiario.tipoPersona.ToString().Equals("1"))
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario FISICA: ", null);
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Fecha FISICA: " + datosBeneficiario.fechaNacimientoBeneficiario, null);


                    objPersonaFisica = new CLMFisica();
                    objPersonaFisica.TIP_PERSONA = TipoPersona.Fisica;
                    objPersonaFisica.NOM_TERCERO = datosBeneficiario.nombreDeBeneficiario;
                    objPersonaFisica.APE1_TERCERO = datosBeneficiario.apellidoPaternoBeneficiario;
                    objPersonaFisica.APE2_TERCERO = datosBeneficiario.apellidoMaternoBeneficiario;
                    objPersonaFisica.COD_PARENTESCO = datosBeneficiario.idParentescoBeneficiarioMapfre;

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Fecha FISICA 2: ", null);

                    objPersonaFisica.FEC_NACIMIENTO = datosBeneficiario.fechaNacimientoBeneficiario;

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("Fecha FISICA 3: ", null);

                    datosBenef = getDatosBenef(AltaBeneficiario(objPersonaFisica, datosBeneficiario));

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario FIN FISICA: ", null);
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("FECHA FISICA: " + objPersonaFisica.FEC_NACIMIENTO, null);


                }
                else
                {
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario MORAL: ", null);

                    objPersonaMoral = new CLMMoral();
                    objPersonaMoral.TIP_PERSONA = TipoPersona.Moral;
                    objPersonaMoral.RAZON_SOCIAL = datosBeneficiario.nombreDeBeneficiario;
                    objPersonaMoral.COD_PARENTESCO = datosBeneficiario.idParentescoBeneficiarioMapfre;
                    objPersonaMoral.FEC_CONSTITUCION = datosBeneficiario.fechaNacimientoBeneficiario;
                    datosBenef = getDatosBenef(AltaBeneficiario(objPersonaMoral, datosBeneficiario));

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario FIN MORAL: ", null);

                }
            }
            return datosBenef;
        }

        public DataRow AltaBeneficiario(CLMPersona objPersona, DatosContratante.DatosBeneficiario ben)
        {
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario BD: ", null);

            if (objPersona != null)
            {
                DataSet objDataSet = new DataSet();
                OracleConnection conexion = null;
                CLMFisica objFisica = new CLMFisica();
                CLMMoral objMoral = new CLMMoral();
                DataRow objDR;

                try
                {
                    Conexion _Conexion = new Conexion();

                    if (objPersona.TIP_PERSONA == TipoPersona.Fisica)
                    {
                        objFisica = (CLMFisica)objPersona;
                    }
                    else
                    {
                        objMoral = (CLMMoral)objPersona;
                    }


                    DataRow dr = new CotizarDao().validaCP(ben.cpBeneficiario, ben.nacionalidadBeneficiario);

                    string codEstado = dr.ItemArray[0].ToString();
                    string codProv = dr.ItemArray[1].ToString();

                    using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                    {
                        conexion.Open();
                        var cmd = new Comando();

                        cmd.Connection = conexion;
                        cmd.CommandText = "dc_k_clm_gestion.p_alta_beneficiario";
                        cmd.AgregarInOutParametro("p_tip_docum", OracleDbType.Varchar2, 10, DBNull.Value);
                        cmd.AgregarInOutParametro("p_cod_docum", OracleDbType.Varchar2, 30, DBNull.Value);
                        cmd.AgregarInParametro("p_mca_fisico", OracleDbType.Varchar2, (objPersona.TIP_PERSONA == TipoPersona.Fisica ? "S" : "N"));
                        cmd.AgregarInParametro("p_nom_tercero", OracleDbType.Varchar2, (objPersona.TIP_PERSONA == TipoPersona.Fisica ? objFisica.NOM_TERCERO.ToUpper() : objMoral.RAZON_SOCIAL.ToUpper()));
                        cmd.AgregarInParametro("p_ape1_tercero", OracleDbType.Varchar2, objFisica.APE1_TERCERO.ToUpper());
                        cmd.AgregarInParametro("p_ape2_tercero", OracleDbType.Varchar2, objFisica.APE2_TERCERO == null ? "" : objFisica.APE2_TERCERO.ToUpper());
                        cmd.AgregarInParametro("p_cod_parentesco", OracleDbType.Int32, (objPersona.TIP_PERSONA == TipoPersona.Fisica ? objFisica.COD_PARENTESCO : objMoral.COD_PARENTESCO));

                        DateTime dt = new DateTime();
                        DateTime.TryParseExact(ben.fechaNacimientoBeneficiario, "dd/MM/yyyy", null, DateTimeStyles.None, out dt);

                        cmd.AgregarInParametro("p_fec_nacimiento", OracleDbType.Date, dt);
                        cmd.AgregarInParametro("p_nom_domicilio1", OracleDbType.Varchar2, ben.domicilioBeneficiarioCalle.ToUpper());
                        cmd.AgregarInParametro("p_nom_domicilio2", OracleDbType.Varchar2, "");
                        cmd.AgregarInParametro("p_num_exterior", OracleDbType.Varchar2, ben.domicilioBeneficiarioNumExt);
                        cmd.AgregarInParametro("p_num_interior", OracleDbType.Varchar2, ben.domicilioBeneficiarioNumInt);
                        cmd.AgregarInParametro("p_nom_colonia", OracleDbType.Varchar2, ben.domicilioBeneficiarioNomColonia.ToUpper());
                        cmd.AgregarInParametro("p_cod_pais", OracleDbType.Varchar2, ben.nacionalidadBeneficiario); //pais-residencia
                        cmd.AgregarInParametro("p_cod_estado", OracleDbType.Varchar2, codEstado);
                        cmd.AgregarInParametro("p_cod_prov", OracleDbType.Varchar2, codProv);
                        cmd.AgregarInParametro("p_cod_postal", OracleDbType.Varchar2, ben.cpBeneficiario);

                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("EJECUTA PROCEDURE BD: ", null);
                        objDR = cmd.EjecutarRegistroSP();
                        MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("EJECUTA PROCEDURE BD FIN: ", null);

                    }

                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("FIN AltaBeneficiario BD: ", null);

                    return objDR;
                    //}
                }
                catch (System.Exception _error)
                {
                    
                    MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("AltaBeneficiario: ", ref idError, _error);
                    System.Diagnostics.Debug.WriteLine("ERROR AltaBeneficiario() : " + idError + " - " + _error);
                    throw new Exception("ERROR AltaBeneficiario : " + idError + " - " + _error.Message);
                }
                finally
                {
                    if (conexion != null && conexion.State != ConnectionState.Closed)
                    {
                        conexion.Close();
                        conexion.Dispose();
                    }

                }
            }
            else
            {
                return null;
            }
        }

        public Hashtable getDatosBenef(DataRow row)
        {
            Hashtable datos = new Hashtable();
            if (row != null)
            {
                datos.Add("tip_docum", row[0].ToString());
                datos.Add("cod_docum", row[1].ToString());
            }
            return datos;
        }

        public string getCodDocum()
        {
            string COD = "";
            string codDocum = "";
            DataRow dr;
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "tron2000.dc_k_clm_control_mmx.p_obtiene_cod_identificador";
                    cmd.AgregarInOutParametro("p_tip_identificador", OracleDbType.Varchar2, 10, "CLM");
                    cmd.AgregarInOutParametro("p_cod_identificador", OracleDbType.Varchar2, 20, null);

                    dr = cmd.EjecutarRegistroSP();

                }
                return dr["p_cod_identificador"].ToString();
                //}
            }
            catch (System.Exception _error)
            {
                
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getCodDocum: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getCodDocum() : " + idError + " - " + _error);
                throw new Exception("ERROR getCodDocum : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public void saveNoExterior(DatosContratante.Datos datos, string codDocum)
        {

            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "tron2000.dc_k_clm_gestion_mmx.p_inserta_g1001330";
                    cmd.AgregarInParametro("p_tip_docum", OracleDbType.Varchar2, "CLM");
                    cmd.AgregarInParametro("p_cod_docum", OracleDbType.Varchar2, codDocum);
                    cmd.AgregarInParametro("p_rep_legal", OracleDbType.Varchar2, "");
                    cmd.AgregarInParametro("p_tip_identi", OracleDbType.Varchar2, datos.msgJson.tipoIdentificacion);
                    cmd.AgregarInParametro("p_num_identi", OracleDbType.Varchar2, datos.msgJson.numeroIdentificacion);
                    cmd.AgregarInParametro("p_num_cot", OracleDbType.Varchar2, "");
                    cmd.AgregarInParametro("p_pais_nac", OracleDbType.Varchar2, datos.msgJson.paisNacimientoContratante);
                    cmd.AgregarInParametro("p_cod_usr", OracleDbType.Varchar2, "");
                    cmd.AgregarInParametro("p_cod_fea", OracleDbType.Varchar2, datos.msgJson.fea);
                    cmd.AgregarInParametro("p_num_exterior", OracleDbType.Varchar2, datos.msgJson.domicilioContratanteNumExt);
                    cmd.AgregarInParametro("p_num_interior", OracleDbType.Varchar2, datos.msgJson.domicilioContratanteNumInt);
                    cmd.AgregarInParametro("p_num_exterior_com", OracleDbType.Varchar2, "");
                    cmd.AgregarInParametro("p_num_interior_com", OracleDbType.Varchar2, "");
                    cmd.AgregarInParametro("p_num_folio_mercantil", OracleDbType.Varchar2, "");

                    cmd.EjecutarSP();

                }

            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("saveNoExterior: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR saveNoExterior() : " + idError + " - " + _error);
                throw new Exception("ERROR saveNoExterior : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }



        public string getNumIdentificador(string noPoliza)
        {

            DataRow dr;
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "tron2000.gc_k_validaciones_facelect_mmx.gc_p_algoritmo_bancomer";
                    cmd.AgregarInParametro("p_poliza", OracleDbType.Varchar2, noPoliza);
                    cmd.AgregarOutParametro("p_digito_ver", OracleDbType.Varchar2, 10);
                    cmd.AgregarOutParametro("p_error", OracleDbType.Varchar2, 30);


                    dr = cmd.EjecutarRegistroSP();

                }
                return dr["p_digito_ver"].ToString();
                //}
            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getNumIdentificador: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getNumIdentificador() : " + idError + " - " + _error);
                throw new Exception("ERROR getNumIdentificador : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public DataRow validaCP(string codPostal, string codPais)
        {

            DataRow dr;
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = " tron2000.dc_k_clm_gestion_mmx.p_valida_cp";
                    cmd.AgregarInParametro("p_cod_pais", OracleDbType.Varchar2, codPais);
                    cmd.AgregarInParametro("p_cod_postal", OracleDbType.Varchar2, codPostal);
                    cmd.AgregarOutParametro("p_cod_estado", OracleDbType.Int64, 15);
                    cmd.AgregarOutParametro("p_cod_prov", OracleDbType.Int64, 15);


                    dr = cmd.EjecutarRegistroSP();

                }
                return dr;

            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("validaCP: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR validaCP() : " + idError + " - " + _error);
                throw new Exception("ERROR validaCP : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public void marcaProvisional(string noPoliza, string codCia)
        {

            DataRow dr;
            OracleConnection conexion = null;
            MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("En marcaProvisional, noPoliza: " + noPoliza + " codCia: " + codCia, null);

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "Tron2000.ev_k_productos_vida_mmx.p_gen_ctrl_tec_millon";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Varchar2, codCia);
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, noPoliza);

                    cmd.EjecutarRegistroSP();

                }
            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("marcaProvisional: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR marcaProvisional() : " + idError + " - " + _error);
                throw new Exception("ERROR marcaProvisional : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public void updatePolizaEmitida(string noCotizacion, string noPolzia)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".UPDATE_POLIZA_EMITIDA";
                    cmd.AgregarInParametro("p_num_cotizacion", OracleDbType.Int32, noCotizacion);
                    cmd.AgregarInParametro("p_num_poliza_emite", OracleDbType.Clob, noPolzia);

                    cmd.EjecutarRegistroSP();
                }
            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("updatePolizaEmitida: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR updatePolizaEmitida() : " + idError + " - " + _error);
                throw new Exception("ERROR updatePolizaEmitida : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public DataTable getDatosPoliza(string noCotizacion)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".GET_POLIZA_EMITIDA";
                    cmd.AgregarInParametro("p_num_cotizacion", OracleDbType.Varchar2, noCotizacion);
                    cmd.AgregarReturnParametro("results", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();

                    return objDataSet.Tables[0];
                }
            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDatosPoliza: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getDatosPoliza() : " + idError + " - " + _error);
                throw new Exception("ERROR getDatosPoliza : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public DataTable getDistribucionFondos(string noPoliza)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".get_distribucion_fondos";
                    cmd.AgregarInParametro("p_num_cotizacion", OracleDbType.Varchar2, noPoliza);
                    cmd.AgregarReturnParametro("results", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();

                    return objDataSet.Tables[0];
                }
            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDistribucionFondos: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getDistribucionFondos() : " + idError + " - " + _error);
                throw new Exception("ERROR getDistribucionFondos : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public double getDeducciones(long noPoliza)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = strPaqueteUnit + ".get_deducciones";
                    cmd.AgregarInParametro("p_num_cotizacion", OracleDbType.Varchar2, noPoliza);
                    cmd.AgregarReturnParametro("results", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();

                    return getDeducciones(objDataSet.Tables[0]);
                }
            }
            catch (System.Exception _error)
            {

                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDeducciones: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getDeducciones() : " + idError + " - " + _error);
                throw new Exception("ERROR getDeducciones : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }

        public double getDeducciones(DataTable dt)
        {

            string deducciones = "";

            foreach (DataRow row in dt.Rows)
            {
                deducciones = row[0].ToString();

            }

            return Convert.ToDouble(deducciones);

        }

        public string getFecAportacion(string periodo, string diaCobro)
        {
            DateTime dt = DateTime.Now;
            string fecAportacion;
            int periodoMes = 0;
            string[] arrFecha;
            string dia;

            if (periodo.Equals("MENSUAL"))
            {
                periodoMes = 1;
            }
            else if (periodo.Equals("TRIMESTRAL"))
            {
                periodoMes = 3;
            }
            else if (periodo.Equals("SEMESTRAL"))
            {
                periodoMes = 6;
            }
            else if (periodo.Equals("ANUAL MULTIANUAL"))
            {
                periodoMes = 12;
            }

            dt = dt.AddMonths(periodoMes);

            fecAportacion = dt.ToString("dd/MM/yyyy");

            //Se valida que el dia sugerido no sea mayor al ultimo dia del mes
            arrFecha = fecAportacion.Split('/');

            int maxDia = DateTime.DaysInMonth(dt.Year, dt.Month);

            if (Convert.ToInt64(diaCobro) > maxDia)
            {
                diaCobro = maxDia.ToString();
            }

            if (Convert.ToInt64(diaCobro) < 10)
            {
                diaCobro = "0" + diaCobro;
            }

            //se reemplaza el dia por el dia sugerido

            fecAportacion = arrFecha[0].ToString().Replace(arrFecha[0].ToString(), diaCobro) + arrFecha[1].ToString() + arrFecha[2].ToString();


            return fecAportacion;

        }

        public int? getConsecutivoCta(string codCia, string codDocum)
        {
            string COD = "";
            DataRow dr;
            OracleConnection conexion = null;

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "tron2000.ev_k_112_unit_mmx.p_get_consec_cta";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, codCia);
                    cmd.AgregarInParametro("p_cod_docum", OracleDbType.NVarchar2, codDocum);
                    cmd.AgregarOutParametro("p_num_consec", OracleDbType.Int64, 10);

                    dr = cmd.EjecutarRegistroSP();

                }

                int result = 0;

                if (int.TryParse(dr["p_num_consec"].ToString(), out result))
                    return result;
                else
                    return null;
                //}
            }
            catch (System.Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getConsecutivoCta: ", ref idError, _error);
                System.Diagnostics.Debug.WriteLine("ERROR getConsecutivoCta() : " + idError + " - " + _error);
                throw new Exception("ERROR getConsecutivoCta : " + idError + " - " + _error.Message);
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }
        }
    }
}