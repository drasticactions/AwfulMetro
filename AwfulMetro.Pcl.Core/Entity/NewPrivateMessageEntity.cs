using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class NewPrivateMessageEntity
    {
        public PostIconEntity Icon { get; set; }

        public string Title { get; set; }

        public string Receiver { get; set; }

        public string Body { get; set; }

        public void MapTo(PostIconEntity icon, string title, string receiver, string body)
        {
            this.Icon = icon;
            this.Title = title;
            this.Receiver = receiver;
            this.Body = body;
        }
    }
}
