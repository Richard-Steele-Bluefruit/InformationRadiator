using System;
using System.Linq;
using System.Collections.Generic;

namespace GitVisualiser.Model
{
    public interface IJsonReaderAndDeleter
    {
        List<Tuple<string, double>> GetArchipelagoDataFromDirectory(string jsonDirectory);
        void RemoveJsonFilesFromDirectory(string jsonDirectory);
    }
}
