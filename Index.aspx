<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Main.Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CTU - E Wallet</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600&display=swap" rel="stylesheet" />
    <link href="Content/styles/LandingPage.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout-wrapper">

            <%--  SIDEBAR  --%>
            <div class="sidebar">
                <div class="sidebar-menu">
                    <asp:LinkButton ID="btnSidebarDashboard" runat="server" CssClass="menu-item active" OnClick="Sidebar_Click" CommandArgument="0">Dashboard</asp:LinkButton>
                    <asp:LinkButton ID="btnSidebarManage" runat="server" CssClass="menu-item" OnClick="Sidebar_Click" CommandArgument="1">Manage Funds</asp:LinkButton>
                    <asp:LinkButton ID="btnSidebarChangePassword" runat="server" CssClass="menu-item" OnClick="btnSidebarChangePassword_Click" CausesValidation="false">Change Password</asp:LinkButton>
                    <asp:LinkButton ID="btnSidebarLogout" runat="server" CssClass="menu-item logout-btn" OnClick="btnSidebarLogout_Click" CausesValidation="false">Log-out</asp:LinkButton>
                </div>
            </div>

            <%--  MAIN CONTENT  --%>
            <div class="main-content">

                <div class="top-header">
                    <div class="header-left"><h2>CTU - E Wallet</h2></div>
                    <div class="header-right">
                        <span class="welcome-text">Welcome, <asp:Label ID="lblUserName" runat="server" Text="User"></asp:Label></span>

                        <div class="notif-bell-wrapper">
                            <button type="button" class="notif-bell-btn" onclick="toggleNotifDropdown()" title="Notifications">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" class="notif-bell-icon">
                                    <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path>
                                    <path d="M13.73 21a2 2 0 0 1-3.46 0"></path>
                                </svg>
                                <asp:Label ID="lblNotifBadge" runat="server" CssClass="notif-badge" Visible="false" Text=""></asp:Label>
                            </button>
                            <div id="notifDropdown" class="notif-dropdown notif-dropdown-hidden">
                                <div class="notif-dropdown-header">
                                    <span class="notif-dropdown-title">Notifications</span>
                                    <span class="notif-dropdown-subtitle">Recently received CloudMoney</span>
                                </div>
                                <div class="notif-dropdown-body">
                                    <asp:Label ID="lblNoNotifications" runat="server" CssClass="notif-dropdown-empty" Text="No recent CloudMoney received." Visible="false"></asp:Label>
                                    <asp:Repeater ID="rptNotifications" runat="server">
                                        <ItemTemplate>
                                            <div class="notif-dropdown-item">
                                                <div class="notif-dropdown-item-left">
                                                    <span class="notif-dropdown-sender"><%# Eval("SenderName") %></span>
                                                    <span class="notif-dropdown-date"><%# Eval("Date", "{0:MM/dd/yyyy hh:mm tt}") %></span>
                                                </div>
                                                <span class="notif-dropdown-amount">+&#8369;<%# Eval("Amount", "{0:N2}") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>

                        <div class="profile-circle">
                            <asp:Label ID="lblProfileInitial" runat="server" CssClass="profile-initial" Text="U"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="content-container">
                    <asp:MultiView ID="mvMainContent" runat="server" ActiveViewIndex="0">

                        <%-- VIEW 0: DASHBOARD --%>
                        <asp:View ID="vwDashboardStats" runat="server">
                            <h2 class="dashboard-title">My Dashboard</h2>
                            <div class="dashboard-profile-card">
                                <div class="profile-avatar">
                                    <asp:Label ID="lblDashInitial" runat="server" Text="U"></asp:Label>
                                </div>
                                <div class="profile-details">
                                    <div class="profile-name"><asp:Label ID="lblDashName" runat="server"></asp:Label></div>
                                    <div class="profile-meta">
                                        <div class="profile-meta-item">
                                            <span class="meta-label">Account No.</span>
                                            <asp:Label ID="lblDashAccountNo" runat="server" CssClass="meta-value gold"></asp:Label>
                                        </div>
                                        <div class="profile-meta-divider"></div>
                                        <div class="profile-meta-item">
                                            <span class="meta-label">Date Registered</span>
                                            <asp:Label ID="lblDashDateReg" runat="server" CssClass="meta-value"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="dashboard-stats-row">
                                <div class="stat-card balance-card">
                                    <div class="stat-info">
                                        <span class="stat-label">Total Current Balance</span>
                                        <span class="stat-value green">&#8369;<asp:Label ID="lblDashBalance" runat="server"></asp:Label></span>
                                    </div>
                                </div>
                                <div class="stat-card sent-card">
                                    <div class="stat-info">
                                        <span class="stat-label">Total Sent Amount</span>
                                        <span class="stat-value">&#8369;<asp:Label ID="lblDashTotalSent" runat="server"></asp:Label></span>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <%-- VIEW 1: MANAGE FUNDS --%>
                        <asp:View ID="vwManageFunds" runat="server">

                            <div class="card action-container">
                                <div class="action-buttons">
                                    <asp:Button ID="btnShowDeposit" runat="server" Text="Deposit" CssClass="action-btn" OnClick="Action_Click" CommandArgument="0" />
                                    <asp:Button ID="btnShowWithdraw" runat="server" Text="Withdraw" CssClass="action-btn" OnClick="Action_Click" CommandArgument="1" />
                                    <asp:Button ID="btnShowSend" runat="server" Text="Send CloudMoney" CssClass="action-btn" OnClick="Action_Click" CommandArgument="2" />
                                </div>

                                <asp:MultiView ID="mvActions" runat="server" ActiveViewIndex="-1">

                                    <%-- DEPOSIT --%>
                                    <asp:View ID="vwDeposit" runat="server">
                                        <div class="action-panel">
                                            <h3>Deposit Funds</h3>
                                            <asp:Panel ID="pnlDepositForm" runat="server">
                                                <p class="hint-text">Min: &#8369;100 | Max: &#8369;2,000 | Must be divisible by 100</p>
                                                <asp:TextBox ID="txtDepositAmount" runat="server" CssClass="form-input" placeholder="Amount"></asp:TextBox>
                                                <asp:Label ID="lblDepositError" runat="server" CssClass="inline-error" Visible="false"></asp:Label>
                                                <asp:Button ID="btnSubmitDeposit" runat="server" Text="Confirm Deposit" CssClass="btn-confirm-deposit" OnClick="btnSubmitDeposit_Click" />
                                            </asp:Panel>
                                            <asp:Panel ID="pnlDepositSuccess" runat="server" Visible="false" CssClass="txn-success-panel">
                                                <div class="txn-success-icon">
                                                    <svg viewBox="0 0 24 24" class="txn-success-svg">
                                                        <polyline points="20 6 9 17 4 12"></polyline>
                                                    </svg>
                                                </div>
                                                <div class="txn-success-title">Deposit Successful!</div>
                                                <div class="txn-success-amount">+&#8369;<asp:Label ID="lblDepositSuccessAmount" runat="server"></asp:Label></div>
                                                <div class="txn-success-sub">Your balance has been updated.</div>
                                                <asp:Button ID="btnDepositSuccessDone" runat="server" Text="Done" CssClass="btn-txn-done" OnClick="btnDepositSuccessDone_Click" CausesValidation="false" />
                                            </asp:Panel>
                                        </div>
                                    </asp:View>

                                    <%-- WITHDRAW --%>
                                    <asp:View ID="vwWithdraw" runat="server">
                                        <div class="action-panel">
                                            <h3>Withdraw Funds</h3>
                                            <asp:Panel ID="pnlWithdrawForm" runat="server">
                                                <p class="withdraw-balance-text">Current Balance: &#8369;<asp:Label ID="lblWithdrawBalance" runat="server"></asp:Label></p>
                                                <p class="hint-text">Min: &#8369;100 | Max: &#8369;2,000 | Must be divisible by 100</p>
                                                <asp:TextBox ID="txtWithdrawAmount" runat="server" CssClass="form-input" placeholder="Amount"></asp:TextBox>
                                                <asp:Label ID="lblWithdrawError" runat="server" CssClass="inline-error" Visible="false"></asp:Label>
                                                <asp:Button ID="btnSubmitWithdraw" runat="server" Text="Confirm Withdraw" CssClass="btn-confirm-withdraw" OnClick="btnSubmitWithdraw_Click" />
                                            </asp:Panel>
                                            <asp:Panel ID="pnlWithdrawSuccess" runat="server" Visible="false" CssClass="txn-success-panel">
                                                <div class="txn-success-icon">
                                                    <svg viewBox="0 0 24 24" class="txn-success-svg">
                                                        <polyline points="20 6 9 17 4 12"></polyline>
                                                    </svg>
                                                </div>
                                                <div class="txn-success-title">Withdrawal Successful!</div>
                                                <div class="txn-success-amount">-&#8369;<asp:Label ID="lblWithdrawSuccessAmount" runat="server"></asp:Label></div>
                                                <div class="txn-success-sub">Your balance has been updated.</div>
                                                <asp:Button ID="btnWithdrawSuccessDone" runat="server" Text="Done" CssClass="btn-txn-done" OnClick="btnWithdrawSuccessDone_Click" CausesValidation="false" />
                                            </asp:Panel>
                                        </div>
                                    </asp:View>

                                    <%-- SEND CLOUDMONEY --%>
                                    <asp:View ID="vwSend" runat="server">
                                        <div class="action-panel">
                                            <h3>Send CloudMoney</h3>
                                           <asp:Panel ID="pnlVerifyReceiver" runat="server">
                                                <asp:TextBox ID="txtSendAccount" runat="server" CssClass="form-input verify-input" placeholder="Recipient Account No."></asp:TextBox>
                                                <asp:Button ID="btnVerifyReceiver" runat="server" Text="Verify" CssClass="btn-verify" OnClick="btnVerifyReceiver_Click" />
                                                <asp:Label ID="lblVerifyError" runat="server" CssClass="inline-error" Visible="false"></asp:Label>
                                           </asp:Panel>
                                            <asp:Panel ID="pnlReceiverInfo" runat="server" Visible="false">
                                                <div class="receiver-info-panel">
                                                    <span class="info-label">Sending To</span>
                                                    <hr class="receiver-info-divider" />
                                                    <span class="info-label">Account No.</span>
                                                    <asp:Label ID="lblReceiverAccountNo" runat="server" CssClass="info-value highlight"></asp:Label>
                                                    <span class="info-label">Name</span>
                                                    <asp:Label ID="lblReceiverName" runat="server" CssClass="info-value"></asp:Label>
                                                </div>
                                            </asp:Panel>
                                            <%-- SEND MONEY FORM--%>
                                            <asp:Panel ID="pnlSendMoneyForm" runat="server" Visible="false" CssClass="send-money-form-panel">
                                                <p class="hint-text">Min: &#8369;100 | Max: &#8369;2,000 | Must be divisible by 100</p>
                                                <asp:TextBox ID="txtSendAmount" runat="server" CssClass="form-input" placeholder="Amount"></asp:TextBox>
                                                <asp:TextBox ID="txtSendPassword" runat="server" CssClass="form-input" placeholder="Enter Your Password" TextMode="Password"></asp:TextBox>
                                                <asp:Label ID="lblSendError" runat="server" CssClass="inline-error" Visible="false"></asp:Label>
                                                <asp:Button ID="btnSubmitSend" runat="server" Text="Send Money" CssClass="btn-submit" OnClick="btnSubmitSend_Click" />
                                                <asp:Button ID="btnCancelSend" runat="server" Text="Cancel" CssClass="btn-filter" OnClick="btnCancelSend_Click" CausesValidation="false" />
                                            </asp:Panel>
                                            <%-- PANEL SEND SUCCESS --%>
                                            <asp:Panel ID="pnlSendSuccess" runat="server" Visible="false" CssClass="txn-success-panel">
                                                <div class="txn-success-icon">
                                                    <svg viewBox="0 0 24 24" class="txn-success-svg">
                                                        <polyline points="20 6 9 17 4 12"></polyline>
                                                    </svg>
                                                </div>
                                                <div class="txn-success-title">Money Sent!</div>
                                                <div class="txn-success-amount">-&#8369;<asp:Label ID="lblSendSuccessAmount" runat="server"></asp:Label></div>
                                                <div class="txn-success-meta">
                                                    <span class="txn-success-meta-label">Sent to</span>
                                                    <span class="txn-success-meta-value"><asp:Label ID="lblSendSuccessReceiver" runat="server"></asp:Label></span>
                                                </div>
                                                <div class="txn-success-sub">Transaction completed successfully.</div>
                                                <asp:Button ID="btnSendSuccessDone" runat="server" Text="Done" CssClass="btn-txn-done" OnClick="btnSendSuccessDone_Click" CausesValidation="false" />
                                            </asp:Panel>
                                        </div>
                                    </asp:View>

                                </asp:MultiView>
                            </div>

                            <div class="card data-container">
                                <%-- TAB BTNS STATEMENT DEPOSITS TRANSACTIONS--%>
                                <div class="tab-buttons">
                                    <asp:LinkButton ID="btnTabStatement" runat="server" CssClass="tab-btn active" OnClick="TableTab_Click" CommandArgument="0">View Statement Of Account</asp:LinkButton>
                                    <asp:LinkButton ID="btnTabDeposits" runat="server" CssClass="tab-btn" OnClick="TableTab_Click" CommandArgument="1">My Deposits or Withdrawals</asp:LinkButton>
                                    <asp:LinkButton ID="btnTabTransactions" runat="server" CssClass="tab-btn" OnClick="TableTab_Click" CommandArgument="2">My Sent or Received Transactions</asp:LinkButton>
                                </div>

                                <%-- CALENDAR FILTER--%>
                                <div class="centered-filter-form">
                                    <h3 class="filter-title"><asp:Label ID="lblFilterTitle" runat="server" Text="My Statement of Account"></asp:Label></h3>
                                    <div class="filter-row"><label>From</label><asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="filter-input-centered"></asp:TextBox></div>
                                    <div class="filter-row"><label>To</label><asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="filter-input-centered"></asp:TextBox></div>
                                    <div class="filter-row">
                                        <label>Type</label>
                                        <asp:DropDownList ID="ddlType" runat="server" CssClass="filter-input-centered"><asp:ListItem Text="All" Value="All"></asp:ListItem></asp:DropDownList>
                                    </div>
                                    <asp:Label ID="lblDateError" runat="server" CssClass="date-error" Visible="false"></asp:Label>
                                    <div class="filter-actions"><asp:Button ID="btnList" runat="server" Text="List" CssClass="btn-list" OnClick="btnList_Click" /></div>
                                </div>
                                <asp:MultiView ID="mvTables" runat="server" ActiveViewIndex="0">
                                    <asp:View ID="vwTableStatement" runat="server">
                                        <asp:GridView ID="gvStatement" runat="server" AutoGenerateColumns="False" CssClass="grid-view" GridLines="None" AllowPaging="True" PageSize="10" PagerStyle-CssClass="grid-pager" OnPageIndexChanging="gvStatement_PageIndexChanging">
                                            <HeaderStyle CssClass="grid-header" /><RowStyle CssClass="grid-row" /><AlternatingRowStyle CssClass="grid-alt-row" />
                                            <Columns>
                                                <asp:BoundField DataField="SeqNum" HeaderText="Seq. #" />
                                                <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                                <asp:BoundField DataField="Debit" HeaderText="Debit" DataFormatString="{0:N2}" NullDisplayText="" />
                                                <asp:BoundField DataField="Credit" HeaderText="Credit" DataFormatString="{0:N2}" NullDisplayText="" />
                                                <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:View>
                                    <asp:View ID="vwTableDeposits" runat="server">
                                        <asp:GridView ID="gvDeposits" runat="server" AutoGenerateColumns="False" CssClass="grid-view" GridLines="None" AllowPaging="True" PageSize="10" PagerStyle-CssClass="grid-pager" OnPageIndexChanging="gvDeposits_PageIndexChanging">
                                            <HeaderStyle CssClass="grid-header" /><RowStyle CssClass="grid-row" /><AlternatingRowStyle CssClass="grid-alt-row" />
                                            <Columns>
                                                <asp:BoundField DataField="SeqNum" HeaderText="Seq. #" />
                                                <asp:BoundField DataField="Type" HeaderText="Type" />
                                                <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                                                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:View>
                                    <asp:View ID="vwTableTransactions" runat="server">
                                        <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False" CssClass="grid-view" GridLines="None" AllowPaging="True" PageSize="10" PagerStyle-CssClass="grid-pager" OnPageIndexChanging="gvTransactions_PageIndexChanging">
                                            <HeaderStyle CssClass="grid-header" /><RowStyle CssClass="grid-row" /><AlternatingRowStyle CssClass="grid-alt-row" />
                                            <Columns>
                                                <asp:BoundField DataField="SeqNum" HeaderText="Seq. #" />
                                                <asp:BoundField DataField="DateSent" HeaderText="Date Sent" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                                                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />
                                                <asp:BoundField DataField="SentTo" HeaderText="Sent To" NullDisplayText="" />
                                                <asp:BoundField DataField="ReceivedFrom" HeaderText="Received From" NullDisplayText="" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:View>
                                </asp:MultiView>
                            </div>
                        </asp:View>

                    </asp:MultiView>
                </div>
            </div>
        </div>
    </form>

    <%-- NOTIFICATION BUTTON FOR SEND CLOUD MONEY --%>
    <script type="text/javascript">
        function toggleNotifDropdown() {
            var dropdown = document.getElementById('notifDropdown');
            dropdown.style.display = (dropdown.style.display === 'none' || dropdown.style.display === '') ? 'block' : 'none';
        }
        document.addEventListener('click', function (e) {
            var wrapper = document.querySelector('.notif-bell-wrapper');
            if (wrapper && !wrapper.contains(e.target)) {
                var dropdown = document.getElementById('notifDropdown');
                if (dropdown) dropdown.style.display = 'none';
            }
        });
    </script>
</body>
</html>
