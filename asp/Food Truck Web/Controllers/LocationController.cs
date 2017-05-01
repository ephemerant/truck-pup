using PusherServer;
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
    public class LocationController : ApiController
    {
        public Dictionary<string, object> Post([FromBody] Dictionary<string, object> data)
        {
            var response = new Dictionary<string, object>();

            try
            {
                // Handle the request type
                if (data.ContainsKey("type"))
                {
                    switch (data["type"].ToString())
                    {
                        // GET
                        case "get":
                            // Retrieve locations
                            var locations = new Dictionary<string, Dictionary<string, object>>();

                            // Have them be a dictionary keyed by UID
                            foreach (var location in Database.GetDictionaries(@"SELECT * FROM Locations WHERE DATEADD(hour, hours, time) > CURRENT_TIMESTAMP"))
                                locations.Add((string)location["uid"], location);

                            response.Add("locations", locations);

                            break;

                        // ADD
                        case "add":
                            // Verify the user's token
                            if (data.ContainsKey("token"))
                            {
                                dynamic payload = Shared.VerifyAndGetPayload(response, data);

                                if (payload != null)
                                {
                                    string uid = payload.user_id;
                                    dynamic dataObj = data["data"];

                                    string lat = dataObj.lat;
                                    string lon = dataObj.lon;
                                    int hours = dataObj.hours;

                                    // Replace location and return its database-given ID
                                    response.Add("success", Database.ExecuteScalar(@"
                                        DELETE FROM Locations WHERE uid=@uid
                                        INSERT INTO Locations (uid, lat, lon, time, hours) VALUES (@uid, @lat, @lon, CURRENT_TIMESTAMP, @hours)
                                        SELECT 1",
                                    new Dictionary<string, object>() {
                                        { "@uid", uid },
                                        { "@lat", lat },
                                        { "@lon", lon },
                                        { "@hours", hours }
                                      }));

                                    // Send out broadcast
                                    var options = new PusherOptions();
                                    options.Encrypted = true;

                                    var pusher = new Pusher("316078", "c367bd18dea7e8eaee28", "36dbabfa4496f090bece", options);

                                    var result = pusher.Trigger("main", "broadcast", new { uid = uid });
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
                                    string uid = payload.user_id;

                                    // Remove user's location
                                    response.Add("success", Database.ExecuteNonQuery("DELETE FROM Locations WHERE uid=@uid",
                                    new Dictionary<string, object>() {
                                        { "@uid", uid }
                                      }));

                                    // Send out broadcast
                                    var options = new PusherOptions();
                                    options.Encrypted = true;

                                    var pusher = new Pusher("316078", "c367bd18dea7e8eaee28", "36dbabfa4496f090bece", options);

                                    var result = pusher.Trigger("main", "broadcast", new { uid = uid });
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

