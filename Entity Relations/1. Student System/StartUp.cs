using P01_StudentSystem.Data;
using System;

namespace P01_StudentSystem
{
    class StartUp
    {
        static void Main(string[] args)
        {
            StudentSystemContext context = new StudentSystemContext();

            if (context.Database.EnsureCreated())
            {
                Console.WriteLine("Db was created!");
            }
        }
    }
}
