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
    /// Interaction logic for Accessory.xaml
    /// </summary>
    public partial class Accessory : UserControl
    {


        public Accessory()
        {
            InitializeComponent();
            BindStudentGrid();
        }


        private void BindStudentGrid()
        {
            try
            {
                AccessoryDA _clsAccessory = new AccessoryDA();
                System.Data.DataTable table = _clsAccessory.SelectAllAccessory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdAccessory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdAccessory.ItemsSource = null;
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
            CMSXtream.Pages.DataEntry.AccessoryForm form = new CMSXtream.Pages.DataEntry.AccessoryForm();
            form.IsAddNew = true;
            PopupHelper dialog = new PopupHelper
            {
                Title = "Add Accessory",
                Content = form,
                ResizeMode = ResizeMode.NoResize
            };
            form.LoadFormContaint();
            dialog.ShowDialog();
            string ReturnMessage = form.OutResult;
            if (ReturnMessage != string.Empty && ReturnMessage != null)
            {
                MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                BindStudentGrid();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            CMSXtream.Pages.DataEntry.AccessoryForm form = new CMSXtream.Pages.DataEntry.AccessoryForm();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Edit Accessory",
                Content = form,
                ResizeMode = ResizeMode.NoResize
            };

            AccessoryAttribute stAttPass = new AccessoryAttribute();
            var selectedRow = grdAccessory.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                stAttPass.ACC_ID = int.Parse(selectedRow["ACC_ID"].ToString());
                stAttPass.ACC_NAME = selectedRow["ACC_NAME"].ToString();
                stAttPass.ACC_COMMENT = selectedRow["ACC_COMMENT"].ToString();
                stAttPass.ACC_PAYBLE_FLG = int.Parse(selectedRow["ACC_PAYBLE_FLG"].ToString());
                stAttPass.ACC_AMOUNT = Double.Parse(selectedRow["ACC_AMOUNT"].ToString());
                stAttPass.ACC_MANDATORY = int.Parse(selectedRow["ACC_PAY_MANDATORY"].ToString());
            }
            form.IsAddNew = false;
            form.acAtt = stAttPass;
            form.LoadFormContaint();
            dialog.ShowDialog();
            string ReturnMessage = form.OutResult;
            if (ReturnMessage != string.Empty && ReturnMessage != null)
            {
                MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                BindStudentGrid();
            }
        }
    }
}
