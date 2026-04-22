using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace Main.Classes
{
    public class UserManager
    {
        
        // Register a new user
        public string RegisterUser(string firstName, string lastName, string email, string password)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

               
                string query = @"
                                INSERT INTO Users
                                (first_name, last_name, email_address, password_hash)
                                OUTPUT INSERTED.account_number
                                VALUES
                                (@first_name, @last_name, @email_address, @password)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Add parameters
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@email_address", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            return null;
        }

        // Checks if the user is in the registered users table
        public bool UserLogin(int pin, string password)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                string query = @"
                               SELECT COUNT(1)
                                 FROM Users
                                 WHERE account_number = @pin AND password_hash = @password
                               ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@pin", pin);
                    cmd.Parameters.AddWithValue("@password", password);

                    int matchCtr = Convert.ToInt32(cmd.ExecuteScalar());
                    return matchCtr > 0;
                }
            }
           
        } 
        
        // Check if email is unique
        public bool IsEmailUnique(string email)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                string query = "SELECT COUNT(1) FROM Users WHERE email_address = @Email";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();

                    // If count is 0, email is wala nagamit
                    // If count is > 0, email is nagamit na
                    return count == 0;
                }
            }
        }
    }
}