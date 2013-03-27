using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using System.Web.Security;

namespace FBSSO.Controllers
{
    public class AccountController : Controller
    {
        private Uri RedirectUri
        {
            get 
            { 
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        // GET: /Account/

        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new {
                client_id = "100106430186046",
                client_secret = "7c9ee3c7e3a1362098ad88b7a9227fc8",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"  // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public  ActionResult Login()
        {
            return View();
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "100106430186046",
                client_secret = "7c9ee3c7e3a1362098ad88b7a9227fc8",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            // Store the access token in the session
            Session["AccessToken"] = accessToken;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me?fields=first_name,last_name,id,email");
            string user = string.Join(" ", me.first_name, me.last_name);

            // Set the auth cookie
            FormsAuthentication.SetAuthCookie(user, false);

            return RedirectToAction("Index", "Home");
        }
    }
}
