using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class ResonDA : XtreamConectionString
    {
        public int RSN_ID { get; set; }
        public string RSN_DES { get; set; }

        public DataSet SelectReason()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_REASON", parameterValues);
        }

        public Int32 SaveReason()
        {
            try
            {
                object[] parameterValues = {RSN_ID, RSN_DES };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_REASON", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 DeleteReason()
        {
            try
            {
                object[] parameterValues = { RSN_ID};
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_REASON", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}
