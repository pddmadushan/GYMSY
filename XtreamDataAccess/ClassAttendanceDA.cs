using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class ClassAttendanceDA : XtreamConectionString
    {
         public DateTime CLS_REC_DATE { get; set; } 
         public Int32 CLS_ID { get; set; } 
         public DataSet SelectNextWeek()
        {
            object[] parameterValues = { CLS_ID , CLS_REC_DATE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_WEEK_FOR_CLASS", parameterValues);
        }
         public DataSet ClassHistory()
         {
             object[] parameterValues = { CLS_ID};
             return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASS_HISTORY", parameterValues);
         }

         public DataSet UpdateStudentNote(Int32 STD_ID)
         {
             try
             {
                 object[] parameterValues = { STD_ID};
                 return SqlHelper.ExecuteDataset(getConnetctionString, "UPDATE_STUDENT_NOTE", parameterValues);
             }
             catch
             {
                 throw;
             }
         }
        
         public Int32[] GetClassAttendanceCount()
         {
             try
             {
                 Int32[] integer = {0,0,0,0};
                 object[] parameterValues = { CLS_ID, CLS_REC_DATE };
                 DataSet ds =  SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASS_STUDENT_COUNT", parameterValues);
                 integer[0] = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
                 integer[1] = Int32.Parse(ds.Tables[1].Rows[0][0].ToString());
                 integer[2] = Int32.Parse(ds.Tables[2].Rows[0][0].ToString());
                 integer[3] = Int32.Parse(ds.Tables[2].Rows[0][1].ToString());
                 return integer;
             }
             catch
             {
                 throw;
             }
         }

         public DataSet CheckClassHeldFlag()
         {
             try
             {
                 object[] parameterValues = { CLS_ID, CLS_REC_DATE };
                 return SqlHelper.ExecuteDataset(getConnetctionString, "CHECK_CLASS_IS_HELD", parameterValues);
             }
             catch
             {
                 throw;
             }
         }
         public Int32 DeleteClassAttendance()
         {
             try
             {
                 object[] parameterValues = {CLS_ID, CLS_REC_DATE};
                 return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CLASS_RECORD", parameterValues);
             }
             catch
             {
                 throw;
             }
         }

         public int STD_ID { get; set; }
         public int IS_INACTIVE_STD { get; set; }
         public Int32 RemoveStudentFromClass()
         {
             try
             {
                 object[] parameterValues = { STD_ID, CLS_ID, CLS_REC_DATE, IS_INACTIVE_STD };
                 return SqlHelper.ExecuteNonQuery(getConnetctionString, "REMOVE_STUDENT_FROM_CLASS", parameterValues);
             }
             catch
             {
                 throw;
             }
         }
    }
}
