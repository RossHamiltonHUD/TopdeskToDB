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

        public List<Ticket> GetTicketsByDatecode(string datecode)
        {
            bool finishedSearching = false;
            int p = 0;
            int resultsPerPage;

            try
            {
                resultsPerPage = Int16.Parse(ConfigurationManager.AppSettings.Get("config_topdesk_api_page_size"));
            }
            catch
            { resultsPerPage = 10000; }

            string searchQuery = "number==" + datecode + "*";

            List<Ticket> ticketList = new List<Ticket>();

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;

                string reqUrl = "https://hud.topdesk.net/tas/api/incidents/?pageSize=" + resultsPerPage + "&start=" + startValue + "&query=" + searchQuery;
                string jsonResult = JsonRequest(reqUrl).Result;

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                var ticketListPage = JsonConvert.DeserializeObject<List<Ticket>>(jsonResult, settings);

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
        public List<KnowledgeItem> GetKnowledge()
        {
            bool finishedSearching = false;
            int p = 0;
            int resultsPerPage = 100;

            List<KnowledgeItem> knowledgeList = new List<KnowledgeItem>();

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;

                string fields = "parent,visibility,urls,manager,title,creator,creationDate,modifier,status,modificationDate";
                string reqUrl = "https://hud.topdesk.net/tas/api/knowledgeItems?pageSize=" + resultsPerPage + "&start=" + startValue + "&fields=" + fields;

                string jsonResult = JsonRequest(reqUrl).Result;

                //jsonResult = jsonResult.Substring(jsonResult.IndexOf("["));
                //jsonResult = jsonResult.Substring(0, jsonResult.IndexOf("]") + 1);

                //Console.WriteLine(jsonResult);
                //Console.ReadLine(); 

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                var items = JsonConvert.DeserializeObject<Item>(jsonResult, settings);
                List<KnowledgeItem> knowledgeListPage = new List<KnowledgeItem>();

                if (items.KnowledgeItems != null)
                {
                    knowledgeListPage = items.KnowledgeItems.ToList<KnowledgeItem>();
                }
                
                if (knowledgeListPage != null && knowledgeListPage.Count > 0)
                {
                    knowledgeList.AddRange(knowledgeListPage);

                    //if (knowledgeListPage.Count < resultsPerPage)
                    //{
                    //    finishedSearching = true;
                    //}
                }
                else { finishedSearching = true; }

                p++;
            }

            return knowledgeList;
        }

        async public Task<string> JsonRequest(string queryUrl)
        {
            var client = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage();

            req.RequestUri = new Uri(queryUrl);

            string auth = ConfigurationManager.AppSettings.Get("config_topdesk_email_address") + ':' + ConfigurationManager.AppSettings.Get("config_topdesk_application_password");

            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(auth)));

            HttpResponseMessage response = client.SendAsync(req).Result;

            string jsonResult = await response.Content.ReadAsStringAsync();

            return jsonResult;
        }
    }
}
