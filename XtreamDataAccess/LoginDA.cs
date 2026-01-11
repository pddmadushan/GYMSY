using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class LoginDA : XtreamConectionString
    {
        public string CLS_USER_ID { get; set; }

        private string UserPassword; // This is the backing field
        public string CLS_USER_PASSWORD
        {
            get { return GenarateXtreamHash(UserPassword); }
            set { UserPassword = value; }
        }
        public Int32 CLS_USER_ACTIVE { get; set; }

        public DataSet GetUserDetails()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SYS_LOGIN_AUTONTYCATION", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectUserAccount()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_USER_ACCOUNT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SaveUser()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_PASSWORD };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_USER_ACCOUNT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateActiveStatus()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_ACTIVE };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_USER_ACTIVE_LOCK", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePassword()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_PASSWORD };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_USER_PASSWORD", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public string GenarateXtreamHash(string password)
        {
            string md5data = Hash(password);
            md5data = Hash(string.Format("$LINDSIS$V1${0}@XTreamSoft", md5data));
            return md5data;
        }

        private string Hash(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public DataTable GetTenantLicensesInfo()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteDataset(getConnetctionString, "sp_GetAllTenantLicenses", parameterValues).Tables[0];
            }
            catch
            {
                throw;
            }
        }
        public void SaveTenantLicenses(string TenentID, string LicenseKeyHash,Int32 Amount , string AmountUpdate)
        {
            try
            {
                object[] parameterValues = { TenentID, LicenseKeyHash, Amount , AmountUpdate };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "sp_UpdateTenantLicense", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}
