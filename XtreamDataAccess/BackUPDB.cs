using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class BackUPDB : XtreamConectionString
    {
        public void TakeDBBackUP(string filePath)
        {
            try
            {
                // read connectionstring from config file
                var connectionString = getConnetctionString;

                // read backup folder from config file ("C:/temp/")
                var backupFolder = filePath;

                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

                // set backupfilename (you will get something like: "C:/temp/MyDatabase-2013-12-07.bak")
                var backupFileName = String.Format("{0}/{1}-{2}.bak",
                    backupFolder, sqlConStrBuilder.InitialCatalog,
                    DateTime.Now.ToString("yyyy-MM-dd"));

                //if (System.IO.File.Exists(backupFileName.ToString()))
                //{
                //    System.IO.File.Delete(backupFileName.ToString());
                //}

                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT",
                        sqlConStrBuilder.InitialCatalog, backupFileName);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetRestoreTables()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_BACKUP_TABLES", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetIncentiveInfo(DateTime INCENTIVE_DATE, Double INCENTIVE_MARGINE, Double INCENTIVE_PERCENTAGE)
        {
            try
            {
                object[] parameterValues = { INCENTIVE_DATE, INCENTIVE_MARGINE, INCENTIVE_PERCENTAGE };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_INCENTIVE_INFO", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}
