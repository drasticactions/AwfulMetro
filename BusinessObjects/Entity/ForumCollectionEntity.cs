using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class ForumCollectionEntity
    {
        public IEnumerable<ForumThreadEntity> ForumThreadList { get; private set; }

        public IEnumerable<ForumEntity> ForumSubcategoryList { get; private set; }

        public string ForumName { get; private set; }

        public IEnumerable<string> ForumType { get; private set; }

        public int CurrentPage { get; private set; }

        public ForumCollectionEntity(string forumName, IEnumerable<ForumThreadEntity> forumThreadList, IEnumerable<ForumEntity> forumSubcategoryList)
        {
            this.ForumThreadList = forumThreadList;
            this.ForumSubcategoryList = forumSubcategoryList;
            this.ForumType = new List<string> { "Subforums", "Threads" };
            this.ForumName = forumName;
            this.CurrentPage = 1;
        }
    }
}
