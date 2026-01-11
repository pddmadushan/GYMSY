using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CMSXtream.Handlers
{
    internal class HashGenerator
    {
        private static readonly byte[] Salt =
        Encoding.UTF8.GetBytes("Gymsy123456^%$#@!");
        private static string passCode = "Gymsy00987&*()";

        public static string Encrypt(string text)
        {
            using (var aes = new RijndaelManaged())
            using (var key = new Rfc2898DeriveBytes(passCode, Salt, 1000))
            {
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms,
                       aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.Close();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        public static string Decrypt(string cipherText)
        {
            using (var aes = new RijndaelManaged())
            using (var key = new Rfc2898DeriveBytes(passCode, Salt, 1000))
            {
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms,
                       aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public static string ResolveKey(string Key)
        {
            return RemoveNonDigits(Key);
        }
        static string RemoveNonDigits(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
