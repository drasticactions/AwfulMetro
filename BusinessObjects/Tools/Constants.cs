using Windows.ApplicationModel.Contacts;

namespace AwfulMetro.Core.Tools
{
    public class Constants
    {
        public const string BOOKMARK_BACKGROUND = "BookmarkBackground";

        public const string BASE_URL = "http://forums.somethingawful.com/";

        public const string BOOKMARKS_URL = BASE_URL + "bookmarkthreads.php?perage=40&sortorder=desc&sortfield=";

        public const string COOKIE_FILE = "SACookie2.txt";

        public const string GOTO_NEW_POST = "&goto=newpost";

        public const string PER_PAGE = "&perpage=40";

        public const string USER_PROFILE = "member.php?action=getinfo&userid={0}";

        public const string USER_RAP_SHEET = "banlist.php?userid={0}";

        public const string RAP_SHEET = "banlist.php?";

        public const string FORUM_LIST_PAGE = "http://forums.somethingawful.com/forumdisplay.php?forumid=48";

        public const string FORUM_PAGE = "forumdisplay.php?forumid={0}";

        public const string QUOTE_EXP = "[quote=\"{0}\" post=\"{1}\"]{2}[/quote]";

        public const string RESET_SEEN = "action=resetseen&threadid={0}&json=1";

        public const string RESET_BASE = BASE_URL + "showthread.php";

        public const string BOOKMARK = BASE_URL + "bookmarkthreads.php";
        
        public const string REMOVE_BOOKMARK = "json=1&action=remove&threadid={0}";

        public const string ADD_BOOKMARK = "json=1&action=add&threadid={0}";

        public const string NEW_THREAD = BASE_URL + "newthread.php?action=newthread&forumid={0}";

        public const string NEW_REPLY = BASE_URL + "newreply.php";

        public const string REPLY_BASE = BASE_URL + "newreply.php?action=newreply&threadid={0}";

        public const string QUOTE_BASE = BASE_URL + "newreply.php?action=newreply&postid={0}";

        public const string USER_POST_HISTORY = BASE_URL + "search.php?action=do_search_posthistory&userid={0}";

        public const string PAGE_NUMBER = "&pagenumber={0}";

        public const string THREAD_PAGE = BASE_URL + "showthread.php?threadid={0}";

        public const string FRONT_PAGE = "http://www.somethingawful.com";

        public const string SMILE_URL = BASE_URL + "misc.php?action=showsmilies";

        public const string USER_CP = "usercp.php?";

        public const string HTML_FILE = "{0}.html";

        public const string HTML_HEADER = "<head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/bbcode.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/forums.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/main.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"><meta name=\"MSSmartTagsPreventParsing\" content=\"TRUE\"><meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1\"><script type=\"text/javascript\" src=\"ms-appx-web:///Assets/jquery.min.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/jquery-ui.css\"><script type=\"text/javascript\" src=\"ms-appx-web:///Assets/jquery-ui.min.js\"></script><script type=\"text/javascript\">disable_thread_coloring = true;</script><script type=\"text/javascript\" src=\"ms-appx-web:///Assets/forums.combined.js\"></script><style type=\"text/css\"></style></head>";

        public const int DEFAULT_TIMEOUT_IN_MILLISECONDS = 60000;

        public const string COOKIE_DOMAIN_URL = "http://fake.forums.somethingawful.com";

        public const string LOGIN_URL = "http://forums.somethingawful.com/account.php?";
    }
}
