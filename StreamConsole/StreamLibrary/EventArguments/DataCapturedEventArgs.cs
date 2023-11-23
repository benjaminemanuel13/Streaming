using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibrary.EventArguments
{
    public class DataCapturedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }

        public DataCapturedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}
