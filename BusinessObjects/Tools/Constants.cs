using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Tools
{
    public class Constants
    {
        public const string BASE_URL = "http://forums.somethingawful.com/";

        public const string COOKIE_FILE = "SACookie2.txt";

        public const string GOTO_NEW_POST = "&goto=newpost";

        public const string QUOTE_URL = "http://forums.somethingawful.com/newreply.php?action=newreply&postid={0}";

        public const string THREAD_POST_URL = "http://forums.somethingawful.com/newreply.php?action=newreply&threadid={0}";

        public const string PAGE_NUMBER = "&pagenumber=";
    }
}
