namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;
    using System.Reflection.Emit;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }

        public DbSet<Album> Albums{ get; set; }

        public DbSet<Performer> Performers { get; set; }

        public DbSet<Producer> Producers { get; set; }

        public DbSet<Writer> Writers { get; set; }

        public DbSet<SongPerformer> SongPerformers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SongPerformer>()
               .HasKey(x => new { x.SongId, x.PerformerId });

            modelBuilder.Entity<SongPerformer>()
                .HasOne(x => x.Song)
                .WithMany(s => s.SongPerformers)
                .HasForeignKey(x => x.SongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SongPerformer>()
                .HasOne(x => x.Performer)
                .WithMany(p => p.PerformerSongs)
                .HasForeignKey(x => x.PerformerId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
