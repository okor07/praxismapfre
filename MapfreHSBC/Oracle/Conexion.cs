// ***********************************************************************
// Ensamblado				: MapfreWebCore
// Autor           			: Israel Lerma Hern�ndez
// Creado          			: 07-20-2015
// IDE						: Microsoft Visual Studio .Net 2013
//
// Ultima modificaci�n por	: Israel Lerma Hern�ndez
// Ultima modificaci�n en 	: 08-14-2015
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
    /// Clase para utilizar Conexi�n a Base de Datos
    /// </summary>
    public class Conexion
	{
        /// <summary>
        /// Obtiene una Conexi�n en base a un identificador de conexi�n dentro de los AppSettings y la abre
        /// </summary>
        /// <param name="idConexion">Identificador de la cadena de conexi�n en el web.config</param>
        /// <returns>OracleConnection con la cadena de conexi�n definida con el identificador 
        /// idConexion em la configuraci�n de la aplicaci�n </returns>
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
        /// Obtiene una Conexi�n en base a una cadena de conexi�n y la abre
        /// </summary>
        /// <param name="conexionString">String con la cadena de conexi�n</param>
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
