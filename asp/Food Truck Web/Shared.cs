using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Food_Truck_Web
{
    public static class Shared
    {
        public static dynamic VerifyAndGetPayload(Dictionary<string, object> response, Dictionary<string, object> data)
        {
            try
            {
                if (data.ContainsKey("token"))
                {
                    var kid = Jose.JWT.Headers(data["token"].ToString())["kid"].ToString();
                    var payload = Jose.JWT.Payload(data["token"].ToString());

                    WebClient client = new WebClient();

                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(client.DownloadString("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com"));

                    // Decode and verify the token
                    if (json.ContainsKey(kid))
                    {
                        var keyData = Convert.FromBase64String(json[kid].Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", ""));
                        var cert = new X509Certificate2(keyData);
                        var key = cert.PublicKey.Key;

                        if (key != null)
                        {
                            var decode = Jose.JWT.Decode(data["token"].ToString(), key);

                            var verified = payload == decode;

                            response.Add("verified", verified);

                            // Verified - we can now accept their request
                            if (verified)
                                return Newtonsoft.Json.JsonConvert.DeserializeObject(payload);
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        public static bool IsValidRegistrationID(string id)
        {
            return (int)Database.ExecuteScalar(@"SELECT COUNT(*) FROM PendingUsers WHERE id=@id",
                new Dictionary<string, object>() {
                    { "@id", id }
                }) > 0;
        }

        public static bool IsAdmin(string uid)
        {
            return (int)Database.ExecuteScalar(@"SELECT COUNT(*) FROM Users WHERE uid=@uid AND admin=1",
                new Dictionary<string, object>() {
                    { "@uid", uid }
                }) > 0;
        }

        public static string NullToEmpty(object input)
        {
            if (input == null)
                return "";
            return input.ToString();
        }

        public static bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
                return true;

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}