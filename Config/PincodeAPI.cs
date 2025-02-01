using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RealEstateManagemaentSystem2024.Helper
{
    public static class PincodeAPI
    {
        public static async Task<(string state, string district, string village)> GetLocationDetailsFromPincodeAsync(string pincode)
        {
            string url = $"https://api.postalpincode.in/pincode/{pincode}";

            using (HttpClient client = new HttpClient())
            {
                // Make a GET request to the Pincode API
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Deserialize as JArray directly
                JArray result = JsonConvert.DeserializeObject<JArray>(content);

                // Check the "Status" in the first element of the array
                string status = result[0]["Status"].ToString();
                if (status == "Success")
                {
                    // Extract the first post office from the PostOffice array
                    JArray postOffices = (JArray)result[0]["PostOffice"];
                    var postOffice = postOffices[0];

                    string state = postOffice["State"].ToString();
                    string district = postOffice["District"].ToString();
                    string village = postOffice["Name"].ToString(); // Some API responses may use 'Name' for village

                    return (state, district, village); // Return the fetched details
                }
                else
                {
                    Console.WriteLine("Invalid Pincode.");
                    return (null, null, null);  // Return nulls if no data found
                }
            }
        }
    }
}
