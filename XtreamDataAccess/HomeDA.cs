using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class HomeDA : XtreamConectionString
    {
        public DataSet SelectClassSchedule()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSSHEDULE", parameterValues);
        }

        //public void SyncAttendanceMachine()
        //{
        //    object[] parameterValues = null;
        //    SqlHelper.ExecuteNonQuery(getConnetctionString, "SYC_ATTMACHINE", parameterValues);
        //}
    }
}
