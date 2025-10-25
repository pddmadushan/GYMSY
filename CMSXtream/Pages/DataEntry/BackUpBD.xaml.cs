using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for BackUpBD.xaml
    /// </summary>
    public partial class BackUpBD : UserControl
    {
        private BackgroundWorker bw = new BackgroundWorker();
        public BackUpBD()
        {
            InitializeComponent();
            try
            {
                string filePath = System.Configuration.ConfigurationManager.AppSettings.Get("BackUPFilePath").ToString();
                lblName.Content = filePath;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = System.Configuration.ConfigurationManager.AppSettings.Get("BackUPFilePath").ToString();

                if (!(Directory.Exists(filePath)))
                {
                    MessageBox.Show("Backup file path has not been configured correctly. Check configuration file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
                else
                {
                    bw.WorkerReportsProgress = true;

                    bw.DoWork -= bw_DoWork;
                    bw.DoWork += bw_DoWork;
                    bw.ProgressChanged += bw_ProgressChanged;
                    bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                    bw.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                pgrsBar.Value = 0;
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = System.Configuration.ConfigurationManager.AppSettings.Get("BackUPFilePath").ToString();
            BackUPDB _clsBackUp = new BackUPDB();

            DataSet backUpDataSet = new DataSet();
            backUpDataSet = _clsBackUp.GetRestoreTables();

            var backupFileName = String.Format("{0}/{1}-{2}.xls", filePath, "LINDSIS_BACKUP", DateTime.Now.ToString("yyyy-MM-dd"));

            if (System.IO.File.Exists(backupFileName.ToString()))
            {
                System.IO.File.Delete(backupFileName.ToString());
            }
            ExportToExcel(backUpDataSet, backupFileName);
        }

        private void Button_ClickOld(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = System.Configuration.ConfigurationManager.AppSettings.Get("BackUPFilePath").ToString();

                if (!(Directory.Exists(filePath)))
                {
                    MessageBox.Show("Backup file path has not been configured correctly. Check configuration file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    BackUPDB _clsBackUp = new BackUPDB();
                    _clsBackUp.TakeDBBackUP(filePath);
                    Mouse.OverrideCursor = null;
                    MessageBox.Show("Backup has been taken successfully!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    //pgrsBar.Value = 0;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pgrsBar.Value = 0;
            MessageBox.Show("Backup has been taken successfully!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgrsBar.Value = e.ProgressPercentage;
        }

        public void ExportToExcel(DataSet dataSet, string outputPath)
        {
            try
            {
                // Create the Excel Application object
                //ApplicationClass excelApp = new ApplicationClass();
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                // Create a new Excel Workbook
                Workbook excelWorkbook = xlApp.Workbooks.Add(Type.Missing);

                int sheetIndex = 0;
                int col = 0;
                int row = 0;
                Worksheet excelSheet = default(Worksheet);

                // Copy each DataTable as a new Sheet
                //pgrsBar.Minimum = 0;
                //pgrsBar.Maximum = dataSet.Tables.Count;
                //pgrsBar.Value = 0;
                int totalCount = dataSet.Tables.Count;
                int progressCount = 0;
                foreach (System.Data.DataTable dt in dataSet.Tables)
                {
                    sheetIndex += 1;
                    progressCount = (sheetIndex * 100 / totalCount);
                    bw.ReportProgress(progressCount);

                    // Copy the DataTable to an object array
                    object[,] rawData = new object[dt.Rows.Count + 1, dt.Columns.Count];

                    // Copy the column names to the first row of the object array
                    for (col = 0; col <= dt.Columns.Count - 1; col++)
                    {
                        rawData[0, col] = dt.Columns[col].ColumnName;
                    }

                    // Copy the values to the object array
                    for (col = 0; col <= dt.Columns.Count - 1; col++)
                    {
                        for (row = 0; row <= dt.Rows.Count - 1; row++)
                        {
                            rawData[row + 1, col] = dt.Rows[row].ItemArray[col];
                        }
                    }

                    // Calculate the final column letter
                    string finalColLetter = string.Empty;
                    string colCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int colCharsetLen = colCharset.Length;

                    if (dt.Columns.Count > colCharsetLen)
                    {
                        finalColLetter = colCharset.Substring((dt.Columns.Count - 1) / colCharsetLen - 1, 1);
                    }

                    finalColLetter += colCharset.Substring((dt.Columns.Count - 1) % colCharsetLen, 1);

                    // Create a new Sheet
                    excelSheet = (Worksheet)excelWorkbook.Sheets.Add(excelWorkbook.Sheets[sheetIndex], Type.Missing, 1, XlSheetType.xlWorksheet);

                    excelSheet.Name = dt.TableName;

                    // Fast data export to Excel
                    string excelRange = string.Format("A1:{0}{1}", finalColLetter, dt.Rows.Count + 1);
                    excelSheet.Range[excelRange, Type.Missing].Value2 = rawData;

                    // Mark the first row as BOLD
                    ((Range)excelSheet.Rows[1, Type.Missing]).Font.Bold = true;

                    excelSheet = null;
                }

                // Save and Close the Workbook
                excelWorkbook.SaveAs(outputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);

                excelWorkbook.Close(true, Type.Missing, Type.Missing);

                excelWorkbook = null;

                // Release the Application object
                xlApp.Quit();
                xlApp = null;

                // Collect the unreferenced objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }
    }
}
