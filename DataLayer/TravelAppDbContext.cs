                                                                                                                                              using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class TravelAppDbContext : IdentityDbContext<User>
    {
        public TravelAppDbContext(): base()
        { }
        public TravelAppDbContext(DbContextOptions options) : base(options) 
        { 
        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-AUDH7G9\\SQLEXPRESS;Database=TravelApp1;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Trips)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
            modelBuilder.Entity<User>()
                .HasMany(u => u.BucketLists)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);  
            modelBuilder.Entity<User>()
                .HasMany(u => u.Stories)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);
            modelBuilder.Entity<Trip>()
                .HasMany(t => t.Countries)
                .WithMany(c => c.Trips);

            //modelBuilder.Entity<Trip>()
            //    .HasOne(t => t.StartingPlace)
            //    .WithOne()
            //    .HasForeignKey<Trip>(t => t.StartingPlaceId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Trip>()
            //    .HasOne(t => t.EndingPlace)
            //    .WithMany()
            //    .HasForeignKey(t => t.EndingPlaceId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasMany(t => t.Places)
                .WithOne(p => p.Trip)
                .HasForeignKey(p => p.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<BucketList> BucketLists { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Country> Countries{ get; set; }
    }
}
