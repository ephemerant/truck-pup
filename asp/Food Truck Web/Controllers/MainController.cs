using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Food_Truck_Web.Controllers
{
    public class MainController : Controller
    {
        /*************
        TESTING
        *************/

        // GET: Test
        [HttpGet]
        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Test(string id)
        {
            return Redirect("test");
        }

        /*************
        MAIN CONTENT
        *************/

        // GET: Main
        public ActionResult Index()
        {
            if (Session["vendor"] != null)
                return Redirect("profile");
            else if (Session["admin"] != null)
                return Redirect("admin");
            else
                return Redirect("login");
        }

        // Login       
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["admin"] != null || Session["vendor"] != null)
                return Redirect("~");

            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult LoginPost(string email, string password)
        {
            return Redirect("~");
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();

            return Redirect("~");
        }

        public ActionResult Register(string id)
        {
            if (id == null || id.Trim() == "" || !Shared.IsValidRegistrationID(id))
                return Redirect("~");

            ViewBag.id = id;

            return View();
        }

        /*************
        ADMIN CONTENT
        *************/

        // GET: Admin
        public ActionResult Admin()
        {
            if (Session["admin"] == null)
                return Redirect("~");

            return View();
        }

        /*************
        VENDOR CONTENT
        *************/

        // GET: Profile
        public ActionResult Profile()
        {
            if (Session["vendor"] == null)
                return Redirect("~");

            var uid = Session["uid"];

            if (uid != null)
            {
                var profile = Food_Truck_Web.Profile.LoadBasic(uid.ToString());

                ViewBag.name = profile.Name;
                ViewBag.about = profile.About;
                ViewBag.logo = profile.Logo;
                ViewBag.facebook = profile.Facebook;
                ViewBag.twitter = profile.Twitter;
            }

            return View();
        }

        // Update profile
        [HttpPost]
        public ActionResult Profile(string id)
        {
            var name = Request.Form["name"];
            var about = Request.Form["about"];

            var facebook = Request.Form["facebook"];
            var twitter = Request.Form["twitter"];

            var logo = Request.Files["logo"];

            var uid = Session["uid"];

            if (uid != null)
                Food_Truck_Web.Profile.UpdateProfile(uid.ToString(), name, about, facebook, twitter, logo);

            return Redirect("profile");
        }

        // GET: Menu
        public ActionResult Menu()
        {
            if (Session["vendor"] == null)
                return Redirect("~");

            return View();
        }

        // GET: Schedule
        public ActionResult Schedule()
        {
            if (Session["vendor"] == null)
                return Redirect("~");

            return View();
        }
    }
}