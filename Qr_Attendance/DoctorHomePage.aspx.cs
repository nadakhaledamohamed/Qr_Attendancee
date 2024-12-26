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
            return Session["Doctor_ID"] != null ? Convert.ToInt32(Session["Doctor_ID"]) : (int?)null;
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
            int? loggedInPeopleId = Session["Doctor_ID"] != null ? Convert.ToInt32(Session["Doctor_ID"]) : (int?)null;

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
                try
                {
                    conn.Open();

                    // Insert attendance record
                    SqlCommand cmd = new SqlCommand("INsertAttendance", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@People_ID", doctorId);
                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleID.Value);
                    cmd.Parameters.AddWithValue("@Status", 1); // Status: 1 for attended
                    cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Attended_Time", DateTime.Now);


                    //read start_time and end_time 
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime startTime = (DateTime)reader["Start_Time"]; // read start time and convert to date time 
                            DateTime endTime = (DateTime)reader["End_Time"]; // read end time and convert to date time 

                            ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
                                $"console.log('Attendance logged successfully for Schedule_ID: {scheduleID.Value} with Start_Time: {startTime} and End_Time: {endTime}.');",
                                true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "NoTimesFound",
                                "console.log('No Start_Time or End_Time found for the specified Schedule_ID.');", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as necessary
                    ScriptManager.RegisterStartupScript(this, GetType(), "ErrorLoggingAttendance",
                        $"console.log('Error logging attendance: {ex.Message}');", true);
                }
            }
        }
    }
}
