namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        public DbSet<Song> Songs { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Performer> Performers { get; set; }

        public DbSet<SongPerformer> SongsPerformers { get; set; }

        public DbSet<Writer> Writers { get; set; }

        public DbSet<Producer> Producers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            //builder.Entity<Song>(s =>
            //{
            //    s.HasOne(s => s.Writer)
            //    .WithMany(w => w.Songs)
            //    .OnDelete(DeleteBehavior.Restrict);

            //    s.HasOne(s => s.Album)
            //     .WithMany(a => a.Songs)
            //     .OnDelete(DeleteBehavior.Restrict);
            //});


            builder.Entity<Album>(a =>
            {
                a.HasOne(a => a.Producer)
                .WithMany(p => p.Albums);
            });

            builder.Entity<SongPerformer>(sp =>
            {
                sp.HasKey(sp => new { sp.PerformerId, sp.SongId });

                //sp.HasOne(sp => sp.Song)
                //.WithMany(s => s.SongPerformers)
                //.OnDelete(DeleteBehavior.Restrict);

                //sp.HasOne(sp => sp.Performer)
                //.WithMany(p => p.PerformerSongs);
            });
        }
    }
}
