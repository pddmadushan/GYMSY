using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;
using XtreamDataAccess;

namespace CMSXtream
{
    /// <summary>
    /// Interaction logic for PaymentPopup.xaml
    /// </summary>
    public partial class PaymentPopup : UserControl
    {
        public string OutResult { get; set; }
        public int studentId { get; set; }
        public string studentName { get; set; }
        public DateTime currentClsDate { get; set; }

        public PaymentPopup()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void BindPaymentGrid()
        {
            try
            {

                PaymentPopupDA _clsPayment = new PaymentPopupDA();
                _clsPayment.STD_ID = studentId;
                System.Data.DataTable table = _clsPayment.SelectTop10Payment().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdPayment.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdPayment.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var selectedRow = grdPayment.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    PaymentPopupDA _clsPayment = new PaymentPopupDA();
                    //TextBlock MyTextBlock = FindChild<TextBlock>(MyDataGrid, "MyTextBlock");

                    DataGridRow row = (DataGridRow)(grdPayment.ItemContainerGenerator.ContainerFromItem(grdPayment.SelectedItem));
                    TextBox txtPayment = Helper.FindChild<TextBox>(row, "txtPayment");
                    CheckBox chkMark = Helper.FindChild<CheckBox>(row, "chkMark");
                    if (txtPayment != null && chkMark != null)
                    {
                        string PaidAmontStrin = txtPayment.Text.ToString();
                        if (PaidAmontStrin.Trim() == "") { PaidAmontStrin = "0"; }
                        double paidAmount = Double.Parse(PaidAmontStrin);
                        double clsFree = Double.Parse(selectedRow["CLASS_FEE"].ToString());
                        if (paidAmount > clsFree || paidAmount == 0)
                        {
                            MessageBox.Show("Please enter valid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            return;
                        }

                        _clsPayment.STD_ID = studentId;
                        _clsPayment.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                        _clsPayment.PAID_YEAR = int.Parse(selectedRow["STM_YEAR"].ToString());
                        _clsPayment.PAID_MONTH = int.Parse(selectedRow["STM_MONTH"].ToString());
                        _clsPayment.STD_REC_DATE = currentClsDate;
                        _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                        _clsPayment.CARD_ISSUED_FLG = chkMark.IsChecked.Value ? 1 : 0;
                        _clsPayment.PAID_AMOUNT = paidAmount;
                        _clsPayment.InsertClassAttendance();
                    }
                }

                OutResult = StaticProperty.SaveMessage;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void MakeGridEditable()
        {
            try
            {
                int rowCount = grdPayment.Items.Count;

                for (int i = rowCount - 1; 0 <= i; i--)
                {
                    // Get the state of what's in column 1 of the current row (in my case a string)
                    var selectedRow = (System.Data.DataRowView)grdPayment.Items[i];
                    DataGridRow row = (DataGridRow)grdPayment.ItemContainerGenerator.ContainerFromItem(selectedRow);
                    TextBox txtPayment = Helper.FindChild<TextBox>(row, "txtPayment");
                    if (txtPayment != null)
                    {
                        string PaidAmontStrin = txtPayment.Text.ToString();
                        if (PaidAmontStrin.Trim() == "") { PaidAmontStrin = "0"; }
                        double paidAmount = Double.Parse(PaidAmontStrin);
                        double clsFree = Double.Parse(selectedRow["CLASS_FEE"].ToString());

                        if (paidAmount < clsFree)
                        {
                            txtPayment.IsEnabled = true;
                            break;
                        }
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                int rowCount = grdPayment.Items.Count;
                for (int i = rowCount - 1; 0 <= i; i--)
                {
                    var selectedRow = (System.Data.DataRowView)grdPayment.Items[i];
                    DataGridRow row = (DataGridRow)grdPayment.ItemContainerGenerator.ContainerFromItem(selectedRow);
                    TextBox txtPayment = Helper.FindChild<TextBox>(row, "txtPayment");
                    if (txtPayment != null)
                    {
                        string PaidAmontStrin = txtPayment.Text.ToString();
                        if (PaidAmontStrin.Trim() == "") { PaidAmontStrin = "0"; }
                        double paidAmount = Double.Parse(PaidAmontStrin);
                        double clsFree = Double.Parse(selectedRow["CLASS_FEE"].ToString());

                        if (paidAmount < clsFree)
                        {
                            txtPayment.IsEnabled = true;
                            break;
                        }
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

        private void grdPayment_Sorting(object sender, DataGridSortingEventArgs e)
        {

        }
    }
}
