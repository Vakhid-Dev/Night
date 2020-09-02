using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    class Multithreading
    {
        static async Task Main(string[] args)
        {

            
            ReadWriteAsync();   // вызов асинхронного метода
            Console.WriteLine("Continue working for main thread");
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000);
            }
            Console.ReadLine();


        }

        // Read and Write file async
        static async void ReadWriteAsync()
        {
            Console.WriteLine("Начало метода ReadWriteAsync");

            string s = "Hello world! One step at a time";

            // hello.txt - файл, который будет записываться и считываться
            using (StreamWriter writer = new StreamWriter("hello.txt", false))
            {
                await writer.WriteLineAsync(s);  // асинхронная запись в файл

                Console.WriteLine("Writed ");
            }
            using (StreamReader reader = new StreamReader("hello.txt"))
            {
                var res = await reader.ReadToEndAsync();
                // асинхронное чтение из файла
                await Task.Delay(3000);
                Console.WriteLine("Datas from file: " + res);
            }

            Console.WriteLine("Конец метода ReadWriteAsync");

        }

        static void Factorial(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            Console.WriteLine($"Факториал числа {n} равен {result}");
        }
        //В этом случае мы можем запустить все задачи параллельно и через метод Task.WhenAll отследить их завершение

        static async void FactorialAsync()
        {
            Task t1 = Task.Run(() => Factorial(4));
            Task t2 = Task.Run(() => Factorial(3));
            Task t3 = Task.Run(() => Factorial(5));
            await Task.WhenAll(new[] { t1, t2, t3 });
        }
        // Try catch error
        static async Task DoMultipleAsync()
        {
            Task allTasks = null;

            try
            {
                Task t1 = Task.Run(() => Factorial(-3));
                Task t2 = Task.Run(() => Factorial(-5));
                Task t3 = Task.Run(() => Factorial(-10));

                allTasks = Task.WhenAll(t1, t2, t3);
                await allTasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение: " + ex.Message);
                Console.WriteLine("IsFaulted: " + allTasks.IsFaulted);
                foreach (var inx in allTasks.Exception.InnerExceptions)
                {
                    Console.WriteLine("Внутреннее исключение: " + inx.Message);
                }
            }
        }

        // Отмена синхронных операций
        static void Factorial(int n, CancellationToken token)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Операция прервана токеном");
                    return;
                }
                result *= i;
                Console.WriteLine($"Факториал числа {i} равен {result}");
                Thread.Sleep(1000);
            }
        }
   
        static async void FactorialAsync(int n, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            await Task.Run(() => Factorial(n, token));
        }

        static void Main()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            FactorialAsync(6, token);
            Thread.Sleep(3000);
            cts.Cancel();
            Console.Read();
        }
        //  Асинхронные стримы
        
        //static async Task Main(string[] args)
        //{
        //    await foreach (var number in GetNumbersAsync())
        //    {
        //        Console.WriteLine(number);
        //    }
        //}

        //public static async IAsyncEnumerable<int> GetNumbersAsync()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        await Task.Delay(100);
        //        yield return i;
        //    }
        //}
    }
}
