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

        public const string HTML_HEADER = "<head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"><meta name=\"MSSmartTagsPreventParsing\" content=\"TRUE\"><meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1\"><!--[if lt IE 7]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie.css?1348360344\"><![endif]--><!--[if lt IE 8]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie7.css?1348360344\"><![endif]--><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/main.css\"><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/bbcode.css\"><!-- <script src=\"/__utm.js\" type=\"text/javascript\"></script> --><!-- script src=\"/js/dojo/dojo.js?1348360344\" type=\"text/javascript\"></script --><script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/themes/redmond/jquery-ui.css\"><script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/forums.css\"><script type=\"text/javascript\">disable_thread_coloring = true;</script><script type=\"text/javascript\" src=\"/js/vb/forums.combined.js?1359653372\"></script><style type=\"text/css\"></style></head>";
    }
}
