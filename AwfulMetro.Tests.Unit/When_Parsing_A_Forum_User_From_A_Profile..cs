﻿using System;
using System.Linq;
using AwfulMetro.Core.Entity;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    // This is only testing the properties currently parsed -- more tests should be added (ideally in TDD fashion) as additional properties are parsed
    [TestClass]
    public class When_Parsing_A_Forum_User_From_A_Profile
    {
        private const string DefaultUserProfileHtml =
            "<!DOCTYPE html>\n<html>\n<head>\n\t<title>Profile for 'elguignolgrande' - The Something Awful Forums</title>\n\t\n\t<meta name=\"MSSmartTagsPreventParsing\" content=\"TRUE\">\n\t<meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1\">\n\t<!--[if lt IE 7]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie.css?1348360344\"><![endif]-->\n\t<!--[if lt IE 8]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie7.css?1348360344\"><![endif]-->\n\t<link rel=\"apple-touch-icon\" href=\"http://i.somethingawful.com/core/icon/iphone-touch/forums.png\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/main.css?1359594161\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/bbcode.css?1348360344\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.somethingawful.com/globalcss/globalmenu.css\">\n\n\t\n\n\t<!-- <script src=\"/__utm.js\" type=\"text/javascript\"></script> -->\n\t<!-- script src=\"/js/dojo/dojo.js?1348360344\" type=\"text/javascript\"></script -->\n\t<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js\"></script>\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/themes/redmond/jquery-ui.css\">\n\t<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js\"></script>\n\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.somethingawful.com/css/forums.css?7\">\n\n\t\n\n\t<script type=\"text/javascript\">\n\t\t\n\t\tadjust_page_position = true;\n\t\tdisable_thread_coloring = true;\n\t\t\n\t</script>\n\n\t<script type=\"text/javascript\" src=\"/js/vb/forums.combined.js?1359653372\"></script>\n\n\t\n\n\t<!-- ts-specific includes -->\n\t\n\t\n\t\n\n\t\n\n\t\n\n\t\n</head>\n<body id=\"something_awful\" class=\"getinfo\">\n\n<div id=\"globalmenu\">\n\t<ul>\n\t\t<li class=\"first\"><a href=\"https://secure.somethingawful.com/\">Buy Forum Stuff</a></li>\n\t\t<li><a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n\t</ul>\n</div>\n\n\n\n<div id=\"container\">\n\n\n\n\t\n\n\t<ul id=\"nav_purchase\">\n\t\t<li><b>Purchase:</b></li>\n\t\t<li><a href=\"https://secure.somethingawful.com/products/register.php\">Account</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/platinum.php\">Platinum Upgrade</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/titlechange.php\">New Avatar</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/titlechange.php\">Other's Avatar</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/archives.php\">Archives</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/noads.php\">No-Ads</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/namechange.php\">New Username</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/ad-banner.php\">Banner Advertisement</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/smilie.php\">Smilie</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/sticky-thread.php\">Stick Thread</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/gift-certificate.php\">Gift Cert.</a></li>\n\t</ul>\n\n\t<ul id=\"navigation\" class=\"navigation\">\n<li class=\"first\"><a href=\"/index.php\">SA Forums</a></li>\n<li>- <a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n<li>- <a href=\"/f/search\">Search the Forums</a></li>\n<li>- <a href=\"/usercp.php\">User Control Panel</a></li>\n<li>- <a href=\"/private.php\">Private Messages</a></li>\n<li>- <a href=\"http://www.somethingawful.com/d/forum-rules/forum-rules.php\">Forum Rules</a></li>\n<li>- <a href=\"/dictionary.php\">SAclopedia</a></li>\n<li>- <a href=\"/stats.php\">Posting Gloryhole</a></li>\n<li>- <a href=\"/banlist.php\">Leper's Colony</a></li>\n<li>- <a href=\"/supportmail.php\">Support</a></li>\n<li>- <a href=\"/account.php?action=logout&amp;ma=05fd1b5e\">Log Out</a></li>\n\n</ul>\n\n\n<div class=\"oma_pal\">\n\t<!--  Begin Rubicon Project Tag -->\n<!--  Site: Something Awful LLC   Zone: Forum ATF Top Quality - Mobile, Pop, Web   Size: Leaderboard  -->\n<!--  PLACEMENT: Above the Fold  -->\n<script language=\"JavaScript\" type=\"text/javascript\">\nrp_account   = '8539';\nrp_site      = '13831';\nrp_zonesize  = '80354-2';\nrp_adtype    = 'iframe';\nrp_width     = '728';\nrp_height    = '90';\nrp_smartfile = 'http://www.somethingawful.com/revv_smart_file.html';\n</script>\n<script type=\"text/javascript\" src=\"https://ads.rubiconproject.com/ad/8539.js\"></script>\n<!--  End Rubicon Project Tag -->\n</div>\n\n\n\n\n\n\t<div id=\"content\">\n\n\t\n\n<table class=\"standard\" style=\"width:800px;margin:0 auto;\">\n<tr>\n<th colspan=\"2\">\n\tUser Profile <span class=\"smalltext\">(<a style=\"color:#fff!important;\" href=\"search.php?action=do_search_posthistory&amp;userid=12831\">find posts by user</a>)</span>\n</th>\n</tr>\n<tr>\n\t<td id=\"thread\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\">elguignolgrande</dt>\n\t\t\t<dd class=\"registered\">Sep  2, 2000</dd>\n\t\t\t<dd class=\"title\"><img src=\"http://i.somethingawful.com/u/elpintogrande/nov12/batmanavatar3.gif\" alt=\"\" class=\"img\" border=\"0\"><br></dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"info\">\n\t\t<h3>About elguignolgrande</h3>\n\t\t<p>\n\t\tThere have been <b>18540</b> posts made by <i>elguignolgrande</i>, an\n\t\taverage of 3.86 posts per day, since registering on <b>Sep  2, 2000</b>.\n\t\t<i>elguignolgrande</i> claims to be a male.\n\t\t</p>\n\t\t<p>This moron has not provided any additional info.  The lack of a gender-specific pronoun here is in no way intended as sexism.</p>\n\n\t\t<h3>Contact Information</h3>\n\t\t<dl class=\"contacts\">\n\t\t\t<dt class=\"pm\">Message</dt><dd><a href=\"private.php?action=newmessage&amp;userid=12831\">Send a private message</a>&nbsp;</dd>\n\n\t\t\t<dt class=\"icq\">ICQ</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"aim\">AIM</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"yahoo\">Yahoo!</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"homepage\">Home Page</dt><dd><a href=\"http://www.bostonandshaun.com\" target=\"_blank\" alt=\"\">http://www.bostonandshaun.com</a>&nbsp;</dd>\n\t\t</dl>\n\n\t\t<h3>User Picture</h3><div class=\"userpic\"><img src=\"/userpic.php?userid=12831\" alt=\"\"></div>\n\n\t\t<h3>Additional Info</h3>\n\t\t<dl class=\"additional\">\n\t\t\t<dt>Member Since</dt><dd>Sep  2, 2000</dd>\n\t\t\t<dt>Post Count</dt><dd>18540</dd>\n\t\t\t<dt>Post Rate</dt><dd>3.86 per day</dd>\n\t\t\t<dt>Last Post</dt><dd>Nov  1, 2013 10:21\n\t\t\t\t\t\t<dt>Seller Rating</dt><dd>5, 5</dd>\n\t\t</dl>\n\t</td>\n</tr>\n\n<tr>\n\t<td style=\"text-align:center\" colspan=\"2\">\n\n\t\t<form method=\"POST\" action=\"/member2.php\">\n\t\t<input type=\"hidden\" name=\"action\" value=\"addlist\">\n\t\t<input type=\"hidden\" name=\"userlist\" value=\"buddy\">\n\t\t<input type=\"hidden\" name=\"formkey\" value=\"36c44f7075ed4a2bf0313e02c0a72ad0\">\n\t\t<input type=\"hidden\" name=\"userid\" value=\"12831\">\n\t\t<input type=\"submit\" value=\"Add user to your Buddy List\">\n\t\t</form>\n\n\t\t<form method=\"POST\" action=\"/member2.php\">\n\t\t<input type=\"hidden\" name=\"action\" value=\"addlist\">\n\t\t<input type=\"hidden\" name=\"userlist\" value=\"ignore\">\n\t\t<input type=\"hidden\" name=\"formkey\" value=\"36c44f7075ed4a2bf0313e02c0a72ad0\">\n\t\t<input type=\"hidden\" name=\"userid\" value=\"12831\">\n\t\t<input type=\"submit\" value=\"Add user to your Ignore List\">\n\t\t</form>\n\n\t</td>\n</tr>\n</table>\n\n<br>\n\n</div><!-- #content -->\n\t<ul class=\"navigation\">\n<li class=\"first\"><a href=\"/index.php\">SA Forums</a></li>\n<li>- <a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n<li>- <a href=\"/f/search\">Search the Forums</a></li>\n<li>- <a href=\"/usercp.php\">User Control Panel</a></li>\n<li>- <a href=\"/private.php\">Private Messages</a></li>\n<li>- <a href=\"http://www.somethingawful.com/d/forum-rules/forum-rules.php\">Forum Rules</a></li>\n<li>- <a href=\"/dictionary.php\">SAclopedia</a></li>\n<li>- <a href=\"/stats.php\">Posting Gloryhole</a></li>\n<li>- <a href=\"/banlist.php\">Leper's Colony</a></li>\n<li>- <a href=\"/supportmail.php\">Support</a></li>\n<li>- <a href=\"/account.php?action=logout&amp;ma=05fd1b5e\">Log Out</a></li>\n\n</ul>\n\t<div id=\"copyright\">\n\t\tPowered by: vBulletin Version 2.2.9 (<a href=\"/CHANGES\">SAVB-v2.1.17</a>)<br>\n\t\tCopyright &copy;2000, 2001, Jelsoft Enterprises Limited.<br>\n\t\tCopyright &copy;2012, Something Awful LLC<br>\n\t</div>\n\n</div><!-- #container -->\n\n<script type=\"text/javascript\">\n\t// quantcast\n\t_qoptions = { qacct:\"p-82vTvmw-enlng\" };\n\n\ttry {\n\t\tif(document.location.hostname != 'forums.somethingawful.com')\n\t\t\tthrow undefined;\n\n\t\t$(document).ready(function() {\n\t\t\tvar qcUrl = 'http://edge.quantserve.com/quant.js';\n\t\t\tjQuery.getScript(qcUrl);\n\n\t\t\tvar gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");\n\t\t\tvar gaUrl = gaJsHost + 'google-analytics.com/ga.js';\n\t\t\tjQuery.getScript(gaUrl, function() {\n\t\t\t\tvar pageTracker = _gat._getTracker(\"UA-3064978-2\");\n\t\t\t\tpageTracker._initData();\n\t\t\t\tpageTracker._trackPageview();\n\t\t\t});\n\t\t});\n\t} catch(e) {};\n\n\t// indie\n\t// try {\n\t// \tif(document.location.hostname != 'forums.somethingawful.com')\n\t// \t\tthrow undefined;\n\n\t// \t$(document).ready(function() {\n\t// \t\tvar ic_element = document.createElement('script');\n\t// \t\tic_element.type = 'text/javascript';\n\t// \t\tic_element.async = true;\n\t// \t\tic_element.id = 'ic_annonymous_pixel';\n\t// \t\tic_element.src = 'http://pixel.indieclick.com/annonymous/domain/somethingawful.com/reach/script_ic.js';\n\t// \t\tvar ic_script = document.getElementsByTagName('script')[0];\n\t// \t\tic_script.parentNode.insertBefore(ic_element, ic_script);\n\t// \t});\n\t// } catch(e) {};\n</script>\n<noscript><img src=\"http://pixel.quantserve.com/pixel/p-82vTvmw-enlng.gif\" style=\"display:none;\" border=\"0\" height=\"1\" width=\"1\" alt=\"Quantcast\"></noscript>\n\n<!-- PLEASE DO NOT REMOVE -->\n<ol id=\"debug\" style=\"display:none\"><li>&nbsp;</ol>\n\n</body>\n</html>";

        private const string UserProfileWithLocation =
            "<!DOCTYPE html>\n<html>\n<head>\n\t<title>Profile for 'Ithaqua' - The Something Awful Forums</title>\n\t\n\t<meta name=\"MSSmartTagsPreventParsing\" content=\"TRUE\">\n\t<meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1\">\n\t<!--[if lt IE 7]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie.css?1348360344\"><![endif]-->\n\t<!--[if lt IE 8]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie7.css?1348360344\"><![endif]-->\n\t<link rel=\"apple-touch-icon\" href=\"http://i.somethingawful.com/core/icon/iphone-touch/forums.png\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/main.css?1359594161\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/bbcode.css?1348360344\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.somethingawful.com/globalcss/globalmenu.css\">\n\n\t\n\n\t<!-- <script src=\"/__utm.js\" type=\"text/javascript\"></script> -->\n\t<!-- script src=\"/js/dojo/dojo.js?1348360344\" type=\"text/javascript\"></script -->\n\t<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js\"></script>\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/themes/redmond/jquery-ui.css\">\n\t<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js\"></script>\n\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.somethingawful.com/css/forums.css?7\">\n\n\t\n\n\t<script type=\"text/javascript\">\n\t\t\n\t\tadjust_page_position = true;\n\t\tdisable_thread_coloring = true;\n\t\t\n\t</script>\n\n\t<script type=\"text/javascript\" src=\"/js/vb/forums.combined.js?1359653372\"></script>\n\n\t\n\n\t<!-- ts-specific includes -->\n\t\n\t\n\t\n\n\t\n\n\t\n\n\t\n</head>\n<body id=\"something_awful\" class=\"getinfo\">\n\n<div id=\"globalmenu\">\n\t<ul>\n\t\t<li class=\"first\"><a href=\"https://secure.somethingawful.com/\">Buy Forum Stuff</a></li>\n\t\t<li><a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n\t</ul>\n</div>\n\n\n\n<div id=\"container\">\n\n\n\n\t\n\n\t<ul id=\"nav_purchase\">\n\t\t<li><b>Purchase:</b></li>\n\t\t<li><a href=\"https://secure.somethingawful.com/products/register.php\">Account</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/platinum.php\">Platinum Upgrade</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/titlechange.php\">New Avatar</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/titlechange.php\">Other's Avatar</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/archives.php\">Archives</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/noads.php\">No-Ads</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/namechange.php\">New Username</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/ad-banner.php\">Banner Advertisement</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/smilie.php\">Smilie</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/sticky-thread.php\">Stick Thread</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/gift-certificate.php\">Gift Cert.</a></li>\n\t</ul>\n\n\t<ul id=\"navigation\" class=\"navigation\">\n<li class=\"first\"><a href=\"/index.php\">SA Forums</a></li>\n<li>- <a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n<li>- <a href=\"/f/search\">Search the Forums</a></li>\n<li>- <a href=\"/usercp.php\">User Control Panel</a></li>\n<li>- <a href=\"/private.php\">Private Messages</a></li>\n<li>- <a href=\"http://www.somethingawful.com/d/forum-rules/forum-rules.php\">Forum Rules</a></li>\n<li>- <a href=\"/dictionary.php\">SAclopedia</a></li>\n<li>- <a href=\"/stats.php\">Posting Gloryhole</a></li>\n<li>- <a href=\"/banlist.php\">Leper's Colony</a></li>\n<li>- <a href=\"/supportmail.php\">Support</a></li>\n<li>- <a href=\"/account.php?action=logout&amp;ma=05fd1b5e\">Log Out</a></li>\n\n</ul>\n\n\n<div class=\"oma_pal\">\n\t<!--  Begin Rubicon Project Tag -->\n<!--  Site: Something Awful LLC   Zone: Forum ATF Top Quality - Mobile, Pop, Web   Size: Leaderboard  -->\n<!--  PLACEMENT: Above the Fold  -->\n<script language=\"JavaScript\" type=\"text/javascript\">\nrp_account   = '8539';\nrp_site      = '13831';\nrp_zonesize  = '80354-2';\nrp_adtype    = 'iframe';\nrp_width     = '728';\nrp_height    = '90';\nrp_smartfile = 'http://www.somethingawful.com/revv_smart_file.html';\n</script>\n<script type=\"text/javascript\" src=\"https://ads.rubiconproject.com/ad/8539.js\"></script>\n<!--  End Rubicon Project Tag -->\n</div>\n\n\n\n\n\n\t<div id=\"content\">\n\n\t\n\n<table class=\"standard\" style=\"width:800px;margin:0 auto;\">\n<tr>\n<th colspan=\"2\">\n\tUser Profile <span class=\"smalltext\">(<a style=\"color:#fff!important;\" href=\"search.php?action=do_search_posthistory&amp;userid=39373\">find posts by user</a>)</span>\n</th>\n</tr>\n<tr>\n\t<td id=\"thread\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\">Ithaqua</dt>\n\t\t\t<dd class=\"registered\">Jul 17, 2003</dd>\n\t\t\t<dd class=\"title\"><img alt=\"\" border=\"0\" src=\"http://fi.somethingawful.com/customtitles/title-ithaqua.gif\" /><br />Only in Kenya.\n<br></dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"info\">\n\t\t<h3>About Ithaqua</h3>\n\t\t<p>\n\t\tThere have been <b>8748</b> posts made by <i>Ithaqua</i>, an\n\t\taverage of 2.33 posts per day, since registering on <b>Jul 17, 2003</b>.\n\t\t<i>Ithaqua</i> claims to be a male.\n\t\t</p>\n\t\t<p>This moron has not provided any additional info.  The lack of a gender-specific pronoun here is in no way intended as sexism.</p>\n\n\t\t<h3>Contact Information</h3>\n\t\t<dl class=\"contacts\">\n\t\t\t<dt class=\"pm\">Message</dt><dd><a href=\"private.php?action=newmessage&amp;userid=39373\">Send a private message</a>&nbsp;</dd>\n\n\t\t\t<dt class=\"icq\">ICQ</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"aim\">AIM</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"yahoo\">Yahoo!</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"homepage\">Home Page</dt><dd><span class=\"unset\">not set</span>&nbsp;</dd>\n\t\t</dl>\n\n\t\t\n\n\t\t<h3>Additional Info</h3>\n\t\t<dl class=\"additional\">\n\t\t\t<dt>Member Since</dt><dd>Jul 17, 2003</dd>\n\t\t\t<dt>Post Count</dt><dd>8748</dd>\n\t\t\t<dt>Post Rate</dt><dd>2.33 per day</dd>\n\t\t\t<dt>Last Post</dt><dd>Nov  2, 2013 17:49\n\t\t\t\t\t\t<dt>Location</dt><dd>cardboard box</dd>\t\t\t<dt>Interests</dt><dd>dinosaurs</dd>\t\t\t<dt>Occupation</dt><dd>i'm a girl lolo</dd>\n\t\t</dl>\n\t</td>\n</tr>\n\n<tr>\n\t<td style=\"text-align:center\" colspan=\"2\">\n\n\t\t<form method=\"POST\" action=\"/member2.php\">\n\t\t<input type=\"hidden\" name=\"action\" value=\"addlist\">\n\t\t<input type=\"hidden\" name=\"userlist\" value=\"buddy\">\n\t\t<input type=\"hidden\" name=\"formkey\" value=\"36c44f7075ed4a2bf0313e02c0a72ad0\">\n\t\t<input type=\"hidden\" name=\"userid\" value=\"39373\">\n\t\t<input type=\"submit\" value=\"Add user to your Buddy List\">\n\t\t</form>\n\n\t\t<form method=\"POST\" action=\"/member2.php\">\n\t\t<input type=\"hidden\" name=\"action\" value=\"addlist\">\n\t\t<input type=\"hidden\" name=\"userlist\" value=\"ignore\">\n\t\t<input type=\"hidden\" name=\"formkey\" value=\"36c44f7075ed4a2bf0313e02c0a72ad0\">\n\t\t<input type=\"hidden\" name=\"userid\" value=\"39373\">\n\t\t<input type=\"submit\" value=\"Add user to your Ignore List\">\n\t\t</form>\n\n\t</td>\n</tr>\n</table>\n\n<br>\n\n</div><!-- #content -->\n\t<ul class=\"navigation\">\n<li class=\"first\"><a href=\"/index.php\">SA Forums</a></li>\n<li>- <a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n<li>- <a href=\"/f/search\">Search the Forums</a></li>\n<li>- <a href=\"/usercp.php\">User Control Panel</a></li>\n<li>- <a href=\"/private.php\">Private Messages</a></li>\n<li>- <a href=\"http://www.somethingawful.com/d/forum-rules/forum-rules.php\">Forum Rules</a></li>\n<li>- <a href=\"/dictionary.php\">SAclopedia</a></li>\n<li>- <a href=\"/stats.php\">Posting Gloryhole</a></li>\n<li>- <a href=\"/banlist.php\">Leper's Colony</a></li>\n<li>- <a href=\"/supportmail.php\">Support</a></li>\n<li>- <a href=\"/account.php?action=logout&amp;ma=05fd1b5e\">Log Out</a></li>\n\n</ul>\n\t<div id=\"copyright\">\n\t\tPowered by: vBulletin Version 2.2.9 (<a href=\"/CHANGES\">SAVB-v2.1.17</a>)<br>\n\t\tCopyright &copy;2000, 2001, Jelsoft Enterprises Limited.<br>\n\t\tCopyright &copy;2012, Something Awful LLC<br>\n\t</div>\n\n</div><!-- #container -->\n\n<script type=\"text/javascript\">\n\t// quantcast\n\t_qoptions = { qacct:\"p-82vTvmw-enlng\" };\n\n\ttry {\n\t\tif(document.location.hostname != 'forums.somethingawful.com')\n\t\t\tthrow undefined;\n\n\t\t$(document).ready(function() {\n\t\t\tvar qcUrl = 'http://edge.quantserve.com/quant.js';\n\t\t\tjQuery.getScript(qcUrl);\n\n\t\t\tvar gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");\n\t\t\tvar gaUrl = gaJsHost + 'google-analytics.com/ga.js';\n\t\t\tjQuery.getScript(gaUrl, function() {\n\t\t\t\tvar pageTracker = _gat._getTracker(\"UA-3064978-2\");\n\t\t\t\tpageTracker._initData();\n\t\t\t\tpageTracker._trackPageview();\n\t\t\t});\n\t\t});\n\t} catch(e) {};\n\n\t// indie\n\t// try {\n\t// \tif(document.location.hostname != 'forums.somethingawful.com')\n\t// \t\tthrow undefined;\n\n\t// \t$(document).ready(function() {\n\t// \t\tvar ic_element = document.createElement('script');\n\t// \t\tic_element.type = 'text/javascript';\n\t// \t\tic_element.async = true;\n\t// \t\tic_element.id = 'ic_annonymous_pixel';\n\t// \t\tic_element.src = 'http://pixel.indieclick.com/annonymous/domain/somethingawful.com/reach/script_ic.js';\n\t// \t\tvar ic_script = document.getElementsByTagName('script')[0];\n\t// \t\tic_script.parentNode.insertBefore(ic_element, ic_script);\n\t// \t});\n\t// } catch(e) {};\n</script>\n<noscript><img src=\"http://pixel.quantserve.com/pixel/p-82vTvmw-enlng.gif\" style=\"display:none;\" border=\"0\" height=\"1\" width=\"1\" alt=\"Quantcast\"></noscript>\n\n<!-- PLEASE DO NOT REMOVE -->\n<ol id=\"debug\" style=\"display:none\"><li>&nbsp;</ol>\n\n</body>\n</html>";

        private const string UserProfileWithoutAvatarOrTitle =
            "<!DOCTYPE html>\n<html>\n<head>\n\t<title>Profile for 'Racetam' - The Something Awful Forums</title>\n\t\n\t<meta name=\"MSSmartTagsPreventParsing\" content=\"TRUE\">\n\t<meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1\">\n\t<!--[if lt IE 7]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie.css?1348360344\"><![endif]-->\n\t<!--[if lt IE 8]><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/ie7.css?1348360344\"><![endif]-->\n\t<link rel=\"apple-touch-icon\" href=\"http://i.somethingawful.com/core/icon/iphone-touch/forums.png\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/main.css?1359594161\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/bbcode.css?1348360344\">\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.somethingawful.com/globalcss/globalmenu.css\">\n\n\t\n\n\t<!-- <script src=\"/__utm.js\" type=\"text/javascript\"></script> -->\n\t<!-- script src=\"/js/dojo/dojo.js?1348360344\" type=\"text/javascript\"></script -->\n\t<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js\"></script>\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/themes/redmond/jquery-ui.css\">\n\t<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js\"></script>\n\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.somethingawful.com/css/forums.css?7\">\n\n\t\n\n\t<script type=\"text/javascript\">\n\t\t\n\t\tadjust_page_position = true;\n\t\tdisable_thread_coloring = true;\n\t\t\n\t</script>\n\n\t<script type=\"text/javascript\" src=\"/js/vb/forums.combined.js?1359653372\"></script>\n\n\t\n\n\t<!-- ts-specific includes -->\n\t\n\t\n\t\n\n\t\n\n\t\n\n\t\n</head>\n<body id=\"something_awful\" class=\"getinfo\">\n\n<div id=\"globalmenu\">\n\t<ul>\n\t\t<li class=\"first\"><a href=\"https://secure.somethingawful.com/\">Buy Forum Stuff</a></li>\n\t\t<li><a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n\t</ul>\n</div>\n\n\n\n<div id=\"container\">\n\n\n\n\t\n\n\t<ul id=\"nav_purchase\">\n\t\t<li><b>Purchase:</b></li>\n\t\t<li><a href=\"https://secure.somethingawful.com/products/register.php\">Account</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/platinum.php\">Platinum Upgrade</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/titlechange.php\">New Avatar</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/titlechange.php\">Other's Avatar</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/archives.php\">Archives</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/noads.php\">No-Ads</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/namechange.php\">New Username</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/ad-banner.php\">Banner Advertisement</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/smilie.php\">Smilie</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/sticky-thread.php\">Stick Thread</a></li>\n\t\t<li>- <a href=\"https://secure.somethingawful.com/products/gift-certificate.php\">Gift Cert.</a></li>\n\t</ul>\n\n\t<ul id=\"navigation\" class=\"navigation\">\n<li class=\"first\"><a href=\"/index.php\">SA Forums</a></li>\n<li>- <a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n<li>- <a href=\"/f/search\">Search the Forums</a></li>\n<li>- <a href=\"/usercp.php\">User Control Panel</a></li>\n<li>- <a href=\"/private.php\">Private Messages</a></li>\n<li>- <a href=\"http://www.somethingawful.com/d/forum-rules/forum-rules.php\">Forum Rules</a></li>\n<li>- <a href=\"/dictionary.php\">SAclopedia</a></li>\n<li>- <a href=\"/stats.php\">Posting Gloryhole</a></li>\n<li>- <a href=\"/banlist.php\">Leper's Colony</a></li>\n<li>- <a href=\"/supportmail.php\">Support</a></li>\n<li>- <a href=\"/account.php?action=logout&amp;ma=05fd1b5e\">Log Out</a></li>\n\n</ul>\n\n\n<div class=\"oma_pal\">\n\t<!--  Begin Rubicon Project Tag -->\n<!--  Site: Something Awful LLC   Zone: Forum ATF Top Quality - Mobile, Pop, Web   Size: Leaderboard  -->\n<!--  PLACEMENT: Above the Fold  -->\n<script language=\"JavaScript\" type=\"text/javascript\">\nrp_account   = '8539';\nrp_site      = '13831';\nrp_zonesize  = '80354-2';\nrp_adtype    = 'iframe';\nrp_width     = '728';\nrp_height    = '90';\nrp_smartfile = 'http://www.somethingawful.com/revv_smart_file.html';\n</script>\n<script type=\"text/javascript\" src=\"https://ads.rubiconproject.com/ad/8539.js\"></script>\n<!--  End Rubicon Project Tag -->\n</div>\n\n\n\n\n\n\t<div id=\"content\">\n\n\t\n\n<table class=\"standard\" style=\"width:800px;margin:0 auto;\">\n<tr>\n<th colspan=\"2\">\n\tUser Profile <span class=\"smalltext\">(<a style=\"color:#fff!important;\" href=\"search.php?action=do_search_posthistory&amp;userid=190010\">find posts by user</a>)</span>\n</th>\n</tr>\n<tr>\n\t<td id=\"thread\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\">Racetam</dt>\n\t\t\t<dd class=\"registered\">Sep 23, 2012</dd>\n\t\t\t<dd class=\"title\"><br></dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"info\">\n\t\t<h3>About Racetam</h3>\n\t\t<p>\n\t\tThere have been <b>138</b> posts made by <i>Racetam</i>, an\n\t\taverage of 0.34 posts per day, since registering on <b>Sep 23, 2012</b>.\n\t\t<i>Racetam</i> claims to be a porpoise.\n\t\t</p>\n\t\t<p>This moron has not provided any additional info.  The lack of a gender-specific pronoun here is in no way intended as sexism.</p>\n\n\t\t<h3>Contact Information</h3>\n\t\t<dl class=\"contacts\">\n\t\t\t<dt class=\"pm\">Message</dt><dd>&nbsp;</dd>\n\n\t\t\t<dt class=\"icq\">ICQ</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"aim\">AIM</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"yahoo\">Yahoo!</dt><dd><span class=\"unset\">not set</span></dd>\n\t\t\t<dt class=\"homepage\">Home Page</dt><dd><span class=\"unset\">not set</span>&nbsp;</dd>\n\t\t</dl>\n\n\t\t\n\n\t\t<h3>Additional Info</h3>\n\t\t<dl class=\"additional\">\n\t\t\t<dt>Member Since</dt><dd>Sep 23, 2012</dd>\n\t\t\t<dt>Post Count</dt><dd>138</dd>\n\t\t\t<dt>Post Rate</dt><dd>0.34 per day</dd>\n\t\t\t<dt>Last Post</dt><dd>Nov  2, 2013 13:47\n\t\t\t\n\t\t</dl>\n\t</td>\n</tr>\n\n<tr>\n\t<td style=\"text-align:center\" colspan=\"2\">\n\n\t\t<form method=\"POST\" action=\"/member2.php\">\n\t\t<input type=\"hidden\" name=\"action\" value=\"addlist\">\n\t\t<input type=\"hidden\" name=\"userlist\" value=\"buddy\">\n\t\t<input type=\"hidden\" name=\"formkey\" value=\"36c44f7075ed4a2bf0313e02c0a72ad0\">\n\t\t<input type=\"hidden\" name=\"userid\" value=\"190010\">\n\t\t<input type=\"submit\" value=\"Add user to your Buddy List\">\n\t\t</form>\n\n\t\t<form method=\"POST\" action=\"/member2.php\">\n\t\t<input type=\"hidden\" name=\"action\" value=\"addlist\">\n\t\t<input type=\"hidden\" name=\"userlist\" value=\"ignore\">\n\t\t<input type=\"hidden\" name=\"formkey\" value=\"36c44f7075ed4a2bf0313e02c0a72ad0\">\n\t\t<input type=\"hidden\" name=\"userid\" value=\"190010\">\n\t\t<input type=\"submit\" value=\"Add user to your Ignore List\">\n\t\t</form>\n\n\t</td>\n</tr>\n</table>\n\n<br>\n\n</div><!-- #content -->\n\t<ul class=\"navigation\">\n<li class=\"first\"><a href=\"/index.php\">SA Forums</a></li>\n<li>- <a href=\"http://www.somethingawful.com/\">Something Awful</a></li>\n<li>- <a href=\"/f/search\">Search the Forums</a></li>\n<li>- <a href=\"/usercp.php\">User Control Panel</a></li>\n<li>- <a href=\"/private.php\">Private Messages</a></li>\n<li>- <a href=\"http://www.somethingawful.com/d/forum-rules/forum-rules.php\">Forum Rules</a></li>\n<li>- <a href=\"/dictionary.php\">SAclopedia</a></li>\n<li>- <a href=\"/stats.php\">Posting Gloryhole</a></li>\n<li>- <a href=\"/banlist.php\">Leper's Colony</a></li>\n<li>- <a href=\"/supportmail.php\">Support</a></li>\n<li>- <a href=\"/account.php?action=logout&amp;ma=05fd1b5e\">Log Out</a></li>\n\n</ul>\n\t<div id=\"copyright\">\n\t\tPowered by: vBulletin Version 2.2.9 (<a href=\"/CHANGES\">SAVB-v2.1.17</a>)<br>\n\t\tCopyright &copy;2000, 2001, Jelsoft Enterprises Limited.<br>\n\t\tCopyright &copy;2012, Something Awful LLC<br>\n\t</div>\n\n</div><!-- #container -->\n\n<script type=\"text/javascript\">\n\t// quantcast\n\t_qoptions = { qacct:\"p-82vTvmw-enlng\" };\n\n\ttry {\n\t\tif(document.location.hostname != 'forums.somethingawful.com')\n\t\t\tthrow undefined;\n\n\t\t$(document).ready(function() {\n\t\t\tvar qcUrl = 'http://edge.quantserve.com/quant.js';\n\t\t\tjQuery.getScript(qcUrl);\n\n\t\t\tvar gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");\n\t\t\tvar gaUrl = gaJsHost + 'google-analytics.com/ga.js';\n\t\t\tjQuery.getScript(gaUrl, function() {\n\t\t\t\tvar pageTracker = _gat._getTracker(\"UA-3064978-2\");\n\t\t\t\tpageTracker._initData();\n\t\t\t\tpageTracker._trackPageview();\n\t\t\t});\n\t\t});\n\t} catch(e) {};\n\n\t// indie\n\t// try {\n\t// \tif(document.location.hostname != 'forums.somethingawful.com')\n\t// \t\tthrow undefined;\n\n\t// \t$(document).ready(function() {\n\t// \t\tvar ic_element = document.createElement('script');\n\t// \t\tic_element.type = 'text/javascript';\n\t// \t\tic_element.async = true;\n\t// \t\tic_element.id = 'ic_annonymous_pixel';\n\t// \t\tic_element.src = 'http://pixel.indieclick.com/annonymous/domain/somethingawful.com/reach/script_ic.js';\n\t// \t\tvar ic_script = document.getElementsByTagName('script')[0];\n\t// \t\tic_script.parentNode.insertBefore(ic_element, ic_script);\n\t// \t});\n\t// } catch(e) {};\n</script>\n<noscript><img src=\"http://pixel.quantserve.com/pixel/p-82vTvmw-enlng.gif\" style=\"display:none;\" border=\"0\" height=\"1\" width=\"1\" alt=\"Quantcast\"></noscript>\n\n<!-- PLEASE DO NOT REMOVE -->\n<ol id=\"debug\" style=\"display:none\"><li>&nbsp;</ol>\n\n</body>\n</html>";

        [TestMethod]
        public void Null_User_Throws_An_Exception()
        {
            Assert.ThrowsException<NullReferenceException>(() => ForumUserEntity.FromPost(null));
        }

        [TestMethod]
        public void Unexpected_Html_Throws_An_Exception()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml("<html><h1>oh god how did this get here i am not good at unit testing</h1></html>");
            Assert.ThrowsException<NullReferenceException>(() => ForumUserEntity.FromPost(doc.DocumentNode));
        }

        [TestMethod]
        public void Username_Is_Successfully_Parsed()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            const string expected = "elguignolgrande";
            Assert.AreEqual(expected, user.Username.Trim());
        }

        [TestMethod]
        public void AboutUser_Is_Successfully_Parsed()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            const string expected =
                @"There have been 18540 posts made by elguignolgrande, an average of 3.86 posts per day, since registering on Sep  2, 2000. elguignolgrande claims to be a male.

This moron has not provided any additional info.  The lack of a gender-specific pronoun here is in no way intended as sexism.";
            Assert.AreEqual(expected, user.AboutUser.Trim());
        }

        [TestMethod]
        public void DateUserJoined_Is_Successfully_Parsed()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            Assert.AreEqual(new DateTime(2000, 9, 2), user.DateJoined);
        }

        [TestMethod]
        public void PostCount_Is_Successfully_Parsed()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            Assert.AreEqual(18540, user.PostCount);
        }

        [TestMethod]
        public void PostRate_Is_Successfully_Parsed()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            Assert.AreEqual("3.86 per day", user.PostRate);
        }

        [TestMethod]
        public void LastPostDate_Is_Successfully_Parsed()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            Assert.AreEqual(new DateTime(2013, 11, 1, 10, 21, 00), user.LastPostDate);
        }

        [TestMethod]
        public void Location_Is_Parsed_When_Provided()
        {
            ForumUserEntity user = GetParsedEntity(UserProfileWithLocation);
            Assert.AreEqual("cardboard box", user.Location);
        }

        [TestMethod]
        public void Location_Is_Null_When_Not_Provided()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            Assert.IsNull(user.Location);
        }

        [TestMethod]
        public void SellerRating_Is_Parsed_When_Provided()
        {
            ForumUserEntity user = GetParsedEntity(DefaultUserProfileHtml);
            Assert.AreEqual("5, 5", user.SellerRating);
        }

        [TestMethod]
        public void SellerRating_Is_Null_When_Not_Provided()
        {
            ForumUserEntity user = GetParsedEntity(UserProfileWithLocation);
            Assert.IsNull(user.SellerRating);
        }

        private static ForumUserEntity GetParsedEntity(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            //TODO: Fix test to reflect removed user parsing logic.
            HtmlNode profileNode = doc.DocumentNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("info"));

            HtmlNode threadNode = doc.DocumentNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("thread"));

            return ForumUserEntity.FromUserProfile(profileNode, threadNode);
        }
    }
}