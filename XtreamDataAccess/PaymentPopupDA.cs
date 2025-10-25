using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class PaymentPopupDA : XtreamConectionString
    {
        public int STD_ID { get; set; }
        public int CLS_ID { get; set; }
        public int PAID_YEAR { get; set; }
        public int PAID_MONTH { get; set; }
        public string MODIFY_USER { get; set; }
        public double PAID_AMOUNT { get; set; }
        public DateTime STD_REC_DATE { get; set; }
        public int CARD_ISSUED_FLG { get; set; }

        public DataSet SelectTop10Payment()
        {
            try
            {
                object[] parameterValues = { STD_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_PAYMENT_TOP10", parameterValues);
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
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, MODIFY_USER, PAID_AMOUNT, STD_REC_DATE, CARD_ISSUED_FLG };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

    }
}
