﻿using System;
using System.Collections.Generic;
using System.Configuration;

namespace TopdeskDataCache
{
    public static class DataHandler
    {
        private static string baseFilepath = "topdeskData";

        //  Set up our classes for interacting with different systems
        private static TopdeskConnector tdConnector = new TopdeskConnector();
        private static FileHandler fileHandler = new FileHandler(baseFilepath);
        private static SqlConnector sqlConnector = new SqlConnector(); //-- For using a SQL Server

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

            //Get a sensible start year from the user
            //int startingYear = GetStartingYear();

            int startingYear = Int16.Parse(ConfigurationManager.AppSettings.Get("config_starting_year"));

            List<string> datecodes = tdConnector.GetDatecodes(startingYear);

            //ImportToDatabase(datecodes);
            //Console.WriteLine("Finished collecting data!");
            
            ImportToFile(datecodes);
            if (!unattendedMode) { Console.WriteLine("Finished collecting data!"); }

            Console.ReadLine();
        }

        public static int GetStartingYear()
        {
            bool yearSelected = false;
            int startYear = 0;
            int lastYear = Int16.Parse(DateTime.Now.ToString("yy")) -1;

            while (!yearSelected)
            {
                if (unattendedMode)
                {
                    //startYear = Int16.Parse(ConfigurationManager.AppSettings.Get("config_starting_year"));
                    startYear = 19;
                    yearSelected = true;
                    continue;
                }

                Console.WriteLine("Please enter a starting year (as 2 digits, between 2016 and now. For example, '18' or '21')");
                Console.WriteLine("Hit enter to default to last year");
                string userInput = Console.ReadLine();

                if (userInput == "")
                {
                    startYear = lastYear;
                    yearSelected = true;
                    continue;
                }

                var r = new System.Text.RegularExpressions.Regex("^\\d{2}$");

                if (r.Matches(userInput).Count != 1)
                {
                    Console.WriteLine("Please ensure you entered 2 digits only");
                    continue;
                }

                startYear = Int16.Parse(userInput);

                if (startYear > lastYear + 1 || startYear < 16)
                {
                    Console.WriteLine("Topdesk data started in 2016 - you must enter between 16 and " + (lastYear + 1));
                }

                yearSelected = true;

            }

            if (!unattendedMode)
            {
                Console.WriteLine("Selected year: " + startYear);
            }

            return startYear;
        }

        //public static void ImportToDatabase(List<string> datecodes)
        //{
        //    int totalImported = 0;
        //    int i = 0;

        //    foreach (string datecode in datecodes)
        //    {
        //        string percentageComplete = ((i * 100) / datecodes.Count) + "%";

        //        int ticketsFound = sqlConnector.GetExistingTicketCount(datecode);

        //        if (ticketsFound != 0)
        //        {
        //            Console.Write("\r(" + percentageComplete + ") " + ticketsFound.ToString("N0") + " tickets in DB for " + datecode + ", skipping...");
        //            i++;
        //            continue;
        //        }

        //        Console.Write("Fetching tickets for " + datecode + " using Topdesk API");

        //        Task<List<Ticket>> task = tdConnector.GetTicketsByDatecode(datecode);
        //        List<Ticket> ticketsForDatecode = task.Result;
        //        sqlConnector.InsertTickets(ticketsForDatecode);

        //        Console.WriteLine("\r(" + percentageComplete + ") Wrote " + ticketsForDatecode.Count.ToString("N0") + " tickets to the database for " + datecode);
        //        totalImported = totalImported + ticketsForDatecode.Count;
        //        i++;
        //    }
        //    Console.WriteLine();

        //    Console.WriteLine("Total of " + totalImported.ToString("N0") + " tickets imported on this run");
        //}

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
                //Task t = new Task(() => PullTicketList(datecode, lastMonth));
                //tasks.Add(t);
                //t.Start();
                PullTicketList(datecode, lastMonth);
            }

            string causedByChange = odata.GetIncidentsCausedByChange();
            string problems = odata.GetProblems();
            string problemDetails = odata.GetProblemDetails();
            string problemIncidentLinks = odata.GetProblemIncidentLinks(); 

            List<KnowledgeItem> knowledge = tdConnector.GetKnowledge();
            List<Change> change = tdConnector.GetChanges();

            fileHandler.SaveKnowledge(knowledge);
            fileHandler.SaveChanges(change);

            fileHandler.SaveCausedByChanges(causedByChange);
            fileHandler.SaveProblems(problems);
            fileHandler.SaveProblemDetails(problemDetails);
            fileHandler.SaveProblemIncidentLinks(problemIncidentLinks);

            //WaitAll(tasks);

            if (!unattendedMode) { Console.WriteLine("\n\nTotal of " + totalImported.ToString("N0") + " tickets imported on this run"); }
        }

        public static void WaitAll(List<Task> tasks)
        {
            if (tasks != null)
            {
                foreach (Task task in tasks)
                { task.Wait(); }
            }
        }

        public static void PullTicketList(string datecode, bool lastMonth)
        {
            List<Ticket> ticketsForDatecode = tdConnector.GetTicketsByDatecode(datecode);
            //Task t = new Task(() => ticketsForDatecode = tdConnector.GetTicketsByDatecode(datecode));
            //t.Start();
            //t.Wait();

            fileHandler.SaveTickets(datecode, ticketsForDatecode, lastMonth);
            //t = new Task(() => fileHandler.SaveTickets(datecode, ticketsForDatecode, lastMonth));
            //t.Start();
            //t.Wait();
        }
    }
}