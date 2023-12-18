using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MapfreHSBC.Models.General
{
    public class DatosCorreo
    {
        public int idCotizacion { get; set; }
        public string de { get; set; }
        public string remitente { get; set; }
        public string para { get; set; }
        public string asunto { get; set; }
        public string mensaje { get; set; }
        // propiedades de los datos requeridos para obtener PDF y enviarlo por correo
        public string numCotizacion { get; set; } 
        public string fecNacimiento { get; set; }
        public string sexo { get; set; } 
        public string correo { get; set; } 
        public string moneda { get; set; } 
        public string plazo { get; set; } 
        public string primaIni { get; set; } 
        public string primaAdd { get; set; }
        public string frecuenciaPrima { get; set; } 
        public string perfil { get; set; } 
        public string pctInversion1 { get; set; } 
        public string pctInversion2 { get; set; } 
        public string pctInversion3 { get; set; }
        public string modalidad { get; set; }


        public override string ToString()
        {
            return 
                new StringBuilder().Append("idCotizacion:").Append(idCotizacion).Append(", ")
                .Append("de:").Append(de).Append(", ")
                .Append("remitente:").Append(remitente).Append(", ")
                .Append("para:").Append(para).Append(", ")
                .Append("asunto:").Append(asunto).Append(", ")
                .Append("mensaje:").Append(mensaje).Append(", ")
                .Append("numCotizacion:").Append(numCotizacion).Append(", ")
                .Append("fecNacimiento:").Append(fecNacimiento).Append(", ")
                .Append("sexo:").Append(sexo).Append(", ")
                .Append("correo:").Append(correo).Append(", ")
                .Append("moneda:").Append(moneda).Append(", ")
                .Append("plazo:").Append(plazo).Append(", ")
                .Append("primaIni:").Append(primaIni).Append(", ")
                .Append("primaAdd:").Append(primaAdd).Append(", ")
                .Append("frecuenciaPrima:").Append(frecuenciaPrima).Append(", ")
                .Append("perfil:").Append(perfil).Append(", ")
                .Append("pctInversion1:").Append(pctInversion1).Append(", ")
                .Append("pctInversion2:").Append(pctInversion2).Append(", ")
                .Append("pctInversion3:").Append(pctInversion3).Append(" ")
                .ToString();
        }


    }
}