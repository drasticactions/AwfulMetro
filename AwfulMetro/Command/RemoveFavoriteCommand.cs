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
    public class RemoveFavoriteCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var forumEntity = (ForumEntity)parameter;
            using (var db = new FavoriteForumContext())
            {
                db.Delete(forumEntity);
                await db.SaveChangesAsync();
                Locator.ViewModels.MainForumsPageVm.GetFavoriteForums();
            }

        }
    }
}
