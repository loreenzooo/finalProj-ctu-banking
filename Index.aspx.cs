using System;
using System.Data;
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

            if (!IsPostBack)
            {
                int currentAccountNo = Convert.ToInt32(Session["AccountNumber"]);
                UserManager userManager = new UserManager();
                TransactionManager txManager = new TransactionManager();

                string name, dateReg;
                userManager.GetDashboardDetails(currentAccountNo, out name, out dateReg);
                lblUserName.Text = name;

                if (!string.IsNullOrEmpty(name))
                    lblProfileInitial.Text = name.Substring(0, 1).ToUpper();

                var notifs = txManager.GetRecentReceivedTransfers(currentAccountNo);
                lblNotifBadge.Visible = notifs.Rows.Count > 0;
                LoadNotifications(currentAccountNo, userManager, notifs);

                LoadDashboardStats();
                UpdateFilterState();
                BindCurrentTable();
            }
        }


        // SIDEBAR NAVIGATION
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

        // DASHBOARD STATS
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

            if (!string.IsNullOrEmpty(name))
                lblDashInitial.Text = name.Substring(0, 1).ToUpper();
        }


        // NOTIFICATIONS
        private void LoadNotifications(int accountNo, UserManager userManager, DataTable rawNotifs)
        {
            if (rawNotifs.Rows.Count == 0)
            {
                lblNoNotifications.Visible = true;
                rptNotifications.Visible = false;
            }
            else
            {
                lblNoNotifications.Visible = false;
                rptNotifications.Visible = true;

                DataTable enriched = new DataTable();
                enriched.Columns.Add("SenderName");
                enriched.Columns.Add("Amount", typeof(decimal));
                enriched.Columns.Add("Date", typeof(DateTime));

                foreach (DataRow row in rawNotifs.Rows)
                {
                    int senderAccount = Convert.ToInt32(row["SenderAccount"]);
                    string senderName = userManager.GetReceiverName(senderAccount) ?? "Unknown";
                    decimal amount = Convert.ToDecimal(row["Amount"]);
                    DateTime date = Convert.ToDateTime(row["Date"]);
                    enriched.Rows.Add(senderName, amount, date);
                }

                rptNotifications.DataSource = enriched;
                rptNotifications.DataBind();
            }
        }

        // Refreshes bell badge + dropdown after a successful send
        private void RefreshNotifications()
        {
            int currentAccountNo = Convert.ToInt32(Session["AccountNumber"]);
            UserManager userManager = new UserManager();
            TransactionManager txManager = new TransactionManager();
            var notifs = txManager.GetRecentReceivedTransfers(currentAccountNo);
            lblNotifBadge.Visible = notifs.Rows.Count > 0;
            LoadNotifications(currentAccountNo, userManager, notifs);
        }

        // ACTION PANELS (DEPOSIT / WITHDRAW / SEND)
        protected void Action_Click(object sender, EventArgs e)
        {
            Button clickedBtn = (Button)sender;
            int actionIndex = int.Parse(clickedBtn.CommandArgument);

            mvActions.ActiveViewIndex = (mvActions.ActiveViewIndex == actionIndex) ? -1 : actionIndex;

            pnlDepositForm.Visible = true;
            pnlDepositSuccess.Visible = false;
            lblDepositError.Visible = false;
            lblDepositError.Text = "";

            pnlWithdrawForm.Visible = true;
            pnlWithdrawSuccess.Visible = false;
            lblWithdrawError.Visible = false;
            lblWithdrawError.Text = "";

            if (actionIndex == 1)
            {
                TransactionManager txManager = new TransactionManager();
                lblWithdrawBalance.Text = txManager.GetBalance(
                Convert.ToInt32(Session["AccountNumber"])).ToString("N2");
            }

            if (actionIndex != 2) ResetSendPanel();
        }

        private bool IsValidAmount(decimal amount)
        {
            return amount >= 100 && amount <= 2000 && (amount % 100 == 0);
        }

        // DEPOSIT
        protected void btnSubmitDeposit_Click(object sender, EventArgs e)
        {
            lblDepositError.Visible = false;
            lblDepositError.Text = "";

            decimal amount;
            if (!decimal.TryParse(txtDepositAmount.Text, out amount))
            {
                lblDepositError.Text = "Please enter a valid numeric amount.";
                lblDepositError.Visible = true;
                return;
            }

            if (!IsValidAmount(amount))
            {
                lblDepositError.Text = "Amount must be between ₱100 and ₱2,000, and divisible by 100.";
                lblDepositError.Visible = true;
                return;
            }

            int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
            TransactionManager txManager = new TransactionManager();

            if (txManager.GetBalance(currentAccount) + amount > 10000)
            {
                lblDepositError.Text = "Maximum account balance cannot exceed ₱10,000.00.";
                lblDepositError.Visible = true;
                return;
            }

            string result = txManager.ProcessDeposit(currentAccount, amount);
            if (result == "Success")
            {
                txtDepositAmount.Text = "";
                pnlDepositForm.Visible = false;
                lblDepositSuccessAmount.Text = amount.ToString("N2");
                pnlDepositSuccess.Visible = true;
                BindCurrentTable();
            }
            else
            {
                lblDepositError.Text = result;
                lblDepositError.Visible = true;
            }
        }

        protected void btnDepositSuccessDone_Click(object sender, EventArgs e)
        {
            pnlDepositSuccess.Visible = false;
            pnlDepositForm.Visible = true;
            mvActions.ActiveViewIndex = -1;
        }


        // WITHDRAW
        protected void btnSubmitWithdraw_Click(object sender, EventArgs e)
        {
            
            lblWithdrawError.Visible = false;
            lblWithdrawError.Text = "";

            decimal amount;
            if (!decimal.TryParse(txtWithdrawAmount.Text, out amount))
            {
                lblWithdrawError.Text = "Please enter a valid numeric amount.";
                lblWithdrawError.Visible = true;
                return;
            }

            if (!IsValidAmount(amount))
            {
                lblWithdrawError.Text = "Amount must be between ₱100 and ₱2,000, and divisible by 100.";
                lblWithdrawError.Visible = true;
                return;
            }

            int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
            TransactionManager txManager = new TransactionManager();

            if (txManager.GetBalance(currentAccount) < amount)
            {
                lblWithdrawError.Text = "Insufficient funds.";
                lblWithdrawError.Visible = true;
                return;
            }

            string result = txManager.ProcessWithdraw(currentAccount, amount);
            if (result == "Success")
            {
                txtWithdrawAmount.Text = "";
                pnlWithdrawForm.Visible = false;
                lblWithdrawSuccessAmount.Text = amount.ToString("N2");
                pnlWithdrawSuccess.Visible = true;
                BindCurrentTable();
            }
            else
            {
                lblWithdrawError.Text = result;
                lblWithdrawError.Visible = true;
            }
        }

        protected void btnWithdrawSuccessDone_Click(object sender, EventArgs e)
        {
            pnlWithdrawSuccess.Visible = false;
            pnlWithdrawForm.Visible = true;
            mvActions.ActiveViewIndex = -1;
        }

        // SEND CLOUDMONEY
        protected void btnVerifyReceiver_Click(object sender, EventArgs e)
        {
            lblVerifyError.Visible = false;
            lblVerifyError.Text = "";

            int receiverAccount;
            if (!int.TryParse(txtSendAccount.Text, out receiverAccount))
            {
                lblVerifyError.Text = "Invalid Account Number format.";
                lblVerifyError.Visible = true;
                return;
            }

            int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
            if (receiverAccount == currentAccount)
            {
                lblVerifyError.Text = "You cannot send money to yourself.";
                lblVerifyError.Visible = true;
                return;
            }

            UserManager userManager = new UserManager();
            string receiverName = userManager.GetReceiverName(receiverAccount);

            if (!string.IsNullOrEmpty(receiverName))
            {
                lblReceiverAccountNo.Text = receiverAccount.ToString();
                lblReceiverName.Text = receiverName;
                pnlVerifyReceiver.Visible = false;
                pnlReceiverInfo.Visible = true;
                pnlSendMoneyForm.Visible = true;
            }
            else
            {
                lblVerifyError.Text = "Receiver account does not exist.";
                lblVerifyError.Visible = true;
            }
        }

        protected void btnCancelSend_Click(object sender, EventArgs e)
        {
            ResetSendPanel();
        }

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
            pnlSendSuccess.Visible = false;
            lblVerifyError.Visible = false;
            lblVerifyError.Text = "";
            lblSendError.Visible = false;
            lblSendError.Text = "";
        }

        protected void btnSubmitSend_Click(object sender, EventArgs e)
        {
            lblSendError.Visible = false;
            lblSendError.Text = "";

            decimal amount;
            if (!decimal.TryParse(txtSendAmount.Text, out amount))
            {
                lblSendError.Text = "Please enter a valid amount.";
                lblSendError.Visible = true;
                return;
            }

            if (!IsValidAmount(amount))
            {
                lblSendError.Text = "Amount must be between ₱100 and ₱2,000, and divisible by 100.";
                lblSendError.Visible = true;
                return;
            }

            int currentAccount = Convert.ToInt32(Session["AccountNumber"]);
            int receiverAccount = Convert.ToInt32(txtSendAccount.Text);
            string password = txtSendPassword.Text;

            UserManager userManager = new UserManager();
            TransactionManager txManager = new TransactionManager();

            if (receiverAccount == currentAccount)
            {
                lblSendError.Text = "You cannot send money to yourself.";
                lblSendError.Visible = true;
                return;
            }

            if (txManager.GetBalance(currentAccount) < amount)
            {
                lblSendError.Text = "Insufficient funds.";
                lblSendError.Visible = true;
                return;
            }

            if (!userManager.UserLogin(currentAccount, password))
            {
                lblSendError.Text = "Security Verification Failed: Incorrect password.";
                lblSendError.Visible = true;
                return;
            }

            if (txManager.GetBalance(receiverAccount) + amount > 10000)
            {
                lblSendError.Text = "Transfer failed. Recipient's balance would exceed ₱10,000.00.";
                lblSendError.Visible = true;
                return;
            }

            string receiverName = lblReceiverName.Text;
            string result = txManager.SendCloudMoney(currentAccount, receiverAccount, amount);
            if (result == "Success")
            {
                pnlReceiverInfo.Visible = false;
                pnlSendMoneyForm.Visible = false;
                lblSendSuccessAmount.Text = amount.ToString("N2");
                lblSendSuccessReceiver.Text = receiverName;
                pnlSendSuccess.Visible = true;
                BindCurrentTable();

                RefreshNotifications();
            }
            else
            {
                lblSendError.Text = result;
                lblSendError.Visible = true;
            }
        }

        protected void btnSendSuccessDone_Click(object sender, EventArgs e)
        {
            ResetSendPanel();
            mvActions.ActiveViewIndex = -1;
        }

        // DATA TABLES & FILTERING
        protected void TableTab_Click(object sender, EventArgs e)
        {
            LinkButton clickedTab = (LinkButton)sender;
            mvTables.ActiveViewIndex = int.Parse(clickedTab.CommandArgument);

            btnTabStatement.CssClass = "tab-btn";
            btnTabDeposits.CssClass = "tab-btn";
            btnTabTransactions.CssClass = "tab-btn";
            clickedTab.CssClass = "tab-btn active";

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
                lblFilterTitle.Text = "My Statement of Account";
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
