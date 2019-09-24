﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoSocNet.DAL.Models;

namespace NoSocNet.DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public virtual DbSet<ChatRoomDto> ChatRooms { get; set; }
        public virtual DbSet<MessageDto> Messages { get; set; }
        public virtual DbSet<UsersChatRoomsDto> UsersRooms { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureMessages(builder.Entity<MessageDto>());
            ConfigureChatRooms(builder.Entity<ChatRoomDto>());
            ConfigureUserRooms(builder.Entity<UsersChatRoomsDto>());
        }

        protected virtual void ConfigureUserRooms(EntityTypeBuilder<UsersChatRoomsDto> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => new { x.UserId, x.ChatRoomId });
            entityTypeBuilder
                .HasOne(ur => ur.ChatRoom)
                .WithMany(cr => cr.UserRooms)
                .HasForeignKey(ur => ur.ChatRoomId);

            entityTypeBuilder
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRooms)
                .HasForeignKey(ur => ur.UserId);
        }

        protected virtual void ConfigureMessages(EntityTypeBuilder<MessageDto> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasOne(x => x.Sender).WithMany(x => x.Messages).HasForeignKey(x => x.SenderId);
            entityTypeBuilder.HasIndex(x => x.SenderId);

        }

        protected virtual void ConfigureChatRooms(EntityTypeBuilder<ChatRoomDto> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasMany(x => x.Messages).WithOne(x => x.ChatRoom).HasForeignKey(x => x.ChatRoomId);
            entityTypeBuilder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId);
        }
    }
}
