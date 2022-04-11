using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace FinalLog
{
    class WriteInHeaders
    {
        //private readonly string _fileExelPath = @"FinalLog\Data\Header.xlsm";
        private readonly string _wellName;
        private readonly List<string> _runs;
        private readonly XmlDocument _coreFile;
        private readonly string _wellType;
        private readonly string _mudType;
        private readonly string _activity;
        private readonly string _customerName;
        private readonly BackgroundWorker _worker;
        private string statusString = "";


        public WriteInHeaders(string wellName, List<string> runs,
            XmlDocument coreFile, string wellType, string mudType,
            string activity, string customerName, BackgroundWorker worker)
        {
            _wellName = wellName;
            _runs = runs;
            _coreFile = coreFile;
            _wellType = wellType;
            _mudType = mudType;
            _activity = activity;
            _customerName = customerName;
            _worker = worker;
            RunFillingHeaders();

        }

        private void RunFillingHeaders()
        {
            DataFromCore data = new(_wellName, _runs, _coreFile, _wellType, _mudType);
            string _fileExelPath = "Data\\Header.xlsm";
            string _fileExelPathDebug = "FinalLog\\Data\\Header.xlsm";
            string currentDirectory = Directory.GetCurrentDirectory();
            var temp = currentDirectory.Split('\\');
            string fullPath = "";
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == "FinalLog")
                {
                    if(temp[i + 1] == "FinalLog")
                        fullPath += Path.Combine(temp[i], _fileExelPathDebug);
                    else
                        fullPath += Path.Combine(temp[i], _fileExelPath);
                    break;
                }
                else
                {
                    fullPath += temp[i] + "\\";
                }
            }

            Application application = new();


            statusString = "Открываем файл excel";
            _worker.ReportProgress(15, statusString);

            Workbook workbook = application.Workbooks as Workbook;
            try
            {
                workbook = application.Workbooks.Open(fullPath);

                statusString = "Заполняем Header Info";
                _worker.ReportProgress(30, statusString);
                HeaderInfo header = new(workbook, data);

                statusString = "Заполняем Run Summary";
                _worker.ReportProgress(45, statusString);
                RunSum runSum = new(workbook, data, _activity, _customerName);

                statusString = "Заполняем Equipment Summary";
                _worker.ReportProgress(60, statusString);
                EquipmentSum equipSum = new(workbook, data);

                statusString = "Заполняем Mud Summary";
                _worker.ReportProgress(75, statusString);
                MudSum mudSum = new(workbook, data);

                statusString = "Заполняем LWD Remarks";
                _worker.ReportProgress(90, statusString);

                application.Visible = true;
                
            }
            catch
            {
                statusString = "Не удается найти файл excel";
                _worker.ReportProgress(100, statusString);
                workbook.Close(true);
                application.Quit();
            }
            
        }
    }
}
