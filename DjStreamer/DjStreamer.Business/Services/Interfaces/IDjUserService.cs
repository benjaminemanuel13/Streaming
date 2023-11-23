using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjStreamer.Business.Services.Interfaces
{
    public interface IDjUserService
    {
        bool KeyMatches(string key, string UserId);
    }
}
