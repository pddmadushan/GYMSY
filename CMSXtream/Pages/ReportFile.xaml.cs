using System;
using System.Collections.Generic;
using System.IO;
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

namespace CMSXtream.Pages
{
    /// <summary>
    /// Interaction logic for ReportFile.xaml
    /// </summary>
    public partial class ReportFile : UserControl
    {
        public ReportFile()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void LoadGrid()
        {
            try
            {

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("REPORT_FILE_NAME", typeof(string));
                table.Columns.Add("REPORT_NAME", typeof(string));
                table.Columns.Add("REPORT_PATH", typeof(string));

                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                path = path + "\\ReportFile";

                if (!(Directory.Exists(path)))
                {
                    MessageBox.Show("Report folder is not exist! Path :" + path , StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
                else
                {
                    DirectoryInfo d = new DirectoryInfo(path);
                    FileInfo[] Files = d.GetFiles("*.rpt");
                    foreach (FileInfo file in Files)
                    {
                        table.Rows.Add(file.Name, file.Name.Replace(".rpt", ""), path);
                    }

                    if (StaticProperty.LoginisAdmin == "1")
                    {
                        path = path + "\\Admin";
                        DirectoryInfo dadmin = new DirectoryInfo(path);
                        FileInfo[] AdminFiles = dadmin.GetFiles("*.rpt");
                        foreach (FileInfo file in AdminFiles)
                        {
                            table.Rows.Add(file.Name, file.Name.Replace(".rpt", ""), path);
                        }
                    }

                    if (table.Rows.Count > 0)
                    {
                        grdReports.ItemsSource = table.DefaultView;
                    }
                    else
                    {
                        grdReports.ItemsSource = null;
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

        private void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdReports.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    LINSISReportViewer.CristalReportViewer rptVeiwer = new  LINSISReportViewer.CristalReportViewer();
                    rptVeiwer.Title = selectedRow["REPORT_NAME"].ToString();
                    rptVeiwer.reportName = selectedRow["REPORT_FILE_NAME"].ToString();
                    rptVeiwer.path = selectedRow["REPORT_PATH"].ToString();
                    rptVeiwer.LoadReportFile();
                    rptVeiwer.Show();
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }
    }
}
