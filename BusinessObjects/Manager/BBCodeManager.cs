using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Entity;

namespace BusinessObjects.Manager
{
    public class BBCodeManager
    {
        public static List<BBCodeCategoryEntity> GetBBCodes()
        {
            List<BBCodeCategoryEntity> bbCodeCategoryList = new List<BBCodeCategoryEntity>();
            List<BBCodeEntity> bbCodeList = new List<BBCodeEntity>();
            bbCodeList.Add(new BBCodeEntity("url", "url"));
            bbCodeList.Add(new BBCodeEntity("email", "email"));
            bbCodeList.Add(new BBCodeEntity("img", "img"));
            bbCodeList.Add(new BBCodeEntity("timg", "timg"));
            bbCodeList.Add(new BBCodeEntity("video", "video"));
            bbCodeList.Add(new BBCodeEntity("b", "b"));
            bbCodeList.Add(new BBCodeEntity("s", "s"));
            bbCodeList.Add(new BBCodeEntity("u", "u"));
            bbCodeList.Add(new BBCodeEntity("i", "i"));
            bbCodeList.Add(new BBCodeEntity("spoiler", "spoiler"));
            bbCodeList.Add(new BBCodeEntity("fixed", "fixed"));
            bbCodeList.Add(new BBCodeEntity("super", "super"));
            bbCodeList.Add(new BBCodeEntity("sub", "sub"));
            bbCodeList.Add(new BBCodeEntity("size", "size"));
            bbCodeList.Add(new BBCodeEntity("color", "color"));
            bbCodeList.Add(new BBCodeEntity("quote", "quote"));
            bbCodeList.Add(new BBCodeEntity("url", "url"));
            bbCodeList.Add(new BBCodeEntity("pre", "pre"));
            bbCodeList.Add(new BBCodeEntity("code", "code"));
            bbCodeList.Add(new BBCodeEntity("php", "php"));
            bbCodeList.Add(new BBCodeEntity("list", "list"));
            bbCodeCategoryList.Add(new BBCodeCategoryEntity("BBCode", bbCodeList));
            return bbCodeCategoryList;
        }

    }
}
