using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream.Control
{
    public class StudentList
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
        public int STD_ACTIVE_FLG { get; set; }
        public string STD_COMMENT { get; set; }
    }
}
