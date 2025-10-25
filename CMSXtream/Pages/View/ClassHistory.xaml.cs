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
    /// Interaction logic for ClassHistory.xaml
    /// </summary>
    public partial class ClassHistory : UserControl
    {
        public Int32 classID { get; set; }
        public ClassHistory()
        {
            InitializeComponent();
        }
        public void LoadFormContaint()
        {
            ClassAttendanceDA _clsPayment = new ClassAttendanceDA();
            _clsPayment.CLS_ID = classID;
            System.Data.DataTable table = _clsPayment.ClassHistory().Tables[0];
            if (table.Rows.Count > 0)
            {
                grdClassHistory.ItemsSource = table.DefaultView;
            }
            else
            {
                grdClassHistory.ItemsSource = null;
            }
        }
    }
}
