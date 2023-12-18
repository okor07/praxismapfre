using MapfreHSBC.Models.Cotizacion;
using MapfreHSBC.Models.General;
using MapfreWebCore.Oracle;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace MapfreHSBC.Models
{
    public class ConsultaDao
    {
        string strPaqueteUnit = ConfigurationManager.AppSettings["EsquemaUnit"].ToString();
        string idError;

        public IEnumerable<String> getPaquetes()
        {
            List<String> paquetes = new List<String>();
            paquetes.Add("Unit Linked");
             return paquetes;
        }

        public IEnumerable<ConsultasCotizaciones> getConsultaCotizaciones(long idCotizacion, int cod_modalidad, string email)
        {
            List<ConsultasCotizaciones> consulta = new List<ConsultasCotizaciones>();

            IEnumerable<ConsultasCotizaciones> cotizaciones;

            try
            {


                if (idCotizacion != 0)
                {
                    cotizaciones = getJsonCotizaciones(1, 112, cod_modalidad, idCotizacion.ToString());
                }
                else
                {
                    cotizaciones = getIdCotizaciones(1, 112, cod_modalidad, email);
                }



                foreach (ConsultasCotizaciones numCotiza in cotizaciones)
                {
                    consulta.Add(
                    new ConsultasCotizaciones()
                    {
                        idCotizacion = Convert.ToInt64(numCotiza.idCotizacion),
                        fecCotizacion = numCotiza.fecCotizacion,
                        fecNacimiento = numCotiza.fecNacimiento,
                        montoAportacionAd = numCotiza.montoAportacionAd,
                        perAportacionAd = numCotiza.perAportacionAd
                    });
                }

                return consulta;
            }
            catch(Exception _ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getConsultaCotizaciones: ", ref idError, _ex);
                System.Diagnostics.Debug.WriteLine("ERROR ConsultaDao.getConsultaCotizaciones() : " + _ex);
                throw new Exception("ERROR ConsultaDao.getConsultaCotizaciones() : " + idError, _ex.InnerException);
            }
        }

        public IEnumerable<ConsultasCotizaciones> getIdCotizaciones(int cod_cia, int cod_ramo, int cod_modalidad, string email)
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
                    cmd.CommandText = strPaqueteUnit + ".GET_POLIZAS_P2300067_EMAIL";
                    cmd.AgregarInParametro("p_cod_cia", OracleDbType.Int32, cod_cia);
                    cmd.AgregarInParametro("p_cod_ramo", OracleDbType.Int32, cod_ramo);
                    cmd.AgregarInParametro("p_cod_modalidad", OracleDbType.Int32, cod_modalidad);
                    cmd.AgregarInParametro("p_email", OracleDbType.Varchar2, email);                    
                    cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                return getDatos(objDataSet.Tables[0]);

            }
            catch (System.Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getIdCotizaciones: ", ref idError, _error);
                
                throw new Exception("Error : " + idError + " - " + _error.Message);
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

        public IEnumerable<ConsultasCotizaciones> getJsonCotizaciones(int cod_cia, int cod_ramo, int cod_modalidad, string num_poliza)
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
                    cmd.AgregarInParametro("p_cod_modalidad", OracleDbType.Int32, cod_modalidad);
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, num_poliza);
                    cmd.AgregarReturnParametro("result", OracleDbType.RefCursor, 0);

                    objDataSet = cmd.EjecutarRefCursorSP();
                }
                return getDatos(objDataSet.Tables[0]);

            }
            catch (System.Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getJsonCotizaciones: ", ref idError, _error);
                throw new Exception("Error getJsonCotizaciones : " + idError, _error.InnerException);
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

        public IEnumerable<ConsultasCotizaciones> getDatos(DataTable tabla)
        {

            List<ConsultasCotizaciones> consulta = new List<ConsultasCotizaciones>();

            string[] json;
            string[] fechaNac;
            string[] monto;
            string[] periodicidad;
            string[] fecCotizacion;

            foreach(DataRow row in tabla.Rows){

                json = row[2].ToString().Replace('"', ' ').Split(',');
                //Extraccion de fecha de nacimiento del json
                fechaNac = json[4].Replace('"', ' ').Split(':');
                //Extraccion de monto de aportacion
                monto = json[8].Replace('"', ' ').Split(':');
                //Extraccion de monto de periodicidad
                periodicidad = json[13].Replace('"', ' ').Split(':');

                fecCotizacion = row[1].ToString().Split(' ');

                consulta.Add(
                    new ConsultasCotizaciones()
                    {
                        idCotizacion = Convert.ToInt64(row[0].ToString()),
                        fecCotizacion = fecCotizacion[0],
                        fecNacimiento = fechaNac[1],
                        montoAportacionAd = Convert.ToDecimal(monto[1]).ToString("C2"),  //new Random().Next(0, 100000).ToString(),
                        perAportacionAd = periodicidad[1]//"Mensual"
                    });

            }

            return consulta;
        }

        public InformacionCP.Respuesta getCP(long idTransaccion, string cp)
        {
            DataSet objDataSet = new DataSet();
            OracleConnection conexion = null;
            InformacionCP.Respuesta resp = new InformacionCP.Respuesta();

            try
            {
                Conexion _Conexion = new Conexion();

                using (conexion = _Conexion.GetConexionId("ConnectionTW"))
                {
                    conexion.Open();
                    var cmd = new Comando();

                    cmd.Connection = conexion;
                    cmd.CommandText = "ev_k_consulta_poliza_112.p_ret_info_cp";
                    cmd.AgregarInParametro("p_cod_postal", OracleDbType.Varchar2, cp);

                    cmd.AgregarOutParametro("p_cod_prov", OracleDbType.Int32, 8);
                    cmd.AgregarOutParametro("p_nom_prov", OracleDbType.Varchar2, 200);
                    cmd.AgregarOutParametro("p_cod_localidad", OracleDbType.Int32, 8);
                    cmd.AgregarOutParametro("p_nom_localidad", OracleDbType.Varchar2, 200);
                    cmd.AgregarOutParametro("p_cod_estado", OracleDbType.Int32, 8);
                    cmd.AgregarOutParametro("p_nom_estado", OracleDbType.Varchar2, 200);
                    cmd.AgregarOutParametro("p_cod_pais", OracleDbType.Varchar2, 200);
                    cmd.AgregarOutParametro("p_nom_pais", OracleDbType.Varchar2, 200);
                    cmd.AgregarOutParametro("p_cod_error", OracleDbType.Int32, 8);
                    cmd.AgregarOutParametro("p_des_error", OracleDbType.Varchar2, 200);


                    objDataSet = cmd.EjecutarRefCursorSP();

                    int id;

                    if (objDataSet != null && objDataSet.Tables[0] != null && objDataSet.Tables[0].Rows.Count > 0 && objDataSet.Tables[0].Rows[0].ItemArray[8].ToString() == "0")
                    {
                        resp.msgJson = new InformacionCP.MsgJsonRespuesta();

                        if (objDataSet.Tables[0].Rows[0].ItemArray[0] != null &&
                           int.TryParse(objDataSet.Tables[0].Rows[0].ItemArray[0].ToString(), out id))
                            resp.msgJson.idPoblacion = id;
                        else
                            resp.msgJson.idPoblacion = 0;

                        resp.msgJson.descPoblacion = objDataSet.Tables[0].Rows[0].ItemArray[1] != null ? objDataSet.Tables[0].Rows[0].ItemArray[1].ToString() : "";

                        if (objDataSet.Tables[0].Rows[0].ItemArray[2] != null &&
                            int.TryParse(objDataSet.Tables[0].Rows[0].ItemArray[2].ToString(), out id))
                            resp.msgJson.idMunicipio = id;
                        else
                            resp.msgJson.idMunicipio = 0;

                        resp.msgJson.descMunicipio = objDataSet.Tables[0].Rows[0].ItemArray[3] != null ? objDataSet.Tables[0].Rows[0].ItemArray[3].ToString() : "";

                        if (objDataSet.Tables[0].Rows[0].ItemArray[4] != null &&
                            int.TryParse(objDataSet.Tables[0].Rows[0].ItemArray[4].ToString(), out id))
                            resp.msgJson.idEstado = id;
                        else
                            resp.msgJson.idEstado = 0;

                        resp.msgJson.descEstado = objDataSet.Tables[0].Rows[0].ItemArray[5] != null ? objDataSet.Tables[0].Rows[0].ItemArray[5].ToString() : "";

                        resp.msgJson.idPais = objDataSet.Tables[0].Rows[0].ItemArray[6] != null ? objDataSet.Tables[0].Rows[0].ItemArray[6].ToString() : "";
                        resp.msgJson.descPais = objDataSet.Tables[0].Rows[0].ItemArray[7] != null ? objDataSet.Tables[0].Rows[0].ItemArray[7].ToString() : "";

                        resp.msgJson.idTransaccion = idTransaccion;
                        resp.msgJson.cp = cp;
                    }
                    else if (objDataSet != null && objDataSet.Tables[0] != null && objDataSet.Tables[0].Rows.Count > 0 && objDataSet.Tables[0].Rows[0].ItemArray[8].ToString() != "0")
                    {
                        resp.codigoError = 1;
                        resp.descripcionError = objDataSet.Tables[0].Rows[0].ItemArray[8].ToString();
                    }
                    else
                    {
                        resp.codigoError = 1;
                        resp.descripcionError = "ERROR al realizar la consulta de CP";
                    }
                }
            }
            catch (Exception ex)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getCP:  ", ref idError, ex);
                resp.descripcionError = "ERROR al realizar la consulta de CP : " + idError;
                resp.codigoError = 1;
            }
            finally
            {
                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }

            }

            return resp;
        }
    }
}