using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapfreHSBC.Models.General
{
    public enum EnumPerfil
    {
        Perfil1,
        Perfil2,
        Perfil3
    }
    public enum EnumSexo 
    {
        Femenino = 0,
        Masculino = 1
    }
    
    #region Clases
    public abstract class General
    {
        #region Constantes
        public const string ANIO = "{0}";
        #endregion

        #region Constructor
        public General()
        { }
        #endregion

        #region Clases

        #region Clase Respuesta
        public class Mail : AsSerializeable
        {
            public string recibe
            { get; set; }

            public string envia
            { get; set; }

            public string titulo
            { get; set; }

            public string texto
            { get; set; }
        }
        public class Respuesta : AsSerializeable
        {
            public int codigoError
            { get; set; }

            public string descripcionError
            { get; set; }
        }
        #endregion

        public class Key : AsSerializeable
        {
            [Required]
            public Int64? idTransaccion
            { get; set; }

            [Required]
            public string idCotizacionMapfre
            { get; set; }
        }

        public abstract class AsSerializeable
        {
            #region Métodos
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }

            public string Encrypt(string superKey, object obj)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;
                settings.ContractResolver = new General.EncryptedStringPropertyResolver(superKey);
                //msgJson = 
                return JsonConvert.SerializeObject(obj, settings);
            }
            #endregion

            //public string msgJson
            //{ get; set; }
        }
        #endregion

        #region Métodos Publicos
        public string EncryptedJSON(string publicKey, string key, string value)
        {
            string json = "{\"key\":\"secret_value\"}";
            json.Replace("key", key);
            json.Replace("secret_value", value);


            // Get the "pk" request parameter from the http request however you need to
            string base64PublicKey = publicKey;//request.getParameter("pk");
            string publicXmlKey = Encoding.ASCII.GetString(Convert.FromBase64String(base64PublicKey));

            // TODO: If you want the extra validation, insert "publicXmlKey" into the json value before 
            //       converting it to bytes
            // var jo = parse(json); jo.pk = publicXmlKey; json = jo.ToString();

            // Convert the string to bytes
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);

            // Encrypt the json using the public key provided by the client
            RSACryptoServiceProvider rsaEncrypt = new RSACryptoServiceProvider();
            rsaEncrypt.FromXmlString(publicXmlKey);
            byte[] encryptedJsonBytes = rsaEncrypt.Encrypt(jsonBytes, false);

            // Send the encrypted json back to the client
            return string.Join(",", encryptedJsonBytes);
        }
        #endregion

        #region Catalogos
        public static IEnumerable<SelectListItem> Sexo
        { 
            get {
                return Enum.GetValues(typeof(EnumSexo))
                    .Cast<EnumSexo>()
                    .Select(se => new SelectListItem
                    {
                        Text = se.ToString(),
                        Value = se.ToString() == "Femenino" ? "0" : "1",
                    }).AsEnumerable<SelectListItem>();
            }
        }

        public static IEnumerable<SelectListItem> Perfil
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Prudente", Value = "1" });
                items.Add(new SelectListItem { Text = "Moderado", Value = "2" });
                items.Add(new SelectListItem { Text = "Decidido", Value = "3" });
                return items;
            }
        }

        public static IEnumerable<SelectListItem> Cobro
        {
            get {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "1", Value = "1" });
                items.Add(new SelectListItem { Text = "2", Value = "2" });
                items.Add(new SelectListItem { Text = "3", Value = "3" });
                items.Add(new SelectListItem { Text = "4", Value = "4" });
                items.Add(new SelectListItem { Text = "5", Value = "5" });
                items.Add(new SelectListItem { Text = "6", Value = "6" });
                items.Add(new SelectListItem { Text = "7", Value = "7" });
                items.Add(new SelectListItem { Text = "8", Value = "8" });
                items.Add(new SelectListItem { Text = "9", Value = "9" });
                return items;
            }
        }

        public static IEnumerable<SelectListItem> Modalidad
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Unit Linked PPR", Value = "1" });
                return items;
            }
        }

        public static IEnumerable<SelectListItem> Plazo
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "EA 60 años", Value = "1" });
                items.Add(new SelectListItem { Text = "EA 70 años", Value = "2" });
                items.Add(new SelectListItem { Text = "EA 80 años", Value = "3" });
                return items;
            }
        }

        public static IEnumerable<SelectListItem> Periodicidad
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Unica", Value = "1" });
                items.Add(new SelectListItem { Text = "Mensual", Value = "2" });
                items.Add(new SelectListItem { Text = "Trimestral", Value = "3" });
                items.Add(new SelectListItem { Text = "Semestral", Value = "4" });
                items.Add(new SelectListItem { Text = "Anual", Value = "4" });
                return items;
            }
        }
        //Catalogo Productos
        public static IEnumerable<SelectListItem> Producto
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "-- Selecciona un Producto --", Value = "" });
                items.Add(new SelectListItem { Text = "Unite Linked", Value = "2" });

                return items;
            }
        }

        public static IEnumerable<SelectListItem> Productos
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Unit Linked", Value = "1" });
                return items;
            }
        }
        #endregion

        #region SHA256
        [AttributeUsage(AttributeTargets.Property)]
        public class JsonEncryptAttribute : Attribute
        {}

        public class EncryptedStringPropertyResolver : DefaultContractResolver
        {
            private byte[] encryptionKeyBytes;

            public EncryptedStringPropertyResolver(string encryptionKey)
            {
                if (encryptionKey == null)
                    throw new ArgumentNullException("encryptionKey");

                // Hash the key to ensure it is exactly 256 bits long, as required by AES-256
                using (SHA256Managed sha = new SHA256Managed())
                {
                    this.encryptionKeyBytes = 
                        sha.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
                }
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);

                // Find all string properties that have a [JsonEncrypt] attribute applied
                // and attach an EncryptedStringValueProvider instance to them
                foreach (JsonProperty prop in props.Where(p => p.PropertyType == typeof(string)))
                {
                    PropertyInfo pi = type.GetProperty(prop.UnderlyingName);
                    if (pi != null && pi.GetCustomAttribute(typeof(JsonEncryptAttribute), true) != null)
                    {
                        prop.ValueProvider = 
                            new EncryptedStringValueProvider(pi, encryptionKeyBytes);
                    }
                }

                return props;
            }

            class EncryptedStringValueProvider : Newtonsoft.Json.Serialization.IValueProvider
            {
                PropertyInfo targetProperty;
                private byte[] encryptionKey;

                public EncryptedStringValueProvider(PropertyInfo targetProperty, byte[] encryptionKey)
                {
                    this.targetProperty = targetProperty;
                    this.encryptionKey = encryptionKey;
                }

        
                public object GetValue(object target)
                {
                    string value = (string)targetProperty.GetValue(target);
                    byte[] buffer = Encoding.UTF8.GetBytes(value);

                    using (MemoryStream inputStream = new MemoryStream(buffer, false))
                    using (MemoryStream outputStream = new MemoryStream())
                    using (AesManaged aes = new AesManaged { Key = encryptionKey })
                    {
                        byte[] iv = aes.IV;  // first access generates a new IV
                        outputStream.Write(iv, 0, iv.Length);
                        outputStream.Flush();

                        ICryptoTransform encryptor = aes.CreateEncryptor(encryptionKey, iv);
                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                        {
                            inputStream.CopyTo(cryptoStream);
                        }

                        return Convert.ToBase64String(outputStream.ToArray());
                    }
                }

                public void SetValue(object target, object value)
                {
                    byte[] buffer = Convert.FromBase64String((string)value);

                    using (MemoryStream inputStream = new MemoryStream(buffer, false))
                    using (MemoryStream outputStream = new MemoryStream())
                    using (AesManaged aes = new AesManaged { Key = encryptionKey })
                    {
                        byte[] iv = new byte[16];
                        int bytesRead = inputStream.Read(iv, 0, 16);
                        if (bytesRead < 16)
                        {
                            throw new CryptographicException("IV está mal o es inváldo.");
                        }

                        ICryptoTransform decryptor = aes.CreateDecryptor(encryptionKey, iv);
                        using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.CopyTo(outputStream);
                        }

                        string decryptedValue = Encoding.UTF8.GetString(outputStream.ToArray());
                        targetProperty.SetValue(target, decryptedValue);
                    }
                }

            }
        }
        #endregion

        #region Example
         //JsonSerializerSettings settings = new JsonSerializerSettings();
         //   settings.Formatting = Formatting.Indented;
         //   settings.ContractResolver = new EncryptedStringPropertyResolver("My-Sup3r-Secr3t-Key");

         //   Console.WriteLine("----- Serialize -----");
         //   string json = JsonConvert.SerializeObject(user, settings);
         //   Console.WriteLine(json);
         //   Console.WriteLine();

         //   Console.WriteLine("----- Deserialize -----");
         //   UserInfo user2 = JsonConvert.DeserializeObject<UserInfo>(json, settings);

        #endregion
    }

    public class AttrProperty : Attribute
    {
        public string Header { get; set; }
    }
    #endregion
}