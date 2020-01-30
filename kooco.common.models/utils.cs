using System.Security.Cryptography;

namespace kooco.common.utils{

    public class tools{
        /// <summary>
        /// 字串加密 
        /// </summary>
        public static string EncryptAES(string text)
        {
            string result = "";

            if(!string.IsNullOrEmpty(text)){
                //AES KEY Generator
                //https://www.allkeysgenerator.com/Random/Security-Encryption-Key-Generator.aspx
                string strKey = "H@McQfTjWnZr4u7x";
                string strIV = "?E(H+MbQeThWmZq4";

                var sourceBytes = System.Text.Encoding.UTF8.GetBytes(text);
                var aes = System.Security.Cryptography.Aes.Create();
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(strKey);
                aes.IV = System.Text.Encoding.UTF8.GetBytes(strIV);
                var transform = aes.CreateEncryptor();
                result = System.Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
            }
            return result;
        }
        
        /// <summary>
        /// 字串解密
        /// </summary>        
        public static string DecryptAES(string text)
        {
            string result = "";

            if(!string.IsNullOrEmpty(text)){
                //AES KEY Generator
                //https://www.allkeysgenerator.com/Random/Security-Encryption-Key-Generator.aspx
                string strKey = "H@McQfTjWnZr4u7x";
                string strIV = "?E(H+MbQeThWmZq4";

                var encryptBytes = System.Convert.FromBase64String(text);
                var aes = System.Security.Cryptography.Aes.Create();
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(strKey);
                aes.IV = System.Text.Encoding.UTF8.GetBytes(strIV);
                var transform = aes.CreateDecryptor();
                result = System.Text.Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
            }
            return result;
        }
    }
}