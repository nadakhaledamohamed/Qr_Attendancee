using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
                if (Session["Student_People_ID"] == null)
                {
                    // Redirect the student to their home page or display an error message
                    
                    Response.Redirect("login.aspx");


                }
                else if (Session["Student_Schedule_ID"] == null)
                {
                    int studentId = Convert.ToInt32(Session["Student_People_ID"]);
                    DateTime startTime = Session["StartTime"] != null ? (DateTime)Session["StartTime"] : DateTime.Now;

                    // Attempt to mark attendance
                    MarkAttendance(studentId, startTime);
                }
                }

            }


         


        private void MarkAttendance(int? scheduleId , DateTime startTime)
        {
            //try
            //{
                //if (Session["Student_People_ID"] == null || Session["Role"] == null)
                //{
                //    lblMessage.Text = "Session expired or not set. Please log in again.";
                //    return;
                //}

                   scheduleId = Session["Student_Schedule_ID"] != null
                    ? (int?)Convert.ToInt32(Session["Student_Schedule_ID"])
                    : null;

                if (scheduleId == null)
                {
                    lblMessage.Text = "No valid schedule found.";
              //  return;
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

                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adpt.Fill(dt);

                    // Fetch the error message from the DataTable
                    string errorMessage = errorMessageParam.Value?.ToString();

                    // Display error message or success message
                    lblMessage.Text = string.IsNullOrEmpty(errorMessage) ? "Attendance marked successfully." : errorMessage;

                    // Debug log for the console
                    ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLog",
                        $"console.log('Attendance Status: {lblMessage.Text}. Schedule_ID: {scheduleId}, People_ID: {studentId}, StartTime: {startTime}.');", true); ;
                }
           


              //  }

            //catch (Exception ex)
            //{
            //    lblMessage.Text = "Failed to mark attendance: " + ex.Message;
            //}

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
                lblMessage.Text = "An error occurred while rendering the page: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "RenderErrorLog",
                    $"console.log('OnPreRender error: {ex.Message}');", true);
            }

            base.OnPreRender(e);
        }



    }
}



