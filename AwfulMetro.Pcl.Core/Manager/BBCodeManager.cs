using System.Collections.Generic;
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Core.Manager
{
    public static class BBCodeManager
    {
        private static IEnumerable<BBCodeCategoryEntity> bbCodes;

        public static IEnumerable<BBCodeCategoryEntity> BBCodes
        {
            get { return bbCodes ?? (bbCodes = GetBBCodes()); }
        }

        private static IEnumerable<BBCodeCategoryEntity> GetBBCodes()
        {
            var bbCodeCategoryList = new List<BBCodeCategoryEntity>();
            var bbCodeList = new List<BBCodeEntity>
            {
                new BBCodeEntity("url", "url"),
                new BBCodeEntity("email", "email"),
                new BBCodeEntity("img", "img"),
                new BBCodeEntity("timg", "timg"),
                new BBCodeEntity("video", "video"),
                new BBCodeEntity("b", "b"),
                new BBCodeEntity("s", "s"),
                new BBCodeEntity("u", "u"),
                new BBCodeEntity("i", "i"),
                new BBCodeEntity("spoiler", "spoiler"),
                new BBCodeEntity("fixed", "fixed"),
                new BBCodeEntity("super", "super"),
                new BBCodeEntity("sub", "sub"),
                new BBCodeEntity("size", "size"),
                new BBCodeEntity("color", "color"),
                new BBCodeEntity("quote", "quote"),
                new BBCodeEntity("url", "url"),
                new BBCodeEntity("pre", "pre"),
                new BBCodeEntity("code", "code"),
                new BBCodeEntity("php", "php"),
                new BBCodeEntity("list", "list")
            };
            bbCodeCategoryList.Add(new BBCodeCategoryEntity("BBCode", bbCodeList));
            return bbCodeCategoryList;
        }
    }
}