using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MonitorApp1
{
    class Program
    {
        static string Massage;
        static bool DoLog;
        static string Path = @"..\\Debug\log.log";
        static void Main(string[] args)
        {
            // Получаем аргументы и проверяем что они не пустые

            string[] Get_Args = Environment.GetCommandLineArgs();
            string NameProcess = null;
            string Args2;
            string Args3;
            int AliveTime = 0;
            int CheckTime = 0;
            CheckLogFile();   // Проверяем возможность создания логов

            if (Get_Args.Length == 4)
            {
                NameProcess = Get_Args[1];
                Args2 = Get_Args[2];
                Args3 = Get_Args[3];

                try
                {
                    AliveTime = int.Parse(Args2);
                    CheckTime = int.Parse(Args3);
                }
                catch (Exception)
                {
                    Massage = "Параметры заданы неверно. Измените параметры." + "\n" + "Приложение будет автоматически закрыто через 3 сикунды";
                    Console.WriteLine(Massage);
                    Log();
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
            }
            else
            {
                Massage = "Параметры заданы неверно. Измените параметры." + "\n" + "Приложение будет автоматически закрыто через 3 сикунды";
                Console.WriteLine(Massage);
                Log();
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            // Выводим что приложение запущено
            Massage = "Приложение активно. Начат поиск процесса";
            Console.WriteLine(Massage);
            Log();
            Console.WriteLine("Для выхода нажмите Q");

            // Ищем открытый процесс
            try
            {
                while (Process.GetProcessesByName(NameProcess).Length <= 0)    // Если его нет ждем минуту и повторяем
                {
                    quit();

                    Massage = "Процесс не найден, ожидание...";
                    Console.WriteLine(Massage);
                    Log();

                    Thread.Sleep(CheckTime * 5000);

                }

                while (Process.GetProcessesByName(NameProcess).Length > 0)    // Когда процесс найден
                {
                    Massage = "Процесс найден";
                    Console.WriteLine(Massage);
                    Log();

                    for (int i = 0; i <= AliveTime; i++)    // Мониторим время работы по аргументу
                    {
                        Massage = "Процесс живет " + i + " мин";
                        Console.WriteLine(Massage);
                        Log();
                        System.Threading.Thread.Sleep(CheckTime * 1000);    // Отсечка проверки на заданное время
                    }

                    try
                    {
                        foreach (Process proc in Process.GetProcessesByName(NameProcess))   // Завершаем процесс
                        {
                            proc.Kill();
                        }

                        Massage = "Процесс принудительно завершен";
                        Console.WriteLine(Massage);    // Сообщаем что процесс завершен
                        Log();
                        NameProcess = null;
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("Исключение при наблюдении: {0}", ex.Message));
                    }
                }
            }

            finally
            {
                Massage = "Наблюдение завершено";
                Console.WriteLine(Massage);
            }

            Log();
            System.Threading.Thread.Sleep(2000);
            Environment.Exit(0);    // Выходим из приложения
        }
        static void CheckLogFile() // Проверка доступности хранилища для логов
        {
            string TestPath = @"..\\Debug\test.log";
            var TestFile = File.Create(TestPath);
            TestFile.Close();
            if (File.Exists(TestPath))
            {
                Console.WriteLine("Лог-файл создан. Логирование включено");
                File.Delete(TestPath);
                DoLog = true;
            }
            else
            {
                Console.WriteLine("Лог-файл недоступен. Логирование Выключено");
                DoLog = false;
            }
        }
        static void PreLog() // Логирование
        {
            File.AppendAllText(Path, Massage + Environment.NewLine);
        }
        static void Exit() // Метод выхода по нажатию
        {
            for (int i = 0; i < 1; )
            {
                ConsoleKeyInfo Q = Console.ReadKey(true);
                if (Q.Key == ConsoleKey.Q)
                {
                    i++;
                }
            }

            Massage = "Наблюдение завершено";
            Console.WriteLine(Massage);
            Log();
            Thread.Sleep(1000);
            Environment.Exit(0);    // Выходим из приложения
        }


        static async Task quit() // Асинхронный выход
        {
            await Task.Run(() => Exit());    
        }

        static async Task Log() // Асинхронное логирование
        {
            if (DoLog == true)
            {
                await Task.Run(() => PreLog());
            }
        }

    }
}