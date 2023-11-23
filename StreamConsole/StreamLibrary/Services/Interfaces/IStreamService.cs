using StreamLibrary.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibrary.Services.Interfaces
{
    public interface IStreamService
    {
        event EventHandler<ServerDisconnectedEventArgs> ServerDisconnected;

        Task Connect();
        Task Disconnect();
        Task Send(byte[] bytes);
    }
}
