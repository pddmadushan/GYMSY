using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class AddTuteDA : XtreamConectionString
    {
        public Int32 ACC_ID { get; set; }
        public Int32 CLS_ID { get; set; }
        public Int32 STD_ID { get; set; }
        public DateTime CLS_REC_DATE { get; set; }
        public double ACC_AMOUNT { get; set; }

        public Int32 SaveClassAccessory()
        {
            try
            {
                object[] parameterValues = { CLS_ID, ACC_ID, CLS_REC_DATE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_CLASS_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 DeleteClassAccessory()
        {
            try
            {
                object[] parameterValues = { CLS_ID, ACC_ID, CLS_REC_DATE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CLASS_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectAccessoryStudent()
        {
            try
            {
                object[] parameterValues = { CLS_ID, ACC_ID, CLS_REC_DATE };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_ACCESSORY_STUDENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectClassAccessory()
        {
            try
            {
                object[] parameterValues = { CLS_ID};
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASS_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        
        public Int32 DeleteStudentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, CLS_REC_DATE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_STUDENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

    }
}
