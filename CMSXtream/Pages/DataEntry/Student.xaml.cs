using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XtreamDataAccess;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Interop;
using swf = System.Windows.Forms;
using System.IO;
using System.Data;
using System.Web.Security;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for Student.xaml
    /// </summary>
    public partial class Student : UserControl
    {
        public Boolean IsAddNew { get; set; }
        public Boolean IsControlDisable { get; set; }
        public string OutResult { get; set; }
        public StudentAttribute stAtt;
        public DateTime attDate { get; set; }
         
        public Student()
        {
            InitializeComponent();            
            if (CMSXtream.StaticProperty.LoginisAdmin != "1")
            {
                btnAddIntroducer.Visibility = System.Windows.Visibility.Hidden;
            }
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;

            chkActive.IsEnabled = true;
            chkInactive.IsEnabled = true;
            chkInactivePernt.IsEnabled = true;
        }

        const int WM_KEYDOWN = 0x100;
        const int WM_SYSKEYDOWN = 0x0104;

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (txtComment.IsFocused)
            {
                if (msg.message == WM_KEYDOWN || msg.message == WM_SYSKEYDOWN)
                {
                    swf.Keys keyData = ((swf.Keys)((int)((long)msg.wParam))) | swf.Control.ModifierKeys;
                    String inputKeyStroke = String.Empty;
                    if (keyData.ToString() == "D, Control")
                    {
                        int curPos = txtComment.SelectionStart;
                        string replaceDateTime = DateTime.Now.ToString("yy/M/d");
                        txtComment.Text = txtComment.Text.Insert(curPos, replaceDateTime);
                        txtComment.CaretIndex = curPos + replaceDateTime.Length;
                    }
                }
            }
            else
            {
                if (txtNote.IsFocused)
                {
                    if (msg.message == WM_KEYDOWN || msg.message == WM_SYSKEYDOWN)
                    {
                        swf.Keys keyData = ((swf.Keys)((int)((long)msg.wParam))) | swf.Control.ModifierKeys;
                        String inputKeyStroke = String.Empty;
                        if (keyData.ToString() == "D, Control")
                        {
                            int curPos = txtNote.SelectionStart;
                            string replaceDateTime = DateTime.Now.ToString("yy/M/d");
                            txtNote.Text = txtNote.Text.Insert(curPos, replaceDateTime);
                            txtNote.CaretIndex = curPos + replaceDateTime.Length;
                        }
                    }
                }
            }
        }

        public void LoadFormContaint()
        {

            BindReason();
            if (IsAddNew)
            {
                grpBoxStudent.IsEnabled = true;
                btnAddReason.Visibility = System.Windows.Visibility.Hidden;
                cmbInactiveReason.IsEnabled = false;
                lblStudentId.Content = "[New Student]";
                btnDelete.Visibility = System.Windows.Visibility.Hidden;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                grpStudentHistory.Visibility = System.Windows.Visibility.Hidden;
                btnSendSMS.Visibility = System.Windows.Visibility.Hidden;

                btnAddIntroducer.Visibility = System.Windows.Visibility.Hidden;
                btnveiwStudent.Visibility = System.Windows.Visibility.Hidden;

                ClearControll();
                LoadClassCombo();
            }
            else
            {
                if (IsControlDisable)
                {

                    cmbClsGroup.IsEnabled = false;
                    txtInitials.IsEnabled = false;
                    txtSurName.IsEnabled = false;
                    txtFullName.IsEnabled = false;
                    rbtnMale.IsEnabled = false;
                    rbtnFemale.IsEnabled = false;
                    dtpBirthday.IsEnabled = false;
                    dtpJoinDate.IsEnabled = false;
                    txtEmail.IsEnabled = false;
                    txtNICNumber.IsEnabled = false;
                    txtAdders.IsEnabled = false;
                    txtClsFee.IsEnabled = false;
                    txtTelephoneNumber.IsEnabled = false;
                    txtComment.IsEnabled = false;
                    txtNote.IsEnabled = false;
                    chkActive.IsEnabled = false;
                    ActiveStatus.Content = "0";
                    chkInactive.IsEnabled = false;
                    chkInactivePernt.IsEnabled = false;
                }

                grpBoxStudent.IsEnabled = true;
                lblStudentId.Content = stAtt.STD_ID;
                lblDisplayId.Content = stAtt.ATT_ID;
                //btnDelete.Visibility = System.Windows.Visibility.Visible;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                grpStudentHistory.Visibility = System.Windows.Visibility.Visible;

                LoadClassComboEdit(stAtt.CLS_ID, stAtt.CLS_NAME, stAtt.STD_CLASS_FEE);
                cmbClsGroup.SelectedValue = stAtt.CLS_ID;

                lblClassId.Content = stAtt.CLS_ID.ToString();

                txtInitials.Text = stAtt.STD_INITIALS;
                txtSurName.Text = stAtt.STD_SURNAME;
                txtFullName.Text = stAtt.STD_FULL_NAME;
                rbtnMale.IsChecked = stAtt.STD_GENDER == 1 ? true : false;
                rbtnFemale.IsChecked = stAtt.STD_GENDER == 1 ? false : true;
                dtpBirthday.SelectedDate = stAtt.STD_DATEOFBIRTH;
                dtpJoinDate.SelectedDate = stAtt.STD_JOIN_DATE;
                txtEmail.Text = stAtt.STD_EMAIL;
                txtNICNumber.Text = stAtt.STD_NIC;
                txtAdders.Text = stAtt.STD_ADDRESS;
                txtClsFee.Text = stAtt.STD_CLASS_FEE.ToString();
                txtTelephoneNumber.Text = stAtt.STD_TELEPHONE;
                ActiveStatus.Content = stAtt.STD_ACTIVE_FLG.ToString();

                switch (stAtt.STD_ACTIVE_FLG)
                {
                    case 1:
                        chkActive.IsChecked = true;
                        btnAddReason.Visibility = System.Windows.Visibility.Hidden;
                        cmbInactiveReason.IsEnabled = false;
                        if (StaticProperty.SKTIPAddres_2 != "")
                        {
                            chkEnableTosec.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            chkEnableTosec.Visibility = System.Windows.Visibility.Hidden;
                        }
                        break;
                    case 2:
                        chkInactive.IsChecked = true;
                        btnAddReason.Visibility = System.Windows.Visibility.Visible;
                        cmbInactiveReason.IsEnabled = true;
                        chkEnableTosec.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    default:
                        chkInactivePernt.IsChecked = true;
                        btnAddReason.Visibility = System.Windows.Visibility.Visible;
                        chkEnableTosec.Visibility = System.Windows.Visibility.Hidden;
                        cmbInactiveReason.IsEnabled = true;
                        break;
                }

                if (stAtt.RSN_ID != 0 && stAtt.STD_ACTIVE_FLG != 1)
                {
                    cmbInactiveReason.SelectedValue = stAtt.RSN_ID;
                }
                else
                {
                    cmbInactiveReason.SelectedIndex = -1;
                }
                txtComment.Text = stAtt.STD_COMMENT;
                txtNote.Text = stAtt.STD_TEMP_NOTE;
                tabStudentHitory.SelectedIndex = 0;
                BindClassHistoryGrid();
                BindAccosryHistoryGrid();
                BindClassMovement();
                BindClassGroup();
                BindIntroducd();
                BindIntroducer();
                BindUserProfilePicture();
            }

        }

        private void BindIntroducer()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectIntroducer().Tables[0];
                if (table.Rows.Count > 0)
                {
                    lblStudentIntroduced.Content = table.Rows[0]["INTRODUCER_ID"];
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindClassMovement()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectStudentMovement().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdMovement.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdMovement.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindClassGroup()
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectStudentGroup().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdAddClasses.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdAddClasses.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindIntroducd()
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.INTRODUCER_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectIntroduced().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdintroduced.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdintroduced.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindAccosryHistoryGrid()
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectStudentAccesory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdAssosory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdAssosory.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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


        private void LoadClassComboEdit(int id, string name, double amount)
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassGroup(1).Tables[0];
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    ClassGroups.Add(new ClassGroup(Int32.Parse(row["CLS_ID"].ToString()), row["CLS_NAME"].ToString(), double.Parse(row["CLS_FEE"].ToString())));
                }

                ClassGroup result = ClassGroups.Find(x => x.CLS_ID == id);
                if (result == null)
                {
                    ClassGroups.Add(new ClassGroup(id, name, amount));
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
                        txtClsFee.Text = result.CLS_FEE.ToString();
                    }

                    if (stAtt != null)
                    {
                        if (stAtt.CLS_ID != classGroupID)
                        {
                            chkActive.IsChecked = true;
                            ActiveStatus.Content = "1";
                        }
                        else
                        {
                            ActiveStatus.Content = stAtt.STD_ACTIVE_FLG.ToString();
                            switch (stAtt.STD_ACTIVE_FLG)
                            {
                                case 1:
                                    chkActive.IsChecked = true;
                                    break;
                                case 2:
                                    chkInactive.IsChecked = true;
                                    break;
                                default:
                                    chkInactivePernt.IsChecked = true;
                                    break;
                            }
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

        public void BindClassHistoryGrid()
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectClassHistory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdStudentClass.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdStudentClass.ItemsSource = null;
                }

                table = _clsStudent.SelectClassHistory().Tables[1];
                if (table.Rows.Count > 0)
                {
                    txtTelephoneNumber2.Text = table.Rows[0]["STD_TELEPHONE2"].ToString();
                    txtLastWeek.Text = table.Rows[0]["STD_LAST_WEEK"].ToString();
                    if (table.Rows[0]["IS_ADMIN"].ToString() == "1")
                    {
                        chkActive.IsEnabled = false;
                        chkInactive.IsEnabled = false;
                        chkInactivePernt.IsEnabled = false;
                    }
                    else
                    {
                        chkActive.IsEnabled = true;
                        chkInactive.IsEnabled = true;
                        chkInactivePernt.IsEnabled = true;
                    }

                    MakeCheckBoxCheckWithouTrigering(table.Rows[0]["IS_ACTIVE_IN_MACHINE2"].ToString() == "1");
                }
                else
                {
                    txtTelephoneNumber2.Text = "0";
                    txtLastWeek.Text = "";
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void BindClassHistoryGridWhenDelete()
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.SelectClassHistory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdStudentClass.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdStudentClass.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void ClearControll()
        {
            try
            {
                cmbClsGroup.SelectedIndex = -1;
                txtInitials.Text = String.Empty;
                txtSurName.Text = String.Empty;
                txtFullName.Text = String.Empty;
                rbtnMale.IsChecked = false;
                dtpBirthday.SelectedDate = DateTime.Now;
                dtpJoinDate.SelectedDate = DateTime.Now;
                txtEmail.Text = String.Empty;
                txtNICNumber.Text = String.Empty;
                txtAdders.Text = String.Empty;
                txtClsFee.Text = "0.00";
                txtTelephoneNumber.Text = "0";
                txtTelephoneNumber2.Text = "0";
                chkActive.IsChecked = true;
                ActiveStatus.Content = "1";
                txtComment.Text = String.Empty;
                txtNote.Text = String.Empty;
                grdStudentClass.ItemsSource = null;
                txtLastWeek.Text = String.Empty;
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
            if (ValidatetoSave())
            {
                try
                {
                    StudentDA _clsStudent = new StudentDA();
                    if (lblStudentId.Content.ToString() == "[New Student]")
                    {
                        _clsStudent.STD_ID = 0;
                    }
                    else
                    {
                        _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                    }
                    _clsStudent.CLS_ID = int.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.STD_INITIALS = txtInitials.Text.Trim();
                    _clsStudent.STD_SURNAME = txtSurName.Text.Trim();
                    _clsStudent.STD_FULL_NAME = txtFullName.Text.Trim();
                    _clsStudent.STD_GENDER = rbtnMale.IsChecked.Value ? 1 : 0;
                    _clsStudent.STD_DATEOFBIRTH = dtpBirthday.SelectedDate.Value;
                    _clsStudent.STD_JOIN_DATE = dtpJoinDate.SelectedDate.Value;
                    _clsStudent.STD_EMAIL = txtEmail.Text.Trim();
                    _clsStudent.STD_NIC = txtNICNumber.Text.Trim();
                    _clsStudent.STD_ADDRESS = txtAdders.Text.Trim();
                    _clsStudent.STD_CLASS_FEE = double.Parse(txtClsFee.Text.Trim());

                    string telNumber = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                    _clsStudent.STD_TELEPHONE = telNumber;

                    string telNumber2 = txtTelephoneNumber2.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                    if (!(telNumber2.Trim() == String.Empty || telNumber2.Trim() == "0"))
                    {
                        _clsStudent.STD_TELEPHONE2 = telNumber2;
                    }
                    else
                    {
                        _clsStudent.STD_TELEPHONE2 = "0";
                    }

                    _clsStudent.STD_ACTIVE_FLG = chkActive.IsChecked.Value ? 1 : (chkInactive.IsChecked.Value ? 2 : 0);
                    _clsStudent.STD_COMMENT = txtComment.Text.Trim();
                    _clsStudent.STD_TEMP_NOTE = txtNote.Text.Trim();

                    _clsStudent.RSN_ID = 0;
                    if (_clsStudent.STD_ACTIVE_FLG != 1)
                    {
                        if (cmbInactiveReason.SelectedIndex > 0)
                        {
                            _clsStudent.RSN_ID = Int32.Parse(cmbInactiveReason.SelectedValue.ToString());
                        }
                    }
                    _clsStudent.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsStudent.STD_LAST_WEEK = txtLastWeek.Text.Trim() == string.Empty ? 0 : Int32.Parse(txtLastWeek.Text.Trim());
                    System.Data.DataTable table = _clsStudent.InsertStudent().Tables[0];


                    //grpBoxStudent.IsEnabled = false;
                    //btnSave.Visibility = System.Windows.Visibility.Hidden;
                    string returnMsg = table.Rows[0]["RETURN_MSG"].ToString();
                    OutResult = returnMsg;

                    if (lblStudentId.Content.ToString() == "[New Student]")
                    {
                        lblStudentId.Content = table.Rows[0]["STD_ID"].ToString();
                        MessageBox.Show(OutResult, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);

                        //btnAddIntroducer.Visibility = System.Windows.Visibility.Visible;
                        //btnveiwStudent.Visibility = System.Windows.Visibility.Visible;

                    }
                    else
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
        }
        private bool ValidatetoSave()
        {
            try
            {
                if (cmbClsGroup.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a group for this student!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    cmbClsGroup.Focus();
                    return false;
                }
                if (txtClsFee.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Fee is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtClsFee.Focus();
                    return false;
                }
                if (txtInitials.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Frist Name is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtInitials.Focus();
                    return false;
                }
                //if (txtSurName.Text.Trim() == String.Empty)
                //{
                //    MessageBox.Show("Surname  is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                //    txtSurName.Focus();
                //    return false;
                //}
                if (txtFullName.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Full Name  is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtFullName.Focus();
                    return false;
                }

                //rbtnMale , rbtnFemale
                if (dtpBirthday.SelectedDate == null)
                {
                    //MessageBox.Show("Birth day  is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    //dtpBirthday.Focus();
                    //return false;
                    dtpBirthday.SelectedDate = DateTime.Now;
                }

                //if (dtpJoinDate.SelectedDate.Value <= dtpBirthday.SelectedDate.Value)
                //{
                //    MessageBox.Show("Birthday should not be greater than or equal to Join date!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                //    dtpBirthday.Focus();
                //    return false;
                //}


                if (dtpJoinDate.SelectedDate == null)
                {
                    MessageBox.Show("Join date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    dtpJoinDate.Focus();
                    return false;
                }

                if (!(rbtnMale.IsChecked.Value == true || rbtnFemale.IsChecked.Value == true))
                {
                    MessageBox.Show("Gender  is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    rbtnMale.Focus();
                    return false;
                }

                if (txtEmail.Text.Trim() != String.Empty)
                {
                    if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                    {
                        MessageBox.Show("Enter a valid email!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtEmail.Select(0, txtEmail.Text.Length);
                        txtEmail.Focus();
                        return false;
                    }
                }

                //if (txtNICNumber.Text.Trim() == String.Empty)
                //{
                //    MessageBox.Show("NIC number is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                //    txtNICNumber.Focus();
                //    return false;
                //}

                if (txtTelephoneNumber.Text.Trim() == String.Empty || txtTelephoneNumber.Text.Trim() == "(0)-")
                {
                    MessageBox.Show("Telephone number is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtTelephoneNumber.Text = "0";
                    txtTelephoneNumber.Focus();
                    return false;
                }

                string telNumber = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                if (telNumber.Length != 10)
                {
                    MessageBox.Show("Incorrect Telephone number!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtTelephoneNumber.Focus();
                    return false;
                }

                string telNumber2 = "";
                if (!(txtTelephoneNumber.Text.Trim() == String.Empty || txtTelephoneNumber.Text.Trim() == "(0)-"))
                {
                    telNumber2 = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                    if (telNumber2.Length != 10)
                    {
                        MessageBox.Show("Incorrect Second Telephone number!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtTelephoneNumber.Focus();
                        return false;
                    }
                }


                StudentDA _clsStudent = new StudentDA();
                if (lblStudentId.Content.ToString() == "[New Student]")
                {
                    _clsStudent.STD_ID = 0;
                }
                else
                {
                    _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                }
                _clsStudent.STD_NIC = txtNICNumber.Text.Trim();
                _clsStudent.STD_TELEPHONE = telNumber;
                System.Data.DataTable table = _clsStudent.ValidateNicandTeliphone().Tables[0];

                string returnType = table.Rows[0]["RET_TYPE"].ToString();
                string returnMsg = table.Rows[0]["RET_MSG"].ToString();

                if (returnType != "0")
                {
                    if (returnType == "1")
                    {
                        MessageBox.Show(returnMsg, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtNICNumber.Focus();
                        return false;
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show(returnMsg, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.No)
                        {
                            txtTelephoneNumber.Focus();
                            return false;
                        }
                    }
                }

                if (!(Boolean.Parse(chkActive.IsChecked.ToString())))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to save student with inactive status?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        chkActive.Focus();
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // btnEdit.Visibility = System.Windows.Visibility.Hidden;
                // btnSave.Visibility = System.Windows.Visibility.Visible;
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
                MessageBoxResult result = MessageBox.Show("Do you want to delete this student?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                    if (_clsStudent.DeleteStudent() > 0)
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

        private void btnDoPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoPayment form = new DoPayment();
                //form.IsAddNew = true;
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Payment",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 530,
                    Height = 600
                };

                var selectedRow = grdStudentClass.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    form.cmbClsGroup.Visibility = System.Windows.Visibility.Hidden;
                    form.dtpPayMonth.Visibility = System.Windows.Visibility.Visible;
                    form.dtpPayMonth.IsEnabled = false;
                    form.studentId = Int32.Parse(lblStudentId.Content.ToString());
                    form.attendanceId = lblDisplayId.Content.ToString();
                    
                    form.classID = int.Parse(selectedRow["CLS_ID"].ToString());
                    form.paidYear = int.Parse(selectedRow["STM_YEAR"].ToString());
                    form.PaidMonth = int.Parse(selectedRow["STM_MONTH"].ToString());

                    form.dtpPayMonth.SelectedDate = new DateTime(form.paidYear, form.PaidMonth, 1, 0, 0, 0);

                    form.cardDeliverd = selectedRow["CARD_ISSUED_FLG"].ToString() == "1" ? true : false;
                    form.className = selectedRow["CLS_NAME"].ToString();
                    form.yearMonth = selectedRow["STM_YEAR"].ToString() + " " + selectedRow["STM_MONTH_NAME"].ToString();
                    form.classFee = selectedRow["CLASS_FEE"].ToString();
                    DateTime aDate = new DateTime(1, 1, 1, 0, 0, 0);
                    form.clsDate = attDate == aDate ? DateTime.Now : attDate;
                }

                form.MemberActive = chkActive.IsChecked.Value ? 1 : 0;

                form.LoadFormContaint();
                dialog.ShowDialog();
                BindClassHistoryGrid();

                if (ActiveStatus.Content.ToString() != "")
                {
                    if (ActiveStatus.Content.ToString() != "1")
                    {
                        if (form.MemberActive == 1)
                        {
                            ActiveStatus.Content = "1";
                            chkActive.IsChecked = true;                            
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

        private void btnDeletePayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.LoginisAdmin != "1")
                {
                    MessageBox.Show("Sorry bro,You can not delete payment!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.No);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var selectedRow = grdStudentClass.SelectedItem as System.Data.DataRowView;
                        if (selectedRow != null)
                        {
                            double paidAmount = Double.Parse(selectedRow["CLASS_FEE_PAYMENT"].ToString());
                            if (paidAmount>0)
                            {
                                MessageBox.Show("Payment details found. You can not delete this record!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.No);
                            }
                            else
                            {
                                DoPaymentDA _clsPayment = new DoPaymentDA();
                                _clsPayment.PAID_YEAR = Int32.Parse(selectedRow["STM_YEAR"].ToString());
                                _clsPayment.PAID_MONTH = Int32.Parse(selectedRow["STM_MONTH"].ToString());
                                _clsPayment.CLS_ID = Int32.Parse(selectedRow["CLS_ID"].ToString());
                                _clsPayment.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                                _clsPayment.DeleteClassFee();
                                BindClassHistoryGridWhenDelete();
                            }
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
        private void btnDoAssosoryPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoAccesoryPayment form = new DoAccesoryPayment();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Payment for Assosories",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 530,
                    Height = 400
                };

                var selectedRow = grdAssosory.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    form.studentId = Int32.Parse(lblStudentId.Content.ToString());
                    form.accID = Int32.Parse(selectedRow["ACC_ID"].ToString());
                    form.accRecDate = DateTime.Parse(selectedRow["ASS_REC_DATE"].ToString());
                    form.accAmount = double.Parse(selectedRow["ASS_REC_AMOUNT"].ToString());
                }
                form.LoadFormContaint();
                dialog.ShowDialog();
                BindAccosryHistoryGrid();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddNewAssosory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoAccesoryPayment form = new DoAccesoryPayment();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Add New Assosory",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 530,
                    Height = 400
                };

                form.studentId = Int32.Parse(lblStudentId.Content.ToString());
                form.accID = -1;
                DateTime aDate = new DateTime(1, 1, 1, 0, 0, 0);
                form.accRecDate = attDate == aDate ? DateTime.Parse(DateTime.Now.ToShortDateString()) : attDate;

                form.LoadFormContaint();
                dialog.ShowDialog();
                BindAccosryHistoryGrid();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnViewHostory_Click(object sender, RoutedEventArgs e)
        {
            CMSXtream.Pages.View.StudentAttendanceHistory form = new CMSXtream.Pages.View.StudentAttendanceHistory();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Class Attendance History",
                Content = form,
                ResizeMode = ResizeMode.NoResize,
                Width = 375,
                Height = 250
            };
        
            form.studentID = Int32.Parse(lblStudentId.Content.ToString());
            
            form.LoadFormContaint();
            dialog.ShowDialog();
            BindClassHistoryGrid();
        }

        private void btnNextMonthPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoPayment form = new DoPayment();
                //form.IsAddNew = true;
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Next Payment",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 530,
                    Height = 600
                };

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                System.Data.DataTable table = _clsStudent.GetStudentNextPayment().Tables[0];

                System.Data.DataTable table1 = _clsStudent.GetStudentNextPayment().Tables[1];
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                foreach (System.Data.DataRow row in table1.Rows)
                {
                    ClassGroups.Add(new ClassGroup(Int32.Parse(row["CLS_ID"].ToString()), row["CLS_NAME"].ToString(), double.Parse(row["CLS_FEE"].ToString())));
                }
                form.cmbClsGroup.ClearValue(ItemsControl.ItemsSourceProperty);
                form.cmbClsGroup.ItemsSource = ClassGroups;
                form.cmbClsGroup.DisplayMemberPath = "CLS_NAME";
                form.cmbClsGroup.SelectedValuePath = "CLS_ID";

                if (ClassGroups.Count > 0)
                {
                    form.cmbClsGroup.SelectedIndex = 0;
                }

                form.lblClass.Visibility = System.Windows.Visibility.Visible;
                form.dtpPayMonth.Visibility = System.Windows.Visibility.Visible;
                form.dtpPayMonth.IsEnabled = true;
                form.studentId = Int32.Parse(lblStudentId.Content.ToString());
                form.attendanceId = lblDisplayId.Content.ToString();
                form.classID = Int32.Parse(lblClassId.Content.ToString());
                form.paidYear = int.Parse(table.Rows[0]["STM_YEAR"].ToString());
                form.PaidMonth = int.Parse(table.Rows[0]["STM_MONTH"].ToString());

                string payCount = table.Rows[0]["PAYMENT_COUNT"].ToString();

                DateTime dt = new DateTime(form.paidYear, form.PaidMonth, 1, 0, 0, 0);
                form.dtpPayMonth.SelectedDate = dt;
                if (payCount != "0")
                {        
                    form.dtpPayMonth.DisplayDateStart = dt;
                }

                form.cardDeliverd = false;
                form.className = cmbClsGroup.Text;
                form.yearMonth = table.Rows[0]["STM_YEAR"].ToString() + " " + table.Rows[0]["STM_MONTH_NAME"].ToString();
                form.classFee = txtClsFee.Text.Trim();
                DateTime aDate = new DateTime(1, 1, 1, 0, 0, 0);
                form.clsDate = DateTime.Now;

                form.MemberActive = chkActive.IsChecked.Value ? 1 : 0 ;

                form.LoadFormContaintnNextButton();
                dialog.ShowDialog();
                BindClassHistoryGrid();

                if (ActiveStatus.Content.ToString() != "")
                {
                    if (ActiveStatus.Content.ToString() != "1")
                    {
                        if (form.MemberActive == 1)
                        {
                            ActiveStatus.Content = "1";
                            chkActive.IsChecked = true;
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

        private void btnSendSMS_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtTelephoneNumber.Text.Trim() == String.Empty || txtTelephoneNumber.Text.Trim() == "(0)-")
                {
                    MessageBox.Show("Telephone number is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtTelephoneNumber.Text = "0";
                    txtTelephoneNumber.Focus();
                    return;
                }

                string telNumber = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                if (telNumber.Length != 10)
                {
                    MessageBox.Show("Incorrect Telephone number!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtTelephoneNumber.Focus();
                    return;
                }

                Int32 studentID = Int32.Parse(lblStudentId.Content.ToString());
                string Telephone = txtTelephoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                string StudentName = txtInitials.Text.ToString();
                string className = cmbClsGroup.Text.ToString();
                Int32 classID = int.Parse(cmbClsGroup.SelectedValue.ToString());
                string mcID = lblDisplayId.Content.ToString(); ;

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("STD_ID", typeof(Int32));
                table.Columns.Add("CLS_ID", typeof(Int32));
                table.Columns.Add("STD_INITIALS", typeof(string));
                table.Columns.Add("CLS_NAME", typeof(string));
                table.Columns.Add("STD_TELEPHONE", typeof(string));
                table.Columns.Add("ATT_ID", typeof(string));
                table.Rows.Add(studentID, classID, StudentName, className, Telephone, mcID);

                CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Sent SMS to Member " + StudentName,
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1100,
                    Height = 680
                };
                form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                form.callingForm = true;
                form.filteredTable = table;
                form.BindStudentGrid();

                form.txtEmpNumber.Text = mcID;
                form.chkAll.IsChecked = false;
                form.LoadSMSHistory();

                dialog.ShowDialog();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddClasses_Click(object sender, RoutedEventArgs e)
        {
            CMSXtream.Pages.DataEntry.AddClassGroup form = new CMSXtream.Pages.DataEntry.AddClassGroup();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Add Classes",
                Content = form,
                ResizeMode = ResizeMode.NoResize,
                Width = 400,
                Height = 200
            };
            form.stdID = Int32.Parse(lblStudentId.Content.ToString());
            form.clsID = int.Parse(cmbClsGroup.SelectedValue.ToString());
            form.LoadClassCombo();
            dialog.ShowDialog();
            BindClassGroup();
        }

        private void btnEdit_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {

                var selectedRow = grdAddClasses.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    CMSXtream.Pages.DataEntry.AddClassGroup form = new CMSXtream.Pages.DataEntry.AddClassGroup();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Add Classes",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 400,
                        Height = 200
                    };
                    form.stdID = Int32.Parse(lblStudentId.Content.ToString());
                    form.clsID = int.Parse(cmbClsGroup.SelectedValue.ToString());
                    form.LoadClassCombo();
                    form.cmbClsGroup.SelectedValue = Int32.Parse(selectedRow["CLS_ID"].ToString());
                    form.cmbClsGroup.IsEnabled = false;
                    form.txtClsFee.Text = selectedRow["STD_CLASS_FEE"].ToString();

                    dialog.ShowDialog();
                    BindClassGroup();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = grdAddClasses.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                _clsStudent.CLS_ID = Int32.Parse(selectedRow["CLS_ID"].ToString());
                _clsStudent.DeleteStudentGroup();
                BindClassGroup();
            }
        }

        private void chkActive_Click(object sender, RoutedEventArgs e)
        {
            if (chkActive.IsChecked.Value)
            {
                if (cmbInactiveReason != null)
                {
                    btnAddReason.Visibility = System.Windows.Visibility.Hidden;
                    cmbInactiveReason.IsEnabled = false;
                    cmbInactiveReason.SelectedIndex = -1;
                } 

                if (ActiveStatus.Content.ToString() == "0")
                {
                    ActiveChecked();
                    if (StaticProperty.SKTIPAddres_2 != "")
                    {
                        chkEnableTosec.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        chkEnableTosec.Visibility = System.Windows.Visibility.Hidden;
                    }
                    chkActive.IsEnabled = false;
                    chkInactive.IsEnabled = false;
                    chkInactivePernt.IsEnabled = false;
                }
            }        
        }

        private void chkInactivePernt_Click(object sender, RoutedEventArgs e)
        {
            if (chkInactivePernt.IsChecked.Value)
            {
                if (cmbInactiveReason != null)
                {
                    btnAddReason.Visibility = System.Windows.Visibility.Visible;
                    cmbInactiveReason.IsEnabled = true;
                }
                if (ActiveStatus.Content.ToString() == "1")
                {
                    ActiveUnchecked();
                    chkActive.IsEnabled = false;
                    chkInactive.IsEnabled = false;
                    chkInactivePernt.IsEnabled = false;
                    chkEnableTosec.Visibility = Visibility.Hidden;
                }
            }
        }

        private void chkInactive_Click(object sender, RoutedEventArgs e)
        {
            if (chkInactive.IsChecked.Value)
            {
                if (cmbInactiveReason != null)
                {
                    btnAddReason.Visibility = System.Windows.Visibility.Visible;
                    cmbInactiveReason.IsEnabled = true;
                }
                if (ActiveStatus.Content.ToString() == "1")
                {
                    ActiveUnchecked();
                    chkActive.IsEnabled = false;
                    chkInactive.IsEnabled = false;
                    chkInactivePernt.IsEnabled = false;
                    chkEnableTosec.Visibility = Visibility.Hidden;
                }
            }
        } 

        private void ActiveChecked()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                if (ActiveStatus.Content.ToString() != "")
                {
                    if (ActiveStatus.Content.ToString() != "1")
                    {
                        if (StaticProperty.SKTIPAddres != "")
                        {
                            if (StaticProperty.SDK.GetConnectState())
                            {
                                int rec = 0;
                                string sUserID = lblDisplayId.Content.ToString();

                                SMSDA _obj = new SMSDA();
                                DataSet usersPF = _obj.GetUsersFP(sUserID);

                                rec = StaticProperty.SDK.sta_SetUserFPInfo(sUserID, usersPF);
                                if (rec != 1)
                                {
                                    MessageBox.Show("Unable to active member from device.Fp, PW or Card not found", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                    btnSave.Visibility = System.Windows.Visibility.Hidden;
                                }
                                else
                                {
                                    ActiveStatus.Content = "1";
                                    OutResult = "Record has been updated";
                                    InsertToDevice2();
                                    MakeCheckBoxCheckWithouTrigering(true);
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
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


        private void MakeCheckBoxCheckWithouTrigering(Boolean Value)
        {
            // Unsubscribe event temporarily
            chkEnableTosec.Checked -= chkEnableTosec_Checked;
            chkEnableTosec.Unchecked -= chkEnableTosec_Checked;

            // Change the value programmatically
            chkEnableTosec.IsChecked = Value; // or false

            // Re-subscribe the event
            chkEnableTosec.Checked += chkEnableTosec_Checked;
            chkEnableTosec.Unchecked += chkEnableTosec_Unchecked;
        }

        private void ActiveUnchecked()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (ActiveStatus.Content.ToString() != "" & ActiveStatus.Content.ToString() == "1")
                {
                    if (StaticProperty.SKTIPAddres != "")
                    {
                        if (StaticProperty.SDK.GetConnectState())
                        {
                            int rec = 0;
                            string sUserID = lblDisplayId.Content.ToString();
                            rec = StaticProperty.SDK.sta_DelUserTmp(sUserID);
                            if (rec != 1)
                            {
                                MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                btnSave.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                ActiveStatus.Content = "0";
                                OutResult = "Record has been updated";
                                DeleteFromDivice2();
                                MakeCheckBoxCheckWithouTrigering(false);
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
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void InsertToDevice2()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (StaticProperty.SKTIPAddres_2 != "")
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        int rec = 0;
                        string sUserID = lblDisplayId.Content.ToString();

                        SMSDA _obj = new SMSDA();
                        DataSet usersPF = _obj.GetUsersFP(sUserID);
                        string strCardno = usersPF.Tables[1].Rows[0]["CAREDNUMBER"].ToString();
                        string strPassword = usersPF.Tables[1].Rows[0]["USER_PASSWORD"].ToString();

                        if (usersPF.Tables[0].Rows.Count == 0 && strCardno == "0" && strPassword == "")
                        {
                            StaticProperty.SDK.sta_UpdateFingerFrintFromPrimary(sUserID);
                            usersPF = _obj.GetUsersFP(sUserID);
                        }

                        rec = StaticProperty.SDK_2.sta_SetUserFPInfo(sUserID, usersPF);
                        if (rec < 1)
                        {
                            MessageBox.Show("Unable to Insert date to the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                        }
                        else
                        {
                            OutResult = "Record has been updated";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void DeleteFromDivice2()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                if (StaticProperty.SKTIPAddres_2 != "")
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        int rec = 0;
                        string sUserID = lblDisplayId.Content.ToString();
                        rec = StaticProperty.SDK_2.sta_DelUserTmp(sUserID);
                        if (rec != 1)
                        {
                            MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                            //btnSave.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            OutResult = "Record has been updated";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Attendance Machine 2 no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                        //btnSave.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private Boolean UpdateBiometricDevice( string sUserID ,Boolean status)
        {
            Boolean retValue = false;
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    int rec = 0;
                  
                        rec = StaticProperty.SDK.sta_SetUserInfo(sUserID, "", "", 0, status);

                        if (rec != 1)
                        {
                            MessageBox.Show("Unable to Update date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                        }
                        else
                        {
                            string msg = "";
                            if (status)
                            {
                                msg = "Member Activated!";
                            }
                            else
                            {
                                msg = "Member Inactivated!";
                            }
                            MessageBox.Show(msg, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            return true;
                        }
                }
                else
                {
                    MessageBox.Show("Attendace Machin not conncted.Please restart service", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                }
                
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            return retValue;
        }

        private void btnAddReason_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.AddReson form = new CMSXtream.Pages.DataEntry.AddReson();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Define Reasons for Inactivation ",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 400,
                    Height = 400
                };
                dialog.ShowDialog();
                BindReason();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindReason()
        {
            try
            {
                ResonDA _clsReason = new ResonDA();
                System.Data.DataTable table = _clsReason.SelectReason().Tables[0];
                cmbInactiveReason.DisplayMemberPath = "RSN_DES";
                cmbInactiveReason.SelectedValuePath = "RSN_ID";
                cmbInactiveReason.ItemsSource = table.DefaultView;

                if (table.Rows.Count > 0)
                {
                    cmbInactiveReason.ItemsSource = table.DefaultView;
                }
                else
                {
                    cmbInactiveReason.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddIntroducer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery();
                form.calltogetStdID = 1;
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Add Member",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 950
                };
                //form.classDate = dtpHoldDate.SelectedDate.Value;
                //form.selectedClass = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                form.ShowAddStudent();
                dialog.ShowDialog();

                if (form.STD_ID > 0)
                {
                    StudentDA studentDaClas = new StudentDA();
                    studentDaClas.STD_ID = Int32.Parse(lblStudentId.Content.ToString());
                    studentDaClas.INTRODUCER_ID = form.STD_ID;

                    if (studentDaClas.STD_ID != studentDaClas.INTRODUCER_ID)
                    {
                        if (studentDaClas.AddStudentToIntroducer() > 0)
                        {
                            lblStudentIntroduced.Content = form.STD_ID;
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

        private void btnveiwStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Introducer Info of " + lblStudentId.Content.ToString(),
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1100
                };

                StudentDA _clsStudent = new StudentDA();
                if (lblStudentIntroduced.Content.ToString() != "[Introduced Student]")
                {
                    _clsStudent.STD_ID = Int32.Parse(lblStudentIntroduced.Content.ToString());
                }
                else
                {
                    _clsStudent.STD_ID = 0;
                }

                StudentAttribute stAttPass = new StudentAttribute();
                System.Data.DataSet ds = _clsStudent.SelectStudentDetails();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    stAttPass.STD_ID = int.Parse(ds.Tables[0].Rows[0]["STD_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(ds.Tables[0].Rows[0]["CLS_ID"].ToString());
                    stAttPass.CLS_NAME = ds.Tables[0].Rows[0]["CLS_NAME"].ToString();
                    stAttPass.STD_INITIALS = ds.Tables[0].Rows[0]["STD_INITIALS"].ToString();
                    stAttPass.STD_SURNAME = ds.Tables[0].Rows[0]["STD_SURNAME"].ToString();
                    stAttPass.STD_FULL_NAME = ds.Tables[0].Rows[0]["STD_FULL_NAME"].ToString();
                    stAttPass.STD_GENDER = int.Parse(ds.Tables[0].Rows[0]["STD_GENDER"].ToString());
                    stAttPass.STD_DATEOFBIRTH = DateTime.Parse(ds.Tables[0].Rows[0]["STD_DATEOFBIRTH"].ToString());
                    stAttPass.STD_JOIN_DATE = DateTime.Parse(ds.Tables[0].Rows[0]["STD_JOIN_DATE"].ToString());
                    stAttPass.STD_EMAIL = ds.Tables[0].Rows[0]["STD_EMAIL"].ToString();
                    stAttPass.STD_NIC = ds.Tables[0].Rows[0]["STD_NIC"].ToString();
                    stAttPass.STD_ADDRESS = ds.Tables[0].Rows[0]["STD_ADDRESS"].ToString();
                    stAttPass.STD_CLASS_FEE = Double.Parse(ds.Tables[0].Rows[0]["STD_CLASS_FEE"].ToString());
                    stAttPass.STD_TELEPHONE = ds.Tables[0].Rows[0]["STD_TELEPHONE"].ToString();
                    stAttPass.STD_ACTIVE_FLG = ds.Tables[0].Rows[0]["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(ds.Tables[0].Rows[0]["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = ds.Tables[0].Rows[0]["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = ds.Tables[0].Rows[0]["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = int.Parse(ds.Tables[0].Rows[0]["RSN_ID"].ToString());

                    form.IsAddNew = false;
                    form.stAtt = stAttPass;
                    form.LoadFormContaint();

                    form.grpBoxStudent.BorderBrush = new SolidColorBrush(Colors.Red);

                    dialog.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindUserProfilePicture()
        {
            try
            {
                string StudentNumber = lblDisplayId.Content.ToString();
                string imagePath = null;
                Boolean FoundImge  = false;

                string[] extension = { ".jpg", ".png", ".jpeg" };
                foreach (string ext in extension)
                {
                    imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProfilePictures", StudentNumber + ext);

                    if (File.Exists(imagePath))
                    {
                        FoundImge = true;
                        break;
                    }
                    imagePath = "";
                }

                if (FoundImge)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.DecodePixelWidth = 200;
                    bitmap.DecodePixelHeight = 200;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgProfilePic.Source = bitmap;
                    imgProfilePic.Tag = "Load";
                }
                else
                {
                    imgProfilePic.Tag = null;
                    if (rbtnMale.IsChecked.Value)
                    {
                        imgProfilePic.Source = new BitmapImage(new Uri("pack://application:,,,/ProfilePictures/DefaultProfileM.jpg"));
                    }
                    else
                    {
                        imgProfilePic.Source = new BitmapImage(new Uri("pack://application:,,,/ProfilePictures/DefaultProfileF.jpg"));
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred. Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void rbtnMale_Checked(object sender, RoutedEventArgs e)
        {
            if (rbtnMale.IsChecked.Value && imgProfilePic.Tag==null)
            {
                BindUserProfilePicture();
            }
        }

        private void rbtnFemale_Checked(object sender, RoutedEventArgs e)
        {
            if (rbtnFemale.IsChecked.Value && imgProfilePic.Tag == null)
            {
                BindUserProfilePicture();
            }
        }

        private void chkEnableTosec_Checked(object sender, RoutedEventArgs e)
        {
            if (chkEnableTosec.IsChecked.Value)
            {
                InsertToDevice2();
            }
        }

        private void chkEnableTosec_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!chkEnableTosec.IsChecked.Value)
            {
                DeleteFromDivice2();
            }
        }

        private void btnPrintrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.View.FingerPrintSummary form = new CMSXtream.Pages.View.FingerPrintSummary();
                form.isActive = ActiveStatus.Content.ToString() == "1";
                form.BADGENUMBER = lblDisplayId.Content.ToString();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Fringer Print Summary",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 800
                };                
                form.ShowAddData();

                dialog.ShowDialog();

                if(form.isChanged)
                {
                    chkActive.IsEnabled = false;
                    chkInactive.IsEnabled = false;
                    chkInactivePernt.IsEnabled = false;
                    chkEnableTosec.IsEnabled = false;
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

    public class DoubleToVisaulConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double accessoryAmount = Double.Parse(value.ToString());
            return accessoryAmount > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }

    public class TelephoneNumberConvetor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            //retrieve only numbers in case we are dealing with already formatted phone no
            string phoneNo = value.ToString().Replace("(", string.Empty).Replace(")", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

            switch (phoneNo.Length)
            {
                case 9:
                    return Regex.Replace(phoneNo, @"(\d{3})(\d{3})(\d{3})", "( )-$1-$2-$3");
                case 10:
                    return Regex.Replace(phoneNo, @"(\d{1})(\d{3})(\d{3})(\d{3})", "($1)-$2-$3-$4");
                default:
                    return phoneNo;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
