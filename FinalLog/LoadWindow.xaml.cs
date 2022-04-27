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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void CheckUpdateProgram(string version)
        {
            
           
            WebClient downloadFile = new();
            
            string url = $"https://github.com/elllektron/FinalLog/releases/download/{version}/FinalLog.zip";

            string checkUrl = $"{url}";//{version}.zip";

            
            downloadFile.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            downloadFile.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            downloadFile.DownloadFileAsync(new Uri(url), "FinalLogNew.zip");
            
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            //statusText.Text = "Загружено байт: " + e.BytesReceived + progressBar.Value;
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
                DialogResult = false;
            }
        }
    }
}
