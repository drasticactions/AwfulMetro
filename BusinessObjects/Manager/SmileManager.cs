using BusinessObjects.Entity;
using BusinessObjects.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class SmileManager
    {
        private readonly IWebManager webManager;
        public SmileManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public SmileManager() : this(new WebManager()) { }

        public async Task<List<SmileCategoryEntity>> GetSmileList()
        {
            List<SmileCategoryEntity> smileCategoryList = new List<SmileCategoryEntity>();

            //inject this
            var doc = (await webManager.DownloadHtml(Constants.SMILE_URL)).Document;
           
            var smileCategoryTitles = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("inner")).Descendants("h3");
            List<string> categoryTitles = smileCategoryTitles.Select(smileCategoryTitle => WebUtility.HtmlDecode(smileCategoryTitle.InnerText)).ToList();
            var smileNodes = doc.DocumentNode.Descendants("ul").Where(node => node.GetAttributeValue("class", "").Contains("smilie_group"));
            int smileCount = 0;
            foreach(var smileNode in smileNodes)
            {
                List<SmileEntity> smileList = new List<SmileEntity>();
                var smileIcons = smileNode.Descendants("li");
                foreach(var smileIcon in smileIcons)
                {
                    SmileEntity smileEntity = new SmileEntity();
                    smileEntity.Parse(smileIcon);
                    smileList.Add(smileEntity);
                }
                smileCategoryList.Add(new SmileCategoryEntity(categoryTitles[smileCount], smileList));
                smileCount++;
            }
            return smileCategoryList;
        }
    }
}
