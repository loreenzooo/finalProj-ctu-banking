using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Main
{
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void registerBtn_Click(object sender, EventArgs e)
        {
            string firstName = firstNameTxt.Text.Trim();
            string lastName = lastNameTxt.Text.Trim();
            string email = emailTxt.Text.Trim();
            string password = passwordTxt.Text.Trim();


        }
    }
}