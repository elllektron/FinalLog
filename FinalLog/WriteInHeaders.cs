﻿using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;

namespace FinalLog
{
    class WriteInHeaders
    {
        private readonly string _wellName;
        private readonly List<string> _runs;
        private readonly XmlDocument _coreFile;
        private readonly string _wellType;
        private readonly string _mudType;
        private readonly string _kcl;
        private readonly string _activity;
        private readonly string _customerName;
        private readonly string _company;
        private readonly BackgroundWorker _worker;
        private readonly bool _check;
        private string statusString = "";
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string updateVersionTextCaption = "Проверьте правильность заполнения CORE";
        private string textBoxMessage = "";

        public WriteInHeaders(string wellName, List<string> runs,
            XmlDocument coreFile, string wellType, string mudType,
            string kcl, string activity, string customerName, string company, 
            BackgroundWorker worker, bool check)
        {
            _wellName = wellName;
            _runs = runs;
            _coreFile = coreFile;
            _wellType = wellType;
            _mudType = mudType;
            _kcl = kcl;
            _activity = activity;
            _customerName = customerName;
            _company = company;
            _worker = worker;
            _check = check;
            log4net.Config.XmlConfigurator.Configure();
            RunFillingHeaders();
        }

        private void RunFillingHeaders()
        {
            DataFromCore data = new(_wellName, _runs, _coreFile, _wellType, _mudType, _company);
            string _fileExelPath = "Data\\Header.xlsm";
            string _fileExelPathDebug = "FinalLog\\Data\\Header.xlsm";
            string currentDirectory = Directory.GetCurrentDirectory();
            var temp = currentDirectory.Split('\\');
            string fullPath = "";
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == "FinalLog")
                {
                    if (i == temp.Length - 1)
                    {
                        fullPath += Path.Combine(temp[i], _fileExelPath);
                        break;
                    }
                    else
                    {
                        fullPath += Path.Combine(temp[i], _fileExelPathDebug);
                        break;
                    }
                }
                else
                {
                    fullPath += temp[i] + "\\";
                }
            }

            if (data.ErrorsInRunsDict.Count > 0)
            {
                foreach (var runErrors in data.ErrorsInRunsDict)
                {
                    string errorList = "";
                    for ( var i = 0; i < runErrors.Value.Count; i++)
                    {
                        errorList += string.Format("{0}, ", runErrors.Value[i]);
                    }
                    textBoxMessage += string.Format("Номер рейса: {0} - Ошибки: {1}\n", runErrors.Key, errorList);
                }
                MessageBoxButton buttonOK = MessageBoxButton.OK;
                MessageBoxResult result = MessageBox.Show(textBoxMessage, updateVersionTextCaption, buttonOK);

            }

            else
            {
                Microsoft.Office.Interop.Excel.Application application = new();


                statusString = "Открываем файл excel";
                _worker.ReportProgress(15, statusString);

                Workbook workbook = application.Workbooks as Workbook;
                try
                {
                    workbook = application.Workbooks.Open(fullPath);

                    statusString = "Заполняем Header Info";
                    _worker.ReportProgress(30, statusString);
                    HeaderInfo header = new(workbook, data, _check);

                    statusString = "Заполняем Run Summary";
                    _worker.ReportProgress(45, statusString);
                    RunSum runSum = new(workbook, data, _activity, _customerName, _kcl);

                    statusString = "Заполняем Equipment Summary";
                    _worker.ReportProgress(60, statusString);
                    EquipmentSum equipSum = new(workbook, data);

                    statusString = "Заполняем Mud Summary";
                    _worker.ReportProgress(75, statusString);
                    MudSum mudSum = new(workbook, data, _kcl);

                    statusString = "Заполняем LWD Remarks";
                    _worker.ReportProgress(90, statusString);

                    application.Visible = true;

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    statusString = "Не удается найти файл excel";
                    _worker.ReportProgress(100, statusString);
                    workbook.Close(true);
                    application.Quit();
                }
            }
            

        }
    }
}
