using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace MapfreWebCore.Oracle
{
    internal class MOracleConnection
    {
        private OracleConnection con;
        private string sGUI;
        private StateChangeEventHandler handler;

        public MOracleConnection()
        {

        }

        public OracleConnection getConexion(string ConnectionString) 
        {
            if (ConnectionString != null)
            {
                con = new OracleConnection();
                con.ConnectionString = ConnectionString;                
                con.Open();
                ConnectionSpy(con);
                return con;                
            }
            else
            {
                throw new Exception("ConnectionString is NULL");
            }
        }

        public OracleConnection Conexion
        {
            get { return con; }
        }

        public string GUI
        {
            set { sGUI = value; }
            get { return sGUI; }
        }

        private void ConnectionSpy(OracleConnection con)
        {
            handler = new StateChangeEventHandler(StateChange);
            con.StateChange += handler;
            GC.SuppressFinalize(con);
        }

        private void StateChange(Object sender, System.Data.StateChangeEventArgs args)
        {
            if (args.CurrentState == ConnectionState.Closed)
            {
                DedicatedConnection.getInstance().Release(sGUI);
                GC.SuppressFinalize(this); 
                con.StateChange -= handler; 
                con = null;
            }
        }

        ~MOracleConnection()    
        {      
            //if we got here then the connection was not closed.
            if (con != null)
            {
                con.Close();                
            }
        }
    }
}
