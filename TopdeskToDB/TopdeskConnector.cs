using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using JsonFlatten;
using System.Threading;

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
                if (selectedYear == endYear) { monthLimit = endMonth; }

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
            List<Task> tasks = new List<Task>();

            try { resultsPerPage = Int16.Parse(ConfigurationManager.AppSettings.Get("config_topdesk_api_page_size")); }
            catch { resultsPerPage = 1000; }

            string searchQuery = "number==" + datecode + "*";
            List<Ticket> ticketList = new List<Ticket>();

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;
                string reqUrl = "https://hud.topdesk.net/tas/api/incidents/?pageSize=" + resultsPerPage + "&start=" + startValue + "&query=" + searchQuery;
                var ticketListPage = new List<Ticket>();
                int retries = 5;
                int tries = 1;
                bool success = false;

                while(tries <= retries && !success)
                {
                    try
                    {
                        string jsonResult = JsonRequest(reqUrl).Result;
                        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                        ticketListPage = JsonConvert.DeserializeObject<List<Ticket>>(jsonResult, settings);

                        if (ticketListPage != null && ticketListPage.Count > 0)
                        {
                            if (ConfigurationManager.AppSettings.Get("config_live_mode") == "off") { ticketListPage = ProcessTicketPage(ticketListPage); }
                            //ticketListPage = ProcessTicketPage(ticketListPage);
                            ticketList.AddRange(ticketListPage);

                            if (ticketListPage.Count < resultsPerPage)
                            {
                                finishedSearching = true;
                            }
                        }
                        else { finishedSearching = true; }

                        success = true;

                        p++;
                    }
                    catch
                    {
                        tries++;
                        success = false;
                    }
                }
                
            }

            //WaitAll(tasks);

            return ticketList;
        }

        public static void WaitAll(List<Task> tasks)
        {
            if (tasks != null)
            {
                foreach (Task task in tasks)
                { task.Wait(); }
            }
        }

        public List<Ticket> ProcessTicketPage(List<Ticket>? ticketPage)
        {
            int actionRequests = 0;
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

            foreach (Ticket ticket in ticketPage)
            {
                if (ticket.ClosureCode == null)
                {
                    continue;
                }

                if (ticket.ClosureCode.Name == "No Response Received")
                {
                    try {
                        //string actionsUrl = "https://hud.topdesk.net/tas/api/incidents/number/" + ticket.Number + "/actions";
                        //string jsonResult = JsonRequest(actionsUrl).Result;
                        //var actionResult = JsonConvert.DeserializeObject<List<Action>>(jsonResult, settings);
                        //actionRequests++;

                        //if (actionResult == null)
                        //{
                        //    continue;
                        //}

                        //ticket.CompletedDate = actionResult.First().EntryDate;

                        var tempDiff = DateTime.Parse(ticket.ClosedDate) - DateTime.Parse(ticket.CallDate);

                        if (tempDiff.Days > 6.95)
                        {
                            ticket.CompletedDate = (DateTime.Parse(ticket.ClosedDate) - TimeSpan.FromDays(7)).ToString("G");
                        }

                        else
                        {
                            ticket.CompletedDate = DateTime.Parse(ticket.CompletedDate).ToString("G");
                        }
                    }
                    catch { }
                }

                //Thread.Sleep(125);
            }

            return ticketPage;
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

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                var items = JsonConvert.DeserializeObject<Item>(jsonResult, settings);

                List<KnowledgeItem> knowledgeListPage = new List<KnowledgeItem>();

                if (items.KnowledgeItems != null) { knowledgeListPage = items.KnowledgeItems.ToList<KnowledgeItem>(); }

                if (knowledgeListPage != null && knowledgeListPage.Count > 0) { knowledgeList.AddRange(knowledgeListPage); }
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

            string auth = ConfigurationManager.AppSettings.Get("config_topdesk_email_address") +
                ':' + ConfigurationManager.AppSettings.Get("config_topdesk_application_password");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(auth)));

            HttpResponseMessage response = client.SendAsync(req).Result;
            string jsonResult = await response.Content.ReadAsStringAsync();

            return jsonResult;
        }
    }
}
