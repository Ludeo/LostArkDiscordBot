using Microsoft.EntityFrameworkCore;

namespace LostArkBot.databasemodels
{
    public partial class LostArkBotContext : DbContext
    {
        public LostArkBotContext()
        {
        }

        public LostArkBotContext(DbContextOptions<LostArkBotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Character> Characters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=192.168.178.48;database=lostarkbot;user=LudeoPC;password=root", ServerVersion.Parse("10.3.29-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Character>(entity =>
            {
                entity.ToTable("characters");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CharacterName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("character-name");

                entity.Property(e => e.ClassName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("class-name");

                entity.Property(e => e.Crit)
                    .HasColumnType("int(11)")
                    .HasColumnName("crit");

                entity.Property(e => e.CustomProfileMessage)
                    .HasColumnType("text")
                    .HasColumnName("custom-profile-message");

                entity.Property(e => e.DiscordUserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("discord-user-id");

                entity.Property(e => e.Dom)
                    .HasColumnType("int(11)")
                    .HasColumnName("dom");

                entity.Property(e => e.End)
                    .HasColumnType("int(11)")
                    .HasColumnName("end");

                entity.Property(e => e.Engravings)
                    .HasColumnType("text")
                    .HasColumnName("engravings");

                entity.Property(e => e.Exp)
                    .HasColumnType("int(11)")
                    .HasColumnName("exp");

                entity.Property(e => e.ItemLevel)
                    .HasColumnType("int(11)")
                    .HasColumnName("item-level");

                entity.Property(e => e.ProfilePicture)
                    .HasColumnType("text")
                    .HasColumnName("profile-picture");

                entity.Property(e => e.Spec)
                    .HasColumnType("int(11)")
                    .HasColumnName("spec");

                entity.Property(e => e.Swift)
                    .HasColumnType("int(11)")
                    .HasColumnName("swift");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}