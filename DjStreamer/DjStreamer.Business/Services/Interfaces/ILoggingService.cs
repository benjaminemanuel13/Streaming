using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjStreamer.Business.Services.Interfaces
{
    public interface ILoggingService
    {
        void Log(string message);
        void ClearLog();
        string GetLog();
    }
}
