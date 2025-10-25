using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class SMSDA : XtreamConectionString
    {
        public DataSet Option01Student(Int32 CLS_FILTER_ID, string CLS_FILTER_TEXT)
        {
            object[] parameterValues = { CLS_FILTER_ID, CLS_FILTER_TEXT };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_01_SMS", parameterValues);
        }

        public DataSet Option02Student(DateTime CLS_FROM, DateTime CLS_TO)
        {
            object[] parameterValues = { CLS_FROM, CLS_TO };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_02_SMS", parameterValues);
        }

        public DataSet Option03Student(DateTime CLS_FROM, DateTime CLS_TO)
        {
            object[] parameterValues = { CLS_FROM, CLS_TO };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_03_SMS", parameterValues);
        }

        public DataSet Option04Student(DateTime CLS_REC_DATE, DateTime TO_CLS_REC_DATE)
        {
            object[] parameterValues = { CLS_REC_DATE, TO_CLS_REC_DATE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_04_SMS", parameterValues);
        }

        public DataSet Option05Student(Int32 RSN_ID, Int32 STD_INACTIVE_FLG)
        {
            object[] parameterValues = { RSN_ID, STD_INACTIVE_FLG };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_05_SMS", parameterValues);
        }

        public DataSet SelectClassFromAttendance(DateTime CLS_REC_DATE)
        {
            object[] parameterValues = { CLS_REC_DATE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSES_FROM_ATTENDANCE", parameterValues);
        }
        public DataSet Option06Student()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_06_SMS", parameterValues);
        }
        public void SMSList(DataTable SMSContactList)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "INSERT_SMS_STUDENT";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSContactList", SMSContactList));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        cmdBulk.ExecuteNonQuery();
                        cmdBulk.Dispose();
                        trn.Commit();
                        trn.Dispose();
                    }
                    catch
                    {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public Int32 DeletePendingSMS()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_PENDINGSMS", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeletePendingStudentSMS(Int32 INBOX_ID, Int32 STD_ID)
        {
            try
            {
                object[] parameterValues = { INBOX_ID, STD_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_STUDENTPENDINGSMS", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetInactiveUsers()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_TOBE_INACTIVELIST", parameterValues);

        }

        public DataSet GetUsersFP(String BADGENUMBER)
        {
            object[] parameterValues = {BADGENUMBER};
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_USER_FP", parameterValues);

        }

        public DataSet GetUpdatebleUsers()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_UPDATEBLE_MEMBERS", parameterValues);

        }

        public Int32 SendPendinPaymentSMS()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SEND_DAILY_SMS", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public void SaveUser(DataTable UserList)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "SYC_ATTMACHINE_USER";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@UsreList", UserList));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        cmdBulk.ExecuteNonQuery();
                        cmdBulk.Dispose();
                        trn.Commit();
                        trn.Dispose();
                    }
                    catch
                    {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void SaveAttLog(DataTable AttendanceList)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "INSERT_ATENDANCE_LOG";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ATTLogList", AttendanceList));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        cmdBulk.ExecuteNonQuery();
                        cmdBulk.Dispose();
                        trn.Commit();
                        trn.Dispose();
                    }
                    catch
                    {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public Int32 InsertHistoryPf(string BADGENUMBER, Int32 FINGER_INDEX, string sFPTmpData, Int32 iFlag, Int32 iFPTmpLength)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER, FINGER_INDEX, sFPTmpData, iFlag, iFPTmpLength };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_DELETED_FP", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateLastFlashDate()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FLASH_MACHINE_DATE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateLastFlashDate2()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FLASH_MACHINE_DATE2", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 InsertFringerPrintDelFlg(string BADGENUMBER, string CARD_NUMBER, string PASS_WORD)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER, CARD_NUMBER , PASS_WORD };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FP_DELETE_FLG", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 InsertFringerPrintDelFlgWithoutSMS(string BADGENUMBER, string CARD_NUMBER, string PASS_WORD)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER, CARD_NUMBER, PASS_WORD };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FP_DELETE_FLG_WITHOUT_SMS", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateCardPassword(string BADGENUMBER, string CARD_NUMBER, string PASS_WORD)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER, CARD_NUMBER, PASS_WORD };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_CARD_PASSWORD", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 InsertFringerPrintInactive(string BADGENUMBER)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FP_DEL_FLG_INACTIVE2", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 ActiveFringerPrintDelFlg(string BADGENUMBER)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FP_DEL_FLG_ACTIVE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 ActiveFringerPrintDelFlg2(string BADGENUMBER)
        {
            try
            {
                object[] parameterValues = { BADGENUMBER };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_FP_DEL_FLG_ACTIVE2", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Boolean CanMachineFlush()
        {
            Boolean returnValue = false;
            try
            {
                object[] parameterValues = null;
                DataSet dt = SqlHelper.ExecuteDataset(getConnetctionString, "GET_CAN_FLASH_MACHINE", parameterValues);
                if (dt.Tables[0].Rows.Count > 0)
                {
                    if (dt.Tables[0].Rows[0][0].ToString() == "1")
                    {
                        returnValue = true;
                    }
                }
                return returnValue;
            }
            catch
            {
                throw;
            }
        }

        public Boolean CanMachineFlush2()
        {
            Boolean returnValue = false;
            try
            {
                object[] parameterValues = null;
                DataSet dt = SqlHelper.ExecuteDataset(getConnetctionString, "GET_CAN_FLASH_MACHINE2", parameterValues);
                if (dt.Tables[0].Rows.Count > 0)
                {
                    if (dt.Tables[0].Rows[0][0].ToString() == "1")
                    {
                        returnValue = true;
                    }
                }
                return returnValue;
            }
            catch
            {
                throw;
            }
        }

    }
}
