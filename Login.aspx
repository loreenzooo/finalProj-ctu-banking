<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Main.Login" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - CTU E-Wallet</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap" rel="stylesheet" />
    <link href="Content/styles/Login.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">

            <%-- LEFT BRAND PANEL --%>
            <div class="left-panel">
                <div class="badge">
                    <div class="badge-dot"></div>
                    CTU E-Wallet
                </div>
                <div class="brand-title">Welcome back</div>
                <div class="brand-sub">Sign in to manage your funds, send CloudMoney, and track your transactions.</div>

                <div class="feature">
                    <div class="feature-icon">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#4163BF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <rect x="2" y="7" width="20" height="14" rx="2"/>
                            <path d="M16 7V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v2"/>
                        </svg>
                    </div>
                    <div>
                        <div class="feature-label">Secure login</div>
                        <div class="feature-desc">Your account is protected</div>
                    </div>
                </div>

                <div class="feature">
                    <div class="feature-icon">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#4163BF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                        </svg>
                    </div>
                    <div>
                        <div class="feature-label">Instant transfers</div>
                        <div class="feature-desc">Send CloudMoney in seconds</div>
                    </div>
                </div>

                <div class="feature">
                    <div class="feature-icon">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#4163BF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
                        </svg>
                    </div>
                    <div>
                        <div class="feature-label">Track transactions</div>
                        <div class="feature-desc">Full statement history</div>
                    </div>
                </div>
            </div>

            <%-- RIGHT FORM PANEL --%>
            <div class="right-panel">
                <div class="form-header">
                    <h2>Sign in</h2>
                    <p>Enter your account number and password to continue.</p>
                </div>

                <div class="field">
                    <label for="txtAccountNumber">Account Number</label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-input" placeholder="e.g. 10042"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server"
                        ControlToValidate="txtAccountNumber"
                        ErrorMessage="Account Number is required."
                        CssClass="field-error"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="field">
                    <label for="txtPassword">Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                        ControlToValidate="txtPassword"
                        ErrorMessage="Password is required."
                        CssClass="field-error"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>

                <asp:Label ID="lblError" runat="server" CssClass="login-error" Visible="false"></asp:Label>

                <asp:Button ID="btnLogin" runat="server" CssClass="btn-login" Text="Login" OnClick="btnLogin_Click" />

                <div class="divider">
                    <div class="divider-line"></div>
                    <span>or</span>
                    <div class="divider-line"></div>
                </div>

                <asp:Button ID="btnCreateAccount" runat="server" CssClass="btn-create" Text="Create Account" OnClick="btnCreateAccount_Click" CausesValidation="false" />
            </div>

        </div>
    </form>
</body>
</html>