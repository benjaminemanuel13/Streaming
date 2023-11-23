using StreamConsole;
using StreamingClientLinux.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingClientLinux.Skia.Gtk.Wrappers
{
    public class ControlWrapper : IControlWrapper
    {
        private Controller _control;

        public ControlWrapper()
        { 
            _control = new Controller();
        }

        public async Task Connect()
        {
            await _control.Connect();
        }

        public async Task Disconnect()
        {
            await _control.Disconnect();
        }

        public async Task Start()
        {
            await _control.Start();
        }

        public async Task Stop()
        {
            await _control.Stop();
        }
    }
}
