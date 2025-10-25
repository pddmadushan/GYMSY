using System;
using System.Collections.Generic;
using System.Data;
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

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for SendSMS.xaml
    /// </summary>
    public partial class SendSMS : UserControl
    {
        public DataTable filteredTable { get; set; }
        public Boolean callingForm { get; set; }
        public String filteredString { get; set; }
        private string teliphoneNumber;
        public SendSMS()
        {
            InitializeComponent();
            chkAll.IsChecked = false;
            teliphoneNumber = System.Configuration.ConfigurationManager.AppSettings.Get("AdminPhoneNumber").ToString();
            txtTelephoneNumber.Text = teliphoneNumber;
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.StudentFilters form = new CMSXtream.Pages.DataEntry.StudentFilters();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Sent SMS to Students",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 800
                };
                form.stdSum = this;
                dialog.ShowDialog();
                BindStudentGrid();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void BindStudentGrid()
        {
            try
            {
                System.Data.DataTable table = null;
                SearchText.Content = filteredString;
                SearchText.ToolTip = filteredString; 
                if (filteredTable != null)
                {
                    table = filteredTable;
                    if (table.Rows.Count > 0)
                    {
                        grdSMSList.ItemsSource = table.DefaultView;
                    }
                    else
                    {
                        grdSMSList.ItemsSource = null;
                    }
                }
                else
                {
                    grdSMSList.ItemsSource = null;                    
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void LoadSMSHistory()
        {
            try
            {
                Int32 empNumber = -1;

                if (!chkAll.IsChecked.Value)
                {
                    if (txtEmpNumber.Text.Trim().ToString() != string.Empty)
                    {

                        Int32 temp;
                        if (Int32.TryParse(txtEmpNumber.Text.Trim().ToString(), out temp))
                        {
                            empNumber = Int32.Parse(txtEmpNumber.Text.Trim().ToString());
                        }
                        else
                        {
                            MessageBox.Show("Enter student number with correct format!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = empNumber;
                System.Data.DataTable table = _clsStudent.SelectSMSHISTORY().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdSMSHistory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdSMSHistory.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DataTable dt = new DataTable();
                dt.Columns.Add("STD_ID", typeof(Int32));
                dt.Columns.Add("TELEPHONE_NO", typeof(string));
                dt.Columns.Add("CLS_MESSAGE", typeof(string));

                Int32 StdID;
                string Telephone = string.Empty;
                string Message;
                Message = txtRichBox.Text.Trim();
                if (Message == string.Empty)
                {
                    MessageBox.Show("There is no message to send!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtRichBox.Focus();
                    return;
                }

                if (!(txtTelephoneNumber.Text.Trim() == String.Empty || txtTelephoneNumber.Text.Trim() == "(0)-"))
                {
                    string telNumber = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                    if (telNumber.Length != 10)
                    {
                        MessageBox.Show("Incorrect Telephone number!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtTelephoneNumber.Focus();
                        return;
                    }
                    else
                    {
                        Telephone = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                        dt.Rows.Add(0, Telephone, Message);
                    }
                }

                int rowCount = grdSMSList.Items.Count;
                if (rowCount == 0 && dt.Rows.Count == 0)
                {
                    MessageBox.Show("Unable to find any recipient!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtTelephoneNumber.Focus();
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Do you want to continue with following details? \n Checking number : " + Telephone + "\n Number of recipient : " + rowCount, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdSMSList.Items[i];
                        StdID = Int32.Parse(selectedRow["STD_ID"].ToString());
                        Telephone = selectedRow["STD_TELEPHONE"].ToString();
                        dt.Rows.Add(StdID, Telephone, Message);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        SMSDA _clsStudent = new SMSDA();
                        _clsStudent.SMSList(dt);
                        grdSMSList.ItemsSource = null;
                        SearchText.Content = "";
                        filteredTable = null;
                        filteredString = string.Empty;
                        txtTelephoneNumber.Text = teliphoneNumber;
                        txtRichBox.Text = string.Empty; 
                        LoadSMSHistory();
                    }
                }

                if (callingForm)
                {
                    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void txtRichBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Int32 maxChr = txtRichBox.MaxLength;
                Int32 currentChr = txtRichBox.Text.Trim().Length;
                Int32 remainChr = maxChr - txtRichBox.Text.Trim().Length;
                RemainingChar.Content = remainChr < 0 ? "0" : remainChr.ToString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }


        private void grdSMSList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = grdSMSList.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                Int32 std = Int32.Parse(selectedRow["STD_ID"].ToString());
                chkAll.IsChecked = false;
                txtEmpNumber.Text = std.ToString();
                LoadSMSHistory();
            }

        }

        private void grdSMSHistory_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = grdSMSHistory.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                string sSMS = selectedRow["CLS_MESSAGE"].ToString();
                txtRichBox.Text = sSMS;
            }
        }

        private void btnSearchSMS_Click(object sender, RoutedEventArgs e)
        {
            chkAll.IsChecked = false;
            LoadSMSHistory();
        }

        private void chkAll_Checked(object sender, RoutedEventArgs e)
        {
            txtEmpNumber.Text = "";
            LoadSMSHistory();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Edit Student",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdSMSList.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());

                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.STD_ID = stAttPass.STD_ID;

                    System.Data.DataTable table = _clsStudent.SelectStudentDetails().Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        stAttPass.CLS_ID = int.Parse(table.Rows[0]["CLS_ID"].ToString());
                        stAttPass.CLS_NAME = table.Rows[0]["CLS_NAME"].ToString();
                        stAttPass.STD_INITIALS = table.Rows[0]["STD_INITIALS"].ToString();
                        stAttPass.STD_SURNAME = table.Rows[0]["STD_SURNAME"].ToString();
                        stAttPass.STD_FULL_NAME = table.Rows[0]["STD_FULL_NAME"].ToString();
                        stAttPass.STD_GENDER = int.Parse(table.Rows[0]["STD_GENDER"].ToString());
                        stAttPass.STD_DATEOFBIRTH = DateTime.Parse(table.Rows[0]["STD_DATEOFBIRTH"].ToString());
                        stAttPass.STD_JOIN_DATE = DateTime.Parse(table.Rows[0]["STD_JOIN_DATE"].ToString());
                        stAttPass.STD_EMAIL = table.Rows[0]["STD_EMAIL"].ToString();
                        stAttPass.STD_NIC = table.Rows[0]["STD_NIC"].ToString();
                        stAttPass.STD_ADDRESS = table.Rows[0]["STD_ADDRESS"].ToString();
                        stAttPass.STD_CLASS_FEE = Double.Parse(table.Rows[0]["STD_CLASS_FEE"].ToString());
                        stAttPass.STD_TELEPHONE = table.Rows[0]["STD_TELEPHONE"].ToString();
                        stAttPass.STD_ACTIVE_FLG = table.Rows[0]["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(table.Rows[0]["STD_ACTIVE_FLG"].ToString());
                        stAttPass.STD_COMMENT = table.Rows[0]["STD_COMMENT"].ToString();
                        stAttPass.STD_TEMP_NOTE = table.Rows[0]["STD_TEMP_NOTE"].ToString();
                        stAttPass.RSN_ID = int.Parse(table.Rows[0]["RSN_ID"].ToString());
                    }
                    else
                    {
                        return;
                    }
                }
                form.IsAddNew = false;
                form.stAtt = stAttPass;
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    BindStudentGrid();
                }
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
                MessageBoxResult result = MessageBox.Show("Do you want to delete Pending SMS?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SMSDA _clsSms = new SMSDA();
                    Int32 effectedCount = _clsSms.DeletePendingSMS();
                    if (effectedCount > 0)
                    {
                        LoadSMSHistory();
                        MessageBox.Show("Successfully deleted Pending SMS", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
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

        private void grdSMSHistory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    var selectedRow = grdSMSHistory.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        MessageBoxResult result = MessageBox.Show("Do you want to delete this message? \n Message Info:  \n " + selectedRow["SMS_INFO"].ToString(), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            SMSDA _clsSms = new SMSDA();
                            Int32 inboxID = int.Parse(selectedRow["INBOX_ID"].ToString());
                            Int32 stdID = int.Parse(selectedRow["STD_ID"].ToString());
                            _clsSms.DeletePendingStudentSMS(inboxID, stdID);
                        }
                        else
                        {
                            e.Handled = true;
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


        //private void txtRichBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextRange textRange = new TextRange(txtRichBox.Document.ContentStart, txtRichBox.Document.ContentEnd);
        //    var text = textRange.Text.Trim();
        //    if (text.Length > 160)
        //    {
        //        int gap = 0;
        //        while (txtRichBox.CaretPosition.DeleteTextInRun(-1) == 0)
        //        {
        //            txtRichBox.CaretPosition = txtRichBox.CaretPosition.GetPositionAtOffset(--gap);
        //        }

        //    }
        //}

        //private void txtRichBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    //TextRange tr = new TextRange(txtRichBox.Document.ContentStart, richTextBox1.Document.ContentEnd);
        //    //if (tr.Text.Length >= 300 || e.Key == Key.Space || e.Key == Key.Enter)
        //    //{
        //    //    e.Handled = true;
        //    //    return;
        //    //}
        //}

    }
}
