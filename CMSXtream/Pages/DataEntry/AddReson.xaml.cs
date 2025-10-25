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
    /// Interaction logic for AddReson.xaml
    /// </summary>
    public partial class AddReson : UserControl
    {
        public AddReson()
        {
            InitializeComponent();
            BindReasonGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtReason.Text.Trim() != string.Empty)
                {
                    ResonDA _clsReason = new ResonDA();
                    _clsReason.RSN_DES = txtReason.Text.Trim();
                    _clsReason.RSN_ID = Int32.Parse(lblId.Content.ToString());
                    if (_clsReason.SaveReason() > 0)
                    {
                        BindReasonGrid();
                        lblId.Content = "-1";
                        txtReason.Text = "";
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

        private void BindReasonGrid()
        {
            try
            {
                ResonDA _clsReason = new ResonDA();
                System.Data.DataTable table = _clsReason.SelectReason().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdReason.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdReason.ItemsSource = null;
                }           

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void grdReason_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                Int32 resonId = Int32.Parse(selectedRow["RSN_ID"].ToString());
                lblId.Content = resonId.ToString();
                txtReason.Text = selectedRow["RSN_DES"].ToString();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to delete this reason?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    ResonDA _clsReason = new ResonDA();
                    Int32 resonId = Int32.Parse(selectedRow["RSN_ID"].ToString());
                    _clsReason.RSN_ID = resonId; 
                    _clsReason.DeleteReason();
                    BindReasonGrid();
                }
            }
        }

    }
}
