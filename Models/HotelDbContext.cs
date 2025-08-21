using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BCrypt.Net;
using HospitalityProject.Models; // Ensure this namespace matches your models
using System; // For DateTime
using System.IO;
using System.Collections.Generic;

namespace HospitalityProject.Models
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string basePath = Path.GetFullPath(@".\Data");
            string userPath = Path.Combine(basePath, "db_users.json");
            string guestPath = Path.Combine(basePath, "db_guests.json");
            string roomPath = Path.Combine(basePath, "db_rooms.json");
            string bookingPath = Path.Combine(basePath, "db_bookings.json");

            // Seed Users
            string userData = File.ReadAllText(userPath);
            List<User> users = JsonConvert.DeserializeObject<List<User>>(userData); // Deserializes the JSON string into a list of User objects.
            foreach (var user in users)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 12); // Take the plain-text password, hash it and use it to replace the Password field
            }
            modelBuilder.Entity<User>().HasData(users);  // Seeds the Users table with the data.

            // Seed Guests
            string guestData = File.ReadAllText(guestPath);
            List<Guest> guests = JsonConvert.DeserializeObject<List<Guest>>(guestData); // Deserializes the JSON string into a list of Guest objects.
            modelBuilder.Entity<Guest>().HasData(guests); // Seeds the Guests table with the data.

            // Seed Rooms
            string roomData = File.ReadAllText(roomPath);
            List<Room> rooms = JsonConvert.DeserializeObject<List<Room>>(roomData);  // Deserializes the JSON string into a list of Room objects.
            modelBuilder.Entity<Room>().HasData(rooms); // Seeds the Rooms table with the data.

            // Seed Bookings
            string bookingData = File.ReadAllText(bookingPath);
            List<Booking> bookings = JsonConvert.DeserializeObject<List<Booking>>(bookingData);  // Deserializes the JSON string into a list of Booking objects.
            modelBuilder.Entity<Booking>().HasData(bookings); // Seeds the Bookings table with the data.

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
