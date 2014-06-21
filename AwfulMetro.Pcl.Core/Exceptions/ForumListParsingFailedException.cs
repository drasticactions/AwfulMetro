using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Pcl.Core.Exceptions
{
    public class ForumListParsingFailedException : Exception
    {
        public ForumListParsingFailedException()
        {
        }

        public ForumListParsingFailedException(string message)
                    : base(message)
        {
        }
    }
}
