using MapfreHSBC.Models.Cotizacion;
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
    public class DetalleDao
    {
        string strPaqueteUnit = ConfigurationManager.AppSettings["EsquemaUnit"].ToString();
        string idError;
        public DetalleCotizacion getDetalleCotizacion(Int64 idCotizacion)
        {

            DetalleCotizacion detalle = new DetalleCotizacion()
            {
                //tipoPersona ="Fisica",
                //nombre = "Juan José de Jesus",
                //apMaterno = "Espinoza de los monteros",
                //apPaterno = "Perez de León",
                perfil = "Moderado",
                modalidad = "Moderado",
                moneda = "Nacional",
                plazo = "EA 60 años",
                coberturas = "Fallecimiento 1% y Muerte Accidental 20%",
                primaInicial = "10,000",
                aportacionesPeriodicas = 5000.00,
                periodicidad = "Trimestral",
                formaPago = "Contado",
                fondoUno = "50% - 560,000",
                fondoDos = "35% - 392,000",
                fondoTres = "15% - 168,000",
                precioSeguro = "11,000",
                derechoPoliza = 00.00,
                primaTotal = "11,000"
            };

            return detalle;
        }

        public DatosContratante.Datos getJsonBenef(string num_poliza)
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
                    cmd.CommandText = strPaqueteUnit + ".GET_MSG_JSON_BENF";
                    cmd.AgregarInParametro("p_num_poliza", OracleDbType.Varchar2, num_poliza);
                    cmd.AgregarOutParametro("p_msg_json_benef", OracleDbType.Clob, 10000);

                    objDataSet = cmd.EjecutarRefCursorSP();

                    return getDetalle(objDataSet.Tables[0]);
                }
            }
            catch (System.Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("getDetalle: ", ref idError, _error);
                throw new Exception("Error getDetalle : " + idError, _error.InnerException);
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

        public DatosContratante.Datos getDetalle(DataTable json)
        {
            try
            {
                string json2 = "";

                foreach (DataRow row in json.Rows)
                {
                     json2 = row[0].ToString();

                }


                DatosContratante.Datos detalle = Newtonsoft.Json.JsonConvert.DeserializeObject<DatosContratante.Datos>(json2);

                return detalle;
               
            }
            catch(Exception _error)
            {
                MapfreWebCore.Registros.RegistroArchivo.GetInstancia().Escribir("", ref idError, _error);
                throw new Exception("Error getDetalle : " + idError, _error.InnerException);
            }
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
                throw new Exception("Error getDetalle : " + idError, _error.InnerException);
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