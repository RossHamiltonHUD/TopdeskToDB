using System;
using System.Collections.Generic;

namespace TopdeskToDB
{
    public static class DataHandler
    {
        //  Choose a directory to use to store the ticket data
        /*  This will accept a relative path (e.g. "topdeskData" for a dir inside the application directory,
            or an absolute path (e.g. "\\nas\CMSXHome\cmsx_itsupport\SDC Stats\TopdeskData" or "C:\TopdeskData"     */
        private static string baseFilepath = "topdeskData";

        //  Set up our classes for interacting with different systems
        private static TopdeskConnector tdConnector = new TopdeskConnector();
        private static FileHandler fileHandler = new FileHandler(baseFilepath);
        //private static SqlConnector sqlConnector = new SqlConnector(); -- For using a SQL Server

        public static void Main()
        {
            //This first section takes user input for the intended earliest collection year, and checks it's the right format
            int startYear = 0;

            while (startYear == 0)
            {
                Console.WriteLine("Please enter a starting year (as 2 digits, e.g. '18' or '21')");
                string userInput = Console.ReadLine();

                var r = new System.Text.RegularExpressions.Regex("^\\d{2}$");

                if (r.Matches(userInput).Count != 1)
                {
                    Console.WriteLine("Please ensure you entered 2 digits only");
                    continue;
                }

                startYear = Int16.Parse(userInput);
            }

            List<string> datecodes = tdConnector.GetDatecodes(startYear);

            //ImportToDatabase(datecodes
            Console.WriteLine("Finished collecting data!");
            ImportToFile(datecodes);

            Console.ReadLine();
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
        //            Console.Write("\r("+percentageComplete+") "+ticketsFound.ToString("N0") + " tickets in DB for " + datecode + ", skipping...");
        //            i++;
        //            continue;
        //        }

        //        Console.Write("Fetching tickets for " + datecode + " using Topdesk API");

        //        Task<List<InputTicket>> task = tdConnector.GetTicketsByDatecode(datecode);
        //        List<InputTicket> ticketsForDatecode = task.Result;
        //        sqlConnector.InsertTickets(ticketsForDatecode);

        //        Console.WriteLine("\r("+percentageComplete+") Wrote " + ticketsForDatecode.Count.ToString("N0") + " tickets to the database for " + datecode);
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
                Console.WriteLine("Files exist for all requested datecodes already!");
                return;
            }

            int totalImported = 0;
            int i = 1;

            foreach (string datecode in datecodesToCheck)
            {
                string percentageComplete = ((i * 100) / datecodesToCheck.Count) + "%";

                //int ticketsFound = sqlConnector.GetExistingTicketCount(datecode);

                Console.Write("Fetching tickets for " + datecode + " using Topdesk API");

                Task<List<InputTicket>> task = tdConnector.GetTicketsByDatecode(datecode);
                List<InputTicket> ticketsForDatecode = task.Result;
                fileHandler.SaveTickets(datecode, ticketsForDatecode);

                Console.WriteLine("\r(" + percentageComplete + ") Wrote " + ticketsForDatecode.Count.ToString("N0") + " tickets to file " + fileHandler.GetFilepathForDatecode(datecode));
                totalImported = totalImported + ticketsForDatecode.Count;
                i++;
            }
            Console.WriteLine();

            Console.WriteLine("Total of " + totalImported.ToString("N0") + " tickets imported on this run");
        }
    }
}