using Microsoft.EntityFrameworkCore;
using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingPartyDataContext : DbContext
    {
        public BookingPartyDataContext(DbContextOptions<BookingPartyDataContext> options) : base(options) { }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<ServiceAvailableInDay> ServiceAvailableInDay { get; set; }
        public virtual DbSet<Facility> Facility { get; set; }
        public virtual DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingDetail>()
               .HasOne(x => x.Service)
               .WithMany(x => x.BookingDetails)
               .HasForeignKey(x => x.ServiceId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingDetail>()
               .HasOne(x => x.Booking)
               .WithMany(x => x.BookingDetails)
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasOne(x => x.BookingService)
                .WithOne(x => x.Contract)
                .HasForeignKey<Contract>(x => x.BookingServiceId);

            modelBuilder.Entity<User>()
                .HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<Notification>()
               .HasOne(x => x.User)
               .WithMany(x => x.Notifications)
               .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<TransactionHistory>()
               .HasOne(x => x.Deposit)
               .WithOne(x => x.TransactionHistory)
               .HasForeignKey<TransactionHistory>(x => x.DepositId);

            modelBuilder.Entity<Booking>()
               .HasOne(x => x.User)
               .WithMany(x => x.Bookings)
               .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Service>()
              .HasOne(x => x.User)
              .WithMany(x => x.Services)
              .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<Service>()
             .HasOne(x => x.Category)
             .WithMany(x => x.Services)
             .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<Image>()
              .HasOne(x => x.Service)
              .WithMany(x => x.Images)
              .HasForeignKey(x => x.ServiceId);

            modelBuilder.Entity<Feedback>()
             .HasOne(x => x.Service)
             .WithMany(x => x.Feedbacks)
             .HasForeignKey(x => x.ServiceId);

            modelBuilder.Entity<Booking>()
             .HasOne(x => x.User)
             .WithMany(x => x.Bookings)
             .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Promotion>()
            .HasOne(x => x.Service)
            .WithMany(x => x.Promotions)
            .HasForeignKey(x => x.ServiceId);

            modelBuilder.Entity<Deposit>()
            .HasOne(x => x.Booking)
            .WithMany(x => x.Deposits)
            .HasForeignKey(x => x.BookingId);

            modelBuilder.Entity<Feedback>()
           .HasOne(x => x.User)
           .WithMany(x => x.Feedbacks)
           .HasForeignKey(x => x.UserId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingDetail>()
           .HasOne(x => x.Room)
           .WithMany(x => x.BookingDetails)
           .HasForeignKey(x => x.RoomId);

            modelBuilder.Entity<Room>()
           .HasOne(x => x.User)
           .WithMany(x => x.Rooms)
           .HasForeignKey(x => x.UserId);

           modelBuilder.Entity<Feedback>()
          .HasOne(x => x.Room)
          .WithMany(x => x.Feedbacks)
          .HasForeignKey(x => x.RoomId)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Promotion>()
          .HasOne(x => x.Room)
          .WithMany(x => x.Promotions)
          .HasForeignKey(x => x.RoomId)
          .OnDelete(DeleteBehavior.Restrict);

           modelBuilder.Entity<Image>()
          .HasOne(x => x.Room)
          .WithMany(x => x.Images)
          .HasForeignKey(x => x.RoomId)
          .OnDelete(DeleteBehavior.Restrict);
           
           modelBuilder.Entity<Facility>()
               .HasOne(x => x.Room)
               .WithMany(x => x.Facilities)
               .HasForeignKey(x => x.RoomId);
           modelBuilder.Entity<ServiceAvailableInDay>()
               .HasOne(x => x.Service)
               .WithMany(x => x.ServiceAvailableInDays)
               .HasForeignKey(x => x.ServiceId);
           modelBuilder.Entity<ServiceAvailableInDay>()
               .Property(x => x.NumberOfAvailableInDay)
               .HasDefaultValue(5);
           modelBuilder.Entity<BookingDetail>()
               .Property(x => x.ServiceId)
               .IsRequired(false);
           modelBuilder.Entity<Image>()
               .Property(x => x.ServiceId)
               .IsRequired(false);
           modelBuilder.Entity<Image>()
               .Property(x => x.RoomId)
               .IsRequired(false);
           modelBuilder.Entity<Feedback>()
               .Property(x => x.RoomId)
               .IsRequired(false);
           modelBuilder.Entity<Feedback>()
               .Property(x => x.ServiceId)
               .IsRequired(false);
           modelBuilder.Entity<WithdrawalRequest>()
               .HasOne(x => x.User)
               .WithMany(x => x.WithdrawalRequests)
               .HasForeignKey(x => x.UserId);
        }
    }
}
