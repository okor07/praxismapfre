// ***********************************************************************
// Ensamblado				: MapfreWebCore
// Autor           			: Israel Lerma Hernández
// Creado          			: 08-06-2015
// Powered by				: BW Meill Corporation
// IDE						: Microsoft Visual Studio .Net 2013
//
// Ultima modificación por	: Israel Lerma Hernández
// Ultima modificación en 	: 08-17-2015
// ***********************************************************************
// <copyright file="RegistroArchivo.cs" company="MAPFRE">
//     Copyright (c) MAPFRE. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Linq;

namespace MapfreWebCore.Registros
{
    /// <summary>
    /// Class RegistroArchivo.
    /// </summary>
    public class RegistroArchivo
    {
        /// <summary>
        /// Instancia de registro de eventos
        /// </summary>
        private static RegistroArchivo _instancia;
        /// <summary>
        /// FileStream que apunta al archivo de registro
        /// </summary>
        private FileStream _archivoRegistro;
        /// <summary>
        /// Nombre del archivo físico
        /// </summary>
        private string _nombreArchivo;
        /// <summary>
        /// Ruta de ubicación del archivo físico
        /// </summary>
        private static string _ruta = "";
        /// <summary>
        /// Constructor del archivo de registro
        /// </summary>
        private RegistroArchivo()
        {
            if (ConfigurationManager.AppSettings["RutaArchivoRegistro"] != null)
            {
                _ruta = ConfigurationManager.AppSettings["RutaArchivoRegistro"];
            }

            _nombreArchivo = _ruta + "ArchivoRegistro" + DateTime.Now.ToString("ddMMyyyy") + ".log";
            Abrir();
        }
        /// <summary>
        /// Recupera una instancia del RegistroArchivo
        /// </summary>
        /// <returns>RegistroArchivo</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static RegistroArchivo GetInstancia()
        {
            return _instancia ?? (_instancia = new RegistroArchivo());
        }
        /// <summary>
        /// Escribir una excepción en el archivo texto en el registro
        /// </summary>
        /// <param name="texto">Texto a escribir</param>
        /// <param name="ex">Objeto de excepción</param>
        /// <exception cref="System.Exception">RegistroArchivo.Escribir(1):  + exc.Message</exception>
        public void Escribir(string texto, Exception ex)
        {
            StreamWriter sw = null;
            Exception inner = null;
            try
            {
                lock (this)
                {
                    sw = new StreamWriter(_archivoRegistro);
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------->" + texto);
                    if (ex != null)
                    {
                        sw.WriteLine("\tMessage:" + ex.Message);
                        sw.WriteLine("\tStackTrace:" + ex.StackTrace);
                        inner = ex.InnerException;
                        if (inner != null)
                        {
                            sw.WriteLine("\tInner:");
                            while (inner != null)
                            {
                                sw.WriteLine("\t" + inner.ToString());
                                inner = inner.InnerException;
                            }
                        }
                    }
                    sw.Flush();
                    this.Cerrar();
                }
            }
            catch (Exception exc)
            {
                throw new Exception("RegistroArchivo.Escribir(1): " + exc.Message, exc);
            }
        }

        /// <summary>
        /// Escribir una excepción en el archivo texto en el registro
        /// </summary>
        /// <param name="texto">Texto a escribir</param>
        /// <param name="ex">Objeto de excepción</param>
        /// <exception cref="System.Exception">RegistroArchivo.Escribir(1):  + exc.Message</exception>
        public void Escribir(string texto, ref string idCorrelacion, Exception ex = null)
        {
            StreamWriter sw = null;
            Exception inner = null;
            try
            {
                lock (this)
                {
                    sw = new StreamWriter(_archivoRegistro);
                    idCorrelacion = RandomCorrelation();
                    sw.WriteLine(string.Format("IdError : {0}  --------------------------------", idCorrelacion));
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------->" + texto);
                    if (ex != null)
                    {
                        sw.WriteLine("\tMessage:" + ex.Message);
                        sw.WriteLine("\tStackTrace:" + ex.StackTrace);
                        inner = ex.InnerException;
                        if (inner != null)
                        {
                            sw.WriteLine("\tInner:");
                            while (inner != null)
                            {
                                sw.WriteLine("\t" + inner.ToString());
                                inner = inner.InnerException;
                            }
                        }
                    }
                    sw.Flush();
                    this.Cerrar();
                }
            }
            catch (Exception exc)
            {
                throw new Exception("RegistroArchivo.Escribir(1): " + exc.Message, exc);
            }
        }
        /// <summary>
        /// Escribir texto especifico.
        /// </summary>
        /// <param name="texto">Texto a escribir</param>
        /// <exception cref="System.Exception">RegistroArchivo.Escribir(2):  + exc.Message</exception>
        public void Escribir(string texto)
        {
            StreamWriter sw = null;

            try
            {
                lock (this)
                {
                    sw = new StreamWriter(_archivoRegistro);
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------->" + texto);
                    sw.Flush();
                    this.Cerrar();
                }
            }
            catch (Exception exc)
            {
                throw new Exception("RegistroArchivo.Escribir(2): " + exc.Message, exc);
            }
        }
        /// <summary>
        /// Abre archivo
        /// </summary>
        /// <exception cref="System.Exception">RegistroArchivo.Abrir():  + ex.Message</exception>
        private void Abrir()
        {
            try
            {
                _archivoRegistro = new FileStream(_nombreArchivo, FileMode.Append, FileAccess.Write, FileShare.Read);
            }
            catch (Exception ex)
            {
                throw new Exception("RegistroArchivo.Abrir(): " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene la ruta del archivo
        /// </summary>
        /// <value>Ruta</value>
        public static string Ruta
        {
            set { _ruta = value + "\\"; }
            get { return _ruta; }
        }
        /// <summary>
        /// Realiza una limpieza del archivo y genera un respaldo
        /// </summary>
        /// <exception cref="System.Exception">RegistroArchivo.LimpiaRespalda():  + ex.Message</exception>
        public void LimpiarRespaldar()
        {
            try
            {
                lock (this)
                {
                    _archivoRegistro.Close();
                    File.Move(_nombreArchivo, (_ruta + DateTime.Now.ToString("yyMMdd-HHmmss-") + "AppLog.log"));
                    _instancia = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RegistroArchivo.LimpiaRespalda(): " + ex.Message, ex);
            }
        }
        /// <summary>
        /// Cerrars la instancia
        /// </summary>
        /// <exception cref="System.Exception">RegistroArchivo.Cerrar():  + ex.Message</exception>
        public void Cerrar()
        {
            try
            {
                _archivoRegistro.Close();
                _instancia = null;
            }
            catch (Exception ex)
            {
                throw new Exception("RegistroArchivo.Cerrar(): " + ex.Message, ex);
            }
        }

        public string RandomCorrelation()
        {
            Random random = new Random();

            const string chars = "0123456789";
            return string.Format("M{0}", new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray()));
        }
    }
}