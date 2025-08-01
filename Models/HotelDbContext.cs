//using System;
//using System.Collections.Generic;
//using System.Net.NetworkInformation;
//using Microsoft.EntityFrameworkCore;

//namespace HospitalityProject.Models;

//public partial class HotelDbContext : DbContext
//{
//    public virtual DbSet<Room> Rooms { get; set; }
//    public virtual DbSet<Booking> Bookings { get; set; }

//    public HotelDbContext()
//    {
//    }

//    public HotelDbContext(DbContextOptions<HotelDbContext> options)
//        : base(options)
//    {
//    }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=Hotel;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    { /* This method is used to configure the database schema:
//            Add constraints, relationships, default values, keys, etc.
//            EF calls this method when the model is being built. */

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}


// Data/HotelDbContext.cs (or Models/HotelDbContext.cs)
using Microsoft.EntityFrameworkCore;
using HospitalityProject.Models; // Ensure this namespace matches your models
using System; // For DateTime

namespace HospitalityProject.Models // Ensure this namespace matches your context location
{
    public partial class HotelDbContext : DbContext
    {
        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options)
        {
        }

        //This prevents it from overriding the appsettings.json connection string.
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=Hotel;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mock data for Rooms
            modelBuilder.Entity<User>().HasData(
                new User { Id= 1, Username = "admin", Password = "admin123"}
            );

            // Mock data for Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { RoomId = 1, RoomNumber = "101", RoomType = "Standard", Capacity = 2, PricePerNight = 100.00m, RoomDescription = "Cozy standard room with city views." },
                new Room { RoomId = 2, RoomNumber = "102", RoomType = "Deluxe", Capacity = 3, PricePerNight = 150.00m, RoomDescription = "Spacious deluxe room with balcony overlooking the garden." },
                new Room { RoomId = 3, RoomNumber = "201", RoomType = "Suite", Capacity = 4, PricePerNight = 250.00m, RoomDescription = "Luxurious suite with separate living area and ocean views." },
                new Room { RoomId = 4, RoomNumber = "202", RoomType = "Standard", Capacity = 2, PricePerNight = 110.00m, RoomDescription = "Comfortable room with twin beds." }
            );

            // Mock data for Guests
            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "Alice", LastName = "Smith", Email = "alice.smith@example.com", PhoneNumber = "555-111-2222" },
                new Guest { GuestId = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob.j@example.com", PhoneNumber = "555-333-4444" }
            );

            // Mock data for Bookings (ensure GuestId and RoomId match seeded data)
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = 1,
                    RoomId = 1,  // Room 101
                    GuestName = "Alice Smith",
                    Email = "c0937885@mylambton.ca",
                    CheckInDate = new DateTime(2025, 8, 1),
                    CheckOutDate = new DateTime(2025, 8, 5),
                    NumberOfGuests = 2,
                    TotalPrice = 400.00m, // 4 nights * 100
                    Status = "Confirmed"
                },
                new Booking
                {
                    BookingId = 2,
                    RoomId = 2,  // Room 102
                    GuestName = "Bon Jovi",
                    Email= "test@mylambton.ca",
                    CheckInDate = new DateTime(2025, 9, 10),
                    CheckOutDate = new DateTime(2025, 9, 12),
                    NumberOfGuests = 1,
                    TotalPrice = 300.00m, // 2 nights * 150
                    Status = "Confirmed"
                }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
