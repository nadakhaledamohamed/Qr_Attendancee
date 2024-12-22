using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Qr_Attendance
{
    public partial class DoctorHomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int? loggedInPeopleId = GetLoggedInUserId();
                if (loggedInPeopleId == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                BindDoctorLectures(loggedInPeopleId.Value);
            }
        }

        private int? GetLoggedInUserId()
        {
            return Session["People_ID"] != null ? Convert.ToInt32(Session["People_ID"]) : (int?)null;
        }

        private void BindDoctorLectures(int peopleId)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("dbo.GetTodaysLecturesForDoctor", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@People_ID", peopleId);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvLectures.DataSource = dt;
                gvLectures.DataBind();

                btnGenerateQR.Visible = dt.Rows.Count > 0;
            }
        }

        protected void btnGenerateQR_Click(object sender, EventArgs e)
        {
            int? loggedInPeopleId = GetLoggedInUserId();
            if (loggedInPeopleId == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int? scheduleID = GetCurrentScheduleID();

            if (scheduleID == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoLecture",
                    "console.log('No ongoing lecture found.');", true);
                return;
            }

            Session["Schedule_ID"] = scheduleID;
            Session["Doctor_ID"] = loggedInPeopleId.Value;
            //autoRefreshQRCode = true;
            LogAttendance(loggedInPeopleId.Value);
            Response.Redirect("QRCodePage.aspx");
        }
        //protected void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (autoRefreshQRCode)
        //    {
        //        string logMessage = $"QR Code refreshed at {DateTime.Now.ToString("hh:mm:ss tt")}";
        //        ScriptManager.RegisterStartupScript(this, GetType(), "LogToConsole", $"console.log('{logMessage}');", true);
        //        Response.Redirect("QRCodePage.aspx");
        //    }
        //}
        private int? GetCurrentScheduleID()
        {
            // Retrieve the logged-in user's People_ID from the session
            int? loggedInPeopleId = Session["People_ID"] != null ? Convert.ToInt32(Session["People_ID"]) : (int?)null;

            if (loggedInPeopleId == null)
            {
                // Redirect to login page if the session is not set
                Response.Redirect("Login.aspx");
                return null;
            }

            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetTodaysLecturesForDoctor", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Use the logged-in user's People_ID
                cmd.Parameters.AddWithValue("@People_ID", loggedInPeopleId.Value);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return Convert.ToInt32(reader["Schedule_ID"]); // Return the Schedule_ID for the ongoing lecture
                }
                else
                {
                    return null; // No ongoing lecture found
                }
            }
        }



        private void LogAttendance(int doctorId)
        {
            int? scheduleID = GetCurrentScheduleID();

            if (scheduleID == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoLectureLog",
                    "console.log('No ongoing lecture found. Attendance log not created.');", true);
                return;
            }

            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Retrieve the End_Time as TIME from the Schedule table
                SqlCommand fetchEndTimeCmd = new SqlCommand(
                    "SELECT End_Time FROM Schedule WHERE Schedule_ID = @Schedule_ID", conn);
                fetchEndTimeCmd.Parameters.AddWithValue("@Schedule_ID", scheduleID.Value);

                conn.Open();
                object endTimeObj = fetchEndTimeCmd.ExecuteScalar();

                DateTime? endTime = null;
                if (endTimeObj != null && endTimeObj != DBNull.Value)
                {
                    TimeSpan endTimeSpan = (TimeSpan)endTimeObj; // Cast to TimeSpan
                    endTime = DateTime.Today.Add(endTimeSpan); // Combine with today's date
                }

                // Insert attendance record
                SqlCommand cmd = new SqlCommand("INsertAttendance", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@People_ID", doctorId);
                cmd.Parameters.AddWithValue("@Schedule_ID", scheduleID.Value);
                cmd.Parameters.AddWithValue("@Start_Time", DateTime.Now);
                cmd.Parameters.AddWithValue("@End_Time", endTime.HasValue ? (object)endTime.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", 1); // Status: 1 for attended
                cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);
                cmd.Parameters.AddWithValue("@Attended_Time", DateTime.Now);

                cmd.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
                    $"console.log('Attendance logged successfully for Schedule_ID: {scheduleID.Value} at {DateTime.Now}.');", true);
            }
        }
    }
}