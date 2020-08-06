using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace CS255_TextCAPTCHA
{
    public partial class Captcha_Image : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["CaptchaChar"] ==null)
            {
                Response.Redirect("~/Captcha_HomePage.aspx");
            }

            //* Choosing a random Captcha Image background from the database

            int rndnbr = GeneraterandomNumber(1, 22); // There are Images with Suffix 1 to 20
           
            /*SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myconnectionstring1"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("select img from CAPTCHA_bgimages where imagename='Image" + rndnbr.ToString() +"'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            System.Data.DataSet ds = new DataSet();
            da.Fill(ds);
            MemoryStream memst = new MemoryStream((byte[])ds.Tables[0].Rows[0]["img"]);
            Bitmap bmp = new Bitmap(mem);
          */

            string CaptchabckgndImg = "Image" + rndnbr.ToString() + ".jpg";
            Bitmap bmp = new Bitmap(Server.MapPath("~/Images/"+ CaptchabckgndImg));
            MemoryStream mem = new MemoryStream();
            
            int imgwidth = bmp.Width;
            int imgheight = bmp.Height;
            string captchatext = Session["CaptchaChar"].ToString();


            Bitmap btmap = new Bitmap(bmp, new Size(imgwidth, imgheight));
            Graphics g = Graphics.FromImage(btmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
    

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            format.LineAlignment = StringAlignment.Far;
            GraphicsPath path = new GraphicsPath();

            //* Adding the text in the Graphics with Various Font Family, Font Size,

            char extract_char;
            string[] fontfam = new string[12] { "Shake that booty", "KG Primary Whimsy", "leftovers", "AR CARTER","Chiller", "BethHand", "AhnbergHand", "Quentin", "Tigger", "Alexa", "tintin", "Tekton Cn bold" };
            //string[] fontfam = new string[10] {"KG Primary Whimsy", "leftovers", "AR CARTER", "Chiller", "BethHand", "AhnbergHand", "Quentin", "Alexa", "tintin", "Tekton Cn bold" };
            string fontfamily = fontfam[GeneraterandomNumber(0, 10)];
            for (int indxcntr = 0; indxcntr < captchatext.Length; indxcntr++)
            {
                extract_char = captchatext[indxcntr];   // string, fontfamily, fontstyle, fontsize,new point, GenericDefault
                int rndfont_size = 0;
                if (extract_char.ToString().Contains("C") || extract_char.ToString().Contains("X") || extract_char.ToString().Contains("M") || extract_char.ToString().Contains("K") || extract_char.ToString().Contains("P") || extract_char.ToString().Contains("S") || extract_char.ToString().Contains("U") || extract_char.ToString().Contains("V") || extract_char.ToString().Contains("W") || extract_char.ToString().Contains("Y") || extract_char.ToString().Contains("Z"))
                {
                    if (char.IsUpper(extract_char))
                         {
                            rndfont_size = GeneraterandomNumber(70, 80);
                         }
                     else
                        {
                            rndfont_size = GeneraterandomNumber(50, 60);
                        }
                }
                else
                {
                    rndfont_size = GeneraterandomNumber(60, 70);
                }
                //int[] rndfont_Xlocation = new int[6] { 1, 55, 110, 160, 220, 290};
                int[] rndfont_Xlocation = new int[6] { 1, 30, 70, 110, 220, 290 };
                int rndfont_ylocation = GeneraterandomNumber(10, 30);

                 path.AddString(extract_char.ToString(), new FontFamily(fontfamily), (int)FontStyle.Bold, rndfont_size, new Point(rndfont_Xlocation[indxcntr], rndfont_ylocation), StringFormat.GenericDefault);
               
                //Difficult CAPTCHA  path.AddString(extract_char.ToString(), new FontFamily(fontfamily), (int)FontStyle.Bold, rndfont_size, new Point(rndfont_Xlocation[indxcntr], rndfont_ylocation), StringFormat.GenericDefault);

            }

            HatchBrush htchbrsh = new HatchBrush(HatchStyle.LargeConfetti, Color.FromName("Black"), Color.FromName("Black"));
            g.FillPath(htchbrsh, path);
           
            // Rendering the Graphics and saving it

            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/jpeg";
            btmap.Save(mem, ImageFormat.Jpeg);
            bmp.Dispose();
            htchbrsh.Dispose();
            g.Dispose();
            mem.WriteTo(HttpContext.Current.Response.OutputStream);
            btmap.Dispose();
          
        }

        // Generic Method to generate a Random Number Method
        public int GeneraterandomNumber(int minrange, int maxrange)
        {
            Random _random = new Random();     
            return _random.Next(minrange, maxrange);
    }
    }
}