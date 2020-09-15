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
             SetJson<TimeKeeper>(fileName, val);
        }

        public async Task<TimeKeeper> getTimekeeperAsync()
        {
            var timekeeper = new TimeKeeper();
            string fileName = "TimeKeeper.json";
            timekeeper = await GetModelByJson<TimeKeeper>(fileName);
            return timekeeper;
        }

        public async Task<T> GetModelByJson<T>(string fileName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (!File.Exists(path))
                File.Create(path);
            var pathJson = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(pathJson);
        }

        public async Task<IEnumerable<T>> GetListModelByJson<T>(string fileName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (!File.Exists(path))
                File.Create(path);
           
            var pathJson = await File.ReadAllTextAsync(path);

            return JsonConvert.DeserializeObject<IList<T>>(pathJson);
        }

        public void SetJson<T>(string fileName, T val)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (!File.Exists(path))
                File.Create(path);
            if (val != null)
                 File.WriteAllText(path, JsonConvert.SerializeObject(val));
            else
                 File.WriteAllText(path, string.Empty);
        }

        public void SetListJson<T>(string fileName, IEnumerable<T> val)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (!File.Exists(path))
                File.Create(path);
            if (val != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(val));
            else
                File.WriteAllText(path, string.Empty);
        }
    }
}
