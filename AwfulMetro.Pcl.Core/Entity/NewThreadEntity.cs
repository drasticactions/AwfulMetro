using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class NewThreadEntity
    {
        public ForumEntity Forum { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public PostIconEntity PostIcon { get; set; }

        public string FormKey { get; set; }

        public string FormCookie { get; set; }

        public bool ParseUrl { get; set; }

        public void MapTo(string subject, string content, ForumEntity forumEntity, PostIconEntity postIconEntity)
        {
            Subject = subject;
            Content = content;
            Forum = forumEntity;
            PostIcon = postIconEntity;
            ParseUrl = true;
        }
    }
}
