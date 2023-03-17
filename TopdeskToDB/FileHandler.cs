using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TopdeskToDB
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

                if (!File.Exists(path))
                {
                    //Console.WriteLine(datecode + ".json not found");
                    datecodesToReturn.Add(datecode);
                    continue;
                }
                //Console.WriteLine(datecode + ".json already exists");
            }

            return datecodesToReturn;
        }

        public void SaveTickets(string datecode, List<InputTicket> tickets)
        {
            string path = GetFilepathForDatecode(datecode);

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
