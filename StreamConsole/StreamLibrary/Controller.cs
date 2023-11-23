using StreamLibrary.Services;
using StreamLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamConsole
{
    public class Controller
    {
        ICaptureService svc = new CaptureService();
        IStreamService streamService = new StreamService();

        public async Task Connect()
        {
            await streamService.Connect();
            streamService.ServerDisconnected += StreamService_ServerDisconnected;
        }

        public async Task Disconnect()
        {
            streamService.ServerDisconnected -= StreamService_ServerDisconnected;
            await streamService.Disconnect();
        }

        public async Task Start()
        {
            svc.Captured += Svc_Captured;

            //string outfile = @"D:\Projects\$FFMpeg\$$Streaming\StreamConsole\StreamLibrary\bin\Debug\net6.0\BensTest.mp3";
            //string capfile = @"D:\Projects\$FFMpeg\$$Streaming\StreamConsole\StreamLibrary\bin\Debug\net6.0\Captured.mp3";

            //string outfile = @"/home/ben/smile/BensTest.mp3";
            //string capfile = @"/home/ben/smile/Captured.mp3";

            string outfile = @"C:\Program Files\Streaming\BensTestTemp.mp3";
            string capfile = @"C:\Program Files\Streaming\BensTest.mp3";

            await svc.StartCapturing(outfile, capfile);
        }

        private void StreamService_ServerDisconnected(object? sender, StreamLibrary.EventArguments.ServerDisconnectedEventArgs e)
        {
            Stop();
        }

        public async Task Stop()
        {
            await svc.StopCapturing();
        }

        private async void Svc_Captured(object? sender, StreamLibrary.EventArguments.DataCapturedEventArgs e)
        {
            await streamService.Send(e.Data);
        }
    }
}
