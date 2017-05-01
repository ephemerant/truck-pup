using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace Food_Truck_Web.Controllers
{
    public class MenuController : ApiController
    {
        public Dictionary<string, object> Post([FromBody] Dictionary<string, object> data)
        {
            var response = new Dictionary<string, object>();

            try
            {
                // Handle the request type
                if (data.ContainsKey("type"))
                {
                    dynamic dataObj;
                    string uid;

                    switch (data["type"].ToString())
                    {
                        // GET
                        case "get":
                            dataObj = data["data"];
                            uid = dataObj.uid;

                            response.Add("menu", Database.ExecuteScalar(@"SELECT menu FROM Users WHERE uid=@uid", new Dictionary<string, object>() { { "@uid", uid } }));

                            break;

                        // UPDATE
                        case "update":
                            // Verify the user's token
                            if (data.ContainsKey("token"))
                            {
                                dynamic payload = Shared.VerifyAndGetPayload(response, data);

                                if (payload != null)
                                {
                                    dataObj = data["data"];
                                    uid = payload.user_id;                                    

                                    string menu = dataObj.menu;

                                    response.Add("success", Database.ExecuteNonQuery(@"UPDATE Users SET menu=@menu WHERE uid=@uid",
                                    new Dictionary<string, object>() {
                                        { "@uid", uid },
                                        { "@menu", menu }
                                      }));
                                }
                            }

                            // See if we were able to authenticate
                            if (!response.ContainsKey("success"))
                                response.Add("error", "Unable to verify user token");

                            break;
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

