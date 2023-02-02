using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace FinalLogUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string currentDir = Directory.GetCurrentDirectory();
            string path = $"{currentDir}\\FinalLogNew.zip";

            FileInfo file = new(path);
            if (file.Exists)
            {
                Process[] proc = Process.GetProcessesByName("FinalLog");
                proc[0].Kill();
                
                ZipFile.ExtractToDirectory(path, currentDir, true);
                System.Threading.Thread.Sleep(3000);
                File.Delete(path);
                
                //Запускаем новый процесс
                Process isStartProcess = new();
                //Получаем папку в которой находится программа
                string currentDirectory = Directory.GetCurrentDirectory();
                //Выбираем программу для запуска
                isStartProcess.StartInfo.FileName = $"{currentDirectory}\\FinalLog.exe";
                isStartProcess.Start();
            }

        }
    }
}
