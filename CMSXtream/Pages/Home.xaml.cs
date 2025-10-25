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
using WpfScheduler;
using XtreamDataAccess;

namespace CMSXtream.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }
        private void scduleWeekLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                scduleWeek.Events.Clear();
                scduleWeek.WeekScheduler.FirstDay = DateTime.Now;               
                scduleWeek.Mode = Mode.Week;
                scduleWeek.StartJourney = new TimeSpan(6, 0, 0);
                scduleWeek.EndJourney = new TimeSpan(23, 0, 0);

                HomeDA _clsHome = new HomeDA();
                System.Data.DataTable table = _clsHome.SelectClassSchedule().Tables[0];

                DateTime startDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                double day = (double)startDate.DayOfWeek;

                if (day == 0)
                {
                    startDate = startDate.AddDays(-6);
                }
                else
                {
                    startDate = startDate.AddDays((day - 1) * -1);
                }
                DateTime initStartDate = startDate;
                foreach (System.Data.DataRow dr in table.Rows)
                {
                    Event ev = new Event();
                    ev.Subject = dr["CLS_NAME"].ToString() + " [Week:" + dr["CLS_WEEK_NUMBER"].ToString() + "]";
                    startDate = initStartDate;
                    double addValue = double.Parse(dr["CLS_DAY"].ToString()) - 1;
                    double clsTime = double.Parse(dr["CLS_TIME"].ToString());
                    double clsDuration = double.Parse(dr["CLS_DURATION"].ToString());
                    Int32 clsHour = Int32.Parse(Math.Floor(clsTime).ToString());
                    Int32 clsMin = Int32.Parse(Math.Round((clsTime - double.Parse(clsHour.ToString())) * 100, 2).ToString());

                    DateTime ClassDate = startDate.AddDays(addValue);
                    ev.Start = new DateTime(ClassDate.Year, ClassDate.Month, ClassDate.Day, clsHour, clsMin, 00); ;

                    clsHour = Int32.Parse(Math.Floor(clsDuration).ToString());
                    clsMin = Int32.Parse(Math.Round((clsDuration - double.Parse(clsHour.ToString())) * 100, 2).ToString());
                    TimeSpan ts = new TimeSpan(clsHour, clsMin, 0);

                    ev.End = ev.Start + ts;
                    ev.Color = Brushes.LightGreen;
                    scduleWeek.AddEvent(ev);
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
