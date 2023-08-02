using System;
using System.Collections.Generic;
using System.Configuration;
using TopdeskDataCache.schema;

namespace TopdeskDataCache
{
    public static class DataHandler
    {
        private static string baseFilepath = ConfigurationManager.AppSettings.Get("config_filepath") + "topdeskData";

        //  Set up our classes for interacting with different systems
        private static TopdeskConnector tdConnector = new TopdeskConnector();
        private static FileHandler fileHandler = new FileHandler(baseFilepath);

        private static oDataInterface odata = new oDataInterface();

        public static string topdeskEmailAddress = ConfigurationManager.AppSettings.Get("config_topdesk_email_address");
        public static string topdeskAppPassword = ConfigurationManager.AppSettings.Get("config_topdesk_application_password");

        public static bool unattendedMode = true;

        public static void Main(string[] args)
        {
            
            if (topdeskEmailAddress == "" || topdeskAppPassword == "")
            {
                if (!unattendedMode) { Console.WriteLine("Please enter your Topdesk details in Topdesk.dll.config! Press enter to quit..."); }
                Console.ReadLine();
                System.Environment.Exit(0);
            }

            int startingYear = Int16.Parse(ConfigurationManager.AppSettings.Get("config_starting_year"));

            List<string> datecodes = tdConnector.GetDatecodes(startingYear);
            
            ImportToFile(datecodes);

            DateTime utcNow = DateTime.UtcNow;

            File.WriteAllText(baseFilepath + "\\last_run.txt", utcNow.Year.ToString() + "-" + utcNow.Month.ToString("00") + "-" + utcNow.Day.ToString("D2") +
                "T" + utcNow.Hour.ToString("D2") + ":" + utcNow.AddMinutes(-1).Minute.ToString("D2") + ":00Z");

            if (!unattendedMode) { Console.WriteLine("Finished collecting data!"); }

            Console.ReadLine();
        }

        public static void ImportToFile(List<string> datecodes)
        {
            List<string> datecodesToCheck = fileHandler.CheckNeededData(datecodes);

            if (datecodesToCheck.Count == 0)
            {
                if (!unattendedMode) { Console.WriteLine("Files exist for all requested datecodes already!"); }
                return;
            }

            int totalImported = 0;
            int i = 1;
            List<Task> tasks = new List<Task>();

            foreach (string datecode in datecodesToCheck)
            {
                bool lastMonth = (datecode == datecodesToCheck[datecodesToCheck.Count - 1]);
                PullTicketList(datecode, lastMonth);
            }

            string causedByChange = odata.GetIncidentsCausedByChange();
            string problems = odata.GetProblems();
            string problemDetails = odata.GetProblemDetails();
            string problemIncidentLinks = odata.GetProblemIncidentLinks(); 
            string categories = odata.GetCategories();
            string subcategories = odata.GetSubcategories();
            string operatorGroups = odata.GetOperatorGroups();
            string operators = odata.GetOperators();
            string snapshots = odata.GetIncidentSnapshots(baseFilepath, File.ReadAllText(baseFilepath + "\\last_run.txt"));
            string changeActivities = odata.GetChangeActivities(File.ReadAllText(baseFilepath + "\\last_run.txt"));
            string statuses = odata.GetStatuses();

            List<KnowledgeItem> knowledge = tdConnector.GetKnowledge();
            List<Change> change = tdConnector.GetChanges();

            fileHandler.SaveKnowledge(knowledge);
            fileHandler.SaveChanges(change);

            fileHandler.SaveCausedByChanges(causedByChange);
            fileHandler.SaveProblems(problems);
            fileHandler.SaveProblemDetails(problemDetails);
            fileHandler.SaveProblemIncidentLinks(problemIncidentLinks);
            fileHandler.SaveIDMappings(categories, subcategories, operatorGroups, operators, statuses);
            fileHandler.SaveIncidentSnapshots(snapshots);
            fileHandler.SaveChangeActivities(snapshots);

            if (!unattendedMode) { Console.WriteLine("\n\nTotal of " + totalImported.ToString("N0") + " tickets imported on this run"); }
        }

        public static void PullTicketList(string datecode, bool lastMonth)
        {
            List<Ticket> ticketsForDatecode = tdConnector.GetTicketsByDatecode(datecode);

            fileHandler.SaveTickets(datecode, ticketsForDatecode, lastMonth);
        }
    }
}