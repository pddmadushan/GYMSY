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
    /// Interaction logic for UserAccount.xaml
    /// </summary>
    public partial class UserAccount : UserControl
    {
        public UserAccount()
        {
            InitializeComponent();
            LoadUserAccount();
        }

        private void LoadUserAccount()
        {
            try
            {
                LoginDA _clsLogin = new LoginDA();
                System.Data.DataTable table = _clsLogin.SelectUserAccount().Tables[0];

                if (table.Rows.Count > 0)
                {
                    grdUserAccount.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdUserAccount.ItemsSource = null;
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
                CMSXtream.Pages.DataEntry.ChangePassword form = new CMSXtream.Pages.DataEntry.ChangePassword();
                form.IsAddNew = true;
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Add User",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 370,
                    Height = 250
                };
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    LoadUserAccount();
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
                CMSXtream.Pages.DataEntry.ChangePassword form = new CMSXtream.Pages.DataEntry.ChangePassword();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Edit User",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 370,
                    Height = 250
                };

                LoginDA _clsLogin = new LoginDA();
                var selectedRow = grdUserAccount.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    form.UserAccount = selectedRow["CLS_USER_ID"].ToString();
                    form.IsActive = selectedRow["CLS_USER_ACTIVE"].ToString()=="1";
                }
                form.IsAddNew = false;
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    LoadUserAccount();
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
