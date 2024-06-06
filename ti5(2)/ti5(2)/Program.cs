using System.Numerics;
using System.Text.RegularExpressions;

List<int> TextToList(string text)// Определение функции для преобразования текста в список целых чисел
{
    List<int> list = new List<int>();
    foreach (char c in text)// Перебор каждого символа во входном тексте
    {
        list.Add(Convert.ToInt32(c));
    }
    return list;
}

List<int> ReadKey(string key)// Определение функции для чтения ключа из строки
{
    List<int> keys = new List<int>();
    foreach (Match match in Regex.Matches(key, @"-?\d+"))// Поиск всех числовых значений в строке с помощью регулярного выражения
    {
        keys.Add(Convert.ToInt32(match.ToString()));
    }
    return keys;
}


void Encrypt(string text, List<int> key)
{
    List<int> list = new List<int>();
    list = TextToList(text);// Преобразование текста в список целых чисел
    string result = "";
    foreach (var l in list)
    {
        result += BigInteger.ModPow(l, key[0], key[1]) + " ";
    }
    File.WriteAllText(@"D:\Laba\ТИ\ti5(1)\ti5(1)\bin\Debug\net8.0\encrypt.txt", result);

}
Console.WriteLine("Введите текст:");
string text = Console.ReadLine();
Console.WriteLine("Введите полученный ключ:");
string key = Console.ReadLine();

Encrypt(text, ReadKey(key));