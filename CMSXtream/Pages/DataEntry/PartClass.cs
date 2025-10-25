using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream.Pages.DataEntry
{
    class PartClass
    {
            public int PART_ID { get; set; }
            public string PART_NAME { get; set; }

            public PartClass(int id, string name)
            {
                this.PART_ID = id;
                this.PART_NAME = name;
            }
    }
}
