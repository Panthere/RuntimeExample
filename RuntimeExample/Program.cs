using System;
using System.Collections.Generic;
using System.Text;

namespace RuntimeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Compile it
            Console.WriteLine(Compiler.Compile.FromSource("Loader.cs", "testFile.exe"));
            Console.ReadLine();
        }
    }
}
