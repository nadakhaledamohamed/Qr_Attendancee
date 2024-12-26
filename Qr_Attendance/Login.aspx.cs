using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Qr_Attendance

{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // Session.Clear(); // Clear session on page load
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
           

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT People_ID, Role FROM People WHERE Username = @Username AND Password = @Password", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    int peopleId = Convert.ToInt32(reader["People_ID"]);
                    int role = Convert.ToInt32(reader["Role"]);

                    // Save session data
                    Session["People_ID"] = peopleId;
                    Session["Role"] = role;

                    if (role == 1) // Student role
                    {
                        int? scheduleId = GetCurrentScheduleIDForStudent(peopleId);
                        if (scheduleId != null)
                        {
                            // Save student-specific session data
                            Session["Student_Schedule_ID"] = scheduleId;
                            Session["Student_People_ID"] = peopleId;

                            // Redirect to the aattendance page
                            Response.Redirect($"StudentAttendancePage.aspx?Schedule_ID={scheduleId},Peopl_ID={peopleId}");
                        }
                        else
                        {
                            Session["Student_People_ID"] = peopleId;
                            Response.Redirect($"StudentAttendancePage.aspx");
                        }
                    }
                    else if (role == 2) // Doctor role
                    {
                       
                        // Save doctor-specific session data
                        Session["Doctor_ID"] = peopleId;

                        // Redirect to the doctor home page or QR Code page
                        Response.Redirect("DoctorHomePage.aspx");
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid username or password.";
                }
            }
        }

        // Helper method to get current schedule for the student
        private int? GetCurrentScheduleIDForStudent(int studentId)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetSchedualeForStd", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@People_ID", studentId);

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

    }
}
