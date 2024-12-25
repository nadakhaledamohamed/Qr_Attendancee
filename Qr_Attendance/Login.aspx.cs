using System;
using System.Collections.Generic;
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
                Session.Clear(); // Clear session on page load
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
                        // Check if Schedule_ID exists in query string or session
                        string scheduleId = Request.QueryString["Schedule_ID"];
                        if (!string.IsNullOrEmpty(scheduleId))
                        {
                            // Redirect back to StudentAttendancePage with Schedule_ID from query string
                            Response.Redirect($"StudentAttendancePage.aspx?Schedule_ID={scheduleId}");
                        }
                        else if (Session["Schedule_ID"] != null)
                        {
                            // Redirect back to StudentAttendancePage with Schedule_ID from session
                            scheduleId = Session["Schedule_ID"].ToString();
                            Response.Redirect($"StudentAttendancePage.aspx?Schedule_ID={scheduleId}");
                        }
                        else
                        {
                            // Default student home page
                            Response.Redirect("StudentAttendancePage.aspx");
                        }
                    }
                    else if (role == 2) // Doctor role
                    {
                        Response.Redirect("DoctorHomePage.aspx");
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid username or password.";
                }
            }
        }
    }
}