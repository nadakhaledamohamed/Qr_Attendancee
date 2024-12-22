//using System;
//using System.Data.SqlClient;
//using System.Web;
//using System.Web.Configuration;

//namespace Qr_Attendance
//{
//    public partial class StudentAttendancePage : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                // Check if Schedule_ID is passed in the query string
//                if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]))
//                {
//                    int scheduleId = int.Parse(Request.QueryString["Schedule_ID"]);

//                    if (Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
//                    {
//                        // Student is logged in, mark attendance directly
//                        MarkAttendance(scheduleId);
//                    }
//                    else
//                    {
//                        // Save Schedule_ID in session and redirect to Login
//                        Session["Pending_Schedule_ID"] = scheduleId;
//                        Response.Redirect("Login.aspx");
//                    }
//                }
//                else
//                {
//                    lblMessage.Text = "Invalid QR code or missing parameters.";
//                }
//            }
//        }

//        private void MarkAttendance(int scheduleId)
//        {
//            try
//            {
//                int studentId = Convert.ToInt32(Session["People_ID"]);
//                string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
//                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

//                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);
//                    cmd.Parameters.AddWithValue("@People_ID", studentId);
//                    cmd.Parameters.AddWithValue("@Attended_Time", DateTime.Now);
//                    cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);

//                    conn.Open();
//                    cmd.ExecuteNonQuery();

//                    lblMessage.Text = "Attendance marked successfully.";
//                }
//            }
//            catch (Exception ex)
//            {
//                lblMessage.Text = "Failed to mark attendance: " + ex.Message;
//            }
//        }

//        //protected void Page_Load(object sender, EventArgs e)
//        //{
//        //    if (!IsPostBack)
//        //    {
//        //        // Check if the query string contains required parameters
//        //        if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]))
//        //        {
//        //            int scheduleId = int.Parse(Request.QueryString["Schedule_ID"]);

//        //            // Check if the student is logged in
//        //            if (Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
//        //            {
//        //                // Student is logged in, mark attendance directly
//        //                MarkAttendance(scheduleId);
//        //            }
//        //            else
//        //            {
//        //                // Save URL parameters in session for redirection after login
//        //                Session["Pending_Schedule_ID"] = scheduleId;
//        //                Response.Redirect("Login.aspx");
//        //            }
//        //        }
//        //        else
//        //        {
//        //            // Invalid access, handle as needed
//        //            lblMessage.Text = "Invalid QR code or missing parameters.";
//        //        }
//        //    }
//        //}

//        //private void MarkAttendance(int scheduleId)
//        //{
//        //    try
//        //    {
//        //        int studentId = Convert.ToInt32(Session["People_ID"]);
//        //        string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

//        //        using (SqlConnection conn = new SqlConnection(connectionString))
//        //        {
//        //            SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
//        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

//        //            cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);
//        //            cmd.Parameters.AddWithValue("@People_ID", studentId);
//        //            cmd.Parameters.AddWithValue("@Attended_Time", DateTime.Now);
//        //            cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);

//        //            conn.Open();
//        //            cmd.ExecuteNonQuery();

//        //            lblMessage.Text = "Attendance marked successfully.";
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        lblMessage.Text = "Failed to mark attendance: " + ex.Message;
//        //    }
//        //}
//    }
//}



////using System;
////using System.Collections.Generic;
////using System.Data.SqlClient;
////using System.Data;
////using System.Linq;
////using System.Web;
////using System.Web.Configuration;
////using System.Web.UI;
////using System.Web.UI.WebControls;

////namespace Qr_Attendance
////{
////    public partial class StudentAttendancePage : System.Web.UI.Page
////    {
////        protected void Page_Load(object sender, EventArgs e)
////        {
////        }

////        protected void btnScanQRCode_Click(object sender, EventArgs e)
////        {
////            try
////            {
////                string qrContent = txtQRCodeContent.Text.Trim(); // Read QR code content
////                Uri uri = new Uri(qrContent);
////                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);


////                int scheduleID = int.Parse(queryParams["Schedule_ID"]);
////                //string startTime = queryParams["StartTime"];
////                //string endTime = queryParams["EndTime"];
////                //int peopleID = int.Parse(queryParams["People_ID"]);
////                //string guideID = queryParams["Guide_ID"];

////                MarkAttendance(scheduleID); // Mark attendance
////            }
////            catch (Exception ex)
////            {
////                lblMessage.Text = "Invalid QR code content.";
////            }
////        }

////        private void MarkAttendance(int scheduleID)
////        {
////            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
////            using (SqlConnection conn = new SqlConnection(connectionString))
////            {
////                SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
////                cmd.CommandType = CommandType.StoredProcedure;

////                cmd.Parameters.AddWithValue("@Schedule_ID", scheduleID);
////                cmd.Parameters.AddWithValue("@People_ID", 3);
////                //cmd.Parameters.AddWithValue("@Start_Time", DateTime.Parse(startTime));
////                //cmd.Parameters.AddWithValue("@End_Time", DateTime.Parse(endTime));

////                cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);
////                cmd.Parameters.AddWithValue("@Attended_Time", DateTime.Now);
////                //cmd.Parameters.AddWithValue("@Status", 1);

////                try
////                {
////                    conn.Open();
////                    cmd.ExecuteNonQuery();
////                    lblMessage.Text = "Attendance marked successfully.";
////                }
////                catch (Exception ex)
////                {
////                    lblMessage.Text = "Failed to mark attendance: " + ex.Message;
////                }
////            }
////        }

////    }
////}


using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace Qr_Attendance
{
    public partial class StudentAttendancePage : System.Web.UI.Page
    {



        //    protected void Page_Load(object sender, EventArgs e)
        //    {
        //        if (!IsPostBack)
        //        {
        //            IsUserLoggedIn();

        //            // Redirect to login if the user is not logged in
        //            if (!IsUserLoggedIn())
        //            {
        //                HandleLoginRedirect();
        //                return; // Stop further processing
        //            }

        //            // User is logged in, validate the QR code parameters
        //            if (!TryParseQRCode(out int scheduleId, out DateTime startTime, out DateTime endTime))
        //            {
        //                return; // Stop further processing if validation fails
        //            }

        //            // Check if the QR code has expired
        //            if (IsQRCodeExpired(endTime))
        //            {
        //                lblMessage.Text = "Error: This QR code has expired. Please scan a new QR code.";
        //                return; // Stop further processing
        //            }

        //            // Proceed to mark attendance
        //            MarkAttendance(scheduleId);
        //        }
        //    }

        //    private bool IsUserLoggedIn()
        //    {
        //        return Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1;
        //    }

        //    private void HandleLoginRedirect()
        //    {
        //        if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]))
        //        {
        //            int scheduleId;
        //            if (int.TryParse(Request.QueryString["Schedule_ID"], out scheduleId))
        //            {
        //                Session["Schedule_ID"] = scheduleId;
        //                Response.Redirect($"Login.aspx?Schedule_ID={scheduleId}", false);
        //                HttpContext.Current.ApplicationInstance.CompleteRequest();
        //            }
        //            else
        //            {
        //                lblMessage.Text = "Invalid Schedule_ID format.";
        //            }
        //        }
        //        else
        //        {
        //            lblMessage.Text = "Invalid QR code or missing Schedule_ID.";
        //        }
        //    }






        //    private bool TryParseQRCode(out int scheduleId, out DateTime startTime, out DateTime endTime)
        //    {
        //        scheduleId = 0;
        //        startTime = DateTime.MinValue;
        //        endTime = DateTime.MinValue;

        //        // Check for required parameters in the query string
        //        if (string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]) ||
        //            string.IsNullOrEmpty(Request.QueryString["StartTime"]) ||
        //            string.IsNullOrEmpty(Request.QueryString["EndTime"]))
        //        {
        //            lblMessage.Text = "Invalid QR code or missing parameters.";
        //            System.Diagnostics.Debug.WriteLine("QR Code validation failed: Missing parameters.");
        //            return false;
        //        }

        //        // Parse Schedule_ID
        //        if (!int.TryParse(Request.QueryString["Schedule_ID"], out scheduleId) || scheduleId <= 0)
        //        {
        //            lblMessage.Text = "Invalid Schedule_ID format.";
        //            System.Diagnostics.Debug.WriteLine($"Invalid Schedule_ID: {Request.QueryString["Schedule_ID"]}");
        //            return false;
        //        }

        //        // Parse StartTime
        //        if (!DateTime.TryParse(Request.QueryString["StartTime"], out startTime))
        //        {
        //            lblMessage.Text = $"Invalid StartTime format: {Request.QueryString["StartTime"]}";
        //            System.Diagnostics.Debug.WriteLine($"Failed to parse StartTime: {Request.QueryString["StartTime"]}");
        //            return false;
        //        }

        //        // Parse EndTime
        //        if (!DateTime.TryParse(Request.QueryString["EndTime"], out endTime))
        //        {
        //            lblMessage.Text = $"Invalid EndTime format: {Request.QueryString["EndTime"]}";
        //            System.Diagnostics.Debug.WriteLine($"Failed to parse EndTime: {Request.QueryString["EndTime"]}");
        //            return false;
        //        }

        //        System.Diagnostics.Debug.WriteLine($"Parsed QR Code: Schedule_ID={scheduleId}, StartTime={startTime}, EndTime={endTime}");
        //        return true;
        //    }



        //    private bool IsQRCodeExpired(DateTime endTime)
        //    {
        //        bool expired = DateTime.Now > endTime;
        //        if (expired)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"QR Code expired. Current time: {DateTime.Now}, EndTime: {endTime}");
        //        }
        //        return expired;

        //}




        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                //Log People_ID to browser console
                if (Session["People_ID"] != null)
                {
                    string script = $"<script>console.log('Logged-in People_ID: {Session["People_ID"]}');</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "LogPeopleID", script);
                }
                else
                {
                    string script = "<script>console.log('No People_ID found in session.');</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "LogNoPeopleID", script);
                }

                // Check if Schedule_ID is passed in the query string
                if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]))
                {
                    int scheduleId;
                    if (int.TryParse(Request.QueryString["Schedule_ID"], out scheduleId))
                    {
                        if (Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
                        {
                            //  Student is logged in, mark attendance directly
                            MarkAttendance(scheduleId);
                        }
                        else
                        {
                            //Save Schedule_ID in session and redirect to Login
                            Session["Schedule_ID"] = scheduleId;
                            Response.Redirect($"Login.aspx?Schedule_ID={scheduleId}");
                        }
                    }
                    else
                    {
                        lblMessage.Text = "Invalid Schedule_ID format.";
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid QR code or missing parameters.";
                }


            }
        }

        private void MarkAttendance(int scheduleId)
        {
            try
            {
                // Validate session data
                if (Session["People_ID"] == null || Session["Role"] == null)
                {
                    lblMessage.Text = "Session expired or not set. Please log in again.";
                    return;
                }

                int studentId = Convert.ToInt32(Session["People_ID"]);
                string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);
                    cmd.Parameters.AddWithValue("@People_ID", studentId);
                    cmd.Parameters.AddWithValue("@Attended_Time", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);


                    // Add the output parameter for Error_Code_ID
                    SqlParameter errorCodeParam = new SqlParameter("@Error_Code_ID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(errorCodeParam);

                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        int errorCode = errorCodeParam.Value != DBNull.Value ? (int)errorCodeParam.Value : 0;
                        string consoleMessage = $"console.log('Attendance Attempt - Schedule_ID: {scheduleId}, People_ID: {studentId}, Error_Code_ID: {errorCode}');";
                        ScriptManager.RegisterStartupScript(this, GetType(), "LogErrorCode", consoleMessage, true);

                        // Display error messages based on Error_Code_ID
                      //  string errorMessage = GetErrorMessage(errorCode);
                        lblMessage.Text = GetErrorMessage(errorCode);
                        //lblMessage.Text = "Attendance marked successfully.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
                     $"console.log('Attendance logged successfully for Schedule_ID: {scheduleId} at {DateTime.Now}.');", true);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("Error: The QR code has expired"))
                        {
                            lblMessage.Text = "Error: The QR code has expired. Please scan a new QR code.";
                        }
                        else
                        {
                            lblMessage.Text = "Failed to mark attendance: " + ex.Message;
                        }
                    }
             
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to mark attendance: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error in MarkAttendance: " + ex.ToString());
            }
        }
        private string GetErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case 1: return "Attendance Recorded Successfully";
                case 2: return "Expired";
                case 3: return "Not Registered";
                case 4: return "Already Marked Attendance";
                case 5: return "Lecture has not started";
                case 6: return "Lecture has ended";
                case 7: return "There is no lecture for today";
                default: return "Unknown error occurred.";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            // Check for pending Schedule_ID after login
            if (Session["Schedule_ID"] != null && Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
            {
                int schedule_ID = Convert.ToInt32(Session["Schedule_ID"]);
                MarkAttendance(schedule_ID);
                Session.Remove("Schedule_ID"); // Clear session only after marking attendance
            }

            base.OnPreRender(e);
        }
    }
}

