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
    /// Interaction logic for TuteDistributedStudent.xaml
    /// </summary>
    public partial class TuteDistributedStudent : UserControl
    {

        public Int32 AccossoryID { get; set; }
        public Int32 classID { get; set; }
        public DateTime classDate { get; set; }
        public Double AccossoryAmount { get; set; }
        public TuteDistributedStudent()
        {
            InitializeComponent();
        }

        public void LoadFormContaint()
        {
            BindStudentGrid();
        }

        private void BindStudentGrid()
        {
            try
            {
                AddTuteDA _clsPayment = new AddTuteDA();
                _clsPayment.CLS_ID = classID;
                _clsPayment.CLS_REC_DATE = classDate;
                _clsPayment.ACC_ID = AccossoryID;
                System.Data.DataTable table = _clsPayment.SelectAccessoryStudent().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdAccessoryStudent.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdAccessoryStudent.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void chkMark_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdAccessoryStudent.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    DoPaymentDA _clsPayment = new DoPaymentDA();
                    _clsPayment.CLS_ID = classID;
                    _clsPayment.ASS_REC_DATE = classDate;
                    _clsPayment.ACC_ID = AccossoryID;
                    _clsPayment.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    _clsPayment.ASS_REC_AMOUNT = AccossoryAmount;
                    _clsPayment.UpdateStudentAccessory();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkMark_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdAccessoryStudent.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        AddTuteDA _clsPayment = new AddTuteDA();
                        _clsPayment.CLS_ID = classID;
                        _clsPayment.CLS_REC_DATE = classDate;
                        _clsPayment.ACC_ID = AccossoryID;
                        _clsPayment.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        _clsPayment.DeleteStudentAccessory();
                    }
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
