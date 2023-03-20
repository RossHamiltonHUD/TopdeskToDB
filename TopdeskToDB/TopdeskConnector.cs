using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using JsonFlatten;

namespace TopdeskDataCache
{
    internal class TopdeskConnector
    {
        public List<string> GetDatecodes(int startYear = 18)
        {
            List<string> datecodes = new List<string>();

            int selectedMonth = 1;
            int selectedYear = startYear;

            int endYear = Int16.Parse(DateTime.Now.ToString("yy"));
            int endMonth = Int16.Parse(DateTime.Now.ToString("MM"));

            int monthLimit = 12;

            while (selectedYear <= endYear)
            {
                if (selectedYear == endYear)
                {
                    monthLimit = endMonth;
                }

                while (selectedMonth <= monthLimit)
                {
                    datecodes.Add("T" + selectedYear.ToString("D2") + selectedMonth.ToString("D2"));

                    selectedMonth++;
                }

                selectedMonth = 1;
                selectedYear++;
            }

            return datecodes;
        }

        async public Task<List<InputTicket>> GetTicketsByDatecode(string datecode, string appPassword = "",
                                                string appUsername = "")
        {
            if (appUsername == "")
            { appUsername = ConfigurationManager.AppSettings.Get("config_topdesk_email_address"); }
            if (appPassword == "")
            { appPassword = ConfigurationManager.AppSettings.Get("config_topdesk_application_password"); }

            bool finishedSearching = false;
            int p = 0;
            int resultsPerPage;

            try
            {
                resultsPerPage = Int16.Parse(ConfigurationManager.AppSettings.Get("config_topdesk_api_page_size"));
            }
            catch
            {   resultsPerPage = 10000;  }

            string searchQuery = "number==" + datecode + "*";

            List<InputTicket> ticketList = new List<InputTicket>();

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;

                var client = new HttpClient();
                HttpRequestMessage req = new HttpRequestMessage();

                req.RequestUri = new Uri("https://hud.topdesk.net/tas/api/incidents/?pageSize=" + resultsPerPage + "&start=" + startValue + "&query=" + searchQuery);
                //Console.Write("\rFetching "+req.RequestUri.ToString() +" with " + appUsername + " : " + appPassword);

                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(appUsername + ":" + appPassword)));

                HttpResponseMessage response = client.SendAsync(req).Result;

                string jsonResult = await response.Content.ReadAsStringAsync();

                List<InputTicket> ticketlistpage = new List<InputTicket>();

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                var ticketListPage = JsonConvert.DeserializeObject<List<InputTicket>>(jsonResult, settings);

                //var jObject = JObject.Parse(jsonResult);
                //var ticketListPage = jObject.Flatten();

                if (ticketListPage != null && ticketListPage.Count > 0)
                {
                    ticketList.AddRange(ticketListPage);

                    if (ticketListPage.Count < resultsPerPage)
                    {
                        finishedSearching = true;
                    }
                }
                else { finishedSearching = true; }

                p++;
            }

            return ticketList;
        }
    }
}
