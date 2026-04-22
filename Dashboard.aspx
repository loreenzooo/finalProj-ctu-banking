<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Main.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard - CTU Bank</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600&display=swap" rel="stylesheet" />
    <link href="../Content/styles/Dashboard.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="top-header">
            <h1>CTU Bank Admin</h1>
        </div>

        <div class="content-container">
            
            <div class="card">
                <div class="table-header-title">Registered Users</div>
                
                <asp:GridView ID="gvRegisteredUsers" runat="server" AutoGenerateColumns="False" 
                    CssClass="grid-view" GridLines="None">
                    <HeaderStyle CssClass="grid-header" />
                    <RowStyle CssClass="grid-row" />
                    <AlternatingRowStyle CssClass="grid-alt-row" />
                    <Columns>
                        <asp:BoundField DataField="account_number" HeaderText="Account No." />
                        <asp:BoundField DataField="first_name" HeaderText="First Name" />
                        <asp:BoundField DataField="middle_initial" HeaderText="M.I." NullDisplayText="-" />
                        <asp:BoundField DataField="last_name" HeaderText="Last Name" />
                        <asp:BoundField DataField="email_address" HeaderText="Email" />
                        <asp:BoundField DataField="date_registered" HeaderText="Date Registered" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="balance" HeaderText="Balance" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>

        </div>
    </form>
</body>
</html>