namespace MusicHub
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);
            //context.Database.EnsureCreated();
            //context.Database.EnsureDeleted();
            Console.WriteLine(ExportAlbumsInfo(context, 9));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var result = context
                .Albums
                .ToArray()
                .Where(a => a.ProducerId == producerId)
                .OrderByDescending(a => a.Price)
                .Select(
                          a => new
                          {
                              AlbumName = a.Name,
                              ReleaseDate = a.ReleaseDate.ToString(@"MM\/dd\/yyyy", CultureInfo.InvariantCulture),
                              producerName = a.Producer.Name,
                              Songs = a.Songs
                              .ToArray()
                                .Select(x => new
                                {
                                    SongName = x.Name,
                                    SongPrice = x.Price.ToString("f2"),
                                    WriterName = x.Writer.Name
                                })
                                .OrderByDescending(x => x.SongName)
                                .ThenBy(s => s.WriterName)
                                .ToArray(),
                              TotalAlbumPrice = a.Price.ToString("f2")
                          })
            .ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var album in result)
            {
                int count = 1;
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.producerName}");
                sb.AppendLine("-Songs:");
                foreach (var songs in album.Songs)
                {
                    sb.AppendLine($"---#{count++}");
                    sb.AppendLine($"---SongName: {songs.SongName}");
                    sb.AppendLine($"---Price: {songs.SongPrice}");
                    sb.AppendLine($"---Writer: {songs.WriterName}");
                }
                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
