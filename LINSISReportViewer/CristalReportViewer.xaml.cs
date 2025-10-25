using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
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
using System.Windows.Shapes;

namespace LINSISReportViewer
{
    /// <summary>
    /// Interaction logic for CristalReportViewer.xaml
    /// </summary>
    public partial class CristalReportViewer : Window
    {
        public string reportName { get; set; }
        public string path { get; set; }
        public CristalReportViewer()
        {
            InitializeComponent();
            //LoadReportFileTest();
        }
        public void LoadReportFile()
        {
            try
            {
                string fileFullPath = string.Empty;
                fileFullPath = path + "\\" + reportName;
                if (File.Exists(fileFullPath))
                {
                    XtreamDataAccess.XtreamConectionString conInfo = new XtreamDataAccess.XtreamConectionString();
                    System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(conInfo.getConnetctionString);
                    // Retrieve the DataSource property.    
                    string serverName = builder.DataSource;
                    string databaseName = builder.InitialCatalog;
                    string userName = builder.UserID;
                    string password = builder.Password;

                    ReportDocument reportDoc = new ReportDocument();
                    reportDoc.Load(fileFullPath);

                    CrystalDecisions.CrystalReports.Engine.Database crDatabase;
                    CrystalDecisions.CrystalReports.Engine.Tables crTables;
                    TableLogOnInfo crTableLogOnInfo;
                    CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;

                    crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                    crConnectionInfo.ServerName = serverName;
                    crConnectionInfo.DatabaseName = databaseName;
                    crConnectionInfo.UserID = userName;
                    crConnectionInfo.Password = password;

                    crDatabase = reportDoc.Database;
                    crTables = crDatabase.Tables;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table crTable in crTables)
                    {
                        crTableLogOnInfo = crTable.LogOnInfo;
                        crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
                        crTable.ApplyLogOnInfo(crTableLogOnInfo);
                    }

                    //Set locate Subroports
                    //foreach(ReportDocument subReport in reportDoc.Subreports)
                    //{
                    //    crDatabase = subReport.Database;
                    //    crTables = crDatabase.Tables;
                    //    foreach (CrystalDecisions.CrystalReports.Engine.Table crTable in crTables)
                    //    {
                    //        crTableLogOnInfo = crTable.LogOnInfo;
                    //        crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
                    //        crTable.ApplyLogOnInfo(crTableLogOnInfo);
                    //    }
                    //}

                    cryRptViewer.ViewerCore.ReportSource = reportDoc;
                }
                else
                {
                    MessageBox.Show("File path or name has been changed");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void LoadReportFileTest()
        {
            try
            {
                string path = @"D:\LINDSIS Xtream\CMSXtream\CMSXtream\bin\Debug\ReportFile\Cash Flow.rpt";
                string fileFullPath = path;
                if (File.Exists(fileFullPath))
                {
                    // XtreamDataAccess.XtreamConectionString conInfo = new XtreamDataAccess.XtreamConectionString();
                    //System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(conInfo.getConnetctionString);
                    // Retrieve the DataSource property.    
                    string serverName = "LP186";
                    string databaseName = "CLSSIS";
                    string userName = "CLSSIS";
                    string password = "CLSSIS";

                    ReportDocument reportDoc = new ReportDocument();
                    reportDoc.Load(fileFullPath);

                    CrystalDecisions.CrystalReports.Engine.Database crDatabase;
                    CrystalDecisions.CrystalReports.Engine.Tables crTables;
                    TableLogOnInfo crTableLogOnInfo;
                    CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;

                    crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                    crConnectionInfo.ServerName = serverName;
                    crConnectionInfo.DatabaseName = databaseName;
                    crConnectionInfo.UserID = userName;
                    crConnectionInfo.Password = password;

                    crDatabase = reportDoc.Database;
                    crTables = crDatabase.Tables;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table crTable in crTables)
                    {
                        crTableLogOnInfo = crTable.LogOnInfo;
                        crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
                        crTable.ApplyLogOnInfo(crTableLogOnInfo);
                    }
                    cryRptViewer.ViewerCore.ReportSource = reportDoc;
                }
                else
                {
                    MessageBox.Show("File path or name has been changed");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
