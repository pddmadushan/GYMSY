using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class DoPaymentDA : XtreamConectionString
    {
        public Int32 STD_ID { get; set; }
        public Int32 CLS_ID { get; set; }
        public int PAID_YEAR { get; set; }
        public int PAID_MONTH { get; set; }
        public string MODIFY_USER { get; set; }
        public double PAID_AMOUNT { get; set; }
        public DateTime STD_REC_DATE { get; set; }
        public int CARD_ISSUED_FLG { get; set; }
        public double CLASS_FEE { get; set; }
        public Int32 PAID_FOR_MONTHS { get; set; }
        public DateTime VALID_DUE_DATE { get; set; }
        public DateTime ACTUAL_PAID_DATE { get; set; }

        public Int32 ACC_ID { get; set; }
        public DateTime ASS_REC_DATE { get; set; }
        public DateTime PMT_REC_DATE { get; set; }
        public double ASS_REC_AMOUNT { get; set; }
        public DateTime MACHIN_DATE { get; set; }

        public DataSet SelectPayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdatePayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, MODIFY_USER, PAID_AMOUNT, STD_REC_DATE, CLASS_FEE, CARD_ISSUED_FLG, PAID_FOR_MONTHS, VALID_DUE_DATE, ACTUAL_PAID_DATE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateCardIssued()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, CARD_ISSUED_FLG };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_CARD_ISSUED", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateClassFee()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, CLASS_FEE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_CLASS_FEE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 DeleteClassFee()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CLASS_FEE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectPaymentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_ACCESSORY_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 UpdatePaymentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, PMT_REC_DATE, MODIFY_USER, PAID_AMOUNT, ASS_REC_AMOUNT };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_PAYMENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 UpdateStudentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, ASS_REC_AMOUNT };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_STUDENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 UpdateAccessoryAmount()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, ASS_REC_AMOUNT };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_STUDENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeletePayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH,STD_REC_DATE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DETATE_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }
         public Int32 DeletePaymentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, PMT_REC_DATE};
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_PAYMENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

         public string CanActive()
         {
             try
             {
                 object[] parameterValues = { STD_ID, MACHIN_DATE };
                 DataSet ds = SqlHelper.ExecuteDataset(getConnetctionString, "CAN_ACTIVATE_MEMBER", parameterValues);
                 return ds.Tables[0].Rows[0]["CAN_ACTIVE"].ToString();
             }
             catch
             {
                 throw;
             }
         }

    }
}
