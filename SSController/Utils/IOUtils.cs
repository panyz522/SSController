using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSController.Utils
{
    public static class IOUtils
    {
        public static string GetDataFolderPath()
        {
            var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SSController");
            Directory.CreateDirectory(dataPath);
            return dataPath;
        }
    }
}
