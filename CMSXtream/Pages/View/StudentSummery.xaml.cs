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
    /// Interaction logic for StudentSummery.xaml
    /// </summary>
    public partial class StudentSummery : UserControl
    {
        public DateTime classDate { get; set; }
        public Int32 selectedClass { get; set; }
        public Boolean NotUserContorlLoad { get; set; }
        public int calltogetStdID { get; set; }
        public int STD_ID { get; set; }
        public StudentSummery()
        {
            InitializeComponent();            

            cmbSearchType.DisplayMemberPath = "Value";
            cmbSearchType.SelectedValuePath = "Key";
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("0", "--All--"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("1", "ID"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("2", "Name"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("3", "NIC"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("4", "Telephone"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("5", "Class Group"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("6", "Reason of Inactivation"));

            cmbSearchType.SelectedIndex = 4;
            cmbClassGroup.SelectedIndex = -1;
            txtSearchText.Text = "";
            LoadClassCombo();
            LoadReason();
            BindStudentGrid();
        }

        public StudentSummery(Int32 selectedClassGroup)
        {
            InitializeComponent();

            cmbSearchType.DisplayMemberPath = "Value";
            cmbSearchType.SelectedValuePath = "Key";
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("0", "--All--"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("1", "ID"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("2", "Name"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("3", "NIC"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("4", "Telephone"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("5", "Class Group"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("6", "Reason of Inactivation"));

            btnAddNew.Visibility = System.Windows.Visibility.Hidden;
            grdStudentSummery.Columns[19].Visibility = Visibility.Hidden;

            cmbSearchType.SelectedIndex = 5;
            cmbSearchType.IsEnabled = false;

            LoadClassCombo();
            cmbClassGroup.SelectedValue = selectedClassGroup.ToString();
            cmbClassGroup.IsEnabled = false;

            btnSearch.Visibility = System.Windows.Visibility.Hidden;

            BindStudentGrid();
            NotUserContorlLoad = true;
        }

        public StudentSummery(Int32 selectedClassGroup, Int32 InactiveState)
        {
            InitializeComponent();

            cmbSearchType.DisplayMemberPath = "Value";
            cmbSearchType.SelectedValuePath = "Key";
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("0", "--All--"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("1", "ID"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("2", "Name"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("3", "NIC"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("4", "Telephone"));
            if (InactiveState == 0)
            {
                cmbSearchType.Items.Add(new KeyValuePair<string, string>("101", "Class Group"));
            }
            else
            {
                cmbSearchType.Items.Add(new KeyValuePair<string, string>("100", "Class Group"));
            }

            btnAddNew.Visibility = System.Windows.Visibility.Hidden;

            cmbSearchType.SelectedIndex = 5;
            cmbSearchType.IsEnabled = false;

            LoadClassCombo();
            cmbClassGroup.SelectedValue = selectedClassGroup.ToString();
            cmbClassGroup.IsEnabled = false;

            btnSearch.Visibility = System.Windows.Visibility.Hidden;

            BindStudentGrid();
            NotUserContorlLoad = true;
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        public void ShowAddStudent()
        {
            //grdStudentSummery.Columns[18].Visibility = Visibility.Hidden;
            grdStudentSummery.Columns[19].Visibility = Visibility.Visible;
            grdStudentSummery.Columns[20].Visibility = Visibility.Hidden;
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

        private void LoadReason()
        {
            try
            {
                ResonDA _clsReason = new ResonDA();
                System.Data.DataTable table = _clsReason.SelectReason().Tables[0];

                System.Data.DataRow toInsert = table.NewRow();
                toInsert["RSN_ID"] = -1;
                toInsert["RSN_DES"] = "Inactive but Reason not assigned";
                table.Rows.InsertAt(toInsert, 0);


                cmbReason.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbReason.ItemsSource = table.DefaultView;
                cmbReason.DisplayMemberPath = "RSN_DES";
                cmbReason.SelectedValuePath = "RSN_ID";
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
                if (cmbSearchType.SelectedValue != null)
                {
                    StudentDA _clsStudent = new StudentDA();
                    int filterID = int.Parse(cmbSearchType.SelectedValue.ToString());
                    string fiterString = txtSearchText.Text.Trim();
                    if (filterID == 5 || filterID == 100 || filterID == 101)
                    {
                        if (cmbClassGroup.SelectedIndex != -1)
                        {
                            fiterString = cmbClassGroup.SelectedValue.ToString();
                        }
                    }
                    if (filterID == 6)
                    {
                        if (cmbReason.SelectedIndex != -1)
                        {
                            fiterString = cmbReason.SelectedValue.ToString();
                        }
                    }

                    System.Data.DataTable table = _clsStudent.SelectAllStudent(filterID, fiterString).Tables[0];
                    
                    if (table.Rows.Count > 0)
                    {
                        grdStudentSummery.ItemsSource = table.DefaultView;
                    }
                    else
                    {
                        grdStudentSummery.ItemsSource = null;
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

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                form.IsAddNew = true;
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Add Member",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1100
                };
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    //MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Edit Member",
                    Content = form,
                    ResizeMode = ResizeMode.CanResize,
                    Width = 1100
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStudentSummery.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                    stAttPass.ATT_ID = int.Parse(selectedRow["ATT_ID"].ToString());
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
                    stAttPass.STD_ACTIVE_FLG = selectedRow["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = int.Parse(selectedRow["RSN_ID"].ToString());
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            BindStudentGrid();
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
                int filterID = int.Parse(cmbSearchType.SelectedValue.ToString());
                if (filterID == 5 || filterID == 100 || filterID == 101)
                {
                    txtSearchText.Visibility = System.Windows.Visibility.Hidden;
                    cmbClassGroup.Visibility = System.Windows.Visibility.Visible;
                    cmbReason.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    if (filterID == 6)
                    {
                        cmbReason.Visibility = System.Windows.Visibility.Visible;
                        txtSearchText.Visibility = System.Windows.Visibility.Hidden;
                        cmbClassGroup.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        txtSearchText.Visibility = System.Windows.Visibility.Visible;
                        cmbClassGroup.Visibility = System.Windows.Visibility.Hidden;
                        cmbReason.Visibility = System.Windows.Visibility.Hidden;
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
            if (!NotUserContorlLoad)
            {
                LoadClassCombo();
                LoadReason();
                BindStudentGrid();
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StudentDA studentDaClas = new StudentDA();
                var selectedRow = grdStudentSummery.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    if (calltogetStdID == 1)
                    {
                        STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    }
                    else
                    {
                        if (selectedRow["STD_ACTIVE_FLG"].ToString() != "1")
                        {
                            MessageBoxResult resultMessageBox = MessageBox.Show("Inactive member found and do you want to continue?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (resultMessageBox == MessageBoxResult.No)
                            {
                                return;
                            }
                            // MessageBox.Show("Inactive student cannot be added to the class", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                            // return;
                        }

                        studentDaClas.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        studentDaClas.CLS_REC_DATE = classDate;
                        studentDaClas.MODIFY_USER = StaticProperty.LoginUserID;

                        studentDaClas.AddStudentToClass();
                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();

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

        private void btnSendSMSALL_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int rowCount = grdStudentSummery.Items.Count;
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
                        var selectedRow = (System.Data.DataRowView)grdStudentSummery.Items[i];
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

        private void btnSendSMS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StudentDA studentDaClas = new StudentDA();
                var selectedRow = grdStudentSummery.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                    string Telephone = selectedRow["STD_TELEPHONE"].ToString();
                    string StudentName = selectedRow["STD_INITIALS"].ToString();
                    string className = selectedRow["CLS_NAME"].ToString();
                    Int32 classID = int.Parse(selectedRow["CLS_ID"].ToString());
                    string mcID =  selectedRow["ATT_ID"].ToString();

                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("STD_ID", typeof(Int32));
                    table.Columns.Add("CLS_ID", typeof(Int32));
                    table.Columns.Add("STD_INITIALS", typeof(string));
                    table.Columns.Add("CLS_NAME", typeof(string));
                    table.Columns.Add("STD_TELEPHONE", typeof(string));
                    table.Columns.Add("ATT_ID", typeof(string));
                    table.Rows.Add(studentID, classID, StudentName, className, Telephone,mcID);

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
                var selectedRow = grdStudentSummery.SelectedItem as System.Data.DataRowView;
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

    }
}
