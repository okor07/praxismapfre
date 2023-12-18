using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapfreHSBC.Models.Cotizacion
{
    public class AltaCotizacion
    {
        private int? dia;
        public string idPromotor { get; set; }
        public long idTransaccion { get; set; }
        public long numCotizacion { get; set; }
        public string fechaCotizacion { get; set; }
        public string fechaNacimiento { get; set; }
        public string sexo { get; set; }
        public string correoElectronico { get; set; }
        public string perfil { get; set; }
        public string perfilText { get; set; }
        public double primaInicial { get; set; }
        public string modalidad { get; set; }
        public string modalidadText { get; set; }
        public double aportaciones { get; set; }
        public string moneda { get; set; }
        public string monedaText { get; set; }
        public string periodicidad { get; set; }
        public string periodicidadText { get; set; }
        public string plazo { get; set; }
        public string formaPago { get; set; }
        public int? diaCobro { get { return dia.HasValue ? dia : 0; } set { dia = value != null ? value : 0; } }
        public double pctInversion1 { get; set; }
        public double pctInversion2 { get; set; }
        public double pctInversion3 { get; set; }
        //HHAC ini
        public double pctInversion4 { get; set; }
        public double pctInversion5 { get; set; }
        //HHAC fin
        public string msgJson { get; set; }

        public override string ToString()
        {
            return String.Format("idPromotor: {0}, idTransaccion:{1}, numCotizacion:{2}, " 
                + "sexo: {3}, correoElectronico: {4}, perfil: {5}, periodicidadText: {6}, msgJson: {7} ",
                idPromotor, idTransaccion, numCotizacion, sexo, correoElectronico, perfil, periodicidadText, msgJson);
        }

    }
}