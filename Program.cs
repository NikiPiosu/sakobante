using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;

namespace sakobante
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //элементы уровня
            int w = 1; int b = 2; int x = 3; int p = 4;
            //уровни
            int[][] level = new int[][]
            {new int[]{
             w,w,w,
             w,x,w,
             w,0,w,
             w,b,w,
             w,p,w,
             w,w,w,
             0,0,3,13},
             new int[] {
             w,w,w,w,w,
             w,x,w,x,w,
             w,0,0,0,w,
             w,b,b,0,w,
             w,0,p,w,w,
             w,w,w,w,0,
             0,0,5,22},
             new int[] {
             w,w,w,w,w,w,w,w,
             w,p,0,0,0,0,x,w,
             w,0,b,w,w,w,w,w,
             w,0,b,0,x,w,0,0,
             w,w,w,w,w,w,0,0,
             0,0,8,9},
             new int[] {
             0,0,0,w,w,w,0,0,
             0,0,0,w,x,w,0,0,
             0,0,w,w,b,w,w,w,
             w,w,w,0,p,b,x,w,
             w,x,b,0,0,w,w,w,
             w,w,w,b,w,w,0,0,
             0,0,w,x,w,0,0,0,
             0,0,w,w,w,0,0,0,
             0,0,8,28
             }
            };
            bool GameOn = true;
            int m = 0;
            int[][] LEVEL = new int[level.Length][];
            ConsoleKey key;
            int[] map = level[0];
            StartPrint(map);
            for (int i = 0; i < level.Length; i++)
            {
                LEVEL[i] = new int[level[i].Length];
                level[i].CopyTo(LEVEL[i], 0);
            }
            //таймер
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                map[^4]++;
                int sec; int m;
                m = map[^4] / 60;
                sec = map[^4] - m * 60;
                Console.SetCursorPosition(0, ((map.Length - 4) / map[^2]) + 1);
                if (sec == 0)
                {
                    Console.Write(m + ":" + sec + " ");
                }
                else
                {
                    Console.Write(m + ":" + sec);
                }
            };
            timer.Start();
            //цикл работы 
            do
            {
                //движение
                key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        Move(-map[^2], map);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        Move(map[^2], map);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        Move(1, map);
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        Move(-1, map);
                        break;
                    case ConsoleKey.Q:
                        if (m != 0)
                        {
                            m--;
                            map = level[m];
                            timer.Start();
                            MapPrint(map);
                        }
                        break;
                    case ConsoleKey.E:
                        if (m != level.Length - 1)
                        {
                            m++;
                            map = level[m];
                            timer.Start();
                            MapPrint(map);
                        }
                        else
                        {
                            m = 0;
                            map = level[m];
                            timer.Start();
                            MapPrint(map);
                        }
                        break;
                    case ConsoleKey.R:
                        LEVEL[m].CopyTo(level[m], 0);
                        level[m].CopyTo(map, 0);
                        MapPrint(map);
                        break;
                    case ConsoleKey.H:
                        StartPrint(map);
                        break;
                    case ConsoleKey.Escape:
                        GameOn = false;
                        break;
                }
                //условия таймера
                if (Win(map)) { timer.Stop(); };
                if (Win(map)) 
                {
                    Console.SetCursorPosition(0, ((map.Length - 4) / map[^2]) + 2);
                    Console.WriteLine("победа");
                };
                if (Win(map) == false) { timer.Start(); };


            } while (GameOn);
        }
        //отрисовка карты
        public static void MapPrint(int[] map)
        {
            Console.Clear();
            for (int i = 0; i < map.Length - 4; i = i + map[^2])
            {
                for (int j = i; j < i + map[^2]; j++)
                {
                    switch (map[j])
                    {
                        case 0:
                            Console.Write(" " + " ");
                            break;
                        case 1:
                            Console.Write("#" + " ");
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("B" + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("X" + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("@" + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 5:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("B" + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 7:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("@" + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }
                }
                Console.Write('\n');
            }
            if (Win(map) == true)
            {
                Console.WriteLine("Победа");
            }
            Console.WriteLine("движения: " + map[^3]);
            Console.SetCursorPosition(0, Console.CursorTop);
            int s; int m;
            m = map[^4] / 60;
            s = map[^4] - m * 60;
            Console.Write(m + ":" + s);

        }
        //функция движения
        public static void Move(int x, int[] map)
        {
            bool Box = false;
            bool win = false;
            int y = map[^1] + x;
            switch (map[y])
            {
                case 0:
                    map[y] = 4;
                    map[map[^1]] = map[map[^1]] - 4;
                    map[^1] = map[^1] + x;
                    map[^3]++;
                    break;
                case 2:
                    switch (map[y + x])
                    {
                        case 0:
                            map[y + x] = 2;
                            map[y] = map[y] + 2;
                            map[map[^1]] = map[map[^1]] - 4;
                            map[^1] = map[^1] + x;
                            map[^3]++;
                            Box = true;
                            break;
                        case 3:
                            map[y + x] = map[y + x] + 2;
                            map[y] = map[y] + 2;
                            map[map[^1]] = map[map[^1]] - 4;
                            map[^1] = map[^1] + x;
                            map[^3]++;
                            Box = true;
                            break;
                    }
                    break;
                case 3:
                    map[y] = map[y] + 4;
                    map[y - x] = map[y - x] - 4;
                    map[^1] = map[^1] + x;
                    map[^3]++;
                    break;
                case 5:
                    switch (map[y + x])
                    {
                        case 0:
                            map[y + x] = 2;
                            map[y] = map[y] + 2;
                            map[map[^1]] = map[map[^1]] - 4;
                            map[^1] = map[^1] + x;
                            map[^3]++;
                            break;
                        case 3:
                            map[y + x] = map[y + x] + 2;
                            map[y] = map[y] + 2;
                            map[map[^1]] = map[map[^1]] - 4;
                            map[^1] = map[^1] + x;
                            map[^3]++;
                            break;
                    }
                    break;
            }
            MovePrint(map, x, Box);
            if (win) { Console.WriteLine("Победа"); }
        }
        //определение победы
        public static bool Win(int[] map)
        {
            for (int i = 0; i < map.Length - 2; i++)
            {
                if (map[i] == 3)
                {
                    return false;
                }
            }
            return true;
        }
        //написание меню помощи
        public static void StartPrint(int[] map)
        {
            Console.Clear();
            Console.Write("управление: " +
                "\n WASD или стрелочки - передвижение " +
                "\n QE - переключение уровней " +
                "\n R - перезапуск - уровня " +
                "\n ESC - выход из игры " +
                "\n H - для вызова этого меню" +
                "\n нажмите любую клавишу для продолжения");
            ConsoleKey key;
            key = Console.ReadKey().Key;
            Console.Clear();
            Console.Write("объекты: " +
                "\n # - стена \n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" @" + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("- персонаж \n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" B" + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("- коробка \n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" X" + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("- цель");
            Console.Write("\n нажмите любую клавишу для продолжения");
            key = Console.ReadKey().Key;
            Console.Clear();
            Console.Write("цель игры: " +
                "\n передвинуть все коробки(B) на все цели(X)" +
                "\n нажмите любую клавишу для продолжения");
            key = Console.ReadKey().Key;
            MapPrint(map);
        }
        //прорисовка движения
        public static void MovePrint(int[] map, int b, bool Box)
        {
            Console.SetCursorPosition((map[^1] % map[^2]) + (map[^1] % map[^2]) - 1, map[^1] / map[^2]);
            switch (map[map[^1]])
            {
               case 4:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" @");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
               case 7:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" @");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.SetCursorPosition(((map[^1] - b) % map[^2]) + ((map[^1] - b) % map[^2]) - 1, (map[^1] - b) / map[^2]);
            switch (map[map[^1] - b])
            {
                case 0:
                    Console.Write("  ");
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" X");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            if (Box)
            {
                Console.SetCursorPosition(((map[^1] + b) % map[^2]) + ((map[^1] + b) % map[^2]) - 1, (map[^1] + b) / map[^2]);
                switch ((map[map[^1] + b]))
                {
                    case 2:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(" B");
                    Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(" B");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }
            Console.SetCursorPosition(10, ((map.Length - 4) / map[^2]));
            Console.Write(map[^3]);
            Console.SetCursorPosition(0, ((map.Length - 4) / map[^2]+3));
        }
    }
}
