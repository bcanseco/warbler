using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Warbler.Areas.Chat.Data;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Migrations
{
    [DbContext(typeof(ChatContext))]
    [Migration("20170416015651_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<DateTime>("LastUsed");

                    b.Property<string>("Name");

                    b.Property<int>("ServerId");

                    b.Property<int>("State");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Channel");
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Membership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChannelId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("Membership");
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChannelId");

                    b.Property<DateTime>("SendDate");

                    b.Property<string>("Text");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool?>("IsAuthEnabled");

                    b.Property<int>("Type");

                    b.Property<int>("UniversityId");

                    b.HasKey("Id");

                    b.HasIndex("UniversityId")
                        .IsUnique();

                    b.ToTable("Server");
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.University", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("Lat");

                    b.Property<float>("Lng");

                    b.Property<string>("Name");

                    b.Property<int>("ServerId");

                    b.HasKey("Id");

                    b.ToTable("University");
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AvatarId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Channel", b =>
                {
                    b.HasOne("Warbler.Areas.Chat.Models.Server", "Server")
                        .WithMany("Channels")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Membership", b =>
                {
                    b.HasOne("Warbler.Areas.Chat.Models.Channel", "Channel")
                        .WithMany("Memberships")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Warbler.Areas.Chat.Models.User", "User")
                        .WithMany("Memberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Message", b =>
                {
                    b.HasOne("Warbler.Areas.Chat.Models.Channel", "Channel")
                        .WithMany("Messages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Warbler.Areas.Chat.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Warbler.Areas.Chat.Models.Server", b =>
                {
                    b.HasOne("Warbler.Areas.Chat.Models.University", "University")
                        .WithOne("Server")
                        .HasForeignKey("Warbler.Areas.Chat.Models.Server", "UniversityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
