using MessagingToolkit.QRCode.Codec;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace Qr_Attendance
{
    public partial class QRCodePage : System.Web.UI.Page
    {
        private DateTime startTime;
        private DateTime endTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Schedule_ID"] == null || Session["People_ID"] == null)
                {
                    Response.Redirect("DoctorHomePage.aspx");
                    return;
                }
                startTime = DateTime.Now;
                endTime = startTime.AddSeconds(25);

                // Save start and end time in the session
                Session["StartTime"] = startTime;
                Session["EndTime"] = endTime;

                GenerateAndDisplayQRCode();
            }
            else
            {
                // Retrieve start and end time from the session for subsequent loads
                if (Session["StartTime"] != null && Session["EndTime"] != null)
                {
                    startTime = (DateTime)Session["StartTime"];
                    endTime = (DateTime)Session["EndTime"];
                }
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            startTime = endTime;
            endTime = startTime.AddSeconds(25);

            // Save  session
            Session["StartTime"] = startTime;
            Session["EndTime"] = endTime;
            GenerateAndDisplayQRCode();
        }

        private void GenerateAndDisplayQRCode()
        {
            int scheduleId = Convert.ToInt32(Session["Schedule_ID"]);
            int doctorId = Convert.ToInt32(Session["People_ID"]);
           

           
            DateTime endTime = startTime.AddSeconds(25); // End time = start time + 25 seconds 
            string qrContent = GenerateQRContent(scheduleId, doctorId, startTime, endTime);

            // Log the QR content to the browser's console
            ScriptManager.RegisterStartupScript(this, GetType(), "LogQRCodeContent",
                $"console.log('QR Code Content: {qrContent}'); console.log('QR Code refreshed at {startTime}');", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "DebugQRCodeContent",
    $"console.log('Generated QR Code Content: Schedule_ID: {Session["Schedule_ID"]}, People_ID: {Session["People_ID"]}');", true);

            ServeQRCode(qrContent);
        }

        private string GenerateQRContent(int scheduleId, int doctorId, DateTime startTime, DateTime endTime)
        {
            return $"{Request.Url.GetLeftPart(UriPartial.Authority)}/StudentAttendancePage.aspx" +
                   $"?Schedule_ID={scheduleId}&Doctor_ID={doctorId}" +
                   $"&StartTime={startTime:yyyy-MM-ddTHH:mm:ss}&EndTime={endTime:yyyy-MM-ddTHH:mm:ss}";
        }

        private void ServeQRCode(string content)
        {
            QRCodeEncoder encoder = new QRCodeEncoder
            {
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H,
                QRCodeScale = 5
            };

            using (Bitmap qrCode = encoder.Encode(content))
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                qrCode.Save(ms, ImageFormat.Png);
                byte[] qrCodeBytes = ms.ToArray();

                string base64Image = Convert.ToBase64String(qrCodeBytes);
                imgQRCode.ImageUrl = $"data:image/png;base64,{base64Image}";
            }
        }
        
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("DoctorHomePage.aspx");
        }
    }
}

