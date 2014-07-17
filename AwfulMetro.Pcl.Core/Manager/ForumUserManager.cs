#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class ForumUserManager
    {
        private readonly IWebManager _webManager;

        public ForumUserManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public ForumUserManager() : this(new WebManager())
        {
        }

        public async Task<ForumUserEntity> GetUserFromProfilePage(long userId)
        {
            string url = Constants.BASE_URL + string.Format(Constants.USER_PROFILE, userId);
            HtmlDocument doc = (await _webManager.GetData(url)).Document;

            /*Get the user profile HTML from the user profile page,
             once we get it, get the nodes for each section of the page and parse them.*/

            HtmlNode profileNode = doc.DocumentNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("info"));

            HtmlNode threadNode = doc.DocumentNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("thread"));
            ForumUserEntity userEntity = ForumUserEntity.FromUserProfile(profileNode, threadNode);

            userEntity.FromUserProfileAvatarInformation(threadNode);

            return userEntity;
        }
    }
}