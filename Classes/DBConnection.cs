using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;

namespace Main.Classes
{
    public class DBConnection
    {
        public static SqlConnection GetConnection()
        {
            string conn = WebConfigurationManager.ConnectionStrings["BankingSystemDB"].ConnectionString;
            return new SqlConnection(conn);
        }
    }
}