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
    public class TruckController : ApiController
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
                            // Retrieve trucks
                            var trucks = new Dictionary<string, Dictionary<string, object>>();

                            // Have them be a dictionary keyed by UID
                            foreach (var truck in Database.GetDictionaries(@"SELECT uid, name, admin, vendor, about, logo, facebook, twitter, menu FROM Users"))
                                trucks.Add((string)truck["uid"], truck);

                            response.Add("trucks", trucks);

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

