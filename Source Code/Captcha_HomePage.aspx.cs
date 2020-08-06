using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace CS255_TextCAPTCHA
{
    public partial class Captcha_Page : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.SetFocus(txtbox_captcha);
            if (!IsPostBack)
            {
                Session["Timer"] = DateTime.Now.AddMinutes(0).ToString();
                Session["incorrectSubmission"] = null;
                lbl_msgresult.Text = "";
                InitiateCAPTCHAprocess();
            }
        }

        public void InitiateCAPTCHAprocess()
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myconnectionstring1"].ConnectionString);
            String UUID = "";
            int regenattempts = 0;
            int numofattempts = 1;
            if (Session["uuid"] ==null)   // Validates if the CAPTCHA is generated for the new user
            {
                UUID = Guid.NewGuid().ToString();
                Session["uuid"] = UUID;
            }
            else    // Indicating CAPTCHA is regenerated for the same user again.Incrementing the regeneration attempt by 1 
            {
                UUID = Session["uuid"].ToString();   
                con.Open();
                try
                {
                    SqlDataAdapter sqladpt = new SqlDataAdapter("Select numofattempts,regeneration_attempts from user_attemptdetails where userid = '" + UUID + "' order by ActivityDatetime desc", con);
                    DataTable dt = new DataTable();
                    sqladpt.Fill(dt);
                    con.Close();
                    numofattempts = 1;
                    if (Session["regenerate_CAPTCHA"]!=null)
                    {
                        numofattempts = Convert.ToInt32(dt.Rows[0]["numofattempts"]);
                        regenattempts = Convert.ToInt32(dt.Rows[0]["regeneration_attempts"]) + 1;
                    }
                    if(Session["incorrectSubmission"] != null)
                    {
                        numofattempts = Convert.ToInt32(dt.Rows[0]["numofattempts"]) + 1; // Indicates the same user.
                    }
                    Session["regenerate_CAPTCHA"] = null;
                    Session["incorrectSubmission"] = null;
                }
                catch(Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", ex.Message + "');", true);
                }
            }

            // Regenerate new CAPTCHA

            string CaptchText = GenerateCAPTCHAtext();
            Session["CaptchaChar"] = CaptchText;
            img_captcha.ImageUrl = "Captcha_Image.aspx";

            //*** Store CAPTCHA details in Database  ***
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    SqlDataAdapter sqladpt = new SqlDataAdapter();
                    SqlCommand sqlcmd = new SqlCommand(
                                     "INSERT INTO user_attemptdetails (userid,captchatext,timetaken,regeneration_attempts,numofattempts,ActivityDatetime) " +
                                      "VALUES (@usrid,@captchatxt,@timetkn,@regenattmpts,@numattempts,@datetime)", con);
                    sqlcmd.Parameters.AddWithValue("@usrid", UUID);
                    sqlcmd.Parameters.AddWithValue("@captchatxt", CaptchText);
                    sqlcmd.Parameters.AddWithValue("@timetkn", "00:00:00");
                    sqlcmd.Parameters.AddWithValue("@regenattmpts", regenattempts);
                    sqlcmd.Parameters.AddWithValue("@numattempts", numofattempts);
                    sqlcmd.Parameters.AddWithValue("@datetime", DateTime.Now);
                    sqladpt.InsertCommand = sqlcmd;
                    sqlcmd.ExecuteNonQuery();
                    con.Close();
                }
                catch(Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", ex.Message + "');", true);
                }
            }
        }
           

        /* Using RNGCryptoServiceProvider Class to generate Random Numbers
           RNG works on the basis of OS Entropy (Key Press, Thermal, Mouse Click, Computer Sound) as 
           seed value which will utilized by CRYTO Class to genrate secure random number
         */
    
        // Method to Submit the CAPTCHA puzzle
        protected void btn_CAPTCHAsubmit_Click(object sender, EventArgs e)
        {
            // Update the Time Consumed by the user to solve the CAPTCHA problem
            try
            {

                string timetracker = Session["timetracker"].ToString();
                string minutes = timetracker.Substring(0, 2).Replace(" ","");
                string seconds= timetracker.Substring(timetracker.Length-10,2).Replace(" ", "");
                if(seconds.Length==1)
                {
                    seconds = "0" + minutes;
                }
                if(minutes.Length == 1)
                {
                    minutes = "0" + minutes;
                }
                SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myconnectionstring1"].ConnectionString);
                SqlDataAdapter sqladpt = new SqlDataAdapter();
                SqlCommand sqlcmd = new SqlCommand();
                con.Open();
                String sqlquery = null;
                sqlquery = "Update user_attemptdetails set timetaken ='00:" + minutes +":" + seconds +"' where userid = '" + Session["uuid"].ToString() + "' and captchatext ='" + Session["CaptchaChar"].ToString() +"'";
                sqladpt.UpdateCommand = con.CreateCommand();
                sqladpt.UpdateCommand.CommandText = sqlquery;
                sqladpt.UpdateCommand.ExecuteNonQuery();
                con.Close();
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", ex.Message + "');", true);
            }

            Session["currentUser"] = Session["uuid"].ToString();
            // If the Submitted CAPTCHA is Correct
            if (Session["CaptchaChar"].ToString()== txtbox_captcha.Text)
            {
                lbl_msgresult.ForeColor = Color.Green;
                lbl_msgresult.Text = "Congratulation!!! your answer is correct.";
                Session["uuid"] = null;
                Session["regenerate_CAPTCHA"] = null;
                Session["CaptchaChar"] = null;

            }
            else
            {
                lbl_msgresult.ForeColor = Color.Blue;
                lbl_msgresult.Text = "Your answer is incorrect. Please Try again.";
                Session["incorrectSubmission"] = "yes";
            }
            txtbox_captcha.Text = "";
            InitiateCAPTCHAprocess();
            Session["Timer"] = DateTime.Now.AddMinutes(0).ToString();
        }

        // Submitting the feedback indicates the user is exiting the application. 
        protected void btn_feedbacksubmit_Click(object sender, EventArgs e)
        {
            if (Session["currentUser"] ==null)
            {
                Session["currentUser"] = Session["uuid"].ToString();
            }
            
            // Insert the user feedback into database
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myconnectionstring1"].ConnectionString);
            con.Open();
            try
            {
                SqlDataAdapter sqladpt = new SqlDataAdapter();
                SqlCommand sqlcmd = new SqlCommand(
                                 "INSERT INTO [user_feedback] (userid,feedback) " +
                                  "VALUES (@usrid,@feedback)", con);

                sqlcmd.Parameters.AddWithValue("@usrid", Session["currentUser"].ToString());
                sqlcmd.Parameters.AddWithValue("@feedback", rdbtn_feedback.SelectedValue.ToString());
                sqladpt.InsertCommand = sqlcmd;
                sqlcmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", ex.Message + "');", true);
            }
            Session["uuid"] = null;
            Session["regenerate_CAPTCHA"] = null;
            Session["CaptchaChar"] = null;
            lbl_msgresult.Text = "";

            rdbtn_feedback.ClearSelection();
            InitiateCAPTCHAprocess();
            Session["Timer"] = DateTime.Now.AddMinutes(0).ToString();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Your feedback has been recorded successfully!')", true);

        }


        // Method to Regenerate a new CAPTCHA
        protected void lnk_refresh_Click(object sender, EventArgs e)
        {
            Session["regenerate_CAPTCHA"] = "yes";
            Response.Redirect("~/Captcha_HomePage.aspx");
        }

        public static byte[] RandomNumberGenerator()
        {
            RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider();
            byte[] bytesarray = new byte[100];  // Definining the size of 100 
            rg.GetBytes(bytesarray);
            return bytesarray;
        }

        // Method that genearates random CAPTCHA Characters
        public static string GenerateCAPTCHAtext()
        {
            //string charpool = "abcdefhijkmnpqrstuwxzABCDEFGHJKLMNPQRSTUWXZ2345678";
           // Having Ambigious Characters 
            string charpool = "abcdefhijklmnopqrstuwxzABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            char[] arrychar = new char[62];
            Random rdm = new Random();
            int sizeCharpool = charpool.Length;
            for (int counter = 0; counter < sizeCharpool; counter++)
            {
                int randomindex = rdm.Next(0, charpool.Length);
                char randomcharacter = charpool[randomindex];
                arrychar[counter] = randomcharacter;
                charpool = charpool.Remove(randomindex, 1);
            }

            // ** Uniform and Random Number Generation for CAPTCHA
            // Using  RNGCryptoServiceProvider service to choose true Random numbers

            byte[] bytesarray = RandomNumberGenerator();
            String captcha_text = "";
            int captcha_size = 6;    // pre defined the size of CAPTCHA 
            for (int counter = 0; counter < bytesarray.Length; counter++)
            {
                if (bytesarray[counter] < sizeCharpool)
                {
                    char chosen = arrychar[bytesarray[counter]];
                    captcha_text = captcha_text + chosen;
                }
                if (captcha_text.Length >= captcha_size)
                    break;
            }
            bool isexist = CheckifCAPTCHAexit(captcha_text);
            if (isexist == true)
            {
                GenerateCAPTCHAtext();
            }
            return captcha_text;
        }

        // Check if the newly generated CAPTCHA already exist
        public static bool CheckifCAPTCHAexit(string CAPTCHAtext)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myconnectionstring1"].ConnectionString);
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                SqlDataAdapter sqladpt = new SqlDataAdapter("select * from user_attemptdetails where captchatext ='" + CAPTCHAtext + "'", con);
                DataTable dt = new DataTable();
                sqladpt.Fill(dt);
                con.Close();
                if(dt.Rows.Count>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
                return true;
        }

        // Starting the timer to track the time consumed by the users
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            int cnt = Convert.ToInt32(Session["Counter"]);
            lbltimer.Text = ((Int32)DateTime.Parse(Session["Timer"].ToString()).Subtract(DateTime.Now).TotalMinutes).ToString().Replace('-', ' ') + " Minutes " +
                           (((Int32)DateTime.Parse(Session["Timer"].ToString()).Subtract(DateTime.Now).TotalSeconds) % 60).ToString().Replace('-', ' ') + " Seconds";
            Session["timetracker"] = lbltimer.Text;
        }


    }
 }
