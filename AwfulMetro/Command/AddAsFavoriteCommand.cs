using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Context;
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Command
{
    public class AddAsFavoriteCommand : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            var forumCategoryEntity = (ForumCategoryEntity) parameter;
            using (var db = new FavoriteForumContext())
            {
                db.Add(forumCategoryEntity);
            }
        }
    }
}
