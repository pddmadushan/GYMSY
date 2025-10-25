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
    /// Interaction logic for CategoryForm.xaml
    /// </summary>
    public partial class CategoryForm : UserControl
    {
        public Boolean IsAddNew { get; set; }
        public string OutResult { get; set; }
        public ClassCategoryAttribute catAtt;

        public CategoryForm()
        {
            InitializeComponent();
        }

        public void LoadFormContaint()
        {
            if (IsAddNew)
            {
                lblAccessoryId.Content = "[New Category]";
                btnDelete.Visibility = System.Windows.Visibility.Hidden;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                grpCategoryAccesory.Visibility = System.Windows.Visibility.Hidden;
                ClearControll();
            }
            else
            {
                lblAccessoryId.Content = catAtt.CAT_ID;
                txtName.Text = catAtt.CAT_NAME.ToString();
                grpCategoryAccesory.Visibility = System.Windows.Visibility.Visible;
                LoadAssosory();
            }
        }

        private void ClearControll()
        {
            try
            {
                txtName.Text = String.Empty;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatetoSave())
            {
                try
                {
                    ClassCategoryDA _clsCategory = new ClassCategoryDA();
                    if (lblAccessoryId.Content.ToString() == "[New Category]")
                    {
                        _clsCategory.classFeilds.CAT_ID = 0;
                    }
                    else
                    {
                        _clsCategory.classFeilds.CAT_ID = Int32.Parse(lblAccessoryId.Content.ToString());
                    }

                    _clsCategory.classFeilds.CAT_NAME = txtName.Text.ToString();
                    System.Data.DataTable table = _clsCategory.InsertCategory().Tables[0];

                    string returnMsg = table.Rows[0]["RETURN_MSG"].ToString();

                    //if (lblAccessoryId.Content.ToString() == "[New Category]")
                    //{
                    //    lblAccessoryId.Content = table.Rows[0]["CAT_ID"].ToString();
                    //    grpCategoryAccesory.Visibility = System.Windows.Visibility.Visible;
                    //    btnDelete.Visibility = System.Windows.Visibility.Visible;
                    //    LoadAssosory();
                    //    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).WindowStyle = WindowStyle.None;
                    //}
                    //else
                    //{
                    lblAccessoryId.Content = table.Rows[0]["CAT_ID"].ToString();
                    OutResult = returnMsg;
                    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    //}
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
                MessageBoxResult result = MessageBox.Show("Do you want to delete this category?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ClassCategoryDA _clsCategory = new ClassCategoryDA();
                    _clsCategory.classFeilds.CAT_ID = Int32.Parse(lblAccessoryId.Content.ToString());
                    if (_clsCategory.DeleteCategory() > 0)
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

        private void LoadAssosory()
        {
            try
            {
                ClassCategoryDA _clsCategory = new ClassCategoryDA();
                _clsCategory.classCategoryFeilds.CAT_ID = Int32.Parse(lblAccessoryId.Content.ToString());
                System.Data.DataTable table = _clsCategory.SelectCategoryAssosory().Tables[0];

                LayoutCategoryAccesory.RowDefinitions.Clear();
                Int32 rowID = 0;
                Int32 gridRowCount = 0;
                foreach (System.Data.DataRow dr in table.Rows)
                {
                    CheckBox chkBox = new CheckBox();
                    chkBox.Name = "chkBox_" + dr["ACC_ID"].ToString();
                    chkBox.Content = dr["ACC_NAME"].ToString();
                    chkBox.IsChecked = dr["IS_CHECKED"].ToString() == "1" ? true : false;
                    chkBox.Checked += new RoutedEventHandler(CheckBox_Checked);
                    chkBox.Unchecked += new RoutedEventHandler(CheckBox_Checked);
                    int modValue = rowID % 3;
                    gridRowCount = ((rowID - modValue) / 3);
                    switch (modValue)
                    {
                        case 1:
                            chkBox.SetValue(Grid.ColumnProperty, 1);
                            chkBox.SetValue(Grid.RowProperty, gridRowCount);
                            break;
                        case 2:
                            chkBox.SetValue(Grid.ColumnProperty, 2);
                            chkBox.SetValue(Grid.RowProperty, gridRowCount);
                            break;
                        default:
                            LayoutCategoryAccesory.RowDefinitions.Add(new RowDefinition());
                            chkBox.SetValue(Grid.ColumnProperty, 0);
                            chkBox.SetValue(Grid.RowProperty, gridRowCount);
                            break;
                    }
                    
                    LayoutCategoryAccesory.Children.Add(chkBox);
                    rowID += 1;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ClassCategoryDA _ClsCategory = new ClassCategoryDA();
                CheckBox chk = (CheckBox)sender;
                Int32 AccesoryID = Int32.Parse(chk.Name.ToString().Replace("chkBox_", ""));
                _ClsCategory.classCategoryFeilds.ACC_ID = AccesoryID;
                _ClsCategory.classCategoryFeilds.CAT_ID = Int32.Parse(lblAccessoryId.Content.ToString());
                _ClsCategory.classCategoryFeilds.SAVE_DELETE_FLG = chk.IsChecked.Value ? 1 : 0 ;
                _ClsCategory.SaveDeleteCategoryAssosory();
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
