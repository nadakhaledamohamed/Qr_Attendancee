using MessagingToolkit.QRCode.Codec;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;

namespace Qr_Attendance
{
    public partial class QRCodePage : System.Web.UI.Page
    {
        private DateTime startTime;

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
                GenerateAndDisplayQRCode();
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            // Refresh the QR code with updated end time
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
                $"console.log('QR Code Content: {qrContent}'); console.log('QR Code refreshed at {DateTime.Now.ToString("hh:mm:ss tt")}');", true);

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
