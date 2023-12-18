using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GEN = MapfreHSBC.Models.General;

namespace MapfreHSBC.Models.Cotizacion
{
    public class DatosContratante
    {
        #region Propiedades
        public const string API = "api/Cotizacion/WS3";
        public const string NameView = "VerificarDatos";
        #endregion

        #region Contrusctor
        public DatosContratante()
        { }
        public DatosContratante(Datos datos)
        { }
        #endregion

        public class MsgJson
        {
            public MsgJson()
            { this.datosBeneficiarios = new List<DatosBeneficiario>(); }
            #region Propiedades
            [Required]
            public Int64? idTransaccion
            { get; set; }

            [Required]
            public string idPromotor
            { get; set; }

            [Required]
            public string idCotizacionMapfre
            { get; set; }

            [Required]
            public string rfc
            { get; set; }

            [Required]
            public string idNacionalidadMapfre
            { get; set; }

            [Required]
            public string domicilioContratanteCalle
            { get; set; }

            [Required]
            public string domicilioContratanteNumInt
            { get; set; }

            [Required]
            public string domicilioContratanteNumExt
            { get; set; }

            [Required]
            public string domicilioContratanteColonia
            { get; set; }

            [Required]
            public string paisResidencia
            { get; set; }

            [Required]
            public string codigoPostal
            { get; set; }

            [Required]
            public string nombreDeContratante
            { get; set; }

            [Required]
            public string apellidoPaternoContrante
            { get; set; }

            public string apellidoMaternoContrante
            { get; set; }

            [Required]
            public string tipoDePersona
            { get; set; }

            public string fea
            { get; set; }

            [Required]
            public string curp
            { get; set; }

            [Required]
            public string paisOrigen
            { get; set; }

            [Required]
            public string telefono
            { get; set; }

            public string telfonoCelular
            { get; set; }

            [Required]
            public string tipoIdentificacion
            { get; set; }

            [Required]
            public string numeroIdentificacion
            { get; set; }

            [Required]
            public string cuentaClabeContratante
            { get; set; }

            [Required]
            public string paisNacimientoContratante
            { get; set; }

            [Required]
            public string cuidadNacimientoContratante
            { get; set; }

            [Required]
            public string paisResidenciaFiscalContratante
            { get; set; }

            [Required]
            public string estadoCivilContratante
            { get; set; }

            [Required]
            public string ocupacionContratante
            { get; set; }

            [Required]
            public int? statusPEPS
            { get; set; }

            [Required]
            public List<DatosBeneficiario> datosBeneficiarios
            { get; set; }


            public override string ToString()
            {
                return String.Format("idTransaccion: {0}, idPromotor:{1}, idCotizacionMapfre:{2}, "
                    + "nombreDeContratante: {3}, apellidoPaternoContrante: {4}, apellidoMaternoContrante: {5} ",
                    idTransaccion, idPromotor, idCotizacionMapfre, nombreDeContratante, apellidoPaternoContrante, apellidoMaternoContrante);
            }

        }

        #region Clases
        public class Datos : GEN.General.AsSerializeable
        {

            public MsgJson msgJson { get; set; }

            #endregion
            public override string ToString()
            {
                return String.Format("msgJson: {0}",msgJson);
            }
        }
        public class MsgJsonRespuesta
        {
            public string url
            { get; set; }
        }

        public class Respuesta : GEN.General.Respuesta
        {
            public Respuesta()
            { }
            public MsgJsonRespuesta msgJson
            { get; set; }
        }

        public class DatosBeneficiario : GEN.General.AsSerializeable
        {
            [Required]
            public Int64? idTransaccion
            { get; set; }

            [Required]
            public string idPromotor
            { get; set; }

            [Required]
            public string idCotizacionMapfre
            { get; set; }

            [Required]
            public int? idBeneficiario
            { get; set; }

            [Required]
            public string nombreDeBeneficiario
            { get; set; }

            [Required]
            public string apellidoPaternoBeneficiario
            { get; set; }

            public string apellidoMaternoBeneficiario
            { get; set; }

            [Required]
            public char tipoPersona
            { get; set; }

            [Required]
            public string idParentescoBeneficiarioMapfre
            { get; set; }

            [Required]
            public string fechaNacimientoBeneficiario
            { get; set; }

            [Required]
            public string domicilioBeneficiarioCalle
            { get; set; }

            [Required]
            public string domicilioBeneficiarioNumInt
            { get; set; }

            [Required]
            public string domicilioBeneficiarioNumExt
            { get; set; }

            [Required]
            public string domicilioBeneficiarioNomColonia
            { get; set; }

            [Required]
            public string cpBeneficiario
            { get; set; }

            [Required]
            public string telefono
            { get; set; }

            public string telfonoCelular
            { get; set; }

            [Required]
            public int? genero
            { get; set; }

            [Required]
            public string nacionalidadBeneficiario
            { get; set; }

            [Required]
            public int? porcentaje
            { get; set; }

            [Required]
            public string tipoBeneficiario
            { get; set; }
        }
        #endregion
    }
}