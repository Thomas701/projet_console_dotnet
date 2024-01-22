using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Projet_Console_Serialisation_Data
{
    public static class CryptoManager
    {
        private const int KeySize = 256;

        public static void EncryptFile(string inputFile, string outputFile, string key)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key.PadRight(KeySize / 8));
            aesAlg.Mode = CipherMode.CBC; 

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            {
                aesAlg.GenerateIV();
                fsOutput.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                using (CryptoStream cryptoStream = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                {
                    fsInput.CopyTo(cryptoStream);
                }
            }
        }

        public static void DecryptFile(string inputFile, string outputFile, string key)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key.PadRight(KeySize / 8));
            aesAlg.Mode = CipherMode.CBC; 

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            {
                byte[] iv = new byte[aesAlg.IV.Length];
                fsInput.Read(iv, 0, iv.Length);
                aesAlg.IV = iv;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                using (CryptoStream cryptoStream = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(fsOutput);
                }
            }
        }
    }
}