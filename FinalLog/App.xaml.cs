using System.Globalization;
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
        protected override void OnStartup(StartupEventArgs e)

        {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-EN"); ;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-EN"); ;



            FrameworkElement.LanguageProperty.OverrideMetadata(

              typeof(FrameworkElement),

              new FrameworkPropertyMetadata(

                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));



            base.OnStartup(e);

        }
    }

}
