using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Configuration; // Add this if pulling connection string from Web.config

namespace Main
{
    public partial class Index : System.Web.UI.Page
    {
        // Update this to your actual connection string source
        private string connectionString = "Server=YOURSERVER;Database=YOURDB;Trusted_Connection=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Default View Setup
                mvDashboard.ActiveViewIndex = 0;
                UpdateFilterDropdownOptions();
                BindCurrentData();
            }
        }

        // --- NAVIGATION LOGIC ---
        protected void Menu_Click(object sender, EventArgs e)
        {
            LinkButton clickedBtn = (LinkButton)sender;
            int viewIndex = int.Parse(clickedBtn.CommandArgument);

            // Change Active View
            mvDashboard.ActiveViewIndex = viewIndex;

            // Reset CSS classes for sidebar
            btnMenuStatement.CssClass = "menu-item";
            btnMenuDeposits.CssClass = "menu-item";
            btnMenuTransactions.CssClass = "menu-item";
            btnDashboard.CssClass = "menu-item";

            // Highlight active menu and set title
            clickedBtn.CssClass = "menu-item active";
            lblPageTitle.Text = clickedBtn.Text;

            // Update DropDown options based on new view
            UpdateFilterDropdownOptions();

            // Fetch new data
            BindCurrentData();
        }

        // --- DYNAMIC FILTER DROPDOWN ---
        private void UpdateFilterDropdownOptions()
        {
            ddlType.Items.Clear();
            ddlType.Items.Add(new ListItem("All", "All"));

            if (mvDashboard.ActiveViewIndex == 1) // Deposits / Withdrawals
            {
                ddlType.Items.Add(new ListItem("Deposit (D)", "D"));
                ddlType.Items.Add(new ListItem("Withdrawal (W)", "W"));
            }
            else if (mvDashboard.ActiveViewIndex == 2) // Transactions
            {
                ddlType.Items.Add(new ListItem("Sent", "Sent"));
                ddlType.Items.Add(new ListItem("Received", "Received"));
            }
            // View 0 (Statement) only needs "All" or optional items.
        }

        // --- FILTER CLICK ---
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindCurrentData();
        }

        // --- DATA BINDING ROUTER ---
        private void BindCurrentData()
        {
            string fromDate = txtFromDate.Text;
            string toDate = txtToDate.Text;
            string type = ddlType.SelectedValue;

            if (mvDashboard.ActiveViewIndex == 0)
                BindStatement(fromDate, toDate);
            else if (mvDashboard.ActiveViewIndex == 1)
                BindDeposits(fromDate, toDate, type);
            else if (mvDashboard.ActiveViewIndex == 2)
                BindTransactions(fromDate, toDate, type);
        }

        // --- SQL DATA FETCHING ---
        private void BindStatement(string fromDate, string toDate)
        {
            // Example ADO.NET SQL setup
            /*
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Date, Description, Debit, Credit, Balance FROM StatementTable WHERE 1=1";
                if (!string.IsNullOrEmpty(fromDate)) query += " AND Date >= @From";
                if (!string.IsNullOrEmpty(toDate)) query += " AND Date <= @To";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(fromDate)) cmd.Parameters.AddWithValue("@From", fromDate);
                    if (!string.IsNullOrEmpty(toDate)) cmd.Parameters.AddWithValue("@To", toDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvStatement.DataSource = dt;
                    gvStatement.DataBind();
                }
            }
            */
        }

        private void BindDeposits(string fromDate, string toDate, string type)
        {
            // Same logic as above, but for Deposits Table
            // if (type != "All") query += " AND Type = @Type";
        }

        private void BindTransactions(string fromDate, string toDate, string type)
        {
            // Same logic as above, but for Transactions Table
        }

        // --- GRIDVIEW PAGING EVENTS ---
        protected void gvStatement_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStatement.PageIndex = e.NewPageIndex;
            BindCurrentData();
        }

        protected void gvDeposits_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDeposits.PageIndex = e.NewPageIndex;
            BindCurrentData();
        }

        protected void gvTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTransactions.PageIndex = e.NewPageIndex;
            BindCurrentData();
        }
    }
}