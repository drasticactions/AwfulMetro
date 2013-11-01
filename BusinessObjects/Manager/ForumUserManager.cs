using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.Core.Manager
{
    public class ForumUserManager
    {
        private readonly IWebManager webManager;
        public ForumUserManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public ForumUserManager() : this(new WebManager()) { }

        public async Task<ForumUserEntity> GetUserFromProfilePage(ForumUserEntity user, long userId)
        {
            string url = Constants.BASE_URL + string.Format(Constants.USER_PROFILE, userId);
            //inject this
            var doc = (await webManager.DownloadHtml(url)).Document;
            
            if(string.IsNullOrEmpty(user.Username))
            {
                user.ParseFromPost(doc.DocumentNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("id", "").Contains("thread")));
            }
            user.ParseFromUserProfile(doc.DocumentNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("info")));
            return user;
        }
    }
}
