using Microsoft.EntityFrameworkCore;
using System.Net;
using WebhookRESTAPI.Features.Webhooks.Models;

namespace WebhookRESTAPI.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasIndex(it => it.Id);

            modelBuilder.Entity<Event>()
                .HasIndex(it => it.CreatedAt);

            modelBuilder.Entity<Event>()
                .HasIndex(it => it.LastUpdated);
            
            modelBuilder.Entity<Event>()
                .HasIndex(it => it.Timestamp);

            modelBuilder.Entity<Subscription>()
                .HasIndex(it => it.Id);

            modelBuilder.Entity<Subscription>()
                .HasIndex(it => it.SubscriberId);

            modelBuilder.Entity<Subscription>()
                .HasIndex(it => it.CreatedAt);

            modelBuilder.Entity<Subscription>()
                .HasIndex(it => it.LastUpdated);
        }

        public DbSet<Event> Events { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
