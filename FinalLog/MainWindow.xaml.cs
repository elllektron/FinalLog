using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Xml;



namespace FinalLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string messageBoxText = "Перед использованием программы убедитесь в правильности заполнения данных Core!" +
            "\n ОЧЕНЬ ВАЖНО НЕ ПЕРЕНОСИТЬ И НЕ УДАЛЯТЬ ФАЙЛ Header.xlsm" +
            "\n ПОСЛЕ ЗАВЕРШЕНИЯ ВЫПОЛНЕНИЯ ПРОГРАММЫ В ОТКРЫВШЕМСЯ ФАЙЛЕ EXCEL НАЖИМАЕМ СОХРАНИТЬ КАК И ВЫБИРАЕМ НУЖНОЕ МЕСТО" +
            "\n При заполнении помимо основных используются следующие данные:" +
            "\n Координаты устья" +
            "\n Название буровой например: BU-3000" +
            "\n Альтитуда" +
            "\n Глубина башмака" +
            "\n Диаметр долота" +
            "\n Температура" +
            "\n Длинна КНБК" +
            "\n Тип КНБК" +
            "\n Часы бурения за рейс" +
            "\n Часы циркуляции за рейс" +
            "\n Начало и конец рейса" +
            "\n Непромеры и aqusiton rate" +
            "\n Номера приборов и ИИИ из рейса" +
            "\n А так же параметры раствора";
        

        string caption = "Внимание!!!";


        private XmlDocument xmlDocument = new();
        private XmlElement xmlElement;

        private XmlNodeList wellNames;
        private XmlNodeList runNumbers;
        private List<string> wellTypes = new() { "Пилотный ствол", "Транспортный ствол", "Горизонтальный ствол" };
        private List<string> mudTypes = new() { 
            "Полимер-глинистый", 
            "Полимерный", 
            "Глинистый", 
            "РУО", 
            "Биополимерный KCL", 
            "Пресный ингибированный",
            "Полимер-карбонатный инкапсулирующий"
        };

        private List<string> activitys = new() { "бурение/запись", "бурение" };

        public string Version { get; set; }
        public string FileName { get; set; }
        public string WellName { get; set; }
        public List<string> RunNumbers { get; set; } = new List<string>();
        public string WellType { get; set; }
        public string MudType { get; set; }
        public string Activity { get; set; }
        public string CustomerName { get; set; }
        public int ProgressBarValue { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            CheckVersionForUpdate();

            MessageBoxButton button = MessageBoxButton.OK;
            MessageBox.Show(messageBoxText, caption, button);
            
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
                }
                else
                {
                    statusText.Text = "Файл не найден";
                }
            }
        }

        private void Start_Filling(object sender, RoutedEventArgs e)
        {

            if (wellNames != null && runNumbers != null && wellType.SelectedItem != null
                && mudType.SelectedItem != null && activity.SelectedItem != null && customerName != null)
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

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
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

            _ = new WriteInHeaders(WellName, RunNumbers, xmlDocument, WellType, MudType, Activity, CustomerName, sender as BackgroundWorker);

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


        private void CheckVersionForUpdate()
        {
            HttpWebResponse response;
            string newVersion;
            string updateVersionTextBoxMessage = "Доступна новая версия программы. \n Вы хотите её обновить?";
            string updateVersionTextCaption = "Доступно обновление";



            //Получаем текущую версию программы
            string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            string currentVersion = $"v{version.Remove(version.Length - 2)}"; 
            //Устанавливаем версию в title
            Title = $"Final Log  v {version}";
       
            //Проверяем есть ли новая версия на сервере
            string url = $"https://github.com/elllektron/FinalLog/releases/latest";
            var uri = WebRequest.Create(url);
            try
            {
                response = (HttpWebResponse)uri.GetResponse();
                var responseList = response.ResponseUri.ToString().Split('/');
                newVersion = responseList[^1];
                if(currentVersion != newVersion)
                {
                    MessageBoxButton buttonYesNo = MessageBoxButton.YesNo;
                    MessageBox.Show(updateVersionTextBoxMessage, updateVersionTextCaption, buttonYesNo);
                }

            }
            catch (WebException we)
            {
                statusText.Text = ((HttpWebResponse)we.Response).StatusCode.ToString();
            }      
        }
    }
}
