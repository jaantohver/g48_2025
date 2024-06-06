using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace ttki
{
    public static class Networking
    {
        const string Base = "https://app.terake.com/ttki/";

        readonly static HttpClient client;

        static Networking() {
            if (client == null) client = new HttpClient();
        }

        public static async Task<List<WorkArea>> GetWorkAreas(string userId)
        {
            string uri = Base + "workareas/" + userId;

            HttpResponseMessage response = await client.GetAsync(uri);

            string content = await response.Content.ReadAsStringAsync();

            JToken json = JToken.Parse(content);

            if (json is JArray)
            {
                return Codec.DecodeWorkAreas(json as JArray);
            } else
            {
                return new List<WorkArea>();
            }
        }

        public static async Task<List<Work>> GetWorks(string userId)
        {
            string uri = Base + "works/" + userId;

            HttpResponseMessage response = await client.GetAsync(uri);

            string content = await response.Content.ReadAsStringAsync();

            JToken json = JToken.Parse(content);

            if (json is JArray)
            {
                return Codec.DecodeWorks(json as JArray);
            }
            else
            {
                return new List<Work>();
            }
        }

        public static async Task<bool> StartWork(string cardNumber, string areaUuid)
        {
            string uri = Base + "start/" + cardNumber + "/" + areaUuid;

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> StopWork(string cardNumber)
        {
            string uri = Base + "stop/" + cardNumber;

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
