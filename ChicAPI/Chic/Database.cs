using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic
{
    public class Database
    {
        static RestClient Client = new RestClient(Environment.GetEnvironmentVariable("REPLIT_DB_URL"));

        public static void SetValue(string key, string value)
            => Client.Execute(new RestRequest(Method.POST).AddParameter(key, value));

        public static string GetValue(string key)
            => Client.Execute(new RestRequest("/{key}", Method.GET).AddUrlSegment("key", key)).Content;

        public static bool TryGetValue(string key, out string value)
        {
            try
            {
                value = Client.Execute(new RestRequest("/{key}").AddUrlSegment("key", key)).Content;
                return true;
            } catch
            {
                value = "";
                return false;
            }
        }

        public static void RemoveValue(string key)
            => Client.Execute(new RestRequest("/{key}", Method.DELETE).AddUrlSegment("key", key));
    }
}
