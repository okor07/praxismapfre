// ***********************************************************************
// Ensamblado				: MapfreWebCore
// Autor           			: Israel Lerma Hernández
// Creado          			: 07-20-2015
// Powered by				: BW Meill Corporation
// IDE						: Microsoft Visual Studio .Net 2013
//
// Ultima modificación por	: Israel Lerma Hernández
// Ultima modificación en 	: 08-18-2015
// ***********************************************************************
// <copyright file="Comando.cs" company="MAPFRE">
//     Copyright (c) MAPFRE. All rights reserved.
// </copyright>
// <summary>
// Manejo de comandos de consultas Oracle   
// </summary>
// ***********************************************************************

using System;
using System.Text;
using System.Data;
using System.Collections;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MapfreWebCore.Oracle
{
    /// <summary>
    /// Clase que se utiliza para ejecutar sentencias SQL y SP de Oracle utilizando ODP.NET
    /// </summary>
    public class Comando
    {
        /// <summary>
        /// Lista con nombre de parámetros empleados en el Comando
        /// </summary>
        private ArrayList _nomParametros;
        /// <summary>
        /// Lista de parámetros y Valores
        /// </summary>
        private Hashtable _parametros;
        /// <summary>
        /// Resultado de la ejecución de un SP
        /// </summary>
        private DataTable _resultadoSP;
        /// <summary>
        /// Dueño de un objeto en Oracle
        /// </summary>
        private string _owner;
        /// <summary>
        /// Empleo de sinónimo
        /// </summary>
        private bool _bisSynonym;
        /// <summary>
        /// Bandera para Empleo de dbLink
        /// </summary>
        private string _dbLink;
        /// <summary>
        /// Constructor de la clase Commando
        /// </summary>
        public Comando()
        {
            _nomParametros = new ArrayList();
            _parametros = new Hashtable();
            _resultadoSP = new DataTable();
        }
        /// <summary>
        /// Ejecuta una sentencia SQL que regresa N registros
        /// </summary>
        /// <returns>Tabla con los datos solicitados</returns>
        /// <exception cref="System.Exception">Comando.EjecutarMultiRegistro() :  + ex.Message + \n + strComErr</exception>
        public DataTable EjecutarMultiRegistro()
        {
            OracleCommand cmd = null;
            OracleDataAdapter objAdapter = null;
            var objTabla = new DataTable("RESULTADO");
            try
            {
                cmd = new OracleCommand(CommandText, Connection) { CommandType = CommandType.Text };
                objAdapter = new OracleDataAdapter(cmd);
                objAdapter.Fill(objTabla);
            }
            catch (Exception ex)
            {
                var strComErr = CommandText;
                throw new Exception("Comando.EjecutarMultiRegistro() : " + ex.Message + "\n" + strComErr, ex);
            }
            finally
            {
                if (objAdapter != null)
                {
                    objAdapter.Dispose();
                    objAdapter = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
            return objTabla;
        }
        /// <summary>
        /// Ejecuta una sentencia SQL que solo regresa UN registro.
        /// <para>SELECT campo1, campo2, campo3 from tabla1 where ...</para>
        /// </summary>
        /// <returns>DataRow con los datos solicitados.
        /// Se pueden obtener los datos por nombre o por indice,
        /// outRow["campo1"],outRow["campo2"], outRow["campo3"]
        /// ó
        /// outRow[0], outRow[1], outRow[2]</returns>
        /// <exception cref="System.Exception">Comando.EjecutarRegistro() :  + ex.Message + \n + strComErr</exception>
        public DataRow EjecutarRegistro()
        {
            OracleCommand cmd = null;
            OracleDataAdapter objAdapter = null;
            var objTabla = new DataTable("RESULTADO");
            DataRow objRow = null;
            try
            {
                cmd = new OracleCommand(CommandText, Connection) { CommandType = CommandType.Text };
                objAdapter = new OracleDataAdapter(cmd);
                objAdapter.Fill(objTabla);

                if (objTabla.Rows.Count > 0)
                {
                    objRow = objTabla.Rows[0];
                }
            }
            catch (Exception ex)
            {
                var strComErr = CommandText;
                throw new Exception("Comando.EjecutarRegistro() : " + ex.Message + "\n" + strComErr, ex);
            }
            finally
            {
                if (objAdapter != null)
                {
                    objAdapter.Dispose();
                    objAdapter = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
            return objRow;
        }
        /// <summary>
        /// Ejecuta sentencias SQL que no regresan ningún dato. Como UPDATES e INSERTS.
        /// </summary>
        /// <exception cref="System.Exception">Comando.Ejecutar() :  + ex.Message + \n + strComErr</exception>
        public void Ejecutar()
        {
            OracleCommand cmd = null;
            try
            {
                cmd = new OracleCommand(CommandText, Connection) { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var strComErr = CommandText;
                throw new Exception("Comando.Ejecutar() : " + ex.Message + "\n" + strComErr, ex);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }
        /// <summary>
        /// Ejecuta PROCEDURE, no tiene ningún parámetro de salida.
        /// </summary>
        /// <returns>OracleParameterCollection Para obtener los valores en los parametros de tipo OUTPUT.</returns>
        /// <exception cref="System.Exception">Comando.EjecutarSP() :  + ex.Message + \n + objComErr.ToString()</exception>
        public OracleParameterCollection EjecutarSP()
        {
            OracleCommand cmd = null;
            OracleParameter parametro = null;
            OracleParameterCollection parametros = null;
            try
            {
                using (cmd = new OracleCommand(CommandText, Connection) { CommandType = CommandType.StoredProcedure })
                {

                    foreach (string nomParametro in _nomParametros)
                    {
                        parametro = (OracleParameter)_parametros[nomParametro];
                        cmd.Parameters.Add(parametro);
                    }
                    cmd.BindByName = true;
                    cmd.ExecuteNonQuery();
                    parametros = cmd.Parameters;
                }
            }
            catch (Exception ex)
            {
                var objComErr = new StringBuilder();
                try
                {
                    objComErr.Append(CommandText);
                    objComErr.Append("\nParametros\n");
                    foreach (string nomParametro in _nomParametros)
                    {
                        parametro = (OracleParameter)_parametros[nomParametro];
                        objComErr.Append(nomParametro);
                        objComErr.Append(" = ");
                        objComErr.Append(parametro.Value != DBNull.Value ? Convert.ToString(parametro.Value) : "NULL");
                        objComErr.Append(" : ");
                        objComErr.Append(Convert.ToString(parametro.Direction));
                        objComErr.Append("\n");
                    }
                }
                catch (Exception exErr)
                {
                    objComErr.Append(exErr.Message);
                }

                throw new Exception("Comando.EjecutarSP() : " + ex.Message + "\n" + objComErr.ToString(), ex);
            }

            return parametros;
        }


        /// <summary>
        /// Ejecuta PROCEDURE ó FUNCTION que no recesan parámetros del tipo REF_CURSOR.
        /// </summary>
        /// <returns>DataRow con los parámetros de salida.
        /// Los resultados se pueden obtener por nombre del parámetro o por indice.
        /// Por ejemplo:  PROCEDURE p_ejemplo(param1 IN  NUMBER,
        /// param2 OUT VARCHAR2,
        /// param3 OUT NUMBER);
        /// outRow["param2"], outRow["param3"]
        /// outRow[0], outRow[1]</returns>
        /// <exception cref="System.Exception">Comando.EjecutarRegistroSP() :  + ex.Message + \n + objComErr.ToString()</exception>
        public DataRow EjecutarRegistroSP()
        {
            OracleCommand cmd = null;
            DataRow objRow = null;
            OracleParameter parametro = null;
            OracleDate oracleDateAux;
            OracleClob oracleClobAux;
            OracleBlob oracleBlobAux;
            try
            {
                using (cmd = new OracleCommand(CommandText, Connection) { CommandType = CommandType.StoredProcedure })
                {

                    foreach (string nomParametro in _nomParametros)
                    {
                        parametro = (OracleParameter)_parametros[nomParametro];
                        cmd.Parameters.Add(parametro);
                    }
                    cmd.BindByName = true;
                    cmd.ExecuteNonQuery();

                    objRow = _resultadoSP.NewRow();

                    foreach (string nomParametro in _nomParametros)
                    {
                        parametro = (OracleParameter)_parametros[nomParametro];
                        if (parametro.Direction.Equals(ParameterDirection.Output) || parametro.Direction.Equals(ParameterDirection.InputOutput) || parametro.Direction.Equals(ParameterDirection.ReturnValue))
                        {
                            //Este IF no funciona en versiones de ODP.NET menores a 10.2
                            //Para versiones -10.2 se debe aplicar "parametro.Value != System.DBNull.Value"
                            if (parametro.Status != OracleParameterStatus.NullFetched)
                            {
                                if (parametro.OracleDbType.Equals(OracleDbType.Date))
                                {
                                    oracleDateAux = (OracleDate)parametro.Value;
                                    objRow[nomParametro] = oracleDateAux.Value;
                                }
                                else if (parametro.OracleDbType.Equals(OracleDbType.Clob))
                                {
                                    oracleClobAux = (OracleClob)parametro.Value;
                                    objRow[nomParametro] = oracleClobAux.Value;
                                }
                                else if (parametro.OracleDbType.Equals(OracleDbType.Blob))
                                {
                                    oracleBlobAux = (OracleBlob)parametro.Value;
                                    objRow[nomParametro] = oracleBlobAux.Value;
                                }
                                else
                                {
                                    objRow[nomParametro] = parametro.Value.ToString();
                                }
                            }
                        }
                    }
                    _resultadoSP.Rows.Add(objRow);
                }

            }
            catch (Exception ex)
            {
                var objComErr = new StringBuilder();
                try
                {
                    objComErr.Append(CommandText);
                    objComErr.Append("\nParametros\n");
                    foreach (string nomParametro in _nomParametros)
                    {
                        parametro = (OracleParameter)_parametros[nomParametro];
                        objComErr.Append(nomParametro);
                        objComErr.Append(" = ");
                        objComErr.Append(parametro.Value != DBNull.Value ? Convert.ToString(parametro.Value) : "NULL");
                        objComErr.Append(" : ");
                        objComErr.Append(Convert.ToString(parametro.Direction));
                        objComErr.Append("\n");
                    }
                }
                catch (Exception exErr)
                {
                    objComErr.Append(exErr.Message);
                }
                throw new Exception("Comando.EjecutarRegistroSP() : " + ex.Message + "\n" + objComErr.ToString(), ex);
            }

            return objRow;
        }
        /// <summary>
        /// Ejecuta PROCEDURE o FUNCTION que NO CONTIENE o CONTIENE al menos un parámetro de
        /// salida del tipo REF_CURSOR.
        /// </summary>
        /// <returns>Por cada parámetro REF_CURSOR de salida se obtendrá un DataTable en el DataSet de retorno.
        /// También se tendrá una tabla extra que contendra los parámetros de salida que no son del tipo REF_CURSOR.
        /// Por ejemplo:
        /// PROCEDURE(param1 IN NUMBER,
        /// param2 OUT VARCHAR2,
        /// param3 OUT NUMBER,
        /// param4 OUT DATE,
        /// param5 OUT REF_CURSOR);
        /// En este caso se tendrá un DataSet con dos Tablas, la primer tabla contendrá un renglón con los
        /// primeros tres parámetros de salida y la segunda tendrá los resultados del REF_CURSOR.</returns>
        /// <exception cref="System.Exception">Comando.EjecutarRefCursorSP() :  + ex.Message + \n + objComErr.ToString()</exception>
        public DataSet EjecutarRefCursorSP()
        {
            OracleCommand cmd = null;
            DataRow objRow = null;
            OracleParameter parametro = null;
            OracleDate oracleDateAux;
            OracleClob oracleClobAux;
            OracleBlob oracleBlobAux;

            OracleDataAdapter objAdapter = null;
            var objDS = new DataSet();

            try
            {
                 cmd = new OracleCommand(CommandText, Connection) { CommandType = CommandType.StoredProcedure };

                foreach (string nomParametro in _nomParametros)
                {
                    parametro = (OracleParameter)_parametros[nomParametro];
                    cmd.Parameters.Add(parametro);
                }
                cmd.BindByName = true;
                objAdapter = new OracleDataAdapter(cmd);
                objAdapter.Fill(objDS);

                objRow = _resultadoSP.NewRow();

                foreach (string nomParametro in _nomParametros)
                {
                    parametro = (OracleParameter)_parametros[nomParametro];
                    if (!parametro.OracleDbType.Equals(OracleDbType.RefCursor) &&
                        (parametro.Direction.Equals(ParameterDirection.Output) ||
                        parametro.Direction.Equals(ParameterDirection.InputOutput)))
                    {
                        if (parametro.Status != OracleParameterStatus.NullFetched)
                        {
                            if (parametro.OracleDbType.Equals(OracleDbType.Date))
                            {
                                oracleDateAux = (OracleDate)parametro.Value;
                                objRow[nomParametro] = oracleDateAux.Value;
                            }
                            else if (parametro.OracleDbType.Equals(OracleDbType.Clob))
                            {
                                oracleClobAux = (OracleClob)parametro.Value;
                                objRow[nomParametro] = oracleClobAux.Value;
                            }
                            else if (parametro.OracleDbType.Equals(OracleDbType.Blob))
                            {
                                oracleBlobAux = (OracleBlob)parametro.Value;
                                objRow[nomParametro] = oracleBlobAux.Value;
                            }
                            else
                            {
                                objRow[nomParametro] = parametro.Value.ToString();
                            }
                        }
                    }
                }

                _resultadoSP.Rows.Add(objRow);

                objDS.Tables.Add(_resultadoSP);

            }
            catch (Exception ex)
            {
                var objComErr = new StringBuilder();
                try
                {
                    objComErr.Append(CommandText);
                    objComErr.Append("\nParametros\n");
                    foreach (string nomParametro in _nomParametros)
                    {
                        parametro = (OracleParameter)_parametros[nomParametro];
                        objComErr.Append(nomParametro);
                        objComErr.Append(" = ");
                        objComErr.Append(parametro.Value != DBNull.Value ? Convert.ToString(parametro.Value) : "NULL");
                        objComErr.Append(" : ");
                        objComErr.Append(Convert.ToString(parametro.Direction));
                        objComErr.Append("\n");
                    }
                }
                catch (Exception exErr)
                {
                    objComErr.Append(exErr.Message);
                }
                throw new Exception("Comando.EjecutarRefCursorSP() : " + ex.Message + "\n" + objComErr.ToString(), ex);
            }
            finally
            {
                if (objAdapter != null)
                {
                    objAdapter.Dispose();
                    objAdapter = null;
                }

                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    cmd = null;
                }
            }
            return objDS;
        }
        /// <summary>
        /// Método que se utiliza para agregar parámetros del tipo IN a un PROCEDURE ó FUNCTION.
        /// Para una FUNCTION el primer parámetro que se debe agregar es el de Retorno, con el método
        /// [AgregarReturnParametro] de esta misma clase.
        /// </summary>
        /// <param name="nombre">Nombre del parámetro</param>
        /// <param name="tipo">Tipo de dato Oracle</param>
        /// <param name="valor">Valor del parámetro de entrada</param>
        public void AgregarInParametro(string nombre, OracleDbType tipo, Object valor)
        {
            var parametro = new OracleParameter(nombre, tipo, ParameterDirection.Input)
            {
                Value = valor ?? System.DBNull.Value
            };
            _parametros.Add(nombre, parametro);
            _nomParametros.Add(nombre);
        }
        /// <summary>
        /// Método que se utiliza para agregar parámetros del tipo OUT a un PROCEDURE.
        /// </summary>
        /// <param name="nombre">Nombre del parámetro de salida.</param>
        /// <param name="tipo">Tipo de dato Oracle.</param>
        /// <param name="tamanio">Tamaño aproximado del dato de salida.</param>
        public void AgregarOutParametro(string nombre, OracleDbType tipo, int tamanio)
        {
            var parametro = new OracleParameter(nombre, tipo, tamanio, ParameterDirection.Output) { Size = tamanio };
            if (tipo.Equals(OracleDbType.Date))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(DateTime)));
            }
            else if (tipo.Equals(OracleDbType.Blob))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(byte[])));
            }
            else if (!tipo.Equals(OracleDbType.RefCursor))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(string)));
            }
            _parametros.Add(nombre, parametro);
            _nomParametros.Add(nombre);
        }
        /// <summary>
        /// Método que se utiliza para agregar parámetros del tipo INOUT a un PROCEDURE.
        /// </summary>
        /// <param name="nombre">Nombre del parámetro de entrada/salida.</param>
        /// <param name="tipo">Tipo de dato Oracle.</param>
        /// <param name="tamanio">Tamaño aproximado del dato de salida.</param>
        /// <param name="valor">Valor del parámetro de entrada.</param>
        public void AgregarInOutParametro(string nombre, OracleDbType tipo, int tamanio, Object valor)
        {
            var parametro = new OracleParameter(nombre, tipo, tamanio, ParameterDirection.InputOutput)
            {
                Size = tamanio,
                Value = valor ?? System.DBNull.Value
            };
            if (tipo.Equals(OracleDbType.Date))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(DateTime)));
            }
            else if (tipo.Equals(OracleDbType.Blob))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(byte[])));
            }
            else
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(string)));
            }
            _parametros.Add(nombre, parametro);
            _nomParametros.Add(nombre);
        }
        /// <summary>
        /// Método que se utiliza para agregar el parámetro de Retorno a FUNCTION.
        /// Para una FUNCTION éste es el primer parámetro que se debe agregar.
        /// </summary>
        /// <param name="nombre">Nombre del parámetro de Retorno.</param>
        /// <param name="tipo">Tipo de dato Oracle.</param>
        /// <param name="tamanio">Tamaño aproximado del dato de Retorno.</param>
        public void AgregarReturnParametro(string nombre, OracleDbType tipo, int tamanio)
        {
            var parametro = new OracleParameter(nombre, tipo, tamanio, ParameterDirection.ReturnValue) { Size = tamanio };
            if (tipo.Equals(OracleDbType.Date))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(DateTime)));
            }
            else if (tipo.Equals(OracleDbType.Blob))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(byte[])));
            }
            else if (!tipo.Equals(OracleDbType.RefCursor))
            {
                _resultadoSP.Columns.Add(new DataColumn(nombre, typeof(string)));
            }
            _parametros.Add(nombre, parametro);
            _nomParametros.Add(nombre);
        }
        /// <summary>
        /// Limpia el objeto para que se puedan ejecutar distintas sentencias SQL, PROCEDURE ó FUCTION
        /// </summary>
        public void RemoveAll()
        {
            _nomParametros.Clear();
            _parametros.Clear();
            _resultadoSP.Clear();
            _resultadoSP.Columns.Clear();
            IsSynonym = false;
            Owner = null;
        }
        /// <summary>
        /// Fija la sentencia SQL, PROCEDURE ó FUNCTION a ejecutar.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText { get; set; }

        /// <summary>
        /// Fija el objeto de conexión.
        /// </summary>
        /// <value>The connection.</value>
        public OracleConnection Connection { get; set; }

        /// <summary>
        /// Propiedad de Dueño del Objeto
        /// </summary>
        /// <value>The owner.</value>
        public string Owner
        {
            set { _owner = value; }
        }
        /// <summary>
        /// Propiedad para empleo de Sinónimos
        /// </summary>
        /// <value><c>true</c> Si la instancia empleada Synonym en otro caso<c>false</c>.</value>
        public bool IsSynonym
        {
            set { _bisSynonym = value; }
        }
        /// <summary>
        /// Ejecutar  Comando con lista de Parametros
        /// </summary>
        /// <param name="parametros">Parámetros que se envían a la consulta</param>
        /// <returns>Por cada parámetro REF_CURSOR de salida se obtendría un DataTable en el DataSet de retorno.
        /// También se tendrá una tabla extra que contendrá los parámetros de salida que no son del tipo REF_CURSOR.
        /// Por ejemplo:
        /// PROCEDURE(param1 IN NUMBER,
        /// param2 OUT VARCHAR2,
        /// param3 OUT NUMBER,
        /// param4 OUT DATE,
        /// param5 OUT REF_CURSOR);
        /// En este caso se tendrá un DataSet con dos Tablas, la primer tabla contendrá un renglón con los
        /// primeros tres parámetros de salida y la segunda tendrá los resultados del REF_CURSOR.</returns>
        /// <exception cref="System.Exception">Comando.Ejecutar() :  + ex.Message</exception>
        public DataSet Ejecutar(ArrayList parametros)
        {
            var objDS = new DataSet();
            DataTable tableParametros = null;
            object oParamValue = null;
            string sParamName = null;
            string sDataType = null;
            string sInOut = null;
            var iPosition = 0;
            var IndexParam = 0;
            try
            {
                tableParametros = GetParametros();
                if (tableParametros != null)
                {
                    for (var i = 0; i < tableParametros.Rows.Count; i++)
                    {
                        sParamName = Convert.ToString(tableParametros.Rows[i]["argument_name"]);
                        sDataType = Convert.ToString(tableParametros.Rows[i]["dataType"]);
                        sInOut = Convert.ToString(tableParametros.Rows[i]["inOut"]);
                        iPosition = Convert.ToInt32(tableParametros.Rows[i]["position"]);
                        oParamValue = parametros[IndexParam];

                        if (sDataType.Equals("NUMBER") && parametros[IndexParam].ToString().Equals(""))
                        {
                            oParamValue = DBNull.Value;
                        }

                        if (sInOut.Equals("IN") || sInOut.Equals("IN/OUT"))
                        {
                            AgregarParametro(sParamName, sInOut, sDataType, iPosition, oParamValue);
                            IndexParam++;
                        }
                        else
                        {
                            AgregarParametro(sParamName, sInOut, sDataType, iPosition, null);
                        }
                    }

                    objDS = EjecutarRefCursorSP();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Comando.Ejecutar() : " + ex.Message, ex);
            }
            return objDS;
        }
        /// <summary>
        /// Obtiene los parámetros empleados en el Commando
        /// </summary>
        /// <returns>DataTable.</returns>
        /// <exception cref="System.Exception">Comando.GetParametros() :  + ex.Message</exception>
        private DataTable GetParametros()
        {
            OracleCommand cmd = null;
            OracleDataAdapter objAdapter = null;
            var objTabla = new DataTable("RESULTADO");
            var strSQL = "";
            string synCommandText = null;
            string[] arrPkg = null;
            var isPackage = false;

            try
            {
                if (_bisSynonym)
                {
                    synCommandText = GetDatosSinonimo() ?? CommandText;

                    arrPkg = synCommandText.Split('.');

                    isPackage = arrPkg.Length > 0 ? true : false;
                    if (isPackage)
                    {
                        strSQL = "select b.position,\n" +
                                 "       b.sequence,\n" +
                                 "       b.dataType,\n" +
                                 "       b.inOut,\n" +
                                 "       b.argument_name\n" +
                                 "from   all_objects" + _dbLink + " a,\n" +
                                 "       all_arguments" + _dbLink + " b\n" +
                                 "where  b.object_name = '" + arrPkg[1].ToUpper() + "'\n" +
                                 "and    b.object_id   = a.object_id\n" +
                                 "and    a.object_type = 'PACKAGE'\n" +
                                 "and    a.object_name = '" + arrPkg[0].ToUpper() + "'\n" +
                                 "and    a.owner       = '" + _owner.ToUpper() + "'";
                    }
                    else
                    {
                        strSQL = "select b.position,\n" +
                                 "       b.sequence,\n" +
                                 "       b.dataType,\n" +
                                 "       b.inOut,\n" +
                                 "       b.argument_name\n" +
                                 "from   all_objects" + _dbLink + " a,\n" +
                                 "       all_arguments" + _dbLink + " b\n" +
                                 "where  b.object_id   = a.object_id\n" +
                                 "and    a.object_name = '" + synCommandText.ToUpper() + "'\n" +
                                 "and    a.owner       = '" + _owner.ToUpper() + "'";
                    }
                }
                else
                {
                    arrPkg = CommandText.Split('.');
                    isPackage = arrPkg.Length > 0 ? true : false;
                    if (isPackage)
                    {
                        strSQL = "select b.position,\n" +
                                 "       b.sequence,\n" +
                                 "       b.dataType,\n" +
                                 "       b.inOut,\n" +
                                 "       b.argument_name\n" +
                                 "from   all_objects a,\n" +
                                 "       all_arguments b\n" +
                                 "where  b.object_name = '" + arrPkg[1].ToUpper() + "'\n" +
                                 "and    b.object_id   = a.object_id\n" +
                                 "and    a.object_type = 'PACKAGE'\n" +
                                 "and    a.object_name = '" + arrPkg[0].ToUpper() + "'\n" +
                                 "and    a.owner       = '" + _owner.ToUpper() + "'";
                    }
                    else
                    {
                        strSQL = "select b.position,\n" +
                                 "       b.sequence,\n" +
                                 "       b.dataType,\n" +
                                 "       b.inOut,\n" +
                                 "       b.argument_name\n" +
                                 "from   all_objects a,\n" +
                                 "       all_arguments b\n" +
                                 "where  b.object_id   = a.object_id\n" +
                                 "and    a.object_name = '" + CommandText.ToUpper() + "'\n" +
                                 "and    a.owner       = '" + _owner.ToUpper() + "'";
                    }
                }

                cmd = new OracleCommand(strSQL, Connection) { CommandType = CommandType.Text };
                objAdapter = new OracleDataAdapter(cmd);
                objAdapter.Fill(objTabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Comando.GetParametros() : " + ex.Message, ex);
            }
            finally
            {
                if (objAdapter != null)
                {
                    objAdapter.Dispose();
                    objAdapter = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }

            return objTabla;
        }
        /// <summary>
        /// Médtodo para recuperar información del Sinónimo
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">Comando.GetDatosSinonimo() :  + ex.Message</exception>
        private string GetDatosSinonimo()
        {
            OracleCommand cmd = null;
            OracleDataAdapter objAdapter = null;
            var objTabla = new DataTable("RESULTADO");
            var strSQL = "";
            string synCommandText = null;
            var arrPkg = CommandText.Split('.');
            var isPackage = arrPkg.Length > 0 ? true : false;

            try
            {
                strSQL = "select table_owner, " +
                         "       table_name, " +
                         "       db_link " +
                         "from   all_synonyms " +
                         "where  synonym_name = '" + (isPackage ? arrPkg[0].ToUpper() : CommandText.ToUpper()) + "'";

                cmd = new OracleCommand(strSQL, Connection) { CommandType = CommandType.Text };
                objAdapter = new OracleDataAdapter(cmd);
                objAdapter.Fill(objTabla);

                if (objTabla.Rows.Count > 0)
                {
                    this._owner = Convert.ToString(objTabla.Rows[0]["table_owner"]);
                    synCommandText = isPackage ? Convert.ToString(objTabla.Rows[0]["table_name"]) + "." + arrPkg[1] : Convert.ToString(objTabla.Rows[0]["table_name"]);

                    if (Convert.ToString(objTabla.Rows[0]["db_link"]).Equals(""))
                    {
                        this._dbLink = "";
                    }
                    else
                    {
                        this._dbLink = "@" + Convert.ToString(objTabla.Rows[0]["db_link"]);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Comando.GetDatosSinonimo() : " + ex.Message, ex);
            }
            finally
            {
                if (objAdapter != null)
                {
                    objAdapter.Dispose();
                    objAdapter = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }

            return synCommandText;
        }
        /// <summary>
        /// Método para agregar parámetros genérico.
        /// </summary>
        /// <param name="nombreParametro">El nombre del parámetro.</param>
        /// <param name="inOut">Tipo de parámetros</param>
        /// <param name="dataType">Tipo de datos</param>
        /// <param name="posicion">La posición</param>
        /// <param name="valor">El valor</param>
        /// <exception cref="System.Exception">Comando.AgregarParametro() : Tipo OracleDbType [ + dataType + ] no conocido
        /// or
        /// Comando.AgregarParametro() : Tipo IN OUT [ + inOut + ] no conocido</exception>
        private void AgregarParametro(string nombreParametro, string inOut, string dataType, int posicion, Object valor)
        {
            OracleDbType tipo;
            var tamanio = 0;

            switch (dataType)
            {
                case "NUMBER":
                    tipo = OracleDbType.Single;
                    tamanio = 0;
                    break;
                case "VARCHAR2":
                    tipo = OracleDbType.Varchar2;
                    tamanio = 255;
                    break;
                case "DATE":
                    tipo = OracleDbType.Date;
                    tamanio = 0;
                    break;
                case "CLOB":
                    tipo = OracleDbType.Clob;
                    tamanio = 0;
                    break;
                case "BLOB":
                    tipo = OracleDbType.Blob;
                    tamanio = 0;
                    break;
                case "REF CURSOR":
                    tipo = OracleDbType.RefCursor;
                    tamanio = 0;
                    break;
                default:
                    throw new Exception("Comando.AgregarParametro() : Tipo OracleDbType [" + dataType + "] no conocido");
            }

            switch (inOut)
            {
                case "IN":
                    AgregarInParametro(nombreParametro, tipo, valor);
                    break;
                case "OUT":
                    if (posicion == 0)
                    {
                        AgregarReturnParametro(nombreParametro, tipo, tamanio);
                    }
                    else
                    {
                        AgregarOutParametro(nombreParametro, tipo, tamanio);
                    }
                    break;
                case "IN/OUT":
                    AgregarInOutParametro(nombreParametro, tipo, tamanio, valor);
                    break;
                default:
                    throw new Exception("Comando.AgregarParametro() : Tipo IN OUT [" + inOut + "] no conocido");
            }
        }
    }
}