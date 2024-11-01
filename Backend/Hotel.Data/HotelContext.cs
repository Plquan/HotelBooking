using Hotel.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public class HotelContext : IdentityDbContext<AppUser>
    {
        public HotelContext(DbContextOptions options) : base(options)
        {

        }  
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomService> RoomServices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<RoomFacility> RoomFacilitys { get; set; }
    }

    

}
