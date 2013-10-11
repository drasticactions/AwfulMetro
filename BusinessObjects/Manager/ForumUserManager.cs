using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
namespace BusinessObjects.Manager
{
    public class ForumUserManager
    {
        public static async Task<ForumUserEntity> GetUserFromProfilePage(ForumUserEntity user, long userId)
        {
            String url = Constants.BASE_URL + string.Format(Constants.USER_PROFILE, userId);
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            if(string.IsNullOrEmpty(user.Username))
            {
                user.ParseFromPost(doc.DocumentNode.Descendants("td").Where(node => node.GetAttributeValue("id", "").Contains("thread")).FirstOrDefault());
            }
            user.ParseFromUserProfile(doc.DocumentNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Contains("info")).FirstOrDefault());
            return user;
        }
    }
}
