using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Pcl.Core.Entity
{
    public class ForumUserSearchEntity
    {
        public ForumUserSearchEntity(string username, long id)
        {
            this.Username = username;
            this.Id = id;
        }
        public string Username { get; private set; }

        public long Id { get; private set; }
    }
}
