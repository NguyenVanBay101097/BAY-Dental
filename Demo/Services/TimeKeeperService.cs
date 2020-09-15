using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.Models;
using System.IO.MemoryMappedFiles;
using System.IO;

namespace Demo.Services
{
    public class TimeKeeperService
    {
        public void AddTimekeeper(TimeKeeper val)
        {
            string fileName = "TimeKeeper.json";
            SetJson<TimeKeeper>(fileName, val);
        }

        public async Task<TimeKeeper> getTimekeeper()
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
            var pathJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(pathJson);
        }

        public async Task<List<T>> GetListModelByJson<T>(string fileName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (!File.Exists(path))
                File.Create(path);
            var pathJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<T>>(pathJson);
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
