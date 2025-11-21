using GameCharacterVsaCqrs.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCharacterVsaCqrs.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<GameCharacter> GameCharacters { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CharacterItem> CharacterItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CharacterItem>()
                .HasOne(ci => ci.GameCharacter)
                .WithMany(c => c.CharacterItems)
                .HasForeignKey(ci => ci.GameCharacterId);

            modelBuilder.Entity<CharacterItem>()
                .HasOne(ci => ci.Item)
                .WithMany(i => i.CharacterItems)
                .HasForeignKey(ci => ci.ItemId);
        }
    }
}
