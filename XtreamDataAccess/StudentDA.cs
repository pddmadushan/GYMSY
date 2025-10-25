using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{

    public class StudentAttribute
    {
        public int STD_ID { get; set; }
        public int ATT_ID { get; set; }
        public int CLS_ID { get; set; }
        public string STD_INITIALS { get; set; }
        public string STD_SURNAME { get; set; }
        public string STD_FULL_NAME { get; set; }
        public int STD_GENDER { get; set; }
        public DateTime STD_DATEOFBIRTH { get; set; }
        public DateTime STD_JOIN_DATE { get; set; }
        public string STD_EMAIL { get; set; }
        public string STD_NIC { get; set; }
        public string STD_ADDRESS { get; set; }
        public double STD_CLASS_FEE { get; set; }
        public string STD_TELEPHONE { get; set; }
        public int STD_ACTIVE_FLG { get; set; }
        public string STD_COMMENT { get; set; }
        public string STD_TEMP_NOTE { get; set; }
        public string CLS_NAME { get; set; }
        public Int32 RSN_ID { get; set; }
    }

    public class StudentDA : XtreamConectionString
    {
        public int STD_ID { get; set; }        
        public int CLS_ID { get; set; }
        public string STD_INITIALS { get; set; }
        public string STD_SURNAME { get; set; }
        public string STD_FULL_NAME { get; set; }
        public int STD_GENDER { get; set; }
        public DateTime STD_DATEOFBIRTH { get; set; }
        public DateTime STD_JOIN_DATE { get; set; }
        public string STD_EMAIL { get; set; }
        public string STD_NIC { get; set; }
        public string STD_ADDRESS { get; set; }
        public double STD_CLASS_FEE { get; set; }
        public string STD_TELEPHONE { get; set; }
        public string STD_TELEPHONE2 { get; set; }
        public string STD_TEMP_NOTE { get; set; }
        public int STD_ACTIVE_FLG { get; set; }
        public string STD_COMMENT { get; set; }
        public int CLS_WEEK { get; set; }
        public int CLS_PART { get; set; }
        public int RSN_ID { get; set; }
        public int STD_LAST_WEEK { get; set; }

        public DateTime CLS_REC_DATE { get; set; }
        public int CLS_HOLD_FLG { get; set; }
        public string CLS_COMMENT { get; set; }

        public string MODIFY_USER { get; set; }
        public int CLS_REC_ATT_FLG { get; set; }
        public DateTime STD_REC_DATE { get; set; }

        public int CLASS_YEAR { get; set; }
        public int CLASS_MONTH { get; set; }

        public int INTRODUCER_ID { get; set; }

        public DataSet SelectStudent()
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT", parameterValues);
        }     

        public DataSet SelectAllStudent(Int32 CLS_FILTER_ID, string CLS_FILTER_TEXT)
        {
            object[] parameterValues = { CLS_FILTER_ID, CLS_FILTER_TEXT };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_ALL", parameterValues);
        }
        public DataSet SelectAllClassGroup(Int32 CLS_FILTER_ID)
        {
            object[] parameterValues = { CLS_FILTER_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "ST_SELECT_CLASSGROUP_ALL", parameterValues);
        }
        public DataSet SelectAllClassForStudent(Int32 CLS_FILTER_ID, Int32 CLS_DAY)
        {
            object[] parameterValues = { CLS_FILTER_ID, CLS_DAY };
            return SqlHelper.ExecuteDataset(getConnetctionString, "ST_SELECT_CLASSGROUP_FILTER", parameterValues);
        }
        public DataSet SelectClassHistory()
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_CLASS_HISTORY", parameterValues);
        }        
        public DataSet InsertStudent()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, STD_INITIALS, STD_SURNAME, STD_FULL_NAME, STD_GENDER, STD_DATEOFBIRTH, STD_JOIN_DATE, STD_EMAIL, STD_NIC, STD_ADDRESS, STD_CLASS_FEE, STD_TELEPHONE, STD_ACTIVE_FLG, STD_COMMENT, STD_TEMP_NOTE, RSN_ID, STD_TELEPHONE2, MODIFY_USER, STD_LAST_WEEK };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_STUDENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 InsertClassAttendance()
        {
            try
            {
                object[] parameterValues = { CLS_ID, CLS_REC_DATE, CLS_HOLD_FLG, CLS_COMMENT, CLS_WEEK, CLS_PART };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_CLASS_ATTENDANCE", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet SelectClassAttendance()
        {
            try
            {
                object[] parameterValues = { CLS_ID, CLS_REC_DATE };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASS_ATTENDANCE", parameterValues);
            }
            catch
            {
                throw;
            }
        }       
        public int UpdateStudentAttendence()
        {
            try
            {
                object[] parameterValues = { CLS_ID, STD_REC_DATE, STD_ID, MODIFY_USER, CLS_REC_ATT_FLG };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "ST_UPDATE_ATTENDANCE", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public string SelectClassHoldFlg()
        {
            try
            {
                object[] parameterValues = { CLS_ID, CLS_REC_DATE};
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASS_HOLD_FLG", parameterValues).Tables[0].Rows[0]["CLS_HOLD_FLG"].ToString();
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeleteStudent()
        {
            try
            {
                object[] parameterValues = { STD_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_STUDENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet SelectStudentAccesory()
        {
            try
            {
                object[] parameterValues = { STD_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet StudentClassAttendance()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, CLASS_YEAR, CLASS_MONTH };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_ATT_HISTORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectStudentMovement()
        {
            try
            {
                object[] parameterValues = { STD_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_MOVEMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetStudentNextPayment()
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_NAXT_PAYMENT_INFO ", parameterValues);
        }

        public Int32 AddStudentToClass()
        {
            try
            {
                object[] parameterValues = { STD_ID,CLS_REC_DATE, MODIFY_USER };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "ADD_STUDENT_TO_CLASS", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet ValidateNicandTeliphone()
        {
            object[] parameterValues = { STD_ID, STD_NIC,STD_TELEPHONE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "CHECK_STUDENT_NIC_TEL", parameterValues);
        }

        public DataSet SelectStudentGroup()
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_CLASS_GROUP", parameterValues);
        }

        public DataSet SelectIntroduced()
        {
            object[] parameterValues = { INTRODUCER_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_INTRODUCED_STUDENT", parameterValues);
        }
        
        public Int32 InsertStudentGroup()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, STD_CLASS_FEE  };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_STUDENT_CLASS_GROUP", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeleteStudentGroup()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_STUDENT_CLASS_GROUP", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectSMSHISTORY()
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_SMS_HISTORY", parameterValues);
        
        }

        public DataSet SelectStudentDetails()
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_INFOR", parameterValues);

        }
        public Int32 AddStudentToIntroducer()
        {
            try
            {
                object[] parameterValues = { STD_ID, INTRODUCER_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_INTRODUCER", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet SelectIntroducer()
        {
            try
            {
                object[] parameterValues = { STD_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_INTRODUCER", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}