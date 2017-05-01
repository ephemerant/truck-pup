using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;

namespace Food_Truck_Web.Controllers
{
    public class LoginController : ApiController
    {
        public Dictionary<string, object> Post([FromBody] Dictionary<string, object> data)
        {
            var response = new Dictionary<string, object>();

            try
            {
                // Verify the user's token
                if (data.ContainsKey("token"))
                {
                    dynamic payload = Shared.VerifyAndGetPayload(response, data);

                    if (payload != null)
                    {
                        response.Add("success", payload);

                        string uid = payload.user_id;
                        string email = payload.email;

                        // Create a profile if it doesn't exist
                        Profile.TryCreate(uid, email);

                        var privileges = Profile.LoadPrivileges(uid);

                        HttpContext.Current.Session.Clear();

                        HttpContext.Current.Session["uid"] = uid;

                        if (privileges.IsVendor)
                            HttpContext.Current.Session["vendor"] = privileges.IsVendor;

                        if (privileges.IsAdmin)
                            HttpContext.Current.Session["admin"] = privileges.IsAdmin;
                    }
                }
            }
            catch (Exception e)
            {
                response.Add("error", e.Message);
            }

            return response;
        }
    }
}

