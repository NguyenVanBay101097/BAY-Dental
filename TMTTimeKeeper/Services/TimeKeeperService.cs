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
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            File.WriteAllText(path, JsonConvert.SerializeObject(val));
        }

        public TimeKeeper getTimekeeper()
        {
            var timekeeper = new TimeKeeper();
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            using (StreamReader sr = File.OpenText(path))
            {
                timekeeper = JsonConvert.DeserializeObject<TimeKeeper>(sr.ReadToEnd());
            }
            return timekeeper;
        }

        public async Task<T> GetModelByJson<T>(string fileName)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            var pathJson = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(pathJson);
        }

        public async Task<IList<T>> GetListModelByJson<T>(string fileName)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            var pathJson = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<IList<T>>(pathJson);
        }

        public async Task SetJson<T>(string fileName, T val)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            if (val != null)
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(val));
            else
                await File.WriteAllTextAsync(path, string.Empty);
        }

        public async Task SetListJson<T>(string fileName, IEnumerable<T> val)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            if (val != null)
               await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(val));
            else
               await File.WriteAllTextAsync(path, string.Empty);
        }
    }
}
