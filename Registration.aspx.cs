using Main.Classes;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Main
{
    public partial class Registration : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Email_ServerValidate(object source, ServerValidateEventArgs args)
        {
            UserManager userManager = new UserManager();
            args.IsValid = userManager.IsEmailUnique(txtEmail.Text.Trim());
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            UserManager userManager = new UserManager();
            string newAccountNumber = userManager.RegisterUser(firstName, lastName, email, password);

            if (newAccountNumber != null)
            {
                lblAccountNumber.Text = newAccountNumber;
                pnlRegistration.Visible = false;
                pnlSuccess.Visible = true;
            }
        }
    }
}