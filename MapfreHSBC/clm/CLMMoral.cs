/**********************************************************************************
* @author                        :  Daniel Ramírez Herrera
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net 
* Name of the File               :  CLMMoral.cs
* Creation/Modification History  :
*                   19-Mayo-2008     Creada 
*
* Sample Overview:
* Datos de Persona Moral
* 
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace MapfreMMX.clm
{
    public class CLMMoral : CLMPersona
    {
        public string RAZON_SOCIAL;
        public string FEC_CONSTITUCION;
        public string NOM_CONTACTO;
        private string _TIP_CARGO;
        public string NOM_CARGO;
        private string _TIP_ACTIVIDAD;
        public string NOM_ACTIVIDAD;
        private string _COD_PARENTESCO;
        ///*=============================================================================*/
        //public void setFEC_CONSTITUCION(string FECHA)
        //{
        //    if (FECHA.IndexOf("dd/mm/") != -1 || FECHA == "")
        //        FECHA = null;
        //    FEC_CONSTITUCION = Convert.ToDateTime(FECHA, CultureInfo.CurrentCulture);
        //}
        ///*=============================================================================*/
        //public object getFEC_CONSTITUCION()
        //{
        //    if (FEC_CONSTITUCION == Convert.ToDateTime(null))
        //        return DBNull.Value;
        //    else return FEC_CONSTITUCION;
        //}
        ///*=============================================================================*/
        //public string getFEC_CONSTITUCIONstring()
        //{
        //    if (FEC_CONSTITUCION == Convert.ToDateTime(null))
        //        return String.Empty;
        //    else return FEC_CONSTITUCION.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        //}
        ///*=============================================================================*/
        public string TIP_CARGO
        {
            get { return _TIP_CARGO; }
            set { _TIP_CARGO = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
        public string TIP_ACTIVIDAD
        {
            get { return _TIP_ACTIVIDAD; }
            set { _TIP_ACTIVIDAD = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
        public string COD_PARENTESCO
        {
            get { return _COD_PARENTESCO; }
            set { _COD_PARENTESCO = validaNoSeleccion(value); }
        }
        /*=============================================================================*/
    }
}
