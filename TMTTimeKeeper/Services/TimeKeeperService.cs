using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMTTimeKeeper.Models;

namespace TMTTimeKeeper.Services
{
    public class TimeKeeperService
    {
        public void AddTimekeeper(TimeKeeper val)
        {
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(val));
        }

        public TimeKeeper getTimekeeper()
        {
            var timekeeper = new TimeKeeper();
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                timekeeper = JsonConvert.DeserializeObject<TimeKeeper>(sr.ReadToEnd());
            }
            return timekeeper;
        }
    }
}
