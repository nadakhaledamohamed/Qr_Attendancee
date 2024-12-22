<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoctorHomePage.aspx.cs" Inherits="Qr_Attendance.DoctorHomePage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Doctor's Dashboard</title>
    <link href="Style/DocStyle.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="header animated-fade">
            <h1>Welcome, Doctor</h1>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="message-label"></asp:Label>
        </div>

        <div class="content animated-slide-up">
            <h2>Today's Lectures</h2>
            <asp:GridView ID="gvLectures" runat="server" AutoGenerateColumns="True" CssClass="lectures-table" EmptyDataText="No lectures scheduled for today."></asp:GridView>
        </div>

        <div class="footer animated-slide-in">
            <asp:Button ID="btnGenerateQR" runat="server" Text="Generate QR Code" CssClass="btn-generate" OnClick="btnGenerateQR_Click"></asp:Button>
        </div>
    </form>

    <script>
        // Animations for page load
        document.addEventListener('DOMContentLoaded', () => {
            document.querySelector('.header').classList.add('visible');
            document.querySelector('.content').classList.add('visible');
            document.querySelector('.footer').classList.add('visible');
        });
    </script>
</body>
</html>--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoctorHomePage.aspx.cs" Inherits="Qr_Attendance.DoctorHomePage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Doctor's Dashboard</title>
    <link href="Style/DocStyle.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background: linear-gradient(to right,#1d2b64, #f8cdda);
            color: #fff;
            margin: 0;
            padding: 0;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
        }

        .container {
            width: 80%;
            max-width: 1200px;
            background: rgba(255, 255, 255, 0.1);
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
        }

        .header {
            text-align: center;
            margin-bottom: 20px;
        }

        .header h1 {
            font-size: 2.5rem;
            margin: 0;
        }

        .message-label {
            display: block;
            font-size: 1.2rem;
            margin-top: 10px;
        }

        .content {
            margin: 20px 0;
        }

        .content h2 {
            font-size: 1.8rem;
            margin-bottom: 15px;
        }

        .lectures-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
            text-align: center;
        }

        .lectures-table th, .lectures-table td {
            padding: 12px;
            border: 1px solid #ddd;
            color: #fff;
        }

       

        .lectures-table tr:nth-child(even) {
            background: rgba(255, 255, 255, 0.1);
        }

        .lectures-table tr:nth-child(odd) {
            background: rgba(255, 255, 255, 0.2);
        }

        .btn-generate {
            background: #28a745;
            color: #fff;
            font-size: 1rem;
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: background 0.3s ease;
        }

        .btn-generate:hover {
            background: #218838;
        }

        .footer {
            text-align: center;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="header">
            <h1>Welcome, Doctor</h1>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="message-label"></asp:Label>
        </div>

        <div class="content">
            <h2>Today's Lectures</h2>
            <asp:GridView ID="gvLectures" runat="server" AutoGenerateColumns="True" CssClass="lectures-table" EmptyDataText="No lectures scheduled for today."></asp:GridView>
        </div>

        <div class="footer">
            <asp:Button ID="btnGenerateQR" runat="server" Text="Generate QR Code" CssClass="btn-generate" OnClick="btnGenerateQR_Click"></asp:Button>
        </div>
    </form>
</body>
</html>

