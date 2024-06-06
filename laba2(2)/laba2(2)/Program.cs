using System;
using System.IO;
using System.Linq;

class VigenereDecryption
{
    static void Main()
    {
        Console.WriteLine("Введите ключевое слово для дешифрования:");
        string keyword = Console.ReadLine().ToUpper();

        string encryptedText = File.ReadAllText(@"D:\Laba\ТИ\output2.txt").ToUpper();
        string repeatedKeyword = GenerateRepeatedKeyword(encryptedText, keyword);
        string decryptedText = DecryptVigenere(encryptedText, repeatedKeyword);

        Console.WriteLine("Расшифрованный текст: " + decryptedText);
    }

    static string GenerateRepeatedKeyword(string text, string keyword)
    {
        int repeats = text.Length / keyword.Length;
        int remainder = text.Length % keyword.Length;
        string repeatedKeyword = string.Concat(Enumerable.Repeat(keyword, repeats)) + keyword.Substring(0, remainder);
        return repeatedKeyword;
    }

    static string DecryptVigenere(string encryptedText, string keyword)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] decryptedChars = new char[encryptedText.Length];

        for (int i = 0; i < encryptedText.Length; i++)
        {
            char currentChar = encryptedText[i];
            if (alphabet.Contains(currentChar))
            {
                int shift = alphabet.IndexOf(keyword[i]);
                int charIndex = (alphabet.IndexOf(currentChar) - shift + alphabet.Length) % alphabet.Length;
                decryptedChars[i] = alphabet[charIndex];
            }
            else
            {
                decryptedChars[i] = currentChar; // Preserve non-alphabetic characters
            }
        }

        return new string(decryptedChars);
    }
}