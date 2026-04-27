<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Main.Registration" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register - CTU E-Wallet</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap" rel="stylesheet" />
    <link href="Content/styles/Registration.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">

            <%-- LEFT PANEL — Step Indicator --%>
            <div class="left-panel">
                <div class="badge">
                    <div class="badge-dot"></div>
                    CTU E-Wallet
                </div>
                <div class="brand-title">Create your account</div>
                <div class="brand-sub">Open your free e-wallet account in minutes and start managing your funds securely.</div>

                <div class="step">
                    <asp:Label ID="lblStep1Circle" runat="server" CssClass="step-circle step-done" Text="✓"></asp:Label>
                    <div>
                        <div class="step-label">Personal info</div>
                        <div class="step-desc">Your name and email</div>
                    </div>
                </div>
                <div class="step-connector"></div>
                <div class="step">
                    <asp:Label ID="lblStep2Circle" runat="server" CssClass="step-circle step-active" Text="2"></asp:Label>
                    <div>
                        <div class="step-label">Security</div>
                        <div class="step-desc">Create your password</div>
                    </div>
                </div>
                <div class="step-connector"></div>
                <div class="step">
                    <asp:Label ID="lblStep3Circle" runat="server" CssClass="step-circle step-inactive" Text="3"></asp:Label>
                    <div>
                        <div class="step-label" style="color: #8d949e;">Account created</div>
                        <div class="step-desc">Receive your account number</div>
                    </div>
                </div>
            </div>

            <%-- RIGHT PANEL --%>
            <div class="right-panel">

                <%-- REGISTRATION FORM --%>
                <asp:Panel ID="pnlRegistration" runat="server">
                    <div class="form-header">
                        <h2>Let's get started</h2>
                        <p>Fill in the details below to create your account.</p>
                    </div>

                    <div class="section-label">Personal information</div>

                    <div class="name-row">
                        <div class="field">
                            <label>First name</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" placeholder="e.g. Juan"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                                ControlToValidate="txtFirstName"
                                ErrorMessage="First name is required."
                                CssClass="field-error"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </div>
                        <div class="field">
                            <label>Last name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" placeholder="e.g. Dela Cruz"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                                ControlToValidate="txtLastName"
                                ErrorMessage="Last name is required."
                                CssClass="field-error"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="field">
                        <label>Email address</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" placeholder="your@email.com"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Email is required."
                            CssClass="field-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvEmailUnique" runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="This email is already registered."
                            CssClass="field-error"
                            Display="Dynamic"
                            OnServerValidate="Email_ServerValidate">
                        </asp:CustomValidator>
                    </div>

                    <div class="section-label">Gender</div>
                    <div class="gender-row">
                        <asp:RadioButton ID="rbMale" runat="server" GroupName="Gender" Text="Male" CssClass="gender-chip" />
                        <asp:RadioButton ID="rbFemale" runat="server" GroupName="Gender" Text="Female" CssClass="gender-chip" />
                    </div>

                    <div class="section-label">Security</div>

                    <div class="field">
                        <label>Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input" TextMode="Password" placeholder="8–20 characters"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                            ControlToValidate="txtPassword"
                            ErrorMessage="Password is required."
                            CssClass="field-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revPasswordLength" runat="server"
                            ControlToValidate="txtPassword"
                            ValidationExpression="^.{8,20}$"
                            ErrorMessage="Password must be between 8 and 20 characters."
                            CssClass="field-error"
                            Display="Dynamic">
                        </asp:RegularExpressionValidator>
                    </div>

                    <div class="field">
                        <label>Confirm password</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-input" TextMode="Password" placeholder="Re-enter password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ErrorMessage="Please confirm your password."
                            CssClass="field-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvPasswordMatch" runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ControlToCompare="txtPassword"
                            ErrorMessage="Passwords do not match."
                            CssClass="field-error"
                            Display="Dynamic">
                        </asp:CompareValidator>
                    </div>

                    <asp:Button ID="btnRegister" runat="server" CssClass="btn-register" Text="Create Account" OnClick="btnRegister_Click" />
                    <a href="Login.aspx" class="back-link">Already have an account? Login here</a>
                </asp:Panel>

                <%-- SUCCESS PANEL --%>
                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="success-panel">
                    <div class="success-icon">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#2DD253" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" width="28" height="28">
                            <polyline points="20 6 9 17 4 12"></polyline>
                        </svg>
                    </div>
                    <h2 class="success-title">Account Created!</h2>
                    <p class="success-sub">Welcome to CTU E-Wallet. Your account number has been assigned.<br />Save it — you will need this to log in.</p>

                    <div class="acct-box">
                        <div class="acct-label">Your account number</div>
                        <asp:Label ID="lblAccountNumber" runat="server" CssClass="acct-number"></asp:Label>
                    </div>

                    <p class="acct-hint">Keep this number safe. You will need it every time you log in.</p>
                    <a href="Login.aspx" class="btn-register" style="text-align:center; text-decoration:none; display:block;">Go to Login</a>
                </asp:Panel>

            </div>
        </div>
    </form>
</body>
</html>