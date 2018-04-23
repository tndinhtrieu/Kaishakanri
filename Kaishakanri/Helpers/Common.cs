using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Kaishakanri.Helpers
{
    public static class Common
    {



        public static string ConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["KaishakanriDBEntities"].ConnectionString;
 
    }
}
