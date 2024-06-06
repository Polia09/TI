using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace VisualConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Зашифровать текст с помощью шифра Виженера.");
            Console.WriteLine("2. Расшифровать текст с помощью шифра Виженера.");
            Console.WriteLine("3. Криптоанализ зашифрованного текста с помощью шифра Виженера.");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    string text1 = File.ReadAllText("D:\\Laba\\ТИ\\input2.txt").ToUpper();
                    string key1 = File.ReadAllText("D:\\Laba\\ТИ\\key2.txt").ToUpper();

                    string encryptedText1 = Encrypt(text1, key1);
                    File.WriteAllText(@"D:\Laba\ТИ\\output2.txt", encryptedText1);
                    break;
                case 2:
                    string text2 = File.ReadAllText("D:\\Laba\\ТИ\\output2.txt").ToUpper();
                    string key2 = File.ReadAllText("D:\\Laba\\ТИ\\key2.txt").ToUpper();

                    string decryptedText2 = Decrypt(text2, key2);
                    Console.WriteLine(decryptedText2);
                    break;
                case 3:
                    string text3 = File.ReadAllText("D:\\Laba\\ТИ\\output2.txt").ToUpper();
                    string key3 = File.ReadAllText("D:\\Laba\\ТИ\\key2.txt").ToUpper();

                    int nGramLength = 3;
                    List<string> nGrams = FindNGrams(text3, nGramLength);
                    Dictionary<string, List<int>> repeatedNGrams = FindRepeatedNGrams(nGrams);

                    int keyLength = FindKeyLength(repeatedNGrams.Values);
                    string foundKey = FindKey(text3, keyLength);

                    TextLengthExperiment(text3, key3);
                    Console.WriteLine();
                    KeyLengthExperiment(text3, key3);
                    Console.WriteLine($"Длина ключа: {keyLength} Ключ: {foundKey}");
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, запустите программу снова и выберите правильную опцию.");
                    break;
            }
        }
        static string Encrypt(string inputText, string key)
        {
            inputText = inputText.ToUpper();
            key = key.ToUpper();
            string result = "";
            int keyLength = key.Length;
            for (int i = 0; i < inputText.Length; i++)
            {
                char symbol = inputText[i];
                if (char.IsLetter(symbol))
                {
                    char shift = 'А';
                    int encryptedCharIndex = ((symbol - shift) + (key[i % keyLength] - shift)) % 32;
                    char encryptedChar = (char)(shift + encryptedCharIndex);
                    result += encryptedChar;
                }
                else
                {
                    result += symbol;
                }
            }

            return result;
        }
        static string Decrypt(string encryptedText, string key)
        {
            encryptedText = encryptedText.ToUpper();
            key = key.ToUpper();
            string result = "";
            int keyLength = key.Length;
            for (int i = 0; i < encryptedText.Length; i++)
            {
                char symbol = encryptedText[i];
                if (char.IsLetter(symbol))
                {
                    char shift = 'А';
                    int decryptedCharIndex = ((symbol - shift) - (key[i % keyLength] - shift) + 32) % 32;
                    char decryptedChar = (char)(shift + decryptedCharIndex);
                    result += decryptedChar;
                }
                else
                {
                    result += symbol;
                }
            }

            return result;
        }


        static string FindKey(string encryptedText, int keyLength)
        {
            string foundKey = "";
            for (int i = 0; i < keyLength; i++)
            {
                string column = new string(encryptedText
                .Where((c, index) => char.IsLetter(c) && index % keyLength == i)  
                .ToArray());
                char mostFrequentChar = column
                .GroupBy(c => c)
                .Where(group => char.IsLetter(group.Key) && group.Any())
                .OrderByDescending(group => group.Count())// Сортируем группы по количеству элементов в убывающем порядке.
                .Select(group => group.Key)
                .FirstOrDefault();// Получаем первый символ или значение по умолчанию, если группы нет.
                if (mostFrequentChar != default(char))
                {
                    int shift = (mostFrequentChar - 'О' + 32) % 32;
                    char keyChar = (char)('А' + shift);
                    foundKey += keyChar;
                }
            }
            return foundKey;
        }
        static List<string> FindNGrams(string text, int n)
        {
            List<string> nGrams = new List<string>();
            for (int i = 0; i < text.Length - n + 1; i++)
            {
                string nGram = text.Substring(i, n); // Получение подстроки текста длиной n символов, начиная с текущего индекса i.
                nGrams.Add(nGram); // Добавление полученной n-граммы в список.
            }
            return nGrams;
        }
        static Dictionary<string, List<int>> FindRepeatedNGrams(List<string> nGrams)
        {
            Dictionary<string, List<int>> repeatedNGrams = new Dictionary<string,
            List<int>>();
            for (int i = 0; i < nGrams.Count - 1; i++)// Внешний цикл перебирает все n-граммы, кроме последней.
            {
                for (int j = i + 1; j < nGrams.Count; j++)// Внутренний цикл
                {
                    if (nGrams[i] == nGrams[j])
                    {
                        string nGram = nGrams[i]; // Получение повторяющейся n-граммы.
                        int distance = j - i;
                        if (!repeatedNGrams.ContainsKey(nGram))
                        {
                            repeatedNGrams[nGram] = new List<int>();
                        }
                        repeatedNGrams[nGram].Add(distance);
                    }
                }
            }
            return repeatedNGrams;
        }

        static int FindKeyLength(IEnumerable<List<int>> distances)
        {
            var allDistances = distances.SelectMany(d => d).ToList();// Создание списка всех расстояний путем объединения всех списков расстояний в один.
            int nod = 0;
            for (int i = 0; i < allDistances.Count - 1; i++)
            {
                for (int j = i + 1; j < allDistances.Count; j++)
                {
                    nod = NOD(allDistances[i], allDistances[j]);
                    if (nod > 1) // Проверка, является ли найденный НОД больше 1 (т.е. расстояния имеют общий делитель).
                    {
                        return nod; // Возврат НОДа, если он больше 1, т.е. общий делитель найден.
                    }
                }
            }
            return nod;
        }
        static int NOD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static void TextLengthExperiment(string encryptedText, string key)
        {
            Console.WriteLine("Исследование по длинне текста:");
            for (int i = 100; i < encryptedText.Length; i += 100)
            {
                string substring = encryptedText.Substring(0, i);
                int nGramLength = 3;
                List<string> nGrams = FindNGrams(substring, nGramLength);
                Dictionary<string, List<int>> repeatedNGrams = FindRepeatedNGrams(nGrams);
                int keyLength = FindKeyLength(repeatedNGrams.Values);
                string foundKey = FindKey(substring, keyLength);
                string result = (key == foundKey) ? "Взломано" : "Невзломано";
                Console.WriteLine($"{result} при длине текста {i}");
            }
        }
        static void KeyLengthExperiment(string encryptedText, string key)
        {
            Console.WriteLine("Исследование по длине ключа: ");
            for (int keyLength = 1; keyLength <= 20; keyLength++)
            {
                string foundKey = FindKey(encryptedText, keyLength);

                string result = (key == foundKey) ? "Взломано" : "Невзломано";
                Console.WriteLine($"{result} при длине ключа: {keyLength}");
                Console.WriteLine(foundKey);
            }
        }
    }
}