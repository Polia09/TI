using System.Numerics;
using System.Text.RegularExpressions;

Console.Write("Введите простое число p: ");
int p;
while (!int.TryParse(Console.ReadLine(), out p) || !IsPrime(p)) { Console.Write("Неверный ввод. Введите простое число p: "); }
Console.Write("Введите простое число q: ");
int q;
while (!int.TryParse(Console.ReadLine(), out q) || !IsPrime(q)) { Console.Write("Неверный ввод. Введите простое число q: "); }

int r = p * q;
int f = (p - 1) * (q - 1);
Console.WriteLine($"q(r): {f}");
Console.Write("Введите число e (должно быть взаимно простым с q(r) и меньше его): ");
int Ko;
while (!int.TryParse(Console.ReadLine(), out Ko)) { Console.Write("Неверный ввод. Введите ещё раз: "); }
int Kc = ModInverse(Ko, f);
Console.WriteLine($"Открытый ключ: {Ko} {r}");

Decryption();
Hack(Ko, r);

void Decryption()
{
    Console.WriteLine("нажмите Enter чтобы продолжить.");
    Console.ReadLine();

    List<int> text = ReadCode(@"D:\Laba\ТИ\ti5(1)\ti5(1)\bin\Debug\net8.0\encrypt.txt");
    List<int> decryptText = new List<int>();
    foreach (int i in text)
    {
        decryptText.Add((int)BigInteger.ModPow(i, Kc, r));
    }

    Console.WriteLine("Расшифрованный текст: ");
    Console.WriteLine(CodesToText(decryptText));
}
void Hack(int Ko, int r)
{
    Console.WriteLine("нажмите Enter чтобы продолжить.");
    Console.ReadLine();
    List<int> text = ReadCode(@"D:\Laba\ТИ\ti5(1)\ti5(1)\bin\Debug\net8.0\encrypt.txt");
    List<int> pq = Factorize(r);
    int hackF = (pq[0] - 1) * (pq[1] - 1);
    int decKey = ModInverse(Ko, hackF);
    string decryptText = "";
    foreach (int i in text)
    {
        decryptText += Convert.ToChar((int)BigInteger.ModPow(i, decKey, r));
    }
    Console.WriteLine("Дешифрованный текст: ");
    Console.WriteLine(decryptText);
}
bool IsPrime(int n) // Функция для проверки, является ли число простым
{
    if (n <= 1)
        return false;
    if (n == 2)
        return true;
    if (n % 2 == 0)
        return false;

    var boundary = (int)Math.Floor(Math.Sqrt(n));// Вычисляем верхнюю границу, которая будет проверяться


    for (int i = 3; i <= boundary; i += 2) // Проверяем делится ли число на все нечётные числа от 3 до границы
        if (n % i == 0)// Если число делится без остатка, оно не является простым
            return false;

    return true;
}
static int ModInverse(int k, int phi)// Функция для вычисления мультипликативного обратного числа (модульного обратного)
{
    int i = phi, v = 0, d = 1;
    while (k > 0)
    {
        int t = i / k, x = k; // Находим частное от деления i на k и сохраняем k в переменной x
        k = i % x; // Находим остаток от деления i на x и присваиваем его k
        i = x; 
        x = d; 
        d = v - t * x; // Вычисляем новое значение d по расширенному алгоритму Евклида
        v = x; // Присваиваем x переменной v
    }
    v %= phi; // Находим остаток от деления v на phi
    if (v < 0) v = (v + phi) % phi; // Если v отрицательно, приводим его к положительному значению в модуле phi
    return v;
}

List<int> ReadCode(string path)// Функция для чтения зашифрованного текста из файла и его преобразования в список целых чисел
{

    string code = File.ReadAllText(path);
    List<int> codes = new List<int>();
    foreach (Match match in Regex.Matches(code, @"-?\d+"))
    {
        codes.Add(Convert.ToInt32(match.ToString()));// Преобразуем найденную строку числа в целое число и добавляем в список codes
    }
    return codes;
}

string CodesToText(List<int> codes)// Функция для преобразования списка целых чисел в строку текста
{
    string result = "";
    foreach (char c in codes)
    {
        result += Convert.ToChar(c);
    }
    return result;
}
List<int> Factorize(int num)// Функция для факторизации числа на простые множители
{
    List<int> factors = new List<int>();
    for (int div = 2; num > 1; div++)// Начинаем с деления на наименьший простой множитель - 2
        while (num % div == 0)// Пока число делится на текущий делитель без остатка
        {
            factors.Add(div);  
            num /= div;
        }
    return factors;
}