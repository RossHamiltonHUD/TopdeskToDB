using System;
using System.Collections.Generic;
using System.Configuration;
using TopdeskDataCache.schema;

namespace TopdeskDataCache
{
    public static class DataHandler
    {
        //set the data folder's filepath
        private static string baseFilepath = ConfigurationManager.AppSettings.Get("config_filepath") + "topdeskData";

        // Grab our Topdesk credentials from the config file
        public static string topdeskEmailAddress = ConfigurationManager.AppSettings.Get("config_topdesk_email_address");
        public static string topdeskAppPassword = ConfigurationManager.AppSettings.Get("config_topdesk_application_password");

        //  Set up our classes for interacting with the Topdesk API, file saving, and the ODATA feed
        private static TopdeskConnector tdConnector = new TopdeskConnector();
        private static FileHandler fileHandler = new FileHandler(baseFilepath);
        private static oDataInterface odata = new oDataInterface(topdeskEmailAddress, topdeskAppPassword);

        public static void Main(string[] args)
        {
            // Check we have some credentials and quit if not
            if (topdeskEmailAddress == "" || topdeskAppPassword == "")  {   System.Environment.Exit(0); }

            //Get the selected starting year from the config
            int startingYear = Int16.Parse(ConfigurationManager.AppSettings.Get("config_starting_year"));

            // Figure out what Topdesk datecodes we'll need to pull
            List<string> datecodes = tdConnector.GetDatecodes(startingYear);
            
            // Run the import method, handles pulling data and saving it
            ImportToFile(datecodes);

            // store a timestamp in a text file so we know what snapshot/change activity data we have next time
            DateTime utcNow = DateTime.UtcNow;
            File.WriteAllText(baseFilepath + "\\last_run.txt", utcNow.Year.ToString() + "-" + utcNow.Month.ToString("00") + "-" + utcNow.Day.ToString("D2") +
                "T" + utcNow.Hour.ToString("D2") + ":" + utcNow.AddMinutes(-1).Minute.ToString("D2") + ":00Z");
        }

        public static void ImportToFile(List<string> datecodes)
        {
            //Check what data we already have ticket-wise for our date range
            List<string> datecodesToCheck = fileHandler.CheckNeededData(datecodes);

            /*----------- DATA COLLECTION -----------*/

            //grab tickets relating to each datecode via topdesk API
            foreach (string datecode in datecodesToCheck)
            {
                //Grab tickets
                List<Ticket> ticketsForDatecode = tdConnector.GetTicketsByDatecode(datecode);

                //Save ticket list to file
                fileHandler.SaveTickets(datecode, ticketsForDatecode, (datecode == datecodesToCheck[datecodesToCheck.Count - 1]));
            }

            //grab all the data we collect via ODATA feed and write each to disk
            string causedByChange = odata.GetIncidentsCausedByChange();            
            fileHandler.SaveCausedByChanges(causedByChange);

            string problems = odata.GetProblems();
            fileHandler.SaveProblems(problems);

            string problemDetails = odata.GetProblemDetails();            
            fileHandler.SaveProblemDetails(problemDetails);

            string problemIncidentLinks = odata.GetProblemIncidentLinks(); 
            fileHandler.SaveProblemIncidentLinks(problemIncidentLinks);

            //this block all gets saved in one folder as they're basically helper files and not actual data
            string categories = odata.GetCategories();
            string subcategories = odata.GetSubcategories();
            string operatorGroups = odata.GetOperatorGroups();
            string operators = odata.GetOperators();
            string statuses = odata.GetStatuses();
            fileHandler.SaveIDMappings(categories, subcategories, operatorGroups, operators, statuses);

            /* The snapshots and change activities get pulled incrementally using the last run date, and
             * other data files might follow this model in future in the interests of smaller API requests */
            string snapshots = odata.GetIncidentSnapshots(baseFilepath, File.ReadAllText(baseFilepath + "\\last_run.txt"));
            fileHandler.SaveIncidentSnapshots(snapshots);
            string changeActivities = odata.GetChangeActivities(File.ReadAllText(baseFilepath + "\\last_run.txt"));
            fileHandler.SaveChangeActivities(changeActivities);

            //grab KIs and Changes via Topdesk API
            List<KnowledgeItem> knowledge = tdConnector.GetKnowledge();
            List<Change> change = tdConnector.GetChanges();
            fileHandler.SaveKnowledge(knowledge);
            fileHandler.SaveChanges(change);
        }
    }
}