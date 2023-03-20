using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TopdeskDataCache
{
    internal class FileHandler
    {
        string baseFilepath;

        public FileHandler(string baseFilepathArg)
        {
            baseFilepath = baseFilepathArg;
            System.IO.Directory.CreateDirectory(baseFilepath);
        }

        public List<string> CheckNeededData(List<string> datecodes)
        {
            List<string> datecodesToReturn = new List<string>();

            foreach (string datecode in datecodes)
            {
                string path = GetFilepathForDatecode(datecode);

                if (!File.Exists(path) || datecode == datecodes[datecodes.Count-1])
                {
                    datecodesToReturn.Add(datecode);
                    continue;
                }
            }

            return datecodesToReturn;
        }

        public void SaveTickets(string datecode, List<InputTicket> tickets, bool currentMonth)
        {
            string path = GetFilepathForDatecode(datecode);

            if (currentMonth) { File.Delete(@path); }

            using (StreamWriter file = File.CreateText(@path))
            {
                JsonSerializer serializer = new JsonSerializer();   
                serializer.Serialize(file, tickets);
            }
        }

        public string GetFilepathForDatecode(string datecode)
        {
            string year = "20" + datecode.Substring(1, 2);
            string dirPath = baseFilepath + "\\" + year + "\\";
            string completePath = dirPath + datecode + ".json";

            System.IO.Directory.CreateDirectory(dirPath);

            return completePath;
        }
    }
}
