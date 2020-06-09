using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MergeSortThreading
{
    class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _ = Thread.CurrentThread; //Инициализация потока 

            /* t.Name = "Главный поток"; //Вывод информации о потоке
             Console.WriteLine($"Имя потока: {t.Name}");
             Console.WriteLine($"Включён ли поток: {t.IsAlive}");
             Console.WriteLine($"Приоритет потока: {t.Priority}");
             Console.WriteLine($"Статус потока: {t.ThreadState}");*/

            int[] arr = new int[65]; //Массив на 66 символов

            Random rnd = new Random(); //Создание переменной Random

            for (int i = 0; i < arr.Length; i++) //Заполнение массива случайными значениями
            {
                arr[i] = rnd.Next(100);
            }

            for (int i = 0; i < arr.Length; i++) //Вывод массива в консоль
            {
                Console.Write($"{arr[i]}, ");
            }
            Console.WriteLine();

            int n;

            while (true) //Ввод кол-ва потоков
            {
                Console.Write("Введите количество потоков(N)(1,2,4,8) = ");
                int.TryParse(Console.ReadLine(), out n);
                if (n == 1 || n == 2 || n == 4 || n == 8) break;
                else Console.WriteLine("Некорректный ввод");
            }


            MultithreadedSort(arr, n); //Сортировка массива


            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write($"{arr[i]}, ");
            }
            Console.WriteLine();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void MultithreadedSort(int[] arr, int threads)
        {
            int[][] arrs = new int[threads][];

            Thread[] sorting = new Thread[threads]; //Создание массива потоков

            for (int i = 0; i < threads; i++) //Пока не закончились потоки
            {
                arrs[i] = new int[arr.Length / threads]; //Создание массива с размерностью (длина начального массива/кол-во потоков)

                Array.Copy(arr, i * (arr.Length / threads), arrs[i], 0, arr.Length / threads); //(исходный массив, длина/кол-во потоков, массив, новый массив, )

                sorting[i] = new Thread(new ParameterizedThreadStart(Sort)); //Инициализация потока для сортировки

                sorting[i].Start(arrs[i]); //Запуск потока для сортировки
            }

            for (int i = 0; i < threads; i++) //Пока не закончились потоки

                sorting[i].Join(); // блокируем предыдующие до завершения

            bool norm = false; //Флаг для 1 прохода

            while (arrs.Length != 1) //Пока длина массива не равна 1
            {
                int k = 0;
                int[][] tmp = new int[arrs.Length / 2][];

                for (int i = 0; i < arrs.Length; i++) //Пока не закончился массив
                {
                    if (arr.Length % 2 != 0 && !norm) 
                    {
                        tmp[k] = new int[arrs[i].Length + arrs[i + 1].Length + 1];
                        tmp[k][tmp[k].Length - 1] = arr[arr.Length - 1];
                        norm = true;
                    }
                    else
                    {
                        tmp[k] = new int[arrs[i].Length + arrs[i + 1].Length]; //Буферная переменная с размерностью (текущего + следующего массива)
                    }
                    Array.Copy(arrs[i], 0, tmp[k], 0, arrs[i].Length); //Копирование в массив массива из буфера
                    try
                    {
                        Array.Copy(arrs[i + 1], 0, tmp[k], arrs[i].Length, arrs[i + 1].Length); //Копирование в массив след массива из буфера
                    }
                    catch (Exception)
                    {

                    }
                    i++;
                    k++;
                }

                Thread[] merging = new Thread[tmp.Length];  //Создание массива парралельных потоков

                for (int j = 0; j < tmp.Length; j++)
                {
                    merging[j] = new Thread(new ParameterizedThreadStart(Sort)); //Инициализация парралельного потока для сортировки

                    merging[j].Start(tmp[j]); //Запуск парралельного потока для сортировки
                }

                for (int i = 0; i < tmp.Length; i++) //Блокирование потока до завершения
                    merging[i].Join();

                arrs = tmp; //Замена предыдующей части массива на следующую
            }

            arrs[0].CopyTo(arr, 0); //Копирует все элементы с 0 из arr
        }

        public static void Sort(object arr) //Сортировщик
        {
            Array.Sort((int[])arr, 0, ((int[])arr).Length);
        }
    }
}
