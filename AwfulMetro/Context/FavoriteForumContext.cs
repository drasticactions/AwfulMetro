using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace AwfulMetro.Context
{
    public class FavoriteForumContext : DbContext
    {
        public DbSet<ForumEntity> Forums { get; set; }

        protected override void OnConfiguring(DbContextOptions builder)
        {
            var dir = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            var connection = "Filename=" + System.IO.Path.Combine(dir, "FavoriteForum.db");

            builder.UseSQLite(connection);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ForumEntity>().Key(b => b.ForumId);
            
            // The EF7 SQLite provider currently doesn't support generated values
            // so setting the keys to be generated from developer code
            builder.Entity<ForumEntity>()
                .Property(b => b.ForumId)
                .GenerateValuesOnAdd(false);
        }
    }
}
