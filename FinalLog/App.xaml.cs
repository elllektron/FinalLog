﻿using log4net;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace FinalLog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        private string file = "logfile.log"; 
        protected override void OnStartup(StartupEventArgs e)

        {
            if (File.Exists(file))
                File.Delete(file);

            log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  Started Logging  =============       :) ");
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); ;

                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); ;



                FrameworkElement.LanguageProperty.OverrideMetadata(

                  typeof(FrameworkElement),

                  new FrameworkPropertyMetadata(

                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                

                
            }catch(Exception ex)
            {
                log.Error(ex);
            }



            base.OnStartup(e);

            }
    }

}
