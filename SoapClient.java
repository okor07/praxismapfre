import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Base64;

public class SoapClient {

    public static void main(String[] args) {
        try {
            String rutaURL = "URL_DEL_SERVICIO_SOAP";
            String userHPExstream = "TU_USUARIO";
            String paswHPExstream = "TU_CONTRASEÑA";
            String xmlHPExstream = "XML_EN_FORMATO_BASE64";
            String emisionSector = "SECTOR_EMISION";
            String sPubFile = "ARCHIVO_PUB";

            URL url = new URL(rutaURL);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();

            // Configurar la conexión HTTP
            connection.setRequestMethod("POST");
            connection.setRequestProperty("Content-Type", "text/xml;charset=utf-8");
            connection.setRequestProperty("Accept", "text/xml");
            connection.setDoOutput(true);

            // Construir el cuerpo del mensaje SOAP
            String soapRequest = "<soapenv:Envelope xmlns:eng=\"urn:hpexstream-services/Engine\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                    + "<soapenv:Header>"
                    + "<wsse:Security soapenv:mustUnderstand=\"0\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">"
                    + "<wsse:UsernameToken wsu:Id=\"UsernameToken-1\">"
                    + "<wsse:Username>" + userHPExstream + "</wsse:Username>"
                    + "<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">" + paswHPExstream + "</wsse:Password>"
                    + "<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">9XbhOoet06M5XXD83sgM7Q==</wsse:Nonce>"
                    + "<wsu:Created>2015-07-21T08:23:30.207Z</wsu:Created>"
                    + "</wsse:UsernameToken>"
                    + "</wsse:Security>"
                    + "</soapenv:Header>"
                    + "<soapenv:Body>"
                    + "<eng:Compose>"
                    + "<EWSComposeRequest>"
                    + "<driver>"
                    + "<!--Fichero de datos en Base64-->"
                    + "<driver>" + xmlHPExstream + "</driver>"
                    + "<fileName>INPUT</fileName>"
                    + "</driver>"
                    + "<engineOptions>"
                    + "<name>IMPORTDIRECTORY</name>"
                    + "<value>/var/opt/exstream/pubs</value>"
                    + "</engineOptions>"
                    + "<engineOptions>"
                    + "<name>FILEMAP</name>"
                    + "<value>REFERENCIAS,/var/opt/exstream/pubs/" + emisionSector + "/REFERENCIAS.ini</value>"
                    + "</engineOptions>"
                    + "<pubFile>" + sPubFile + "</pubFile>"
                    + "</EWSComposeRequest>"
                    + "</eng:Compose>"
                    + "</soapenv:Body>"
                    + "</soapenv:Envelope>";

            // Imprimir la solicitud SOAP (puedes eliminar esto en producción)
            System.out.println("soapRequest: ");
            System.out.println(soapRequest);

            // Enviar la solicitud SOAP al servidor
            try (OutputStream os = connection.getOutputStream()) {
                byte[] input = soapRequest.getBytes("utf-8");
                os.write(input, 0, input.length);
            }

            // Recibir la respuesta del servidor
            try (BufferedReader br = new BufferedReader(new InputStreamReader(connection.getInputStream(), "utf-8"))) {
                StringBuilder response = new StringBuilder();
                String responseLine = null;
                while ((responseLine = br.readLine()) != null) {
                    response.append(responseLine.trim());
                }

                // Procesar la respuesta SOAP
                int iCadenaIni = response.indexOf("<fileOutput>") + 12;
                int iCadenaFin = response.indexOf("</fileOutput>");

                String sFileOutput = response.substring(iCadenaIni, iCadenaFin);

                byte[] yourByteArray = Base64.getDecoder().decode(sFileOutput);

                // Imprimir la respuesta (puedes eliminar esto en producción)
                System.out.println("Soap Response:");
                System.out.println(response.toString());
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
