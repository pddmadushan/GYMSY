using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CMSXtream
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ApplicationStart(object sender, StartupEventArgs e)
        {
            try
            {
                //Disable shutdown when the dialog closes
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var dialog = new WpfApplication2.MainWindow();
                dialog.ShowDialog();
                if (dialog.Login == true)
                {
                    //var mainWindow = new MainWindow(dialog.Data);
                    var mainWindow = new MainWindow();
                    ////Re-enable normal shutdown mode.
                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    Current.MainWindow = mainWindow;
                    //mainWindow.Topmost = true;
                    mainWindow.Show();
                }
                else
                {
                    //MessageBox.Show("Unable to load data.", "Error", MessageBoxButton.OK);
                    Current.Shutdown(-1);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to connect to the server. Please check the log file for more details!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
    }
}
