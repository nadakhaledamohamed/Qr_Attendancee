using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace Qr_Attendance
{
    public partial class StudentAttendancePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            
                if (Session["Student_People_ID"] == null || Session["Student_Schedule_ID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }


                // DateTime startTime;
                // int studentId = Convert.ToInt32(Session["Student_People_ID"]);
                //DateTime startTime = (DateTime)Session["StartTime"];
                ////  Validate StartTime passed from query string of qr code 
                //if (!string.IsNullOrEmpty(Request.QueryString["StartTime"]) &&
                //    DateTime.TryParse(Request.QueryString["StartTime"], out startTime))
                //{
                    
                //    lblMessage.Text = "Invalid or missing StartTime in the QR code.";
                //   // int scheduleId = Convert.ToInt32(Session["Student_Schedule_ID"]);
                //    if (Session["Role"] != null && Convert.ToInt32(Session["Role"]) == 1 && 
                //        Session["Student_People_ID"] != null && Session["Student_Schedule_ID"] != null)
                //    {
                //        // If the student is logged in, mark attendance directly
                //        int scheduleId = Convert.ToInt32(Session["Student_Schedule_ID"]);
                      
                //         MarkAttendance(scheduleId, startTime);

                //        string script = $"<script>console.log('Logged-in People_ID: {Session["Student_People_ID"]}, Scheduale_ID_of_Student: {Session["Student_Schedule_ID"]}');</script>";
                //          ClientScript.RegisterStartupScript(this.GetType(), "LogPeopleID", script);
                //    }
                //    else
                //    {
                //        lblMessage.Text = "Invalid or missing Data.";
                //        //  Response.Redirect($"Login.aspx");
                //    }
                //}
               
            }


            }
     
      

        private void MarkAttendance(int? scheduleId , DateTime startTime)
        {
            try
            {
                if (Session["Student_People_ID"] == null || Session["Role"] == null)
                {
                    lblMessage.Text = "Session expired or not set. Please log in again.";
                    return;
                }

                int studentId = Convert.ToInt32(Session["Student_People_ID"]);
                string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);
                    cmd.Parameters.AddWithValue("@People_ID", studentId);
                    cmd.Parameters.AddWithValue("@Start_time", startTime);
                    cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);

                    SqlParameter errorMessageParam = new SqlParameter("@Error_Message", SqlDbType.NVarChar, 255)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(errorMessageParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    string errorMessage = errorMessageParam.Value.ToString();
                    lblMessage.Text = string.IsNullOrEmpty(errorMessage) ? "Attendance marked successfully." : errorMessage;
                       ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
                               $"console.log('Attendance logged successfully for Schedule_ID: {scheduleId} at {startTime}.  {errorMessage}');", true);



                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to mark attendance: " + ex.Message;
            }
        }
        protected override void OnPreRender(EventArgs e) //OnPreRender runs every time the page is rendered
        {
            try
            {
                if (Session["Student_Schedule_ID"] != null && Session["StartTime"] != null &&
                    Session["Student_People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
                {
                    int scheduleId = Convert.ToInt32(Session["Student_Schedule_ID"]);
                    DateTime startTime = (DateTime)Session["StartTime"];

                    // Validate StartTime to compare it with sql when it gives me 000 
                    if (startTime < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue)
                    {
                        lblMessage.Text = "Invalid StartTime in session. Please try scanning the QR code again.";
                        return;
                    }

                    // Mark attendance
                    MarkAttendance(scheduleId, startTime);

                    // Clear session variables after marking attendance
                    //Session.Remove("Student_Schedule_ID");
                    //Session.Remove("StartTime");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred: " + ex.Message;
            }

            base.OnPreRender(e);
        }



    }
}



