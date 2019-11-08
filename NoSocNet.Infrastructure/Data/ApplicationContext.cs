using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Models;

namespace NoSocNet.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserEntity>, IUnitOfWork
    {
        #region Chat DbSets

        public virtual DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public virtual DbSet<MessageEntity> Messages { get; set; }
        public virtual DbSet<UsersChatRoomsEntity> UsersRooms { get; set; }
        public virtual DbSet<MessageReadByUserEntity> MessagesReadBy { get; set; }

        #endregion

        #region Survey DbSets

        public virtual DbSet<SurveyEntity> Surveys { get; set; }
        public virtual DbSet<QuestionEntity> Questions { get; set; }
        public virtual DbSet<OptionEntity> QuestionOptions { get; set; }
        public virtual DbSet<SurveyInstanceEntity> SurveyInstances { get; set; }
        public virtual DbSet<SurveyUserResultEntity> SurveyUserResults { get; set; }
        public virtual DbSet<QuestionResultEntity> QuestionResults { get; set; }
        public virtual DbSet<SelectedOptionEntity> SelectedOptions { get; set; }

        #endregion


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Chat

            ConfigureMessages(builder.Entity<MessageEntity>());
            ConfigureChatRooms(builder.Entity<ChatRoomEntity>());
            ConfigureUserRooms(builder.Entity<UsersChatRoomsEntity>());
            ConfigureMessageRead(builder.Entity<MessageReadByUserEntity>());
            #endregion

            #region Survey

            ConfigurreSurveyEntity(builder.Entity<SurveyEntity>());
            ConfigurreQuestionEntity(builder.Entity<QuestionEntity>());
            ConfigurreOptionEntity(builder.Entity<OptionEntity>());
            ConfigurreSurveyInstanceEntity(builder.Entity<SurveyInstanceEntity>());
            ConfigurreSurveyUserResultEntity(builder.Entity<SurveyUserResultEntity>());
            ConfigurreQuestionResultEntity(builder.Entity<QuestionResultEntity>());
            ConfigurreSelectedOptionEntity(builder.Entity<SelectedOptionEntity>());

            #endregion

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "user", NormalizedName = "USER" }, new IdentityRole { Name = "admin", NormalizedName = "ADMIN" });
        }

        #region Chat

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
            entityTypeBuilder.HasIndex(x => x.SendDate);
        }

        protected virtual void ConfigureChatRooms(EntityTypeBuilder<ChatRoomEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasMany(x => x.Messages).WithOne(x => x.ChatRoom).HasForeignKey(x => x.ChatRoomId);
            entityTypeBuilder.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId);
        }

        #endregion

        #region Survey

        protected virtual void ConfigurreSelectedOptionEntity(EntityTypeBuilder<SelectedOptionEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => new { x.OptionId, x.QuestionResultId });

            entityTypeBuilder
                .HasOne(x => x.Option)
                .WithMany()
                .HasForeignKey(x => x.OptionId);

            entityTypeBuilder
                .HasOne(x => x.QuestionResult)
                .WithMany(x => x.SelectedOptions)
                .HasForeignKey(x => x.QuestionResultId);
        }

        protected virtual void ConfigurreQuestionResultEntity(EntityTypeBuilder<QuestionResultEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);

            entityTypeBuilder.HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId);
            entityTypeBuilder.HasOne(x => x.SurveyUserResult).WithMany().HasForeignKey(x => x.QuestionId);
            entityTypeBuilder.HasMany(x => x.SelectedOptions).WithOne(x => x.QuestionResult).HasForeignKey(x => x.QuestionResultId);

            entityTypeBuilder.Property(x => x.CreatedDate).HasDefaultValueSql("getdate()");
        }

        protected virtual void ConfigurreSurveyUserResultEntity(EntityTypeBuilder<SurveyUserResultEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);

            entityTypeBuilder.HasOne(x => x.SurveyInstance).WithMany(x => x.UserResults).HasForeignKey(x => x.SurveyInstanceId);
            entityTypeBuilder.HasOne(x => x.SurveyeeUser).WithMany().HasForeignKey(x => x.SurveyeeUserId);

            entityTypeBuilder.HasMany(x => x.QuestionResults).WithOne(x => x.SurveyUserResult);
        }

        protected virtual void ConfigurreSurveyInstanceEntity(EntityTypeBuilder<SurveyInstanceEntity> entityTypeBuilder)
        {
            entityTypeBuilder.Property(x => x.Id);

            entityTypeBuilder.HasOne(x => x.InitiatorUser).WithMany().HasForeignKey(x => x.InitiatorUserId);
            entityTypeBuilder.HasOne(x => x.Survey).WithMany(x => x.Instances).HasForeignKey(x => x.SurveyId);
            entityTypeBuilder.HasMany(x => x.UserResults).WithOne(x => x.SurveyInstance).HasForeignKey(x => x.SurveyInstanceId);

            /// uncomment if filter on dates ever be used
            //entityTypeBuilder.HasIndex(x => x.CreatedDate);
            //entityTypeBuilder.HasIndex(x => x.EndDate);
            //entityTypeBuilder.HasIndex(x => x.BeginningDate);
        }

        protected virtual void ConfigurreOptionEntity(EntityTypeBuilder<OptionEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasOne(x => x.Question).WithMany(x => x.Options).HasForeignKey(x => x.QuestionId);
        }

        protected virtual void ConfigurreQuestionEntity(EntityTypeBuilder<QuestionEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasOne(x => x.Survey).WithMany(x => x.Questions).HasForeignKey(x => x.SurveyId);
            entityTypeBuilder.HasMany(x => x.Options).WithOne(x => x.Question).HasForeignKey(x => x.QuestionId);
        }

        protected virtual void ConfigurreSurveyEntity(EntityTypeBuilder<SurveyEntity> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.HasMany(x => x.Instances).WithOne(x => x.Survey).HasForeignKey(x => x.SurveyId);
            entityTypeBuilder.HasMany(x => x.Questions).WithOne(x => x.Survey).HasForeignKey(x => x.SurveyId);
        }

        #endregion

        #region IUnitOfWork
        public void MarkCreate(params object[] entities)
        {
            this.AddRange(entities);
        }

        public void MarkCreate(IEnumerable<object> entities)
        {
            this.AddRange(entities);
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public void MarkDelete(params object[] entities)
        {
            this.RemoveRange(entities);
        }

        public void MarkDelete(IEnumerable<object> entities)
        {
            this.RemoveRange(entities);
        }

        public void MarkUpdate(params object[] entities)
        {
            this.UpdateRange(entities);
        }

        public void MarkUpdate(IEnumerable<object> entities)
        {
            this.UpdateRange(entities);
        }
        #endregion
    }
}
