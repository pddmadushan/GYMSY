using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CMSXtream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {

                this.Title = StaticProperty.ClientName + " [" + StaticProperty.LoginUserID + "]";
                if (StaticProperty.LoginisAdmin != "1")
                {
                    var window = App.Current.MainWindow as ModernWindow;
                    //Admin,sms, report
                    var toRemove = window.MenuLinkGroups.ElementAt(2);
                    window.MenuLinkGroups.Remove(toRemove);
                    toRemove = window.MenuLinkGroups.ElementAt(2);
                    window.MenuLinkGroups.Remove(toRemove);
                    //toRemove = window.MenuLinkGroups.ElementAt(2);
                    //window.MenuLinkGroups.Remove(toRemove);   
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }


    }
}
