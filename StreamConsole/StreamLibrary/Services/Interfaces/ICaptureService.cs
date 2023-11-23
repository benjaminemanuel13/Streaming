using StreamLibrary.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibrary.Services.Interfaces
{
    public interface ICaptureService
    {
        event EventHandler<DataCapturedEventArgs> Captured;
        Task StartCapturing(string filename, string capturefilename);
        Task StopCapturing();
    }
}
