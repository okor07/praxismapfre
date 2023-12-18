/**********************************************************************************
* @author                        :  Daniel Ramírez Herrera
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net 
* Name of the File               :  CLMPersona.cs
* Creation/Modification History  :
*                   19-Mayo-2008     Creada 
*
* Sample Overview:
* Supertipo para el Tipo de Persona
* 
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapfreMMX.clm
{
    /***********************************************************************************/
    public enum TipoPersona
    {
        Fisica = 'S',
        Moral = 'N'
    }
    /***********************************************************************************/
    public class CLMPersona
    {
        private string sCOD_DOCUM;
        private string sTIP_DOCUM;
        private string sCOD_DOCUM_PADRE;
        private string sTIP_DOCUM_PADRE;
        private TipoPersona sTIP_PERSONA;
        private string sCVE_RFC;
        private string sOBSERVACIONES;
        /*=============================================================================*/
        public string COD_DOCUM
        {
            get { return sCOD_DOCUM; }
            set 
            {
                if (value != null && value.Trim().Length > 0)
                    sCOD_DOCUM = value;                
            }
        }
        /*=============================================================================*/
        public string TIP_DOCUM
        {
            get { return sTIP_DOCUM; }
            set 
            {
                if (value != null && value.Trim().Length > 0)
                    sTIP_DOCUM = value;
                else
                    sTIP_DOCUM = "CLM";
            }
        }
        /*=============================================================================*/
        public string COD_DOCUM_PADRE
        {
            get { return sCOD_DOCUM_PADRE; }
            set 
            {
                if (value != null && value.Trim().Length > 0)
                    sCOD_DOCUM_PADRE = value; 
            }
        }
        /*=============================================================================*/
        public string TIP_DOCUM_PADRE
        {
            get { return sTIP_DOCUM_PADRE; }
            set 
            {
                if (value != null && value.Trim().Length > 0)
                    sTIP_DOCUM_PADRE = value; 
            }
        }
        /*=============================================================================*/
        public TipoPersona TIP_PERSONA
        {
            get { return sTIP_PERSONA; }
            set { sTIP_PERSONA = value; }
        }
        /*=============================================================================*/
        public string CVE_RFC
        {
            get { return sCVE_RFC; }
            set 
            {
                if (value != null && value.Trim().Length > 0)
                    sCVE_RFC = value; 
            }
        }
        /*=============================================================================*/
        public string OBSERVACIONES
        {
            get { return sOBSERVACIONES; }
            set 
            {
                if (value != null && value.Trim().Length > 0)
                    sOBSERVACIONES = value; 
            }
        }        
        /*=============================================================================*/
        // Funcion para validar la seleccion de combos
        protected string validaNoSeleccion(string valor)
        {
            int intValor = -1;
            try
            {
                if (valor != "")
                    intValor = 1;
                intValor = Convert.ToInt32(valor);
            }
            catch { }

            if (intValor < 1)
                valor = null;

            return valor;
        }
        /*=============================================================================*/
    }
}
