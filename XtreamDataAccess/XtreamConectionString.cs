using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtreamDataAccess
{
    public class XtreamConectionString
    {
        public XtreamConectionString()
        {
            _connection = System.Configuration.ConfigurationManager.ConnectionStrings["CMSXtreamConnection"].ToString(); 
        }
        private string _connection;
        public string getConnetctionString
        {
            get { return this._connection; }
        }
    }
}
