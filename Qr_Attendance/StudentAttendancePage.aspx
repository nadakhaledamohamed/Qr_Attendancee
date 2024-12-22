<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentAttendancePage.aspx.cs" Inherits="Qr_Attendance.StudentAttendancePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student Attendance</title>
    <style>
           body {
        font-family: 'Arial', sans-serif;
        background: linear-gradient(to right, #1d2b64, #f8cdda);
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
            max-width: 600px;
            background: rgba(255, 255, 255, 0.1);
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
            text-align: center;
        }

        h2 {
            font-size: 2rem;
            margin-bottom: 20px;
        }

        label {
            font-size: 1.2rem;
            display: block;
            margin-bottom: 10px;
        }

        .input-field {
            width: 90%;
            max-width: 400px;
            padding: 10px;
            margin: 10px 0;
            border: none;
            border-radius: 8px;
            font-size: 1rem;
        }

        .btn-submit {
            background: #28a745;
            color: #fff;
            font-size: 1rem;
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: background 0.3s ease;
        }

        .btn-submit:hover {
            background: #218838;
        }

        .message-label {
    display: inline-block;
    font-size: 1.2rem;
    font-weight: bold;
    color: #283048; /* A bright red color for error messages */
    background: rgba(255, 76, 76, 0.1); /* Light red background for better visibility */
    border: 1px solid #ff4c4c;
    padding: 10px 15px;
    border-radius: 8px;
    margin-top: 15px;
    width: auto;
    text-align: center;
    transition: all 0.3s ease;
}

.message-label.success {
    color: #28a745; /* Green for success */
    background: rgba(40, 167, 69, 0.1);
    border: 1px solid #28a745;
}

.message-label.warning {
    color: #ffc107; /* Yellow for warnings */
    background: rgba(255, 193, 7, 0.1);
    border: 1px solid #ffc107;
}

    </style>
</head>
<body>
    <form id="formStudentAttendance" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div>
            <h2>Student Attendance</h2>
       
      
        </div>

        <div>
            <asp:Label ID="lblMessage" runat="server" CssClass="message-label"></asp:Label>
        </div>
    </form>
</body>
</html>
