using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using JsonFlatten;
using System.Threading;
using TopdeskDataCache.schema;

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

            try { resultsPerPage = 10000; }
            catch { resultsPerPage = 1000; }

            string urlPrefix = ConfigurationManager.AppSettings.Get("config_topdesk_url");

            string searchQuery = "number==" + datecode + "*";
            List<Ticket> ticketList = new List<Ticket>();

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;
                string reqUrl = "https://" + urlPrefix + ".topdesk.net/tas/api/incidents/?pageSize=" + resultsPerPage + "&start=" + startValue + "&query=" + searchQuery;
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

            return ticketList;
        }

        public List<KnowledgeItem> GetKnowledge()
        {
            bool finishedSearching = false;
            int p = 0;
            int resultsPerPage = 100;
            List<KnowledgeItem> knowledgeList = new List<KnowledgeItem>();
            string urlPrefix = ConfigurationManager.AppSettings.Get("config_topdesk_url");

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;
                string fields = "parent,visibility,urls,manager,title,creator,creationDate,modifier,status,modificationDate";
                string reqUrl = "https://" + urlPrefix + ".topdesk.net/tas/api/knowledgeItems?pageSize=" + resultsPerPage + "&start=" + startValue + "&fields=" + fields;
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

        public List<Change> GetChanges()
        {
            bool finishedSearching = false;
            int p = 0;
            int resultsPerPage = 1000;
            List<Change> changeList = new List<Change>();
            string urlPrefix = ConfigurationManager.AppSettings.Get("config_topdesk_url");

            while (!finishedSearching)
            {
                int startValue = p * resultsPerPage;
                string fields = "id,number,changeType,simple,briefDescription,creationDate,status.name,submitDate,creator,modifier,type.name,requester.name," +
                    "requester.department.name,requestDate,urgent,impact,canceled,category,subcategory,phases,processingStatus";
                string reqUrl = "https://" + urlPrefix + ".topdesk.net/tas/api/operatorChanges?pageSize=" + resultsPerPage + "&pageStart=" + startValue + "&fields=" + fields;
                string jsonResult = JsonRequest(reqUrl).Result;

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                var items = JsonConvert.DeserializeObject<ChangeList>(jsonResult, settings);

                List<Change> changeListPage = new List<Change>();

                if (items.Results != null) { changeListPage = items.Results.ToList<Change>(); }

                if (changeListPage != null && changeListPage.Count > 0) { changeList.AddRange(changeListPage); }
                else { finishedSearching = true; }

                p++;
            }

            return changeList;
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
