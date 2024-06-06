using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

class Program
{
    static void Main()
    {
        string inputFile = "D:\\Laba\\ТИ\\input.txt";
        string encryptedFile = "D:\\Laba\\ТИ\\encrypted.txt";
        string decryptedFile = "D:\\Laba\\ТИ\\decrypted.txt";
        string keyFile = "D:\\Laba\\ТИ\\key.txt";

        byte[] key = new byte[16]; 
        SecureRandom rng = new SecureRandom(); 
        rng.NextBytes(key); // Генерация случайного ключа   
        File.WriteAllBytes(keyFile, key); 

        EncryptFile(inputFile, encryptedFile, keyFile);
           
        DecryptFile(encryptedFile, decryptedFile, keyFile);

        Console.WriteLine("Файлы зашифрованы и дешифрованы успешно.");
    }

    static void EncryptFile(string inputFile, string outputFile, string keyFile)
    {
        
        byte[] key = File.ReadAllBytes(keyFile);

        // Создание экземпляра шифра IDEA
        IdeaEngine cipher = new IdeaEngine();

        cipher.Init(true, new KeyParameter(key));

        using (var inputFileStream = new FileStream(inputFile, FileMode.Open))
        using (var outputFileStream = new FileStream(outputFile, FileMode.Create))
        {
            // Получение размера блока шифрования
            int blockSize = cipher.GetBlockSize();

            byte[] inputBuffer = new byte[blockSize];
            byte[] outputBuffer = new byte[blockSize];

            int bytesRead; 

            // Чтение данных из входного файла блоками и их шифрование
            while ((bytesRead = inputFileStream.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
            {
                
                if (bytesRead == blockSize)
                {
                    int outputLength = cipher.ProcessBlock(inputBuffer, 0, outputBuffer, 0);
                    outputFileStream.Write(outputBuffer, 0, outputLength);
                }
                else
                {
                    byte[] paddedInput = new byte[blockSize];
                    Array.Copy(inputBuffer, paddedInput, bytesRead);

                    int outputLength = cipher.ProcessBlock(paddedInput, 0, outputBuffer, 0);
                    outputFileStream.Write(outputBuffer, 0, outputLength);
                }
            }
        }
    }


    static void DecryptFile(string inputFile, string outputFile, string keyFile)
    {
        byte[] key = File.ReadAllBytes(keyFile);

        // Создание экземпляра шифра IDEA
        IdeaEngine cipher = new IdeaEngine();

        cipher.Init(false, new KeyParameter(key));

        // Открытие потоков для чтения зашифрованного файла и записи расшифрованных данных в выходной файл
        using (var inputFileStream = new FileStream(inputFile, FileMode.Open))
        using (var outputFileStream = new FileStream(outputFile, FileMode.Create))
        {
            // Получение размера блока шифрования555555
            int blockSize = cipher.GetBlockSize();

            byte[] inputBuffer = new byte[blockSize];
            byte[] outputBuffer = new byte[blockSize];

            int bytesRead; 

            while ((bytesRead = inputFileStream.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
            {
                int outputLength = cipher.ProcessBlock(inputBuffer, 0, outputBuffer, 0);
                outputFileStream.Write(outputBuffer, 0, outputLength);
            }
        }
    }


}
