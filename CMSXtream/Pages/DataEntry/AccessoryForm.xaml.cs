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
    /// Interaction logic for AccessoryForm.xaml
    /// </summary>
    public partial class AccessoryForm : UserControl
    {
        public Boolean IsAddNew { get; set; }
        public string OutResult { get; set; }
        public AccessoryAttribute acAtt;

        public AccessoryForm()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void LoadFormContaint()
        {
       
            if (IsAddNew)
            {
                lblAccessoryId.Content = "[New Accessory]";
                btnDelete.Visibility = System.Windows.Visibility.Hidden;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                ClearControll();
            }
            else
            {
                lblAccessoryId.Content = acAtt.ACC_ID;
                txtName.Text = acAtt.ACC_NAME.ToString();
                txtComment.Text = acAtt.ACC_COMMENT.ToString();
                chkPayble.IsChecked = acAtt.ACC_PAYBLE_FLG == 1 ? true : false; 
                txtAccessoryAmount.Text = acAtt.ACC_AMOUNT.ToString();
                chkMandatory.IsChecked = acAtt.ACC_MANDATORY == 1 ? true : false; ;
            }
        }
        private void ClearControll()
        {
            try
            {
                txtName.Text = String.Empty;
                txtComment.Text = String.Empty;
                txtComment.Text = String.Empty;
                chkPayble.IsChecked = false;
                txtAccessoryAmount.Text = "0.00";
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
            if (ValidatetoSave())
            {
                try
                {
                    AccessoryDA _clsAccessory = new AccessoryDA();
                    if (lblAccessoryId.Content.ToString() == "[New Accessory]")
                    {
                        _clsAccessory.classFeilds.ACC_ID = 0;
                    }
                    else
                    {
                        _clsAccessory.classFeilds.ACC_ID = Int32.Parse(lblAccessoryId.Content.ToString());
                    }

                    _clsAccessory.classFeilds.ACC_NAME = txtName.Text.ToString();
                    _clsAccessory.classFeilds.ACC_COMMENT = txtComment.Text.Trim();
                    _clsAccessory.classFeilds.ACC_AMOUNT = double.Parse(txtAccessoryAmount.Text.Trim());
                    _clsAccessory.classFeilds.ACC_PAYBLE_FLG = chkPayble.IsChecked.Value ? 1 : 0;
                    _clsAccessory.classFeilds.ACC_MANDATORY = chkMandatory.IsChecked.Value ? 1 : 0;
                    System.Data.DataTable table = _clsAccessory.InsertAccessory().Tables[0];

                    lblAccessoryId.Content = table.Rows[0]["ACC_ID"].ToString();
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
                MessageBoxResult result = MessageBox.Show("Do you want to delete this accessory?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    AccessoryDA _clsAccessory = new AccessoryDA();
                    _clsAccessory.classFeilds.ACC_ID = Int32.Parse(lblAccessoryId.Content.ToString());
                    if (_clsAccessory.DeleteAccessory() > 0)
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

        private bool ValidatetoSave()
        {
            try
            {
                
               
                if (txtName.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Description is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    txtName.Focus();
                    return false;
                }

                if (Boolean.Parse(chkPayble.IsChecked.ToString()))
                {
                    if (txtAccessoryAmount.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Amount is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtAccessoryAmount.Focus();
                        return false;
                    }
                }
                else
                {
                    txtAccessoryAmount.Text = "0.00";
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

    }
}
