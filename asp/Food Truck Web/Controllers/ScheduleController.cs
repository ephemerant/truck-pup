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
    public class ScheduleController : ApiController
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

                            if (uid == "all")
                                response.Add("events", Database.GetDictionaries(@"SELECT * FROM Calendar"));
                            else
                                response.Add("events", Database.GetDictionaries(@"SELECT * FROM Calendar WHERE uid=@uid", new Dictionary<string, object>() { { "@uid", uid } }));

                            break;

                        // ADD
                        case "add":
                            // Verify the user's token
                            if (data.ContainsKey("token"))
                            {
                                dynamic payload = Shared.VerifyAndGetPayload(response, data);

                                if (payload != null)
                                {
                                    dataObj = data["data"];
                                    uid = payload.user_id;

                                    string title = dataObj.title;
                                    DateTime start = dataObj.start;
                                    DateTime end = dataObj.end;

                                    response.Add("success", Database.ExecuteScalar(@"
INSERT INTO Calendar (uid, title, start, [end]) VALUES (@uid, @title, @start, @end)
SELECT SCOPE_IDENTITY()
",
                                    new Dictionary<string, object>() {
                                        { "@uid", uid },
                                        { "@title", title },
                                        { "@start", start },
                                        { "@end", end }
                                      }));
                                }
                            }

                            // See if we were able to authenticate
                            if (!response.ContainsKey("success"))
                                response.Add("error", "Unable to verify user token");

                            break;

                        // REMOVE                        
                        case "remove":
                            // Verify the user's token
                            if (data.ContainsKey("token"))
                            {
                                dynamic payload = Shared.VerifyAndGetPayload(response, data);

                                if (payload != null)
                                {
                                    dataObj = data["data"];
                                    uid = payload.user_id;

                                    string id = dataObj.id;

                                    response.Add("success", Database.ExecuteNonQuery(@"DELETE FROM Calendar WHERE id=@id AND uid=@uid",
                                    new Dictionary<string, object>() {
                                        { "@uid", uid },
                                        { "@id", id }
                                      }));
                                }
                            }

                            // See if we were able to authenticate
                            if (!response.ContainsKey("success"))
                                response.Add("error", "Unable to verify user token");

                            break;

                        // EDIT                        
                        case "edit":
                            // Verify the user's token
                            if (data.ContainsKey("token"))
                            {
                                dynamic payload = Shared.VerifyAndGetPayload(response, data);

                                if (payload != null)
                                {
                                    dataObj = data["data"];
                                    uid = payload.user_id;

                                    string id = dataObj.id;

                                    string title = dataObj.title;
                                    DateTime start = dataObj.start;
                                    DateTime end = dataObj.end;

                                    response.Add("success", Database.ExecuteNonQuery(@"UPDATE Calendar SET uid=@uid, title=@title, start=@start, [end]=@end WHERE uid=@uid AND id=@id",
                                    new Dictionary<string, object>() {
                                        { "@id", id },
                                        { "@uid", uid },
                                        { "@title", title },
                                        { "@start", start },
                                        { "@end", end }
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

