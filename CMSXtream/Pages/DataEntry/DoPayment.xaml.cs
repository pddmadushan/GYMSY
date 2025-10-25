using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for DoPayment.xaml
    /// </summary>
    public partial class DoPayment : UserControl
    {
        public Int32 studentId { get; set; }
        public string attendanceId { get; set; }
        public Int32 classID { get; set; }
        public int paidYear { get; set; }
        public int PaidMonth { get; set; }

        public Boolean cardDeliverd { get; set; }
        public string className { get; set; }
        public string yearMonth { get; set; }
        public string classFee { get; set; }
        public DateTime clsDate { get; set; }

        public int MemberActive { get; set; }

        public DoPayment()
        {
            InitializeComponent();
            LoadMonths();
            dtpDatePaid.SelectedDate = DateTime.Now;
            //dtpDatePicke.SelectedDate = DateTime.Now;
        }

        public void LoadMonths()
        {
            try
            {
                List<PartClass> PartClass = new List<PartClass>();
                PartClass.Add(new PartClass(1, "1 Month"));
                PartClass.Add(new PartClass(2, "2 Months"));
                PartClass.Add(new PartClass(3, "3 Months"));
                PartClass.Add(new PartClass(4, "4 Months"));
                PartClass.Add(new PartClass(5, "5 Months"));
                PartClass.Add(new PartClass(6, "6 Months"));
                PartClass.Add(new PartClass(7, "7 Months"));
                PartClass.Add(new PartClass(8, "8 Months"));
                PartClass.Add(new PartClass(9, "9 Months"));
                PartClass.Add(new PartClass(10, "10 Months"));
                PartClass.Add(new PartClass(11, "11 Months"));
                PartClass.Add(new PartClass(12, "12 Months"));
                cmbPaidMonth.ItemsSource = PartClass;
                cmbPaidMonth.DisplayMemberPath = "PART_NAME";
                cmbPaidMonth.SelectedValuePath = "PART_ID";
                cmbPaidMonth.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //public void LoadFormDateNextPayment()
        //{
        //    if (paidYear * 12 + PaidMonth <= DateTime.Now.Year * 12 + DateTime.Now.Month)
        //    {
        //        dtpDatePicke.SelectedDate = DateTime.Now;
        //        DateTime value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //        dtpDatePicke.DisplayDateStart = value;
        //    }
        //}

        public void LoadFormContaintnNextButton()
        {
            lblClass.Content = className;
            lblYearMonth.Content = yearMonth;
            txtAmount.Text = classFee;
            chkCardIssue.IsChecked = cardDeliverd;

            //if (paidYear * 12 + PaidMonth < DateTime.Now.Year * 12 + DateTime.Now.Month)
            //{
            //    dtpDatePicke.SelectedDate = DateTime.Now;
            //    DateTime value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //    dtpDatePicke.DisplayDateStart = value;
            //}
            //else
            //{
            //    DateTime value = new DateTime(paidYear, PaidMonth, 1);
            //    dtpDatePicke.SelectedDate = value;
            //    dtpDatePicke.DisplayDateStart = value;
            //}
            //dtpDatePicke.IsEnabled = true;

            rdoBtn.IsChecked = false;
            BindPaymentHistoryGrid();
        }

        public void LoadFormContaint()
        {
            lblClass.Content = className;
            lblYearMonth.Content = yearMonth;
            txtAmount.Text = classFee;
            chkCardIssue.IsChecked = cardDeliverd;

            rdoBtn.IsChecked = false;
            BindPaymentHistoryGrid();
        }
        public void BindPaymentHistoryGrid()
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;

                if (lblClass.Visibility == System.Windows.Visibility.Visible)
                {
                    _clsPayment.CLS_ID = classID;
                    _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                    _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;
                }
                else
                {
                    _clsPayment.CLS_ID = int.Parse(cmbClsGroup.SelectedValue.ToString()); ;
                    _clsPayment.PAID_YEAR = dtpDatePicke.SelectedDate.Value.Year;
                    _clsPayment.PAID_MONTH = dtpDatePicke.SelectedDate.Value.Month;
                }

                System.Data.DataTable table = _clsPayment.SelectPayment().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdPayHistory.ItemsSource = table.DefaultView;
                    cmbPaidMonth.SelectedValue = table.Rows[0]["PAID_FOR_MONTHS"].ToString();
                    cmbPaidMonth.IsEnabled = false;
                    DateTime selectedDate = DateTime.Parse(table.Rows[0]["VALID_DUE_DATE"].ToString());
                    dtpValidTillDate.SelectedDate = selectedDate;
                    dtpDatePicke.SelectedDate = DateTime.Parse(table.Rows[0]["STD_REC_DATE"].ToString());
                    dtpDatePicke.IsEnabled = false;
                }
                else
                {
                    dtpDatePicke.IsEnabled = true;
                    cmbPaidMonth.IsEnabled = true;
                    grdPayHistory.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnDoPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dtpDatePicke.SelectedDate == null)
                {
                    MessageBox.Show("Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                DateTime d;
                if (grdPayHistory.Items.Count == 0)
                {
                    d = dtpDatePicke.SelectedDate.Value;
                }
                else
                {
                    d = dtpDatePaid.SelectedDate.Value;
                }

                double paidYearamt = getPaidAmount(d);

                if ((double.Parse(txtAmount.Text.Trim()) <= paidYearamt))
                {
                    MessageBox.Show("Full payment has already been done!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                if (rdoBtn.IsChecked.Value == false)
                {
                    if (txtPayment.Text.Trim() == "")
                    {
                        MessageBox.Show("Payment amount is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                        return;
                    }
                    if (double.Parse(txtPayment.Text.Trim()) < 0)
                    {
                        MessageBox.Show("Please define a valid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                        return;
                    }

                    if ((double.Parse(txtAmount.Text.Trim()) - paidYearamt) < double.Parse(txtPayment.Text.Trim()))
                    {
                        MessageBox.Show("Payment should not be greater than balance to paid!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                        return;
                    }
                }

                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;

                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;

                _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                _clsPayment.PAID_AMOUNT = double.Parse(txtPayment.Text.Trim());

                if (grdPayHistory.Items.Count == 0)
                {
                    _clsPayment.STD_REC_DATE = dtpDatePicke.SelectedDate.Value;
                }
                else
                {
                    _clsPayment.STD_REC_DATE = dtpDatePaid.SelectedDate.Value;
                }


                _clsPayment.CLASS_FEE = double.Parse(txtAmount.Text.Trim());
                _clsPayment.CARD_ISSUED_FLG = chkCardIssue.IsChecked.Value ? 1 : 0;

                _clsPayment.PAID_FOR_MONTHS = Int32.Parse(cmbPaidMonth.SelectedValue.ToString());
                _clsPayment.VALID_DUE_DATE = dtpValidTillDate.SelectedDate.Value;
                _clsPayment.ACTUAL_PAID_DATE = dtpDatePaid.SelectedDate.Value;

                if (_clsPayment.UpdatePayment() > 0)
                {
                    if (MemberActive !=1)
                    {
                        if (StaticProperty.SKTIPAddres != "")
                        {
                            if (StaticProperty.SDK.GetConnectState())
                            {
                                DateTime machinDate = StaticProperty.SDK.getdeviseTime().Value;
                                _clsPayment.MACHIN_DATE = machinDate;
                                if (_clsPayment.CanActive() == "1")
                                {
                                    int rec = 0;
                                    string sUserID = attendanceId;
                                    SMSDA _obj = new SMSDA();
                                    DataSet usersPF = _obj.GetUsersFP(sUserID);

                                    rec = StaticProperty.SDK.sta_SetUserFPInfo(sUserID, usersPF);
                                    if (rec != 1)
                                    {
                                        MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                    }
                                    else
                                    {
                                        this.MemberActive = 1;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                btnSave.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                    }
                }

                BindPaymentHistoryGrid();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private double getPaidAmount(DateTime selectDate)
        {
            double paidgetPaidAmount = 0;
            int rowCount = grdPayHistory.Items.Count;
            for (int i = 0; i < rowCount; i++)
            {
                var selectedRow = (System.Data.DataRowView)grdPayHistory.Items[i];
                if (txtPayment != null)
                {

                    double PaidFree = Double.Parse(selectedRow["PAID_AMOUNT"].ToString());
                    DateTime selectedDate = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                    if (selectedDate != selectDate)
                    {
                        paidgetPaidAmount += PaidFree;
                    }
                }
            }
            return paidgetPaidAmount;

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtpDatePicke.SelectedDate == null)
                {
                    MessageBox.Show("Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                DateTime d;
                if (grdPayHistory.Items.Count == 0)
                {
                    d = dtpDatePicke.SelectedDate.Value;
                }
                else
                {
                    d = dtpDatePaid.SelectedDate.Value;
                }

                double paidYearamt = getPaidAmount(d);

                double balanceTobepaid = double.Parse(txtAmount.Text.Trim()) - paidYearamt;
                if (balanceTobepaid < 0) { balanceTobepaid = 0; }

                txtPayment.IsEnabled = false;
                txtPayment.Text = balanceTobepaid.ToString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPayment.IsEnabled = true;
                txtPayment.Text = "0";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkCardIssue_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;
                _clsPayment.CARD_ISSUED_FLG = 1;
                _clsPayment.UpdateCardIssued();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkCardIssue_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = paidYear;
                _clsPayment.PAID_MONTH = PaidMonth;
                _clsPayment.CARD_ISSUED_FLG = 0;
                _clsPayment.UpdateCardIssued();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                lblChange.Visibility = System.Windows.Visibility.Hidden;
                lblUpdate.Visibility = System.Windows.Visibility.Visible;
                txtAmount.IsEnabled = true;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void lblUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Fee is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }
                if (double.Parse(txtAmount.Text.Trim()) < 0)
                {
                    MessageBox.Show("Please define a valid Fee!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                double paidYearamt = getPaidAmount(new DateTime(1, 1, 1));

                if ((double.Parse(txtAmount.Text.Trim()) < paidYearamt))
                {
                    MessageBox.Show("Define Member fee is less than already paid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;
                _clsPayment.CLASS_FEE = double.Parse(txtAmount.Text.Trim());
                _clsPayment.UpdateClassFee();

                lblChange.Visibility = System.Windows.Visibility.Visible;
                lblUpdate.Visibility = System.Windows.Visibility.Hidden;
                txtAmount.IsEnabled = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.LoginisAdmin != "1")
                {
                    MessageBox.Show("Sorry, You can not delete the records!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.No);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdPayHistory.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        DoPaymentDA _clsPayment = new DoPaymentDA();
                        _clsPayment.STD_ID = studentId;
                        _clsPayment.CLS_ID = classID;
                        _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                        _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;
                        _clsPayment.STD_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString()); ;
                        _clsPayment.DeletePayment();
                        BindPaymentHistoryGrid();
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

        private void dtpDatePicke_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (rdoBtn != null)
                {
                    rdoBtn.IsChecked = false;
                }
                SetTillDate();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbClsGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                ClassGroups = (List<ClassGroup>)cmbClsGroup.ItemsSource;
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    Int32 classGroupID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    ClassGroup result = ClassGroups.Find(x => x.CLS_ID == classGroupID);
                    if (result != null)
                    {
                        txtAmount.Text = result.CLS_FEE.ToString();
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

        private void cmbPaidMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetTillDate();
        }

        private void SetTillDate()
        {
            try
            {
                int rowCount = grdPayHistory.Items.Count;
                if (rowCount == 0)
                {
                    if (cmbPaidMonth.SelectedIndex >= 0 && dtpDatePicke.SelectedDate != null)
                    {
                        Int32 monthPaidFor = Int32.Parse(cmbPaidMonth.SelectedValue.ToString());
                        dtpValidTillDate.SelectedDate = dtpDatePicke.SelectedDate.Value.AddMonths(monthPaidFor);
                    }
                }
                else
                {
                    var selectedRow = (System.Data.DataRowView)grdPayHistory.Items[0];
                    DateTime selectedDate = DateTime.Parse(selectedRow["VALID_DUE_DATE"].ToString());
                    dtpValidTillDate.SelectedDate = selectedDate;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpPayMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DateTime dt = dtpPayMonth.SelectedDate.Value;
                dtpDatePicke.SelectedDate = DateTime.Now;
                DateTime value = new DateTime(dt.Year, dt.Month, 1);
                dtpDatePicke.DisplayDateStart = value;
                DateTime valueEnd = value.AddMonths(1).AddDays(-1);
                dtpDatePicke.DisplayDateEnd = valueEnd;

                if (value <= DateTime.Now & DateTime.Now <= valueEnd)
                {
                    dtpDatePicke.SelectedDate = DateTime.Now;
                }
                else
                {
                    dtpDatePicke.SelectedDate = value;
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
