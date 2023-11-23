using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjStreamer.Data.Models
{
    public class DjUser
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }

        public string AccessKey { get; set; }
    }
}
