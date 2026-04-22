using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Main.Classes;


namespace Main
{
    public partial class Dashboard : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // TODO: Execute your SELECT * FROM Users query 
                // and bind it to gvRegisteredUsers here.

                BindUserGrid();
            }
        }
        private void BindUserGrid()
        {
            DashboardManager dbManager = new DashboardManager();
            gvRegisteredUsers.DataSource = dbManager.GetRegisteredUsers();
            gvRegisteredUsers.DataBind();


        }
    }
}