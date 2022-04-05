using System;
using System.IO;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace FinalLogTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "Data\\Header.xlsm";
            string currentDirectory = Directory.GetCurrentDirectory();
            var temp = currentDirectory.Split('\\');
            string fullPath = "";
            for (int i = 0; i < temp.Length; i++)
            {
                
                if(temp[i] == "FinalLogTest")
                {
                    fullPath = Path.Combine(temp[i], filePath);
                }
            }

            Console.WriteLine(fullPath);

            
            
            
        }
    }
}
