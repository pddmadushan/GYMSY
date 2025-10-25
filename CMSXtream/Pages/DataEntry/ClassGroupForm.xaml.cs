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
    /// Interaction logic for ClassGroupForm.xaml
    /// </summary>
    public partial class ClassGroupForm : UserControl
    {
        public Boolean IsAddNew { get; set; }
        public string OutResult { get; set; }
        public ClassGroupAttribute clasAtt;
        public Double ClassFee { get; set; }

        public ClassGroupForm()
        {
            InitializeComponent();
        }

        public void LoadFormContaint()
        {
            genarateTime();
            genarateDay();
            genarateDuration();
            LoadClassCatogory();

            if (IsAddNew)
            {
                lblClassId.Content = "[New Class]";
                btnDelete.Visibility = System.Windows.Visibility.Hidden;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                grdClassInfo.Visibility = System.Windows.Visibility.Hidden;
                cmbday.SelectedIndex = -1;
                rdbtnClass.IsEnabled = true;
                rdbtnClass.IsChecked = false;
                grpStudentTransfer.Visibility = System.Windows.Visibility.Hidden;
                ClearControll();
            }
            else
            {
                lblClassId.Content = clasAtt.CLS_ID.ToString();
                cmbClsCatogory.SelectedValue = clasAtt.CAT_ID.ToString();
                dtpStartDate.SelectedDate = clasAtt.CLS_START_DATE;
                cmbday.SelectedValue = clasAtt.CLS_DAY.ToString();
                cmbClassTime.SelectedValue = clasAtt.CLS_TIME.ToString();
                cmbClassDuration.SelectedValue = clasAtt.CLS_DURATION.ToString();
                txtClassName.Text = clasAtt.CLS_NAME.ToString();
                txtClsFee.Text = clasAtt.CLS_FEE.ToString();
                ClassFee = clasAtt.CLS_FEE; 
                txtClsAdmission.Text = clasAtt.CLS_ADMITION_AMT.ToString();
                chkActive.IsChecked = clasAtt.CLS_ACTIVE_FLG == 0 ? false : true;
                txtComment.Text = clasAtt.CLS_COMMENT.ToString();
                txtWeekNumber.Text = clasAtt.TOTAL_NUMBER_OF_WEEK.ToString();
                rdbtnClass.IsChecked = clasAtt.IS_CLASS_FLG == 1 ? true : false;
                if (clasAtt.IS_CLASS_FLG == 1)
                {
                    grdClassInfo.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    grdClassInfo.Visibility = System.Windows.Visibility.Hidden;
                }
                rdbtnClass.IsEnabled = false;
                LoadClassCombo();
            }
        }

        private bool ValidatetoSave()
        {
            try
            {
                if (txtClassName.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Description is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtClassName.Focus();
                    return false;
                }

                if (!(Boolean.Parse(chkActive.IsChecked.ToString())))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to inactive this class?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        chkActive.Focus();
                        return false;
                    }
                }

                if (txtClsFee.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Fee is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtClsFee.Focus();
                    return false;
                }


                if (rdbtnClass.IsChecked.Value)
                {
                    if (cmbClsCatogory.SelectedIndex < 0)
                    {
                        MessageBox.Show("Please select a class category!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        cmbClsCatogory.Focus();
                        return false;
                    }

                    if (cmbday.SelectedIndex < 0)
                    {
                        MessageBox.Show("Please select a day of the week!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        cmbday.Focus();
                        return false;
                    }
             
                    if (cmbClassTime.SelectedIndex < 0)
                    {
                        MessageBox.Show("Please select a start time!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        cmbClassTime.Focus();
                        return false;
                    }
                    if (txtClsAdmission.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Class Admission is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtClsAdmission.Focus();
                        return false;
                    }
                    if (cmbClassDuration.SelectedIndex < 0)
                    {
                        MessageBox.Show("Please select a duration!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        cmbClassDuration.Focus();
                        return false;
                    }
                    if (txtWeekNumber.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Total Number of Weeks is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtClsFee.Focus();
                        return false;
                    }
                    if (Int32.Parse(txtWeekNumber.Text.Trim()) == 0)
                    {
                        MessageBox.Show("Total Number of Weeks should not be zero!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtClsFee.Focus();
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return false;
            }
        }

        private void ClearControll()
        {
            try
            {
                cmbClsCatogory.SelectedIndex = -1;
                txtClassName.Text = String.Empty;
                cmbday.SelectedIndex = -1;
                dtpStartDate.SelectedDate = DateTime.Now;
                cmbClassTime.SelectedIndex = -1;
                cmbClassDuration.SelectedIndex = -1;
                txtClsAdmission.Text = "0.00";
                txtClsFee.Text = "";
                chkActive.IsChecked = true;
                txtComment.Text = String.Empty;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Int32 updateClassFees = 0 ;
            if (ValidatetoSave())
            {
                try
                {
                    ClassGroupDA _clsClassGroup = new ClassGroupDA();
                    if (lblClassId.Content.ToString() == "[New Class]")
                    {
                        _clsClassGroup.classFeilds.CLS_ID = 0;
                    }
                    else
                    {
                        _clsClassGroup.classFeilds.CLS_ID = Int32.Parse(lblClassId.Content.ToString());
                    }
                    _clsClassGroup.classFeilds.CLS_NAME = txtClassName.Text.ToString();
                    _clsClassGroup.classFeilds.CLS_ACTIVE_FLG = chkActive.IsChecked.Value ? 1 : 0;
                    _clsClassGroup.classFeilds.CLS_COMMENT = txtComment.Text.ToString();
                    _clsClassGroup.classFeilds.CLS_FEE = double.Parse(txtClsFee.Text.Trim());
                    if (rdbtnClass.IsChecked.Value)
                    {
                        _clsClassGroup.classFeilds.CAT_ID = Int32.Parse(cmbClsCatogory.SelectedValue.ToString());
                        _clsClassGroup.classFeilds.CLS_START_DATE = dtpStartDate.SelectedDate.Value;
                        _clsClassGroup.classFeilds.CLS_DAY = Int32.Parse(cmbday.SelectedValue.ToString());
                        _clsClassGroup.classFeilds.CLS_TIME = double.Parse(cmbClassTime.SelectedValue.ToString());
                        _clsClassGroup.classFeilds.CLS_DURATION = double.Parse(cmbClassDuration.SelectedValue.ToString());                        
                        
                        _clsClassGroup.classFeilds.CLS_ADMITION_AMT = double.Parse(txtClsAdmission.Text.Trim());
                        _clsClassGroup.classFeilds.TOTAL_NUMBER_OF_WEEK = Int32.Parse(txtWeekNumber.Text.Trim());
                        _clsClassGroup.classFeilds.IS_CLASS_FLG = 1;

                        if (_clsClassGroup.classFeilds.CLS_ID != 0 && ClassFee != _clsClassGroup.classFeilds.CLS_FEE)
                        {
                            MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to update existing students’ class fees from " + ClassFee.ToString() + " to " + _clsClassGroup.classFeilds.CLS_FEE.ToString() + " ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (resultMessageBox == MessageBoxResult.Yes)
                            {
                                updateClassFees = 1 ; 
                            }
                        }
                    }
                    else
                    {
                        _clsClassGroup.classFeilds.CAT_ID = 0;
                        _clsClassGroup.classFeilds.CLS_START_DATE = DateTime.Now;
                        _clsClassGroup.classFeilds.CLS_DAY = 0;
                        _clsClassGroup.classFeilds.CLS_TIME = 0;
                        _clsClassGroup.classFeilds.CLS_DURATION = 0;
                        _clsClassGroup.classFeilds.CLS_ADMITION_AMT = 0;
                        _clsClassGroup.classFeilds.TOTAL_NUMBER_OF_WEEK = 0;
                        _clsClassGroup.classFeilds.IS_CLASS_FLG = 0;
                    }
                    System.Data.DataTable table = _clsClassGroup.InsertClass(updateClassFees).Tables[0];
                    lblClassId.Content = table.Rows[0]["CLS_ID"].ToString();
                    string returnMsg = table.Rows[0]["RETURN_MSG"].ToString();

                    OutResult = returnMsg;
                    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();

                }
                catch (Exception ex)
                {
                    LogFile logger = new LogFile();
                    logger.MyLogFile(ex);
                    MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this class?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ClassGroupDA _clsClass = new ClassGroupDA();
                    _clsClass.classFeilds.CLS_ID = Int32.Parse(lblClassId.Content.ToString());
                    if (_clsClass.DeleteClass() > 0)
                    {
                        OutResult = "Record has been successful deleted.";
                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    MessageBox.Show("You cannot delete this record .Record has been used somewhere else in the system!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                }
                else
                {
                    LogFile logger = new LogFile();
                    logger.MyLogFile(ex);
                    MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                }
            }
        }

        private void LoadClassCatogory()
        {
            ClassCategoryDA _clsClassGroup = new ClassCategoryDA();
            System.Data.DataTable table = _clsClassGroup.SelectAllCategory().Tables[0];

            cmbClsCatogory.DisplayMemberPath = "CAT_NAME";
            cmbClsCatogory.SelectedValuePath = "CAT_ID";
            cmbClsCatogory.ItemsSource = table.DefaultView;

        }

        private void LoadClassCombo()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassGroup(-1).Tables[0];
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    ClassGroups.Add(new ClassGroup(Int32.Parse(row["CLS_ID"].ToString()), row["CLS_NAME"].ToString(), double.Parse(row["CLS_FEE"].ToString())));
                }
                cmbClsGroup.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbClsGroup.ItemsSource = ClassGroups;
                cmbClsGroup.DisplayMemberPath = "CLS_NAME";
                cmbClsGroup.SelectedValuePath = "CLS_ID";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void genarateDay()
        {
            cmbday.DisplayMemberPath = "Value";
            cmbday.SelectedValuePath = "Key";
            cmbday.Items.Add(new KeyValuePair<string, string>("1", "Monday"));
            cmbday.Items.Add(new KeyValuePair<string, string>("2", "Tuesday"));
            cmbday.Items.Add(new KeyValuePair<string, string>("3", "Wednesday"));
            cmbday.Items.Add(new KeyValuePair<string, string>("4", "Thursday"));
            cmbday.Items.Add(new KeyValuePair<string, string>("5", "Friday"));
            cmbday.Items.Add(new KeyValuePair<string, string>("6", "Saturday"));
            cmbday.Items.Add(new KeyValuePair<string, string>("7", "Sunday"));
        }

        private void genarateDuration()
        {
            cmbClassDuration.DisplayMemberPath = "Value";
            cmbClassDuration.SelectedValuePath = "Key";
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("1", "1 Hour"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("1.15", "1 Hour 15 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("1.3", "1 Hour 30 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("1.45", "1 Hour 45 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("2", "2 Hours"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("2.15", "2 Hours 15 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("2.3", "2 Hours 30 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("2.45", "2 Hours 45 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("3", "3 Hours"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("3.15", "3 Hours 15 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("3.3", "3 Hours 30 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("3.45", "3 Hours 45 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("4", "4 Hours"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("4.15", "4 Hours 15 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("4.3", "4 Hours 30 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("4.45", "4 Hours 45 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("5", "5 Hours"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("5.15", "5 Hours 15 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("5.3", "5 Hours 30 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("5.45", "5 Hours 45 Minites"));
            cmbClassDuration.Items.Add(new KeyValuePair<string, string>("6", "6 Hours"));
        }

        private void genarateTime()
        {
            cmbClassTime.DisplayMemberPath = "Value";
            cmbClassTime.SelectedValuePath = "Key";
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("5", "05:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("5.15", "05:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("5.3", "05:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("5.45", "05:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("6", "06:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("6.15", "06:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("6.3", "06:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("6.45", "06:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("7", "07:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("7.15", "07:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("7.3", "07:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("7.45", "07:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("8", "08:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("8.15", "08:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("8.3", "08:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("8.45", "08:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("9", "09:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("9.15", "09:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("9.3", "09:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("9.45", "09:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("10", "10:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("10.15", "10:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("10.3", "10:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("10.45", "10:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("11", "11:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("11.15", "11:15 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("11.3", "11:30 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("11.45", "11:45 AM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("12", "12:00 AM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("12.15", "12:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("12.3", "12:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("12.45", "12:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("13", "01:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("13.15", "01:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("13.3", "01:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("13.45", "01:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("14", "02:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("14.15", "02:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("14.3", "02:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("14.45", "02:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("15", "03:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("15.15", "03:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("15.3", "03:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("15.45", "03:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("16", "04:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("16.15", "04:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("16.3", "04:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("16.45", "04:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("17", "05:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("17.15", "05:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("17.3", "05:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("17.45", "05:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("18", "06:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("18.15", "06:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("18.3", "06:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("18.45", "06:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("19", "07:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("19.15", "07:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("19.3", "07:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("19.45", "07:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("20", "08:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("20.15", "08:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("20.3", "08:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("20.45", "08:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("21", "09:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("21.15", "09:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("21.3", "09:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("21.45", "09:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("22", "10:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("22.15", "10:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("22.3", "10:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("22.45", "10:45 PM"));

            cmbClassTime.Items.Add(new KeyValuePair<string, string>("23", "11:00 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("23.15", "11:15 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("23.3", "11:30 PM"));
            cmbClassTime.Items.Add(new KeyValuePair<string, string>("23.45", "11:45 PM"));

        }

        private void dtpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DateTime sdate = dtpStartDate.SelectedDate.Value;
                Int32 day = (Int32)sdate.DayOfWeek;

                if (day == 0)
                {
                    day = 7;
                }

                if (cmbday != null)
                {
                    cmbday.SelectedValue = day.ToString();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void rdbtnClass_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                grdClassInfo.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void rdbtnClass_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                grdClassInfo.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 if (cmbClsGroup.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a class group to transfer students!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    cmbClsGroup.Focus();
                    return;
                }

                if(cmbClsGroup.SelectedValue.ToString()==lblClassId.Content.ToString())
                {
                    MessageBox.Show("Please select a different class group!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    cmbClsGroup.Focus();
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Do you want to transfer student from " + txtClassName.Text.ToString() +" to " + cmbClsGroup.Text.ToString() + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    cmbClsGroup.Focus();
                    return;
                }

                ClassGroupDA _clsClassGroup = new ClassGroupDA();
                _clsClassGroup.classFeilds.CLS_ID = Int32.Parse(lblClassId.Content.ToString());
                System.Data.DataTable table = _clsClassGroup.TransferStudent(Int32.Parse(cmbClsGroup.SelectedValue.ToString())).Tables[0];

                string returnMsg = table.Rows[0]["RETURN_MSG"].ToString();
                MessageBox.Show(returnMsg, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                
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
