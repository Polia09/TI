using System;
using System.IO;
using System.Security.Cryptography;

class Program
{
    static string CalculateSHA1(string filePath)// Создаем экземпляр объекта для вычисления SHA-1 хеша
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha1.ComputeHash(stream);// Преобразуем массив байт хеша в строку шестнадцатеричного представления
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    static byte[] GenerateDigitalSignature(string filePath, RSAParameters privateKey)//генерации цифровой подписи файла с помощью закрытого ключа RSA.
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            byte[] hash = sha1.ComputeHash(fileData);// Вычисляем хеш для содержимого файла

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);// Импортируем параметры закрытого ключа RSA
                return rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);// Подписываем хеш с использованием закрытого ключа RSA и алгоритма SHA-1
            }
        }
    }

    static bool VerifyDigitalSignature(string filePath, byte[] signature, RSAParameters publicKey)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            byte[] hash = sha1.ComputeHash(fileData);// Вычисляем хеш для содержимого файла

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);// Проверяем подпись с использованием открытого ключа RSA и алгоритма SHA-1
                return rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
        }
    }

    static void Main()
    {
        string filePath = "D:\\Laba\\ТИ\\input.txt";
        string sha1Hash = CalculateSHA1(filePath);
        Console.WriteLine($"SHA-1 хеш файла: {sha1Hash}");

        // Генерация и использование ключей RSA
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            // Генерация приватного и открытого ключей
            RSAParameters privateKey = rsa.ExportParameters(true);
            RSAParameters publicKey = rsa.ExportParameters(false);

            // Генерация ЭЦП и проверка подписи
            byte[] signature = GenerateDigitalSignature(filePath, privateKey);
            Console.WriteLine("ЭЦП сгенерирована.");
            Console.Write("нажмите enter чтобы продолжить");
            Console.Read();
            bool isVerified = VerifyDigitalSignature(filePath, signature, publicKey);
            Console.WriteLine($"Проверка подписи: {isVerified}");
        }
    }
}
