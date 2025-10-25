using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream.Pages.DataEntry
{
    public class ClassGroup
    {
        public int CLS_ID { get; set; }
        public string CLS_NAME { get; set; }
        public double CLS_FEE { get; set; }

        public ClassGroup(int id, string name, double amount)
        {
            this.CLS_ID = id;
            this.CLS_NAME = name;
            this.CLS_FEE = amount;
        }
    }
}
