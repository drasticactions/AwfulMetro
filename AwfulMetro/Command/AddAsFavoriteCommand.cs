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
        public async override void Execute(object parameter)
        {
            var forumEntity = (ForumEntity) parameter;
            if (Locator.ViewModels.MainForumsPageVm.FavoriteForumGroupList.First().ForumList.Any(forum => forum.ForumId == forumEntity.ForumId))
            {
                return;
            }
            using (var db = new FavoriteForumContext())
            {
                db.Add(forumEntity);
                await db.SaveChangesAsync();
                Locator.ViewModels.MainForumsPageVm.GetFavoriteForums();
            }
            
        }
    }
}
