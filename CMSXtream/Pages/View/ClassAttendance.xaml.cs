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
using System.Windows.Interop;
using swf = System.Windows.Forms;
using System.Data;
using System.Windows.Threading;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for ClassAttendance.xaml
    /// </summary>
    public partial class ClassAttendance : UserControl
    {
        public Int32 isClassHeld = 0;
        public Int32 lastLoadClassGroup = -1;

        public DispatcherTimer timer;
        private bool isTransferRunning = false;

        public Int32 machinTimePrevious = DateTime.Now.Minute;
        public DateTime PreviousDate;
        public DataTable memberLst;
        public ClassAttendance()
        {
            InitializeComponent();
            dtpHoldDate.DisplayDateEnd = DateTime.Today;
            PreviousDate = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
            lblAttendanceCount.Content = "0";

            BindStudentGrid();
            FristTimeBindStudentGrid();
        }

        private void FristTimeBindStudentGrid()
        {
            try
            {
                if (StaticProperty.SKTIPAddres == "")
                {
                    lblError.Visibility = System.Windows.Visibility.Hidden;
                    pbStatus.Visibility = System.Windows.Visibility.Hidden;
                }

                if (StaticProperty.SKTIPAddres_2 == "")
                {
                    lblError2.Visibility = System.Windows.Visibility.Hidden;
                    pbStatus2.Visibility = System.Windows.Visibility.Hidden;
                }

                CheckAttStatus();
                CheckAttStatus2();

                Int32 TimeInterval = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("RefershTimeInterval").ToString());

                timer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = new TimeSpan(0, 0, TimeInterval)
                };
                timer.Tick += Timer_Tick;
                timer.Start();
                /*
                timer = new DispatcherTimer(new TimeSpan(0, 0, TimeInterval), DispatcherPriority.Normal, async delegate
                {
                    if (isTransferRunning)
                        return;

                    string datetime = dtpHoldDate.SelectedDate?.ToString();
                    if (string.IsNullOrEmpty(datetime))
                        return;

                    isTransferRunning = true;
                    try
                    {
                        await Task.Run(() =>
                        {
                            BindStudentGrid_test(datetime);
                        });
                    }
                    finally
                    {
                        isTransferRunning = false;
                    }

                    if (memberLst.Rows.Count != grdStudentAttendance.Items.Count)
                    {
                        grdStudentAttendance.ItemsSource = memberLst.DefaultView;
                    }

                    CheckAttStatus();
                    CheckAttStatus2();
                }, this.Dispatcher);
                */

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {

            if (isTransferRunning)
                return;

            string datetime = dtpHoldDate.SelectedDate?.ToString();
            if (string.IsNullOrEmpty(datetime))
                return;

            isTransferRunning = true;
            try
            {
                await Task.Run(() =>
                {
                    BindStudentGrid_test(datetime);
                });
            }
            finally
            {
                isTransferRunning = false;
            }

            if (memberLst.Rows.Count != grdStudentAttendance.Items.Count)
            {
                grdStudentAttendance.ItemsSource = memberLst.DefaultView;
            }
            CheckAttStatus();
            CheckAttStatus2();
        }

        //public void StartTimer()
        //{
        //    System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        //    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        //    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        //    dispatcherTimer.Start();
        //}
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    txtComment.Text = DateTime.Now.ToString("HH:mmTongue Tieds");
        //}

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
        }


        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TS_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TS.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scrollviewer.LineUp();
            }
            else
            {
                scrollviewer.LineDown();
            }
            e.Handled = true;
        }

        public void SetClassHeldFlag()
        {
            try
            {
                ClassAttendanceDA _clsClass = new ClassAttendanceDA();
                _clsClass.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                _clsClass.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                System.Data.DataTable table = _clsClass.CheckClassHeldFlag().Tables[0];
                isClassHeld = 0;
                if (table.Rows.Count > 0)
                {
                    isClassHeld = 1;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void LoadClasess()
        {
            try
            {
                Int32 clsFilterID = 0;

                if (chkShowInactive.IsChecked.Value)
                {
                    clsFilterID = 1;
                }

                Int32 day = -1;
                if (!(chkSelectedDay.IsChecked.Value))
                {
                    day = (Int32)dtpHoldDate.SelectedDate.Value.DayOfWeek;
                    if (day == 0)
                    {
                        day = 7;
                    }
                }

                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassForStudent(clsFilterID, day).Tables[0];
                cmbClsGroup.ItemsSource = table.DefaultView;
                cmbClsGroup.DisplayMemberPath = "CLS_NAME";
                cmbClsGroup.SelectedValuePath = "CLS_ID";

                if (lastLoadClassGroup != -1)
                {
                    cmbClsGroup.SelectedValue = lastLoadClassGroup;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }
        public void LoadPart()
        {
            try
            {
                List<PartClass> PartClass = new List<PartClass>();
                PartClass.Add(new PartClass(1, "Part 01"));
                PartClass.Add(new PartClass(2, "Part 02"));
                PartClass.Add(new PartClass(3, "Part 03"));
                PartClass.Add(new PartClass(4, "Part 04"));
                PartClass.Add(new PartClass(5, "Part 05"));
                PartClass.Add(new PartClass(6, "Part 06"));
                PartClass.Add(new PartClass(7, "Part 07"));
                PartClass.Add(new PartClass(8, "Part 08"));
                PartClass.Add(new PartClass(9, "Part 09"));
                PartClass.Add(new PartClass(10, "Part 10"));
                cmbPart.ItemsSource = PartClass;
                cmbPart.DisplayMemberPath = "PART_NAME";
                cmbPart.SelectedValuePath = "PART_ID";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }
        public void LoadWeek()
        {
            try
            {
                if (cmbClsGroup.SelectedIndex != -1 & cmbClsGroup.SelectedValue != null)
                {
                    ClassAttendanceDA _clsClass = new ClassAttendanceDA();
                    _clsClass.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                    _clsClass.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    System.Data.DataTable table = _clsClass.SelectNextWeek().Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        txtWeekNumber.Text = table.Rows[0]["CLS_WEEK"].ToString();
                        cmbPart.SelectedValue = table.Rows[0]["CLS_PART"].ToString();
                        txtComment.Text = table.Rows[0]["CLS_COMMENT"].ToString();
                    }
                    else
                    {
                        txtWeekNumber.Text = "0";
                        cmbPart.SelectedValue = "1";
                        txtComment.Text = "";
                    }
                }
                else
                {
                    txtWeekNumber.Text = "";
                    cmbPart.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void btnSaveClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }
                if (txtWeekNumber.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Please define a week!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_COMMENT = txtComment.Text;
                Boolean iscancelCall = true;
                if (!(chkClassHoldFlg.IsChecked.Value))
                {
                    _clsStudent.CLS_WEEK = 0;
                    _clsStudent.CLS_PART = 0;
                    MessageBoxResult resultMessageBox = MessageBox.Show("Are you sure that class is not held?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultMessageBox != MessageBoxResult.Yes)
                    {
                        iscancelCall = false;
                    }
                }
                else
                {
                    _clsStudent.CLS_WEEK = Int32.Parse(txtWeekNumber.Text.Trim());
                }

                if (iscancelCall)
                {
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                    _clsStudent.CLS_HOLD_FLG = chkClassHoldFlg.IsChecked.Value ? 1 : 0;
                    if (cmbPart.SelectedValue == null)
                    {
                        _clsStudent.CLS_PART = 1;
                    }
                    else
                    {
                        _clsStudent.CLS_PART = Int32.Parse(cmbPart.SelectedValue.ToString());
                    }
                    if (_clsStudent.InsertClassAttendance() > 0)
                    {
                        MessageBox.Show("Record has been successfully saved!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtWeekNumber.IsEnabled = false;
                        cmbPart.IsEnabled = false;
                        //btnAddStudent.IsEnabled = true;
                        //btnAddTute.IsEnabled = true;
                        BindStudentGrid();

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

        private void BindStudentGrid()
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_ID = 1;
                _clsStudent.CLS_HOLD_FLG = 1;
                _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());

                if (!chkStopRefresh.IsChecked.Value || PreviousDate != _clsStudent.CLS_REC_DATE)
                {

                    DataTable table = _clsStudent.SelectClassAttendance().Tables[0];
                    Int32 recordCount = table.Rows.Count;
                    if (PreviousDate != _clsStudent.CLS_REC_DATE || Int32.Parse(lblAttendanceCount.Content.ToString()) != recordCount)
                    {
                        lblAttendanceCount.Content = recordCount.ToString();
                        PreviousDate = _clsStudent.CLS_REC_DATE;

                        string expression = "STD_PAID_STATUS =1";
                        DataRow[] selectedRows = table.Select(expression);
                        lblCardCount.Content = selectedRows.Length.ToString();
                        memberLst = table;
                        if (table.Rows.Count > 0)
                        {
                            grdStudentAttendance.ItemsSource = table.DefaultView;
                        }
                        else
                        {
                            grdStudentAttendance.ItemsSource = null;
                        }
                    }
                }

                if (StaticProperty.SKTIPAddres != "")
                {
                    Int32 machinTime = DateTime.Now.Minute;
                    if (machinTime != machinTimePrevious)
                    {
                        machinTimePrevious = machinTime;
                        CheckAttStatus();
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
        private void BindStudentGrid_test(string selecteddate)
        {
            try
            {

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_ID = 1;
                _clsStudent.CLS_HOLD_FLG = 1;
                _clsStudent.CLS_REC_DATE = DateTime.Parse(selecteddate);

                DataTable table = _clsStudent.SelectClassAttendance().Tables[0];

                string expression = "STD_PAID_STATUS =1";
                DataRow[] selectedRows = table.Select(expression);
                memberLst = table;

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        
        private void BindHoldFlag()
        {
            try
            {
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());

                    string clsHoldFlag = _clsStudent.SelectClassHoldFlg();
                    if (clsHoldFlag == "0")
                    {
                        chkClassHoldFlg.IsChecked = false;
                        //btnAddStudent.IsEnabled = false;
                        //btnAddTute.IsEnabled = false;
                    }
                    else
                    {
                        chkClassHoldFlg.IsChecked = true;
                        //btnAddStudent.IsEnabled = true;
                        //btnAddTute.IsEnabled = true;
                    }
                }
                else
                {
                    chkClassHoldFlg.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                var result = (e.OriginalSource as CheckBox).IsChecked;

                StudentDA _clsStudent = new StudentDA();
                //var result = (sender as CheckBox).IsChecked;
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {

                    _clsStudent.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.STD_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                    _clsStudent.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsStudent.CLS_REC_ATT_FLG = 1;
                    _clsStudent.UpdateStudentAttendence();
                    setAttendanceInfo();

                    //New student 
                    if (selectedRow["CLASS_FEE"].ToString() == "")
                    {
                        BindStudentGrid();
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

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = (e.OriginalSource as CheckBox).IsChecked;
                MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to mark this member as absent?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultMessageBox == MessageBoxResult.Yes)
                {
                    StudentDA _clsStudent = new StudentDA();
                    //var result = (sender as CheckBox).IsChecked;
                    var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        _clsStudent.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                        _clsStudent.STD_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                        _clsStudent.MODIFY_USER = StaticProperty.LoginUserID;
                        _clsStudent.CLS_REC_ATT_FLG = 0;
                        _clsStudent.UpdateStudentAttendence();
                        setAttendanceInfo();
                    }
                }
                else
                {
                    //(sender as CheckBox).IsChecked = true;
                    (e.OriginalSource as CheckBox).IsChecked = true;
                }
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
                if (cmbClsGroup.IsDropDownOpen)
                {
                    lastLoadClassGroup = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    BindStudentGrid();
                    BindHoldFlag();
                    LoadWeek();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpHoldDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbClsGroup != null)
                {

                    string selectedValue = "";
                    if (cmbClsGroup.SelectedValue != null)
                    {
                        selectedValue = cmbClsGroup.SelectedValue.ToString();
                    }

                    if (DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToShortDateString()) > DateTime.Parse(DateTime.Now.ToShortDateString()))
                    {
                        btnAddStudent.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        btnAddStudent.Visibility = Visibility.Visible;
                    }

                    //cmbClsGroup.SelectedIndex = -1;
                    if (!(chkSelectedDay.IsChecked.Value))
                    {
                        LoadClasess();
                        if (selectedValue != "")
                        {
                            cmbClsGroup.SelectedValue = selectedValue;
                        }
                    }
                    BindStudentGrid();
                    BindHoldFlag();
                    LoadWeek();
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
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Member Info",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1100
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    stAttPass.ATT_ID = int.Parse(selectedRow["ATT_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                    stAttPass.CLS_NAME = "Defualt";
                    stAttPass.STD_INITIALS = selectedRow["STD_INITIALS"].ToString();
                    stAttPass.STD_SURNAME = selectedRow["STD_SURNAME"].ToString();
                    stAttPass.STD_FULL_NAME = selectedRow["STD_FULL_NAME"].ToString();
                    stAttPass.STD_GENDER = int.Parse(selectedRow["STD_GENDER"].ToString());
                    stAttPass.STD_DATEOFBIRTH = DateTime.Parse(selectedRow["STD_DATEOFBIRTH"].ToString());
                    stAttPass.STD_JOIN_DATE = DateTime.Parse(selectedRow["STD_JOIN_DATE"].ToString());
                    stAttPass.STD_EMAIL = selectedRow["STD_EMAIL"].ToString();
                    stAttPass.STD_NIC = selectedRow["STD_NIC"].ToString();
                    stAttPass.STD_ADDRESS = selectedRow["STD_ADDRESS"].ToString();
                    stAttPass.STD_CLASS_FEE = Double.Parse(selectedRow["STD_CLASS_FEE"].ToString());
                    stAttPass.STD_TELEPHONE = selectedRow["STD_TELEPHONE"].ToString();
                    stAttPass.STD_ACTIVE_FLG = int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = int.Parse(selectedRow["RSN_ID"].ToString());
                }
                form.IsAddNew = false;
                //form.IsControlDisable = true;
                form.stAtt = stAttPass;
                form.attDate = dtpHoldDate.SelectedDate.Value;
                form.LoadFormContaint();
                form.btnSave.Visibility = System.Windows.Visibility.Visible;
                form.btnDelete.Visibility = System.Windows.Visibility.Hidden;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                dialog.ShowDialog();
                btnRefresh_Click(null, null);
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkAelectedDay_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkAelectedDay_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkClassHoldFlg_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    chkClassHoldFlg.IsChecked = !(chkClassHoldFlg.IsChecked.Value);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                // HomeDA _clsStudent = new HomeDA();
                //MessageBox.Show("Records has been successfully sync!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Add Member Attendance",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 950
                };
                form.classDate = dtpHoldDate.SelectedDate.Value;
                form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                form.ShowAddStudent();
                dialog.ShowDialog();
                BindStudentGrid();
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddTute_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClsGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a class to distribute accessory !", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }

            ClassAttendanceDA clsAtt = new ClassAttendanceDA();
            clsAtt.CLS_REC_DATE = dtpHoldDate.SelectedDate.Value;
            clsAtt.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
            if (clsAtt.GetClassAttendanceCount()[0] == 0)
            {
                MessageBox.Show("Nobody is in the class room. You cannot distribute accessory.!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }

            CMSXtream.Pages.View.AddTute form = new CMSXtream.Pages.View.AddTute();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Accessory Distribution",
                Content = form,
                ResizeMode = ResizeMode.NoResize,
                Width = 500
            };

            form.classID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
            form.classDate = dtpHoldDate.SelectedDate.Value;
            form.LoadFormContaint();
            dialog.ShowDialog();
            BindStudentGrid();
        }

        private void btnAddHistory_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClsGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }

            CMSXtream.Pages.View.ClassHistory form = new CMSXtream.Pages.View.ClassHistory();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Class History",
                Content = form,
                ResizeMode = ResizeMode.NoResize,
                Width = 500
            };

            form.classID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
            form.LoadFormContaint();
            dialog.ShowDialog();

        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtWeekNumber.IsEnabled = true;
            cmbPart.IsEnabled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 inactiveFlg = 0;
                MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to remove this member from attendance?", "Attendance Removal Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                inactiveFlg = 0;
                if (resultMessageBox == MessageBoxResult.Yes)
                {
                    ClassAttendanceDA _clsAttendance = new ClassAttendanceDA();
                    var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        _clsAttendance.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        _clsAttendance.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                        _clsAttendance.CLS_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                        _clsAttendance.IS_INACTIVE_STD = inactiveFlg;
                        _clsAttendance.RemoveStudentFromClass();
                        if (inactiveFlg == 1)
                        {
                            MessageBox.Show("Member has been removed from attendance and Inactivated!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        }
                        BindStudentGrid();
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void setAttendanceInfo()
        {
            try
            {
                Int32[] attCount = { 0, 0, 0, 0 };
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    ClassAttendanceDA clsAtt = new ClassAttendanceDA();
                    clsAtt.CLS_REC_DATE = dtpHoldDate.SelectedDate.Value;
                    clsAtt.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    attCount = clsAtt.GetClassAttendanceCount();
                }
                lblAttendanceCount.Content = attCount[0].ToString() + " / " + grdStudentAttendance.Items.Count.ToString();
                lblCardCount.Content = attCount[1].ToString();
                string tooltip = attCount.ToString() + " student(s) came from " + grdStudentAttendance.Items.Count.ToString() + " student(s)";
                lblAttendanceCount.ToolTip = tooltip;
                lblAttendanceImage.ToolTip = tooltip;

                //lblTempInactive.Content = attCount[3].ToString();
                //lblPermInactive.Content = attCount[2].ToString();

                btnTempInactive.Content = attCount[3].ToString();
                btnPermInactive.Content = attCount[2].ToString();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Member Info",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1100
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                    stAttPass.CLS_NAME = selectedRow["CLS_NAME"].ToString();
                    stAttPass.STD_INITIALS = selectedRow["STD_INITIALS"].ToString();
                    stAttPass.STD_SURNAME = selectedRow["STD_SURNAME"].ToString();
                    stAttPass.STD_FULL_NAME = selectedRow["STD_FULL_NAME"].ToString();
                    stAttPass.STD_GENDER = int.Parse(selectedRow["STD_GENDER"].ToString());
                    stAttPass.STD_DATEOFBIRTH = DateTime.Parse(selectedRow["STD_DATEOFBIRTH"].ToString());
                    stAttPass.STD_JOIN_DATE = DateTime.Parse(selectedRow["STD_JOIN_DATE"].ToString());
                    stAttPass.STD_EMAIL = selectedRow["STD_EMAIL"].ToString();
                    stAttPass.STD_NIC = selectedRow["STD_NIC"].ToString();
                    stAttPass.STD_ADDRESS = selectedRow["STD_ADDRESS"].ToString();
                    stAttPass.STD_CLASS_FEE = Double.Parse(selectedRow["STD_CLASS_FEE"].ToString());
                    stAttPass.STD_TELEPHONE = selectedRow["STD_TELEPHONE"].ToString();
                    stAttPass.STD_ACTIVE_FLG = int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = int.Parse(selectedRow["RSN_ID"].ToString());
                }
                form.IsAddNew = false;
                form.stAtt = stAttPass;
                form.attDate = dtpHoldDate.SelectedDate.Value;
                form.LoadFormContaint();

                form.btnSave.Visibility = System.Windows.Visibility.Visible;
                form.btnDelete.Visibility = System.Windows.Visibility.Hidden;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                dialog.ShowDialog();
                btnRefresh_Click(null, null);
                if (!timer.IsEnabled)
                {
                    timer.Start();
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

                int rowCount = grdStudentAttendance.Items.Count;
                if (rowCount > 0)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("STD_ID", typeof(Int32));
                    table.Columns.Add("CLS_ID", typeof(Int32));
                    table.Columns.Add("STD_INITIALS", typeof(string));
                    table.Columns.Add("CLS_NAME", typeof(string));
                    table.Columns.Add("STD_TELEPHONE", typeof(string));

                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdStudentAttendance.Items[i];
                        Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                        string Telephone = selectedRow["STD_TELEPHONE"].ToString();
                        string StudentName = selectedRow["STD_INITIALS"].ToString();
                        string className = selectedRow["CLS_NAME"].ToString();
                        Int32 classID = int.Parse(selectedRow["CLS_ID"].ToString());
                        table.Rows.Add(studentID, classID, StudentName, className, Telephone);
                    }

                    CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Sent SMS to Member ",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000,
                        Height = 680
                    };
                    form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                    form.callingForm = true;
                    form.filteredTable = table;
                    form.BindStudentGrid();
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

        private void btnSendSMS_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("STD_ID", typeof(Int32));
                    table.Columns.Add("CLS_ID", typeof(Int32));
                    table.Columns.Add("STD_INITIALS", typeof(string));
                    table.Columns.Add("CLS_NAME", typeof(string));
                    table.Columns.Add("STD_TELEPHONE", typeof(string));
                    table.Columns.Add("ATT_ID", typeof(string));

                    Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                    string Telephone = selectedRow["STD_TELEPHONE"].ToString();
                    string StudentName = selectedRow["STD_INITIALS"].ToString();
                    string className = "Defualt";
                    Int32 classID = int.Parse(selectedRow["CLS_ID"].ToString());
                    string mcID = selectedRow["ATT_ID"].ToString();

                    table.Rows.Add(studentID, classID, StudentName, className, Telephone, mcID);

                    CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Sent SMS to Member " + StudentName,
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000,
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
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnComments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    string note = selectedRow["STD_TEMP_NOTE"].ToString();
                    string StudentName = selectedRow["STD_INITIALS"].ToString();
                    MessageBoxResult resultMessageBox = MessageBox.Show(" Note : " + note + "\n Do you want to delete special note?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultMessageBox == MessageBoxResult.Yes)
                    {
                        Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                        ClassAttendanceDA objClss = new ClassAttendanceDA();
                        objClss.UpdateStudentNote(studentID);
                        BindStudentGrid();
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

        private void btnPrintrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grdStudentAttendance.Items.Count > 0)
                {
                    PrintDialog pt = new PrintDialog();
                    if (pt.ShowDialog() == true)
                    {
                        pt.PrintVisual(PaymentParent, "Attendance Sheet");
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled && sender!=null)
            {
                timer.Stop();
            }

            CheckAttStatus();
            CheckAttStatus2();
            try
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_ID = 1;
                _clsStudent.CLS_HOLD_FLG = 1;
                _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());


                DataTable table = _clsStudent.SelectClassAttendance().Tables[0];
                Int32 recordCount = table.Rows.Count;

                lblAttendanceCount.Content = recordCount.ToString();
                PreviousDate = _clsStudent.CLS_REC_DATE;

                string expression = "STD_PAID_STATUS =1";
                DataRow[] selectedRows = table.Select(expression);
                lblCardCount.Content = selectedRows.Length.ToString();

                if (table.Rows.Count > 0)
                {
                    grdStudentAttendance.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdStudentAttendance.ItemsSource = null;
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
                if (!timer.IsEnabled && sender != null)
                {
                    timer.Start();
                }
            }
        }

        

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.SelectedIndex > -1)
                {
                    MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to delete all attendance?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultMessageBox == MessageBoxResult.Yes)
                    {
                        ClassAttendanceDA clsAtt = new ClassAttendanceDA();
                        clsAtt.CLS_REC_DATE = dtpHoldDate.SelectedDate.Value;
                        clsAtt.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                        clsAtt.DeleteClassAttendance();
                        BindStudentGrid();
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

        private void lblTempInactive_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnTempInactive_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cmbClsGroup.SelectedIndex > -1)
                {

                    CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery(Int32.Parse(cmbClsGroup.SelectedValue.ToString()), 2);
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Veiw Member",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000
                    };
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

        private void btnPermInactive_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cmbClsGroup.SelectedIndex > -1)
                {
                    CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery(Int32.Parse(cmbClsGroup.SelectedValue.ToString()), 0);
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Veiw Member",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000
                    };
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }

        private void chkStopRefresh_Checked(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }

        private void chkStopRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        private void CheckAttStatus()
        {
            if (StaticProperty.SKTIPAddres != "")
            {
                try
                {
                    if (StaticProperty.SDK.GetConnectState())
                    {
                        DateTime? diviceTime = null;
                        diviceTime = StaticProperty.SDK.getdeviseTime();
                        if (diviceTime == null)
                        {
                            lblError.Content = "Attendance machine 1 disconnected";
                            lblError.Foreground = System.Windows.Media.Brushes.Red;
                            pbStatus.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            lblError.Foreground = System.Windows.Media.Brushes.Green;
                            lblError.Content = "Connected.Device 1 time: " + diviceTime.Value.ToShortDateString() + ":" + diviceTime.Value.ToLongTimeString();
                            pbStatus.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    else
                    {
                        lblError.Content = "Attendance machine 1 disconnected.";
                        lblError.Foreground = System.Windows.Media.Brushes.Red;
                        pbStatus.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    LogFile logger = new LogFile();
                    logger.MyLogFile(ex);
                }
            }
        }

        private void CheckAttStatus2()
        {
            if (StaticProperty.SKTIPAddres_2 != "")
            {
                try
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        DateTime? diviceTime = null;
                        diviceTime = StaticProperty.SDK_2.getdeviseTime();
                        if (diviceTime == null)
                        {
                            lblError2.Content = "Attendance 2 machine disconnected";
                            lblError2.Foreground = System.Windows.Media.Brushes.Red;
                            pbStatus2.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            lblError2.Foreground = System.Windows.Media.Brushes.Green;
                            lblError2.Content = "Connected.Device 2 time: " + diviceTime.Value.ToShortDateString() + ":" + diviceTime.Value.ToLongTimeString();
                            pbStatus2.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    else
                    {
                        lblError2.Content = "Attendance machine 2 disconnected.";
                        lblError2.Foreground = System.Windows.Media.Brushes.Red;
                        pbStatus2.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    LogFile logger = new LogFile();
                    logger.MyLogFile(ex);
                }
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SKTIPAddres != "")
                {
                    if (StaticProperty.SKTIPAddres == "" || StaticProperty.SKTPortName == "" || StaticProperty.SKTCommKey == "")
                    {
                        MessageBox.Show("*Name, IP, Port or Commkey cannot be null !");
                        return;
                    }

                    if (Convert.ToInt32(StaticProperty.SKTPortName) <= 0 || Convert.ToInt32(StaticProperty.SKTPortName) > 65535)
                    {
                        MessageBox.Show("*Port illegal!");
                        return;
                    }

                    if (Convert.ToInt32(StaticProperty.SKTCommKey) < 0 || Convert.ToInt32(StaticProperty.SKTCommKey) > 999999)
                    {
                        MessageBox.Show("*CommKey illegal!");
                        return;
                    }

                    if (ConnectDivice())
                    {

                        DateTime? diviceTime = null;
                        diviceTime = StaticProperty.SDK.getdeviseTime();

                        if (diviceTime == null)
                        {
                            MessageBox.Show("Device date not set");
                            return;
                        }

                        DateTime mcDate = DateTime.Now;
                        if (diviceTime.Value.ToShortDateString() != mcDate.ToShortDateString())
                        {
                            MessageBox.Show("PC date should be same as Device date");
                            return;
                        }

                        lblError.Content = "Connected.Device time: " + diviceTime.Value.ToShortDateString() + ":" + diviceTime.Value.ToLongTimeString();

                        SyncUserLog();
                        SyncAttendanceLog();

                        lblError.Visibility = System.Windows.Visibility.Visible;
                        pbStatus.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Unablr to connect to the Device");
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

        private void btnConnect2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SKTIPAddres_2 != "")
                {
                    if (ConnectDivice2())
                    {
                        DateTime? diviceTime2 = null;
                        diviceTime2 = StaticProperty.SDK_2.getdeviseTime();

                        if (diviceTime2 == null)
                        {
                            MessageBox.Show("Device 2 date not set");
                            return;
                        }

                        DateTime mcDate2 = DateTime.Now;
                        if (diviceTime2.Value.ToShortDateString() != mcDate2.ToShortDateString())
                        {
                            MessageBox.Show("PC date should be same as Device 2 date");
                            return;
                        }
                        lblError2.Content = "Connected.Device 2 time: " + diviceTime2.Value.ToShortDateString() + ":" + diviceTime2.Value.ToLongTimeString();
                        SyncAttendanceLog2();
                        lblError2.Visibility = System.Windows.Visibility.Visible;
                        pbStatus2.Visibility = System.Windows.Visibility.Visible;
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

        public bool SyncUserLog()
        {
            Boolean retValue = false;
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    int rec = StaticProperty.SDK.sta_GetAllUserInfo_MC();
                    if (rec != 1)
                    {
                        MessageBox.Show("Unable to Sync Attendance from the device.Error Code :" + rec.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Attendace machine not conncted.Please restart the system");
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
            return retValue;
        }

        public bool SyncAttendanceLog()
        {
            Boolean retValue = false;
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    int rec = StaticProperty.SDK.sta_readAttLog();
                    if (rec != 1)
                    {
                        MessageBox.Show("Unable to Sync Attendance from the device.Error Code :" + rec.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Attendace machine not conncted.Please restart the system");
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
            return retValue;
        }

        public bool SyncAttendanceLog2()
        {
            Boolean retValue = false;
            try
            {
                if (StaticProperty.SDK_2.GetConnectState())
                {
                    int rec = StaticProperty.SDK_2.sta_readAttLog();
                    if (rec != 1)
                    {
                        MessageBox.Show("Unable to Sync Attendance 2 from the device.Error Code :" + rec.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Attendace machine 2 not conncted.Please check the connection and retry");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
            return retValue;
        }

        private Boolean ConnectDivice()
        {
            Boolean returnValue = false;
            try
            {
                int ret = StaticProperty.SDK.sta_ConnectTCP(StaticProperty.SKTIPAddres, StaticProperty.SKTPortName, StaticProperty.SKTCommKey);
                if (ret == 1)
                {
                    returnValue = true;             
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
            return returnValue;
        }

        private Boolean ConnectDivice2()
        {
            Boolean returnValue = false;
            try
            {               
                int ret = StaticProperty.SDK_2.sta_ConnectTCP(StaticProperty.SKTIPAddres_2, StaticProperty.SKTPortName, StaticProperty.SKTCommKey);
                if (ret == 1)
                {
                    returnValue = true;
                }               
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
            return returnValue;
        }

    }
}
