using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopdeskDataCache
{
    internal class oDataInterface
    {
        string username = ConfigurationManager.AppSettings.Get("config_topdesk_email_address");
        string password = ConfigurationManager.AppSettings.Get("config_topdesk_application_password");
        string encoded;

        static readonly HttpClient client = new HttpClient();

        public oDataInterface()
        {
            encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encoded);
        }

        public static async Task<string> PullData(string url)
        {
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);

            return responseBody;
        }

        public string GetIncidentsCausedByChange()
        {
            string urlString = "https://hud.topdesk.net/services/reporting/v2/odata/IncidentsCausedByChangesLinks";
            string data = oDataInterface.PullData(urlString).Result;

            return data;
        }

        public string GetProblems()
        {
            string urlString = "https://hud.topdesk.net/services/reporting/v2/odata/ProblemDetails";
            string data = oDataInterface.PullData(urlString).Result;

            return data;
        }

        public string GetProblemIncidentLinks()
        {
            string urlString = "https://hud.topdesk.net/services/reporting/v2/odata/ProblemIncidentLinks";
            string data = oDataInterface.PullData(urlString).Result;

            return data;
        }
    }
}
