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
            //string pathZip = $"{ currentDir}\\NewFinalLog";
            FileInfo file = new(path);
            if (file.Exists)
            {
                Process[] proc = Process.GetProcessesByName("FinalLog");
                proc[0].Kill();
                System.Threading.Thread.Sleep(1000);
                ZipFile.ExtractToDirectory(path, currentDir, true);
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
