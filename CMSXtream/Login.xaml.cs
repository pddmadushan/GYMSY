using CMSXtream;
using CMSXtream.Handlers;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using XtreamDataAccess;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Boolean Login { get; set; }
        //private DispatcherTimer timer;
        public MainWindow()
        {

            try
            {

                InitializeComponent();

                var IP = System.Configuration.ConfigurationManager.AppSettings.Get("DeviceIP");
                string IPAdress = "";
                if (IP != null)
                {
                    IPAdress = IP.ToString();
                }

                var IP_2 = System.Configuration.ConfigurationManager.AppSettings.Get("DeviceIP_2");
                string IPAdress_2 = "";
                if (IP_2 != null)
                {
                    IPAdress_2 = IP_2.ToString();
                }

                StaticProperty.SKTIPAddres = IPAdress;
                StaticProperty.SKTIPAddres_2 = IPAdress_2;
                StaticProperty.SKTPortName = System.Configuration.ConfigurationManager.AppSettings.Get("COMPORT").ToString();
                StaticProperty.SKTCommKey = System.Configuration.ConfigurationManager.AppSettings.Get("COMMKEY").ToString();

                if (!ValidLicense())
                {
                    txtUserName.Visibility = System.Windows.Visibility.Hidden;
                    txtPassword.Visibility = System.Windows.Visibility.Hidden;
                    btnLogin.Visibility = System.Windows.Visibility.Hidden;
                    btnConnect.Visibility = System.Windows.Visibility.Hidden;
                    pbStatus.Visibility = Visibility.Hidden;
                    return;
                }

                if (StaticProperty.SKTIPAddres == "")
                {
                    LoadLogin();
                }
                else
                {
                    txtUserName.Visibility = System.Windows.Visibility.Hidden;
                    txtPassword.Visibility = System.Windows.Visibility.Hidden;
                    btnLogin.Visibility = System.Windows.Visibility.Hidden;
                }

                SetLogin();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void LoadLogin()
        {
            try
            {

                txtUserName.Visibility = System.Windows.Visibility.Visible;
                txtPassword.Visibility = System.Windows.Visibility.Visible;
                pbStatus.Visibility = System.Windows.Visibility.Hidden;
                btnConnect.Visibility = System.Windows.Visibility.Hidden;
                btnLogin.Visibility = System.Windows.Visibility.Visible;

                txtUserName.Focus();

                string ComName = System.Configuration.ConfigurationManager.AppSettings.Get("CompanyName").ToString();
                CMSXtream.StaticProperty.ClientName = ComName;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void LoadAttSync()
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

                    if (StaticProperty.SDK.GetConnectState() || ConnectDivice())
                    {

                        DateTime? diviceTime = null;
                        diviceTime = StaticProperty.SDK.getdeviseTime();

                        if (diviceTime == null)
                        {
                            MessageBox.Show("Device date not set");
                            btnSetTime1.Visibility = Visibility.Visible;
                            return;
                        }

                        DateTime mcDate = DateTime.Now;
                        if (diviceTime.Value.ToShortDateString() != mcDate.ToShortDateString())
                        {
                            MessageBox.Show("PC date should be same as device date");
                            btnSetTime1.Visibility = Visibility.Visible;
                            return;
                        }


                        if (StaticProperty.SKTIPAddres_2 != "")
                        {
                            if (StaticProperty.SDK_2.GetConnectState() || ConnectDivice2())
                            {
                                DateTime? diviceTime2 = null;
                                diviceTime2 = StaticProperty.SDK_2.getdeviseTime();

                                if (diviceTime2 == null)
                                {
                                    MessageBox.Show("Device 2 date not set");
                                    btnSetTime2.Visibility = Visibility.Visible;
                                    return;
                                }

                                DateTime mcDate2 = DateTime.Now;
                                if (diviceTime2.Value.ToShortDateString() != mcDate2.ToShortDateString())
                                {
                                    MessageBox.Show("PC date should be same as device 2 date");
                                    btnSetTime2.Visibility = Visibility.Visible;
                                    return;
                                }
                                setTime2(diviceTime2.Value);
                            }
                        }

                        setTime(diviceTime.Value);

                        SyncUserLog();

                        SyncAttendanceLog();
                        SyncAttendanceLog2();

                        DeleteAttendanceLog();
                        DeleteAttendanceLog2();

                        DeleteUserFringerPint();

                        SendPaymentDailySMS();

                        LoadLogin();
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

        private void BindStudentGrid()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_ID = 1;
                _clsStudent.CLS_HOLD_FLG = 1;
                _clsStudent.CLS_REC_DATE = DateTime.Now;

                DataTable table = _clsStudent.SelectClassAttendance().Tables[0];

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
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


        private void SetLogin()
        {
            txtUserName.Text = "sysadmin";
            txtPassword.Password = "0772018009";
            btnKeyGen.Visibility = Visibility.Visible;            
            btnUpdateLicenseKey.Visibility = Visibility.Visible;

            txtLicenseKey.Visibility = Visibility.Visible;
            txtLicenseKey.Text = "G0000FZQzZJd00NfslXdJ00PaMRkVxY00UiaGhUR05000KN";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errorValidator.Visibility = System.Windows.Visibility.Hidden;
            if (txtUserName.Text.Trim() == string.Empty)
            {
                errorValidator.Visibility = System.Windows.Visibility.Visible;
                errorValidator.ToolTip = "Enter Username";
                errorValidator.SetValue(Grid.RowProperty, 1);
                errorValidator.SetValue(Grid.ColumnProperty, 3);
                return;
            }

            if (txtPassword.Password.Trim() == string.Empty || txtPassword.Password.Trim() == "^%!@#%$")
            {
                errorValidator.Visibility = System.Windows.Visibility.Visible;
                errorValidator.ToolTip = "Enter Password";
                errorValidator.SetValue(Grid.RowProperty, 2);
                errorValidator.SetValue(Grid.ColumnProperty, 3);
                return;
            }

            Boolean passwordValidation = false;
            string accountisAdmin;
            //This is for SysAdmin
            if (txtUserName.Text.Trim().ToUpper() == "SYSADMIN")
            {
                if (txtPassword.Password.Trim() == "0772018009")
                {
                    passwordValidation = true;
                    accountisAdmin = "1";
                }
                else
                {
                    errorValidator.Visibility = System.Windows.Visibility.Visible;
                    errorValidator.ToolTip = "Incorrect Password";
                    errorValidator.SetValue(Grid.RowProperty, 2);
                    errorValidator.SetValue(Grid.ColumnProperty, 3);
                    return;
                }
            }
            else
            {
                LoginDA loginDA = new LoginDA();
                loginDA.CLS_USER_ID = txtUserName.Text.Trim().ToUpper();

                System.Data.DataTable table = loginDA.GetUserDetails().Tables[0];
                if (table.Rows.Count > 0)
                {
                    string accountActiveFlg = table.Rows[0]["CLS_USER_ACTIVE"].ToString();
                    accountisAdmin = table.Rows[0]["USER_IS_ADMIN"].ToString();
                    string accountPW = table.Rows[0]["CLS_USER_PASSWORD"].ToString();
                    if (accountActiveFlg == "1")
                    {
                        StringComparer comparer = StringComparer.Ordinal;
                        string getHash = loginDA.GenarateXtreamHash(txtPassword.Password.Trim());
                        if (comparer.Compare(getHash, accountPW) == 0)
                        {
                            //txtUserName.Text = "Administrator";
                            passwordValidation = true;
                        }
                        else
                        {
                            errorValidator.Visibility = System.Windows.Visibility.Visible;
                            errorValidator.ToolTip = "Incorrect Password";
                            errorValidator.SetValue(Grid.RowProperty, 2);
                            errorValidator.SetValue(Grid.ColumnProperty, 3);
                            return;
                        }
                    }
                    else
                    {
                        errorValidator.Visibility = System.Windows.Visibility.Visible;
                        errorValidator.ToolTip = "Account has been inactivated";
                        errorValidator.SetValue(Grid.RowProperty, 1);
                        errorValidator.SetValue(Grid.ColumnProperty, 3);
                        return;
                    }
                }
                else
                {
                    errorValidator.Visibility = System.Windows.Visibility.Visible;
                    errorValidator.ToolTip = "Incorrect Username";
                    errorValidator.SetValue(Grid.RowProperty, 1);
                    errorValidator.SetValue(Grid.ColumnProperty, 3);
                    return;
                }

            }

            if (passwordValidation)
            {
                CMSXtream.StaticProperty.LoginUserID = txtUserName.Text.Trim().ToUpper();
                CMSXtream.StaticProperty.LoginisAdmin = accountisAdmin;
                Login = true;
                this.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Login = false;
            this.Close();
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = string.Empty;
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == string.Empty)
            {
                txtPassword.Password = "^%!@#%$";
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //timer.Stop();
        }

        private Boolean ConnectDivice()
        {
            Boolean returnValue = false;
            try
            {
                if (lblMasg.Content == "Connected.")
                {
                    MessageBox.Show("Close the system and try to reconect!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return returnValue;
                }

                int ret = StaticProperty.SDK.sta_ConnectTCP(StaticProperty.SKTIPAddres, StaticProperty.SKTPortName, StaticProperty.SKTCommKey);

                if (ret == 1)
                {
                    lblMasg.Content = "Connected.";
                    returnValue = true;
                }
                else
                {
                    //Newly added and not checked - 2020/06/07
                    setLableMessage("Attendace device not conncted.Please check the connectivity and retry");
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
                if (lblMasg_2.Content == "Connected.")
                {
                    MessageBox.Show("Divice 2 is not connected.Close the system and try to reconect!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return returnValue;
                }

                int ret = StaticProperty.SDK_2.sta_ConnectTCP(StaticProperty.SKTIPAddres_2, StaticProperty.SKTPortName, StaticProperty.SKTCommKey);

                if (ret == 1)
                {
                    lblMasg_2.Content = "Connected.";
                    returnValue = true;
                }
                else
                {
                    setLableMessage("Attendace device 2 is not conncted.Please check the connectivity and retry");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
            return returnValue;
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
                        setLableMessage("Unable to Sync Attendance from the device.Error Code :" + rec.ToString());
                    }
                }
                else
                {
                    setLableMessage("Attendace device not conncted.Please check the connection and retry");
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
                    Task<int> task = Task.Run(() => StaticProperty.SDK.sta_readAttLog());
                    task.ContinueWith(antecedent =>
                    {
                        if (antecedent.IsFaulted)
                        {
                            setLableMessage("An error occurred during the SDK(Device 1) call.");
                        }
                        else
                        {
                            int rec1 = antecedent.Result; // Get the result from the completed task
                            setLableMessage("The SDK(Device 1) read {rec1} attendance logs.");
                        }
                    });

                    //int rec = StaticProperty.SDK.sta_readAttLog();
                    //if (rec != 1)
                    //{
                    //    setLableMessage("Unable to Sync Attendance from the device.Error Code :" + rec.ToString());
                    //}
                }
                else
                {
                    setLableMessage("Attendace device not conncted.Please check the connection and retry");
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
                if (StaticProperty.SKTIPAddres_2 != "")
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        int rec = StaticProperty.SDK_2.sta_readAttLog();
                        if (rec != 1)
                        {
                            setLableMessage("Unable to Sync Attendance 2 from the device.Error Code :" + rec.ToString());
                        }
                    }
                    else
                    {
                        setLableMessage("Attendace device 2 not conncted.Please check the connection and retry");
                    }
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

        public bool DeleteAttendanceLog()
        {
            Boolean retValue = false;
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    int rec = StaticProperty.SDK.sta_DeleteLogUptoPreviousDay();
                    if (rec != 1)
                    {
                        setLableMessage("Unable to Delete date from the device.Error Code :" + rec.ToString());
                    }
                }
                else
                {
                    setLableMessage("Attendace device not conncted.Please check the connection and retry");
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

        public bool DeleteAttendanceLog2()
        {
            Boolean retValue = false;
            try
            {
                if (StaticProperty.SKTIPAddres_2 != "")
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        int rec = StaticProperty.SDK_2.sta_DeleteLogUptoPreviousDay2();
                        if (rec != 1)
                        {
                            setLableMessage("Unable to Delete date from the device.Error Code :" + rec.ToString());
                        }
                    }
                    else
                    {
                        setLableMessage("Attendace device 2 not conncted.Please check the connection and retry");
                    }
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

        public bool DeleteUserFringerPint()
        {
            try
            {
                Boolean retValue = false;
                try
                {
                    SMSDA _obj = new SMSDA();
                    DataSet inactiveUsers = _obj.GetInactiveUsers();
                    if (inactiveUsers.Tables[0].Rows.Count > 0)
                    {
                        if (StaticProperty.SDK.GetConnectState())
                        {
                            int rec = 0;
                            foreach (DataRow rd in inactiveUsers.Tables[0].Rows)
                            {
                                string sUserID = rd["BADGENUMBER"].ToString();
                                rec = StaticProperty.SDK.sta_DelUserTmp(sUserID);
                                if (rec != 1)
                                {
                                    setLableMessage("Unable to Delete date from the device.Error Code :" + rec.ToString());
                                }
                            }

                            if (StaticProperty.SKTIPAddres_2 != "")
                            {
                                if (StaticProperty.SDK_2.GetConnectState())
                                {
                                    rec = 0;
                                    foreach (DataRow rd in inactiveUsers.Tables[0].Rows)
                                    {
                                        string sUserID = rd["BADGENUMBER"].ToString();
                                        rec = StaticProperty.SDK_2.sta_DelUserTmp(sUserID);
                                        if (rec != 1)
                                        {
                                            setLableMessage("Unable to Delete date from the device 2.Error Code :" + rec.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    setLableMessage("Attendace device 2 not conncted.Please check the connection and retry");
                                }
                            }
                        }
                        else
                        {
                            setLableMessage("Attendace device not conncted.Please check the connection and retry");
                        }

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
            catch
            {
                throw;
            }
        }
        public void SendPaymentDailySMS()
        {
            try
            {
                SMSDA _obj = new SMSDA();
                _obj.SendPendinPaymentSMS();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
        }

        private void setLableMessage(string messageLable)
        {
            try
            {
                lblError.Content = messageLable;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }

        private void setTime(DateTime dt)
        {
            try
            {
                lblMasg.Content = "Connected. Device 1 time: " + dt.ToShortDateString() + ":" + dt.ToLongTimeString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }

        private void setTime2(DateTime dt)
        {
            try
            {
                lblMasg_2.Content = "Connected. Device 2 time: " + dt.ToShortDateString() + ":" + dt.ToLongTimeString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            lblError.Content = "";
            LoadAttSync();
        }

        private void btnSetTime1_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.SDK.GetConnectState())
            {
                System.Windows.Forms.ListBox lblOutputInfo = new System.Windows.Forms.ListBox();
                StaticProperty.SDK.sta_SetDeviceTime(lblOutputInfo, DateTime.Now);
                btnSetTime1.Visibility = Visibility.Hidden;
            }
        }

        private void btnSetTime2_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.SDK_2.GetConnectState())
            {
                System.Windows.Forms.ListBox lblOutputInfo = new System.Windows.Forms.ListBox();
                StaticProperty.SDK_2.sta_SetDeviceTime(lblOutputInfo, DateTime.Now);
                btnSetTime2.Visibility = Visibility.Hidden;
            }
        }
        
        private void btnSetTime3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnKeyGen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Key = txtLicenseKey.Text.Trim();
                string newKey = HashGenerator.Encrypt(Key);
                txtLicenseKey.Text = newKey;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private Boolean ValidLicense()
        {
            try
            {
                lblError.Content = "";
                LoginDA loginDA = new LoginDA();
                System.Data.DataTable table = loginDA.GetTenantLicensesInfo();

                if (table.Rows.Count == 0)
                {
                    MessageBox.Show("Your product is not installed with a valid license key.", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    EnableKeyControll();
                    return false;
                }

                string newKey = table.Rows[0][1].ToString();
                string tenantDB = table.Rows[0][0].ToString();
                if (!ValidateLicenseKey(newKey))
                {
                    MessageBox.Show("The license key is no longer valid. It may have expired. [" + tenantDB + "]", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    EnableKeyControll();
                    return false;
                }

                string deKey = HashGenerator.Decrypt(newKey);
                string decriptKey = HashGenerator.ResolveKey(deKey);
                string tenant = decriptKey.Substring(0, 4);
                string day = decriptKey.Substring(4, 2);
                string month = decriptKey.Substring(6, 2);
                string Year = decriptKey.Substring(8, 2);
                string amount = decriptKey.Substring(10, 5);
                string updateAmount = deKey.Substring(deKey.Length - 1);

                string dateText = day + "/" + month + "/" + Year;
                DateTime date;
                DateTime.TryParseExact(
                            dateText,
                            "dd/MM/yy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out date);

                if (date.AddDays(-7) < DateTime.Now)
                {
                    lblError.Content = "[" + tenant + "]Your license is about to expire.\r\nKindly complete the payment of " + double.Parse(amount) + " before " + dateText + " to keep your license active.";
                    EnableKeyControll();
                }
                CMSXtream.StaticProperty.Clientlicense = "[" + tenant + "] license valid until " + dateText;
                return true;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            return false;
        }

        private void btnUpdateLicenseKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newKey = txtLicenseKey.Text.Trim();
                if (!ValidateLicenseKey(newKey))
                {
                    MessageBox.Show("Please enter valid license Key", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtLicenseKey.Text = "";
                    return;
                }

                string deKey = HashGenerator.Decrypt(newKey);
                string decriptKey = HashGenerator.ResolveKey(deKey);
                string tenant = decriptKey.Substring(0, 4);
                string day = decriptKey.Substring(4, 2);
                string month = decriptKey.Substring(6, 2);
                string Year = decriptKey.Substring(8, 2);
                string amount = decriptKey.Substring(10, 5);
                string updateAmount = deKey.Substring(deKey.Length - 1);

                string dateText = day + "/" + month + "/" + Year;
                DateTime date;
                DateTime.TryParseExact(
                            dateText,
                            "dd/MM/yy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out date);

                LoginDA loginDA = new LoginDA();
                loginDA.SaveTenantLicenses(tenant, newKey, Int32.Parse(amount), updateAmount);
                lblError.Content = "";
                txtLicenseKey.Text = "";
                EnableLogginControll();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("Please enter valid license Key.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private Boolean ValidateLicenseKey(string newKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newKey))
                {
                    return false;
                }
                string deKey = HashGenerator.Decrypt(newKey);
                string decriptKey = HashGenerator.ResolveKey(deKey);
                if (decriptKey.Length != 15)
                {
                    return false;
                }

                string tenant = decriptKey.Substring(0, 4);
                string day = decriptKey.Substring(4, 2);
                string month = decriptKey.Substring(6, 2);
                string Year = decriptKey.Substring(8, 2);
                string amount = decriptKey.Substring(10, 5);
                string updateAmount = deKey.Substring(deKey.Length - 1);

                string dateText = day + "/" + month + "/" + Year;
                DateTime date;
                if (!DateTime.TryParseExact(
                            dateText,
                            "dd/MM/yy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out date))
                {
                    return false;
                }

                if (date < DateTime.Now)
                {
                    return false;
                }

                if (updateAmount != "U" && updateAmount != "N")
                {
                    return false;
                }
                LoginDA loginDA = new LoginDA();
                System.Data.DataTable table = loginDA.GetTenantLicensesInfo();
                string tenantSys = table.Rows[0][0].ToString();
                if (tenant != tenantSys)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                return false;
            }
        }
        private void EnableLogginControll()
        {
            if (StaticProperty.SKTIPAddres == "")
            {
                LoadLogin();
            }
            else
            {
                pbStatus.Visibility = Visibility.Visible;
                btnConnect.Visibility = Visibility.Visible;

                txtUserName.Visibility = System.Windows.Visibility.Hidden;
                txtPassword.Visibility = System.Windows.Visibility.Hidden;
                btnLogin.Visibility = System.Windows.Visibility.Hidden;
            }

            txtLicenseKey.Visibility = Visibility.Hidden;
            btnUpdateLicenseKey.Visibility = Visibility.Hidden;
        }
        private void EnableKeyControll()
        {
            if (StaticProperty.SKTIPAddres == "")
            {
                pbStatus.Visibility = Visibility.Hidden;
                btnConnect.Visibility = Visibility.Hidden;
            }

            txtLicenseKey.Visibility = Visibility.Visible;
            btnUpdateLicenseKey.Visibility = Visibility.Visible;
        }
    }
}
