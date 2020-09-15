using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Demo.Models;

namespace Demo.Services
{
    public class TimeKeeperService
    {
        public void AddTimekeeper(TimeKeeper val)
        {
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(val));
        }

        public TimeKeeper getTimekeeper()
        {
            var timekeeper = new TimeKeeper();
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                timekeeper = JsonConvert.DeserializeObject<TimeKeeper>(sr.ReadToEnd());
            }
            return timekeeper;
        }

        public T GetModelByJson<T>(string fileName)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            var pathJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(pathJson);
        }

        public IList<T> GetListModelByJson<T>(string fileName)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            var pathJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<IList<T>>(pathJson);
        }

        public void SetJson<T>(string fileName, T val)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (val != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(val));
            else
                File.WriteAllText(path, string.Empty);
        }

        public void SetListJson<T>(string fileName, IEnumerable<T> val)
        {
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (val != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(val));
            else
                File.WriteAllText(path, string.Empty);
        }
    }
}
