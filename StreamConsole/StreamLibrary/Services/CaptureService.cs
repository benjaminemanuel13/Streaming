using StreamLibrary.EventArguments;
using StreamLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibrary.Services
{
    public class CaptureService : ICaptureService
    {
        public delegate void CaptureDelegate(IntPtr ptr, int size);

        [DllImport("stream", EntryPoint = "setcallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetCaptureCallback(CaptureDelegate aCallback);

        [DllImport("stream", EntryPoint = "freecallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreeCaptureCallback();

        [DllImport("stream", EntryPoint = "capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartCapture(string format, string filename, string tempfilename);

        [DllImport("stream", EntryPoint = "stopcapture", CallingConvention = CallingConvention.Cdecl)]
        public static extern int StopCapture();

        public event EventHandler<DataCapturedEventArgs> Captured;

        //FileStream stream = null;
        CaptureDelegate capD = null;

        public void CaptureCallback(IntPtr ptr, int size)
        {
            byte[] data = new byte[size];
            Marshal.Copy(ptr, data, 0, size);

            Captured?.Invoke(this, new DataCapturedEventArgs(data));

            //stream.Write(data, 0, size);
            //stream.Flush();
        }

        public async Task StartCapturing(string filename, string capturefilename)
        {
            capD = new CaptureDelegate(CaptureCallback);
            await Task.Run(() => { SetCaptureCallback(capD); });

            //stream = new FileStream(capturefilename, FileMode.Create);

            Go(filename);
        }

        private async void Go(string filename)
        {
            //Microphone
            //string device = "audio=Microphone (Realtek High Definition Audio)";
            
            //Web Cam Microphone
            //string device = "audio=Microphone (HD Pro Webcam C920)";
            
            //Loopback
            //string device = "audio=Stereo Mix (Realtek High Definition Audio)";

            //This is the Dell Microphone(old)
            string device = "audio=Microphone (2- High Definition Audio Device)";

            //Raspberry Pi
            //string device = "plughw:1,0";
            //string device = "default";

            string temp = Path.GetFullPath(filename) + "temp.pcm";

            Task.Run(() => { 
                int res = StartCapture(device, filename, temp); 
            });
        }
        
        public async Task StopCapturing()
        {
            //FreeCaptureCallback();
            StopCapture();
        }
    }
}
