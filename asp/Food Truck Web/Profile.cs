using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Food_Truck_Web
{
    public static class Profile
    {
        // Create a profile if it doesn't exist
        public static void TryCreate(string uid, string email, string name = "Untitled")
        {
            Database.ExecuteNonQuery(
                @"IF NOT EXISTS (SELECT * FROM Users WHERE uid=@uid)
                  BEGIN INSERT INTO Users (uid, email, name, vendor) VALUES (@uid, @email, @name, 1) END",
                new Dictionary<string, object>()
                {
                    {"@uid", uid },
                    {"@email", email },
                    {"@name", name }
                });
        }

        // Load a user's basic profile information
        public static ProfileObject LoadBasic(string uid)
        {
            var dict = Database.GetDictionary(
                "SELECT name, about, logo, facebook, twitter FROM Users WHERE uid=@uid",
                new Dictionary<string, object>()
                {
                    {"@uid", uid }
                });

            var name = "";
            var about = "";
            var logo = "";
            var facebook = "";
            var twitter = "";

            if (dict["name"] != DBNull.Value)
                name = (string)dict["name"];

            if (dict["about"] != DBNull.Value)
                about = (string)dict["about"];

            if (dict["logo"] != DBNull.Value)
                logo = (string)dict["logo"];

            if (dict["facebook"] != DBNull.Value)
                facebook = (string)dict["facebook"];

            if (dict["twitter"] != DBNull.Value)
                twitter = (string)dict["twitter"];

            return new ProfileObject { Name = name, About = about, Logo = logo, Facebook = facebook, Twitter = twitter };
        }

        // Update a profile's basic information
        public static void UpdateProfile(string uid, string name, string about, string facebook, string twitter, HttpPostedFileBase logo)
        {
            // Double-check Facebook & Twitter URLs

            if (facebook != "" && !(new Regex(@"^(http:\/\/|https:\/\/)?(www\.)?facebook\.com\/.*$").IsMatch(facebook)))
                facebook = "";

            if (twitter != "" && !(new Regex(@"^(http:\/\/|https:\/\/)?(www\.)?twitter\.com\/.*$").IsMatch(twitter)))
                twitter = "";

            // Commit to database

            Database.ExecuteNonQuery(
                "UPDATE Users SET name=@name, about=@about, facebook=@facebook, twitter=@twitter WHERE uid=@uid",
                new Dictionary<string, object>()
                {
                    {"@name", name },
                    {"@about", about },
                    {"@facebook", facebook },
                    {"@twitter", twitter },
                    {"@uid", uid }
                });

            // Server-side validation of the logo

            if (logo.ContentLength > 0 && logo.FileName != "")
            {
                // No larger than 2 MB
                if (logo.ContentLength > 1024 * 1024 * 2)
                    HttpContext.Current.Session["error_message"] = "Logos cannot exceed 2 MB";
                else
                {
                    if (!Shared.IsImage(logo))
                        HttpContext.Current.Session["error_message"] = "Logos must be images";
                    else
                    {
                        // Upload image
                        var path = Path.Combine(@"C:\inetpub\cdn\logo", uid + Path.GetExtension(logo.FileName));

                        logo.SaveAs(path);

                        var extPath = Path.Combine(@"https://cdn.ephemerant.com\logo", uid + Path.GetExtension(logo.FileName)).Replace("\\", "/");

                        Database.ExecuteNonQuery("UPDATE Users SET logo=@logo WHERE uid=@uid",
                            new Dictionary<string, object>()
                            {
                                {"@uid", uid },
                                {"@logo", extPath }
                            });

                        HttpContext.Current.Session["message"] = "Profile successfully updated";
                    }
                }
            }
            else
                HttpContext.Current.Session["message"] = "Profile successfully updated";
        }

        // Load a user's account privileges
        public static ProfileObject LoadPrivileges(string uid)
        {
            var dict = Database.GetDictionary(
                "SELECT admin, vendor FROM Users WHERE uid=@uid",
                new Dictionary<string, object>()
                {
                    {"@uid", uid }
                });

            return new ProfileObject { IsVendor = (bool)dict["vendor"], IsAdmin = (bool)dict["admin"] };
        }
    }

    public class ProfileObject
    {
        public bool IsVendor { get; set; }
        public bool IsAdmin { get; set; }

        public string Name { get; set; }
        public string About { get; set; }
        public string Logo { get; set; }

        public string Facebook { get; set; }
        public string Twitter { get; set; }
    }
}