<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Main.Registration" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration - CTU Lab Equipment Tracker</title>
    <link href="../Content/Login.css" rel="stylesheet" type="text/css" />
    <link href="../Content/Registration.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div id="reg-container">
                <div id="form">
                    <h2 style="color: #EBA800;">Create Account</h2>

                    <asp:TextBox ID="fullNameTxt" runat="server" CssClass="login-input" placeholder="Full Name"></asp:TextBox>
                    
                    <asp:TextBox ID="emailTxt" runat="server" CssClass="login-input" placeholder="Email Address"></asp:TextBox>
                    
                    <asp:TextBox ID="usernameTxt" runat="server" CssClass="login-input" placeholder="Username"></asp:TextBox>
                    
                    <asp:TextBox ID="passwordTxt" runat="server" CssClass="login-input" TextMode="Password" placeholder="Password"></asp:TextBox>
                    
                    <asp:TextBox ID="confirmPassTxt" runat="server" CssClass="login-input" TextMode="Password" placeholder="Confirm Password"></asp:TextBox>

                    <asp:Button ID="registerBtn" runat="server" CssClass="reg-btn" Text="Register" OnClick="registerBtn_Click" />
                    
                    <a href="Login.aspx" class="back-link">Already have an account? Login here</a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>