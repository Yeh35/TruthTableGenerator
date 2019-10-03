using System;
using System.Diagnostics;

namespace TruthTableGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //TruthTableGenerator.TestPrivate();
            Console.WriteLine("불대수를 입력해주시면 진리표를 출력하는 프로그램입니다.");
            Console.WriteLine("진리표는 다음 규칙에 따라 입력해주세요.");
            Console.WriteLine("\t~  : NOT");
            Console.WriteLine("\t*  : OR");
            Console.WriteLine("\t+  : AND");
            Console.WriteLine("\t() : 우선계산");
            Console.WriteLine("\t예 : A * ( B + C )");
            Console.WriteLine("종료하시고 싶다면 'EXIT'를 입력하세요.");
            while (true)
            {
                Console.Write("입력 : ");
                string inputLine = Console.ReadLine();

                if (inputLine == "EXIT")
                {
                    Console.WriteLine("Good Bye~");
                    break;
                }

                try
                {
                    Console.WriteLine(TruthTableGenerator.GetTruthTable(inputLine));
                }
                catch (Exception)
                {
                    Console.WriteLine("이상한 값입니다.");
                }
            }

        }
    }
}
