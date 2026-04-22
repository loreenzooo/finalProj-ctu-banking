using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Main.Classes;


namespace Main.Classes
{
    public class DashboardManager
    {
        public DataTable GetRegisteredUsers()
        {
            DataTable dtUsers = new DataTable();

            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string query = @"
                                SELECT
                               account_number,
                                first_name,
                                middle_initial,
                                last_name,
                                email_address,
                                date_registered,
                                balance
                                FROM Users
                                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dtUsers);
                    }
                }
            }
            return dtUsers;
        }
    }
}