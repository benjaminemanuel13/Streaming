using DjStreamer.Business.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;

namespace DjStreamer.Business.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly string _logpath;

        public LoggingService(string logpath)
        { 
            _logpath = logpath + "\\Logs\\";

            if (!Directory.Exists(_logpath))
            { 
                Directory.CreateDirectory(_logpath);
            }
        }

        private void CreateLogFile()
        {
            FileStream stream = new FileStream(_logpath + "\\Log.txt", FileMode.Create);
            stream.Close();
        }

        public void Log(string message) {
            FileStream stream = new FileStream(_logpath + "\\Log.txt", FileMode.Open);
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine(message);
            writer.Flush();
            stream.Flush();

            writer.Close();
            stream.Close();
        }

        public void ClearLog()
        {
            File.Delete(_logpath + "\\Log.txt");
        }

        public string GetLog()
        {
            FileStream stream = new FileStream(_logpath + "\\Log.txt", FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(stream);

            string log = reader.ReadToEnd();

            reader.Close();
            stream.Close();

            return log;
        }
    }
}
