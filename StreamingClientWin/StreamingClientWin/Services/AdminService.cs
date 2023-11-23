using DjStreamer.Models.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingClientWin.Services
{
    internal class AdminService
    {
#if DEBUG
        //private string url = "https://192.168.0.15:5130/api/Admin/";
        private string url = "https://localhost:7130/api/Admin/";
        //private string url = "https://smile-stream.club/api/Admin/";
        
#else
        //private string url = "https://smile-stream.club/api/Admin/";
        private string url = "http://192.168.0.15:5130/api/Admin/";
#endif

        public async Task<List<Server>> GetServers()
        {
            HttpClient client = new HttpClient();

            url += "GetServers";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            { 
                string json = await res.Content.ReadAsStringAsync();

                var servers = JsonConvert.DeserializeObject<List<Server>>(json);

                return servers;
            }

            return null;
        }

        public async Task<bool> ClearServers()
        {
            HttpClient client = new HttpClient();
            url += "ClearServers";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ClearLog()
        {
            HttpClient client = new HttpClient();
            url += "ClearLog";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<string> GetLog()
        {
            HttpClient client = new HttpClient();
            url += "GetLog";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                string log = await res.Content.ReadAsStringAsync();
                return log;
            }

            return "Failed Get Log";
        }
    }
}
