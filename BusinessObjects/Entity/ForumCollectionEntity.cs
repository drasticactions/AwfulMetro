using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class ForumCollectionEntity
    {
        public List<ForumThreadEntity> ForumThreadList { get; private set; }

        public List<ForumEntity> ForumSubcategoryList { get; private set; }

        public String ForumName { get; private set; }

        public List<String> ForumType { get; private set; }

        public int CurrentPage { get; private set; }
        public ForumCollectionEntity(String forumName, List<ForumThreadEntity> forumThreadList, List<ForumEntity> forumSubcategoryList)
        {
            this.ForumThreadList = forumThreadList;
            this.ForumSubcategoryList = forumSubcategoryList;
            this.ForumType = new List<string>();
            this.ForumType.Add("Subforums");
            this.ForumType.Add("Threads");
            this.ForumName = forumName;
            this.CurrentPage = 1;
        }
    }
}
