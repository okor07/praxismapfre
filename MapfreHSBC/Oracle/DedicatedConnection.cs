/**********************************************************************************
* @author                        :  Daniel Ramírez Herrera
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net 
* Name of the File               :  DedicatedConnection.cs
* Creation/Modification History  :
*                   26-Julio-2007     Creada 
*
* Sample Overview:
* Conexion dedicada a base de datos.
* 
* 
**********************************************************************************/
using System;
using System.Data;
using System.Collections;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace MapfreWebCore.Oracle
{
    /// <summary>
    /// Conexion dedicada a base de datos.
    /// </summary>
    public class DedicatedConnection
    {
        private static DedicatedConnection Instance;
        private static Hashtable HashUserConnect;
        private static string sConnectionString;        
        /// <summary>
        /// Constructor privado para solo tener una sola instancia de la clase(SINGLETON).
        /// </summary>
        private DedicatedConnection()
        {
            HashUserConnect = new Hashtable();
            sConnectionString = null;
        }
        /// <summary>
        /// Genera la única instacia de esta clase.
        /// </summary>
        /// <returns>Un objeto del tipo DedicatedConnection único para toda la aplicación</returns>
        public static DedicatedConnection getInstance()
        {
            if (Instance == null)
            {
                Instance = new DedicatedConnection();
            }
            return Instance;
        }

        public OracleConnection getConexion(string SESSION_GUI)
        {
            OracleConnection conexion = null;
            MOracleConnection MCon = null;
            try
            {
                if (HashUserConnect.Contains(SESSION_GUI))
                {
                    MCon = (MOracleConnection)HashUserConnect[SESSION_GUI];
                    conexion = MCon.Conexion;                    
                }
                else
                {
                    MCon = new MOracleConnection();
                    MCon.GUI = SESSION_GUI;
                    conexion = MCon.getConexion(sConnectionString);
                    HashUserConnect.Add(SESSION_GUI, MCon);
                }                
            }
            catch (Exception ex)
            {
                throw new Exception("DedicatedConnection.getConexion(): " + ex.Message);
            }
            return conexion;
        }

        internal void Release(string SESSION_GUI)
        {
            if (SESSION_GUI != null)
            {
                HashUserConnect.Remove(SESSION_GUI);                
            }
        }

        public void ReleaseSession(string SESSION_GUI)
        {
            if (SESSION_GUI != null && HashUserConnect[SESSION_GUI] != null)
            {
                MOracleConnection MCon = (MOracleConnection)HashUserConnect[SESSION_GUI];
                OracleConnection conexion = MCon.Conexion;
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public bool ContainsSessionGUI(string SESSION_GUI)
        {
            return HashUserConnect.Contains(SESSION_GUI);                
        }

        public string ConnectionString
        {
            set { sConnectionString = value; }            
        }

        public int CountUsers
        {
            get { return HashUserConnect.Count; }
        }
        public ICollection GetConnectedUsers
        {
            get { return HashUserConnect.Keys; }
        }
    }
}