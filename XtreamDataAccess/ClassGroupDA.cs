using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class ClassGroupAttribute
    {
        public Int32 CLS_ID { get; set; }
        public string CLS_NAME { get; set; }
        public int CLS_ACTIVE_FLG { get; set; }
        public double CLS_FEE { get; set; }
        public double CLS_ADMITION_AMT { get; set; }
        public int CLS_DAY { get; set; }
        public double CLS_TIME { get; set; }
        public string CLS_COMMENT { get; set; }
        public DateTime CLS_START_DATE { get; set; }
        public double CLS_DURATION { get; set; }
        public int CAT_ID { get; set; }
        public int IS_CLASS_FLG { get; set; }
        public Int32 TOTAL_NUMBER_OF_WEEK { get; set; }
    }

    public class ClassGroupDA : XtreamConectionString
    {
        public ClassGroupAttribute classFeilds;

        public ClassGroupDA()
        {
            classFeilds = new ClassGroupAttribute();
        }

        public DataSet SelectAllClass()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSGROUP_ALL", parameterValues);
        }
        public DataSet InsertClass(Int32 UPDATE_STUDENT_FEE)
        {
            object[] parameterValues = { classFeilds.CLS_ID, classFeilds.CLS_NAME, classFeilds.CLS_ACTIVE_FLG, classFeilds.CLS_FEE, classFeilds.CLS_ADMITION_AMT, classFeilds.CLS_DAY, classFeilds.CLS_TIME, classFeilds.CLS_COMMENT, classFeilds.CLS_START_DATE, classFeilds.CLS_DURATION, classFeilds.CAT_ID, classFeilds.IS_CLASS_FLG, classFeilds.TOTAL_NUMBER_OF_WEEK, UPDATE_STUDENT_FEE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_CLASS_GROUP", parameterValues);
        }
        public Int32 DeleteClass()
        {
            try
            {
                object[] parameterValues = { classFeilds.CLS_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CLASS_GROUP", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet TransferStudent(Int32 TRANSFER_CLS_ID)
        {
            object[] parameterValues = { classFeilds.CLS_ID, TRANSFER_CLS_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "TRASFER_STUDENT", parameterValues);
        }
    }
}
