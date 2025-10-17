using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Configuration;

namespace CSF.CITASWEB.WS
{
    public static class CifradoAES256
    {
        private static byte[] xSecretIV = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        //private static string xEncryptKey = "1234";
        public static string EncryptStringAES256(this string pCadena, string xEncryptKey = "")
        {
            SHA256 wSHA256 = SHA256.Create();
            if (String.IsNullOrEmpty(xEncryptKey)) xEncryptKey = ConfigurationManager.AppSettings["claveEncriptar"] != null ? ConfigurationManager.AppSettings["claveEncriptar"].ToString() : "1234";
            byte[] wEncryptKey = wSHA256.ComputeHash(Encoding.ASCII.GetBytes(xEncryptKey));

            Aes wAESEncriptadoCreado = Aes.Create();
            wAESEncriptadoCreado.Mode = CipherMode.CBC;

            byte[] wAESKey = new byte[32];
            Array.Copy(wEncryptKey, 0, wAESKey, 0, 32);
            wAESEncriptadoCreado.Key = wAESKey;
            wAESEncriptadoCreado.IV = xSecretIV;

            MemoryStream wMemoryStream = new MemoryStream();
            ICryptoTransform wAESCryptoTransform = wAESEncriptadoCreado.CreateEncryptor();
            CryptoStream wCryptoStream = new CryptoStream(wMemoryStream, wAESCryptoTransform, CryptoStreamMode.Write);

            byte[] wTextoBytes = Encoding.ASCII.GetBytes(pCadena);
            wCryptoStream.Write(wTextoBytes, 0, wTextoBytes.Length);
            wCryptoStream.FlushFinalBlock();
            byte[] wCifradoBytes = wMemoryStream.ToArray();

            wMemoryStream.Close();
            wCryptoStream.Close();

            string wCifradoTexto = Convert.ToBase64String(wCifradoBytes, 0, wCifradoBytes.Length);
            return wCifradoTexto;
        }

        public static string DecryptStringAES256(this string pCadena, string xEncryptKey = "")
        {
            SHA256 wSHA256 = SHA256Managed.Create();
            if (String.IsNullOrEmpty(xEncryptKey)) xEncryptKey = ConfigurationManager.AppSettings["claveEncriptar"] != null ? ConfigurationManager.AppSettings["claveEncriptar"].ToString() : "1234";
            byte[] wEncryptKey = wSHA256.ComputeHash(Encoding.ASCII.GetBytes(xEncryptKey));

            Aes wAESEncriptadoCreado = Aes.Create();
            wAESEncriptadoCreado.Mode = CipherMode.CBC;

            byte[] wAESKey = new byte[32];
            Array.Copy(wEncryptKey, 0, wAESKey, 0, 32);
            wAESEncriptadoCreado.Key = wAESKey;
            wAESEncriptadoCreado.IV = xSecretIV;

            MemoryStream wMemoryStream = new MemoryStream();
            ICryptoTransform wAESDesCryptoTransform = wAESEncriptadoCreado.CreateDecryptor();
            CryptoStream wCryptoStream = new CryptoStream(wMemoryStream, wAESDesCryptoTransform, CryptoStreamMode.Write);

            string wDescrifadoTexto = string.Empty;

            try
            {
                byte[] wCifradoBytes = Convert.FromBase64String(pCadena);
                wCryptoStream.Write(wCifradoBytes, 0, wCifradoBytes.Length);
                wCryptoStream.FlushFinalBlock();
                byte[] wTextoBytes = wMemoryStream.ToArray();

                wDescrifadoTexto = Encoding.ASCII.GetString(wTextoBytes, 0, wTextoBytes.Length);
            }
            finally
            {
                wMemoryStream.Close();
                wCryptoStream.Close();
            }

            return wDescrifadoTexto;
        }
    }
}