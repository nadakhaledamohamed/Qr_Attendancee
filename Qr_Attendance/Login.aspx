<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Qr_Attendance.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Animated Login Form</title>

    <link href="Style/LoginStyle.css" rel="stylesheet" />
  
</head>
<body>
    <form id="loginForm" runat="server" class="login-container">
        <h2>Log In</h2>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        <asp:TextBox ID="txtUsername" runat="server" Placeholder="Username" />
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password" />
        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
        
    </form>
</body>
</html>