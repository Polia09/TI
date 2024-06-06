using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

class Program
{
    static void Main()
    {

        string filePath = "D:\\Laba\\ТИ\\input.txt";
        string text = File.ReadAllText(filePath);

        
        var frequencyTable = BuildFrequencyTable(text);// Строим таблицу частот встречаемости символов в тексте
        var huffmanTree = BuildHuffmanTree(frequencyTable);// Строим дерево Хаффмана на основе таблицы частот
        var huffmanCodes = BuildHuffmanCodes(huffmanTree);// Строим коды Хаффмана на основе построенного дерева

        PrintTable(frequencyTable, huffmanCodes);// Выводим таблицу частот встречаемости и соответствующие коды Хаффмана
        
        double entropy = CalculateEntropy(frequencyTable);// Вычисляем энтропию текста
        double averageLength = CalculateAverageLength(frequencyTable, huffmanCodes);

        
        string encodedText = EncodeText(text, huffmanCodes);// Кодируем исходный текст с помощью построенных кодов Хаффмана
        SaveEncodedText(encodedText, "D:\\Laba\\ТИ\\hf.txt");

        Console.WriteLine("\nЭнтропия: " + entropy);
        Console.WriteLine("Средняя длина кодового слова: " + averageLength);
        CompareEntropyAndAverageLength(entropy, averageLength);
        Console.WriteLine("Текст на коде Хаффмана:\n" + encodedText);
    }

    // Метод для построения таблицы частот встречаемости символов в тексте
    static Dictionary<char, int> BuildFrequencyTable(string text)  // Метод возвращает словарь, где ключ - это символ, а значение - количество его встреч в тексте
                                                                   
    {
        return text.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
    }

    // Метод принимает словарь с частотами символов в качестве входного параметра
    static Node BuildHuffmanTree(Dictionary<char, int> frequencyTable)
    {
        
        var nodes = new List<Node>(frequencyTable.Select(kvp => new Node { Symbol = kvp.Key, Frequency = kvp.Value }));// Создаем список узлов, где каждый узел представляет символ и его частоту

        while (nodes.Count > 1)
        {
            
            var orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();// Сортируем узлы по их частоте в возрастающем порядке, чтобы выбрать узлы с наименьшей частотой
            if (orderedNodes.Count >= 2)
            {
                var taken = orderedNodes.Take(2).ToList();
                // Создаем родительский узел, у которого символом будет '*'. Левым потомком становится первый взятый узел, а правым - второй
                var parent = new Node
                {
                    Symbol = '*',
                    Frequency = taken[0].Frequency + taken[1].Frequency,
                    Left = taken[0],
                    Right = taken[1]
                };
                // Удаляем взятые узлы из списка и добавляем новый родительский узел
                nodes.Remove(taken[0]);
                nodes.Remove(taken[1]);
                nodes.Add(parent);
            }
        }
        return nodes.FirstOrDefault();// Возвращаем первый (единственный) узел в списке, который будет корнем дерева Хаффмана
    }

    // Метод для построения кодов Хаффмана на основе дерева
    static Dictionary<char, string> BuildHuffmanCodes(Node node)
    {
        if (node == null)
            return new Dictionary<char, string>();

        if (node.IsLeaf)   // Если узел является листом, возвращаем словарь с кодом пустой строки для этого символа
            return new Dictionary<char, string> { { node.Symbol, "" } };

       
        var codes = new Dictionary<char, string>(); // Создаем словарь для хранения кодовых слов
        TraverseHuffmanTree(node, "", codes);// Обходим дерево Хаффмана, начиная с пустой строки в качестве начального кода
        return codes;   // Возвращаем словарь с кодами Хаффмана
    }

    // Метод для обхода дерева Хаффмана и построения кодовых слов
    static void TraverseHuffmanTree(Node node, string code, Dictionary<char, string> codes)
    {
        if (node == null)
            return;

        // Если узел является листом, добавляем его символ и кодовое слово в словарь, и завершаем обход
        if (node.IsLeaf)
        {
            codes[node.Symbol] = code;
            return;
        }

        TraverseHuffmanTree(node.Left, code + "0", codes);  // Рекурсивный вызов для левого потомка с добавлением "0" к текущему коду
        TraverseHuffmanTree(node.Right, code + "1", codes);
    }

    // Метод для добавления таблицы частот и кодов Хаффмана в файл
    static void AppendTableToFile(Dictionary<char, int> frequencyTable, Dictionary<char, string> huffmanCodes, string filePath)
    {
        // Создаем объект StreamWriter для записи в файл с возможностью добавления в конец файла
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            foreach (var kvp in huffmanCodes)// Итерируем по кодам Хаффмана
            {
                double probability = (double)frequencyTable[kvp.Key] / frequencyTable.Sum(x => x.Value);// Вычисляем вероятность появления символа в исходных данных
                writer.WriteLine($"{kvp.Key}");
                writer.WriteLine($"{kvp.Value}");
            }
        }
    }


    // Метод для вывода таблицы частот и кодов Хаффмана
    static void PrintTable(Dictionary<char, int> frequencyTable, Dictionary<char, string> huffmanCodes)
    {
        Console.WriteLine("{0,-10} {1,-12} {2,-15} {3,-10}", "Символ", "Вероятность", "Кодовое слово", "Длина кодового слова");// Выводим заголовки столбцов таблицы с выравниванием
        foreach (var kvp in huffmanCodes)
        {
            double probability = (double)frequencyTable[kvp.Key] / frequencyTable.Sum(x => x.Value);// Вычисляем вероятность встречи символа в исходных данных
            Console.WriteLine($"{kvp.Key,-10} {probability,-12:F5} {kvp.Value,-15} {kvp.Value.Length,-10}");
        }
        AppendTableToFile(frequencyTable, huffmanCodes, "D:\\Laba\\ТИ\\w.txt");
    }

    // Метод для вычисления энтропии текста на основе таблицы частот символов
    static double CalculateEntropy(Dictionary<char, int> frequencyTable)
    {
        double entropy = 0;
        int totalChars = frequencyTable.Sum(x => x.Value);// Вычисляем общее количество символов в тексте
        foreach (var kvp in frequencyTable)
        {
            double probability = (double)kvp.Value / totalChars; // Вычисляем вероятность встречи символа в тексте
            entropy -= probability * Math.Log(probability);// Вычисляем вклад символа в энтропию и суммируем
        }
        return entropy;
    }

    // Метод для вычисления средней длины кодового слова на основе таблицы частот и кодов Хаффмана
    static double CalculateAverageLength(Dictionary<char, int> frequencyTable, Dictionary<char, string> huffmanCodes)
    {
        double avgLength = 0;
        int totalChars = frequencyTable.Sum(x => x.Value);
        foreach (var kvp in frequencyTable)
        {
            double probability = (double)kvp.Value / totalChars;
            avgLength += probability * huffmanCodes[kvp.Key].Length;// Учитываем в средней длине длину кодового слова, взвешенную вероятностью символа
        }
        return avgLength;
    }

    static void CompareEntropyAndAverageLength(double entropy, double averageLength)
    {
        if (entropy > averageLength)
        {
            Console.WriteLine("Энтропия больше средней длины кодового слова");
        }
        else if (entropy < averageLength)
        {
            Console.WriteLine("Энтропия меньше средней длины кодового слова");
        }
        else
        {
            Console.WriteLine("Энтропия равна средней длине кодового слова");
        }
    }

    // Метод для кодирования исходного текста с использованием кодов Хаффмана
    static string EncodeText(string text, Dictionary<char, string> huffmanCodes)
    {
        // Применяем коды Хаффмана к каждому символу исходного текста и объединяем результат в одну строку
        return string.Concat(text.Select(c => huffmanCodes[c]));
    }

    static void SaveEncodedText(string encodedText, string filePath)
    {
        File.WriteAllText(filePath, encodedText);
    }
}

class Node
{
    public char Symbol { get; set; }
    public int Frequency { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }

    public bool IsLeaf => Left == null && Right == null;  // Проверка, является ли узел листом
}