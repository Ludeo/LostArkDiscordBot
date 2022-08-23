using System.Collections.Generic;
using LostArkBot.Src.Bot.FileObjects;
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

        public virtual DbSet<ActiveMerchant> ActiveMerchants { get; set; }
        public virtual DbSet<ChallengeAbyss> ChallengeAbysses { get; set; }
        public virtual DbSet<ChallengeGuardian> ChallengeGuardians { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<Engraving> Engravings { get; set; }
        public virtual DbSet<MerchantItem> MerchantItems { get; set; }
        public virtual DbSet<StaticGroup> StaticGroups { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Config config = Config.Default;
                optionsBuilder.UseMySql($"server={config.DbServer};database={config.DbName};user={config.DbUser};password={config.DbPassword}", ServerVersion.Parse("10.3.29-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<ActiveMerchant>(entity =>
            {
                entity.ToTable("ActiveMerchant");

                entity.HasIndex(e => e.CardId, "activemerchantfk1_idx");

                entity.HasIndex(e => e.RapportId, "activemerchantfk2_idx");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .HasColumnName("id");

                entity.Property(e => e.CardId)
                    .HasColumnType("int(11)")
                    .HasColumnName("cardId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.RapportId)
                    .HasColumnType("int(11)")
                    .HasColumnName("rapportId");

                entity.Property(e => e.Votes)
                    .HasColumnType("int(11)")
                    .HasColumnName("votes");

                entity.Property(e => e.Zone)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("zone");

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.ActiveMerchantCards)
                    .HasForeignKey(d => d.CardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activemerchantfk1");

                entity.HasOne(d => d.Rapport)
                    .WithMany(p => p.ActiveMerchantRapports)
                    .HasForeignKey(d => d.RapportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activemerchantfk2");
            });

            modelBuilder.Entity<ChallengeAbyss>(entity =>
            {
                entity.ToTable("ChallengeAbyss");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(45)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ChallengeGuardian>(entity =>
            {
                entity.ToTable("ChallengeGuardian");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("name");

                entity.Property(e => e.WeekNumber)
                    .HasColumnType("int(11)")
                    .HasColumnName("weekNumber");
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.ToTable("Character");

                entity.HasIndex(e => e.UserId, "characterfk1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CharacterName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("characterName");

                entity.Property(e => e.ClassName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("className");

                entity.Property(e => e.Crit)
                    .HasColumnType("int(11)")
                    .HasColumnName("crit");

                entity.Property(e => e.CustomProfileMessage)
                    .HasColumnType("text")
                    .HasColumnName("customProfileMessage");

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
                    .HasColumnName("itemLevel");

                entity.Property(e => e.ProfilePicture)
                    .HasColumnType("text")
                    .HasColumnName("profilePicture");

                entity.Property(e => e.Spec)
                    .HasColumnType("int(11)")
                    .HasColumnName("spec");

                entity.Property(e => e.Swift)
                    .HasColumnType("int(11)")
                    .HasColumnName("swift");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("characterfk1");
            });

            modelBuilder.Entity<Engraving>(entity =>
            {
                entity.ToTable("Engraving");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Class)
                    .HasMaxLength(50)
                    .HasColumnName("class");

                entity.Property(e => e.Desc0)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("desc0");

                entity.Property(e => e.Desc1)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("desc1");

                entity.Property(e => e.Desc2)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("desc2");

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("icon");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Penalty).HasColumnName("penalty");
            });

            modelBuilder.Entity<MerchantItem>(entity =>
            {
                entity.ToTable("MerchantItem");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Rarity)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("rarity");
            });

            modelBuilder.Entity<StaticGroup>(entity =>
            {
                entity.ToTable("StaticGroup");

                entity.HasIndex(e => e.LeaderId, "staticgroupfk1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.LeaderId)
                    .HasColumnType("int(11)")
                    .HasColumnName("leaderId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("name");

                entity.HasOne(d => d.Leader)
                    .WithMany(p => p.StaticGroups)
                    .HasForeignKey(d => d.LeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staticgroupfk1");

                entity.HasMany(d => d.Characters)
                    .WithMany(p => p.StaticGroups)
                    .UsingEntity<Dictionary<string, object>>(
                        "StaticGroupCharacter",
                        l => l.HasOne<Character>().WithMany().HasForeignKey("CharacterId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("staticgroupusersfk2"),
                        r => r.HasOne<StaticGroup>().WithMany().HasForeignKey("StaticGroupId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("staticgroupusersfk1"),
                        j =>
                        {
                            j.HasKey("StaticGroupId", "CharacterId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("StaticGroupCharacters");

                            j.HasIndex(new[] { "CharacterId" }, "staticgroupusersfk2_idx");

                            j.IndexerProperty<int>("StaticGroupId").HasColumnType("int(11)").HasColumnName("staticGroupId");

                            j.IndexerProperty<int>("CharacterId").HasColumnType("int(11)").HasColumnName("characterId");
                        });
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ItemId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("Subscription");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("userId");

                entity.Property(e => e.ItemId)
                    .HasColumnType("int(11)")
                    .HasColumnName("itemId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subscriptionfk1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DiscordUserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("discordUserId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
