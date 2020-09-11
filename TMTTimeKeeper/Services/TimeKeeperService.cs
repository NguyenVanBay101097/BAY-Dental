using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<T> GetModelByJson<T>(string fileName)
        {
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            var pathJson = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(pathJson);
        }

        public async Task<IList<T>> GetListModelByJson<T>(string fileName)
        {
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            var pathJson = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<IList<T>>(pathJson);
        }

        public void SetJson<T>(string fileName, T val)
        {
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            if (val != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(val));
            else
                File.WriteAllText(path, string.Empty);
        }

        public void SetListJson<T>(string fileName, IEnumerable<T> val)
        {
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            if (val != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(val));
            else
                File.WriteAllText(path, string.Empty);
        }
    }
}
