using System;
using System.Web.UI;
using Main.Classes;

namespace Main
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (!int.TryParse(txtAccountNumber.Text.Trim(), out int accountNumber))
            {
                lblError.Text = "Please enter a valid numeric Account Number.";
                lblError.Visible = true;
                return;
            }

            string password = txtPassword.Text.Trim();

            UserManager userManager = new UserManager();
            bool loginSuccess = userManager.UserLogin(accountNumber, password);

            if (loginSuccess)
            {
                Session["AccountNumber"] = accountNumber;
                Response.Redirect("Index.aspx");
            }
            else
            {
                lblError.Text = "Invalid Account Number or Password. Please try again.";
                lblError.Visible = true;
            }
        }

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }
    }
}