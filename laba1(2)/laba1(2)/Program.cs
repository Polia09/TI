using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string encodedText = File.ReadAllText("D:\\Laba\\ТИ\\hf.txt");
        Dictionary<string, string> huffmanTable = LoadHuffmanTable("D:\\ТИ\\w.txt");

        // Декодируем закодированный текст с использованием таблицы Хаффмана
        string decodedText = DecodeHuffman(encodedText, huffmanTable);

        Console.WriteLine("Декодированный текст:");
        Console.WriteLine(decodedText);

        File.WriteAllText("D:\\ТИ\\output.txt", decodedText);
    }

    // Метод для загрузки таблицы Хаффмана из файла
    static Dictionary<string, string> LoadHuffmanTable(string filePath)
    {
        Dictionary<string, string> huffmanTable = new Dictionary<string, string>();

        // Читаем все строки из файла
        string[] lines = File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Count() - 1; i += 2)
            huffmanTable.Add(lines[i], lines[i + 1]);


        return huffmanTable;
    }

    // Метод для декодирования закодированного текста с использованием таблицы Хаффмана
    static string DecodeHuffman(string encodedText, Dictionary<string, string> huffmanTable)
    {
        string decodedText = "";
        string currentCode = "";

        foreach (char bit in encodedText)
        {
            currentCode += bit;
            if (huffmanTable.ContainsValue(currentCode))
            {
                string symbol = huffmanTable.FirstOrDefault(x => x.Value == currentCode).Key;
                decodedText += symbol;
                currentCode = "";
            }
        }

        return decodedText;
    }
}

