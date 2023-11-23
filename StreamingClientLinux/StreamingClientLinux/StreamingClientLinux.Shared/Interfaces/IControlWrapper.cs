using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamingClientLinux.Interfaces
{
    public interface IControlWrapper
    {
        Task Connect();
        Task Disconnect();
        Task Start();
        Task Stop();
    }
}
