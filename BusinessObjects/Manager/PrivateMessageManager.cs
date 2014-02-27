using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class PrivateMessageManager
    {
          private readonly IWebManager _webManager;

        public PrivateMessageManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public PrivateMessageManager()
            : this(new WebManager())
        {
        }

        public async Task<List<PrivateMessageEntity>> GetPrivateMessages(int page)
        {
            var privateMessageEntities = new List<PrivateMessageEntity>();
            var url = Constants.PRIVATE_MESSAGES;
            if (page > 0)
            {
                url = Constants.PRIVATE_MESSAGES + string.Format(Constants.PAGE_NUMBER, page);
            }

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

            HtmlNode forumNode =
                doc.DocumentNode.Descendants("tbody").FirstOrDefault();


            foreach (
                HtmlNode threadNode in
                    forumNode.Descendants("tr"))
            {
                var threadEntity = new PrivateMessageEntity();
                threadEntity.Parse(threadNode);
                privateMessageEntities.Add(threadEntity);
            }
            return privateMessageEntities;
        }

    }
}
