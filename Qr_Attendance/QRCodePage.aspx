<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QRCodePage.aspx.cs" Inherits="Qr_Attendance.QRCodePage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>QR Code Page</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
                    background: linear-gradient(to right, #1d2b64, #f8cdda);
          /*  background: linear-gradient(120deg, #6c5ce7, #81ecec);*/
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .qr-container {
            background: rgba(255, 255, 255, 0.95);
            padding: 2rem;
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
            text-align: center;
            animation: fadeIn 1s ease;
        }

        h2 {
            color: #2c3e50;
            margin-bottom: 1rem;
        }

        img {
            max-width: 300px;
            margin: 20px auto;
            border: 1px solid #ddd;
            border-radius: 8px;
        }

        button {
            padding: 0.8rem 1.2rem;
            border: none;
            border-radius: 8px;
            background: #6c5ce7;
            color: white;
            font-size: 1rem;
            cursor: pointer;
            transition: background 0.3s ease, transform 0.3s ease;
        }

            button:hover {
                background: #341f97;
                transform: scale(1.05);
            }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(-20px);
            }

            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
    </style>
</head>
<body>
    <form id="formQRCode" runat="server" class="qr-container">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <h2>Your QR Code</h2>
        <asp:Image ID="imgQRCode" runat="server" />
        <br />
        <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" OnClick="btnBack_Click" />
        <asp:Timer ID="Timer1" runat="server" Interval="25000" OnTick="Timer_Tick" Enabled="true" />
        
        <div>
            <asp:Label ID="lblMessage" runat="server" CssClass="message-label"></asp:Label>
        </div>
    </form>
</body>
</html>
