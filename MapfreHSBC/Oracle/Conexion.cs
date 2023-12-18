// ***********************************************************************
// Ensamblado				: MapfreWebCore
// Autor           			: Israel Lerma Hernández
// Creado          			: 07-20-2015
// IDE						: Microsoft Visual Studio .Net 2013
//
// Ultima modificación por	: Israel Lerma Hernández
// Ultima modificación en 	: 08-14-2015
// ***********************************************************************
// <copyright file="Conexion.cs" company="MAPFRE">
//     Copyright (c) MAPFRE. All rights reserved.
// </copyright>
// <summary>
// Manejo de conexiones Oracle 
// </summary>
// ***********************************************************************

using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace MapfreWebCore.Oracle
{
    /// <summary>
    /// Clase para utilizar Conexión a Base de Datos
    /// </summary>
    public class Conexion
	{
        /// <summary>
        /// Obtiene una Conexión en base a un identificador de conexión dentro de los AppSettings y la abre
        /// </summary>
        /// <param name="idConexion">Identificador de la cadena de conexión en el web.config</param>
        /// <returns>OracleConnection con la cadena de conexión definida con el identificador 
        /// idConexion em la configuración de la aplicación </returns>
        /// <exception cref="System.Exception">
        /// La fuente de datos -- idConexion -- solicitada no existe
        /// o
        /// Error Conexion.GetConexionId(): Exception.Message
        /// </exception>
		public  OracleConnection GetConexionId(string idConexion)
		{
			OracleConnection conexion = null;			
			try 
			{
                if (ConfigurationManager.AppSettings[idConexion] != null) 
				{
				    conexion = new OracleConnection
				    {
                        ConnectionString = ConfigurationManager.AppSettings[idConexion].ToString()
				    };
				}
				else 
				{					
					throw new Exception("La fuente de datos --"+ idConexion +"-- solicitada no existe");
				}
			}
			catch(Exception ex) 
			{				
				throw new Exception("Error Conexion.GetConexionId(): " + ex.Message, ex);
			}
			return conexion;
		}
        /// <summary>
        /// Obtiene una Conexión en base a una cadena de conexión y la abre
        /// </summary>
        /// <param name="conexionString">String con la cadena de conexión</param>
        /// <returns>OracleConnection.</returns>
        /// <exception cref="System.Exception">Error Conexion.GetConexion(): Exception.Message</exception>
        public static OracleConnection GetConexion(string conexionString)
        {
            OracleConnection conexion = null;
            try
            {
                conexion = new OracleConnection {ConnectionString = conexionString};
            }
            catch (Exception ex)
            {
                throw new Exception("Error Conexion.GetConexion(): " + ex.Message, ex);
            }
            return conexion;
        }
	}
}
