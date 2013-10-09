using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Tools
{
    public class UserMetadata
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public List<Cookie> Cookies { get; set; }
    }
}
