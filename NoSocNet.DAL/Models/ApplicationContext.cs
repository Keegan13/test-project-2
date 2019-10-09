using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoSocNet.DAL.Models;

namespace NoSocNet.DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public virtual DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public virtual DbSet<MessageEntity> Messages { get; set; }
        public virtual DbSet<UsersChatRoomsEntity> UsersRooms { get; set; }

        public virtual DbSet<MessageReadByUserEntity> MessagesReadBy { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureMessages(builder.Entity<MessageEntity>());
            ConfigureChatRooms(builder.Entity<ChatRoomEntity>());
            ConfigureUserRooms(builder.Entity<UsersChatRoomsEntity>());
            ConfigureMessageRead(builder.Entity<MessageReadByUserEntity>());
        }

        protected virtual void ConfigureUserRooms(EntityTypeBuilder<UsersChatRoomsEntity> entityTypeBuilder)
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

        protected virtual void ConfigureMessageRead(EntityTypeBuilder<MessageReadByUserEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => new { x.UserId, x.MessageId });

            entityTypeBuilder
                .HasOne(mu => mu.Message)
                .WithMany(m => m.ReadByUsers)
                .HasForeignKey(mu => mu.MessageId);

            entityTypeBuilder
                .HasOne(mu => mu.User)
                .WithMany()
                .HasForeignKey(mu => mu.UserId);

            entityTypeBuilder
                .Property(x => x.DateRead)
                .HasDefaultValueSql("getdate()");
        }

        protected virtual void ConfigureMessages(EntityTypeBuilder<MessageEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasOne(x => x.SenderUser).WithMany(x => x.Messages).HasForeignKey(x => x.SenderUserId);
            entityTypeBuilder.HasIndex(x => x.SenderUserId);

        }

        protected virtual void ConfigureChatRooms(EntityTypeBuilder<ChatRoomEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasMany(x => x.Messages).WithOne(x => x.ChatRoom).HasForeignKey(x => x.ChatRoomId);
            entityTypeBuilder.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId);
        }
    }
}
