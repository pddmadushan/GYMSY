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

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for FingerPrintSummary.xaml
    /// </summary>
    public partial class FingerPrintSummary : UserControl
    {
        public FingerPrintSummary()
        {
            InitializeComponent();
        }

        public string BADGENUMBER { get; set; }
        public Boolean isActive { get; set; }
        public Boolean isChanged { get; set; }

        public DataSet usersPF { get; set; }
        internal void ShowAddData()
        {
            if (isActive)
            {
                btnDeleteFPPrymary.Visibility = Visibility.Visible;
                btnDeleteFPSecondory.Visibility = Visibility.Visible;
            }
            else
            {
                btnDeleteFPPrymary.Visibility = Visibility.Hidden;
                btnDeleteFPSecondory.Visibility = Visibility.Hidden;
            }
            lblAttendanceID.Content = "Attendance Number (MC): " + BADGENUMBER;
            LoadSystemPF();
            LoadPrimary();
            LoadSecondory();
            isChanged = false;
        }
        private void LoadSystemPF()
        {
            try
            {
                SMSDA _obj = new SMSDA();
                usersPF = _obj.GetUsersFP(BADGENUMBER);
                System.Data.DataTable table = usersPF.Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdFPSys.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdFPSys.ItemsSource = null;
                }
                string strCardno = usersPF.Tables[1].Rows[0]["CAREDNUMBER"].ToString();
                string strPassword = usersPF.Tables[1].Rows[0]["USER_PASSWORD"].ToString();

                lblSysCardPw.Content = "";
                if (strCardno != "0")
                {
                    lblSysCardPw.Content = "CRD: " + strCardno;
                }
                if (strPassword != "")
                {
                    lblSysCardPw.Content = lblSysCardPw.Content + "PW: " + strPassword;
                }

                if (table.Rows.Count == 0 && lblSysCardPw.Content.ToString().Trim() == "")
                {
                    btnUpdatetoPrimaryMachin.IsEnabled = false;
                    btnUpdatetoSecondoryMachin.IsEnabled = false;
                }
                else
                {
                    btnUpdatetoPrimaryMachin.IsEnabled = true;
                    btnUpdatetoSecondoryMachin.IsEnabled = true;

                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void LoadPrimary()
        {
            try
            {
                if (StaticProperty.SKTIPAddres != "")
                {
                    if (StaticProperty.SDK.GetConnectState())
                    {                        
                        System.Data.DataTable table = StaticProperty.SDK.sta_GetUserTmp(BADGENUMBER);
                        if (table.Rows.Count > 0)
                        {
                            grdFPPry.ItemsSource = table.DefaultView;                           
                        }
                        else
                        {
                            
                            grdFPPry.ItemsSource = null;
                        }
                        lblDvc1CardPw.Content = StaticProperty.SDK.sta_GetCardandPassword(BADGENUMBER);

                        if (table.Rows.Count == 0 && lblDvc1CardPw.Content.ToString().Trim() == "")
                        {
                            btnUpdatetoSystem.IsEnabled = false;
                        }
                        else
                        {
                            btnUpdatetoSystem.IsEnabled = true;
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

        private void LoadSecondory()
        {
            try
            {
                if (StaticProperty.SKTIPAddres != "" && StaticProperty.SKTIPAddres_2 != "")
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        System.Data.DataTable table = StaticProperty.SDK_2.sta_GetUserTmp(BADGENUMBER);
                        if (table.Rows.Count > 0)
                        {
                            grdFPSec.ItemsSource = table.DefaultView;
                        }
                        else
                        {
                            grdFPSec.ItemsSource = null;
                        }
                        lblDvc2CardPw.Content = StaticProperty.SDK_2.sta_GetCardandPassword(BADGENUMBER);
                        if (table.Rows.Count == 0 && lblDvc2CardPw.Content.ToString().Trim() == "")
                        {
                            btnUpdatetoSystemfromSecond.IsEnabled = false;
                        }
                        else
                        {
                            btnUpdatetoSystemfromSecond.IsEnabled = true;
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

        private void btnR1_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemPF();
        }

        private void btnR2_Click(object sender, RoutedEventArgs e)
        {
            LoadPrimary();
        }

        private void btnR3_Click(object sender, RoutedEventArgs e)
        {
            LoadSecondory();
        }

        private void btnUpdatetoPrimaryMachin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to update Primary Finger  Print?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (StaticProperty.SKTIPAddres != "")
                    {
                        if (StaticProperty.SDK.GetConnectState())
                        {
                            int rec = StaticProperty.SDK.sta_SetUserFPInfo(BADGENUMBER, usersPF);
                            if (rec != 1)
                            {
                                MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                            }
                            else
                            {
                                MessageBox.Show("Record has been updated", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                                isChanged = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void btnUpdatetoSecondoryMachin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to update Second Machine Finger  Print?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (StaticProperty.SKTIPAddres != "" && StaticProperty.SKTIPAddres_2 != "")
                    {
                        if (StaticProperty.SDK_2.GetConnectState())
                        {
                            int rec = StaticProperty.SDK_2.sta_SetUserFPInfo(BADGENUMBER, usersPF);
                            if (rec != 1)
                            {
                                MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                            }
                            else
                            {
                                MessageBox.Show("Record has been updated", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                                isChanged = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void btnUpdatetoSystem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to update System Finger Print from Primary Machine?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (StaticProperty.SKTIPAddres != "")
                    {
                        if (StaticProperty.SDK.GetConnectState())
                        {
                            int rec = 0;
                            rec = StaticProperty.SDK.sta_InsertFPDatatoDB(BADGENUMBER);
                            if (rec > 0)
                            {
                                MessageBox.Show(rec.ToString() + " finger print,card no or password push to database", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            }
                            else
                            {
                                if (rec == -2)
                                {
                                    MessageBox.Show("Unableto find any supported authentication methods:" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                }
                                else
                                {
                                    MessageBox.Show("Error Occored.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void btnDeleteFPPrymary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SystemExistsLogingInfo())
                {
                    MessageBoxResult result = MessageBox.Show("Do you really want to delete Primary Machine Finger Print?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (StaticProperty.SKTIPAddres != "")
                        {
                            if (StaticProperty.SDK.GetConnectState())
                            {
                                int rec = 0;
                                rec = StaticProperty.SDK.sta_OnyDelUserTmp(BADGENUMBER);
                                if (rec != 1)
                                {
                                    MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                }
                                else
                                {
                                    MessageBox.Show("Finger Print has been deleted from Primary Machine succesfully", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                                    isChanged = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
        private Boolean SystemExistsLogingInfo()
        {
            if (grdFPSys.Items.Count > 0 || lblSysCardPw.Content.ToString().Trim() != "")
            {
                return true;
            }
            return false;
        }
        private void btnUpdatetoSystemfromSecond_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to update System Finger Print from Second Machine?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (StaticProperty.SKTIPAddres != "" && StaticProperty.SKTIPAddres_2 != "")
                    {
                        if (StaticProperty.SDK_2.GetConnectState())
                        {
                            int rec = 0;
                            rec = StaticProperty.SDK_2.sta_InsertFPDatatoDB4mSec(BADGENUMBER);
                            if (rec > 0)
                            {
                                MessageBox.Show(rec.ToString() + " finger print,card no or password push to database", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            }
                            else
                            {
                                MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void btnDeleteFPSecondory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SystemExistsLogingInfo())
                {
                    MessageBoxResult result = MessageBox.Show("Do you really want to delete Second Machine Finger Print?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (StaticProperty.SKTIPAddres != "" && StaticProperty.SKTIPAddres_2 != "")
                        {
                            if (StaticProperty.SDK_2.GetConnectState())
                            {
                                int rec = 0;
                                rec = StaticProperty.SDK_2.sta_OnyDelUserTmp(BADGENUMBER);
                                if (rec != 1)
                                {
                                    MessageBox.Show("Unable to Delete date from the device.Error Code :" + rec.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                                }
                                else
                                {
                                    MessageBox.Show("Finger Print has been deleted from Second Machine succesfully", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                                    isChanged = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Attendance Machine no Connected!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
    }
}
