using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=366; Integrated Security=True;");
        }
        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerStatistic>(ps =>
            {
                ps.HasKey(ps => new { ps.PlayerId, ps.GameId });
            });

            modelBuilder.Entity<Team>(t =>
            {
                t.HasOne(t => t.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .OnDelete(DeleteBehavior.Restrict);

                t.HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .OnDelete(DeleteBehavior.Restrict);

                t.HasOne(t => t.Town)
                .WithMany(t => t.Teams)
                .OnDelete(DeleteBehavior.Restrict);

                t.HasMany(t => t.Players)
                .WithOne(p => p.Team)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Game>(g =>
            {
                g.HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .OnDelete(DeleteBehavior.Restrict);

                g.HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .OnDelete(DeleteBehavior.Restrict);

                g.HasMany(g => g.PlayerStatistics)
                .WithOne(ps => ps.Game)
                .OnDelete(DeleteBehavior.Restrict);

                g.HasMany(g => g.Bets)
                .WithOne(g => g.Game)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bet>(b =>
            {
                b.HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(u =>
            {
                u.HasMany(u => u.Bets)
                .WithOne(bets => bets.User)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Town>(tw =>
            {
                tw.HasMany(tw => tw.Teams)
                    .WithOne(t => t.Town)
                    .OnDelete(DeleteBehavior.Restrict);

                tw.HasOne(tw => tw.Country)
                .WithMany(c => c.Towns)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Player>(p =>
            {
                p.HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .OnDelete(DeleteBehavior.Restrict);

                p.HasOne(p => p.Position)
                .WithMany(pos => pos.Players)
                .OnDelete(DeleteBehavior.Restrict);

                p.HasMany(p => p.PlayerStatistics)
                .WithOne(ps => ps.Player)
                .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
