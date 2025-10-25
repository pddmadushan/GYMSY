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
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for Incentive.xaml
    /// </summary>
    public partial class Incentive : UserControl
    {
        private double incentiveMargine;
        private double incentivePercentage;



        public Incentive()
        {
            InitializeComponent();
            Loaded += Incentive_Loaded;

        }


        void Incentive_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInt();
            LoadInfo();
        }

        private void LoadInt()
        {
            try
            {
                incentiveMargine = double.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("IncentiveMargin_in_K").ToString()) * 1000;
                incentivePercentage = double.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("IncentivePresentation").ToString());

                dtpPayMonth.SelectedDate = DateTime.Now;
                dtpPayMonth.DisplayDateStart = new DateTime(2019, 5, 1);
                dtpPayMonth.DisplayDateEnd = DateTime.Now;
                pgrsBar.Value = 0;
            }
            catch (Exception ex)
            {
                pgrsBar.Value = 0;
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpPayMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadInfo();
        }

        private void LoadInfo()
        {
            try
            {
                BackUPDB _clsBackUp = new BackUPDB();
                DateTime selectedDate = new DateTime(dtpPayMonth.SelectedDate.Value.Year, dtpPayMonth.SelectedDate.Value.Month, 1);
                System.Data.DataTable table = _clsBackUp.GetIncentiveInfo(selectedDate, incentiveMargine, incentivePercentage).Tables[0];

                pgrsBar.Value = 0;
                lblInfo.Content = string.Empty;
                lblIncentiveAmt.Content = string.Empty;

                if (table.Rows.Count > 0)
                {
                    pgrsBar.Value = double.Parse(table.Rows[0][0].ToString());
                    lblInfo.Content = table.Rows[0][1].ToString();
                    lblIncentiveAmt.Content = table.Rows[0][2].ToString();
                }
            }
            catch (Exception ex)
            {
                pgrsBar.Value = 0;
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
    }
}
