using DjStreamer.Business.Services.Interfaces;
using DjStreamer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjStreamer.Business.Services
{
    public class DjUserService : IDjUserService
    {
        private readonly ApplicationDbContext _db;

        public DjUserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool KeyMatches(string key, string userId)
        {
            var user = _db.DjUsers.Find(userId);

            if (user == null || user.AccessKey != key)
            {
                return false;
            }

            return true;
        }
    }
}
