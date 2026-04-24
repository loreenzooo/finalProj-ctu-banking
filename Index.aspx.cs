using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Main.Classes;

namespace Main
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountNumber"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Always rebuild filter dropdown on every postback (prevents it from losing items)
            UpdateFilterState();

            if (!IsPostBack)
            {
                int currentAccountNo = Convert.ToInt32(Session["AccountNumber"]);
                UserManager userManager = new UserManager();

                string firstName = userManager.GetUserFirstName(currentAccountNo);
                lblUserName.Text = firstName;
                if (!string.IsNullOrEmpty(firstName))
                    lblProfileInitial.Text = firstName.Substring(0, 1).ToUpper();

                LoadDashboardStats();
                BindCurrentTable();
            }
        }

        // ==========================================
        // SIDEBAR NAVIGATION
        // ==========================================
        protected void Sidebar_Click(object sender, EventArgs e)
        {
            LinkButton clickedBtn = (LinkButton)sender;
            int viewIndex = int.Parse(clickedBtn.CommandArgument);

            mvMainContent.ActiveViewIndex = viewIndex;
            btnSidebarDashboard.CssClass = "menu-item";
            btnSidebarManage.CssClass = "menu-item";
            clickedBtn.CssClass = "menu-item active";

            if (viewIndex == 0) LoadDashboardStats();
        }

        protected void btnSidebarChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx");
        }

        protected void btnSidebarLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // ==========================================
        // DASHBOARD STATS
        // ==========================================
        private void LoadDashboardStats()
        {
            int currentAccountNo = Convert.ToInt32(Session["AccountNumber"]);
            UserManager userManager = new UserManager();
            TransactionManager txManager = new TransactionManager();

            string name, dateReg;
            userManager.GetDashboardDetails(currentAccountNo, out name, out dateReg);
            decimal balance = txManager.GetBalance(currentAccountNo);
            decimal totalSent = txManager.GetTotalSent(currentAccountNo);

            lblDashAccountNo.Text = currentAccountNo.ToString();
            lblDashName.Text = name;
            lblDashDateReg.Text = dateReg;
            lblDashBalance.Text = balance.ToString("N2");
            lblDashTotalSent.Text = totalSent.ToString("N2");
        }

        // ==========================================
        // ACTION PANELS (DEPOSIT / WITHDRAW / SEND)
        // ==========================================
        protected void Action_Click(object sender, EventArgs e)
        {
            Button clickedBtn = (Button)sender;
            int actionIndex = int.Parse(clickedBtn.CommandArgument);

            // Toggle: clicking the same button again closes the panel
            mvActions.ActiveViewIndex = (mvActions.ActiveViewIndex == actionIndex) ? -1 : actionIndex;

            // Load current balance when Withdraw panel is opened
            if (actionIndex == 1)
            {
                TransactionManager txManager = new TransactionManager();
                lblWithdrawBalance.Text = txManager.GetBalance(Convert.ToInt32(Session["AccountNumber"])).ToString("N2");
            }

            // Reset Send panel if navigating away from it
            if (actionIndex != 2)
                ResetSendPanel();
        }

        // Rules: Min ₱100, Max ₱2,000, divisible by 100
        private bool IsValidAmount(decimal amount)
        {
            return amount >= 100 && amount <= 2000 && (amount % 100 == 0);
        }

        // ==========================================
        // DEPOSIT
        // ==========================================
        protected void btnSubmitDeposit_Click(object sender, EventArgs e)
        {
            decimal amount;
            if (decimal.TryParse(txtDepositAmount.Text, out amount))
            {
                if (!IsValidAmount(amount))
                {
                    ShowAlert("Deposit must be between ₱100 and ₱2,000, and divisible by 100.");
                    return;
                }

                int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
                TransactionManager txManager = new TransactionManager();

                if (txManager.GetBalance(currentAccount) + amount > 10000)
                {
                    ShowAlert("Deposit failed. Maximum account balance cannot exceed ₱10,000.00.");
                    return;
                }

                string result = txManager.ProcessDeposit(currentAccount, amount);
                if (result == "Success")
                {
                    ShowAlert("Deposit successful!");
                    txtDepositAmount.Text = "";
                    mvActions.ActiveViewIndex = -1;
                    BindCurrentTable();
                }
                else ShowAlert(result);
            }
            else ShowAlert("Please enter a valid numeric amount.");
        }

        // ==========================================
        // WITHDRAW
        // ==========================================
        protected void btnSubmitWithdraw_Click(object sender, EventArgs e)
        {
            decimal amount;
            if (decimal.TryParse(txtWithdrawAmount.Text, out amount))
            {
                if (!IsValidAmount(amount))
                {
                    ShowAlert("Withdrawal must be between ₱100 and ₱2,000, and divisible by 100.");
                    return;
                }

                int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
                TransactionManager txManager = new TransactionManager();

                if (txManager.GetBalance(currentAccount) < amount)
                {
                    ShowAlert("Insufficient funds.");
                    return;
                }

                string result = txManager.ProcessWithdraw(currentAccount, amount);
                if (result == "Success")
                {
                    ShowAlert("Withdrawal successful!");
                    txtWithdrawAmount.Text = "";
                    mvActions.ActiveViewIndex = -1;
                    BindCurrentTable();
                }
                else ShowAlert(result);
            }
            else ShowAlert("Please enter a valid numeric amount.");
        }

        // ==========================================
        // SEND CLOUDMONEY
        // ==========================================

        // STEP 1: Verify receiver — shows Account No. and Name in separate fields
        protected void btnVerifyReceiver_Click(object sender, EventArgs e)
        {
            int receiverAccount;
            if (int.TryParse(txtSendAccount.Text, out receiverAccount))
            {
                int currentAccount = Convert.ToInt32(Session["AccountNumber"]);

                // Cannot send to yourself
                if (receiverAccount == currentAccount)
                {
                    ShowAlert("You cannot send money to yourself.");
                    return;
                }

                UserManager userManager = new UserManager();
                string receiverName = userManager.GetReceiverName(receiverAccount);

                if (!string.IsNullOrEmpty(receiverName))
                {
                    // Populate separate Account No. and Name labels
                    lblReceiverAccountNo.Text = receiverAccount.ToString();
                    lblReceiverName.Text = receiverName;

                    // Show info card + send form, hide verify input
                    pnlVerifyReceiver.Visible = false;
                    pnlReceiverInfo.Visible = true;
                    pnlSendMoneyForm.Visible = true;
                }
                else ShowAlert("Receiver account does not exist.");
            }
            else ShowAlert("Invalid Account Number format.");
        }

        protected void btnCancelSend_Click(object sender, EventArgs e)
        {
            ResetSendPanel();
        }

        // Centralised reset for the entire Send CloudMoney panel
        private void ResetSendPanel()
        {
            txtSendAccount.Text = "";
            txtSendAmount.Text = "";
            txtSendPassword.Text = "";
            lblReceiverAccountNo.Text = "";
            lblReceiverName.Text = "";
            pnlVerifyReceiver.Visible = true;
            pnlReceiverInfo.Visible = false;
            pnlSendMoneyForm.Visible = false;
        }

        // STEP 2: Submit send — validates all rules then processes transfer
        protected void btnSubmitSend_Click(object sender, EventArgs e)
        {
            decimal amount;
            if (decimal.TryParse(txtSendAmount.Text, out amount))
            {
                if (!IsValidAmount(amount))
                {
                    ShowAlert("Amount must be between ₱100 and ₱2,000, and divisible by 100.");
                    return;
                }

                int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
                int receiverAccount = Convert.ToInt32(txtSendAccount.Text);
                string password = txtSendPassword.Text;

                UserManager userManager = new UserManager();
                TransactionManager txManager = new TransactionManager();

                // Rule 1: Cannot send to yourself
                if (receiverAccount == currentAccount)
                {
                    ShowAlert("You cannot send money to yourself.");
                    return;
                }

                // Rule 2: Sender must have sufficient funds
                if (txManager.GetBalance(currentAccount) < amount)
                {
                    ShowAlert("Insufficient funds.");
                    return;
                }

                // Rule 3: Password verification
                if (!userManager.UserLogin(currentAccount, password))
                {
                    ShowAlert("Security Verification Failed: Incorrect password.");
                    return;
                }

                // Rule 4: Receiver balance cap
                if (txManager.GetBalance(receiverAccount) + amount > 10000)
                {
                    ShowAlert("Transfer failed. Recipient's balance would exceed ₱10,000.00.");
                    return;
                }

                string result = txManager.SendCloudMoney(currentAccount, receiverAccount, amount);
                if (result == "Success")
                {
                    ShowAlert("Money sent successfully!");
                    ResetSendPanel();
                    mvActions.ActiveViewIndex = -1;
                    BindCurrentTable();
                }
                else ShowAlert(result);
            }
            else ShowAlert("Please enter a valid amount.");
        }

        // ==========================================
        // DATA TABLES & FILTERING
        // ==========================================
        private void ShowAlert(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }

        protected void TableTab_Click(object sender, EventArgs e)
        {
            LinkButton clickedTab = (LinkButton)sender;
            mvTables.ActiveViewIndex = int.Parse(clickedTab.CommandArgument);

            btnTabStatement.CssClass = "tab-btn";
            btnTabDeposits.CssClass = "tab-btn";
            btnTabTransactions.CssClass = "tab-btn";
            clickedTab.CssClass = "tab-btn active";

            // Clear any date error when switching tabs
            lblDateError.Visible = false;
            lblDateError.Text = "";

            UpdateFilterState();
            BindCurrentTable();
        }

        private void UpdateFilterState()
        {
            ddlType.Items.Clear();
            ddlType.Items.Add(new ListItem("All", "All"));

            if (mvTables.ActiveViewIndex == 0)
            {
                lblFilterTitle.Text = "My Statement of Account";
            }
            else if (mvTables.ActiveViewIndex == 1)
            {
                lblFilterTitle.Text = "My Deposits or Withdrawals";
                ddlType.Items.Add(new ListItem("Deposit (D)", "D"));
                ddlType.Items.Add(new ListItem("Withdrawal (W)", "W"));
            }
            else if (mvTables.ActiveViewIndex == 2)
            {
                lblFilterTitle.Text = "My Sent or Received Transactions";
                ddlType.Items.Add(new ListItem("Sent", "Sent"));
                ddlType.Items.Add(new ListItem("Received", "Received"));
            }
        }

        // Date validation: no future dates, from must be <= to
        protected void btnList_Click(object sender, EventArgs e)
        {
            lblDateError.Visible = false;
            lblDateError.Text = "";

            DateTime today = DateTime.Today;
            bool hasFrom = !string.IsNullOrEmpty(txtFromDate.Text);
            bool hasTo = !string.IsNullOrEmpty(txtToDate.Text);
            DateTime fromDate, toDate;

            if (hasFrom && DateTime.TryParse(txtFromDate.Text, out fromDate))
            {
                if (fromDate > today)
                {
                    lblDateError.Text = "From date cannot be a future date.";
                    lblDateError.Visible = true;
                    return;
                }
            }

            if (hasTo && DateTime.TryParse(txtToDate.Text, out toDate))
            {
                if (toDate > today)
                {
                    lblDateError.Text = "To date cannot be a future date.";
                    lblDateError.Visible = true;
                    return;
                }
            }

            if (hasFrom && hasTo &&
                DateTime.TryParse(txtFromDate.Text, out fromDate) &&
                DateTime.TryParse(txtToDate.Text, out toDate))
            {
                if (fromDate > toDate)
                {
                    lblDateError.Text = "From date must be earlier than or equal to the To date.";
                    lblDateError.Visible = true;
                    return;
                }
            }

            BindCurrentTable();
        }

        private void BindCurrentTable()
        {
            if (Session["AccountNumber"] == null) return;

            int currentAccountNo = Convert.ToInt32(Session["AccountNumber"]);
            string fromDate = txtFromDate.Text;
            string toDate = txtToDate.Text;
            string type = ddlType.SelectedValue;

            TransactionManager txManager = new TransactionManager();

            if (mvTables.ActiveViewIndex == 0)
            {
                gvStatement.DataSource = txManager.GetStatement(currentAccountNo, fromDate, toDate);
                gvStatement.DataBind();
            }
            else if (mvTables.ActiveViewIndex == 1)
            {
                gvDeposits.DataSource = txManager.GetDepositsWithdrawals(currentAccountNo, fromDate, toDate, type);
                gvDeposits.DataBind();
            }
            else if (mvTables.ActiveViewIndex == 2)
            {
                gvTransactions.DataSource = txManager.GetTransfers(currentAccountNo, fromDate, toDate, type);
                gvTransactions.DataBind();
            }
        }

        protected void gvStatement_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvStatement.PageIndex = e.NewPageIndex; BindCurrentTable(); }
        protected void gvDeposits_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvDeposits.PageIndex = e.NewPageIndex; BindCurrentTable(); }
        protected void gvTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvTransactions.PageIndex = e.NewPageIndex; BindCurrentTable(); }
    }
}
