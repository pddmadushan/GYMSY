using System;
using System.Collections.Generic;
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
    /// Interaction logic for StudentFilters.xaml
    /// </summary>
    public partial class StudentFilters : UserControl
    {
        public CMSXtream.Pages.DataEntry.SendSMS stdSum { get; set; }
        public StudentFilters()
        {
            InitializeComponent();

            txtSearchText.Text = "";

            cmbSearchType.DisplayMemberPath = "Value";
            cmbSearchType.SelectedValuePath = "Key";
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("0", "--All--"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("1", "ID"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("2", "Name"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("3", "NIC"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("4", "Telephone"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("5", "Member Group"));

            cmbSearchType.SelectedIndex = 5;
            cmbClassGroup.SelectedIndex = -1;

            LoadClassCombo();
            LoadReason();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //private void LoadClassCatogory()
        //{
        //    ClassCategoryDA _clsClassGroup = new ClassCategoryDA();
        //    System.Data.DataTable table = _clsClassGroup.SelectAllCategory().Tables[0];
        //    cmbClsCatogory.DisplayMemberPath = "CAT_NAME";
        //    cmbClsCatogory.SelectedValuePath = "CAT_ID";
        //    cmbClsCatogory.ItemsSource = table.DefaultView;
        //    if (table.Rows.Count > 0)
        //    {
        //        cmbClsCatogory.SelectedIndex = 0;
        //    }
        //}

        private void LoadReason()
        {
            ResonDA _clsReason = new ResonDA();
            System.Data.DataTable table = _clsReason.SelectReason().Tables[0];
            cmbInactiveReason.DisplayMemberPath = "RSN_DES";
            cmbInactiveReason.SelectedValuePath = "RSN_ID";            

            if (table.Rows.Count > 0)
            {
                System.Data.DataRow toInsert = table.NewRow();
                toInsert["RSN_ID"] = -1;
                toInsert["RSN_DES"] = "Inactive but Reason not assigned";
                table.Rows.InsertAt(toInsert, 0);

                cmbInactiveReason.ItemsSource = table.DefaultView;
            }
            else
            {
                cmbInactiveReason.ItemsSource = null;
            }
        }

        private void LoadClassCombo()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassGroup(-1).Tables[0];
                cmbClassGroup.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbClassGroup.ItemsSource = table.DefaultView;
                cmbClassGroup.DisplayMemberPath = "CLS_NAME";
                cmbClassGroup.SelectedValuePath = "CLS_ID";               
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbSearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbSearchType.SelectedValue.ToString() == "0")
                {
                    txtSearchText.Text = string.Empty;
                    txtSearchText.IsEnabled = false;
                }
                else
                {
                    txtSearchText.IsEnabled = true;
                }
                if (cmbSearchType.SelectedValue.ToString() == "5")
                {
                    txtSearchText.Visibility = System.Windows.Visibility.Hidden;
                    cmbClassGroup.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    txtSearchText.Visibility = System.Windows.Visibility.Visible;
                    cmbClassGroup.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SMSDA _clsStudent = new SMSDA();
                int filterID = int.Parse(cmbSearchType.SelectedValue.ToString());
                string fiterString = txtSearchText.Text.Trim();
                string fiterDisString = txtSearchText.Text.Trim();
                if (filterID == 5)
                {
                    if (cmbClassGroup.SelectedIndex != -1)
                    {
                        fiterString = cmbClassGroup.SelectedValue.ToString();
                        fiterDisString = cmbClassGroup.Text.ToString();
                    }
                }
                System.Data.DataTable table = _clsStudent.Option01Student(filterID, fiterString).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by " + cmbSearchType.Text.ToString() + "[" + fiterDisString.ToString() + "]";
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime fromDate = DateTime.Parse(FromDate.SelectedDate.Value.ToShortDateString());
                DateTime toDate = DateTime.Parse(ToDate.SelectedDate.Value.ToShortDateString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option02Student(fromDate, toDate).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by Start Date" + "[" + FromDate.SelectedDate.Value.ToShortDateString().ToString() + " To " + ToDate.SelectedDate.Value.ToShortDateString().ToString() + "]";
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime fromDate = DateTime.Parse(FromDateEx.SelectedDate.Value.ToShortDateString());
                DateTime toDate = DateTime.Parse(ToDateEx.SelectedDate.Value.ToShortDateString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option03Student(fromDate, toDate).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Search by next payment due date" + "[" + FromDateEx.SelectedDate.Value.ToShortDateString().ToString() + " To " + ToDateEx.SelectedDate.Value.ToShortDateString().ToString() + "]";
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FromDateExAbs.SelectedDate == null)
                {
                    MessageBox.Show("Last attendance from Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                if (ToDateExAbs.SelectedDate == null)
                {
                    MessageBox.Show("Last attendance to Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                DateTime classDate = DateTime.Parse(FromDateExAbs.SelectedDate.Value.ToShortDateString());
                DateTime classToDate = DateTime.Parse(ToDateExAbs.SelectedDate.Value.ToShortDateString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option04Student(classDate, classToDate).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by Last Attendance" + "[From: " + FromDateExAbs.SelectedDate.Value.ToShortDateString() + " To " + ToDateExAbs.SelectedDate.Value.ToShortDateString().ToString() + "]";
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        
        private void btnAddList5_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInactiveReason.SelectedIndex == -1 & chkInactive.IsChecked.Value)
            {
                MessageBox.Show("Select a Reason!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                return;
            }

            Int32 ReasonId = -1;
            if (cmbInactiveReason.SelectedIndex != -1)
            {
                ReasonId = Int32.Parse(cmbInactiveReason.SelectedValue.ToString());
            }            
            Int32 InactiveFlg = chkAllInactive.IsChecked.Value ? 1 : (chkInactive.IsChecked.Value ? 2 : 0);
            String InactiveString = chkAllInactive.IsChecked.Value ? "Any Inactive" : (chkInactive.IsChecked.Value ? "Temporarily Inactive" : "Permanently Inactive");
            SMSDA _clsStudent = new SMSDA();
            System.Data.DataTable table = _clsStudent.Option05Student(ReasonId, InactiveFlg).Tables[0];
            stdSum.filteredTable = table;
            stdSum.filteredString = "Searched by Reason" + "[ " + cmbInactiveReason.Text + "][ " + InactiveString.ToString() + "]";
            ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
        }

        private void btnAddList6_Click(object sender, RoutedEventArgs e)
        {
            SMSDA _clsStudent = new SMSDA();
            System.Data.DataTable table = _clsStudent.Option06Student().Tables[0];
            stdSum.filteredTable = table;
            stdSum.filteredString = "Searched by still Valid Payment but Not Attend";
            ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
        }

    }
}
