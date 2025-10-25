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
    /// Interaction logic for ClassCategory.xaml
    /// </summary>
    public partial class ClassCategory : UserControl
    {
        public ClassCategory()
        {
            InitializeComponent();
            BindStudentGrid();
        }
        private void BindStudentGrid()
        {
            try
            {
                ClassCategoryDA _ClsCategory = new ClassCategoryDA();
                System.Data.DataTable table = _ClsCategory.SelectAllCategory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdClsCategory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdClsCategory.ItemsSource = null;
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
            CMSXtream.Pages.DataEntry.CategoryForm form = new CMSXtream.Pages.DataEntry.CategoryForm();
            form.IsAddNew = true;
            PopupHelper dialog = new PopupHelper
            {
                Title = "Add Category",
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
            CMSXtream.Pages.DataEntry.CategoryForm form = new CMSXtream.Pages.DataEntry.CategoryForm();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Edit Category",
                Content = form,
                ResizeMode = ResizeMode.NoResize
            };

            ClassCategoryAttribute catAttPass = new ClassCategoryAttribute();
            var selectedRow = grdClsCategory.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                catAttPass.CAT_ID = int.Parse(selectedRow["CAT_ID"].ToString());
                catAttPass.CAT_NAME = selectedRow["CAT_NAME"].ToString();
            }
            form.IsAddNew = false;
            form.catAtt = catAttPass;
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
