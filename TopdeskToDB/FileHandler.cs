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
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2016");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2017");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2018");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2019");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2020");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2021");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2022");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2023");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\tickets\\2024");

            System.IO.Directory.CreateDirectory(baseFilepath + "\\knowledge");
            System.IO.Directory.CreateDirectory(baseFilepath + "\\changes");
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

        public void SaveTickets(string datecode, List<Ticket> tickets, bool currentMonth)
        {
            string path = GetFilepathForDatecode(datecode);

            if (currentMonth) { File.Delete(@path); }

            using (StreamWriter file = File.CreateText(@path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, tickets);
            }
        }
        public void SaveKnowledge(List<KnowledgeItem> knowledge)
        {
            string path = baseFilepath + "\\knowledge";
            System.IO.Directory.CreateDirectory(path);

            path += "\\knowledge.json";

            using (StreamWriter file = File.CreateText(@path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, knowledge);
            }
        }

        public void SaveChanges(List<Change> change)
        {
            string path = baseFilepath + "\\changes";
            System.IO.Directory.CreateDirectory(path);

            path += "\\changes.json";

            using (StreamWriter file = File.CreateText(@path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, change);
            }
        }
        public void SaveCausedByChanges(string causedByChanges)
        {
            string path = baseFilepath + "\\changes";
            System.IO.Directory.CreateDirectory(path);

            path += "\\causedByChanges.json";
            File.WriteAllText(path, causedByChanges);
        }

        public void SaveProblems(string problems)
        {
            string path = baseFilepath + "\\problems";
            System.IO.Directory.CreateDirectory(path);

            path += "\\problems.json";
            File.WriteAllText(path, problems);
        }

        public void SaveProblemDetails(string problems)
        {
            string path = baseFilepath + "\\problems";
            System.IO.Directory.CreateDirectory(path);

            path += "\\problemDetails.json";
            File.WriteAllText(path, problems);
        }

        public void SaveProblemIncidentLinks(string problemLinks)
        {
            string path = baseFilepath + "\\problems";
            System.IO.Directory.CreateDirectory(path);

            path += "\\problemIncidentLinks.json";
            File.WriteAllText(path, problemLinks);
        }

        public string GetFilepathForDatecode(string datecode)
        {
            string year = "20" + datecode.Substring(1, 2);
            string dirPath = baseFilepath + "\\tickets\\" + year + "\\";
            string completePath = dirPath + datecode + ".json";

            System.IO.Directory.CreateDirectory(dirPath);

            return completePath;
        }
    }
}
