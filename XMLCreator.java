package xmltest;

import java.io.StringWriter;
import java.util.List;
import java.util.Map;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;


public class XMLCreator {

    public static String creaXML_HP(String ramo, String num_cotizacion, String edad, String sexo, String plazo,
            String prima_ini, String prima_adi, String periodo, 
            String sumaasegB, String sumaasegO, String nomcliente, String pFrac, String moneda) throws Exception {
    	Document xmlDoc = null;

    	DocumentBuilderFactory documentBuilderFactory = DocumentBuilderFactory.newInstance();
    	xmlDoc = documentBuilderFactory.newDocumentBuilder().newDocument();

        Element xmlRoot = xmlDoc.createElement("ROOT");
        xmlDoc.appendChild(xmlRoot);
        
        xmlRoot.appendChild(createNode(xmlDoc, "COMPANIA", "1"));	
        xmlRoot.appendChild(createNode(xmlDoc, "RAMO", ramo));
        xmlRoot.appendChild(createNode(xmlDoc, "ACTIVIDAD", "COTIZACION"));
        xmlRoot.appendChild(createNode(xmlDoc,  "IDIOMA", "mx_ES"));
        
        Element elementDist = createNode(xmlDoc, "DISTRIBUCION", null);
        
        Element element = createNode(xmlDoc, "DUPLEX_MODE", "false");
        elementDist.appendChild(element);
        
        element = createNode(xmlDoc, "LOCAL", "true");
        elementDist.appendChild(element);
        
        xmlRoot.appendChild(elementDist);
        
        Element elementCabecera = createNode(xmlDoc, "CABECERA", null);
        
        element = createNode(xmlDoc, "PRODUCTO", "var CONTIGO EN TU INVERSIÓN");
        elementCabecera.appendChild(element);
        
        element = createNode(xmlDoc, "NUMERO_COTIZACION", num_cotizacion);
        elementCabecera.appendChild(element);
        
        element = createNode(xmlDoc, "FECHA_COTIZACION", "var fecha now");
        elementCabecera.appendChild(element);
        
        xmlRoot.appendChild(elementCabecera);
        
        xmlRoot.appendChild(createNode(xmlDoc,  "PIE", "var CV_PIE_COTIZACION_UNIT_LINKED.rtf"));

        xmlRoot.appendChild(createNode(xmlDoc, "VI_ESTADO" ,null));
        
        xmlRoot.appendChild(createNode(xmlDoc,  "VI_POBLACION", null));
        
        xmlRoot.appendChild(createNode(xmlDoc,  "VI_FECHA", "var dddd dd DE MMMM DE yyyy"));
        
        xmlRoot.appendChild(createNode(xmlDoc,  "VI_DIAS_VALIDEZ", "30"));
        
        xmlRoot.appendChild(createNode(xmlDoc,  "REG_MERCANTIL", "var CV_REGISTRO_MERCANTIL_MAPFRE_MEXICO.rtf"));
        
        Element elementInsured = createNode(xmlDoc, "ASEGURADO", null);
        
        element = createNode(xmlDoc, "NOMBRE", nomcliente);
        elementInsured.appendChild(element);
        
        element = createNode(xmlDoc, "EDAD", edad);
        elementInsured.appendChild(element);
        
        element = createNode(xmlDoc, "SEXO", "var MASCULINO"); //1 Masculino
        elementInsured.appendChild(element);
        
        element = createNode(xmlDoc, "CORREO", null);
        elementInsured.appendChild(element);
        
        xmlRoot.appendChild(elementInsured);
        
        //Convert to double and format pfrac, prima_ini and prima_adi
        
        Element elementQuote= createNode(xmlDoc, "DATOS_COTIZACION", null);
        
        element = createNode(xmlDoc, "MODALIDAD", "var CONTIGO EN TU INVERSIÓN"); //11204
        elementQuote.appendChild(element);
        
        element = createNode(xmlDoc, "MONEDA", "moneda");
        elementQuote.appendChild(element);
        
        element = createNode(xmlDoc, "PLAZO_SEGURO", plazo + " AÑOS");
        elementQuote.appendChild(element);
        
        element = createNode(xmlDoc, "FRECUENCIA_PRIMA_ADICIONAL", periodo); 
        elementQuote.appendChild(element);
        
        xmlRoot.appendChild(elementQuote);
        
        /*element = createNode(xmlDoc, "PRIMA_INICIAL", prima_ini); //format
        elementQuote.appendChild(element);
        
        
        
        if(!prima_adi.equals("0")) {
	        element = createNode(xmlDoc, "PRIMA_ADICIONAL", prima_adi); //1 Masculino
	        elementQuote.appendChild(element);
	        
	        element = createNode(xmlDoc, "FRECUENCIA_PRIMA_ADICIONAL", periodo); 
	        elementQuote.appendChild(element);
	        
	        
        }*/
        
        Element elementCoverage = createNode(xmlDoc, "COBERTURAS", null);
        
        element = createNode(xmlDoc, "ENCABEZADO_PRIMA", "PRIMA ANUAL"); //11204
        elementCoverage.appendChild(element);
        
        //for
        
		Element elementSingleCoverage = createNode(xmlDoc, "COBERTURA", null);
		        
        element = createNode(xmlDoc, "DESCRIPCION", "FALLECIMIENTO"); 
        elementSingleCoverage.appendChild(element);
        		
        element = createNode(xmlDoc, "SUMA_ASEGURADA", "50,000.00"); 
        elementSingleCoverage.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA", "114.28"); 
        elementSingleCoverage.appendChild(element);
        
        elementCoverage.appendChild(elementSingleCoverage);
        
        xmlRoot.appendChild(elementCoverage);
        
        
        
        xmlRoot.appendChild(createNode(xmlDoc,  "TOTAL_PRIMA", "114.28"));
        
        xmlRoot.appendChild(createNode(xmlDoc,  "VI_SUMA_MUERTE_ACCIDENTAL", null));
        
        //aportaciones
        
        Element elementContribution = createNode(xmlDoc, "APORTACIONES", null);
        
        Element elementIni = createNode(xmlDoc, "APORTACION_INICIAL", null); 
        
        element = createNode(xmlDoc, "ENCABEZADO", "APORTACION INICIAL:"); 
        elementIni.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA_DE_AHORRO", prima_ini); 
        elementIni.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA_DE_RIESGO", pFrac); 
        elementIni.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA_TOTAL", "sum"); 
        elementIni.appendChild(element);
        
        elementContribution.appendChild(elementIni);
        
        
        Element elementAdi = createNode(xmlDoc, "APORTACION_ADICIONAL", null); 
        
        element = createNode(xmlDoc, "ENCABEZADO", "APORTACIONES ADICIONALES:"); 
        elementAdi.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA_DE_AHORRO", prima_adi); 
        elementAdi.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA_DE_RIESGO", pFrac); 
        elementAdi.appendChild(element);
        
        element = createNode(xmlDoc, "PRIMA_TOTAL", "sum"); 
        elementAdi.appendChild(element);
        
        elementContribution.appendChild(elementAdi);
        
        xmlRoot.appendChild(elementContribution);
        
        //fondo
        
        Element elementDistri = createNode(xmlDoc, "TASAS_INTERES", null);
        //for
        
        Element elementInv = createNode(xmlDoc, "INVERSION", null); 
        
        element = createNode(xmlDoc, "PERFIL", "Conservador1"); 
        elementInv.appendChild(element);
        
        element = createNode(xmlDoc, "TIPO_INVERSION", "Actigob"); 
        elementInv.appendChild(element);
        
        element = createNode(xmlDoc, "PCT_INVERSION", "50"); 
        elementInv.appendChild(element);
        
        element = createNode(xmlDoc, "MONTO_INICIAL", "1,431.83"); 
        elementInv.appendChild(element);
        
        elementDistri.appendChild(elementInv);
        
        xmlRoot.appendChild(elementDistri);
        
        
        Element elementPrivacy = createNode(xmlDoc, "AVISO_PRIVACIDAD", null);
        
        element = createNode(xmlDoc, "CONTENIDO", "var CV_ANEXO_AVISO_PRIVACIDAD.rtf");
        elementPrivacy.appendChild(element);
        
        xmlRoot.appendChild(elementPrivacy);
        
        Element elementImages = createNode(xmlDoc, "IMAGENES", null);
        
        element = createNode(xmlDoc, "PUBLICIDAD", "var CV_PUBLICIDAD_UNIT.jpg");
        elementImages.appendChild(element);
        
        element = createNode(xmlDoc, "VENTA_CRUZADA", "var CV_VENTA_UNIT.jpg");
        elementImages.appendChild(element);
        
        xmlRoot.appendChild(elementImages);
        
        
        //xmlDoc.appendChild(xmlRoot);
        //System.out.println(xmlRoot);
        // ... Resto del código ...

        //return System.Web.HttpUtility.HtmlDecode(xmlDoc.toString());
        Transformer transformer = TransformerFactory.newInstance().newTransformer();
        transformer.setOutputProperty(OutputKeys.INDENT, "yes");
        StringWriter stringWriter = new StringWriter();
        transformer.transform(new DOMSource(xmlDoc), new StreamResult(stringWriter));

        return stringWriter.toString();
    }
    
    //public static void createNode(Document docXml, Element parentXml, String tagName, String innerText) {
    //	Element childXml = docXml.createElement(tagName);
    //	childXml.appendChild(docXml.createTextNode(innerText));
    //	parentXml.appendChild(childXml);
    //}
    
    public static Element createNode(Document docXml, String tagName, String innerText) {
    	Element childXml = docXml.createElement(tagName);
    	if(innerText != null)
    		childXml.appendChild(docXml.createTextNode(innerText));;
    	return childXml;
    }

    public static void main(String[] args) {
        String xml;
		try {
			//(String ramo, String num_cotizacion, String edad, String sexo, String plazo,
            //String prima_ini, String prima_adi, String periodo, 
            //String sumaasegB, String sumaasegO, String nomcliente, String pFrac, String moneda)
			xml = creaXML_HP("Ramo", "123", "25", "1", "12", "1000", "200", "Mensual", "100000", "10500", "Nombre","5", "USD");
			System.out.println(xml);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
        
    }
}
