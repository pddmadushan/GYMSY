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
    /// Interaction logic for AddClassGroup.xaml
    /// </summary>
    public partial class AddClassGroup : UserControl
    {
        public Int32 stdID { get; set; }
        public Int32 clsID { get; set; }
        public String OutResult { get; set; }
        public AddClassGroup()
        {
            InitializeComponent();
        }
        public void LoadClassCombo()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassGroup(-1).Tables[0];
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    ClassGroups.Add(new ClassGroup(Int32.Parse(row["CLS_ID"].ToString()), row["CLS_NAME"].ToString(), double.Parse(row["CLS_FEE"].ToString())));
                }
                cmbClsGroup.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbClsGroup.ItemsSource = ClassGroups;
                cmbClsGroup.DisplayMemberPath = "CLS_NAME";
                cmbClsGroup.SelectedValuePath = "CLS_ID";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void cmbClsGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                ClassGroups = (List<ClassGroup>)cmbClsGroup.ItemsSource;
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    ClassGroup result = ClassGroups.Find(x => x.CLS_ID == Int32.Parse(cmbClsGroup.SelectedValue.ToString()));
                    if (result != null)
                    {
                        txtClsFee.Text = result.CLS_FEE.ToString();
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 clessID = int.Parse(cmbClsGroup.SelectedValue.ToString());

                if (clsID == clessID)
                {
                    MessageBox.Show("Student cannot have same class twice!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                StudentDA _clsStudent = new StudentDA();

                _clsStudent.STD_ID = stdID;
                _clsStudent.CLS_ID = int.Parse(cmbClsGroup.SelectedValue.ToString());
                _clsStudent.STD_CLASS_FEE = double.Parse(txtClsFee.Text.Trim());

                _clsStudent.InsertStudentGroup();

                OutResult = "Records successfully updated";
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
}
