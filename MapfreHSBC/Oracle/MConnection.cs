/**********************************************************************************
* @author                        :  Daniel Ramírez Herrera
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net 
* Name of the File               :  MConexion.cs
* Creation/Modification History  :
*                   18-Enero-2005     Creada 
*
* Sample Overview:
* 
* 
**********************************************************************************/
using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace MapfreWebCore.Oracle
{
    /// <summary>
    /// Clase que provee de conexiones utilizando el ODP.NET
    /// </summary>
    public class MConnection
    {
        public MConnection()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Método que se utiliza para obtener conexiones
        /// </summary>
        /// <param name="ConnectionString">Cadena de conexión</param>
        /// <returns>Una conexión a Oracle abierta</returns>
        public static OracleConnection getConexion(string ConnectionString)
        {
            OracleConnection conexion = null;
            try
            {
                conexion = new OracleConnection();
                conexion.ConnectionString = ConnectionString;
                conexion.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("MConnection.getConexion(): " + ex.Message);
            }
            return conexion;
        }        
    }
}

