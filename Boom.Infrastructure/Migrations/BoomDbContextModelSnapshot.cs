﻿// <auto-generated />
using System;
using Boom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Boom.Infrastructure.Migrations
{
    [DbContext(typeof(BoomDbContext))]
    partial class BoomDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Article", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Link")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("link");

                    b.Property<string>("LinkTitle")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("link_title");

                    b.Property<string>("Message")
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("message");

                    b.Property<bool>("Popup")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("popup");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("articles");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Ghost", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("longblob")
                        .HasColumnName("data");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("ghosts");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Level", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("BgId")
                        .HasColumnType("bigint")
                        .HasColumnName("bg_id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<bool>("Custom")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("custom");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("varchar(60)")
                        .HasColumnName("display_name");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("file_path");

                    b.Property<string>("LevelId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("level_id");

                    b.Property<bool>("Online")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("online");

                    b.Property<long>("ThemeId")
                        .HasColumnType("bigint")
                        .HasColumnName("theme_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<short>("Version")
                        .HasColumnType("smallint")
                        .HasColumnName("version");

                    b.HasKey("Id");

                    b.HasIndex("BgId");

                    b.HasIndex("ThemeId");

                    b.ToTable("level");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.LevelTarget", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<long>("LevelId")
                        .HasColumnType("bigint")
                        .HasColumnName("level_id");

                    b.Property<int>("Order")
                        .HasColumnType("int")
                        .HasColumnName("order");

                    b.Property<int?>("TargetAmount")
                        .HasColumnType("int")
                        .HasColumnName("target_amount");

                    b.Property<long>("TargetId")
                        .HasColumnType("bigint")
                        .HasColumnName("target_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("LevelId");

                    b.HasIndex("TargetId");

                    b.ToTable("level_target");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Player", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Badge")
                        .HasColumnType("int")
                        .HasColumnName("badge");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("varchar(3)")
                        .HasColumnName("country_code");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Device")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("device");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("email");

                    b.Property<string>("EngineStyle")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("engine_style");

                    b.Property<long?>("FacebookId")
                        .HasColumnType("bigint")
                        .HasColumnName("facebook_id");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("fullname");

                    b.Property<string>("HeroStyle")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("hero_style");

                    b.Property<string>("Ios")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("ios");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("last_login_at");

                    b.Property<string>("MaxGroupIdUnlocked")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("max_group_id_unlocked");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("nickname");

                    b.Property<string>("Notification")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("notification");

                    b.Property<int?>("Rev")
                        .HasColumnType("int")
                        .HasColumnName("rev");

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("secret_key");

                    b.Property<string>("Timezone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("timezone");

                    b.Property<int>("TimezoneSecondsOffset")
                        .HasColumnType("int")
                        .HasColumnName("timezone_seconds_offset");

                    b.Property<string>("TinyUrl")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("tiny_url");

                    b.Property<double>("TotalDistance")
                        .HasColumnType("double")
                        .HasColumnName("total_distance");

                    b.Property<int>("TotalEarnedMedals")
                        .HasColumnType("int")
                        .HasColumnName("total_earned_medals");

                    b.Property<int>("TotalEarnedSuperstars")
                        .HasColumnType("int")
                        .HasColumnName("total_earned_superstars");

                    b.Property<int?>("TotalHiddenPilesFound")
                        .HasColumnType("int")
                        .HasColumnName("total_hidden_piles_found");

                    b.Property<int>("TournamentsAggregatedRank")
                        .HasColumnType("int")
                        .HasColumnName("tournaments_aggregated_rank");

                    b.Property<long?>("TwitterId")
                        .HasColumnType("bigint")
                        .HasColumnName("twitter_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("Uuid")
                        .HasMaxLength(36)
                        .HasColumnType("char(36)")
                        .HasColumnName("uuid");

                    b.Property<int>("VsPlayed")
                        .HasColumnType("int")
                        .HasColumnName("vs_played");

                    b.Property<int>("VsWon")
                        .HasColumnType("int")
                        .HasColumnName("vs_won");

                    b.Property<int>("WcPlayed")
                        .HasColumnType("int")
                        .HasColumnName("wc_played");

                    b.Property<int>("WcWon")
                        .HasColumnType("int")
                        .HasColumnName("wc_won");

                    b.Property<string>("WheelStyle")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("wheel_style");

                    b.Property<int?>("WorldRank")
                        .HasColumnType("int")
                        .HasColumnName("world_rank");

                    b.HasKey("Id");

                    b.ToTable("players");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Standing", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("EngineStyle")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("engine_style");

                    b.Property<long>("GhostId")
                        .HasColumnType("bigint")
                        .HasColumnName("ghost_id");

                    b.Property<string>("HeroStyle")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("hero_style");

                    b.Property<int>("Time")
                        .HasColumnType("int")
                        .HasColumnName("time");

                    b.Property<long>("TournamentId")
                        .HasColumnType("bigint")
                        .HasColumnName("tournament_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<string>("WheelStyle")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("wheel_style");

                    b.HasKey("Id");

                    b.HasIndex("GhostId");

                    b.HasIndex("TournamentId");

                    b.HasIndex("UserId");

                    b.ToTable("standings");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Target", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("type");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("targets");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Theme", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("BgName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("bg_name");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("name");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("themes");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Tournament", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Cheaters")
                        .HasColumnType("int")
                        .HasColumnName("cheaters");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<int>("EloGroup")
                        .HasColumnType("int")
                        .HasColumnName("elo_group");

                    b.Property<long>("TournamentGroupId")
                        .HasColumnType("bigint")
                        .HasColumnName("tournament_group_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("Uuid")
                        .HasMaxLength(36)
                        .HasColumnType("char(36)")
                        .HasColumnName("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TournamentGroupId");

                    b.ToTable("tournaments");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.TournamentGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("EndsAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("ends_at");

                    b.Property<long>("LevelTargetId")
                        .HasColumnType("bigint")
                        .HasColumnName("level_target_id");

                    b.Property<bool>("NoSuper")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("no_super");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("starts_at");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("Uuid")
                        .HasMaxLength(36)
                        .HasColumnType("char(36)")
                        .HasColumnName("uuid");

                    b.HasKey("Id");

                    b.HasIndex("LevelTargetId");

                    b.ToTable("tournament_groups");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(191)
                        .HasColumnType("varchar(191)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("EmailVerifiedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("email_verified_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(191)
                        .HasColumnType("varchar(191)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(191)
                        .HasColumnType("varchar(191)")
                        .HasColumnName("password");

                    b.Property<string>("RememberToken")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("remember_token");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Level", b =>
                {
                    b.HasOne("Boom.Infrastructure.Data.Entities.Theme", "Background")
                        .WithMany()
                        .HasForeignKey("BgId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Boom.Infrastructure.Data.Entities.Theme", "Theme")
                        .WithMany()
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Background");

                    b.Navigation("Theme");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.LevelTarget", b =>
                {
                    b.HasOne("Boom.Infrastructure.Data.Entities.Level", "Level")
                        .WithMany()
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Boom.Infrastructure.Data.Entities.Target", "Target")
                        .WithMany()
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Level");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Standing", b =>
                {
                    b.HasOne("Boom.Infrastructure.Data.Entities.Ghost", "Ghost")
                        .WithMany()
                        .HasForeignKey("GhostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Boom.Infrastructure.Data.Entities.Tournament", "Tournament")
                        .WithMany()
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Boom.Infrastructure.Data.Entities.Player", "Player")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ghost");

                    b.Navigation("Player");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.Tournament", b =>
                {
                    b.HasOne("Boom.Infrastructure.Data.Entities.TournamentGroup", "TournamentGroup")
                        .WithMany()
                        .HasForeignKey("TournamentGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TournamentGroup");
                });

            modelBuilder.Entity("Boom.Infrastructure.Data.Entities.TournamentGroup", b =>
                {
                    b.HasOne("Boom.Infrastructure.Data.Entities.LevelTarget", "LevelTarget")
                        .WithMany()
                        .HasForeignKey("LevelTargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LevelTarget");
                });
#pragma warning restore 612, 618
        }
    }
}
