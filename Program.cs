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
            int w = 1; int b = 2; int x = 3; int p = 4;
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
            bool GameOn=true;
            int M = 0;
            int m = 0;
            int[][] LEVEL = new int[level.Length][];
            for (int i = 0; i < level.Length; i++)
            {
                LEVEL[i] = new int[level[i].Length];
                level[i].CopyTo(LEVEL[i], 0);
            }
            int[] map = level[0];
            MapPrint(map);
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                map[^4]++;
                int sec; int m;
                m = map[^4] / 60;
                sec = map[^4] - m * 60;
                if ( sec == 0)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(m + ":" + sec+" ");
                }
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(m + ":" + sec);
            };
            timer.Start();  
            do
            {
                ConsoleKey key;
                key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        Move(-map[^2], map);
                        break;
                    case ConsoleKey.DownArrow:
                        Move(map[^2], map);
                        break;
                    case ConsoleKey.RightArrow:
                        Move(1, map);
                        break;
                    case ConsoleKey.LeftArrow:
                        Move(-1, map);
                        break;
                    case ConsoleKey.Q:
                        if (m != 0)
                        {
                            m--;
                            map = level[m];
                            M = 0;
                            timer.Start();   
                            MapPrint(map);
                        }
                        break;
                    case ConsoleKey.E:
                        if (m != 3)
                        {
                            m++;
                            map = level[m];
                            M = 0;
                            timer.Start();
                            MapPrint(map);
                        }
                        break;
                    case ConsoleKey.R:
                        LEVEL[m].CopyTo(level[m], 0);
                        level[m].CopyTo(map, 0);
                        MapPrint(map);
                        break;
                    case ConsoleKey.Escape:
                        GameOn = false;
                        break;
                }
                if (Win(map)) { timer.Stop(); };
                if (Win(map)==false) { timer.Start(); };


            } while (GameOn);
        }
        public static void MapPrint(int[] map)
        {
            Console.Clear();
            for (int i = 0; i < map.Length-4; i = i + map[^2])
            {
                for (int j = i; j < i + map[^2]; j++)
                {
                    switch (map[j]){
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
            m = map[^4]/60;
            s = map[^4]-m*60;
            Console.Write(m+":"+s);

        }
        public static void Move(int x, int[] map)
        {
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
                            break;
                        case 3:
                            map[y + x] = map[y + x] + 2;
                            map[y] = map[y]+2;
                            map[map[^1]] = map[map[^1]] - 4;
                            map[^1] = map[^1] + x;
                            map[^3]++;
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
            MapPrint(map);
            if (win) { Console.WriteLine("Победа"); }
        }
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
    }
}
