/**********************************************************************************
* @author                        :  Daniel Ramírez Herrera
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net 
* Name of the File               :  CLMFisica.cs
* Creation/Modification History  :
*                   19-Mayo-2008     Creada 
*
* Sample Overview:
* Datos de Persona Fisica
* 
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace MapfreMMX.clm
{
    /***********************************************************************************/
    public enum TipoGenero
    {
        Femenino,
        Masculino
        //Masculino = 1,
        //Femenino = 0
    }
    /***********************************************************************************/
    public class CLMFisica : CLMPersona
    {
        public string NOM_TERCERO;
        public string APE1_TERCERO;
        private string _APE2_TERCERO;
        public string FEC_NACIMIENTO;
        private string _CURP;
        public TipoGenero COD_SEXO;
        private string _COD_EST_CIVIL;
        public string NOM_EST_CIVIL;
        private string _COD_PROFESION;
        public string NOM_PROFESION;
        public string TLF_MOVIL;
        private string _COD_PARENTESCO;
        public string NOM_PARENTESCO;
        private string _COD_NACIONALIDAD;
        public string NOM_NACIONALIDAD;
        ///*=============================================================================*/
        //public void setFEC_NACIMIENTO(string FECHA)
        //{
        //    if (FECHA.IndexOf("dd/mm/") != -1 || FECHA == "")
        //        FECHA = null;
        //    FEC_NACIMIENTO = Convert.ToDateTime(FECHA, CultureInfo.CurrentCulture);
        //}
        ///*=============================================================================*/
        //public object getFEC_NACIMIENTO()
        //{
        //    if (FEC_NACIMIENTO == Convert.ToDateTime(null))
        //        return DBNull.Value;
        //    else return FEC_NACIMIENTO;
        //}
        ///*=============================================================================*/
        //public string getFEC_NACIMIENTOstring()
        //{
        //    if (FEC_NACIMIENTO == Convert.ToDateTime(null))
        //        return String.Empty;
        //    else return FEC_NACIMIENTO.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        //}
        ///*=============================================================================*/
        public string APE2_TERCERO
        {
            get
            {
                return _APE2_TERCERO;
            }
            set
            {
                if (value != null && value.Trim().Length > 0)
                    _APE2_TERCERO = value;
            }
        }
        /*=============================================================================*/
        public string CURP
        {
            get
            {
                return _CURP;
            }
            set
            {
                if (value != null && value.Trim().Length > 0)
                    _CURP = value;
            }
        }
        /*=============================================================================*/
        public string COD_EST_CIVIL
        {
            get { return _COD_EST_CIVIL; }
            set { _COD_EST_CIVIL = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
        public string COD_NACIONALIDAD
        {
            get { return _COD_NACIONALIDAD; }
            set { _COD_NACIONALIDAD = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
        public string COD_PARENTESCO
        {
            get { return _COD_PARENTESCO; }
            set { _COD_PARENTESCO = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
        public string COD_PROFESION
        {
            get { return _COD_PROFESION; }
            set { _COD_PROFESION = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
    }
}
