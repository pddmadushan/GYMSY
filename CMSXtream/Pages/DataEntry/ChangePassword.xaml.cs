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

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : UserControl
    {

        public Boolean IsAddNew { get; set; }
        public String UserAccount { get; set; }
        public Boolean IsActive { get; set; }
        public string OutResult { get; set; }

        public ChangePassword()
        {
            InitializeComponent();
        }

        public void LoadFormContaint()
        {
            txtPassWord.Password = "";
            txtConfirmPassWord.Password = "";

            if (IsAddNew)
            {
                txtUserId.Text = string.Empty;
                txtUserId.IsEnabled = true;
                chkActive.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                txtUserId.IsEnabled = false;
                chkActive.Visibility = System.Windows.Visibility.Visible;
                txtUserId.Text = UserAccount;
                chkActive.IsChecked = IsActive;
            }
            OutResult = "";
        }

        private void chkActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginDA _clsLogin = new LoginDA();
                _clsLogin.CLS_USER_ID = UserAccount;
                _clsLogin.CLS_USER_ACTIVE = 1;
                _clsLogin.UpdateActiveStatus();
                OutResult = "User Account information has been updated!"; 
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkActive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginDA _clsLogin = new LoginDA();
                _clsLogin.CLS_USER_ID = UserAccount;
                _clsLogin.CLS_USER_ACTIVE = 0;
                _clsLogin.UpdateActiveStatus();
                OutResult = "User Account information has been updated!"; 
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
                LoginDA _clsLogin = new LoginDA();
                if (IsAddNew)
                {

                    if (txtUserId.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Please define a user name!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtUserId.Focus();
                        return;
                    }

                    if (txtPassWord.Password.Trim() == String.Empty)
                    {
                        MessageBox.Show("Please define a password!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtPassWord.Focus();
                        return;
                    }

                    if (txtPassWord.Password.Trim() != txtConfirmPassWord.Password.Trim())
                    {
                        MessageBox.Show("Password confirmation is wrong!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtConfirmPassWord.Focus();
                        return;
                    }
                    
                    _clsLogin.CLS_USER_ID = txtUserId.Text.Trim();
                    _clsLogin.CLS_USER_PASSWORD = txtPassWord.Password.Trim();

                    System.Data.DataTable table = _clsLogin.SaveUser().Tables[0];

                    string rutValue = table.Rows[0]["CLS_USER_ID"].ToString();
                    string returnMsg = table.Rows[0]["RETURN_MSG"].ToString();
                    if (rutValue == "1")
                    {
                        OutResult = returnMsg;
                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    }
                    else
                    {
                        MessageBox.Show(returnMsg, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    }
                }
                else
                {
                    if (txtPassWord.Password.Trim() != String.Empty)
                    {
                        if (txtPassWord.Password.Trim() != txtConfirmPassWord.Password.Trim())
                        {
                            MessageBox.Show("Password confirmation is wrong!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            txtConfirmPassWord.Focus();
                            return;
                        }

                        _clsLogin.CLS_USER_ID = UserAccount;
                        _clsLogin.CLS_USER_PASSWORD = txtPassWord.Password.Trim();
                        _clsLogin.UpdatePassword();
                        OutResult = "User Account information has been updated!";                        
                    }

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
}
