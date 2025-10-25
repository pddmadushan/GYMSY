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
    /// Interaction logic for Class.xaml
    /// </summary>
    public partial class ClassGroup : UserControl
    {
        public ClassGroup()
        {
            InitializeComponent();
            BindStudentGrid();
        }
        private void BindStudentGrid()
        {
            try
            {
                ClassGroupDA _clsAccessory = new ClassGroupDA();
                System.Data.DataTable table = _clsAccessory.SelectAllClass().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdClass.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdClass.ItemsSource = null;
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
            CMSXtream.Pages.DataEntry.ClassGroupForm form = new CMSXtream.Pages.DataEntry.ClassGroupForm();
            form.IsAddNew = true;
            PopupHelper dialog = new PopupHelper
            {
                Title = "Add Group",
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
            CMSXtream.Pages.DataEntry.ClassGroupForm form = new CMSXtream.Pages.DataEntry.ClassGroupForm();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Edit Group",
                Content = form,
                ResizeMode = ResizeMode.NoResize
            };

            ClassGroupAttribute clsAttPass = new ClassGroupAttribute();
            var selectedRow = grdClass.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                clsAttPass.CLS_ID = Int32.Parse(selectedRow["CLS_ID"].ToString());
                clsAttPass.CAT_ID = Int32.Parse(selectedRow["CAT_ID"].ToString());
                clsAttPass.CLS_START_DATE = DateTime.Parse(selectedRow["CLS_START_DATE"].ToString());
                clsAttPass.CLS_DAY = int.Parse(selectedRow["CLS_DAY"].ToString());
                clsAttPass.CLS_TIME = double.Parse(selectedRow["CLS_TIME"].ToString());
                clsAttPass.CLS_DURATION = double.Parse(selectedRow["CLS_DURATION"].ToString()); ;
                clsAttPass.CLS_NAME = selectedRow["CLS_NAME"].ToString();
                clsAttPass.CLS_FEE = Double.Parse(selectedRow["CLS_FEE"].ToString());
                clsAttPass.CLS_ADMITION_AMT = Double.Parse(selectedRow["CLS_ADMITION_AMT"].ToString());
                clsAttPass.CLS_ACTIVE_FLG = int.Parse(selectedRow["CLS_ACTIVE_FLG"].ToString());
                clsAttPass.CLS_COMMENT = selectedRow["CLS_COMMENT"].ToString();
                clsAttPass.IS_CLASS_FLG = int.Parse(selectedRow["IS_CLASS_FLG"].ToString());
                clsAttPass.TOTAL_NUMBER_OF_WEEK = Int32.Parse(selectedRow["TOTAL_NUMBER_OF_WEEK"].ToString());
            }
            form.IsAddNew = false;
            form.clasAtt = clsAttPass;
            form.LoadFormContaint();
            dialog.ShowDialog();
            string ReturnMessage = form.OutResult;
            if (ReturnMessage != string.Empty && ReturnMessage != null)
            {
                MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                BindStudentGrid();
            }
        }

        private void btnveiwStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                   var selectedRow = grdClass.SelectedItem as System.Data.DataRowView;
                   if (selectedRow != null)
                   {
                       CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery(Int32.Parse(selectedRow["CLS_ID"].ToString()));
                       PopupHelper dialog = new PopupHelper
                       {
                           Title = "Veiw Student",
                           Content = form,
                           ResizeMode = ResizeMode.NoResize,
                           Width = 1000
                       };
                       dialog.ShowDialog();
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
    }
}
