using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Word = Microsoft.Office.Interop.Word;
using log4net;


namespace FinalLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Версия программы
        private readonly string  version = "v1.1.9";
        private bool checkVersion;
       
        private XmlDocument xmlDocument = new();
        private XmlElement xmlElement;

        private XmlNodeList wellNames;
        private XmlNodeList runNumbers;
        private List<string> wellTypes = new() { "Пилотный ствол", "Транспортный ствол", "Горизонтальный ствол" };
        private List<string> mudTypes = new() 
        { 
            "Полимер-глинистый", 
            "Полимерный", 
            "Глинистый", 
            "РУО", 
            "Биополимерный KCL", 
            "Пресный ингибированный",
            "Полимер-карбонатный инкапсулирующий"
        };
        private List<string> companyList = new()
        {
            "АО \"Мессояханефтегаз\"",
            "ООО \"РН - Уватнефтегаз\"",
            "РН-Юганскнефтегаз",
            "РН-Няганьнефтегаз",
            "ООО \"Газпромнефть - Заполярье\" Песцовое",
            "ООО \"Меретояханефтегаз\"",
            "ООО \"Газпромнефть - Ямал\"",
            "ООО \"ЛУКОЙЛ - Западная Сибирь\"",
            "ООО \"СПП - Развитие\"",
            "ООО \"СевКомНефтегаз\"",
            "ООО \"ВТС\"",
            "ООО \"РусГазБурение\"",
            "АО \"РОСПАН ИНТЕРНЕШНЛ\"",
        };
        private List<string> activitys = new() { "бурение/запись", "бурение" };

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string FileName { get; set; }
        public string WellName { get; set; }
        public List<string> RunNumbers { get; set; } = new List<string>();
        public string WellType { get; set; }
        public string MudType { get; set; }
        public string Activity { get; set; }
        public string CustomerName { get; set; }
        public string Company { get; set; }



        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            //Проверяем запущена ли программа FinalLogUpdater

            Process[] procFinalLog = Process.GetProcessesByName("FinalLogUpdater");
            if (procFinalLog.Length != 0)
                //Если запущена останавливаем её
                procFinalLog[0].Kill();

            //Устанавливаем версию в title
            Title = $"Final Log {version}";
            checkVersion = CheckVersionForUpdate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.CheckFileExists)
                {
                    FileName = openFileDialog.FileName;
                    xmlDocument.Load(FileName);

                    wellBox.Items.Clear();
                    wellType.Items.Clear();
                    mudType.Items.Clear();
                    activity.Items.Clear();
                    company.Items.Clear();
                    xmlElement = xmlDocument.DocumentElement;
                    wellNames = xmlElement.GetElementsByTagName("Well");

                    for (int i = 0; i < wellNames.Count; i++)
                    {
                        wellBox.Items.Add(wellNames.Item(i).Attributes.GetNamedItem("Name").Value);
                    }
                    for (int i = 0; i < wellTypes.Count; i++)
                    {
                        if (wellNames.Count > 1)
                        {
                            if (wellTypes[i] == "Горизонтальный ствол")
                            {
                                for (int j = 1; j < wellNames.Count + 1; j++)
                                {
                                    wellType.Items.Add(wellTypes[i] + j);
                                }
                            }
                            else
                            {
                                wellType.Items.Add(wellTypes[i]);
                            }
                        }
                        else
                        {
                            wellType.Items.Add(wellTypes[i]);
                        }
                    }

                    for (int i = 0; i < mudTypes.Count; i++)
                    {
                        mudType.Items.Add(mudTypes[i]);
                    }
                    for (int i = 0; i < activitys.Count; i++)
                    {
                        activity.Items.Add(activitys[i]);
                    }
                    for (int i = 0; i < companyList.Count; i++)
                    {
                        company.Items.Add(companyList[i]);
                    }
                }
                else
                {
                    statusText.Text = "Файл не найден";
                }
            }
        }

        private void Start_Filling(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (wellNames != null && runNumbers != null && wellType.SelectedItem != null
                && mudType.SelectedItem != null && activity.SelectedItem != null && customerName != null && company != null)
                {
                    if (RunNumbers.Count > 0)
                        RunNumbers.Clear();

                    for (int i = 0; i < runBox.SelectedItems.Count; i++)
                    {
                        RunNumbers.Add(runBox.SelectedItems[i].ToString());
                        RunNumbers.Sort();
                    }
                    WellType = wellType.SelectedItem.ToString();
                    MudType = mudType.SelectedItem.ToString();
                    Activity = activity.SelectedItem.ToString();
                    CustomerName = customerName.Text;
                    Company = company.SelectedItem.ToString();

                    BackgroundWorker worker = new();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += Worker_DoWork;
                    worker.ProgressChanged += Worker_ProgressChanged;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusText.Text = "Готово";
            btnStart.IsEnabled = true;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
                statusText.Text = e.UserState.ToString();
            btnStart.IsEnabled = false;

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            _ = new WriteInHeaders(WellName, RunNumbers, xmlDocument, WellType, MudType, Activity, CustomerName, Company, sender as BackgroundWorker);

            (sender as BackgroundWorker).ReportProgress(100, "Сохраняем результаты");
        }

        private void wellBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            runNumbers = xmlElement.GetElementsByTagName("Run_Number");
            if (wellBox.Items.Count > 0)
                WellName = wellBox.SelectedItem.ToString();
            if (runBox.Items.Count > 0)
            {
                runBox.Items.Clear();
            }

            for (int i = 0; i < runNumbers.Count; i++)
            {
                if (runNumbers.Item(i).ParentNode.ParentNode.ParentNode.Attributes.GetNamedItem("Name").Value == WellName)
                {
                    if (runNumbers.Item(i).InnerText.Length < 3)
                    {
                        runBox.Items.Add(runNumbers.Item(i).InnerText);
                    }
                }
            }
        }


        private bool CheckVersionForUpdate()
        {
            HttpWebResponse response;
            string newVersion;
            string updateVersionTextBoxMessage = "Доступна новая версия программы. \n Вы хотите её обновить?";
            string updateVersionTextCaption = "Доступно обновление";

            //Проверяем есть ли новая версия на сервере
            string url = $"https://github.com/elllektron/FinalLog/releases/latest";
            var uri = WebRequest.Create(url);
            try
            {
                response = (HttpWebResponse)uri.GetResponse();
                var responseList = response.ResponseUri.ToString().Split('/');
                newVersion = responseList[^1];
                if (version != newVersion)
                {
                    
                    MessageBoxButton buttonYesNo = MessageBoxButton.YesNo;
                    MessageBoxResult result =  MessageBox.Show(updateVersionTextBoxMessage, updateVersionTextCaption, buttonYesNo);

                    if(result == MessageBoxResult.Yes)
                    {
                        LoadWindow loadWindow = new();
                        loadWindow.CheckUpdateProgram(newVersion);
                        loadWindow.ShowDialog();

                        //Запускаем новый процесс
                        Process isStartProcess = new();
                        //Получаем папку в которой находится программа
                        string currentDirectory = Directory.GetCurrentDirectory();
                        //Выбираем программу для запуска
                        isStartProcess.StartInfo.FileName = $"{currentDirectory}\\FinalLogUpdater.exe";

                        isStartProcess.Start();
                    }
                    return true;
                }
                return false;
            }

            catch (WebException we)
            {
                statusText.Text = ((HttpWebResponse)we.Response).StatusCode.ToString();
            }
            return false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Файл инструкций не найден возможно он был удален.";
            string caption = "Ошибка";
            string path = $"{Directory.GetCurrentDirectory()}\\ИНСТРУКЦИЯ!!!!!.docx";
            if (File.Exists(path))
            {

                Word.Application appWord = new();

                try
                {
                    appWord.Documents.Open(path);
                }
                catch 
                {
                    appWord.Documents.Close();

                }
                appWord.Application.Visible = true;

            }
            else
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show(messageBoxText, caption, button);
            }
        }
    }
}
