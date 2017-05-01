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
    public class AdminController : ApiController
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

                    // Verify the user's token
                    if (data.ContainsKey("token"))
                    {
                        dynamic payload = Shared.VerifyAndGetPayload(response, data);

                        if (payload != null)
                        {
                            uid = payload.user_id;

                            // Make sure they're an admin
                            if (Shared.IsAdmin(uid))
                            {
                                response.Add("success", true);

                                switch (data["type"].ToString())
                                {
                                    // DELETE THE USER OR PENDING USER
                                    case "delete":
                                        dataObj = data["data"];

                                        if (dataObj.uid != null)
                                        {
                                            uid = dataObj.uid;

                                            response.Add("deleted", Database.ExecuteNonQuery(@"DELETE FROM Users WHERE uid=@uid AND admin=0",
                                                new Dictionary<string, object>() {
                                                { "@uid", uid }
                                                }));
                                        }

                                        if (dataObj.id != null)
                                            response.Add("deleted", Database.ExecuteNonQuery(@"DELETE FROM PendingUsers WHERE id=@id",
                                                new Dictionary<string, object>() {
                                                { "@id", (int) dataObj.id }
                                                }));

                                        break;

                                    // TOGGLE ACCOUNT STATUS
                                    case "toggle":
                                        dataObj = data["data"];
                                        uid = dataObj.uid;

                                        response.Add("toggled", Database.ExecuteNonQuery(@"UPDATE Users SET enabled = ~enabled WHERE uid=@uid",
                                            new Dictionary<string, object>() {
                                                { "@uid", uid }
                                            }));

                                        break;

                                    // GET USERS
                                    case "get":
                                        response.Add("pendingUsers", Database.GetDictionaries(@"SELECT * FROM PendingUsers"));
                                        response.Add("users", Database.GetDictionaries(@"SELECT * FROM Users"));

                                        break;

                                    // CREATE USER
                                    case "create":
                                        dataObj = data["data"];
                                        string name = dataObj.name;

                                        response.Add("id", Database.ExecuteScalar(@"INSERT INTO PendingUsers (name) VALUES (@name); SELECT SCOPE_IDENTITY()",
                                            new Dictionary<string, object>() {
                                                { "@name", name }
                                            }));

                                        break;
                                }
                            }
                        }

                        // See if we were able to authenticate
                        if (!response.ContainsKey("success"))
                            response.Add("error", "Unable to verify user token");
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

