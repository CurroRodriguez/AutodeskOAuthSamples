using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class callback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
            //this page is callback page
            //get the oauth_verifier parameter from callback url
            if (Request.Url.ToString().IndexOf("oauth_verifier") > 0)
            {
                var qs = HttpUtility.ParseQueryString(Request.Url.Query);
                string oauth_verifier = qs["oauth_verifier"];

                //display the verifier just for reference, you dont have to show it up.
                lblVerifier.Text = oauth_verifier;

                //save it into session for usage in other pages
                Session["verifier"] = oauth_verifier;
            }
        }
    }
}