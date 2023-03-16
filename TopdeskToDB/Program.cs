using System;
using System.Collections.Generic;

namespace TopdeskToDB
{
    public static class DataHandler
    {
        private static string baseFilepath = "topdeskData";

        private static TopdeskConnector tdConnector = new TopdeskConnector();
        private static SqlConnector sqlConnector = new SqlConnector();
        private static FileHandler fileHandler = new FileHandler(baseFilepath);

        public static void Main()
        {
            Console.WriteLine("Please enter a starting year (as 2 digits, e.g. '18' or '21')");
            int startYear = Int16.Parse(Console.ReadLine());

            List<string> datecodes = tdConnector.GetDatecodes(startYear);

            //ImportToDatabase(datecodes);

            //Console.WriteLine("Finished collecting data!");

            ImportToFile(datecodes);

            Console.ReadLine();
        }

        public static void ImportToDatabase(List<string> datecodes)
        {
            int totalImported = 0;
            int i = 0;
            
            foreach (string datecode in datecodes)
            {
                string percentageComplete = ((i * 100) / datecodes.Count) + "%";

                int ticketsFound = sqlConnector.GetExistingTicketCount(datecode);

                if (ticketsFound != 0)
                {
                    Console.Write("\r("+percentageComplete+") "+ticketsFound.ToString("N0") + " tickets in DB for " + datecode + ", skipping...");
                    i++;
                    continue;
                }

                Console.Write("Fetching tickets for " + datecode + " using Topdesk API");

                Task<List<Ticket>> task = tdConnector.GetTickets(datecode);
                List<Ticket> ticketsForDatecode = task.Result;
                sqlConnector.InsertTickets(ticketsForDatecode);

                Console.WriteLine("\r("+percentageComplete+") Wrote " + ticketsForDatecode.Count.ToString("N0") + " tickets to the database for " + datecode);
                totalImported = totalImported + ticketsForDatecode.Count;
                i++;
            }
            Console.WriteLine();

            Console.WriteLine("Total of " + totalImported.ToString("N0") + " tickets imported on this run");
        }

        public static void ImportToFile(List<string> datecodes)
        {
            List<string> datecodesToCheck = fileHandler.CheckNeededData(datecodes);

            if (datecodesToCheck.Count == 0)
            {
                Console.WriteLine("Files exist for all requested datecodes already!");
                return;
            }

            int totalImported = 0;
            int i = 0;

            foreach (string datecode in datecodesToCheck)
            {
                string percentageComplete = ((i * 100) / datecodesToCheck.Count) + "%";

                //int ticketsFound = sqlConnector.GetExistingTicketCount(datecode);

                Console.Write("Fetching tickets for " + datecode + " using Topdesk API");

                Task<List<Ticket>> task = tdConnector.GetTickets(datecode);
                List<Ticket> ticketsForDatecode = task.Result;
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