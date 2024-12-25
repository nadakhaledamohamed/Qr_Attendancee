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
                // Log the People_ID for debugging
                if (Session["People_ID"] != null)
                {
                    string script = $"<script>console.log('Logged-in People_ID: {Session["People_ID"]}');</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "LogPeopleID", script);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // Retrieve Schedule_ID and StartTime from the query string
                string scheduleIdString = Request.QueryString["Schedule_ID"];
                string startTimeString = Request.QueryString["StartTime"];

                if (!string.IsNullOrEmpty(scheduleIdString) && int.TryParse(scheduleIdString, out int scheduleId) &&
                    !string.IsNullOrEmpty(startTimeString) && DateTime.TryParse(startTimeString, out DateTime startTime))
                {
                    if (Session["Role"] != null && Convert.ToInt32(Session["Role"]) == 1)
                    {
                        // If the student is logged in, mark attendance directly
                        MarkAttendance(scheduleId, startTime);
                    }
                    else
                    {
                        // If the role is not student, redirect to login
                        Response.Redirect("Login.aspx");
                    }
                }
                else
                {
                    // If Schedule_ID is not passed, try to fetch it dynamically
                    int? fetchedScheduleId = GetCurrentScheduleID();
                    if (fetchedScheduleId.HasValue)
                    {
                        Response.Redirect($"StudentAttendancePage.aspx?Schedule_ID={fetchedScheduleId.Value}&StartTime={DateTime.Now:yyyy-MM-ddTHH:mm:ss}");
                    }
                    else
                    {
                        lblMessage.Text = "No schedule found for today.";
                    }
                }
            }
        }

        private int? GetCurrentScheduleID()
        {
            // Retrieve the logged-in user's People_ID from the session
            int? loggedInPeopleId = Session["People_ID"] != null ? Convert.ToInt32(Session["People_ID"]) : (int?)null;

            if (loggedInPeopleId == null)
            {
                Response.Redirect("Login.aspx");
                return null;
            }

            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetSchedualeForStd", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@People_ID", loggedInPeopleId.Value);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return Convert.ToInt32(reader["Schedule_ID"]);
                }
                else
                {
                    return null;
                }
            }
        }

        private void MarkAttendance(int scheduleId, DateTime startTime)
        {
            try
            {
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
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to mark attendance: " + ex.Message;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Session["Schedule_ID"] != null && Session["StartTime"] != null &&
                Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
            {
                int scheduleId = Convert.ToInt32(Session["Schedule_ID"]);
                DateTime startTime = (DateTime)Session["StartTime"];

                MarkAttendance(scheduleId, startTime);

                Session.Remove("Schedule_ID");
                Session.Remove("StartTime");
            }

            base.OnPreRender(e);
        }
    }
}

//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Globalization;
//using System.Web;
//using System.Web.Configuration;
//using System.Web.UI;

//namespace Qr_Attendance
//{
//    public partial class StudentAttendancePage : System.Web.UI.Page
//    {


//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                string startTimeString = Request.QueryString["StartTime"];
//                // Log People_ID to browser console
//                if (Session["People_ID"] != null)
//                {
//                    string script = $"<script>console.log('Logged-in People_ID: {Session["People_ID"]}');</script>";
//                    ClientScript.RegisterStartupScript(this.GetType(), "LogPeopleID", script);
//                }
//                else
//                {
//                    string script = "<script>console.log('No People_ID found in session.');</script>";
//                    ClientScript.RegisterStartupScript(this.GetType(), "LogNoPeopleID", script);
//                    //Response.Redirect("Login.aspx"); // Redirect to login if People_ID is not set
//                    //return;
//                }


//                int scheduleId = (int)GetCurrentScheduleID();


//                if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"])
//                    && int.TryParse(Request.QueryString["Schedule_ID"], out scheduleId))
//                {
//                    //if (!string.IsNullOrEmpty(startTimeString)
//                    //&& DateTime.TryParse(Request.QueryString["StartTime"], out DateTime startTime))
//                    //{
//                    // If Schedule_ID is passed and valid
//                    if (Session["Role"] != null && Convert.ToInt32(Session["Role"]) == 1)
//                    {
//                        MarkAttendance(scheduleId); // Student is logged in, mark attendance directly
//                    }
//                    else
//                    {

//                        // Session["Pending_StartTime"] = startTime;
//                        Response.Redirect($"Login.aspx");
//                        lblMessage.Text = "Unauthorized access. Only students can mark attendance.";
//                    }
//                }
//                //}
//                else
//                {
//                    // If Schedule_ID is not passed in the query string, fetch it dynamically
//                    int? fetchedScheduleId = GetCurrentScheduleID();
//                    //DateTime startTime;
//                    //startTime = (DateTime)Session["StartTime"];
//                    if (fetchedScheduleId.HasValue)
//                    {

//                        Response.Redirect($"StudentAttendancePage.aspx?Schedule_ID={fetchedScheduleId.Value}");
//                    }
//                    else
//                    {
//                        lblMessage.Text = "No schedule found for today.";
//                    }
//                }
//            }
//        }


//        private int? GetCurrentScheduleID()
//        {
//            // Retrieve the logged-in user's People_ID from the session
//            int? loggedInPeopleId = Session["People_ID"] != null ? Convert.ToInt32(Session["People_ID"]) : (int?)null;

//            if (loggedInPeopleId == null)
//            {
//                // Redirect to login page if the session is not set
//                Response.Redirect("Login.aspx");
//                return null;
//            }

//            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = new SqlCommand("GetSchedualeForStd", conn);
//                cmd.CommandType = CommandType.StoredProcedure;

//                // Use the logged-in user's People_ID
//                cmd.Parameters.AddWithValue("@People_ID", loggedInPeopleId.Value);

//                conn.Open();
//                SqlDataReader reader = cmd.ExecuteReader();

//                if (reader.Read())
//                {
//                    return Convert.ToInt32(reader["Schedule_ID"]); // Return the Schedule_ID for the ongoing lecture
//                }
//                else
//                {
//                    return null; // No ongoing lecture found
//                }
//            }
//        }
//        private void MarkAttendance(int scheduleId)
//        {
//            try
//            {
//                // Validate session data
//                if (Session["People_ID"] == null || Session["Role"] == null)
//                {
//                    lblMessage.Text = "Session expired or not set. Please log in again.";
//                    return;
//                }

//                int studentId = Convert.ToInt32(Session["People_ID"]);

//                string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;


//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
//                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

//                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);
//                    cmd.Parameters.AddWithValue("@People_ID", studentId);
//                    cmd.Parameters.AddWithValue("@Start_time", DateTime.Now); //it is start time fetch from qr code link when student scaned qr code );
//                    cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);

//                    //Error Parameter ///////////////
//                    SqlParameter errorMessageParam = new SqlParameter("@Error_Message", SqlDbType.NVarChar, 255)
//                    {
//                        Direction = ParameterDirection.Output
//                    };
//                    cmd.Parameters.Add(errorMessageParam);

//                    //////////////////

//                    conn.Open();
//                    try
//                    {
//                        cmd.ExecuteNonQuery();

//                        //Error Message 
//                        string errorMessage = errorMessageParam.Value.ToString();
//                        lblMessage.Text = errorMessage;

//                        string consoleMessage = $"console.log('Attendance Attempt - Schedule_ID: {scheduleId}, People_ID: {studentId}');";



//                        ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
//                        $"console.log('Attendance logged successfully for Schedule_ID: {scheduleId} at {DateTime.Now}.  {errorMessage}');", true);
//                    }
//                    catch (SqlException ex)
//                    {
//                        lblMessage.Text = "Failed to mark attendance: " + ex.Message;
//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                lblMessage.Text = "Failed to mark attendance: " + ex.Message;
//                System.Diagnostics.Debug.WriteLine("Error in MarkAttendance: " + ex.ToString());
//            }
//        }

//        protected override void OnPreRender(EventArgs e)
//        {
//            // Check for pending Schedule_ID after login
//            if (Session["Schedule_ID"] != null && Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
//            {
//                int schedule_ID = Convert.ToInt32(Session["Schedule_ID"]);
//                MarkAttendance(schedule_ID);
//                Session.Remove("Schedule_ID"); // Clear session only after marking attendance
//            }

//            base.OnPreRender(e);
//        }

//    }
//}
//{
//    public partial class StudentAttendancePage : System.Web.UI.Page
//    {
//        //protected void Page_Load(object sender, EventArgs e)
//        //{

//        //    if (!IsPostBack)
//        //    {

//        //        //Log People_ID to browser console
//        //        if (Session["People_ID"] != null)
//        //        {
//        //            string script = $"<script>console.log('Logged-in People_ID: {Session["People_ID"]}');</script>";
//        //            ClientScript.RegisterStartupScript(this.GetType(), "LogPeopleID", script);
//        //        }
//        //        else
//        //        {
//        //            string script = "<script>console.log('No People_ID found in session.');</script>";
//        //            ClientScript.RegisterStartupScript(this.GetType(), "LogNoPeopleID", script);
//        //        }

//        //        // Check if Schedule_ID is passed in the query string
//        //        if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]))
//        //        {
//        //            //int peopleId = Convert.ToInt32(Session["People_ID"]);
//        //            //int? scheduleId = GetScheduleIdForToday(peopleId);
//        //            int scheduleId ;
//        //            if (int.TryParse(Request.QueryString["Schedule_ID"], out scheduleId))
//        //            {
//        //                if (Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
//        //                {
//        //                    //  Student is logged in, mark attendance directly
//        //                    MarkAttendance(scheduleId);
//        //                }
//        //                else
//        //                {
//        //                    //Save Schedule_ID in session and redirect to Login
//        //                    Session["Schedule_ID"] = scheduleId;
//        //                    Response.Redirect($"Login.aspx?Schedule_ID={scheduleId}");
//        //                }
//        //            }
//        //            else
//        //            {
//        //                lblMessage.Text = "Invalid Schedule_ID format.";
//        //            }
//        //        }
//        //        else
//        //        {
//        //            lblMessage.Text = "Invalid QR code or missing parameters.";
//        //        }


//        //    }
//        //}


//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                // Log People_ID to browser console
//                if (Session["People_ID"] != null)
//                {
//                    string script = $"<script>console.log('Logged-in People_ID: {Session["People_ID"]}');</script>";
//                    ClientScript.RegisterStartupScript(this.GetType(), "LogPeopleID", script);
//                }
//                else
//                {
//                    string script = "<script>console.log('No People_ID found in session.');</script>";
//                    ClientScript.RegisterStartupScript(this.GetType(), "LogNoPeopleID", script);
//                    //Response.Redirect("Login.aspx"); // Redirect to login if People_ID is not set
//                    //return;
//                }

//                // Check if Schedule_ID is passed in the query string
//                int scheduleId = (int)GetCurrentScheduleID();

//                // DateTime startTime = (DateTime)Session["Pending_StartTime"];
//                string startTimeString = Request.QueryString["StartTime"];

//                if (!string.IsNullOrEmpty(Request.QueryString["Schedule_ID"]) && !string.IsNullOrEmpty(startTimeString))
//                {
//                    if (int.TryParse(Request.QueryString["Schedule_ID"], out  scheduleId) && DateTime.TryParse(startTimeString, out DateTime startTime))
//                    {
//                        // If Schedule_ID is passed and valid

//                        if (Session["Role"] != null && Convert.ToInt32(Session["Role"]) == 1)
//                        {
//                            MarkAttendance(scheduleId, startTime); // Student is logged in, mark attendance directly
//                        }
//                        else
//                        {
//                            //Session["Schedule_ID"] = scheduleId;
//                            //int? fetchedScheduleId = GetCurrentScheduleID();
//                            Session["Pending_StartTime"] = startTime;
//                            Response.Redirect($"Login.aspx");
//                            lblMessage.Text = "Unauthorized access. Only students can mark attendance.";
//                        }
//                    }
//                    else
//                    {
//                        // If Schedule_ID is not passed in the query string, fetch it dynamically
//                        int? fetchedScheduleId = GetCurrentScheduleID();
//                        if (fetchedScheduleId.HasValue)
//                        {
//                            Response.Redirect($"StudentAttendancePage.aspx?Schedule_ID={fetchedScheduleId.Value}");
//                        }
//                        else
//                        {
//                            lblMessage.Text = "No schedule found for today.";
//                        }
//                    }
//                }
//            }
//        }

//        private int? GetCurrentScheduleID()
//        {
//            // Retrieve the logged-in user's People_ID from the session
//            int? loggedInPeopleId = Session["People_ID"] != null ? Convert.ToInt32(Session["People_ID"]) : (int?)null;

//            if (loggedInPeopleId == null)
//            {
//                // Redirect to login page if the session is not set
//                Response.Redirect("Login.aspx");
//                return null;
//            }

//            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = new SqlCommand("GetSchedualeForStd", conn);
//                cmd.CommandType = CommandType.StoredProcedure;

//                // Use the logged-in user's People_ID
//                cmd.Parameters.AddWithValue("@People_ID", loggedInPeopleId.Value);
//                cmd.Parameters.AddWithValue("@role", 1);
//                conn.Open();
//                SqlDataReader reader = cmd.ExecuteReader();

//                if (reader.Read())
//                {
//                    return Convert.ToInt32(reader["Schedule_ID"]); // Return the Schedule_ID for the ongoing lecture
//                }
//                else
//                {
//                    return null; // No ongoing lecture found
//                }
//            }
//        }
//        private void MarkAttendance(int scheduleId, DateTime startTime)
//        {
//            try
//            {
//                // Validate session data
//                if (Session["People_ID"] == null || Session["Role"] == null)
//                {
//                    lblMessage.Text = "Session expired or not set. Please log in again.";
//                    return;
//                }

//                int studentId = Convert.ToInt32(Session["People_ID"]);
//                string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    SqlCommand cmd = new SqlCommand("MarkAttendance", conn);
//                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

//                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);
//                    cmd.Parameters.AddWithValue("@People_ID", studentId);
//                    cmd.Parameters.AddWithValue("@Start_Time", startTime); //it is start time fetch from qr code link when student scaned qr code );
//                    cmd.Parameters.AddWithValue("@Browser_Data", Request.UserAgent);

//                    //Error Parameter ///////////////
//                    SqlParameter errorMessageParam = new SqlParameter("@Error_Message", SqlDbType.NVarChar, 255)
//                    {
//                        Direction = ParameterDirection.Output
//                    };
//                    cmd.Parameters.Add(errorMessageParam);

//                    //////////////////

//                    conn.Open();
//                    try
//                    {
//                        cmd.ExecuteNonQuery();

//                        //Error Message 
//                        string errorMessage = errorMessageParam.Value.ToString();
//                        lblMessage.Text = errorMessage;

//                        string consoleMessage = $"console.log('Attendance Attempt - Schedule_ID: {scheduleId}, People_ID: {studentId}');";

//                        ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
//                        $"console.log('Attendance logged successfully for Schedule_ID: {scheduleId}, People_ID: {studentId}, StartTime: {startTime}');", true);



//                        ScriptManager.RegisterStartupScript(this, GetType(), "AttendanceLogged",
//                                $"console.log('Attendance logged successfully for Schedule_ID: {scheduleId} at {DateTime.Now}.  {errorMessage}');", true);
//                    }

//                    catch (SqlException ex)
//                    {
//                        lblMessage.Text = "Failed to mark attendance: " + ex.Message;
//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                lblMessage.Text = "Failed to mark attendance: " + ex.Message;
//                System.Diagnostics.Debug.WriteLine("Error in MarkAttendance: " + ex.ToString());
//            }
//        }

//        protected override void OnPreRender(EventArgs e)
//        {
//            // Check for pending Schedule_ID and StartTime after login
//            if (Session["Schedule_ID"] != null && Session["Pending_StartTime"] != null && Session["People_ID"] != null && Convert.ToInt32(Session["Role"]) == 1)
//            {
//                int scheduleId = Convert.ToInt32(Session["Schedule_ID"]);
//                DateTime startTime = (DateTime)Session["Pending_StartTime"];
//                MarkAttendance(scheduleId, startTime);

//                // Clear session data after marking attendance
//                Session.Remove("Schedule_ID");
//                Session.Remove("Pending_StartTime");
//            }

//            base.OnPreRender(e);
//        }

//    }
//}

