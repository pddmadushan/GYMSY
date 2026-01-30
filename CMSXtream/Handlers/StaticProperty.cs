using StandaloneSDKDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream
{
    public class StaticProperty
    {
        public static string ClientName;
        public static string Clientlicense;
        public static string SaveMessage = "Record has been saved successfully!";
        public static string UnabletoSaveMessage = "Unable to save record !";
        public static string LoginUserID;
        public static string LoginisAdmin;
        public static SDKHelper SDK = new SDKHelper(true);
        public static SDKHelper SDK_2 = new SDKHelper(1);

        public static string SKTPortName = null;
        public static string SKTIPAddres = null;
        public static string SKTCommKey;

        public static string SKTIPAddres_2 = null;

    }
}
