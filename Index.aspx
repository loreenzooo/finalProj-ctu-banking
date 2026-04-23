<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Main.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CTU - Bank Dashboard</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600&display=swap" rel="stylesheet" />
    <link href="../Content/styles/LandingPage.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout-wrapper">
            
            <div class="sidebar">
                <div class="sidebar-brand">
                    <h2>CTU Bank</h2>
                </div>
                <div class="sidebar-menu">
                    <asp:LinkButton ID="btnMenuStatement" runat="server" CssClass="menu-item active" OnClick="Menu_Click" CommandArgument="0">Statement of Account</asp:LinkButton>
                    <asp:LinkButton ID="btnMenuDeposits" runat="server" CssClass="menu-item" OnClick="Menu_Click" CommandArgument="1">Deposits / Withdrawals</asp:LinkButton>
                    <asp:LinkButton ID="btnMenuTransactions" runat="server" CssClass="menu-item" OnClick="Menu_Click" CommandArgument="2">Sent / Received</asp:LinkButton>
                    <%--  <asp:LinkButton ID="btnDashboard" runat="server" CssClass="menu-item" OnClick="Menu_Click" CommandArgument="2">Dashboard</asp:LinkButton> --%>
                </div>
            </div>

            <div class="main-content">
                
                <div class="top-header">
                    <h1><asp:Label ID="lblPageTitle" runat="server" Text="Statement of Account"></asp:Label></h1>
                </div>

                <div class="content-container">
                    
                    <div class="card filter-card">
                        <div class="filter-group">
                            <label>From Date:</label>
                            <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="filter-input"></asp:TextBox>
                        </div>
                        <div class="filter-group">
                            <label>To Date:</label>
                            <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="filter-input"></asp:TextBox>
                        </div>
                        <div class="filter-group">
                            <label>Type:</label>
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="filter-input">
                                <asp:ListItem Text="All" Value="All"></asp:ListItem>
                                <asp:ListItem Text="Deposit" Value="Depo"></asp:ListItem>
                                <asp:ListItem Text="Withdraw" Value="With"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="filter-group">
                            <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn-filter" OnClick="btnFilter_Click" />
                        </div>
                    </div>

                    <div class="card">
                        <asp:MultiView ID="mvDashboard" runat="server" ActiveViewIndex="0">
                            
                            <%-- STATEMENT OF ACCOUNT --%>
                            <asp:View ID="vwStatement" runat="server">
                                <h2 class="table-header-title">Statement of Account</h2>
                                <asp:GridView ID="gvStatement" runat="server" AutoGenerateColumns="False" 
                                    CssClass="grid-view" GridLines="None" AllowPaging="True" AllowSorting="True" PageSize="10" OnPageIndexChanging="gvStatement_PageIndexChanging">
                                    <HeaderStyle CssClass="grid-header" />
                                    <RowStyle CssClass="grid-row" />
                                    <AlternatingRowStyle CssClass="grid-alt-row" />
                                    <Columns>
                                        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                        <asp:BoundField DataField="Debit" HeaderText="Debit" DataFormatString="{0:N2}" NullDisplayText="-" />
                                        <asp:BoundField DataField="Credit" HeaderText="Credit" DataFormatString="{0:N2}" NullDisplayText="-" />
                                        <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />
                                    </Columns>
                                </asp:GridView>
                            </asp:View>

                            <%-- DEPOSITS --%>
                            <asp:View ID="vwDeposits" runat="server">
                                <h2 class="table-header-title">My Deposits or Withdrawals</h2>
                                <asp:GridView ID="gvDeposits" runat="server" AutoGenerateColumns="False" 
                                    CssClass="grid-view" GridLines="None" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvDeposits_PageIndexChanging">
                                    <HeaderStyle CssClass="grid-header" />
                                    <RowStyle CssClass="grid-row" />
                                    <AlternatingRowStyle CssClass="grid-alt-row" />
                                    <Columns>
                                        <asp:BoundField DataField="SeqNum" HeaderText="Seq. #" />
                                        <asp:BoundField DataField="Type" HeaderText="Type (W/D)" />
                                        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                                        <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />
                                    </Columns>
                                </asp:GridView>
                            </asp:View>

                            <%-- TRANSACTIONS --%>
                            <asp:View ID="vwTransactions" runat="server">
                                <h2 class="table-header-title">My Sent or Received Transactions</h2>
                                <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False" 
                                    CssClass="grid-view" GridLines="None" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvTransactions_PageIndexChanging">
                                    <HeaderStyle CssClass="grid-header" />
                                    <RowStyle CssClass="grid-row" />
                                    <AlternatingRowStyle CssClass="grid-alt-row" />
                                    <Columns>
                                        <asp:BoundField DataField="SeqNum" HeaderText="Seq. #" />
                                        <asp:BoundField DataField="DateSent" HeaderText="Date Sent" DataFormatString="{0:MM/dd/yyyy}" />
                                        <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />
                                        <asp:BoundField DataField="SentTo" HeaderText="Sent To" NullDisplayText="-" />
                                        <asp:BoundField DataField="ReceivedFrom" HeaderText="Received From" NullDisplayText="-" />
                                    </Columns>
                                </asp:GridView>
                            </asp:View>

                        </asp:MultiView>
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>