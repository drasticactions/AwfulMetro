using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class SmileManager
    {
        public static async Task<List<SmileCategoryEntity>> GetSmileList()
        {
            List<SmileCategoryEntity> smileCategoryList = new List<SmileCategoryEntity>();

            HttpWebRequest request = await AuthManager.CreateGetRequest(Constants.SMILE_URL);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            var smileCategoryTitles = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("inner")).FirstOrDefault().Descendants("h3");
            List<string> categoryTitles = new List<string>();
            foreach(var smileCategoryTitle in smileCategoryTitles)
            {
                categoryTitles.Add(WebUtility.HtmlDecode(smileCategoryTitle.InnerText));
            }
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
