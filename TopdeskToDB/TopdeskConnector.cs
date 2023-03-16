using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TopdeskToDB
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
                    monthLimit = endMonth - 1;
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

        async public Task<List<Ticket>> GetTickets(string datecode, string appPassword = "dds4i-hiuia-7idx5-kf6m2-wtl2j",
                                                string appUsername = "r.hamilton@hud.ac.uk")
        {
            bool finishedSearching = false;
            int p = 0;
            int resultsPerPage = 10000;

            string searchQuery = "closed==true;number==" + datecode + "*";

            List<Ticket> ticketList = new List<Ticket>();

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;

                var client = new HttpClient();
                HttpRequestMessage req = new HttpRequestMessage();

                req.RequestUri = new Uri("https://hud.topdesk.net/tas/api/incidents/?pageSize=" + resultsPerPage + "&start=" + startValue + "&query=closed==true;number==" + datecode + "*");

                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(appUsername + ":" + appPassword)));

                HttpResponseMessage response = client.SendAsync(req).Result;

                string jsonResult = await response.Content.ReadAsStringAsync();

                List<Ticket> ticketListPage = new List<Ticket>();

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                ticketListPage = JsonConvert.DeserializeObject<List<Ticket>>(jsonResult, settings);

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
