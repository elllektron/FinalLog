using System;
using System.ComponentModel;
using System.Net;
using System.Windows;

namespace FinalLog
{
    /// <summary>
    /// Логика взаимодействия для LoadWindow.xaml
    /// </summary>
    public partial class LoadWindow : Window
    {
        public LoadWindow()
        {
            InitializeComponent();
            //CheckUpdateProgramm();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void CheckUpdateProgramm()
        {
            
           
            WebClient downloadFile = new();
            string url = "https://github.com/elllektron/FinalLog/files/8500776/FinalLog.zip";
            //https://github.com/elllektron/FinalLog/files/8500776/FinalLog.zip

            string checkUrl = $"{url}";//{version}.zip";

            
            downloadFile.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            downloadFile.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            downloadFile.DownloadFileAsync(new Uri(checkUrl), "FinalLogNew.zip");
            
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            statusText.Text = "Загружено байт: " + e.BytesReceived + progressBar.Value;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                statusText.Text = "Скачивание завершено";
                TabIndex = 1;
            }
        }
    }
}
