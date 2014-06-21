using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;

namespace AwfulMetro.Pcl.Core.Manager
{
    public class SearchManager
    {
        private readonly IWebManager _webManager;

        public SearchManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public SearchManager()
            : this(new WebManager())
        {
        }

        public async Task<List<ForumUserSearchEntity>> GetUsernames(string username)
        {
            // Gets the fake 'JSON' html
            WebManager.Result result = await _webManager.GetData(string.Format(Constants.SEARCH_URL, username));

            // TODO: if result fails, alert the user!
            string fakeJsonHtml = result.Document.DocumentNode.OuterHtml;
            // Parses the fake json into an array of string. We will parse this further into search user objects.
            string[] userList = fakeJsonHtml.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.None);
            // Now parse the user list into the username and ID.
            var userAndIdList = userList.Select(node => node.Split(new string[] { "<>" }, StringSplitOptions.None));

            // Convert it into a list of ForumUserSearch entities. 
            // TODO: Handle errors if we don't get the expected list.
            return (from user in userAndIdList where user.Length == 2 && !string.IsNullOrEmpty(user[0]) && !string.IsNullOrEmpty(user[1]) select new ForumUserSearchEntity(user[0], Convert.ToInt64(user[1]))).ToList();
        }
    }
}
