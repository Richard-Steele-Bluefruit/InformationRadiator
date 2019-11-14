using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace GitVisualiser.Model
{
    public class JsonReaderAndDeleter : IJsonReaderAndDeleter
    {
        public List<Tuple<string, double>> GetArchipelagoDataFromDirectory(string jsonDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(jsonDirectory);
            FileInfo[] Files = d.GetFiles("*.json");

            List<Tuple<string, double>> archipelagoData = new List<Tuple<string, double>>();
            foreach (FileInfo file in Files)
            {
                JObject jsonObject = JObject.Parse(File.ReadAllText(jsonDirectory + file.Name));

                string archipelagoName = (string)jsonObject["archipelago"];
                double distance = (double)jsonObject["distance"];

                var newArchipelago = new Tuple<string, double>(archipelagoName, distance);
                archipelagoData.Add(newArchipelago);
            }

            return archipelagoData;
        }

        public void RemoveJsonFilesFromDirectory(string jsonDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(jsonDirectory);
            FileInfo[] Files = d.GetFiles("*.json");

            foreach (FileInfo file in Files)
            {
                var fileDeleted = false;
                while (!fileDeleted) // the file may still be in use
                {
                    try
                    {
                        File.Delete(file.FullName);
                        fileDeleted = true;
                    }
                    catch (IOException)
                    {
                        // fileDeleted is still false, try again
                    }
                }
            }
        }
    }
}
