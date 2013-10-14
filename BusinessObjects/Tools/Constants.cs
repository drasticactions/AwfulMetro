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

        public const string USER_PROFILE = "member.php?action=getinfo&userid={0}";

        public const string USER_RAP_SHEET = "banlist.php?userid={0}";

        public const string RAP_SHEET = "banlist.php?";

        public const string QUOTE_URL = "http://forums.somethingawful.com/newreply.php?action=newreply&postid={0}";

        public const string THREAD_POST_URL = "http://forums.somethingawful.com/newreply.php?action=newreply&threadid={0}";

        public const string USER_POST_HISTORY = BASE_URL + "search.php?action=do_search_posthistory&userid={0}";

        public const string PAGE_NUMBER = "&pagenumber={0}";

        public const string THREAD_PAGE = BASE_URL + "showthread.php?threadid={0}";

        public const string FRONT_PAGE = "http://www.somethingawful.com";

        public const string USER_CP = "usercp.php?";

        public const string HTML_HEADER = "<head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/bbcode.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/forums.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/main.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"><meta name=\"MSSmartTagsPreventParsing\" content=\"TRUE\"><meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1\"><script type=\"text/javascript\" src=\"ms-appx-web:///Assets/jquery.min.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/jquery-ui.css\"><script type=\"text/javascript\" src=\"ms-appx-web:///Assets/jquery-ui.min.js\"></script><script type=\"text/javascript\">disable_thread_coloring = true;</script><script type=\"text/javascript\" src=\"ms-appx-web:///Assets/forums.combined.js\"></script><style type=\"text/css\"></style></head>";
    }
}
